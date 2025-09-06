namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpIfTests : BaseIfTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedIfEqualsStatementCode() => @"if (number == 0) {
    value += 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedIfLessThanStatementCode() => @"if (number < 0) {
    value += 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedIfMultipleThenStatementCode() => @"if (number == 0) {
    value += 42;
    exit();
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedResultInsideIfStatementCode() => @"if (number < 0) {
    return 42;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedIfWithFlagSetStatement() => @"if (is_set(flag)) {
    return 42;
}
".ReplaceLineEndings(NewLine);
}
