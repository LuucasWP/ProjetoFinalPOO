using System;
using ProjetoFinalPOO.Controladores;
using ProjetoFinalPOO.Model;

namespace ProjetoFinalPOO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configura o console com resolução ampla (110 colunas x 32 linhas), UTF-8 e cursor oculto
                ConfiguradorTela.ConfigurarTelaCheia();

                // Carrega todos os dados e configurações a partir dos arquivos JSON externos
                CarregadorDadosJogo.CarregarTodosDados();

                // Inicializa o orquestrador do jogo
                ControladorJogo jogo = new ControladorJogo();
                jogo.Iniciar();
            }
            catch (Exception ex)
            {
                ConfiguradorTela.RestaurarTela();
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERRO CRÍTICO NO SISTEMA]: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nPressione qualquer tecla para encerrar...");
                try
                {
                    Console.ReadKey(true);
                }
                catch { }
            }
            finally
            {
                // Restaura o cursor e o terminal original
                ConfiguradorTela.RestaurarTela();
            }
        }
    }
}
