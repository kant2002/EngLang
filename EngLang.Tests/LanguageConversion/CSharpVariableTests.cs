namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpVariableTests : BaseVariableTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedVariable() => @"apple name;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedVariableWithStringLiteral() => "string greetings = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "number answer_to_all_things = 42";

    protected override string GetExpectedVariableWithDash() => @"zero_index_variable".ReplaceLineEndings(NewLine);
}
