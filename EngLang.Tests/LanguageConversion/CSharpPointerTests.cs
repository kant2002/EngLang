using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class CSharpPointerTests : BasePointerTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetPointerTypeDeclaration() => @"public class data_pointer(data data); // pointer
".ReplaceLineEndings(NewLine);
}
