namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpConversionTests : BaseLanguageConversionTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedVariable() => @"apple name;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedVariableWithStringLiteral() => "string greetings = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "number answer_to_all_things = 42";

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

    protected override string GetExpectedIfEqualsStatementCode() => @"if (number == 0) {
    value += 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedIfLessThanStatementCode() => @"if (number < 0) {
    value += 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedResultStatementCode() => @"return 42;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedResultInsideIfStatementCode() => @"if (number < 0) {
    return 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedLabeledStatementCode() => @"void do_something()
{
    return 1;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedLabeledStatementParametersCode() => @"void calculate_area_from(width, height)
{
    return 1;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedInvocationStatementParametersCode() => @"previous_factorial = calculate_factorial_of(previous_number);
".ReplaceLineEndings(NewLine);
}
