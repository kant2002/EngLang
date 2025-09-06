using EngLang.LanguageConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BasePointerTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertPointerTypeDeclaration()
    {
        var sentence = "A data pointer is a pointer to a data.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetPointerTypeDeclaration();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetPointerTypeDeclaration();
}
