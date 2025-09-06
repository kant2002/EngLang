using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseConstantsTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertIntegerConstantDeclaration()
    {
        var sentence = "The hundred is 100.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetIntegerConstantDeclaration();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetIntegerConstantDeclaration();

    [Fact]
    public void ConvertStringConstantDeclaration()
    {
        var sentence = "The home town is \"Kharkiv\".";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetStringConstantDeclaration();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetStringConstantDeclaration();
}
