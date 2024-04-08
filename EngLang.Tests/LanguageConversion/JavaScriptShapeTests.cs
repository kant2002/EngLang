namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class JavaScriptShapeTests : BaseShapeTests
{
    protected override ILanguageConverter CreateConverter() => new JavaScriptConverter();

    protected override string GetExpectedShapeDeclarationStatementCode() => @"class apple extends fruit {
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedShapeDeclarationStatementWithSlotsCode() => @"class rectangle extends shape {
    width;
    height;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedSimpleShapeDeclarationStatementWithSlotsCode() => @"class rectangle {
    width;
    height;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedEncodeLongIndentifierCode() => @"class rectangle {
    width;
    height;
    fill_colour;
}
".ReplaceLineEndings(NewLine);
}
