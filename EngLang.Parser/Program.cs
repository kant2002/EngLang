using Catalyst;
using EngLang;
using Mosaik.Core;
using static System.Console;
#if SPACY
using NlpPipeline = Catalyst.Spacy.Pipeline;
#else
using NlpPipeline = Catalyst.Pipeline;
#endif

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

if (args.Length > 0)
{
    var lines = await CollectSentences(args[0]);
    foreach (var line in lines)
    {
        await Out.WriteLineAsync(line);
    }
}
else
{
    await ProcessSpacy(samples);
}

async Task<string[]> CollectSentences(string fileName)
{
    var text = await File.ReadAllTextAsync(args[0]);
    var parser = (ParagraphList)EngLangParser.Parse(text);
    var markers = parser.Paragraphs.Where(p => p.Label is not null).SelectMany(p => p.Label!.Markers);
    return markers.ToArray();
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
