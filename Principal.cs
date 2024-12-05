namespace CompiladorChiwis
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Este método no está siendo utilizado, lo puedes mantener vacío o implementarlo si lo deseas
        }

        public void validarCodigo()
        {
            string code = textBox1.Text;
            MessageBox.Show(code);

            try
            {
                var lexer = new Lexer();
                var tokens = lexer.Tokenize(code);

                MessageBox.Show("Tokens:");
                foreach (var token in tokens)
                {
                    MessageBox.Show($"{token.TokenType}: {token.Value}");
                }

                var parser = new Parser(tokens);
                parser.Parse();

                MessageBox.Show("El código es válido.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            validarCodigo();
        }
    }
}
