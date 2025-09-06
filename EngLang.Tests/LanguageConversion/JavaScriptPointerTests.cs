using EngLang.LanguageConversion;
using static System.Environment;

namespace EngLang.Tests.LanguageConversion;

public class JavaScriptPointerTests : BasePointerTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetPointerTypeDeclaration() => @"public class data_pointer // pointer
{
    constructor(data)
    {
        this.data = data;
    }
}
".ReplaceLineEndings(NewLine);
}
