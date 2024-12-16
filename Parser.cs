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
                    string loopCondition = CurrentToken.Value;
                    ValidateForLoop(loopCondition);
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
        private void ValidateForLoop(string loopCondition)
        {
            // Dividir la condición completa del ciclo en inicialización, condición y actualización
            string pattern = @"^\s*(?<init>[^;]+);\s*(?<cond>[^;]+);\s*(?<update>.+)\s*$";
            Match match = Regex.Match(loopCondition, pattern);

            if (!match.Success)
            {
                throw new Exception($"Error de sintaxis: la declaración del ciclo 'para' es inválida: {loopCondition}");
            }

            string initialization = match.Groups["init"].Value.Trim();
            string condition = match.Groups["cond"].Value.Trim();
            string update = match.Groups["update"].Value.Trim();

            // Crear un diccionario local para rastrear variables dentro del ciclo
            Dictionary<string, string> localVariables = new();

            // Validar la inicialización
            ValidateInitialization(initialization, localVariables);

            // Validar la condición
            ValidateCondition(condition, localVariables);

            // Validar el incremento/decremento
            ValidateIncrementDecrement(update, localVariables);
        }



        private void ValidateInitialization(string initialization, Dictionary<string, string> localVariables)
        {
            HashSet<string> reservedWords = new()
            {
              "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
            };
            // Expresión regular para la inicialización, ejemplo: "ent i = 1"
            Regex initRegex = new(@"^(ent)\s+(\w+)\s*=\s*(-?\d+)$");
            Match match = initRegex.Match(initialization);

            if (!match.Success)
            {
                throw new Exception($"Error de sintaxis en la inicialización: {initialization}");
            }

            string type = match.Groups[1].Value;
            string variableName = match.Groups[2].Value;

            // Verificar que el nombre de la variable no sea una palabra reservada
            if (reservedWords.Contains(variableName))
            {
                throw new Exception($"Error: el nombre de la variable '{variableName}' es una palabra reservada.");
            }

            // Registrar la variable localmente
            localVariables[variableName] = type;
        }


        private void ValidateCondition(string condition, Dictionary<string, string> localVariables)
        {
            HashSet<string> reservedWords = new()
            {
              "ent", "cad", "flot", "sn", "let", "para", "Si", "No", "True", "False", "kiara", "list"
            };
            // Verificar que no haya palabras reservadas
            foreach (string word in reservedWords)
            {
                if (condition.Contains(word))
                {
                    throw new Exception($"Error: la condición contiene la palabra reservada '{word}'.");
                }
            }

            // Verificar que las variables usadas en la condición estén declaradas localmente
            Regex variableRegex = new(@"\b\w+\b");
            foreach (Match match in variableRegex.Matches(condition))
            {
                string variable = match.Value;
                if (!localVariables.ContainsKey(variable) && !int.TryParse(variable, out _))
                {
                    throw new Exception($"Error: la variable '{variable}' en la condición no está declarada.");
                }
            }
        }



        private void ValidateIncrementDecrement(string update, Dictionary<string, string> localVariables)
        {
            // Expresión regular para soportar todas las formas de incremento/decremento:
            // Ejemplos válidos: i++, i--, i+=1, i-=2, i*=3, i/=4, i%=5
            Regex updateRegex = new(@"^(?<variable>\w+)\s*(?<operator>(\+\+|--|[\+\-\*/%]=))\s*(?<value>-?\d+)?$");
            Match match = updateRegex.Match(update);

            if (!match.Success)
            {
                throw new Exception($"Error de sintaxis en la actualización: {update}");
            }

            string variable = match.Groups["variable"].Value;
            string operation = match.Groups["operator"].Value;
            string value = match.Groups["value"].Value;

            // Verificar que la variable esté declarada localmente
            if (!localVariables.ContainsKey(variable))
            {
                throw new Exception($"Error: la variable '{variable}' en la actualización no está declarada.");
            }

            // Verificar que el valor sea un número si el operador lo requiere
            if ((operation != "++" && operation != "--") && string.IsNullOrEmpty(value))
            {
                throw new Exception($"Error: se esperaba un valor numérico en la operación '{operation}' para la variable '{variable}'.");
            }
        }






    }
}
