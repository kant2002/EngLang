using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class JavaScriptUnitAliasTests : BaseUnitAliasTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetAliasDeclaration() => @"const million = 1000 * thousand;
".ReplaceLineEndings(NewLine);
}
