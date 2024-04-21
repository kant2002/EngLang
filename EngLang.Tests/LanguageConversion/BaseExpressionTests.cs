using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseExpressionTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertEqualityExpression()
    {
        var sentence = "a number is 0";
        var lexer = new EngLangLexer(sentence);
        var parser = new EngLangParser(lexer);
        var parseResult = parser.ParseExpression();
        Assert.True(parseResult.IsOk);
        var expression = parseResult.Ok.Value;
        var converter = CreateConverter();

        var result = converter.Convert(expression);

        var expectedCode = GetEqualityExpression();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetEqualityExpression();

    [Fact]
    public void ConvertPosessiveStringExpression()
    {
        var sentence = "\" blabla \"'s pointer";
        var lexer = new EngLangLexer(sentence);
        var parser = new EngLangParser(lexer);
        var parseResult = parser.ParseExpression();
        Assert.True(parseResult.IsOk);
        var expression = parseResult.Ok.Value;
        var converter = CreateConverter();

        var result = converter.Convert(expression);

        var expectedCode = GetPosessiveStringExpression();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetPosessiveStringExpression();
}
