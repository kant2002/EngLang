namespace EngLang
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    public class EngLangParser
    {
        public static SyntaxNode Parse(string sourceCode)
        {
            if (sourceCode.Contains('.'))
            {
                var statmementTexts = sourceCode.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var statementsArray = statmementTexts
                    .Select(ParseNode)
                    .Select(ConvertToStatement)
                    .ToArray();
                var statements = ImmutableList.Create<Statement>(statementsArray);
                return new BlockStatement(statements);
            }

            return ParseNode(sourceCode);
        }

        private static Statement ConvertToStatement(SyntaxNode node)
        {
            return node switch
            {
                VariableDeclaration variableDeclaration => new VariableDeclarationStatement(variableDeclaration),
                _ => throw new InvalidOperationException(),
            };
        }

        private static SyntaxNode ParseNode(string sourceCode)
        {
            var parts = sourceCode.Split(' ');
            switch (parts[0])
            {
                case "a":
                case "an":
                    var variableReference = ParseIdentifierReference(string.Join(' ', parts.Skip(1)));
                    return variableReference;
                case "the":
                    var variableDeclaration = ParseVariableDeclaration(string.Join(' ', parts.Skip(1)));
                    return variableDeclaration;
                default:
                    throw new NotImplementedException();
            }
        }

        private static IdentifierReference ParseIdentifierReference(string content)
        {
            var variableName = content;
            var identifierReference = new IdentifierReference(variableName);
            return identifierReference;
        }

        private static VariableDeclaration ParseVariableDeclaration(string content)
        {
            var parts = content.Split(" is ");
            var variableName = parts[0];
            var variableSpecification = parts[1];
            var specificationParts = variableSpecification.Split(" equal to");
            var syntaxNode = Parse(specificationParts[0]);
            var identifierReference = syntaxNode as IdentifierReference;
            if (identifierReference == null)
            {
                throw new InvalidOperationException($"Identifier expected. {variableSpecification} given");
            }

            Expression? expression = null;
            if (specificationParts.Length > 1)
            {
                expression = ParseExpression(specificationParts[1].Trim());
            }

            var variableDeclaration = new VariableDeclaration(variableName, identifierReference, expression);
            return variableDeclaration;
        }

        private static Expression ParseExpression(string expressionString)
        {
            if (expressionString.StartsWith('"'))
            {
                return new StringLiteralExpression(expressionString.Trim('"'));
            }

            throw new InvalidOperationException($"Cannot parse expression '{expressionString}'");
        }
    }
}