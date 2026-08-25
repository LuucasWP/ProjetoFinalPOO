using System;
using System.Collections.Generic;
using System.Linq;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.EncontroPlaneta;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Mapa;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de Encontro e Eventos Planetários.
    /// Exibida quando a nave realiza um salto hiperespacial para um nó/planeta do Grafo do Mapa.
    /// Apresenta narrativa sci-fi, opções táticas, coleta de recursos (README 22),
    /// troca de habilidades nas áreas de descanso (README 14) e preparação para combate tático (README 5 e 21).
    /// </summary>
    public class TelaEventoPlaneta : ITela
    {
        private readonly Vertice _verticePlaneta;
        private readonly List<Combatente> _tripulacao;
        private readonly List<Item> _inventarioEquipe;

        private readonly int _numeroGalaxia;
        private readonly int _numeroPlaneta;
        private readonly TipoEncontro _tipoEncontro;
        private readonly string _nomeBioma;
        private readonly string _descricaoNarrativa;
        private readonly string _perigoSetor;
        private readonly string _recompensaEstimada;
        private int _opcaoSelecionada;
        private readonly List<string> _opcoesDisponiveis;

        public int CreditosGanhos { get; private set; }
        public int IntegridadeReparada { get; private set; }
        public bool VitoriaNoCombate { get; private set; }
        public bool DerrotaFinal { get; private set; }

        public TelaEventoPlaneta(
            Vertice verticePlaneta,
            List<Combatente> tripulacao,
            List<Item> inventarioEquipe)
        {
            _verticePlaneta = verticePlaneta ?? throw new ArgumentNullException(nameof(verticePlaneta));
            _tripulacao = tripulacao ?? new List<Combatente>();
            _inventarioEquipe = inventarioEquipe ?? new List<Item>();
            _opcaoSelecionada = 0;
            _opcoesDisponiveis = new List<string>();
            CreditosGanhos = 0;
            IntegridadeReparada = 0;
            VitoriaNoCombate = false;
            DerrotaFinal = false;

            // Extrai números da galáxia e do planeta pelo nome do vértice
            ExtrairDadosVertice(_verticePlaneta.Nome, out _numeroGalaxia, out _numeroPlaneta);

            // Gera dados contextuais de acordo com a galáxia e planeta
            GerarDetalhesPlaneta(out _tipoEncontro, out _nomeBioma, out _descricaoNarrativa, out _perigoSetor, out _recompensaEstimada);

            ConfigurarOpcoes();
        }

        private void ExtrairDadosVertice(string nome, out int galaxia, out int planeta)
        {
            galaxia = 1;
            planeta = 1;

            if (nome.Equals("Inicio", StringComparison.OrdinalIgnoreCase))
            {
                galaxia = 0;
                planeta = 0;
                return;
            }

            try
            {
                var partes = nome.Split(new[] { "Galáxia", "Planeta", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 2)
                {
                    int.TryParse(partes[0], out galaxia);
                    int.TryParse(partes[1], out planeta);
                }
            }
            catch
            {
                galaxia = 1;
                planeta = 1;
            }
        }

        private void GerarDetalhesPlaneta(out TipoEncontro tipo, out string bioma, out string narrativa, out string perigo, out string recompensa)
        {
            if (_numeroGalaxia == 0)
            {
                tipo = TipoEncontro.EstacaoReparo;
                bioma = "Base Estelar de Lançamento Vanguarda";
                narrativa = "Ponto de partida da missão mercenária. Motores abastecidos e Carga 73 protegida no compartimento estase.";
                perigo = "Nenhum (Área Segura sob controle aliado)";
                recompensa = "Módulos de bordo calibrados, armeria ativa e cartas iniciais.";
                return;
            }

            if (_numeroGalaxia == 6)
            {
                tipo = TipoEncontro.Chefe;
                bioma = "Núcleo Estelar do Setor Sindical - Fortaleza Dreadnought";
                narrativa = "ALERTA MÁXIMO: A esquadra capitânia da Frota Sindical bloqueia o corredor de evacuação. A Carga 73 é o alvo prioritário deles!";
                perigo = "EXTREMO - Esquadrão Titã de Elite (Armaduras pesadas e bio-armas corrosivas).";
                recompensa = "Vitória da Campanha, +500 Créditos Estelares e liberdade da Carga 73.";
                return;
            }

            if (_verticePlaneta.Encontro?.Comportamento is EncontroBaseEspacial)
            {
                tipo = TipoEncontro.EstacaoReparo;
                bioma = $"Estação de Apoio e Armeria {_verticePlaneta.Nome}";
                narrativa = $"Estação neutra de apoio com serviços de manutenção, armeria e descanso na Galáxia {_numeroGalaxia}.";
                perigo = "Nenhum (Área Segura)";
                recompensa = "Troca de Habilidades & Reparo de Nave";
                return;
            }

            tipo = TipoEncontro.CombateComum;
            bioma = $"Setor Planetário {_verticePlaneta.Nome}";
            narrativa = $"Setor da Galáxia {_numeroGalaxia} sob vigilância de drones hostis.";
            perigo = "Moderado";
            recompensa = $"Sucata (+{60 + _numeroGalaxia * 10} EC) & Item Tático";
        }

        private void ConfigurarOpcoes()
        {
            _opcoesDisponiveis.Clear();

            if (_numeroGalaxia == 0)
            {
                _opcoesDisponiveis.Add("Acessar Armeria (Trocar Habilidades da Tripulação)");
                _opcoesDisponiveis.Add("Verificar Inventário de Bordo & Carga 73");
                _opcoesDisponiveis.Add("Iniciar Travessia Estelar (Abrir Mapa de Dobra)");
                return;
            }

            if (_tipoEncontro == TipoEncontro.Chefe)
            {
                _opcoesDisponiveis.Add("INICIAR CONFRONTO FINAL (Enfrentar Nave Capitânia Sindical)");
                _opcoesDisponiveis.Add("Escanear Fraquezas e Afinidades do Dreadnought Inimigo");
                _opcoesDisponiveis.Add("Acessar Armeria de Emergência (Reorganizar Habilidades)");
                _opcoesDisponiveis.Add("Retornar ao Mapa Estelar");
                return;
            }

            switch (_tipoEncontro)
            {
                case TipoEncontro.CombateComum:
                case TipoEncontro.CombateElite:
                    _opcoesDisponiveis.Add("Engajar em Combate Tático 3v3 (Cartas & Moedas)");
                    _opcoesDisponiveis.Add("Escanear Fraquezas e Blindagens Inimigas (Afinidades)");
                    _opcoesDisponiveis.Add("Retornar ao Mapa Estelar");
                    break;

                case TipoEncontro.EstacaoReparo:
                    _opcoesDisponiveis.Add("Descansar & Reparar Tripulação (+40 HP, +25 Recursos Especiais, +35% Casco)");
                    _opcoesDisponiveis.Add("Acessar Armeria (Trocar Habilidades da Tripulação - README 14)");
                    _opcoesDisponiveis.Add("Coletar Espólio de Suprimentos na Estação (Escolher 1 Item - README 22)");
                    _opcoesDisponiveis.Add("Retornar ao Mapa Estelar");
                    break;

                case TipoEncontro.Comercio:
                    _opcoesDisponiveis.Add("Negociar com Bazar de Sucateiros (Escolher 1 Item - README 22)");
                    _opcoesDisponiveis.Add("Calibrar Sensores e Reabastecer Energia (+20 EN para todos)");
                    _opcoesDisponiveis.Add("Retornar ao Mapa Estelar");
                    break;

                default: // EventoAnomalia
                    _opcoesDisponiveis.Add("Canalizar Fenda com o Biomancer Pasteur (+30 Recursos Especiais)");
                    _opcoesDisponiveis.Add("Enviar Drones para Extrair Artefatos de Vácuo (Escolher 1 Item - README 22)");
                    _opcoesDisponiveis.Add("Manter Distância Segura e Prosseguir no Mapa");
                    break;
            }
        }

        public void Entrar()
        {
            _opcaoSelecionada = 0;
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
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        _opcaoSelecionada = (_opcaoSelecionada - 1 + _opcoesDisponiveis.Count) % _opcoesDisponiveis.Count;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        _opcaoSelecionada = (_opcaoSelecionada + 1) % _opcoesDisponiveis.Count;
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        emExecucao = ProcessarEscolha(_opcaoSelecionada);
                        break;

                    case ConsoleKey.Escape:
                        emExecucao = false;
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                        int idx = tecla.Key - ConsoleKey.D1;
                        if (idx >= 0 && idx < _opcoesDisponiveis.Count)
                        {
                            _opcaoSelecionada = idx;
                            emExecucao = ProcessarEscolha(_opcaoSelecionada);
                        }
                        break;
                }
            }
        }

        private bool ProcessarEscolha(int indice)
        {
            string textoOpcao = _opcoesDisponiveis[indice];

            if (textoOpcao.Contains("Retornar ao Mapa", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Iniciar Travessia", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Manter Distância", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // 1. TROCAR HABILIDADES NAS ÁREAS DE DESCANSO (README 14)
            if (textoOpcao.Contains("Trocar Habilidades", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Acessar Armeria", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Reorganizar Habilidades", StringComparison.OrdinalIgnoreCase))
            {
                var telaTroca = new TelaTrocaHabilidades(_tripulacao);
                telaTroca.Executar();
                return true;
            }

            // 2. ESCOLHER ITEM NA EXPLORAÇÃO (ANOMALIA / BAZAR / ESPÓLIO - README 22)
            if (textoOpcao.Contains("Escolher 1 Item", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Coletar Espólio", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Negociar com Bazar", StringComparison.OrdinalIgnoreCase) ||
                textoOpcao.Contains("Extrair Artefatos", StringComparison.OrdinalIgnoreCase))
            {
                var telaItem = new TelaEscolhaItem(BancoItens.GerarEscolhaTresItens(), _inventarioEquipe);
                telaItem.Executar();
                return false; // Escolha única consumida: finaliza o evento e retorna ao mapa
            }

            // 3. COMBATE TÁTICO INTERATIVO (README 5, 6, 16, 17, 18, 19, 20, 21)
            if (textoOpcao.Contains("Combate", StringComparison.OrdinalIgnoreCase) || textoOpcao.Contains("CONFRONTO FINAL", StringComparison.OrdinalIgnoreCase))
            {
                List<Combatente> inimigos = GerarEsquadraoInimigo();
                var telaCombate = new TelaCombate(
                    _tripulacao,
                    inimigos,
                    _inventarioEquipe,
                    nomeEncontro: $"{_verticePlaneta.Nome} [{_tipoEncontro}]",
                    ehChefe: _tipoEncontro == TipoEncontro.Chefe
                );

                bool vitoria = telaCombate.Executar();
                VitoriaNoCombate = vitoria;

                if (vitoria)
                {
                    int expGanho = 30 + _numeroGalaxia * 15;
                    CreditosGanhos += (_tipoEncontro == TipoEncontro.Chefe ? 400 : 60 + _numeroGalaxia * 20);

                    // Concede EXP aos heróis sobreviventes e sobe de nível se acumulado (Cap level 10 - README 9)
                    foreach (var aliado in _tripulacao.Where(c => !c.EstaMorto))
                    {
                        aliado._exp += expGanho;
                        while (aliado._exp >= 100 && aliado._level < 10)
                        {
                            aliado._level++;
                            aliado._exp -= 100;
                            aliado._vidaTotal += 12;
                            aliado._vidaAtual = Math.Min(aliado.VidaTotal, aliado.VidaAtual + 20);
                            aliado._defesa += 1;
                            if (aliado._level % 3 == 0)
                            {
                                aliado._agilidade += 1;
                            }
                        }
                    }

                    // Recompensa de escolha de 1 item pós-combate (README 22)
                    var telaItem = new TelaEscolhaItem(BancoItens.GerarEscolhaTresItens(), _inventarioEquipe);
                    telaItem.Executar();
                }
                else
                {
                    DerrotaFinal = true;
                }

                return false;
            }

            // 4. REPAROS & DESCANSO (ESTAÇÃO DE REPARO)
            if (textoOpcao.Contains("Descansar", StringComparison.OrdinalIgnoreCase) || textoOpcao.Contains("Reparar", StringComparison.OrdinalIgnoreCase))
            {
                IntegridadeReparada += 35;
                foreach (var c in _tripulacao)
                {
                    c.Defender();
                    c.AlterarModificador(15);
                }

                ExibirMensagemResultado(
                    "DESCANSO & MANUTENÇÃO CONCLUÍDOS",
                    "A tripulação descansou nos alojamentos da estação. Todos recuperaram integridade e calibraram seus módulos especiais!",
                    "Casco da Vanguarda reparado em +35% de integridade estrutural."
                );
                return false; // Conclui o descanso e retorna ao mapa
            }

            // 5. EVENTOS MÍSTICOS / ANOMALIAS (CANALIZAR FENDA)
            if (textoOpcao.Contains("Canalizar Fenda", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var c in _tripulacao)
                {
                    c.AlterarModificador(20);
                }

                ExibirMensagemResultado(
                    "HARMONIA BIO-MÍSTICA COM O ÉTER",
                    "Pasteur canalizou as frequências bio-místicas da fenda estelar. As reservas de Adrenalina, Superaquecimento e Mana da tripulação se estabilizaram.",
                    "Toda a tripulação potencializou seus recursos especiais! Chance de cara nas moedas aumentada."
                );
                return false; // Escolha única da anomalia consumida: finaliza o evento e retorna ao mapa
            }

            // 6. REABASTECER NO COMÉRCIO
            if (textoOpcao.Contains("Calibrar Sensores", StringComparison.OrdinalIgnoreCase) || textoOpcao.Contains("Reabastecer Energia", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var c in _tripulacao)
                {
                    c.AlterarModificador(10);
                }

                ExibirMensagemResultado(
                    "SENSORES E ENERGIA RECALIBRADOS",
                    "Os técnicos do entreposto realizaram manutenção nos subsistemas da tripulação.",
                    "Todos os tripulantes recuperaram postura e foco tático!"
                );
                return false;
            }

            // 7. ESCANEAR AFINIDADES E INIMIGOS
            if (textoOpcao.Contains("Escanear", StringComparison.OrdinalIgnoreCase))
            {
                ExibirScannerAfinidades();
                return true; // Apenas informativo
            }

            // Outras opções de relatório
            ExibirMensagemResultado(
                "DIRETRIZ PROCESSADA",
                $"Ação '{textoOpcao}' executada com sucesso pelos sistemas da nave.",
                "Dados atualizados no diário de bordo."
            );
            return false;
        }

        private List<Combatente> GerarEsquadraoInimigo()
        {
            var inimigos = new List<Combatente>();
            int fatorNivel = Math.Max(1, _numeroGalaxia);

            if (_tipoEncontro == TipoEncontro.Chefe || _numeroGalaxia == 6)
            {
                // Esquadrão Chefe: Capitânia Sindical Titânica e Escolta Pesada
                int vidaChefe = 260 + (fatorNivel * 15);
                int defChefe = 14 + fatorNivel;
                var chefe = new Inimigo("Capitânia Dreadnought", vidaChefe, defChefe, 11, AfinidadeDefesa.Armadurado);
                chefe._level = fatorNivel;
                chefe.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("chefe", fatorNivel));
                chefe.AdicionarHabilidadesDisponiveis(chefe.Habilidades);

                var escolta1 = new Inimigo("Drone Balístico Pesado", 80 + (fatorNivel * 8), 10 + fatorNivel, 14, AfinidadeDefesa.Mecanico);
                escolta1._level = fatorNivel;
                escolta1.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("drone", fatorNivel));
                escolta1.AdicionarHabilidadesDisponiveis(escolta1.Habilidades);

                var escolta2 = new Inimigo("Algoz Cibernético", 90 + (fatorNivel * 8), 8 + fatorNivel, 22, AfinidadeDefesa.Biologico);
                escolta2._level = fatorNivel;
                escolta2.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("pirata", fatorNivel));
                escolta2.AdicionarHabilidadesDisponiveis(escolta2.Habilidades);

                inimigos.Add(chefe);
                inimigos.Add(escolta1);
                inimigos.Add(escolta2);
            }
            else if (_tipoEncontro == TipoEncontro.CombateElite || _numeroGalaxia >= 4)
            {
                // Inimigos de Elite / Setor Avançado
                int vidaBase = 80 + (fatorNivel * 12);
                int defBase = 8 + fatorNivel;

                var elite1 = new Inimigo("Corsário Blindado", vidaBase + 20, defBase + 2, 10, AfinidadeDefesa.Armadurado);
                elite1._level = fatorNivel;
                elite1.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("pirata", fatorNivel));
                elite1.AdicionarHabilidadesDisponiveis(elite1.Habilidades);

                var elite2 = new Inimigo("Autômato de Guerra", vidaBase + 10, defBase + 3, 12, AfinidadeDefesa.Mecanico);
                elite2._level = fatorNivel;
                elite2.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("drone", fatorNivel));
                elite2.AdicionarHabilidadesDisponiveis(elite2.Habilidades);

                var elite3 = new Inimigo("Franco-Atirador Sindical", vidaBase - 10, defBase - 1, 23, AfinidadeDefesa.Biologico);
                elite3._level = fatorNivel;
                elite3.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("pirata", fatorNivel));
                elite3.AdicionarHabilidadesDisponiveis(elite3.Habilidades);

                inimigos.Add(elite1);
                inimigos.Add(elite2);
                inimigos.Add(elite3);
            }
            else
            {
                // Combate comum: Escala progressiva de Vida, Defesa e Habilidades com a galáxia
                int vidaComum = 50 + (fatorNivel * 10);
                int defComum = 6 + fatorNivel;

                var d1 = new Inimigo("Drone de Patrulha Alpha", vidaComum, defComum, 16, AfinidadeDefesa.Mecanico);
                d1._level = fatorNivel;
                d1.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("drone", fatorNivel));
                d1.AdicionarHabilidadesDisponiveis(d1.Habilidades);

                var d2 = new Inimigo("Sentinela de Choque", vidaComum + 15, defComum + 2, 7, AfinidadeDefesa.Armadurado);
                d2._level = fatorNivel;
                d2.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("drone", fatorNivel));
                d2.AdicionarHabilidadesDisponiveis(d2.Habilidades);

                inimigos.Add(d1);
                inimigos.Add(d2);

                if (_numeroGalaxia >= 2)
                {
                    var d3 = new Inimigo("Corsário de Reconhecimento", vidaComum + 5, defComum, 18, AfinidadeDefesa.Biologico);
                    d3._level = fatorNivel;
                    d3.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo("pirata", fatorNivel));
                    d3.AdicionarHabilidadesDisponiveis(d3.Habilidades);
                    inimigos.Add(d3);
                }
            }

            return inimigos;
        }

        private void ExibirScannerAfinidades()
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho("SCANNER TÁTICO DE AFINIDADES & FRAQUEZAS (README 23-26)", 0, ConsoleColor.Cyan);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("TABELA DE EFETIVIDADE ELEMENTAL (ATAQUE VS BLINDAGEM)", 0, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM ARMADURA : Fraca contra ÁCIDO (2.0x) | Neutra contra ELÉTRICO (1.0x) | Forte contra FOGO (0.5x)", 0, ConsoleColor.Yellow, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM MECÂNICO : Fraco contra ELÉTRICO (2.0x) | Neutro contra FOGO (1.0x) | Forte contra ÁCIDO (0.5x)", 0, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharLinhaConteudo("  - BLINDAGEM BIOLÓGICO: Fraco contra FOGO (2.0x) | Neutro contra ÁCIDO (1.0x) | Forte contra ELÉTRICO (0.5x)", 0, ConsoleColor.Green, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkCyan);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("DICA TÁTICA DA NAVE VANGUARDA", 0, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaConteudo("Use as habilidades de Ácido do Sentinela/Biomancer contra Armaduras, Elétricas do Engenheiro contra Mecânicos,", 0, ConsoleColor.White, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharLinhaConteudo("e ataques de Fogo contra alvos Biológicos para maximizar as chances de vitória nos embates!", 0, ConsoleColor.White, ConsoleColor.DarkYellow);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkYellow);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para retornar ao painel do planeta... ]");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        private void ExibirMensagemResultado(string titulo, string linha1, string linha2)
        {
            Console.Clear();
            RenderizadorUI.DesenharCabecalho(titulo, 0, ConsoleColor.Green);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("RELATÓRIO DE BORDO", 0, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaCentralizada(linha1, 0, ConsoleColor.White, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharLinhaCentralizada(linha2, 0, ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGreen);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para continuar... ]");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        public void Renderizar()
        {
            Limpar();
            DesenharCabecalhoPlaneta();
            DesenharPainelCenarioETelemetria();
            DesenharStatusTripulacaoCompacto();
            DesenharPainelAcoes();
            DesenharRodapeControles();
        }

        private void DesenharCabecalhoPlaneta()
        {
            ConsoleColor corTema = _tipoEncontro switch
            {
                TipoEncontro.Chefe => ConsoleColor.Red,
                TipoEncontro.CombateElite => ConsoleColor.Magenta,
                TipoEncontro.CombateComum => ConsoleColor.Yellow,
                TipoEncontro.Comercio => ConsoleColor.Cyan,
                TipoEncontro.EstacaoReparo => ConsoleColor.Green,
                _ => ConsoleColor.Blue
            };

            RenderizadorUI.DesenharCabecalho($"EXPLORAÇÃO DE SETOR: {_verticePlaneta.Nome.ToUpper()} [{_tipoEncontro.ToString().ToUpper()}]", 0, corTema);
            Console.WriteLine();
        }

        private void DesenharPainelCenarioETelemetria()
        {
            string[] arteCenario = BancoSprites.ObterArteCenario($"{_tipoEncontro} {_nomeBioma} {_verticePlaneta.Nome}");
            string tituloCenario = $"VISOR ÓPTICO: {_nomeBioma.ToUpper()}";

            List<string> linhasInfo = new List<string>
            {
                $"Bioma / Estrutura: {_nomeBioma,-32} | Setor: Galáxia {_numeroGalaxia}",
                $"Nível de Ameaça:   {_perigoSetor,-32} | Rotas Conectadas: {_verticePlaneta.Arestas.Count} salto(s)",
                $"Recompensa Estimada: {_recompensaEstimada}",
                $"----------------------------------------------------------------------------------------",
                $"Sensores de Bordo: {_descricaoNarrativa}",
                $"Status da Carga: Compartimento Estase Seguro (Carga 73 sob proteção mercenária)",
                $"Diretriz de Operação: Escolha uma ação abaixo para conduzir o esquadrão no setor."
            };

            ConsoleColor corTema = _tipoEncontro switch
            {
                TipoEncontro.Chefe => ConsoleColor.Red,
                TipoEncontro.CombateElite => ConsoleColor.Magenta,
                TipoEncontro.CombateComum => ConsoleColor.Yellow,
                TipoEncontro.Comercio => ConsoleColor.Cyan,
                TipoEncontro.EstacaoReparo => ConsoleColor.Green,
                _ => ConsoleColor.DarkCyan
            };

            RenderizadorUI.DesenharPainelCenarioComTexto(
                arteCenario,
                tituloCenario,
                linhasInfo,
                "TRANSMISSÃO DOS SENSORES & DADOS AMBIENTAIS",
                corCenario: corTema,
                corTexto: ConsoleColor.White,
                corBorda: corTema
            );
            Console.WriteLine();
        }

        private void DesenharStatusTripulacaoCompacto()
        {
            if (_tripulacao.Count == 0) return;

            RenderizadorUI.DesenharInicioSecao($"STATUS DA TRIPULAÇÃO ({_tripulacao.FindAll(c => !c.EstaMorto).Count}/3 VIVOS) | ITENS NO INVENTÁRIO: {_inventarioEquipe.Count}", 0, ConsoleColor.DarkGreen);

            string linha = "  ";
            foreach (var c in _tripulacao)
            {
                string tag = !c.EstaMorto ? $"[ {c.Nome} (Lvl {c.Level}) HP:{c.VidaAtual}/{c.VidaTotal} Def:{c.Defesa} ]" : $"[ {c.Nome} (ABATIDO) ]";
                linha += tag + "   ";
            }

            RenderizadorUI.DesenharLinhaCentralizada(linha, 0, ConsoleColor.Green, ConsoleColor.DarkGreen);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGreen);
            Console.WriteLine();
        }

        private void DesenharPainelAcoes()
        {
            RenderizadorUI.DesenharInicioSecao("DIRETRIZES E AÇÕES DISPONÍVEIS", 0, ConsoleColor.Cyan);

            for (int i = 0; i < _opcoesDisponiveis.Count; i++)
            {
                if (i == _opcaoSelecionada)
                {
                    string linha = $">> [{i + 1}]  {_opcoesDisponiveis[i]}  <<";
                    RenderizadorUI.DesenharLinhaCentralizada(linha, 0, ConsoleColor.Green, ConsoleColor.Cyan);
                }
                else
                {
                    string linha = $"   [{i + 1}]  {_opcoesDisponiveis[i]}   ";
                    RenderizadorUI.DesenharLinhaCentralizada(linha, 0, ConsoleColor.Gray, ConsoleColor.Cyan);
                }
            }

            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.Cyan);
            Console.WriteLine();
        }

        private void DesenharRodapeControles()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  [▲/▼ ou W/S] Selecionar Ação | [ENTER / Espaço] Confirmar Diretriz | [1-4 / ESC] Atalho rápido");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
