using Catalyst;
using Mosaik.Core;
using static System.Console;

Catalyst.Models.English.Register(); // You need to pre-register each language (and install the respective NuGet Packages)

Storage.Current = new DiskStorage("catalyst-models");
var nlp = await Pipeline.ForAsync(Language.English);
var doc = new Document("the console is a console. the name is an string.", Language.English);
nlp.ProcessSingle(doc);
foreach (var sentence in doc.TokensData)
{
    WriteLine("========== Sentence =============");
    PrintSentence(sentence);
}

void PrintSentence(List<TokenData> tokens)
{
    foreach (var word in tokens)
    {
        PrintWord(word);
    }
}

void PrintWord(TokenData tokens)
{
    WriteLine($"{tokens.Tag} {Word(tokens)}");
}

ReadOnlySpan<char> Word(TokenData tokens)
{
    return doc.Value.AsSpan().Slice(tokens.LowerBound, 1 + tokens.UpperBound - tokens.LowerBound);
}