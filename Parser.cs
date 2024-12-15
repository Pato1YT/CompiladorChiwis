using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompiladorChiwis
{
    internal class Parser
    {
        private readonly List<(string TokenType, string Value)> tokens;
        private int currentIndex = 0;
        private int openBracesCount = 0; // Contador de llaves abiertas

        public Parser(List<(string TokenType, string Value)> tokens)
        {
            this.tokens = tokens;
        }

        private (string TokenType, string Value) CurrentToken =>
            currentIndex < tokens.Count ? tokens[currentIndex] : ("EOF", "");

        private void Consume(string expectedType)
        {
            if (CurrentToken.TokenType == expectedType)
                currentIndex++;
            else
                throw new Exception($"Error de sintaxis: se esperaba {expectedType}, pero se encontró {CurrentToken.TokenType}");
        }

        public void Parse()
        {
            while (CurrentToken.TokenType != "EOF")
            {
                if (CurrentToken.TokenType == "MainMethod")
                {
                    Consume("MainMethod");
                    ParseBlock();
                }
                else
                {
                    throw new Exception($"Error inesperado en el token {CurrentToken.Value}");
                }
            }

            // Al final, las llaves abiertas deben estar balanceadas
            if (openBracesCount != 0)
            {
                throw new Exception("Error de sintaxis: las llaves no están balanceadas.");
            }
        }

        private void ParseBlock()
        {
            if (CurrentToken.TokenType == "OpenBrace")
            {
                Consume("OpenBrace");
                openBracesCount++; // Contar la llave de apertura
            }
            else
            {
                throw new Exception($"Error de sintaxis: se esperaba OpenBrace, pero se encontró {CurrentToken.TokenType}");
            }

            while (CurrentToken.TokenType != "CloseBrace" && CurrentToken.TokenType != "EOF")
            {
                // Ignorar los comentarios
                if (CurrentToken.TokenType == "Comment")
                {
                    Consume("Comment");  // Ignorar el comentario
                    continue;  // Continuar con el siguiente token
                }

                if (CurrentToken.TokenType == "VariableEnt")
                {
                    Consume("VariableEnt");
                }

                else if (CurrentToken.TokenType == "DeclarationFlot")
                {
                    Consume("DeclarationFlot");
                }

                else if (CurrentToken.TokenType == "DeclarationSn")
                {
                    Consume("DeclarationSn");
                }

                else if (CurrentToken.TokenType == "DeclarationCad")
                {
                    Consume("DeclarationCad");
                }

                else if (CurrentToken.TokenType == "DeclarationLet")
                {
                    Consume("DeclarationLet");
                }

                else if (CurrentToken.TokenType == "ListDeclaration")
                {
                    Consume("ListDeclaration");
                }
                else if (CurrentToken.TokenType == "Input")
                {
                    Consume("Input");
                }
                else if (CurrentToken.TokenType == "Output")
                {
                    Consume("Output");
                }
                else if (CurrentToken.TokenType == "IfStatement")
                {
                    string condition = ExtractCondition(CurrentToken.Value);

                    // Validar la condición
                    ValidateIfCondition(condition);

                    // Consumir el token del `Si`
                    Consume("IfStatement");

                    // Procesar el bloque del `Si`
                    ParseBlock();
                    if (CurrentToken.TokenType == "ElseStatement")
                    {
                        Consume("ElseStatement");
                        ParseBlock(); // Procesa el bloque del `No`
                    }
                }
                else if (CurrentToken.TokenType == "ForLoop")
                {
                    Consume("ForLoop");
                    ParseBlock(); // Procesa el bloque del `para`
                }
                else
                {
                    throw new Exception($"Error de sintaxis: token inesperado {CurrentToken.TokenType} en {CurrentToken.Value}");
                }
            }

            if (CurrentToken.TokenType == "CloseBrace")
            {
                Consume("CloseBrace");
                openBracesCount--; // Contar la llave de cierre
            }
            else
            {
                throw new Exception($"Error de sintaxis: se esperaba CloseBrace, pero se encontró {CurrentToken.TokenType}");
            }

            // Verifica si las llaves se han balanceado al final del bloque
            if (openBracesCount < 0)
            {
                throw new Exception("Error de sintaxis: llave de cierre sin llave de apertura correspondiente.");
            }
        }

        private void ValidateIfCondition(string condition)
        {
            // Lista de palabras reservadas
            HashSet<string> reservedWords = new()
            {
              "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
            };

            // Separar por operadores lógicos y relacionales
            string[] parts = Regex.Split(condition, @"(==|!=|<|>|<=|>=|&&|\|\|)").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            // Validar si hay una sola  
            if (parts.Length == 1)
            {
                string part = parts[0].Trim();

                // Verificar que no sea una palabra reservada
                if (reservedWords.Contains(part))
                {
                    throw new Exception($"Error sintáctico: La palabra reservada '{part}' no puede usarse como operando en la condición '{condition}'");
                }

                // Validar que sea un identificador válido
                if (!Regex.IsMatch(part, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
                {
                    throw new Exception($"Error sintáctico: Operando inválido '{part}' en la condición '{condition}'");
                }

                // Si es válido, salir del método
                return;
            }

            // Si hay más de una parte, validar operadores y operandos
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();

                if (i % 2 == 0) // Operandos (en posiciones pares)
                {
                    // Verificar que no sea una palabra reservada
                    if (reservedWords.Contains(part))
                    {
                        throw new Exception($"Error sintáctico: La palabra reservada '{part}' no puede usarse como operando en la condición '{condition}'");
                    }

                    // Validar que sea un identificador válido, un número, o un string
                    if (!Regex.IsMatch(part, @"^([a-zA-Z_][a-zA-Z0-9_]*|\d+|\"".*\"")$"))
                    {
                        throw new Exception($"Error sintáctico: Operando inválido '{part}' en la condición '{condition}'");
                    }
                }
                else // Operadores (en posiciones impares)
                {
                    // Validar que sea un operador válido
                    if (!Regex.IsMatch(part, @"^(==|!=|<|>|<=|>=|&&|\|\|)$"))
                    {
                        throw new Exception($"Error sintáctico: Operador inválido '{part}' en la condición '{condition}'");
                    }
                }
            }

            // Verificar que la condición empiece y termine con un operando
            if (!Regex.IsMatch(parts[0].Trim(), @"^([a-zA-Z_][a-zA-Z0-9_]*|\d+|\"".*\"")$") ||
                !Regex.IsMatch(parts[^1].Trim(), @"^([a-zA-Z_][a-zA-Z0-9_]*|\d+|\"".*\"")$"))
            {
                throw new Exception($"Error sintáctico: La condición del `Si` debe comenzar y terminar con un operando válido: '{condition}'");
            }
        }


        private string ExtractCondition(string ifStatement)
        {
            var match = Regex.Match(ifStatement, @"Si\s*\((.*)\)");
            if (!match.Success)
            {
                throw new Exception($"Error sintáctico: No se pudo extraer la condición del `Si`: '{ifStatement}'");
            }
            return match.Groups[1].Value.Trim();
        }

        private void ValidateVariable(string variable)
        {
            // Lista de palabras reservadas
            HashSet<string> reservedWords = new()
            {
              "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
            };

            // Validar que no sea una palabra reservada
            if (reservedWords.Contains(variable))
            {
                throw new Exception($"Error sintáctico: La palabra reservada '{variable}' no puede usarse como identificador.");
            }

            // Validar que sea un identificador válido
            if (!Regex.IsMatch(variable, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                throw new Exception($"Error sintáctico: Identificador inválido '{variable}'.");
            }
        }




    }
}
