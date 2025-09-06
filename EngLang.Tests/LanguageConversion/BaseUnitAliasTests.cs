using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseUnitAliasTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertAliasDeclaration()
    {
        var sentence = "The million is 1000 thousands.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetAliasDeclaration();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetAliasDeclaration();
}
