using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.EncontroPlaneta;
using ProjetoFinalPOO.Mapa;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Modo de visualização do painel da Tela do Mapa Estelar.
    /// </summary>
    public enum ModoVisualizacaoMapa
    {
        NavegacaoRotas,
        StatusTripulacaoInventario,
        HistoricoDeVoo
    }

    /// <summary>
    /// Tela interativa de navegação pelo Mapa Estelar (Grafo 2D estilo Slay the Spire / FTL).
    /// Renderiza visualmente as 6 Galáxias x 3 Faixas orbitais, nós com ícones temáticos de encontro,
    /// hipervias de salto, indicador da nave Vanguarda, scanner holográfico do planeta alvo e telemetria.
    /// </summary>
    public class TelaMapa : ITela
    {
        private readonly Mapa.Mapa _mapa;
        private readonly Grafo _grafo;
        private readonly List<Combatente> _tripulacao;
        private readonly List<Item> _inventarioEquipe;

        private Vertice _verticeAtual;
        private readonly List<Vertice> _historicoCaminho;
        private int _indiceArestaSelecionada;
        private ModoVisualizacaoMapa _modoVisualizacao;
        private int _totalSaltosRealizados;
        private int _combatesVencidos;
        private int _danoTotalCausado;
        private int _creditosEspaciais;
        private int _integridadeNave;

        public TelaMapa(
            Mapa.Mapa mapa,
            Grafo grafo = null,
            List<Combatente> tripulacao = null,
            List<Item> inventarioEquipe = null)
        {
            _mapa = mapa ?? throw new ArgumentNullException(nameof(mapa));
            _grafo = grafo ?? new MapaRPGBuilder()
                .GerarInicio()
                .AdicionarGalaxia(1)
                .AdicionarGalaxia(2)
                .AdicionarGalaxia(3)
                .AdicionarGalaxia(4)
                .AdicionarGalaxia(5)
                .AdicionarGalaxia(6)
                .Construir();

            // Inicializa tripulação pré-definida de 3 combatentes (README 28)
            _tripulacao = tripulacao ?? new List<Combatente>
            {
                BancoHabilidades.CriarSentinela(),
                BancoHabilidades.CriarEngenheiro(),
                BancoHabilidades.CriarBiomancer()
            };

            // Inicializa inventário
            _inventarioEquipe = inventarioEquipe ?? BancoItens.ObterInventarioInicial();

            // Inicializa a nave no vértice inicial ("Inicio")
            _verticeAtual = _grafo.Vertices.Find(v => v.Nome.Equals("Inicio", StringComparison.OrdinalIgnoreCase))
                            ?? _grafo.Vertices.FirstOrDefault()
                            ?? new Vertice("Inicio");

            _historicoCaminho = new List<Vertice> { _verticeAtual };
            _indiceArestaSelecionada = 0;
            _modoVisualizacao = ModoVisualizacaoMapa.NavegacaoRotas;
            _totalSaltosRealizados = 0;
            _combatesVencidos = 0;
            _danoTotalCausado = 0;
            _creditosEspaciais = 200;
            _integridadeNave = 100;
        }

        public void Entrar()
        {
            _indiceArestaSelecionada = 0;
        }

        public void Atualizar() { }
        public void Sair() { }

        public void Executar()
        {
            bool emViagem = true;

            while (emViagem)
            {
                Renderizar();
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        MoverSelecaoCima();
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        MoverSelecaoBaixo();
                        break;

                    case ConsoleKey.I:
                        _modoVisualizacao = _modoVisualizacao == ModoVisualizacaoMapa.StatusTripulacaoInventario
                            ? ModoVisualizacaoMapa.NavegacaoRotas
                            : ModoVisualizacaoMapa.StatusTripulacaoInventario;
                        break;

                    case ConsoleKey.H:
                        _modoVisualizacao = _modoVisualizacao == ModoVisualizacaoMapa.HistoricoDeVoo
                            ? ModoVisualizacaoMapa.NavegacaoRotas
                            : ModoVisualizacaoMapa.HistoricoDeVoo;
                        break;

                    case ConsoleKey.Tab:
                    case ConsoleKey.M:
                        _modoVisualizacao = _modoVisualizacao == ModoVisualizacaoMapa.NavegacaoRotas
                            ? ModoVisualizacaoMapa.StatusTripulacaoInventario
                            : ModoVisualizacaoMapa.NavegacaoRotas;
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        if (_modoVisualizacao == ModoVisualizacaoMapa.NavegacaoRotas)
                        {
                            emViagem = RealizarSaltoHiperespacial();
                        }
                        else
                        {
                            _modoVisualizacao = ModoVisualizacaoMapa.NavegacaoRotas;
                        }
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        int idx = (tecla.Key >= ConsoleKey.D1 && tecla.Key <= ConsoleKey.D3)
                            ? tecla.Key - ConsoleKey.D1
                            : tecla.Key - ConsoleKey.NumPad1;

                        var arestasDisponiveis = ObterArestasAdjacentesValidas();
                        if (idx >= 0 && idx < arestasDisponiveis.Count)
                        {
                            _indiceArestaSelecionada = idx;
                            if (_modoVisualizacao == ModoVisualizacaoMapa.NavegacaoRotas)
                            {
                                emViagem = RealizarSaltoHiperespacial();
                            }
                        }
                        break;

                    case ConsoleKey.Escape:
                        emViagem = ConfirmarRetornoMenu();
                        break;
                }
            }
        }

        private List<Aresta> ObterArestasAdjacentesValidas()
        {
            if (_verticeAtual == null || _verticeAtual.Arestas == null) return new List<Aresta>();

            int pOrig = ObterNumeroPlaneta(_verticeAtual.Nome);
            int gOrig = ObterNumeroGalaxia(_verticeAtual.Nome);

            if (gOrig == 0 || _verticeAtual.Nome.Equals("Inicio", StringComparison.OrdinalIgnoreCase))
            {
                return _verticeAtual.Arestas;
            }

            return _verticeAtual.Arestas.Where(a =>
            {
                int gDest = ObterNumeroGalaxia(a.Destino.Nome);
                int pDest = ObterNumeroPlaneta(a.Destino.Nome);

                if (gDest == 6) return true; // Na Galáxia 6 converge para o Chefe
                return Math.Abs(pDest - pOrig) <= 1; // Apenas nós imediatamente adjacentes (delta <= 1)
            }).ToList();
        }

        private void MoverSelecaoCima()
        {
            var arestas = ObterArestasAdjacentesValidas();
            if (arestas.Count == 0) return;
            _indiceArestaSelecionada = (_indiceArestaSelecionada - 1 + arestas.Count) % arestas.Count;
        }

        private void MoverSelecaoBaixo()
        {
            var arestas = ObterArestasAdjacentesValidas();
            if (arestas.Count == 0) return;
            _indiceArestaSelecionada = (_indiceArestaSelecionada + 1) % arestas.Count;
        }

        private bool RealizarSaltoHiperespacial()
        {
            var arestas = ObterArestasAdjacentesValidas();
            if (arestas.Count == 0)
            {
                // Chegou ao fim do mapa (Galáxia 6)
                ExibirFimDeJogoFinal(vitoria: true);
                return false;
            }

            _indiceArestaSelecionada = Math.Clamp(_indiceArestaSelecionada, 0, arestas.Count - 1);
            Aresta arestaEscolhida = arestas[_indiceArestaSelecionada];
            Vertice destino = arestaEscolhida.Destino;

            // Animação de Salto Hiperespacial
            ExecutarAnimacaoSalto(destino.Nome, arestaEscolhida.Peso);

            // Atualiza estado do mapa e nave
            _verticeAtual = destino;
            _historicoCaminho.Add(_verticeAtual);
            _totalSaltosRealizados++;
            _creditosEspaciais += 25;
            _indiceArestaSelecionada = 0;

            // Abre o evento/encontro do planeta de destino
            var telaEvento = new TelaEventoPlaneta(_verticeAtual, _tripulacao, _inventarioEquipe);
            telaEvento.Executar();

            // Integra os resultados do evento
            _creditosEspaciais += telaEvento.CreditosGanhos;
            _integridadeNave = Math.Min(100, _integridadeNave + telaEvento.IntegridadeReparada);

            if (telaEvento.VitoriaNoCombate)
            {
                _combatesVencidos++;
            }

            if (telaEvento.DerrotaFinal || _tripulacao.All(c => c.EstaMorto))
            {
                ExibirFimDeJogoFinal(vitoria: false);
                return false;
            }

            // Verifica se este vértice finaliza a campanha (Chefe derrotado na Galáxia 6)
            if (ObterNumeroGalaxia(_verticeAtual.Nome) == 6 && telaEvento.VitoriaNoCombate)
            {
                ExibirFimDeJogoFinal(vitoria: true);
                return false;
            }

            return true;
        }

        private void ExecutarAnimacaoSalto(string nomeDestino, int pesoDobra)
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho("INICIANDO SEQUÊNCIA DE SALTO HIPERESPACIAL", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("MOTORES DE DOBRA ESPACIAL - NAVE VANGUARDA", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo($">> Traçando coordenadas gravitacionais para: {nomeDestino}...", RenderizadorUI.LarguraPadrao, ConsoleColor.White, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo($">> Custo de Dobra: {pesoDobra} Unidade(s) de Éter | Estabilidade dos Propulsores: 100%", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo(">> Colapso do campo gravitacional em andamento...", RenderizadorUI.LarguraPadrao, ConsoleColor.Gray, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  [ DOBRA ] ");
            for (int i = 0; i <= 28; i++)
            {
                Console.Write("█");
                Thread.Sleep(25);
            }
            Console.WriteLine(" 100% - SALTO EXECUTADO!\n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [OK] Nave Vanguarda emergiu com sucesso no setor: {nomeDestino}");
            Console.WriteLine("  [ Pressione qualquer tecla para desembarcar no planeta... ]");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        private void ExibirFimDeJogoFinal(bool vitoria)
        {
            var telaFim = new TelaFimDeJogo(
                vitoria: vitoria,
                saltosRealizados: _totalSaltosRealizados,
                combatesVencidos: _combatesVencidos,
                danoTotalCausado: _danoTotalCausado,
                creditosObtidos: _creditosEspaciais,
                tripulacao: _tripulacao
            );
            telaFim.Executar();

            var telaCreditos = new TelaCreditos();
            telaCreditos.Executar();
        }

        private bool ConfirmarRetornoMenu()
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho("PAUSA NA EXPEDIÇÃO ESTELAR", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("CONFIRMAÇÃO DE DIRETRIZ", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaCentralizada("Deseja realmente pausar a navegação e retornar ao Menu Principal?", RenderizadorUI.LarguraPadrao, ConsoleColor.White, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaCentralizada("[ENTER] Sim, retornar ao Menu  |  [ESC] Não, continuar no Mapa", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkYellow);
            Console.WriteLine();

            ConsoleKeyInfo c = Console.ReadKey(true);
            return c.Key != ConsoleKey.Enter;
        }

        public void Renderizar()
        {
            Limpar();
            DesenharCabecalhoPrincipal();
            DesenharBarraTelemetriaNave();

            switch (_modoVisualizacao)
            {
                case ModoVisualizacaoMapa.StatusTripulacaoInventario:
                    DesenharPainelTripulacaoDetalhado();
                    DesenharPainelInventario();
                    break;

                case ModoVisualizacaoMapa.HistoricoDeVoo:
                    DesenharHistoricoCaminhoBox(ConsoleColor.DarkCyan);
                    break;

                case ModoVisualizacaoMapa.NavegacaoRotas:
                default:
                    DesenharMapaVisualEstelar();
                    DesenharScannerPlanetaAlvo();
                    break;
            }

            DesenharRodapeControles();
        }

        private void DesenharCabecalhoPrincipal()
        {
            string titulo = $"SISTEMA DE NAVEGAÇÃO ESTELAR // MAPA DE ROTAS TÁTICO (SETOR ÉTER-HELIOS) - ESCOLTA DA CARGA 73";
            RenderizadorUI.DesenharCabecalho(titulo, RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private void DesenharBarraTelemetriaNave()
        {
            int galaxiaAtual = ObterNumeroGalaxia(_verticeAtual.Nome);
            string strGalaxia = galaxiaAtual == 0 ? "PONTO DE PARTIDA (INÍCIO)" : $"GALÁXIA {galaxiaAtual} DE 6";

            RenderizadorUI.DesenharInicioSecao("TELEMETRIA DE BORDO & STATUS DA MISSÃO VANGUARDA", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaDupla(
                $"Localização Atual: [ * {_verticeAtual.Nome.ToUpper()} ] ({strGalaxia})",
                $"Setor Operacional: ÉTER-HELIOS | Status Carga: PROTEGIDA [CARGA 73]",
                RenderizadorUI.LarguraPadrao,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.DarkCyan
            );

            string statusTripCompacto = string.Join(" | ", _tripulacao.Select(c => $"{c.Nome} [HP:{c.VidaAtual}/{c.VidaTotal}]"));
            RenderizadorUI.DesenharLinhaDupla(
                $"Integridade do Casco: {RenderizadorUI.ObterBarraCompacta(_integridadeNave, 100, 10)} | Créditos: {_creditosEspaciais} EC | Saltos: {_totalSaltosRealizados}",
                $"Tripulação: {statusTripCompacto}",
                RenderizadorUI.LarguraPadrao,
                ConsoleColor.Green,
                ConsoleColor.White,
                ConsoleColor.DarkCyan
            );

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        /// <summary>
        /// Renderiza o Grafo Estelar 2D completo com 7 colunas (Início + 6 Galáxias) e 3 faixas orbitais,
        /// desenhando conexões, nós perfeitamente alinhados, destaque da nave e seleção ativa da rota.
        /// </summary>
        private void DesenharMapaVisualEstelar()
        {
            int galaxiaAtual = ObterNumeroGalaxia(_verticeAtual.Nome);
            int largura = RenderizadorUI.LarguraPadrao;

            int espacoDisponivel = Math.Max(0, largura - 4);
            int padEsq = Math.Max(0, (espacoDisponivel - 159) / 2);
            int padDir = Math.Max(0, espacoDisponivel - 159 - padEsq);
            string strPadEsq = new string(' ', padEsq);
            string strPadDir = new string(' ', padDir);

            RenderizadorUI.DesenharInicioSecao("MAPA ESTELAR TÁTICO // REDE DE HIPERVIAS DO SETOR", largura, ConsoleColor.DarkCyan);

            // Nomes das 7 Colunas (17 caracteres cada)
            string[] titulosColunas = new string[]
            {
                "   [ INÍCIO ]    ",
                "  [ GALÁXIA 1 ]  ",
                "  [ GALÁXIA 2 ]  ",
                "  [ GALÁXIA 3 ]  ",
                "  [ GALÁXIA 4 ]  ",
                "  [ GALÁXIA 5 ]  ",
                "  [ G6: CHEFE ]  "
            };

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│ " + strPadEsq);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" FAIXA / SETOR: ");
            for (int g = 0; g <= 6; g++)
            {
                if (g == galaxiaAtual)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(titulosColunas[g]);
                }
                else if (g == galaxiaAtual + 1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(titulosColunas[g]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(titulosColunas[g]);
                }

                if (g < 6)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("    ");
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(strPadDir);
            Console.WriteLine(" │");
            Console.ResetColor();

            RenderizadorUI.DesenharSeparador(largura, ConsoleColor.DarkCyan);

            // Renderiza as 3 Faixas Orbitais (Planeta 1, Planeta 2, Planeta 3)
            for (int p = 1; p <= 3; p++)
            {
                DesenharFaixaOrbital(p, galaxiaAtual, strPadEsq, strPadDir);

                // Desenha as conexões diagonais entre faixas (entre faixa 1-2 e faixa 2-3)
                if (p < 3)
                {
                    DesenharHiperviasDiagonais(p, galaxiaAtual, strPadEsq, strPadDir);
                }
            }

            RenderizadorUI.DesenharFimSecao(largura, ConsoleColor.DarkCyan);
            Console.WriteLine();
        }

        private void DesenharFaixaOrbital(int numeroPlaneta, int galaxiaAtual, string strPadEsq, string strPadDir)
        {
            string labelFaixa = numeroPlaneta switch
            {
                1 => " ROTA ALPHA (P1)",
                2 => " ROTA BETA  (P2)",
                _ => " ROTA GAMMA (P3)"
            };

            // Linha 1: Topo dos Boxes de cada nó
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│ " + strPadEsq);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{labelFaixa,-16}");

            for (int g = 0; g <= 6; g++)
            {
                ObterDadosNoVisual(g, numeroPlaneta, galaxiaAtual, out _, out _, out ConsoleColor corBorda, out _, out bool ativo);

                Console.ForegroundColor = corBorda;
                if ((g == 0 && numeroPlaneta != 2) || (g == 6 && numeroPlaneta != 2))
                {
                    Console.Write("                 ");
                }
                else
                {
                    Console.Write(ativo ? "╔═══════════════╗" : "┌───────────────┐");
                }

                if (g < 6)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("    ");
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(strPadDir);
            Console.WriteLine(" │");

            // Linha 2: Conteúdo central dos Boxes e Conectores Horizontais
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│ " + strPadEsq);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("                ");

            for (int g = 0; g <= 6; g++)
            {
                ObterDadosNoVisual(g, numeroPlaneta, galaxiaAtual, out string textoNo, out ConsoleColor corTexto, out ConsoleColor corBorda, out bool isCurrent, out bool ativo);

                if ((g == 0 && numeroPlaneta != 2) || (g == 6 && numeroPlaneta != 2))
                {
                    Console.Write("                 ");
                }
                else
                {
                    Console.ForegroundColor = corBorda;
                    Console.Write(ativo ? "║" : "│");

                    Console.ForegroundColor = corTexto;
                    string textoFormatado = (textoNo.Length >= 15) ? textoNo.Substring(0, 15) : textoNo.PadRight(15);
                    Console.Write(textoFormatado);

                    Console.ForegroundColor = corBorda;
                    Console.Write(ativo ? "║" : "│");
                }

                // Conector horizontal entre esta coluna e a próxima
                if (g < 6)
                {
                    ObterConectorHorizontal(g, numeroPlaneta, galaxiaAtual, out string txtCon, out ConsoleColor corCon);
                    Console.ForegroundColor = corCon;
                    Console.Write(txtCon);
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(strPadDir);
            Console.WriteLine(" │");

            // Linha 3: Fundo dos Boxes de cada nó
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│ " + strPadEsq);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("                ");

            for (int g = 0; g <= 6; g++)
            {
                ObterDadosNoVisual(g, numeroPlaneta, galaxiaAtual, out string _, out ConsoleColor _, out ConsoleColor corBorda, out bool _, out bool ativo);

                Console.ForegroundColor = corBorda;
                if ((g == 0 && numeroPlaneta != 2) || (g == 6 && numeroPlaneta != 2))
                {
                    Console.Write("                 ");
                }
                else
                {
                    Console.Write(ativo ? "╚═══════════════╝" : "└───────────────┘");
                }

                if (g < 6)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("    ");
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(strPadDir);
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        private void DesenharHiperviasDiagonais(int planetaOrigem, int galaxiaAtual, string strPadEsq, string strPadDir)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│ " + strPadEsq);
            Console.Write("                ");

            for (int g = 0; g <= 6; g++)
            {
                Console.Write("                 ");
                if (g < 6)
                {
                    ObterConectorDiagonal(g, planetaOrigem, galaxiaAtual, out string txtDiag, out ConsoleColor corDiag);
                    Console.ForegroundColor = corDiag;
                    Console.Write(txtDiag);
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(strPadDir);
            Console.WriteLine(" │");
            Console.ResetColor();
        }

        private void ObterConectorHorizontal(
            int g,
            int numeroPlaneta,
            int galaxiaAtual,
            out string textoConector,
            out ConsoleColor corConector)
        {
            textoConector = "    ";
            corConector = ConsoleColor.DarkGray;

            // 1. Verifica se faz parte da rota já percorrida (Histórico)
            for (int k = 0; k < _historicoCaminho.Count - 1; k++)
            {
                var orig = _historicoCaminho[k];
                var dest = _historicoCaminho[k + 1];

                int gOrig = ObterNumeroGalaxia(orig.Nome);
                int pOrig = ObterNumeroPlaneta(orig.Nome);
                int gDest = ObterNumeroGalaxia(dest.Nome);
                int pDest = ObterNumeroPlaneta(dest.Nome);

                if (gOrig == g && gDest == g + 1 && pOrig == numeroPlaneta && pDest == numeroPlaneta)
                {
                    textoConector = "──► ";
                    corConector = ConsoleColor.DarkGreen;
                    return;
                }
            }

            // 2. Se for a posição atual, verifica se há aresta horizontal disponível partindo deste nó
            int pAtual = ObterNumeroPlaneta(_verticeAtual.Nome);
            if (g == galaxiaAtual && pAtual == numeroPlaneta)
            {
                var arestasValidas = ObterArestasAdjacentesValidas();
                for (int i = 0; i < arestasValidas.Count; i++)
                {
                    var aresta = arestasValidas[i];
                    int pDest = ObterNumeroPlaneta(aresta.Destino.Nome);

                    if (pDest == numeroPlaneta)
                    {
                        if (i == _indiceArestaSelecionada)
                        {
                            textoConector = "══► ";
                            corConector = ConsoleColor.Yellow;
                        }
                        else
                        {
                            textoConector = "──► ";
                            corConector = ConsoleColor.Cyan;
                        }
                        return;
                    }
                }
            }
        }

        private void ObterConectorDiagonal(
            int g,
            int planetaOrigem,
            int galaxiaAtual,
            out string textoDiagonal,
            out ConsoleColor corDiagonal)
        {
            textoDiagonal = "    ";
            corDiagonal = ConsoleColor.DarkGray;

            // 1. Verifica se faz parte da rota já percorrida (Histórico)
            for (int k = 0; k < _historicoCaminho.Count - 1; k++)
            {
                var orig = _historicoCaminho[k];
                var dest = _historicoCaminho[k + 1];

                int gOrig = ObterNumeroGalaxia(orig.Nome);
                int pOrig = ObterNumeroPlaneta(orig.Nome);
                int gDest = ObterNumeroGalaxia(dest.Nome);
                int pDest = ObterNumeroPlaneta(dest.Nome);

                if (gOrig == g && gDest == g + 1)
                {
                    if (planetaOrigem == 1)
                    {
                        if (pOrig == 1 && pDest == 2)
                        {
                            textoDiagonal = " ╲  ";
                            corDiagonal = ConsoleColor.DarkGreen;
                            return;
                        }
                        else if (pOrig == 2 && pDest == 1)
                        {
                            textoDiagonal = " ╱  ";
                            corDiagonal = ConsoleColor.DarkGreen;
                            return;
                        }
                    }
                    else if (planetaOrigem == 2)
                    {
                        if (pOrig == 2 && pDest == 3)
                        {
                            textoDiagonal = " ╲  ";
                            corDiagonal = ConsoleColor.DarkGreen;
                            return;
                        }
                        else if (pOrig == 3 && pDest == 2)
                        {
                            textoDiagonal = " ╱  ";
                            corDiagonal = ConsoleColor.DarkGreen;
                            return;
                        }
                    }
                }
            }

            // 2. Se for a posição atual, verifica se há aresta diagonal disponível partindo deste nó
            int pAtu = ObterNumeroPlaneta(_verticeAtual.Nome);
            if (g == galaxiaAtual)
            {
                var arestasValidas = ObterArestasAdjacentesValidas();
                for (int i = 0; i < arestasValidas.Count; i++)
                {
                    var aresta = arestasValidas[i];
                    int pDest = ObterNumeroPlaneta(aresta.Destino.Nome);
                    bool isSel = (i == _indiceArestaSelecionada);

                    if (planetaOrigem == 1)
                    {
                        if (pAtu == 1 && pDest == 2)
                        {
                            textoDiagonal = " ╲  ";
                            corDiagonal = isSel ? ConsoleColor.Yellow : ConsoleColor.Cyan;
                            return;
                        }
                        else if (pAtu == 2 && pDest == 1)
                        {
                            textoDiagonal = " ╱  ";
                            corDiagonal = isSel ? ConsoleColor.Yellow : ConsoleColor.Cyan;
                            return;
                        }
                    }
                    else if (planetaOrigem == 2)
                    {
                        if (pAtu == 2 && pDest == 3)
                        {
                            textoDiagonal = " ╲  ";
                            corDiagonal = isSel ? ConsoleColor.Yellow : ConsoleColor.Cyan;
                            return;
                        }
                        else if (pAtu == 3 && pDest == 2)
                        {
                            textoDiagonal = " ╱  ";
                            corDiagonal = isSel ? ConsoleColor.Yellow : ConsoleColor.Cyan;
                            return;
                        }
                    }
                }
            }
        }

        private void ObterDadosNoVisual(int g, int p, int galaxiaAtual, out string textoNo, out ConsoleColor corTexto, out ConsoleColor corBorda, out bool isCurrent, out bool isSelected)
        {
            isCurrent = false;
            isSelected = false;

            if (g == 0)
            {
                if (p == 2)
                {
                    bool emInicio = _verticeAtual.Nome.Equals("Inicio", StringComparison.OrdinalIgnoreCase);
                    if (emInicio)
                    {
                        isCurrent = true;
                        textoNo = "* INICIO (G0) * ";
                        corTexto = ConsoleColor.Green;
                        corBorda = ConsoleColor.Green;
                    }
                    else
                    {
                        textoNo = "[V] INICIO (G0) ";
                        corTexto = ConsoleColor.DarkGreen;
                        corBorda = ConsoleColor.DarkGreen;
                    }
                }
                else
                {
                    textoNo = "               ";
                    corTexto = ConsoleColor.Black;
                    corBorda = ConsoleColor.Black;
                }
                return;
            }

            if (g == 6 && p != 2)
            {
                // Galáxia 6 possui apenas nó único de Chefe na faixa central (p == 2)
                textoNo = "               ";
                corTexto = ConsoleColor.Black;
                corBorda = ConsoleColor.Black;
                return;
            }

            string nomeNo = $"Galáxia {g}, Planeta {p}";
            bool visitado = _historicoCaminho.Any(v => v.Nome.Equals(nomeNo, StringComparison.OrdinalIgnoreCase));
            bool atual = _verticeAtual.Nome.Equals(nomeNo, StringComparison.OrdinalIgnoreCase);

            string tagEncontro = ObterTagEncontroCurta(g, p);

            if (atual)
            {
                isCurrent = true;
                textoNo = "* VANGUARDA *  ";
                corTexto = ConsoleColor.Green;
                corBorda = ConsoleColor.Green;
            }
            else if (visitado)
            {
                textoNo = $"[V] {tagEncontro}".PadRight(15).Substring(0, 15);
                corTexto = ConsoleColor.DarkGreen;
                corBorda = ConsoleColor.DarkGreen;
            }
            else if (g == galaxiaAtual + 1)
            {
                // Verifica se este planeta é um destino diretamente adjacente e alcançável a partir da posição atual
                var arestasValidas = ObterArestasAdjacentesValidas();
                int arestaIndex = arestasValidas.FindIndex(a => a.Destino.Nome.Equals(nomeNo, StringComparison.OrdinalIgnoreCase));

                if (arestaIndex >= 0)
                {
                    // Rota adjacente disponível
                    if (_indiceArestaSelecionada == arestaIndex)
                    {
                        isSelected = true;
                        string tagSel = $">[{arestaIndex + 1}] {tagEncontro}<";
                        textoNo = tagSel.Length > 15 ? tagSel.Substring(0, 15) : RenderizadorUI.CentralizarTexto(tagSel, 15);
                        corTexto = ConsoleColor.Yellow;
                        corBorda = ConsoleColor.Yellow;
                    }
                    else
                    {
                        string tagDisp = $"[{arestaIndex + 1}] {tagEncontro}";
                        textoNo = tagDisp.Length > 15 ? tagDisp.Substring(0, 15) : RenderizadorUI.CentralizarTexto(tagDisp, 15);
                        corTexto = ConsoleColor.Cyan;
                        corBorda = ConsoleColor.DarkCyan;
                    }
                }
                else
                {
                    // Rota não adjacente / bloqueada (ex: topo para baixo ou baixo para topo)
                    string tagBloq = $"[X] {tagEncontro}";
                    textoNo = tagBloq.Length > 15 ? tagBloq.Substring(0, 15) : RenderizadorUI.CentralizarTexto(tagBloq, 15);
                    corTexto = ConsoleColor.DarkGray;
                    corBorda = ConsoleColor.DarkGray;
                }
            }
            else
            {
                // Inexplorado no futuro
                string tagInexp = $"[-] {tagEncontro}";
                textoNo = tagInexp.Length > 15 ? tagInexp.Substring(0, 15) : RenderizadorUI.CentralizarTexto(tagInexp, 15);
                corTexto = ConsoleColor.DarkGray;
                corBorda = ConsoleColor.DarkGray;
            }
        }

        private string ObterTagEncontroCurta(int galaxia, int planeta)
        {
            var vertice = _grafo?.Vertices.Find(v => v.Nome.Equals($"Galáxia {galaxia}, Planeta {planeta}", StringComparison.OrdinalIgnoreCase));
            if (vertice?.Encontro?.Comportamento != null)
            {
                if (vertice.Encontro.Comportamento is EncontroBaseEspacial)
                    return "DESCANSO";
                if (vertice.Encontro.Comportamento is EncontroBatalha)
                    return galaxia == 6 ? "CHEFE G6" : "COMBATE";
            }

            if (galaxia == 6) return "CHEFE G6";
            return "COMBATE";
        }

        /// <summary>
        /// Painel com o Scanner Planetário Holográfico e Telemetria do Alvo Selecionado.
        /// </summary>
        private void DesenharScannerPlanetaAlvo()
        {
            var arestasValidas = ObterArestasAdjacentesValidas();
            if (_verticeAtual == null || arestasValidas.Count == 0)
            {
                RenderizadorUI.DesenharInicioSecao($"DESTINO FINAL ALCANÇADO: {_verticeAtual?.Nome.ToUpper()}", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
                RenderizadorUI.DesenharLinhaCentralizada(">> A CARGA 73 FOI ENTREGUE EM SEGURANÇA NO PONTO DE EXTRAÇÃO! <<", RenderizadorUI.LarguraPadrao, ConsoleColor.Green, ConsoleColor.Cyan);
                RenderizadorUI.DesenharLinhaCentralizada("[ Pressione ENTER para concluir a missão estelar ]", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow, ConsoleColor.Cyan);
                RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
                Console.WriteLine();
                return;
            }

            int indexEfetivo = Math.Clamp(_indiceArestaSelecionada, 0, arestasValidas.Count - 1);
            var arestaSelecionada = arestasValidas[indexEfetivo];
            var destino = arestaSelecionada.Destino;

            string[] arteCenario = BancoSprites.ObterArteCenario(destino.Nome);
            string tituloCenario = $"RADAR: {destino.Nome.ToUpper()}";

            ExtrairInfoPlaneta(destino.Nome, out string bioma, out string perigo, out string recompensa, out string descricao);
            string tipoEncontroTexto = ObterTipoEncontroResumido(destino.Nome);

            List<string> infoScanner = new List<string>
            {
                $"DESTINO ALVO:   [ {destino.Nome.ToUpper()} ]  |  ENCONTRO: {tipoEncontroTexto}",
                $"BIOMA / AMBIENTE: {bioma}",
                $"NÍVEL DE AMEAÇA:  {perigo}",
                $"ESPÓLIO PROVÁVEL: {recompensa}",
                $"SENSOR DE BORDO:  {descricao}",
                $"TRAJETÓRIA:       Custo de Salto: {arestaSelecionada.Peso} Unidade de Dobra  |  Estabilidade: 100%",
                $"COMANDO TÁTICO:   >> [ ENTER / ESPAÇO ] Iniciar Salto Hiperespacial com a Carga 73 <<"
            };

            RenderizadorUI.DesenharPainelCenarioComTexto(
                arteCenario,
                tituloCenario,
                infoScanner,
                "SCANNER HOLOGRÁFICO DE BORDO // TELEMETRIA DO PLANETA ALVO",
                corCenario: ConsoleColor.Cyan,
                corTexto: ConsoleColor.White,
                corBorda: ConsoleColor.DarkCyan
            );
            Console.WriteLine();
        }

        private Vertice arestaEscolhidaDestino(Aresta aresta)
        {
            return aresta?.Destino;
        }

        private void DesenharPainelTripulacaoDetalhado()
        {
            RenderizadorUI.DesenharInicioSecao("REGISTRO DETALHADO DOS COMBATENTES MERCENÁRIOS", RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);

            foreach (var c in _tripulacao)
            {
                bool vivo = !c.EstaMorto;
                string statusVivo = vivo ? $"HP:{c.VidaAtual}/{c.VidaTotal}" : "[ABATIDO]";
                string modInfo = c switch
                {
                    Sentinela s => $"Adren: {s.Adrenalina}/45",
                    Engenheiro eng => $"Aquec: {eng.Sobreaquecimento}/45",
                    Biomancer bio => $"Mana: {bio.Mana}/45",
                    _ => $"Def: {c.Defesa}"
                };

                string esq = $"  - {c.Nome,-22} | Nível: {c.Level,2}/10 | Classe: {c.GetType().Name,-12} | Blindagem: [{c.Afinidade}]";
                string dir = $"{statusVivo} | Def: {c.Defesa} | Agi: {c.Agilidade} | {modInfo} | Baralho: {c.Habilidades.Count} cartas";

                RenderizadorUI.DesenharLinhaDupla(esq, dir, RenderizadorUI.LarguraPadrao, ConsoleColor.White, ConsoleColor.Green, ConsoleColor.Cyan);
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private void DesenharPainelInventario()
        {
            RenderizadorUI.DesenharInicioSecao($"COMPARTIMENTO DE CARGA & ITENS ({_inventarioEquipe.Count} ITENS ARMAZENADOS)", RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow);

            if (_inventarioEquipe.Count == 0)
            {
                RenderizadorUI.DesenharLinhaConteudo("Nenhum item armazenado no momento.", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkGray, ConsoleColor.Yellow);
            }
            else
            {
                for (int i = 0; i < _inventarioEquipe.Count; i++)
                {
                    var item = _inventarioEquipe[i];
                    string linhaEsq = $"  [{i + 1}] {item.Nome,-28} [{item.Raridade.ToString().ToUpper()}]";
                    string linhaDir = $"Efeito: +{item.ValorEfeito} {item.Tipo} - {item.Descricao}";
                    RenderizadorUI.DesenharLinhaDupla(linhaEsq, linhaDir, RenderizadorUI.LarguraPadrao, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Yellow);
                }
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, ConsoleColor.Yellow);
            Console.WriteLine();
        }

        private void DesenharHistoricoCaminhoBox(ConsoleColor corBorda)
        {
            RenderizadorUI.DesenharInicioSecao("DIÁRIO DE VOO - TRAJETÓRIA REALIZADA PELA NAVE", RenderizadorUI.LarguraPadrao, corBorda);

            if (_historicoCaminho.Count == 0)
            {
                RenderizadorUI.DesenharLinhaConteudo("Nenhum salto registrado ainda.", RenderizadorUI.LarguraPadrao, ConsoleColor.DarkGray, corBorda);
            }
            else
            {
                string rotaStr = string.Join(" ═► ", _historicoCaminho.Select((v, i) => $"({i}) {v.Nome}"));
                RenderizadorUI.DesenharLinhaConteudo(rotaStr, RenderizadorUI.LarguraPadrao, ConsoleColor.Cyan, corBorda);
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, corBorda);
            Console.WriteLine();
        }

        private void DesenharRodapeControles()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [◄/► ou A/D] Selecionar Rota Adjacente | [1-3] Salto Direto | [ENTER / Espaço] Iniciar Dobra Espacial");
            Console.ResetColor();
        }

        private int ObterNumeroGalaxia(string nomeVertice)
        {
            if (string.IsNullOrWhiteSpace(nomeVertice) || nomeVertice.Equals("Inicio", StringComparison.OrdinalIgnoreCase))
                return 0;

            var partes = nomeVertice.Split(new[] { "Galáxia", "Planeta", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 1 && int.TryParse(partes[0], out int galaxia))
            {
                return galaxia;
            }
            return 0;
        }

        private int ObterNumeroPlaneta(string nomeVertice)
        {
            if (string.IsNullOrWhiteSpace(nomeVertice) || nomeVertice.Equals("Inicio", StringComparison.OrdinalIgnoreCase))
                return 2;

            var partes = nomeVertice.Split(new[] { "Galáxia", "Planeta", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 2 && int.TryParse(partes[1], out int planeta))
            {
                return planeta;
            }
            return 2;
        }

        private string ObterTipoEncontroResumido(string nomeVertice)
        {
            var vertice = _grafo?.Vertices.Find(v => v.Nome.Equals(nomeVertice, StringComparison.OrdinalIgnoreCase));
            if (vertice?.Encontro?.Comportamento != null)
            {
                if (vertice.Encontro.Comportamento is EncontroBaseEspacial)
                    return "[ÁREA DE DESCANSO] Base Espacial";
                if (vertice.Encontro.Comportamento is EncontroBatalha)
                {
                    int g = ObterNumeroGalaxia(nomeVertice);
                    return g == 6 ? "[CHEFE SUPREMO] Fortaleza Dreadnought Sindical" : "[COMBATE] Patrulha Espacial Hostil";
                }
            }

            int galaxia = ObterNumeroGalaxia(nomeVertice);
            if (galaxia == 6) return "[CHEFE SUPREMO] Fortaleza Dreadnought Sindical";
            return "[COMBATE] Patrulha de Drones";
        }

        private void ExtrairInfoPlaneta(string nomeVertice, out string bioma, out string perigo, out string recompensa, out string descricao)
        {
            var vertice = _grafo?.Vertices.Find(v => v.Nome.Equals(nomeVertice, StringComparison.OrdinalIgnoreCase));
            int galaxia = ObterNumeroGalaxia(nomeVertice);

            if (vertice?.Encontro?.Comportamento is EncontroBaseEspacial)
            {
                bioma = "Estação Neutra de Reparo & Armeria";
                perigo = "Nenhum (Área Segura)";
                recompensa = "Troca de Habilidades & Reparo de Nave";
                descricao = "Estação de apoio com serviços de manutenção e troca de cartas táticas.";
                return;
            }
            else if (vertice?.Encontro?.Comportamento is EncontroBatalha || galaxia > 0)
            {
                if (galaxia == 6)
                {
                    bioma = "Fortaleza Dreadnought Sindical";
                    perigo = "MÁXIMO (Chefe Final)";
                    recompensa = "Vitória da Campanha + Liberdade da Carga 73";
                    descricao = "O coração do bloqueio militar inimigo. A passagem final para a evacuação.";
                }
                else
                {
                    bioma = $"Setor Militarizado (Galáxia {galaxia})";
                    perigo = "Moderado / Alto";
                    recompensa = $"Sucata (+{60 + galaxia * 10} EC) & Item";
                    descricao = "Forças hostis interceptaram nossos sensores e preparam engajamento.";
                }
                return;
            }

            bioma = "Porto Espacial Inicial da Vanguarda";
            perigo = "Seguro (Zona Livre)";
            recompensa = "Preparação Tática";
            descricao = "Ponto de partida da missão mercenária. A Carga 73 está a bordo e protegida.";
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
