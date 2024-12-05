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
            { "VariableDeclaration", @"\s*(ent|cad|flot|sn|let)\s+[a-zA-Z_][a-zAZ0-9_]*\s*(=\s*[^;]+)?;" },
            { "ListDeclaration", @"\s*list\s+[a-zA-Z_][a-zA-Z0-9_]*\s*\[.*\];" },
            { "Input", @"\s*kiara\.entrada\([a-zA-Z_][a-zA-Z0-9_]*\)\s*;" },
            { "Output", @"\s*kiara\.salida\([^\)]+\)\s*;" },
            { "IfStatement", @"\s*Si\s*\([^\)]+\)\s*\*?" },
            { "ElseStatement", @"\s*\}?\s*No\s*\*?" },
            { "ForLoop", @"\s*para\s*\(.*\)\s*\*?" },
            { "Comment", @"~\s*.*\s*~" },
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
                            tokens.Add((type, match.Value.Trim()));

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
