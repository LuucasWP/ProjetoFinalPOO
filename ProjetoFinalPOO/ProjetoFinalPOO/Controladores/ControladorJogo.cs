using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Mapa;
using ProjetoFinalPOO.Model;
using ProjetoFinalPOO.Model.Telas;
using ProjetoFinalPOO.Música;

namespace ProjetoFinalPOO.Controladores
{
    /// <summary>
    /// Orquestrador do fluxo principal do jogo (Menu Principal, Configurações, Carregamento,
    /// Mapa Estelar, Exploração e Batalha Tática 3v3).
    /// Gerencia o estado persistente da tripulação mercenária (README 28) e seus itens (README 18 e 22).
    /// </summary>
    public class ControladorJogo
    {
        private readonly GerenciadorTelas _gerenciador;
        private List<Combatente> _tripulacao;
        private List<Item> _inventarioEquipe;

        public ControladorJogo()
        {
            _gerenciador = GerenciadorTelas.Instancia;
            ResetarEstadoJogo();
        }

        private void ResetarEstadoJogo()
        {
            // Inicializa a party pré-definida com três personagens: Optimus (Sentinela), Asimov (Engenheiro) e Pasteur (Biomancer)
            _tripulacao = new List<Combatente>
            {
                BancoHabilidades.CriarSentinela(),
                BancoHabilidades.CriarEngenheiro(),
                BancoHabilidades.CriarBiomancer()
            };

            // Inicializa o inventário com itens iniciais (README 18 e 22)
            _inventarioEquipe = BancoItens.ObterInventarioInicial();
        }

        public void Iniciar()
        {
            bool executando = true;

            try
            {
                var musicaInicio = BibliotecaDeMusicas.MusicaInicio();
                musicaInicio?.Tocar();
            }
            catch
            {
                // Proteção para plataformas sem suporte a Console.Beep
            }

            while (executando)
            {
                var telaMenu = ControladorTela.CriarTelaMenu();
                _gerenciador.AlterarTela(telaMenu);

                OpcaoMenuPrincipal opcao = telaMenu.Executar();

                switch (opcao)
                {
                    case OpcaoMenuPrincipal.NovoJogo:
                        IniciarNovoJogo();
                        break;

                    case OpcaoMenuPrincipal.CarregarJogo:
                        AbrirCarregarJogo();
                        break;

                    case OpcaoMenuPrincipal.Opcoes:
                        AbrirOpcoes();
                        break;

                    case OpcaoMenuPrincipal.Creditos:
                        AbrirCreditos();
                        break;

                    case OpcaoMenuPrincipal.Sair:
                        executando = false;
                        break;
                }
            }

            ExibirDespedida();
        }

        private void IniciarNovoJogo()
        {
            ResetarEstadoJogo();

            Console.Clear();
            RenderizadorUI.DesenharCabecalho("PREPARANDO SALTO HIPERESPACIAL - TRIPULAÇÃO VANGUARDA", 0, ConsoleColor.Green);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("INICIALIZAÇÃO DA MISSÃO MERCENÁRIA (ESCOLTA DA CARGA 73)", 0, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaConteudo("Iniciando propulsores de dobra espacial da nave Vanguarda...", 0, ConsoleColor.White, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaConteudo("Tripulação a postos: Optimus [Sentinela - Adrenalina], Asimov [Engenheiro - Superaquecimento] e Pasteur [Biomancer - Mana].", 0, ConsoleColor.Cyan, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaConteudo("Carregando mapa estelar em grafo de 6 galáxias com estações, fendas e patrulhas sindicais...", 0, ConsoleColor.Gray, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGreen);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para acessar o Mapa Estelar... ]");
            Console.ResetColor();
            Console.ReadKey(true);

            // Instancia o Mapa e constrói o Grafo com 6 Galáxias via MapaRPGBuilder
            var mapa = new ProjetoFinalPOO.Mapa.Mapa("Setor Éter-Helios");
            var grafo = new MapaRPGBuilder()
                .GerarInicio()
                .AdicionarGalaxia(1)
                .AdicionarGalaxia(2)
                .AdicionarGalaxia(3)
                .AdicionarGalaxia(4)
                .AdicionarGalaxia(5)
                .AdicionarGalaxia(6)
                .Construir();

            var telaMapa = ControladorTela.CriarTelaMapa(mapa, grafo, _tripulacao, _inventarioEquipe);
            _gerenciador.AlterarTela(telaMapa);
            telaMapa.Executar();
        }

        private void AbrirCarregarJogo()
        {
            var telaCarregar = ControladorTela.CriarTelaCarregarJogo();
            _gerenciador.AlterarTela(telaCarregar);
            telaCarregar.Executar();
        }

        private void AbrirOpcoes()
        {
            var telaOpcoes = ControladorTela.CriarTelaOpcoes();
            _gerenciador.AlterarTela(telaOpcoes);
            telaOpcoes.Executar();
        }

        private void AbrirCreditos()
        {
            var telaCreditos = ControladorTela.CriarTelaCreditos();
            _gerenciador.AlterarTela(telaCreditos);
            telaCreditos.Executar();
        }

        private void ExibirDespedida()
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho("MERCENÁRIOS DO ÉTER - SISTEMA DESCONECTADO", 0, ConsoleColor.Cyan);
            Console.WriteLine();
            RenderizadorUI.DesenharInicioSecao("SESSÃO ENCERRADA", 0, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaCentralizada("Obrigado por jogar! Até a próxima expedição estelar.", 0, ConsoleColor.White, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }
    }
}
