using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela do Menu Principal do jogo.
    /// Exibe arte ASCII temática, sinopse da missão mercenária espacial, opções interativas
    /// e roda em um layout de 104 colunas, ideal para transicionar para telas de combate 3v3.
    /// </summary>
    public class TelaMenu : ITela
    {
        private readonly (string Titulo, string Descricao)[] _opcoes;
        private int _selecionado;

        private static readonly string[] BannerAscii = new string[]
        {
            @"   __  __ _____ ____   ____ _____ _   _    _    ____  ___ ___  ____    ____   ___    _____ _____ _____ ____  ",
            @"  |  \/  | ____|  _ \ / ___| ____| \ | |  / \  |  _ \|_ _/ _ \/ ___|  |  _ \ / _ \  | ____|_   _| ____|  _ \ ",
            @"  | |\/| |  _| | |_) | |   |  _| |  \| | / _ \ | |_) || | | | \___ \  | | | | | | | |  _|   | | |  _| | |_) |",
            @"  | |  | | |___|  _ <| |___| |___| |\  |/ ___ \|  _ < | | |_| |___) | | |_| | |_| | | |___  | | | |___|  _ < ",
            @"  |_|  |_|_____|_| \_\\____|_____|_| \_/_/   \_\_| \_\___\___/|____/  |____/ \___/  |_____| |_| |_____|_| \_\"
        };

        public TelaMenu()
        {
            _opcoes = new (string, string)[]
            {
                ("NOVO JOGO",      "Iniciar travessia estelar com sua tripulação mercenária e carga valiosa."),
                ("CARREGAR JOGO",  "Retomar progresso de uma expedição espacial anterior."),
                ("CONFIGURACOES",  "Ver parâmetros técnicos do sistema e exibição."),
                ("DIARIO & LORE",  "Ver detalhes da missão, classes de tripulantes e mecânicas de combate."),
                ("SAIR",           "Desconectar terminal de bordo e encerrar operação.")
            };
            _selecionado = 0;
        }

        public void Entrar()
        {
            _selecionado = 0;
        }

        public void Atualizar() { }
        public void Sair() { }

        public OpcaoMenuPrincipal Executar()
        {
            while (true)
            {
                Renderizar();
                ConsoleKeyInfo teclaInfo = Console.ReadKey(true);

                if (ProcessarTecla(teclaInfo.Key, out OpcaoMenuPrincipal opcaoConfirmada))
                {
                    return opcaoConfirmada;
                }
            }
        }

        private bool ProcessarTecla(ConsoleKey tecla, out OpcaoMenuPrincipal opcaoConfirmada)
        {
            opcaoConfirmada = (OpcaoMenuPrincipal)_selecionado;

            switch (tecla)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    MoverCima();
                    return false;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    MoverBaixo();
                    return false;

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    opcaoConfirmada = (OpcaoMenuPrincipal)_selecionado;
                    return true;

                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    _selecionado = 0;
                    opcaoConfirmada = OpcaoMenuPrincipal.NovoJogo;
                    return true;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    _selecionado = 1;
                    opcaoConfirmada = OpcaoMenuPrincipal.CarregarJogo;
                    return true;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    _selecionado = 2;
                    opcaoConfirmada = OpcaoMenuPrincipal.Opcoes;
                    return true;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    _selecionado = 3;
                    opcaoConfirmada = OpcaoMenuPrincipal.Creditos;
                    return true;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                case ConsoleKey.Escape:
                    _selecionado = 4;
                    opcaoConfirmada = OpcaoMenuPrincipal.Sair;
                    return true;

                default:
                    return false;
            }
        }

        public void Renderizar()
        {
            Limpar();
            DesenharCabecalhoGrafico();
            DesenharPainelCentralMenu();
            DesenharRodapeControles();
        }

        private void DesenharCabecalhoGrafico()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔" + new string('═', Math.Max(0, RenderizadorUI.LarguraPadrao - 2)) + "╗");

            foreach (var linha in BannerAscii)
            {
                string centralizada = RenderizadorUI.CentralizarTexto(linha, Math.Max(0, RenderizadorUI.LarguraPadrao - 2));
                Console.WriteLine("║" + centralizada + "║");
            }

            string subtitulo = "[ SISTEMA TÁTICO DE COMBATE POR CARTAS & NAVEGAÇÃO ESPACIAL ]";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("║" + RenderizadorUI.CentralizarTexto(subtitulo, Math.Max(0, RenderizadorUI.LarguraPadrao - 2)) + "║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚" + new string('═', Math.Max(0, RenderizadorUI.LarguraPadrao - 2)) + "╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DesenharPainelCentralMenu()
        {
            int larguraTotal = RenderizadorUI.LarguraAtual;
            int larguraEsq = Math.Max(28, Math.Min(48, (int)(larguraTotal * 0.30)));
            int larguraDir = Math.Max(38, larguraTotal - larguraEsq - 4);
            const int totalLinhas = 12;

            ConsoleColor corBorda = ConsoleColor.DarkCyan;
            string[] arteNave = BancoSprites.ObterArteNaveVanguarda();

            for (int l = 0; l < totalLinhas; l++)
            {
                // ================= COLUNA ESQUERDA =================
                Console.Write(" ");
                if (l == 0)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("┌" + new string('─', larguraEsq - 2) + "┐");
                }
                else if (l == 11)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("└" + new string('─', larguraEsq - 2) + "┘");
                }
                else
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("│ ");

                    string conteudoEsq;
                    ConsoleColor corTextoEsq;

                    if (l == 1)
                    {
                        conteudoEsq = "NAVE VANGUARDA (SETOR HELIOS-8)";
                        corTextoEsq = ConsoleColor.Cyan;
                    }
                    else if (l >= 2 && l <= 8)
                    {
                        int spriteIdx = l - 2;
                        conteudoEsq = (spriteIdx < arteNave.Length) ? arteNave[spriteIdx] : "";
                        conteudoEsq = RenderizadorUI.CentralizarTexto(conteudoEsq, larguraEsq - 4);
                        corTextoEsq = ConsoleColor.DarkCyan;
                    }
                    else if (l == 9)
                    {
                        conteudoEsq = "Carga: CARGA 73 (PROTEGIDA)";
                        corTextoEsq = ConsoleColor.Green;
                    }
                    else // l == 10
                    {
                        conteudoEsq = "Setor: ÉTER-HELIOS (SETOR 01)";
                        corTextoEsq = ConsoleColor.DarkGreen;
                    }

                    Console.ForegroundColor = corTextoEsq;
                    Console.Write(RenderizadorUI.TruncarOuPad(conteudoEsq, larguraEsq - 4));

                    Console.ForegroundColor = corBorda;
                    Console.Write(" │");
                }

                Console.Write("  ");

                // ================= COLUNA DIREITA =================
                if (l == 0)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("┌" + new string('─', larguraDir - 2) + "┐");
                }
                else if (l == 7)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("├" + new string('─', larguraDir - 2) + "┤");
                }
                else if (l == 11)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("└" + new string('─', larguraDir - 2) + "┘");
                }
                else
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("│ ");

                    if (l == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(RenderizadorUI.TruncarOuPad("TERMINAL DE COMANDO - SELECIONE A DIRETRIZ", larguraDir - 4));
                    }
                    else if (l >= 2 && l < 2 + _opcoes.Length)
                    {
                        int opIdx = l - 2;
                        bool selecionado = (opIdx == _selecionado);
                        string textoOpcao = selecionado
                            ? $">> [{opIdx + 1}]  {_opcoes[opIdx].Titulo}  <<"
                            : $"   [{opIdx + 1}]  {_opcoes[opIdx].Titulo}   ";

                        Console.ForegroundColor = selecionado ? ConsoleColor.Green : ConsoleColor.DarkGray;
                        Console.Write(RenderizadorUI.TruncarOuPad(textoOpcao, larguraDir - 4));
                    }
                    else if (l == 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(RenderizadorUI.TruncarOuPad($"DETALHES: {_opcoes[_selecionado].Descricao}", larguraDir - 4));
                    }
                    else if (l == 9)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(RenderizadorUI.TruncarOuPad("OBJETIVO: Escolta mercenária por 6 galáxias até o ponto de extração.", larguraDir - 4));
                    }
                    else if (l == 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(RenderizadorUI.TruncarOuPad("SISTEMA: Combate 3v3 por cartas e moedas influenciadas por Recursos Especiais.", larguraDir - 4));
                    }

                    Console.ForegroundColor = corBorda;
                    Console.Write(" │");
                }

                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private void DesenharRodapeControles()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [▲/▼ ou W/S] Navegar entre opções  |  [ENTER / Espaço] Confirmar seleção  |  [1-5 / ESC] Atalho rápido");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }

        public int GetOpcaoSelecionada() => _selecionado;

        public void MoverCima()
        {
            _selecionado = (_selecionado - 1 + _opcoes.Length) % _opcoes.Length;
        }

        public void MoverBaixo()
        {
            _selecionado = (_selecionado + 1) % _opcoes.Length;
        }
    }
}
