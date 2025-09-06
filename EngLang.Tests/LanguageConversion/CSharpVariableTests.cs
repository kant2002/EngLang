namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpVariableTests : BaseVariableTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedVariable() => @"apple name;
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedVariableWithStringType() => "string greetings = \"Hello\"";

    protected override string GetExpectedVariableWithStringName() => "charseq @string = \"Hello\"";

    protected override string GetExpectedVariableWithIntLiteral() => "number answer = 42";

    protected override string GetExpectedVariableWithDash() => @"zero_index_variable".ReplaceLineEndings(NewLine);
}
