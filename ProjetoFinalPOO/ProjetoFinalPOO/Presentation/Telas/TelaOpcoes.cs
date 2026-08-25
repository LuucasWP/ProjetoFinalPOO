using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Configurações e Parâmetros Técnicos do Sistema.
    /// Exibe resolução, codificação e mecânicas ativas.
    /// </summary>
    public class TelaOpcoes : ITela
    {
        public TelaOpcoes() { }

        public void Entrar() { }
        public void Atualizar() { }
        public void Sair() { }

        public void Executar()
        {
            while (true)
            {
                Renderizar();
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                if (tecla.Key == ConsoleKey.Enter || tecla.Key == ConsoleKey.Escape || tecla.Key == ConsoleKey.Spacebar)
                {
                    return;
                }
            }
        }

        public void Renderizar()
        {
            Limpar();
            RenderizadorUI.DesenharCabecalho("CONFIGURAÇÕES DO SISTEMA // PARÂMETROS TÉCNICOS", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow);
            Console.WriteLine();

            DesenharParametrosTecnicos();
            DesenharInstrucoes();
        }

        private void DesenharParametrosTecnicos()
        {
            RenderizadorUI.DesenharInicioSecao("PARÂMETROS DE RENDERIZAÇÃO E RESOLUÇÃO", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaDupla("Modo de Resolução", $"{ConfiguradorTela.LarguraAlvo} Colunas x {ConfiguradorTela.AlturaAlvo} Linhas (Amplo para Batalha 3v3)", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaDupla("Codificação do Terminal", "UTF-8 Nativo com Alternate Screen Buffer", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaDupla("Mecânica de Combate", "Cartas Táticas + Moedas (Limbus Company)", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaDupla("Sistema de Câmera", "Câmera 3D Perspectiva com Interpolação e Trepidação", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private void DesenharInstrucoes()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [ENTER / ESC] Voltar ao Menu Principal");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
