using Catalyst;
using Mosaik.Core;
using static System.Console;

Catalyst.Models.English.Register(); // You need to pre-register each language (and install the respective NuGet Packages)

Storage.Current = new DiskStorage("catalyst-models");
var nlp = await Pipeline.ForAsync(Language.English);

var analyzer = new TextAnalyzer(nlp);
analyzer.AnalyseText("To calculate area from a width and a height: multiply a width by a height.");
analyzer.AnalyseText("the console is a console. the name is an string.");
analyzer.AnalyseText("the red vibrating console is a console. the name is an string.");
analyzer.AnalyseText("the my vibrating console is a console. the name is an string.");
analyzer.AnalyseText("the shiny red vibrating console is a console. the name is an string.");
analyzer.AnalyseText("the my console is a console. the name is an string.");
analyzer.AnalyseText("add 42 to a value");
analyzer.AnalyseText("add two to a value");
analyzer.AnalyseText("add forty two to a value");
analyzer.AnalyseText("if a number is 0 then add 42 to a value.");
analyzer.AnalyseText("add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ");


class TextAnalyzer
{
    Document doc;
    Pipeline nlp;
    public TextAnalyzer(Pipeline pipeline)
    {
        this.nlp = pipeline;
    }

    public void AnalyseText(string text)
    {
        doc = new Document(text, Language.English);
        nlp.ProcessSingle(doc);
        foreach (var sentence in doc.TokensData)
        {
            //WriteLine("========== Sentence =============");
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
