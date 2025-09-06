using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class CSharpConstantsTests : BaseConstantsTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetIntegerConstantDeclaration() => @"public partial class constants
{
    public const long hundred = 100;
}
".ReplaceLineEndings(NewLine);

    protected override string GetStringConstantDeclaration() => @"public partial class constants
{
    public const string home_town = ""Kharkiv"";
}
".ReplaceLineEndings(NewLine);

    protected override string GetByteConstantDeclaration() => @"byte @byte = 65;
".ReplaceLineEndings(NewLine);
}
