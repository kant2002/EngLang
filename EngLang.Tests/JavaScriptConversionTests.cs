namespace EngLang.Tests;

public class JavaScriptConversionTests : BaseLanguageConversionTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetExpectedVariable() => "let name;\r\n";

    protected override string GetExpectedVariableWithStringLiteral() => "let greetings = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "let answer_to_all_things = 42";

    protected override string GetExpectedAdditionCode() => "value += 42";

    protected override string GetExpectedSubstractionCode() => "value -= 42";

    protected override string GetExpectedMultiplicationCode() => "value *= 42";

    protected override string GetExpectedDivisionCode() => "value /= 42";

    protected override string GetExpectedAssignmentCode() => "value = 40";
}
