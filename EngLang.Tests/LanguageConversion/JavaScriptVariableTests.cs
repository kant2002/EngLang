namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class JavaScriptVariableTests : BaseVariableTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetExpectedVariable() => @"let name;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedVariableWithStringType() => "let greetings = \"Hello\"";

    protected override string GetExpectedVariableWithStringName() => "let string = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "let answer = 42";

    protected override string GetExpectedVariableWithDash() => @"zero_index_variable".ReplaceLineEndings(NewLine);
}
