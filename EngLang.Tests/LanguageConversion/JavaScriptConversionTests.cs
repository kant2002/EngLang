namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class JavaScriptConversionTests : BaseLanguageConversionTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetExpectedVariable() => @"let name;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedVariableWithStringLiteral() => "let greetings = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "let answer_to_all_things = 42";

    protected override string GetExpectedAdditionCode() => "value += 42";

    protected override string GetExpectedSubtractionCode() => "value -= 42";

    protected override string GetExpectedMultiplicationCode() => "value *= 42";

    protected override string GetExpectedDivisionCode() => "value /= 42";

    protected override string GetExpectedAssignmentCode() => "value = 40";

    protected override string GetExpectedStatementsCode() => @"value = 40;
value += 42;
value -= 42;
value *= 42;
value /= 42;

".ReplaceLineEndings(NewLine);

    protected override string GetExpectedIfStatementCode() => @"if (number == 0) {
    value += 42;
}

".ReplaceLineEndings(NewLine);
}
