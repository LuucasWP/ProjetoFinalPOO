using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Seleção de Espólio / Itens da Exploração (README 22).
    /// Nas áreas exploradas é apresentada uma lista de 3 itens e o jogador escolhe 1 para levar com a tripulação.
    /// </summary>
    public class TelaEscolhaItem : ITela
    {
        private readonly List<Item> _itensDisponiveis;
        private readonly List<Item> _inventarioEquipe;
        private int _indiceSelecionado;
        private Item _itemEscolhido;

        public TelaEscolhaItem(List<Item> itensDisponiveis, List<Item> inventarioEquipe)
        {
            _itensDisponiveis = itensDisponiveis ?? BancoItens.GerarEscolhaTresItens();
            _inventarioEquipe = inventarioEquipe ?? new List<Item>();
            _indiceSelecionado = 0;
            _itemEscolhido = null;
        }

        public void Entrar()
        {
            _indiceSelecionado = 0;
        }

        public void Atualizar() { }
        public void Sair() { }

        public Item Executar()
        {
            while (true)
            {
                Renderizar();
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        _indiceSelecionado = (_indiceSelecionado - 1 + _itensDisponiveis.Count) % _itensDisponiveis.Count;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        _indiceSelecionado = (_indiceSelecionado + 1) % _itensDisponiveis.Count;
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        _itemEscolhido = _itensDisponiveis[_indiceSelecionado];
                        _inventarioEquipe.Add(_itemEscolhido);
                        ExibirConfirmacaoEscolha();
                        return _itemEscolhido;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                        int idx = tecla.Key - ConsoleKey.D1;
                        if (idx >= 0 && idx < _itensDisponiveis.Count)
                        {
                            _indiceSelecionado = idx;
                            _itemEscolhido = _itensDisponiveis[idx];
                            _inventarioEquipe.Add(_itemEscolhido);
                            ExibirConfirmacaoEscolha();
                            return _itemEscolhido;
                        }
                        break;
                }
            }
        }

        private void ExibirConfirmacaoEscolha()
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho("ESPÓLIO ADQUIRIDO COM SUCESSO!", 0, ConsoleColor.Green);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("TRANSFERÊNCIA PARA O COMPARTIMENTO DE CARGA DA VANGUARDA", 0, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaCentralizada($"[OK] Item selecionado: {_itemEscolhido.Nome.ToUpper()} [{_itemEscolhido.Raridade.ToString().ToUpper()}]", 0, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaCentralizada($"Efeito: {_itemEscolhido.Descricao}", 0, ConsoleColor.White, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaCentralizada($"Total de Itens no Inventário: {_inventarioEquipe.Count}", 0, ConsoleColor.Cyan, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGreen);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para continuar a expedição... ]");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        public void Renderizar()
        {
            Limpar();
            RenderizadorUI.DesenharCabecalho("RECONHECIMENTO DO SETOR - SELEÇÃO DE ESPÓLIO E ITENS", 0, ConsoleColor.Cyan);
            Console.WriteLine();

            DesenharPainelItens();
            DesenharInventarioAtual();
            DesenharRodape();
        }

        private void DesenharPainelItens()
        {
            RenderizadorUI.DesenharInicioSecao("RECURSOS DETECTADOS NO PLANETA (ESCOLHA 1 ITEM PARA LEVAR - README 22)", 0, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharTresCardsItens(_itensDisponiveis, _indiceSelecionado);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private void DesenharInventarioAtual()
        {
            RenderizadorUI.DesenharInicioSecao($"INVENTÁRIO ATUAL DA TRIPULAÇÃO ({_inventarioEquipe.Count} ITENS ARMAZENADOS)", 0, ConsoleColor.DarkYellow);

            if (_inventarioEquipe.Count == 0)
            {
                RenderizadorUI.DesenharLinhaConteudo("Nenhum item armazenado no momento.", 0, ConsoleColor.DarkGray, ConsoleColor.DarkYellow);
            }
            else
            {
                string itensTexto = string.Join(" | ", _inventarioEquipe);
                RenderizadorUI.DesenharLinhaConteudo(itensTexto, 0, ConsoleColor.Gray, ConsoleColor.DarkYellow);
            }

            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkYellow);
            Console.WriteLine();
        }

        private void DesenharRodape()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [▲/▼ ou W/S ou ◄/►] Navegar entre Itens | [ENTER / Espaço / 1-3] Escolher Item para a Carga");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
