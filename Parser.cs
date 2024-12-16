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
                else if (CurrentToken.TokenType == "VariableAssignment")
                {
                    Consume("VariableAssignment");
                }
                else if (CurrentToken.TokenType == "Input")
                {
                    Consume("Input");
                }
                else if (CurrentToken.TokenType == "Output")
                {
                    string condicion = ExtractConditionSalida(CurrentToken.Value);
                    ValidarSalida(condicion);
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
                    string forLoopCondition =CurrentToken.Value; // Extrae la condición del ciclo
                    ValidateForLoop(forLoopCondition);
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



        //Metodos que validan la condicion de (condicion) en el Si
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
            if (parts.Length<1)
            {
                throw new Exception($"Error sintáctico: La condición esta vacía 'Si (??)'");
            }
            else if (parts.Length == 1)
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

        private string ExtractConditionSalida(string salida)
        {
            var match = Regex.Match(salida, @"kiara.salida\s*\((.*)\);");
            if (!match.Success)
            {
                throw new Exception($"Error sintáctico: No se pudo extraer el mensaje de `kiara.salida`: '{salida}'");
            }
            return match.Groups[1].Value.Trim();
        }
    
        private void ValidarSalida(string condicion)
        {
            // Lista de palabras reservadas
            HashSet<string> reservedWords = new()
            {
               "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
            };

            // Método para validar variables
            void ValidarVariable(string variable)
            {
                // Verificar que no sea una palabra reservada
                if (reservedWords.Contains(variable))
                {
                    throw new Exception($"Error sintáctico: La palabra reservada '{variable}' no puede usarse como identificador en '{condicion}'");
                }

                // Validar que sea un identificador válido
                if (!Regex.IsMatch(variable, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                {
                    throw new Exception($"Error sintáctico: Identificador inválido '{variable}' en '{condicion}'");
                }
            }

            // Separar por comas
            string[] segments = condicion.Split(',');

            // Validar que no haya cadenas en combinaciones con comas
            foreach (string segment in segments)
            {
                string trimmedSegment = segment.Trim();

                // Si contiene operadores de concatenación (`+`), validar concatenaciones
                if (Regex.IsMatch(trimmedSegment, @"^([a-zA-Z_][a-zA-Z0-9_]*|\"".*\"")(\s*\+\s*([a-zA-Z_][a-zA-Z0-9_]*|\"".*\""))*$"))
                {
                    // Separar por `+` y validar cada parte
                    string[] parts = Regex.Split(trimmedSegment, @"\+").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                    foreach (string part in parts)
                    {
                        string trimmedPart = part.Trim();

                        if (trimmedPart.StartsWith("\"") && trimmedPart.EndsWith("\""))
                        {
                            // Validar que sea una cadena válida
                            if (!Regex.IsMatch(trimmedPart, @"^\"".*\""$"))
                            {
                                throw new Exception($"Error sintáctico: Cadena inválida '{trimmedPart}' en '{condicion}'");
                            }
                        }
                        else
                        {
                            // Validar que sea una variable válida
                            ValidarVariable(trimmedPart);
                        }
                    }
                }
                else if (Regex.IsMatch(trimmedSegment, @"^\"".*\""$"))
                {
                    // Lanzar error si las cadenas están separadas por comas
                    if (segments.Length > 1)
                    {
                        throw new Exception($"Error sintáctico: Las cadenas completas no pueden combinarse con comas en '{condicion}'");
                    }
                }
                else if (Regex.IsMatch(trimmedSegment, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                {
                    // Validar que sea una variable
                    ValidarVariable(trimmedSegment);
                }
                else
                {
                    throw new Exception($"Error sintáctico: Segmento inválido '{trimmedSegment}' en '{condicion}'");
                }
            }
        }
        //NO SIRVE 
        private void ValidateForLoop(string forLoop)
        {
            // Expresión regular para extraer la parte entre los paréntesis del ciclo `para()`
            var match = Regex.Match(forLoop, @"para\s*\((.*)\)");
            if (!match.Success)
            {
                throw new Exception($"Error sintáctico: No se pudo extraer la condición del ciclo `Para`: '{forLoop}'");
            }

            string condition = match.Groups[1].Value.Trim();

            // Separamos la condición en tres partes: declaración, condición e incremento
            string[] parts = Regex.Split(condition, @"\s*;\s*")
                                   .Where(p => !string.IsNullOrWhiteSpace(p))
                                   .ToArray();

            // Verificamos que hay exactamente tres partes: declaración, condición e incremento
            if (parts.Length != 3)
            {
                throw new Exception($"Error sintáctico: La estructura del ciclo `Para` no es válida: '{forLoop}'");
            }

            // Validamos cada parte del ciclo: declaración, condición, incremento
            ValidateForLoopDeclaration(parts[0]);
            ValidateForLoopCondition(parts[1]);
            ValidateForLoopIncrement(parts[2]);
        }

        // Validar la declaración en el ciclo `Para`
        private void ValidateForLoopDeclaration(string declaration)
        {
            // Expresión regular para validar la declaración 'ent x = 0;'
            string pattern = @"\s*(ent)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(\d+)\s*";
            var match = Regex.Match(declaration, pattern);

            if (!match.Success)
            {
                throw new Exception($"Error sintáctico: La declaración del ciclo `para` no es válida: '{declaration}'");
            }

            string variableName = match.Groups[2].Value;
            string value = match.Groups[3].Value;

            // Validar que el nombre de la variable no sea una palabra reservada
            if (IsReservedWord(variableName))
            {
                throw new Exception($"Error semántico: La variable '{variableName}' no puede ser una palabra reservada.");
            }

            // Validar que el valor sea un número
            if (!int.TryParse(value, out _))
            {
                throw new Exception($"Error semántico: El valor '{value}' en la declaración no es un número válido.");
            }
        }

        // Validar la condición del ciclo `Para`
        private void ValidateForLoopCondition(string condition)
        {
            condition = Regex.Replace(condition, @"\s+", " ").Trim();

            string[] parts = Regex.Split(condition, @"\b(<=|>=|==|!=|<|>)\b")
                                   .Where(p => !string.IsNullOrWhiteSpace(p))
                                   .ToArray();

            if (parts.Length != 3)
            {
                throw new Exception($"Error sintáctico: La condición del ciclo `Para` no es válida: '{condition}'");
            }

            string leftOperand = parts[0].Trim();
            string operatorPart = parts[1].Trim();
            string rightOperand = parts[2].Trim();

            // Validar los operandos
            ValidateForLoopOperand(leftOperand);
            ValidateForLoopOperand(rightOperand);

            // Validar el operador
            if (!Regex.IsMatch(operatorPart, @"^(==|!=|<|>|<=|>=)$"))
            {
                throw new Exception($"Error sintáctico: El operador '{operatorPart}' no es válido en la condición: '{condition}'");
            }
        }

        // Validar los operandos en la condición del ciclo `Para`
        private void ValidateForLoopOperand(string operand)
        {
            // Si el operando es una palabra reservada, se lanza un error semántico
            if (IsReservedWord(operand))
            {
                throw new Exception($"Error semántico: El operando '{operand}' no puede ser una palabra reservada.");
            }

            // Validar que el operando sea un identificador válido o un número
            if (!Regex.IsMatch(operand, @"^([a-zA-Z_][a-zA-Z0-9_]*|\d+|\"".*\"")$"))
            {
                throw new Exception($"Error sintáctico: El operando '{operand}' no es válido.");
            }
        }

        // Validar el incremento/decremento del ciclo `Para`
        private void ValidateForLoopIncrement(string increment)
        {
            // Validar que el incremento/decremento sea válido
            if (!Regex.IsMatch(increment, @"^\s*[a-zA-Z_][a-zA-Z0-9_]*\s*(\+\+|--|\+=|\-=|\*=|\/=)\s*\d*\s*$"))
            {
                throw new Exception($"Error sintáctico: El incremento/decremento del ciclo `Para` no es válido: '{increment}'");
            }
        }

        // Método para verificar si una palabra es reservada
        private bool IsReservedWord(string word)
        {
            HashSet<string> reservedWords = new()
    {
        "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
    };
            return reservedWords.Contains(word);
        }

    }
}
