using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompiladorChiwis
{
    internal class Lexer
    {
        private static readonly Dictionary<string, string> TokenPatterns = new()
        {
            { "MainMethod", @"^\s*main\s*\(\s*\)\s*\*?" },
            { "OpenBrace", @"\{" },
            { "CloseBrace", @"\}" },
            { "VariableEnt", @"\s*ent\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*-?\d+(\s*[\+\-\*/%]\s*-?\d+)*\s*)?;"},//Valida operaciones
            { "DeclarationFlot",@"\s*flot\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*-?\d+(\.\d+)?(\s*[\+\-\*/%]\s*-?\d+(\.\d+)?)*)?;"},
            { "DeclarationCad", @"\s*cad\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*"".*""\s*)?;"},//Ya valida
            { "DeclarationSn", @"\s*sn\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*True|False\s*)?;"},//Validado
            { "DeclarationLet", @"\s*let\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*"".""\s*)?;"},//Validado
            { "ListDeclaration", @"\s*list\s+(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*\[\s*(\d+|(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*)?\s*\]\s*(=\s*\{\s*(((\""[^\""]*\"")|(\d+(\.\d+)?|[a-zA-Z_][a-zA-Z0-9_]*))(,\s*((\""[^\""]*\"")|(\d+(\.\d+)?|[a-zA-Z_][a-zA-Z0-9_]*)))*\s*)?\}\s*)?;" },
            { "VariableAssignment", @"\s*(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\s*(=\s*(-?\d+(\.\d+)?(\s*[\+\-\*/%]\s*-?\d+(\.\d+)?)*)|((\+=|-=|\*=|/=|%=)\s*(-?\d+(\.\d+)?|(?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*)))\s*;" },
            { "Input", @"\s*kiara\.entrada\((?!ent\b|cad\b|flot\b|sn\b|let\b|para\b|Si\b|No\b|True\b|False\b|kiara\b|list\b)[a-zA-Z_][a-zA-Z0-9_]*\)\s*;" },
            { "Output", @"\s*kiara\.salida\([^\)]+\)\s*;" },
            { "IfStatement", @"\s*Si\s*\((.*)\)\s*" },
            { "ElseStatement", @"\s*No\s*" },
            { "ForLoop", @"para\s*\((.*)\)\s*"},
            { "Comment", @"\~.*\~" },
            { "Whitespace", @"\s+" }
        };

    

        public List<(string TokenType, string Value)> Tokenize(string code)
        {
            var tokens = new List<(string, string)>();
            int index = 0;

            while (index < code.Length)
            {
                bool matched = false;

                foreach (var (type, pattern) in TokenPatterns)
                {
                    var match = Regex.Match(code.Substring(index), pattern);

                    if (match.Success && match.Index == 0)
                    {
                        if (type != "Whitespace") // Ignorar espacios
                        {
                            tokens.Add((type, match.Value.Trim()));
                            MessageBox.Show($"Token encontrado: {type} - {match.Value}");
                        }

                        index += match.Length;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    throw new Exception($"Error léxico en posición {index}: {code.Substring(index, Math.Min(20, code.Length - index))}");

            }

            return tokens;
        }

    }
}
