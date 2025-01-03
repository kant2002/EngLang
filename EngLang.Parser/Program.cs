using Catalyst;
using EngLang;
using Mosaik.Core;
using static System.Console;
using System.Text.RegularExpressions;
using Yoakke.SynKit.Text;
using System.Xml.Serialization;


#if SPACY
using NlpPipeline = Catalyst.Spacy.Pipeline;
#else
using NlpPipeline = Catalyst.Pipeline;
#endif

using EngLangToken = Yoakke.SynKit.Lexer.IToken<EngLang.EngLangTokenType>;

var samples = new[]
{
    "To calculate area from a width and a height: multiply a width by a height.",
    "the console is a console. the name is an string.",
    "the red vibrating console is a console. the name is an string.",
    "the my vibrating console is a console. the name is an string.",
    "the shiny red vibrating console is a console. the name is an string.",
    "the my console is a console. the name is an string.",
    "add 42 to a value",
    "add two to a value",
    "add forty two to a value",
    "if a number is 0 then add 42 to a value.",
    "add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ",
};

var parsedArgs = CommandLineArguments.Parse(args);
if (!parsedArgs.IsSuccess)
{
    CommandLineArguments.PrintHelp();
}
else
{
    var result = parsedArgs.Result!;
    var linesFile = result.LinesFile;
    if (linesFile is not null)
    {
        var writer = Out;
        await DumpLines(linesFile, writer);
    }

    var processingDirectory = result.ProcessingDirectory;
    if (processingDirectory is not null)
    {
        if (result.OutputDirectory is null)
        {
            Error.WriteLine("The output directory is missing");
            return;
        }

        Directory.CreateDirectory(result.OutputDirectory);
        int success = 0;
        int totalFiles = 0;
        foreach (var file in Directory.GetFiles(processingDirectory, "*.", result.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
            var baseFile = Path.GetRelativePath(processingDirectory, file) + ".lines";
            var outputFile = Path.Combine(result.OutputDirectory, baseFile);
            var currentOutputDirectory = Path.GetDirectoryName(outputFile);
            if (currentOutputDirectory is not null)
            {
                Directory.CreateDirectory(currentOutputDirectory);
            }

            try
            {
                using var writer = new StreamWriter(File.OpenWrite(outputFile));
                await DumpLines(file, writer);
                success++;
            }
            catch (Exception ex)
            {
                await Error.WriteLineAsync($"During processing of file '{baseFile}' error happens: {ex.Message}");
            }

            totalFiles++;
        }

        await Out.WriteLineAsync($"Total files {totalFiles} processed. Successul: {success}");
    }

    var nlpFile = parsedArgs.Result!.NlpFile;
    if (nlpFile is not null)
    {
        var lines = File.ReadAllLines(nlpFile);
        var data = lines.Aggregate(new List<List<string>>() { new List<string>() }, (prev, currentItem) =>
        {
            if (currentItem == "")
            {
                prev.Add(new List<string>());
                return prev;
            }

            prev[prev.Count - 1].Add(currentItem);
            return prev;
        });
        //var data = lines.Chunk(4).ToList();
        foreach (var item in data)
        {
            ValidateSentence(sentence: item[0], spacy: item[1], stanza: item.Last(), Console.Out, new StringWriter());
        }
    }

    if (linesFile is null && processingDirectory is null && nlpFile is null)
    {
        await ProcessSpacy(samples);
    }
}

static async Task DumpLines(string linesFile, TextWriter writer)
{
    var lines = await CollectSentences(linesFile);
    foreach (var line in lines)
    {
        await writer.WriteLineAsync(line);
    }
}

void ValidateSentence(string sentence, string spacy, string stanza, TextWriter success, TextWriter error)
{
    var spacyParts = SplitNlpText(spacy);
    var stanzaParts = SplitNlpText(stanza);
    if (spacyParts.Length != stanzaParts.Length)
    {
        error.WriteLine($"Sentence '{sentence}' has different block length");
        error.WriteLine(spacy);
        error.WriteLine(stanza);
    }
    else
    {
        bool isInvalid = spacyParts.Length != stanzaParts.Length;
        for (int i = 0; i < spacyParts.Length; i++)
        {
            var spacyPart = spacyParts[i];
            var stanzaPart = stanzaParts[i];
            if (spacyPart.Trim() != stanzaPart.Trim())
            {
                isInvalid = true;
                error.WriteLine($"Different parts '{spacyPart}' and '{stanzaPart}' in sentence '{sentence}'");
                break;
            }
        }

        if (!isInvalid)
        {
            success.WriteLine(sentence.Trim());
        }
    }

    string[] SplitNlpText(string text)
    {
        var parts = text.Split(") (");
        return parts;
    }
}

static async Task<string[]> CollectSentences(string fileName)
{
    var text = await File.ReadAllTextAsync(fileName);
    var collector = new SentenceCollector(text, fileName)
    {
        CollectMarkers = true,
        CollectSentences = true,
    };
    return collector.Collect();
}

async Task ProcessSpacy(string[] samples)
{
#if SPACY
    using var pythonLock = await Spacy.Initialize(Spacy.ModelSize.Large, Language.English);
    var nlp = Spacy.For(Spacy.ModelSize.Large, Language.English);
#else
    Catalyst.Models.English.Register(); // You need to pre-register each language (and install the respective NuGet Packages)

    Storage.Current = new DiskStorage("catalyst-models");
    var nlp = await Pipeline.ForAsync(Language.English);
#endif
    var analyzer = new TextAnalyzer(nlp);
    foreach (var sample in samples)
    {
        analyzer.AnalyseText(sample);
    }
}

class SentenceCollector
{
    private readonly string text;
    private readonly string fileName;
    private readonly int[] linePositions;

    public SentenceCollector(string text, string fileName)
    {
        this.text = text;
        this.fileName = fileName;
        this.linePositions = new[] { 0 }.Union(Regex.Matches(text, "(\r\n|\n|\r)").OfType<Match>().Select(_ => _.Index + _.ValueSpan.Length)).ToArray();
    }

    public required bool CollectMarkers { get; set; }

    public required bool CollectSentences { get; set; }

    public string[] Collect()
    {
        var parser = (ParagraphList)EngLangParser.Parse(text, this.fileName);
        var paragraphWithLabels = parser.Paragraphs.Where(p => p.Label is not null);
        var markers = paragraphWithLabels.SelectMany(CollectSentencesFromParagraph);
        return markers.OrderBy(_ => _).Distinct().ToArray();
    }

    IEnumerable<string> CollectSentencesFromParagraph(Paragraph p)
    {
        if (CollectMarkers)
        {
            yield return GetText(p.Label!.Range);
        }

        if (CollectSentences)
        {
            foreach (var statement in p.Statements.SelectMany(CollectSentencesFromStatement))
                yield return statement;
        }
    }

    IEnumerable<string> CollectSentencesFromStatement(Statement s)
    {
        switch (s)
        {
            case BlockStatement blockStatement:
                {
                    foreach (var statement in blockStatement.Statements.SelectMany(CollectSentencesFromStatement))
                        yield return statement;
                }
                break;
            case InvocationStatement invocationStatement:
                {
                    yield return GetText(invocationStatement.Range);
                }
                break;
            case InvalidStatement invalidStatement:
                yield return GetText(invalidStatement.Range);
                break;
            default:
                yield return GetText(s.Range);
                break;
        }
    }

    string GetText(EngLangToken token) => GetText(token.Range);

    string GetText(Yoakke.SynKit.Text.Range range) => text[GetPosition(range.Start)..GetPosition(range.End)];

    string GetText(EngLangToken start, EngLangToken end)
    {
        System.Range range = GetPosition(start.Location.Range.Start)..GetPosition(end.Location.Range.End);
        return text[range];
    }

    int GetPosition(Position pos) => this.linePositions[pos.Line] + pos.Column;
}

class TextAnalyzer
{
    Document doc;
    NlpPipeline nlp;
    public TextAnalyzer(NlpPipeline pipeline) => this.nlp = pipeline;

    public void AnalyseText(string text)
    {
        WriteLine($"========== {text} =============");
        doc = new Document(text, Language.English);
        nlp.ProcessSingle(doc);
        foreach (var sentence in doc.TokensData)
        {
            PrintSentence(sentence);
        }
    }

    void PrintSentence(List<TokenData> tokens)
    {
        foreach (var word in tokens)
        {
            PrintWord(word);
        }

        WriteLine();
    }

    void PrintWord(TokenData tokenData)
    {
        //WriteLine($"{tokenData.Tag} {Word(tokenData)}");
        Write($"{tokenData.Tag}({Word(tokenData)}) ");
    }

    ReadOnlySpan<char> Word(TokenData tokenData)
    {
        return doc.Value.AsSpan().Slice(tokenData.LowerBound, 1 + tokenData.UpperBound - tokenData.LowerBound);
    }
}

// CommandLine attributes are very good API, I would like stick with it.
partial class CommandLineArguments
{
    [CommandLine.Value(0)]
    public string? LinesFile { get; set; }

    [CommandLine.Option("nlp")]
    public string? NlpFile { get; set; }

    [CommandLine.Option("directory")]
    public string? ProcessingDirectory { get; set; }

    [CommandLine.Option('r')]
    public bool Recursive { get; set; }

    [CommandLine.Option("out")]
    public string? OutputDirectory { get; set; }
}

file partial class CommandLineArguments
{
    public static ParseResult Parse(string[] args)
    {
        var result = new CommandLineArguments();
        var success = true;
        var _LinesFileParsed = false;
        var _LinesFileHasError = false;
        var _NlpFileParsed = false;
        var _NlpFileHasError = false;
        var _ProcessingDirectoryParsed = false;
        var _ProcessingDirectoryHasError = false;
        var _OutputDirectoryParsed = false;
        var _OutputDirectoryHasError = false;
        var _RecursiveParsed = false;
        var _RecursiveHasError = false;
        int position = 0;
        string? currentElement;

        currentElement = args.ElementAtOrDefault(position);
        while (currentElement is not null && currentElement.StartsWith("-"))
        {
            if (currentElement == "--nlp" /*long name*/)
            {
                position++;
                currentElement = args.ElementAtOrDefault(position);
                if (currentElement is null || currentElement.StartsWith("-"))
                {
                    _NlpFileHasError = true;
                    _NlpFileParsed = true;
                }
                else
                {
                    result.NlpFile = currentElement;
                    _NlpFileParsed = true;
                    position++;
                    currentElement = args.ElementAtOrDefault(position);
                }

                continue;
            }

            if (currentElement == "--directory" /*long name*/)
            {
                position++;
                currentElement = args.ElementAtOrDefault(position);
                if (currentElement is null || currentElement.StartsWith("-"))
                {
                    _ProcessingDirectoryHasError = true;
                    _ProcessingDirectoryParsed = true;
                }
                else
                {
                    result.ProcessingDirectory = currentElement;
                    _ProcessingDirectoryParsed = true;
                    position++;
                    currentElement = args.ElementAtOrDefault(position);
                }

                continue;
            }

            if (currentElement == "--out" /*long name*/)
            {
                position++;
                currentElement = args.ElementAtOrDefault(position);
                if (currentElement is null || currentElement.StartsWith("-"))
                {
                    _OutputDirectoryHasError = true;
                    _OutputDirectoryParsed = true;
                }
                else
                {
                    result.OutputDirectory = currentElement;
                    _OutputDirectoryParsed = true;
                    position++;
                    currentElement = args.ElementAtOrDefault(position);
                }

                continue;
            }

            if (currentElement == "-r" /*short*/)
            {
                result.Recursive = true;
                _RecursiveParsed = true;
                position++;
                currentElement = args.ElementAtOrDefault(position);

                continue;
            }

            // Unknown parameter.
            break;
        }

        currentElement = args.ElementAtOrDefault(position + 0);
        if (currentElement != null)
        {
            result.LinesFile = currentElement;
            _LinesFileParsed = true;
        }

        // Put validation here.
        success = !_NlpFileHasError
            && !_ProcessingDirectoryHasError
            && !_OutputDirectoryHasError
            && !_RecursiveHasError
            && !_LinesFileHasError;
        return new(result, success);
    }

    public static void PrintHelp()
    {
        Console.WriteLine("EngLang.Parser");
    }

    public record ParseResult(CommandLineArguments? Result, bool IsSuccess);
}
