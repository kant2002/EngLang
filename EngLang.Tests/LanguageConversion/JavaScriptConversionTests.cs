namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class JavaScriptConversionTests : BaseLanguageConversionTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

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

    protected override string GetExpectedResultStatementCode() => @"return 42;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedLabeledStatementCode() => @"function do_something() {
    return 1;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedLabeledStatementParametersCode() => @"function calculate_area_from_and(width, height) {
    return 1;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedInvocationStatementParametersCode() => @"calculate_factorial_of_into(previous_number, previous_factorial);
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedObjectPropertiesAccess() => @"rectangle.width *= rectangle.height;
".ReplaceLineEndings(NewLine);
}
