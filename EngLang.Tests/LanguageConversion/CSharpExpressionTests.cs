namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpExpressionTests : BaseExpressionTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetEqualityExpression() => @"number == 0".ReplaceLineEndings(NewLine);

    protected override string GetPosessiveStringExpression() => @""" blabla "".pointer".ReplaceLineEndings(NewLine);
}
