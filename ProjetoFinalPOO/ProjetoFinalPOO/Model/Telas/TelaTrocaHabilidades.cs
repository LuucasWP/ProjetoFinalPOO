using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Gerenciamento e Visualização de Habilidades nas Áreas de Descanso.
    /// Permite ao jogador visualizar todas as habilidades de cada combatente da tripulação
    /// e inspecionar suas categorias, moedas e modificadores.
    /// </summary>
    public class TelaTrocaHabilidades : ITela
    {
        private readonly List<Combatente> _tripulacao;
        private int _indicePersonagem;
        private int _indiceHabilidade;
        private string _mensagemStatus;

        public TelaTrocaHabilidades(List<Combatente> tripulacao)
        {
            _tripulacao = tripulacao ?? new List<Combatente>();
            _indicePersonagem = 0;
            _indiceHabilidade = 0;
            _mensagemStatus = "Use [◄/►] para alternar tripulante e [▲/▼] para inspecionar habilidades.";
        }

        public void Entrar()
        {
            _indicePersonagem = 0;
            _indiceHabilidade = 0;
        }

        public void Atualizar() { }
        public void Sair() { }

        public void Executar()
        {
            bool emExecucao = true;

            while (emExecucao)
            {
                Renderizar();
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        _indicePersonagem = (_indicePersonagem - 1 + _tripulacao.Count) % _tripulacao.Count;
                        _indiceHabilidade = 0;
                        _mensagemStatus = $"Visualizando habilidades de {_tripulacao[_indicePersonagem].Nome}.";
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        _indicePersonagem = (_indicePersonagem + 1) % _tripulacao.Count;
                        _indiceHabilidade = 0;
                        _mensagemStatus = $"Visualizando habilidades de {_tripulacao[_indicePersonagem].Nome}.";
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        var heroiCima = _tripulacao[_indicePersonagem];
                        if (heroiCima.Habilidades.Count > 0)
                        {
                            _indiceHabilidade = (_indiceHabilidade - 1 + heroiCima.Habilidades.Count) % heroiCima.Habilidades.Count;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        var heroiBaixo = _tripulacao[_indicePersonagem];
                        if (heroiBaixo.Habilidades.Count > 0)
                        {
                            _indiceHabilidade = (_indiceHabilidade + 1) % heroiBaixo.Habilidades.Count;
                        }
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                        int habIdx = tecla.Key - ConsoleKey.D1;
                        var heroiNum = _tripulacao[_indicePersonagem];
                        if (habIdx >= 0 && habIdx < heroiNum.Habilidades.Count)
                        {
                            _indiceHabilidade = habIdx;
                        }
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Enter:
                        emExecucao = false;
                        break;
                }
            }
        }

        public void Renderizar()
        {
            Limpar();
            RenderizadorUI.DesenharCabecalho("ÁREA DE DESCANSO & ARMERIA - BARALHO TÁTICO DE COMBATE DA TRIPULAÇÃO", 0, ConsoleColor.Green);
            Console.WriteLine();

            DesenharAbasTripulantes();
            DesenharDetalhesCombatente();
            DesenharListaHabilidades();
            DesenharStatusMensagem();
            DesenharRodape();
        }

        private void DesenharAbasTripulantes()
        {
            RenderizadorUI.DesenharInicioSecao("SELEÇÃO DE TRIPULANTE (A/D ou ◄/► para alternar)", 0, ConsoleColor.DarkGreen);

            string abas = "";
            for (int i = 0; i < _tripulacao.Count; i++)
            {
                var c = _tripulacao[i];
                string tag = (i == _indicePersonagem) ? $"[* {c.Nome.ToUpper()} ({c.GetType().Name}) *]" : $"[ {c.Nome} ]";
                abas += tag + "    ";
            }

            RenderizadorUI.DesenharLinhaCentralizada(abas, 0, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGreen);
            Console.WriteLine();
        }

        private void DesenharDetalhesCombatente()
        {
            var heroi = _tripulacao[_indicePersonagem];
            RenderizadorUI.DesenharDossierPersonagemComArte(heroi, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private void DesenharListaHabilidades()
        {
            var heroi = _tripulacao[_indicePersonagem];
            RenderizadorUI.DesenharInicioSecao($"BARALHO TÁTICO COMPLETO ({heroi.Habilidades.Count} CARTAS)", 0, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharGridHabilidadesCards(heroi.Habilidades, heroi.Habilidades, _indiceHabilidade);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkYellow);
            Console.WriteLine();
        }

        private void DesenharStatusMensagem()
        {
            RenderizadorUI.DesenharInicioSecao("PAINEL DE COMUNICAÇÃO DE BORDO", 0, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo(_mensagemStatus, 0, ConsoleColor.Yellow, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private void DesenharRodape()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [◄/► ou A/D] Trocar Personagem | [▲/▼ ou W/S] Selecionar Carta | [1-6] Seleção Direta | [ENTER/ESC] Voltar");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
