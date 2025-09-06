using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class CSharpUnitAliasTests : BaseUnitAliasTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetAliasDeclaration() => @"public partial class constants
{
    public const long million = 1000 * thousand;
}
".ReplaceLineEndings(NewLine);
}
