using System;
using System.Collections.Generic;
using System.Linq;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Fornece componentes visuais padronizados para o console (molduras contínuas em Unicode, cabeçalhos, cards 3v3,
    /// painéis com artes ASCII, barras de status e placeholders para imagens).
    /// Redimensiona-se automaticamente de acordo com o tamanho da janela do console,
    /// com suporte a grids de batalha de 3 combatentes x 3 inimigos, painéis de exploração e decks de cartas.
    /// </summary>
    public static class RenderizadorUI
    {
        public static int LarguraPadrao => ConfiguradorTela.ObterLarguraConsole();
        public static int LarguraAtual => ConfiguradorTela.ObterLarguraConsole();

        public static void DesenharCabecalho(string titulo, int largura = 0, ConsoleColor cor = ConsoleColor.Cyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = cor;
            Console.WriteLine("╔" + new string('═', Math.Max(0, largura - 2)) + "╗");

            string textoCentralizado = CentralizarTexto(titulo, Math.Max(0, largura - 2));
            Console.WriteLine("║" + textoCentralizado + "║");

            Console.WriteLine("╚" + new string('═', Math.Max(0, largura - 2)) + "╝");
            Console.ResetColor();
        }

        public static void DesenharInicioSecao(string titulo, int largura = 0, ConsoleColor cor = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = cor;
            string tag = $"[ {titulo} ]";
            if (tag.Length + 6 >= largura)
            {
                tag = TruncarOuPad(tag, Math.Max(0, largura - 6));
            }

            string linha = $"┌─ {tag} ";
            if (linha.Length < largura - 1)
            {
                linha += new string('─', largura - 1 - linha.Length) + "┐";
            }
            else
            {
                linha = linha.Substring(0, Math.Max(0, largura - 1)) + "┐";
            }
            Console.WriteLine(linha);
            Console.ResetColor();
        }

        public static void DesenharLinhaConteudo(string texto, int largura = 0, ConsoleColor corTexto = ConsoleColor.Gray, ConsoleColor corBorda = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = corBorda;
            Console.Write("│ ");
            Console.ForegroundColor = corTexto;

            int espacoInterno = Math.Max(0, largura - 4);
            string textoFormatado = (texto.Length > espacoInterno) ? texto.Substring(0, espacoInterno) : texto.PadRight(espacoInterno);
            Console.Write(textoFormatado);

            Console.ForegroundColor = corBorda;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        public static void DesenharLinhaCentralizada(string texto, int largura = 0, ConsoleColor corTexto = ConsoleColor.White, ConsoleColor corBorda = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = corBorda;
            Console.Write("│ ");
            Console.ForegroundColor = corTexto;

            int espacoInterno = Math.Max(0, largura - 4);
            string centralizado = CentralizarTexto(texto, espacoInterno);
            Console.Write(centralizado);

            Console.ForegroundColor = corBorda;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        public static void DesenharLinhaDupla(string textoEsquerda, string textoDireita, int largura = 0, ConsoleColor corEsq = ConsoleColor.Gray, ConsoleColor corDir = ConsoleColor.Yellow, ConsoleColor corBorda = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            int espacoInterno = Math.Max(0, largura - 4);
            string esq = textoEsquerda ?? "";
            string dir = textoDireita ?? "";

            // Se o comprimento total exceder o espaço interno, trunca com segurança
            if (esq.Length + dir.Length + 2 > espacoInterno)
            {
                int maxDir = Math.Min(dir.Length, espacoInterno / 2);
                if (dir.Length > maxDir && maxDir > 3)
                {
                    dir = dir.Substring(0, maxDir - 3) + "...";
                }

                int maxEsq = espacoInterno - dir.Length - 1;
                if (esq.Length > maxEsq && maxEsq > 3)
                {
                    esq = esq.Substring(0, maxEsq - 3) + "...";
                }
            }

            int espacoMeio = espacoInterno - esq.Length - dir.Length;
            if (espacoMeio < 0) espacoMeio = 0;

            Console.ForegroundColor = corBorda;
            Console.Write("│ ");

            Console.ForegroundColor = corEsq;
            Console.Write(esq);

            Console.Write(new string(' ', espacoMeio));

            Console.ForegroundColor = corDir;
            Console.Write(dir);

            Console.ForegroundColor = corBorda;
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        public static void DesenharSeparador(int largura = 0, ConsoleColor cor = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = cor;
            Console.WriteLine("├" + new string('─', Math.Max(0, largura - 2)) + "┤");
            Console.ResetColor();
        }

        public static void DesenharFimSecao(int largura = 0, ConsoleColor cor = ConsoleColor.DarkCyan)
        {
            if (largura <= 0) largura = LarguraAtual;

            Console.ForegroundColor = cor;
            Console.WriteLine("└" + new string('─', Math.Max(0, largura - 2)) + "┘");
            Console.ResetColor();
        }

        public static void DesenharBarraProgresso(string rotulo, int atual, int maximo, int tamanhoBarra = 24, ConsoleColor corBarra = ConsoleColor.Green)
        {
            if (maximo <= 0) maximo = 1;
            if (atual < 0) atual = 0;
            if (atual > maximo) atual = maximo;

            double porcentagem = (double)atual / maximo;
            int preenchidos = (int)Math.Round(porcentagem * tamanhoBarra);
            int vazios = tamanhoBarra - preenchidos;

            Console.Write($"{rotulo,-12} [");

            ConsoleColor corDinamica = corBarra;
            if (rotulo.Contains("HP", StringComparison.OrdinalIgnoreCase) || rotulo.Contains("Casco", StringComparison.OrdinalIgnoreCase) || rotulo.Contains("Vida", StringComparison.OrdinalIgnoreCase))
            {
                if (porcentagem > 0.5) corDinamica = ConsoleColor.Green;
                else if (porcentagem > 0.25) corDinamica = ConsoleColor.Yellow;
                else corDinamica = ConsoleColor.Red;
            }

            Console.ForegroundColor = corDinamica;
            Console.Write(new string('█', preenchidos));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', vazios));
            Console.ResetColor();

            Console.Write($"] {atual}/{maximo}\n");
        }

        public static string ObterBarraCompacta(int atual, int maximo, int tamanho = 10)
        {
            if (maximo <= 0) maximo = 1;
            if (atual < 0) atual = 0;
            if (atual > maximo) atual = maximo;

            if (tamanho < 4) tamanho = 4;

            double porcentagem = (double)atual / maximo;
            int preenchidos = (int)Math.Round(porcentagem * tamanho);
            int vazios = tamanho - preenchidos;

            return $"[{new string('█', preenchidos)}{new string('░', vazios)}] {atual}/{maximo}";
        }

        public static string CentralizarTexto(string texto, int largura)
        {
            if (string.IsNullOrEmpty(texto)) return new string(' ', Math.Max(0, largura));
            if (texto.Length >= largura) return texto.Substring(0, largura);

            int espacosEsquerda = (largura - texto.Length) / 2;
            int espacosDireita = largura - texto.Length - espacosEsquerda;

            return new string(' ', espacosEsquerda) + texto + new string(' ', espacosDireita);
        }

        public static string TruncarOuPad(string texto, int largura)
        {
            if (string.IsNullOrEmpty(texto)) return new string(' ', Math.Max(0, largura));
            if (texto.Length > largura) return texto.Substring(0, largura);
            return texto.PadRight(largura);
        }

        /// <summary>
        /// Desenha 3 cards de combatentes lado a lado (estilo 3v3) com molduras contínuas em Unicode,
        /// barras de HP, afinidades e previews de ação, calculando a largura dinamicamente.
        /// </summary>
        public static void DesenharTresCardsCombatentes(
            Slot[] slots,
            Combatente combatenteAtivo,
            bool saoInimigos,
            Slot[] slotsAdversarios = null,
            Combatente combatenteHovered = null,
            int larguraCard = 0)
        {
            if (larguraCard <= 0)
            {
                larguraCard = Math.Max(24, (LarguraAtual - 8) / 3);
            }

            int espacoInterno = Math.Max(0, larguraCard - 4);
            List<string[]> cardsLinhas = new List<string[]>();

            for (int i = 0; i < 3; i++)
            {
                var slot = (slots != null && i < slots.Length) ? slots[i] : null;
                var c = slot?.Combatente;
                bool ativo = (c != null && c == combatenteAtivo);
                bool hovered = (c != null && c == combatenteHovered);
                string[] linhas = new string[10];

                if (c == null || c.EstaMorto)
                {
                    string tagStatus = (c == null) ? "-- VAZIO --" : "-- ABATIDO --";
                    linhas[0] = "┌" + new string('─', larguraCard - 2) + "┐";
                    linhas[1] = "│ " + TruncarOuPad($"[SLOT {i + 1}] {tagStatus}", espacoInterno) + " │";
                    linhas[2] = "│ " + CentralizarTexto(".---.", espacoInterno) + " │";
                    linhas[3] = "│ " + CentralizarTexto("/ X X \\", espacoInterno) + " │";
                    linhas[4] = "│ " + CentralizarTexto("|   ~   |", espacoInterno) + " │";
                    linhas[5] = "│ " + CentralizarTexto("\\_____/", espacoInterno) + " │";
                    linhas[6] = "├" + new string('─', larguraCard - 2) + "┤";
                    linhas[7] = "│ " + TruncarOuPad("Inativo no combate", espacoInterno) + " │";
                    linhas[8] = "│ " + TruncarOuPad("", espacoInterno) + " │";
                    linhas[9] = "└" + new string('─', larguraCard - 2) + "┘";
                }
                else
                {
                    string[] sprite = BancoSprites.ObterSprite(c.Nome);
                    string tagStatus;
                    if (hovered)
                        tagStatus = "> ALVO MIRA <";
                    else if (ativo)
                        tagStatus = "[ATIVO]";
                    else
                        tagStatus = saoInimigos ? "INIMIGO" : "TRIPULANTE";

                    string tituloCard = $"[S{i + 1}] {c.Nome} ({tagStatus})";
                    int tamanhoBarra = Math.Max(4, Math.Min(8, espacoInterno - 14));
                    string barraHp = ObterBarraCompacta(c.VidaAtual, c.VidaTotal, tamanhoBarra);
                    string afinidadeBadge = $"Def:[{c.Afinidade}]";

                    string statusLinha;
                    if (saoInimigos)
                    {
                        if (hovered)
                        {
                            statusLinha = "►► MIRA TRAVADA [ALVO SELECIONADO] ◄◄";
                        }
                        else if (!slot.JaAtacouNestaRodada && !c.EstaMorto && slot.HabilidadePlanejada != null && slot.AlvoPlanejadoSlot >= 0 && slotsAdversarios != null && slot.AlvoPlanejadoSlot < slotsAdversarios.Length)
                        {
                            var alvo = slotsAdversarios[slot.AlvoPlanejadoSlot]?.Combatente;
                            string nomeAlvo = alvo != null ? alvo.Nome : $"Slot {slot.AlvoPlanejadoSlot + 1}";
                            statusLinha = $"Mira:>>{nomeAlvo}<< [{slot.HabilidadePlanejada.Afinidade}]";
                        }
                        else
                        {
                            statusLinha = $"Def:{c.Defesa} | Agi:{c.Agilidade}";
                        }
                    }
                    else
                    {
                        string modInfo = c switch
                        {
                            Sentinela s => $"Adren: {s.Adrenalina}/45",
                            Engenheiro eng => $"Aquec: {eng.Sobreaquecimento}/45",
                            Biomancer bio => $"Mana: {bio.Mana}/45",
                            _ => $"Def:{c.Defesa}"
                        };
                        statusLinha = $"Def:{c.Defesa} | Agi:{c.Agilidade} | {modInfo}";
                    }

                    bool destaque = (hovered || ativo);
                    char charBordaH = destaque ? '═' : '─';
                    char charBordaV = destaque ? '║' : '│';
                    char charCantoTL = destaque ? '╔' : '┌';
                    char charCantoTR = destaque ? '╗' : '┐';
                    char charCantoBL = destaque ? '╚' : '└';
                    char charCantoBR = destaque ? '╝' : '┘';
                    char charDivL = destaque ? '╠' : '├';
                    char charDivR = destaque ? '╣' : '┤';

                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad(tituloCard, espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + CentralizarTexto(sprite.Length > 0 ? sprite[0] : "", espacoInterno) + $" {charBordaV}";
                    linhas[3] = $"{charBordaV} " + CentralizarTexto(sprite.Length > 1 ? sprite[1] : "", espacoInterno) + $" {charBordaV}";
                    linhas[4] = $"{charBordaV} " + CentralizarTexto(sprite.Length > 2 ? sprite[2] : "", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + CentralizarTexto(sprite.Length > 3 ? sprite[3] : "", espacoInterno) + $" {charBordaV}";
                    linhas[6] = charDivL + new string(charBordaH, larguraCard - 2) + charDivR;
                    linhas[7] = $"{charBordaV} " + TruncarOuPad($"{barraHp} | {afinidadeBadge}", espacoInterno) + $" {charBordaV}";
                    linhas[8] = $"{charBordaV} " + TruncarOuPad(statusLinha, espacoInterno) + $" {charBordaV}";
                    linhas[9] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }

                cardsLinhas.Add(linhas);
            }

            for (int l = 0; l < 10; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < 3; c++)
                {
                    bool ativo = (slots != null && c < slots.Length && slots[c]?.Combatente == combatenteAtivo);
                    bool hovered = (slots != null && c < slots.Length && slots[c]?.Combatente == combatenteHovered);

                    if (hovered)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (ativo)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (saoInimigos)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    }

                    Console.Write(cardsLinhas[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Desenha 3 cards de itens lado a lado para a tela de exploração e escolha de espólios com linhas contínuas Unicode.
        /// </summary>
        public static void DesenharTresCardsItens(List<Item> itens, int indiceSelecionado, int larguraCard = 0)
        {
            if (itens == null || itens.Count == 0) return;

            if (larguraCard <= 0)
            {
                larguraCard = Math.Max(24, (LarguraAtual - 8) / 3);
            }

            int espacoInterno = Math.Max(0, larguraCard - 4);
            List<string[]> cardsLinhas = new List<string[]>();

            for (int i = 0; i < 3; i++)
            {
                string[] linhas = new string[10];
                bool selecionado = (i == indiceSelecionado);

                char charBordaH = selecionado ? '═' : '─';
                char charBordaV = selecionado ? '║' : '│';
                char charCantoTL = selecionado ? '╔' : '┌';
                char charCantoTR = selecionado ? '╗' : '┐';
                char charCantoBL = selecionado ? '╚' : '└';
                char charCantoBR = selecionado ? '╝' : '┘';

                if (i >= itens.Count)
                {
                    linhas[0] = "┌" + new string('─', larguraCard - 2) + "┐";
                    for (int l = 1; l < 9; l++) linhas[l] = "│ " + new string(' ', espacoInterno) + " │";
                    linhas[9] = "└" + new string('─', larguraCard - 2) + "┘";
                }
                else
                {
                    var item = itens[i];
                    string[] spriteItem = BancoSprites.ObterArteItem(item.Tipo);
                    string raridadeStr = $"[{item.Raridade.ToString().ToUpper()}]";

                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad($"[OPÇÃO {i + 1}] {item.Nome}", espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + CentralizarTexto(spriteItem.Length > 0 ? spriteItem[0] : "", espacoInterno) + $" {charBordaV}";
                    linhas[3] = $"{charBordaV} " + CentralizarTexto(spriteItem.Length > 1 ? spriteItem[1] : "", espacoInterno) + $" {charBordaV}";
                    linhas[4] = $"{charBordaV} " + CentralizarTexto(spriteItem.Length > 2 ? spriteItem[2] : "", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + CentralizarTexto(spriteItem.Length > 3 ? spriteItem[3] : "", espacoInterno) + $" {charBordaV}";
                    linhas[6] = $"{charBordaV} " + TruncarOuPad($"Raridade: {raridadeStr} | {item.Tipo}", espacoInterno) + $" {charBordaV}";
                    linhas[7] = $"{charBordaV} " + TruncarOuPad($"Efeito: +{item.ValorEfeito}", espacoInterno) + $" {charBordaV}";
                    linhas[8] = $"{charBordaV} " + TruncarOuPad(selecionado ? ">> [ ESCOLHER ESTE ITEM ] <<" : "   [ Pressione ENTER/1-3 ]   ", espacoInterno) + $" {charBordaV}";
                    linhas[9] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }

                cardsLinhas.Add(linhas);
            }

            for (int l = 0; l < 10; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < 3; c++)
                {
                    bool selecionado = (c == indiceSelecionado);
                    Console.ForegroundColor = selecionado ? ConsoleColor.Green : ConsoleColor.DarkCyan;
                    Console.Write(cardsLinhas[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Desenha um painel dividido com bordas contínuas em Unicode: coluna esquerda com arte do cenário
        /// e coluna direita com telemetria e transmissão narrativa.
        /// </summary>
        public static void DesenharPainelCenarioComTexto(
            string[] arteCenario,
            string tituloCenario,
            List<string> linhasTexto,
            string tituloTexto,
            ConsoleColor corCenario = ConsoleColor.Cyan,
            ConsoleColor corTexto = ConsoleColor.White,
            ConsoleColor corBorda = ConsoleColor.DarkCyan)
        {
            int larguraTotal = LarguraAtual;
            int larguraEsq = Math.Max(28, Math.Min(44, (int)(larguraTotal * 0.28)));
            int larguraDir = Math.Max(40, larguraTotal - larguraEsq - 4);
            const int totalLinhas = 9;

            // Formatação do título do Radar / Cenário
            string tagEsq = $"[ {tituloCenario} ]";
            int maxTagEsq = Math.Max(0, larguraEsq - 6);
            string safeTagEsq = (tagEsq.Length > maxTagEsq) ? tagEsq.Substring(0, maxTagEsq) : tagEsq;
            int tracosEsq = Math.Max(0, larguraEsq - 5 - safeTagEsq.Length);

            // Formatação do título da Telemetria
            string tagDir = $"[ {tituloTexto} ]";
            int maxTagDir = Math.Max(0, larguraDir - 6);
            string safeTagDir = (tagDir.Length > maxTagDir) ? tagDir.Substring(0, maxTagDir) : tagDir;
            int tracosDir = Math.Max(0, larguraDir - 5 - safeTagDir.Length);

            // Renderiza as 9 linhas lado a lado
            for (int l = 0; l < totalLinhas; l++)
            {
                // ================= COLUNA ESQUERDA =================
                Console.Write(" ");
                if (l == 0)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("┌─ ");
                    Console.ForegroundColor = corCenario;
                    Console.Write(safeTagEsq);
                    Console.ForegroundColor = corBorda;
                    Console.Write(" " + new string('─', tracosEsq) + "┐");
                }
                else if (l == 8)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("└" + new string('─', larguraEsq - 2) + "┘");
                }
                else
                {
                    string linhaArte = (arteCenario != null && (l - 1) < arteCenario.Length) ? arteCenario[l - 1] : "";
                    Console.ForegroundColor = corBorda;
                    Console.Write("│ ");
                    Console.ForegroundColor = corCenario;
                    Console.Write(CentralizarTexto(linhaArte, larguraEsq - 4));
                    Console.ForegroundColor = corBorda;
                    Console.Write(" │");
                }

                Console.Write(" ");

                // ================= COLUNA DIREITA =================
                if (l == 0)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("┌─ ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(safeTagDir);
                    Console.ForegroundColor = corBorda;
                    Console.Write(" " + new string('─', tracosDir) + "┐");
                }
                else if (l == 8)
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write("└" + new string('─', larguraDir - 2) + "┘");
                }
                else
                {
                    string linhaInfo = (linhasTexto != null && (l - 1) < linhasTexto.Count) ? linhasTexto[l - 1] : "";
                    Console.ForegroundColor = corBorda;
                    Console.Write("│ ");

                    if (l == 7)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = corTexto;
                    }

                    Console.Write(TruncarOuPad(linhaInfo, larguraDir - 4));
                    Console.ForegroundColor = corBorda;
                    Console.Write(" │");
                }

                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Desenha um dossiê do personagem com bordas contínuas Unicode: coluna esquerda com retrato ASCII e coluna direita com atributos.
        /// </summary>
        public static void DesenharDossierPersonagemComArte(Combatente c, ConsoleColor corTema = ConsoleColor.Cyan)
        {
            if (c == null) return;

            string[] retrato = BancoSprites.ObterRetratoGrande(c.Nome);
            int larguraTotal = LarguraAtual;
            int larguraEsq = Math.Max(28, Math.Min(38, (int)(larguraTotal * 0.28)));
            int larguraDir = Math.Max(38, larguraTotal - larguraEsq - 4);
            const int totalLinhas = 11;

            // Detalhes do Combatente
            int tamBarra = Math.Max(6, Math.Min(14, larguraDir / 8));
            string barraHp = ObterBarraCompacta(c.VidaAtual, c.VidaTotal, tamBarra);

            string modInfo = c switch
            {
                Sentinela s => $"Adrenalina Tática: {s.Adrenalina}/45 (Eleva chance de moedas no embate)",
                Engenheiro eng => $"Sobreaquecimento: {eng.Sobreaquecimento}/45 (Eleva chance de moedas no embate)",
                Biomancer bio => $"Mana Arcana:       {bio.Mana}/45 (Gasta no combate)",
                _ => $"Defesa: {c.Defesa}"
            };

            List<string> dados = new List<string>
            {
                $"Classe: {c.GetType().Name,-14} | Nome de Batalha: {c.Nome}",
                $"Nível de Experiência: {c.Level,2}/10 | EXP Atual: {c.Exp}",
                $"Integridade Física (HP): {barraHp}",
                $"{modInfo}",
                $"Defesa Base: {c.Defesa,2} | Agilidade / Iniciativa: {c.Agilidade,2}",
                $"Blindagem Defensiva: [{c.Afinidade.ToString().ToUpper()}] (Ácido vs Armadurado: 2x | Choque vs Mecânico: 2x | Fogo vs Bio: 2x)",
                $"Habilidades no Baralho: {c.Habilidades.Count} cartas ativas",
                $"Habilidades Prontas para Ação: {c.HabilidadesDisponiveis.Count} cartas"
            };

            for (int l = 0; l < totalLinhas; l++)
            {
                // Coluna Esquerda
                Console.Write(" ");
                if (l == 0)
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("┌" + new string('─', larguraEsq - 2) + "┐");
                }
                else if (l == 10)
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("└" + new string('─', larguraEsq - 2) + "┘");
                }
                else
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("│ ");
                    if (l == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(TruncarOuPad($"RETRATO: {c.Nome.ToUpper()}", larguraEsq - 4));
                    }
                    else
                    {
                        string linhaRetrato = (retrato != null && (l - 2) < retrato.Length) ? retrato[l - 2] : "";
                        Console.ForegroundColor = corTema;
                        Console.Write(CentralizarTexto(linhaRetrato, larguraEsq - 4));
                    }
                    Console.ForegroundColor = corTema;
                    Console.Write(" │");
                }

                Console.Write("  ");

                // Coluna Direita
                if (l == 0)
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("┌" + new string('─', larguraDir - 2) + "┐");
                }
                else if (l == 10)
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("└" + new string('─', larguraDir - 2) + "┘");
                }
                else
                {
                    Console.ForegroundColor = corTema;
                    Console.Write("│ ");
                    if (l == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(TruncarOuPad("FICHA TÁTICA DO COMBATENTE - NAVE VANGUARDA", larguraDir - 4));
                    }
                    else
                    {
                        string linha = ((l - 2) < dados.Count) ? dados[l - 2] : "";
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(TruncarOuPad(linha, larguraDir - 4));
                    }
                    Console.ForegroundColor = corTema;
                    Console.Write(" │");
                }

                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Desenha a grade de cartas de habilidades lado a lado (1 linha de cartas),
        /// com a carta selecionada expandida com todos os detalhes e as demais em formato compacto.
        /// </summary>
        public static void DesenharGridHabilidadesCards(
            List<Habilidade> todasHabilidades,
            List<Habilidade> equipadas,
            int indiceSelecionado,
            int larguraDisponivel = 0)
        {
            if (todasHabilidades == null || todasHabilidades.Count == 0) return;

            int totalCartas = todasHabilidades.Count;
            if (larguraDisponivel <= 0) larguraDisponivel = LarguraAtual;

            int espacosSeparadores = (totalCartas - 1) + 2;
            int larguraUtil = Math.Max(60, larguraDisponivel - espacosSeparadores);

            int larguraNaoSel = Math.Max(12, Math.Min(16, (larguraUtil - 38) / Math.Max(1, totalCartas - 1)));
            int larguraSel = Math.Max(34, larguraUtil - (larguraNaoSel * (totalCartas - 1)));

            const int totalLinhas = 8;
            List<string[]> cardsLinhas = new List<string[]>();
            List<ConsoleColor> coresCartas = new List<ConsoleColor>();

            for (int i = 0; i < totalCartas; i++)
            {
                var hab = todasHabilidades[i];
                bool ehEquipada = (equipadas != null && equipadas.Contains(hab));
                bool selecionada = (i == indiceSelecionado);
                int larguraCard = selecionada ? larguraSel : larguraNaoSel;
                int espacoInterno = Math.Max(0, larguraCard - 4);

                string[] linhas = new string[totalLinhas];

                ConsoleColor corCard = selecionada ? ConsoleColor.Yellow : (ehEquipada ? ConsoleColor.Green : ConsoleColor.DarkGray);
                coresCartas.Add(corCard);

                char charBordaH = selecionada ? '═' : '─';
                char charBordaV = selecionada ? '║' : '│';
                char charCantoTL = selecionada ? '╔' : '┌';
                char charCantoTR = selecionada ? '╗' : '┐';
                char charCantoBL = selecionada ? '╚' : '└';
                char charCantoBR = selecionada ? '╝' : '┘';
                char charDivL = selecionada ? '╠' : '├';
                char charDivR = selecionada ? '╣' : '┤';

                string tagCat = hab.Categoria switch
                {
                    CategoriaHabilidade.Basica => "BAS",
                    CategoriaHabilidade.Avancada => "AVN",
                    CategoriaHabilidade.Especialista => "ESP",
                    _ => "HAB"
                };

                if (selecionada)
                {
                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad($"►► [{i + 1}] {hab.Nome.ToUpper()} [{tagCat}]", espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + TruncarOuPad($"Modificador: {hab.Modificador,2} | Afinidade: [{hab.Afinidade}]", espacoInterno) + $" {charBordaV}";
                    linhas[3] = charDivL + new string(charBordaH, larguraCard - 2) + charDivR;
                    linhas[4] = $"{charBordaV} " + TruncarOuPad($"Poder Base: {hab.PoderBase,2} | Moedas: {hab.Moeda}x (+{hab.PoderAdicionalMoeda}/cara)", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + TruncarOuPad($"Efeito: {hab.Descricao}", espacoInterno) + $" {charBordaV}";
                    linhas[6] = $"{charBordaV} " + TruncarOuPad(ehEquipada ? ">> [ HABILIDADE EQUIPADA ] <<" : ">> [ HABILIDADE NO BARALHO ] <<", espacoInterno) + $" {charBordaV}";
                    linhas[7] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }
                else
                {
                    string statusComp = ehEquipada ? "[EQUIP]" : "[DISP]";
                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad($"[{i + 1}] {hab.Nome}", espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + TruncarOuPad($"[{hab.Afinidade}] {statusComp}", espacoInterno) + $" {charBordaV}";
                    linhas[3] = charDivL + new string(charBordaH, larguraCard - 2) + charDivR;
                    linhas[4] = $"{charBordaV} " + CentralizarTexto("░░░░░░", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + CentralizarTexto("░░░░░░", espacoInterno) + $" {charBordaV}";
                    linhas[6] = $"{charBordaV} " + CentralizarTexto(ehEquipada ? "[ATIVA]" : "[DISP]", espacoInterno) + $" {charBordaV}";
                    linhas[7] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }

                cardsLinhas.Add(linhas);
            }

            for (int l = 0; l < totalLinhas; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < totalCartas; c++)
                {
                    Console.ForegroundColor = coresCartas[c];
                    Console.Write(cardsLinhas[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Desenha a mão de cartas de combate em formato horizontal lado a lado.
        /// As cartas não selecionadas aparecem em formato compacto mostrando o topo com nome e afinidade,
        /// enquanto a carta atualmente selecionada se expande exibindo todos os detalhes táticos.
        /// </summary>
        public static void DesenharMaoCartasCombate(
            Combatente aliado,
            int indiceSelecionado,
            int larguraDisponivel = 0)
        {
            if (aliado == null || aliado.Habilidades == null || aliado.Habilidades.Count == 0) return;

            var cartas = aliado.Habilidades;
            int totalCartas = cartas.Count;
            if (totalCartas == 0) return;

            if (larguraDisponivel <= 0)
            {
                larguraDisponivel = LarguraAtual;
            }

            int espacosSeparadores = (totalCartas - 1) + 2;
            int larguraUtil = Math.Max(60, larguraDisponivel - espacosSeparadores);

            int larguraNaoSel = Math.Max(12, Math.Min(16, (larguraUtil - 38) / Math.Max(1, totalCartas - 1)));
            int larguraSel = Math.Max(34, larguraUtil - (larguraNaoSel * (totalCartas - 1)));

            const int totalLinhas = 8;
            List<string[]> cardsLinhas = new List<string[]>();
            List<ConsoleColor> coresCartas = new List<ConsoleColor>();

            for (int i = 0; i < totalCartas; i++)
            {
                var hab = cartas[i];
                bool disponivel = aliado.HabilidadesDisponiveis.Exists(h => h.Id == hab.Id && h.Moeda > 0);
                bool selecionada = (i == indiceSelecionado);
                int larguraCard = selecionada ? larguraSel : larguraNaoSel;
                int espacoInterno = Math.Max(0, larguraCard - 4);

                string[] linhas = new string[totalLinhas];

                ConsoleColor corCard;
                if (!disponivel)
                {
                    corCard = ConsoleColor.DarkGray;
                }
                else if (selecionada)
                {
                    corCard = ConsoleColor.Yellow;
                }
                else
                {
                    corCard = hab.Afinidade switch
                    {
                        AfinidadeAtaque.Fogo => ConsoleColor.Red,
                        AfinidadeAtaque.Eletrico => ConsoleColor.DarkYellow,
                        AfinidadeAtaque.Acido => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    };
                }
                coresCartas.Add(corCard);

                char charBordaH = selecionada ? '═' : '─';
                char charBordaV = selecionada ? '║' : '│';
                char charCantoTL = selecionada ? '╔' : '┌';
                char charCantoTR = selecionada ? '╗' : '┐';
                char charCantoBL = selecionada ? '╚' : '└';
                char charCantoBR = selecionada ? '╝' : '┘';
                char charDivL = selecionada ? '╠' : '├';
                char charDivR = selecionada ? '╣' : '┤';

                string tagCat = hab.Categoria switch
                {
                    CategoriaHabilidade.Basica => "BAS",
                    CategoriaHabilidade.Avancada => "AVN",
                    CategoriaHabilidade.Especialista => "ESP",
                    _ => "HAB"
                };

                if (selecionada)
                {
                    string statusCarta;
                    if (!disponivel) statusCarta = ">> [ HABILIDADE ESGOTADA NESTA RODADA ] <<";
                    else statusCarta = ">> [ PRONTA / ENTER P/ JOGAR NO EMBATE ] <<";

                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad($"►► [{i + 1}] {hab.Nome.ToUpper()} [{tagCat}]", espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + TruncarOuPad($"Modificador: {hab.Modificador,2} | Afinidade: [{hab.Afinidade}]", espacoInterno) + $" {charBordaV}";
                    linhas[3] = charDivL + new string(charBordaH, larguraCard - 2) + charDivR;
                    linhas[4] = $"{charBordaV} " + TruncarOuPad($"Poder Base: {hab.PoderBase,2} | Moedas: {hab.Moeda}x (+{hab.PoderAdicionalMoeda}/cara)", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + TruncarOuPad($"Efeito: {hab.Descricao}", espacoInterno) + $" {charBordaV}";
                    linhas[6] = $"{charBordaV} " + TruncarOuPad(statusCarta, espacoInterno) + $" {charBordaV}";
                    linhas[7] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }
                else
                {
                    string statusComp;
                    if (!disponivel) statusComp = "[ESGOTADA]";
                    else statusComp = $"[{hab.Afinidade.ToString().Substring(0, Math.Min(3, hab.Afinidade.ToString().Length)).ToUpper()}]";

                    linhas[0] = charCantoTL + new string(charBordaH, larguraCard - 2) + charCantoTR;
                    linhas[1] = $"{charBordaV} " + TruncarOuPad($"[{i + 1}] {hab.Nome}", espacoInterno) + $" {charBordaV}";
                    linhas[2] = $"{charBordaV} " + TruncarOuPad($"Mod:{hab.Modificador,2} {statusComp}", espacoInterno) + $" {charBordaV}";
                    linhas[3] = charDivL + new string(charBordaH, larguraCard - 2) + charDivR;
                    linhas[4] = $"{charBordaV} " + CentralizarTexto("░░░░░░", espacoInterno) + $" {charBordaV}";
                    linhas[5] = $"{charBordaV} " + CentralizarTexto("░░░░░░", espacoInterno) + $" {charBordaV}";
                    linhas[6] = $"{charBordaV} " + CentralizarTexto(disponivel ? "[DISP]" : "[GASTA]", espacoInterno) + $" {charBordaV}";
                    linhas[7] = charCantoBL + new string(charBordaH, larguraCard - 2) + charCantoBR;
                }

                cardsLinhas.Add(linhas);
            }

            for (int l = 0; l < totalLinhas; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < totalCartas; c++)
                {
                    Console.ForegroundColor = coresCartas[c];
                    Console.Write(cardsLinhas[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
}
