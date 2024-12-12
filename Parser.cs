using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                else if (CurrentToken.TokenType =="DeclarationSn")
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
                    Consume("IfStatement");
                    ParseBlock(); // Procesa el bloque del `Si`
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

    }
}
