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
        }

        private void ParseBlock()
        {
            if (CurrentToken.TokenType == "OpenBrace")
            {
                Consume("OpenBrace"); // Consume la llave de apertura
            }
            else
            {
                throw new Exception($"Error de sintaxis: se esperaba OpenBrace, pero se encontró {CurrentToken.TokenType}");
            }

            while (CurrentToken.TokenType != "CloseBrace" && CurrentToken.TokenType != "EOF")
            {
                if (CurrentToken.TokenType == "VariableDeclaration")
                {
                    Consume("VariableDeclaration");
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
                Consume("CloseBrace"); // Consume la llave de cierre
            }
            else
            {
                throw new Exception($"Error de sintaxis: se esperaba CloseBrace, pero se encontró {CurrentToken.TokenType}");
            }
        }

    }
}
