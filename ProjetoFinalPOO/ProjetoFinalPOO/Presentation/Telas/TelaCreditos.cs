using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Diário de Bordo, Lore, Classes e Regras do Sistema (atende a todos os requisitos do README).
    /// </summary>
    public class TelaCreditos : ITela
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
            RenderizadorUI.DesenharCabecalho("DIÁRIO DE BORDO, TRIPULAÇÃO, AFINIDADES & REGRAS TÁTICAS (README)", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();

            DesenharHistoriaLore();
            DesenharClassesTripulacao();
            DesenharTabelaAfinidades();
            DesenharMecanicasCombate();
            DesenharRodape();
        }

        private void DesenharHistoriaLore()
        {
            RenderizadorUI.DesenharInicioSecao("NARRATIVA & MISSÃO MERCENÁRIA (README 1, 3 e 4)", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaConteudo("Você lidera um esquadrão mercenário a bordo da nave 'Vanguarda' através de um mapa estelar de 6 galáxias.", RenderizadorUI.LarguraPadrao, ConsoleColor.White, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaConteudo("Sua missão é escoltar uma pessoa misteriosa (Carga 73) para fora do território controlado pela Frota Sindical.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaConteudo("O confronto final ocorre na Galáxia 6 contra a nave capitânia Dreadnought e seu time de elite.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkYellow);
            Console.WriteLine();
        }

        private void DesenharClassesTripulacao()
        {
            RenderizadorUI.DesenharInicioSecao("CLASSES DE COMBATENTES & PROGRESSÃO ATÉ NÍVEL 10 (README 7, 9, 27 e 28)", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);

            int larguraCard = Math.Max(24, (RenderizadorUI.LarguraAtual - 8) / 3);
            int espacoInterno = Math.Max(0, larguraCard - 4);
            List<string[]> cards = new List<string[]>();

            string[][] sprites = new string[][]
            {
                BancoSprites.ObterSprite("optimus"),
                BancoSprites.ObterSprite("asimov"),
                BancoSprites.ObterSprite("pasteur")
            };

            string[] titulos = new string[] { "1. SENTINELA [OPTIMUS]", "2. ENGENHEIRO [ASIMOV]", "3. BIOMANCER [PASTEUR]" };
            string[] papeis = new string[] { "Função: Vanguarda / Adrenalina", "Função: Suporte / Superaquecimento", "Função: Bio-Místico / Mana" };
            string[] blindagens = new string[] { "Blindagem: [ARMADURA] (Ácido 2x)", "Blindagem: [MECÂNICO] (Choque 2x)", "Blindagem: [BIOLÓGICO] (Fogo 2x)" };
            string[] habs = new string[] { "6 Habilidades (Impacto/Fogo/Ácido)", "6 Habilidades (Nanorobôs/Elétrico)", "6 Habilidades (Cáustico/Bio-Vórtice)" };

            for (int i = 0; i < 3; i++)
            {
                string[] linhas = new string[10];
                linhas[0] = "┌" + new string('─', larguraCard - 2) + "┐";
                linhas[1] = "│ " + RenderizadorUI.TruncarOuPad(titulos[i], espacoInterno) + " │";
                linhas[2] = "│ " + RenderizadorUI.CentralizarTexto(sprites[i].Length > 0 ? sprites[i][0] : "", espacoInterno) + " │";
                linhas[3] = "│ " + RenderizadorUI.CentralizarTexto(sprites[i].Length > 1 ? sprites[i][1] : "", espacoInterno) + " │";
                linhas[4] = "│ " + RenderizadorUI.CentralizarTexto(sprites[i].Length > 2 ? sprites[i][2] : "", espacoInterno) + " │";
                linhas[5] = "│ " + RenderizadorUI.CentralizarTexto(sprites[i].Length > 3 ? sprites[i][3] : "", espacoInterno) + " │";
                linhas[6] = "│ " + RenderizadorUI.TruncarOuPad(papeis[i], espacoInterno) + " │";
                linhas[7] = "│ " + RenderizadorUI.TruncarOuPad(blindagens[i], espacoInterno) + " │";
                linhas[8] = "│ " + RenderizadorUI.TruncarOuPad(habs[i], espacoInterno) + " │";
                linhas[9] = "└" + new string('─', larguraCard - 2) + "┘";
                cards.Add(linhas);
            }

            for (int l = 0; l < 10; l++)
            {
                Console.Write(" ");
                for (int c = 0; c < 3; c++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(cards[c][l]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private void DesenharTabelaAfinidades()
        {
            RenderizadorUI.DesenharInicioSecao("MATRIZ DE AFINIDADES DE ATAQUE E DEFESA (README 23, 24, 25 e 26)", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkMagenta);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM ARMADURA : Fraca contra ÁCIDO (2.0x)  | Neutra contra ELÉTRICO (1.0x) | Forte contra FOGO (0.5x)", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow, ConsoleColor.DarkMagenta);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM MECÂNICO : Fraco contra ELÉTRICO (2.0x) | Neutro contra FOGO (1.0x)     | Forte contra ÁCIDO (0.5x)", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan, ConsoleColor.DarkMagenta);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM BIOLÓGICO: Fraco contra FOGO (2.0x)     | Neutro contra ÁCIDO (1.0x)    | Forte contra ELÉTRICO (0.5x)", RenderizadorUI.LarguraPadrao, ConsoleColor.Green, ConsoleColor.DarkMagenta);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkMagenta);
            Console.WriteLine();
        }

        private void DesenharMecanicasCombate()
        {
            RenderizadorUI.DesenharInicioSecao("SISTEMA DE COMBATE TÁTICO INSPIRADO EM LIMBUS COMPANY (README 5, 6, 14, 16-22)", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Iniciativa por Velocidade: Ordem de ação nos turnos definida pela velocidade dos combatentes.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Preview de Ações: Os sensores exibem os alvos e habilidades planejadas pelos inimigos antes da decisão.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Ações do Jogador: [1] Atacar com Cartas Equipadas, [2] Defender (ergue escudo), [3] Usar Itens do inventário.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Embate (Clash): Confronto direto onde moedas (Cara/Coroa) influenciadas pelos recursos de classe decidem o vencedor.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Ataque Unilateral: Se o alvo já atacou nesta rodada, recebe o ataque direto sem embate de defesa.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Áreas de Descanso: Acesso à armeria para trocar o conjunto de habilidades equipadas da tripulação.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - Exploração & Espólios: Em planetas explorados ou após batalhas, escolha 1 item entre 3 para sua carga.", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private void DesenharRodape()
        {
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
