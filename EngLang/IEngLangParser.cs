using Yoakke.SynKit.Parser;

namespace EngLang;

public interface IEngLangParser
{
    ParseResult<EngLang.ParagraphList> ParseParagraphList();
    ParseResult<EngLang.Paragraph> ParseParagraph();
    ParseResult<EngLang.BlockStatement> ParseBlockStatement();
    ParseResult<EngLang.BlockStatement> ParseStatementList();
    ParseResult<EngLang.LabeledStatement> ParseLabeledStatement();
    ParseResult<EngLang.LabeledStatement> ParseLabeledStatementSimple();
}
