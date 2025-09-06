using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class JavaScriptConstantsTests : BaseConstantsTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetIntegerConstantDeclaration() => @"const hundred = 100;
".ReplaceLineEndings(NewLine);

    protected override string GetStringConstantDeclaration() => @"const home_town = ""Kharkiv"";
".ReplaceLineEndings(NewLine);

    protected override string GetByteConstantDeclaration() => @"let byte = 65;
".ReplaceLineEndings(NewLine);
}
