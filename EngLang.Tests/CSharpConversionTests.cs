namespace EngLang.Tests;

public class CSharpConversionTests : BaseLanguageConversionTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedVariable() => "apple name;\r\n";

    protected override string GetExpectedVariableWithStringLiteral() => "string greetings = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "number answer_to_all_things = 42";

    protected override string GetExpectedAdditionCode() => "value += 42";

    protected override string GetExpectedSubstractionCode() => "value -= 42";

    protected override string GetExpectedMultiplicationCode() => "value *= 42";

    protected override string GetExpectedDivisionCode() => "value /= 42";

    protected override string GetExpectedAssignmentCode() => "value = 40";
}
