using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Yoakke.SynKit.Parser;

namespace EngLang;

public class StupidEngLangParser : IEngLangParser
{
    private readonly string sourceCode;

    public StupidEngLangParser(string? sourceCode)
    {
        this.sourceCode = sourceCode ?? "";
    }

    public ParseResult<BlockStatement> ParseBlockStatement()
    {
        return ParseResult.Error("Blockstatement", "ParagraphList", 1, "");
    }

    public ParseResult<LabeledStatement> ParseLabeledStatement()
    {
        return ParseResult.Error("Blockstatement", "ParagraphList", 1, "");
    }

    public ParseResult<LabeledStatement> ParseLabeledStatementSimple()
    {
        return ParseResult.Error("Blockstatement", "ParagraphList", 1, "");
    }

    public ParseResult<Paragraph> ParseParagraph()
    {
        return ParseParagraph(sourceCode);
    }

    private ParseResult<Paragraph> ParseParagraph(string text)
    {
        text = RemoveCommentsComments(text);
        if (text.StartsWith("the ") || text.StartsWith("The "))
        {
            var code = string.Join("\r\n", text.Split("\r\n").Where(_ => !_.Trim().StartsWith("\\"))).Trim().TrimEnd('.');
            var variableName = code.Replace("the ", "");
            var parts = variableName.Split(" is ", 2);
            if (parts.Length == 1)
            {
                parts = variableName.Split(" are ", 2);
            }

            variableName = parts[0];
            var typeName = parts.ElementAtOrDefault(1);
            string? defaultValue = null;
            if (typeName is not null)
            {
                parts = typeName.Replace("a ", "").Replace("an ", "").Split(" equal to ", 2);
                typeName = parts[0];
                defaultValue = parts.ElementAtOrDefault(1);
            }

            var defaultValueExpression = defaultValue is null ? null
                : defaultValue.StartsWith("\"") ? (Expression?)new StringLiteralExpression(defaultValue)
                : defaultValue.StartsWith("$") ? (Expression?)new StringLiteralExpression(defaultValue)
                : new IntLiteralExpression(int.Parse(defaultValue));
            Statement declStat = new VariableDeclarationStatement(new VariableDeclaration(variableName, new TypeIdentifierReference(typeName, false), defaultValueExpression));
            var para = new Paragraph(new[] { declStat }.ToImmutableList(), null);
            return ParseResult.Ok(para, 0);
        }

        if (text.StartsWith("an ") || text.StartsWith("An ") || text.StartsWith("a ") || text.StartsWith("A ") || text.StartsWith("some ") || text.StartsWith("Some "))
        {
            var code = text.Trim().TrimEnd('.');
            var parts = code.Split(" is ", 2);
            if (parts.Length == 1)
            {
                parts = code.Split(" has ", 2); ;
                var typeName = Regex.Replace(parts[0], @"^an?\s", "");
                Statement shapeDeclStat = new ShapeDeclarationStatement(new ShapeDeclaration(typeName, null));
                var para = new Paragraph(new[] { shapeDeclStat }.ToImmutableList(), null);
                return ParseResult.Ok(para, 0);
            }
            else
            {
                var typeName = Regex.Replace(parts[0], @"^an?\s", "");
                var basePart = Regex.Replace(parts[1], @"^an?\s", "");
                Statement shapeDeclStat = new ShapeDeclarationStatement(new ShapeDeclaration(typeName, new TypeIdentifierReference(basePart, false)));
                var para = new Paragraph(new[] { shapeDeclStat }.ToImmutableList(), null);
                return ParseResult.Ok(para, 0);
            }

        }

        if (text.StartsWith("to ") || text.StartsWith("To "))
        {
            var parts = text.Split(":", 2);
            var label = parts[0];
            var code = parts[1];
            var stmts = code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(_ => new InvocationStatement(_, Array.Empty<IdentifierReference>(), null));
            var labelledStatement = new InvokableLabel(label, Array.Empty<IdentifierReference>(), null);
            var para = new Paragraph(stmts.ToImmutableList<Statement>(), labelledStatement);
            return ParseResult.Ok(para, 0);
        }

        //return ParseResult.Ok(new InvalidStatement(), 0);
        return ParseResult.Error("Blockstatement", "ParagraphList", 1, text);

        string StripComments(string text)
        {
            var commentStartPosition = text.IndexOf("\\");
            if (commentStartPosition == -1) return text;
            return text[0..commentStartPosition];
        }
    }

    string RemoveCommentsComments(string text)
    {
        var lines = string.Join("\r\n", text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Select(StripComments)).Trim();
        return lines;

        string StripComments(string text)
        {
            var commentStartPosition = text.IndexOf("\\");
            if (commentStartPosition == -1) return text;
            return text[0..commentStartPosition];
        }
    }

    public ParseResult<ParagraphList> ParseParagraphList()
    {
        var paragraphs = sourceCode.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        List<Paragraph> paragraphsList = new();
        foreach (var paragraph in paragraphs)
        {
            var paragraphResult = ParseParagraph(paragraph);
            if (paragraphResult.IsError)
            {
                if (paragraphResult.Error.Elements.ContainsKey("")) continue;
                return new ParseResult<ParagraphList>(paragraphResult.Error);
            }
            paragraphsList.Add(paragraphResult.Ok.Value);
        }

        return ParseResult.Ok(new ParagraphList(paragraphsList.ToImmutableList()), 0);
    }

    public ParseResult<BlockStatement> ParseStatementList()
    {
        return ParseResult.Error("Blockstatement", "ParagraphList", 1, "");
    }
}
