using System;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Carregamento de Partidas Salvas.
    /// </summary>
    public class TelaCarregarJogo : ITela
    {
        public void Entrar() { }
        public void Atualizar() { }
        public void Sair() { }

        public void Executar()
        {
            Renderizar();
            Console.ReadKey(true);
        }

        public void Renderizar()
        {
            Limpar();
            RenderizadorUI.DesenharCabecalho("TERMINAL DE DADOS - CARREGAR EXPEDIÇÃO ESPACIAL", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("REGISTROS DE MISSÕES NO BANCO DE MEMÓRIA DA VANGUARDA", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);

            int larguraCard = Math.Max(24, (RenderizadorUI.LarguraAtual - 8) / 3);
            int espacoInterno = Math.Max(0, larguraCard - 4);
            List<string[]> cards = new List<string[]>();

            for (int i = 1; i <= 3; i++)
            {
                string[] linhas = new string[9];
                linhas[0] = "┌" + new string('─', larguraCard - 2) + "┐";
                linhas[1] = "│ " + RenderizadorUI.TruncarOuPad($"[SLOT {i}] REGISTRO DE VÔO {i}", espacoInterno) + " │";
                linhas[2] = "│ " + RenderizadorUI.CentralizarTexto("     .---------.     ", espacoInterno) + " │";
                linhas[3] = "│ " + RenderizadorUI.CentralizarTexto("    /  VAZIO  \\    ", espacoInterno) + " │";
                linhas[4] = "│ " + RenderizadorUI.CentralizarTexto("   |  [SEM DADOS]|   ", espacoInterno) + " │";
                linhas[5] = "│ " + RenderizadorUI.CentralizarTexto("    \\_________/    ", espacoInterno) + " │";
                linhas[6] = "│ " + RenderizadorUI.TruncarOuPad("Nenhuma coordenada salva.", espacoInterno) + " │";
                linhas[7] = "│ " + RenderizadorUI.TruncarOuPad("[ Pressione 1-3 para iniciar ]", espacoInterno) + " │";
                linhas[8] = "└" + new string('─', larguraCard - 2) + "┘";
                cards.Add(linhas);
            }

            for (int l = 0; l < 9; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < 3; c++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(cards[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para retornar ao Menu Principal... ]");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
