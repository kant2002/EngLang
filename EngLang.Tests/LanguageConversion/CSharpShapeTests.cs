namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using static System.Environment;

public class CSharpShapeTests : BaseShapeTests
{
    protected override ILanguageConverter CreateConverter() => new CSharpConverter();

    protected override string GetExpectedShapeDeclarationStatementCode() => @"public class apple : fruit
{
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedShapeDeclarationStatementWithSlotsCode() => @"public class rectangle : shape
{
    public object width;
    public object height;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedSimpleShapeDeclarationStatementWithSlotsCode() => @"public class rectangle
{
    public object width;
    public object height;
}
".ReplaceLineEndings(NewLine);

    protected override string GetExpectedEncodeLongIndentifierCode() => @"public class rectangle
{
    public object width;
    public object height;
    public object fill_colour;
}
".ReplaceLineEndings(NewLine);
}
