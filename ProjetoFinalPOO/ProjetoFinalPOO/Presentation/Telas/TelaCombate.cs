using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Música;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela e Orquestrador Visual do Sistema de Combate Tático 3v3 inspirado em Limbus Company.
    /// Utiliza o ControladorCombate para resolução de embates de moedas, afinidades e turnos,
    /// e executa animações cinematográficas de combate no console com buffer duplo e controle de câmera.
    /// </summary>
    public class TelaCombate : ITela
    {
        private readonly Slot[] _slotsAliados;
        private readonly Slot[] _slotsInimigos;
        private readonly List<Item> _inventarioEquipe;
        private readonly string _nomeEncontro;
        private readonly bool _ehChefe;
        private readonly ControladorCombate _controladorCombate;

        private int _rodadaAtual;
        private List<Combatente> _ordemIniciativa;
        private Combatente _combatenteAtivo;
        private Combatente _inimigoHovered;
        private readonly List<string> _logBatalha;
        private readonly Random _rng;

        public int DanoTotalCausadoPelaTripulacao { get; private set; }
        public int InimigosDerrotados { get; private set; }

        public TelaCombate(
            List<Combatente> tripulacao,
            List<Combatente> inimigos,
            List<Item> inventarioEquipe,
            string nomeEncontro = "Patrulha Espacial",
            bool ehChefe = false)
        {
            _slotsAliados = new Slot[3];
            _slotsInimigos = new Slot[3];
            _inventarioEquipe = inventarioEquipe ?? new List<Item>();
            _nomeEncontro = nomeEncontro;
            _ehChefe = ehChefe;
            _rodadaAtual = 1;
            _logBatalha = new List<string>();
            _rng = new Random();
            DanoTotalCausadoPelaTripulacao = 0;
            InimigosDerrotados = 0;

            _controladorCombate = ControladorCombate.Instancia();

            // Inicializa slots dos aliados com as instâncias singleton / tripulação
            for (int i = 0; i < 3; i++)
            {
                var c = (tripulacao != null && i < tripulacao.Count)
                    ? tripulacao[i]
                    : (i == 0 ? BancoHabilidades.CriarSentinela() : i == 1 ? BancoHabilidades.CriarEngenheiro() : BancoHabilidades.CriarBiomancer());
                _slotsAliados[i] = new Slot(i + 1, c);
            }

            // Inicializa slots dos inimigos
            for (int i = 0; i < 3; i++)
            {
                var c = (inimigos != null && i < inimigos.Count) ? inimigos[i] : null;
                _slotsInimigos[i] = new Slot(i + 1, c);
            }

            // Configura habilidades disponíveis iniciais se vazias
            foreach (var slot in _slotsAliados.Concat(_slotsInimigos))
            {
                if (slot.Combatente != null)
                {
                    if (slot.Combatente.Habilidades.Count == 0)
                    {
                        slot.Combatente.AdicionarHabilidade(BancoHabilidades.ObterHabilidadesInimigo(slot.Combatente.Nome));
                    }
                    if (slot.Combatente.HabilidadesDisponiveis.Count == 0)
                    {
                        slot.Combatente.AdicionarHabilidadesDisponiveis(slot.Combatente.Habilidades);
                    }
                }
            }

            // Inicializa o ControladorCombate com a lista de inimigos
            var listaInimigos = _slotsInimigos.Where(s => s.Combatente != null).Select(s => s.Combatente).ToList();
            _controladorCombate.IniciarCombate(listaInimigos);

            _logBatalha.Add($"Engajamento tático iniciado em {_nomeEncontro.ToUpper()}!");
            _logBatalha.Add("Iniciativa calculada por Agilidade. Embates decididos por moedas e afinidades.");
        }

        public void Entrar() { }
        public void Atualizar() { }
        public void Sair() { }

        public bool Executar()
        {
            while (true)
            {
                // Início de nova rodada no ControladorCombate
                PrepararNovaRodada();

                // Execução dos turnos em ordem de iniciativa consumindo o ControladorCombate
                while (_controladorCombate.VerificarFimDeRodada())
                {
                    var (ativo, ehAliado) = _controladorCombate.AcaoProximoCombatente();
                    if (ativo == null || ativo.EstaMorto) continue;

                    _combatenteAtivo = ativo;

                    if (ehAliado)
                    {
                        ExecutarTurnoAliado(ativo);
                    }
                    else
                    {
                        ExecutarTurnoInimigo(ativo);
                    }

                    // Checa condições de fim de combate consumindo o ControladorCombate
                    if (VerificarFimDeCombate(out bool vitoriaAliada))
                    {
                        ExibirFimDeCombate(vitoriaAliada);
                        return vitoriaAliada;
                    }
                }

                _rodadaAtual++;
            }
        }

        private void PrepararNovaRodada()
        {
            // Reseta turnos dos slots
            foreach (var slot in _slotsAliados.Concat(_slotsInimigos))
            {
                slot.ResetarTurno();
            }

            // Inicia rodada no ControladorCombate (calcula ordem, habilidades disponíveis e intenções de ataque dos inimigos)
            _controladorCombate.IniciarRodada();
            _ordemIniciativa = _controladorCombate.Ordem.ToList();

            // Atualiza intenções visuais nos slots inimigos utilizando o ControladorCombate
            foreach (var slotInimigo in _slotsInimigos.Where(s => s.Combatente != null && !s.Combatente.EstaMorto))
            {
                if (_controladorCombate.IntencaoAtaqueInimigos.TryGetValue(slotInimigo.Combatente, out var habIntencao))
                {
                    slotInimigo.HabilidadePlanejada = habIntencao;
                    Combatente alvoEscolhido = _controladorCombate.DecidirAlvoAtaqueSemOposicao();
                    slotInimigo.AlvoPlanejadoSlot = Array.FindIndex(_slotsAliados, s => s.Combatente == alvoEscolhido);
                }
            }

            _logBatalha.Add($"--- [ INÍCIO DA RODADA {_controladorCombate.Rodada} ] ---");
        }

        private void ExecutarTurnoAliado(Combatente aliado)
        {
            var slotAliado = _slotsAliados.FirstOrDefault(s => s.Combatente == aliado);
            if (slotAliado == null || slotAliado.JaAtacouNestaRodada) return;

            bool turnoConcluido = false;
            OpcaoMenuCombate opcaoMenu = OpcaoMenuCombate.Atacar;

            while (!turnoConcluido)
            {
                Renderizar();
                DesenharCaixaEscolhaAcaoAliado(aliado, opcaoMenu);

                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        opcaoMenu = (opcaoMenu == OpcaoMenuCombate.Atacar) ? OpcaoMenuCombate.Defender : (opcaoMenu - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        opcaoMenu = (opcaoMenu == OpcaoMenuCombate.Defender) ? OpcaoMenuCombate.Atacar : (opcaoMenu + 1);
                        break;

                    case ConsoleKey.D1:
                        opcaoMenu = OpcaoMenuCombate.Atacar;
                        turnoConcluido = ProcessarAtaqueAliado(aliado, slotAliado);
                        break;

                    case ConsoleKey.D2:
                        opcaoMenu = OpcaoMenuCombate.UsarItem;
                        turnoConcluido = ProcessarUsoItemAliado(aliado);
                        break;

                    case ConsoleKey.D3:
                        opcaoMenu = OpcaoMenuCombate.Defender;
                        ProcessarDefesaAliado(aliado, slotAliado);
                        turnoConcluido = true;
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        switch (opcaoMenu)
                        {
                            case OpcaoMenuCombate.Atacar:
                                turnoConcluido = ProcessarAtaqueAliado(aliado, slotAliado);
                                break;
                            case OpcaoMenuCombate.UsarItem:
                                turnoConcluido = ProcessarUsoItemAliado(aliado);
                                break;
                            case OpcaoMenuCombate.Defender:
                                ProcessarDefesaAliado(aliado, slotAliado);
                                turnoConcluido = true;
                                break;
                        }
                        break;
                }
            }

            slotAliado.JaAtacouNestaRodada = true;
        }

        private bool ProcessarAtaqueAliado(Combatente aliado, Slot slotAliado)
        {
            var cartasTotais = aliado.Habilidades;
            if (cartasTotais == null || cartasTotais.Count == 0)
            {
                _logBatalha.Add($"[!] {aliado.Nome} não possui habilidades equipadas!");
                return false;
            }

            // Verifica se há ao menos uma habilidade disponível com moedas ativas
            if (aliado.HabilidadesDisponiveis.Count == 0 || !aliado.HabilidadesDisponiveis.Any(h => h.Moeda > 0))
            {
                _logBatalha.Add($"[!] {aliado.Nome} não possui habilidades disponíveis para atacar!");
                return false;
            }

            int primeiroDisp = cartasTotais.FindIndex(h => aliado.HabilidadesDisponiveis.Exists(hd => hd.Id == h.Id && hd.Moeda > 0));
            int cartaIdx = primeiroDisp >= 0 ? primeiroDisp : 0;
            bool escolhendoCarta = true;

            // 1. Escolha da Habilidade
            while (escolhendoCarta)
            {
                Renderizar();
                DesenharPainelSelecaoCartas(aliado, cartaIdx);

                ConsoleKeyInfo tecla = Console.ReadKey(true);

                switch (tecla.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        cartaIdx = (cartaIdx - 1 + cartasTotais.Count) % cartasTotais.Count;
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        cartaIdx = (cartaIdx + 1) % cartasTotais.Count;
                        break;

                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                        int numIdx = tecla.Key - ConsoleKey.D1;
                        if (numIdx >= 0 && numIdx < cartasTotais.Count)
                        {
                            cartaIdx = numIdx;
                            var habSel = cartasTotais[cartaIdx];
                            bool disp = aliado.HabilidadesDisponiveis.Exists(hd => hd.Id == habSel.Id && hd.Moeda > 0);
                            if (disp)
                            {
                                escolhendoCarta = false;
                            }
                            else
                            {
                                _logBatalha.Add($"[!] A habilidade '{habSel.Nome}' já foi gasta! Escolha outra.");
                            }
                        }
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        var habAtual = cartasTotais[cartaIdx];
                        bool habDisp = aliado.HabilidadesDisponiveis.Exists(hd => hd.Id == habAtual.Id && hd.Moeda > 0);
                        if (habDisp)
                        {
                            escolhendoCarta = false;
                        }
                        else
                        {
                            _logBatalha.Add($"[!] A habilidade '{habAtual.Nome}' já foi gasta! Escolha outra.");
                        }
                        break;

                    case ConsoleKey.Escape:
                        return false;
                }
            }

            var cartaEscolhida = cartasTotais[cartaIdx];
            var habilidadeEscolhida = aliado.HabilidadesDisponiveis.FirstOrDefault(hd => hd.Id == cartaEscolhida.Id && hd.Moeda > 0);
            if (habilidadeEscolhida == null)
            {
                _logBatalha.Add($"[!] A habilidade '{cartaEscolhida.Nome}' não está disponível!");
                return false;
            }

            // 2. Escolha do Alvo Inimigo
            var inimigosVivos = _slotsInimigos.Where(s => s.Combatente != null && !s.Combatente.EstaMorto).ToList();
            if (inimigosVivos.Count == 0) return true;

            int alvoIdx = 0;
            bool escolhendoAlvo = true;

            while (escolhendoAlvo)
            {
                _inimigoHovered = inimigosVivos[alvoIdx].Combatente;
                Renderizar();
                DesenharPainelSelecaoAlvoInimigo(inimigosVivos, alvoIdx, habilidadeEscolhida);

                ConsoleKeyInfo tecla = Console.ReadKey(true);

                if (tecla.Key == ConsoleKey.LeftArrow || tecla.Key == ConsoleKey.A)
                    alvoIdx = (alvoIdx - 1 + inimigosVivos.Count) % inimigosVivos.Count;
                else if (tecla.Key == ConsoleKey.RightArrow || tecla.Key == ConsoleKey.D)
                    alvoIdx = (alvoIdx + 1) % inimigosVivos.Count;
                else if (tecla.Key >= ConsoleKey.D1 && tecla.Key < ConsoleKey.D1 + inimigosVivos.Count)
                {
                    alvoIdx = tecla.Key - ConsoleKey.D1;
                    _inimigoHovered = inimigosVivos[alvoIdx].Combatente;
                    escolhendoAlvo = false;
                }
                else if (tecla.Key == ConsoleKey.Enter || tecla.Key == ConsoleKey.Spacebar)
                {
                    escolhendoAlvo = false;
                }
                else if (tecla.Key == ConsoleKey.Escape)
                {
                    _inimigoHovered = null;
                    return false;
                }
            }

            _inimigoHovered = null;
            var slotAlvoInimigo = inimigosVivos[alvoIdx];

            slotAliado.HabilidadePlanejada = habilidadeEscolhida;
            slotAliado.AlvoPlanejadoSlot = Array.IndexOf(_slotsInimigos, slotAlvoInimigo);

            // 3. Execução do Embate ou Ataque Unilateral
            ExecutarResolucaoAcao(slotAliado, slotAlvoInimigo, atacanteEhAliado: true);
            return true;
        }

        private void ProcessarDefesaAliado(Combatente aliado, Slot slotAliado)
        {
            slotAliado.Defendendo = true;
            aliado.Defender();
            _logBatalha.Add($"[DEFESA & RECUPERAÇÃO] {aliado.Nome} adotou Postura Defensiva (+5% HP e reforço na blindagem)!");
        }

        private bool ProcessarUsoItemAliado(Combatente aliado)
        {
            if (_inventarioEquipe.Count == 0)
            {
                _logBatalha.Add("[!] O compartimento de itens está vazio!");
                return false;
            }

            int itemIdx = 0;
            bool escolhendoItem = true;

            while (escolhendoItem)
            {
                Renderizar();
                DesenharPainelSelecaoItem(itemIdx);

                ConsoleKeyInfo tecla = Console.ReadKey(true);

                if (tecla.Key == ConsoleKey.UpArrow || tecla.Key == ConsoleKey.W)
                    itemIdx = (itemIdx - 1 + _inventarioEquipe.Count) % _inventarioEquipe.Count;
                else if (tecla.Key == ConsoleKey.DownArrow || tecla.Key == ConsoleKey.S)
                    itemIdx = (itemIdx + 1) % _inventarioEquipe.Count;
                else if (tecla.Key == ConsoleKey.Enter || tecla.Key == ConsoleKey.Spacebar)
                    escolhendoItem = false;
                else if (tecla.Key == ConsoleKey.Escape)
                    return false;
            }

            var item = _inventarioEquipe[itemIdx];
            _inventarioEquipe.RemoveAt(itemIdx);

            if (item.Tipo == TipoItem.GranadaFogo || item.Tipo == TipoItem.GranadaEletrica || item.Tipo == TipoItem.GranadaAcido)
            {
                var inimigosVivos = _slotsInimigos.Where(s => s.Combatente != null && !s.Combatente.EstaMorto).ToList();
                if (inimigosVivos.Count > 0)
                {
                    var inimigoAlvo = inimigosVivos[0].Combatente;
                    item.Usar(aliado, inimigoAlvo, _logBatalha);
                    DanoTotalCausadoPelaTripulacao += item.ValorEfeito;
                    if (inimigoAlvo.EstaMorto) InimigosDerrotados++;
                }
            }
            else
            {
                item.Usar(aliado, aliado, _logBatalha);
            }

            return true;
        }

        private void ExecutarTurnoInimigo(Combatente inimigo)
        {
            var slotInimigo = _slotsInimigos.FirstOrDefault(s => s.Combatente == inimigo);
            if (slotInimigo == null || slotInimigo.JaAtacouNestaRodada) return;

            var aliadosVivos = _slotsAliados.Where(s => s.Combatente != null && !s.Combatente.EstaMorto).ToList();
            if (aliadosVivos.Count == 0) return;

            Slot slotAlvo = null;
            if (slotInimigo.AlvoPlanejadoSlot >= 0 && slotInimigo.AlvoPlanejadoSlot < 3)
            {
                var candidato = _slotsAliados[slotInimigo.AlvoPlanejadoSlot];
                if (candidato.Combatente != null && !candidato.Combatente.EstaMorto)
                {
                    slotAlvo = candidato;
                }
            }

            slotAlvo ??= _slotsAliados.FirstOrDefault(s => s.Combatente != null && !s.Combatente.EstaMorto);
            if (slotAlvo == null) return;

            if (slotInimigo.HabilidadePlanejada == null)
            {
                if (inimigo.HabilidadesDisponiveis.Count > 0)
                    slotInimigo.HabilidadePlanejada = inimigo.HabilidadesDisponiveis[_rng.Next(inimigo.HabilidadesDisponiveis.Count)];
                else if (inimigo.Habilidades.Count > 0)
                    slotInimigo.HabilidadePlanejada = inimigo.Habilidades[_rng.Next(inimigo.Habilidades.Count)];
            }

            ExecutarResolucaoAcao(slotInimigo, slotAlvo, atacanteEhAliado: false);
            slotInimigo.JaAtacouNestaRodada = true;
            slotInimigo.HabilidadePlanejada = null;
            slotInimigo.AlvoPlanejadoSlot = -1;
            _controladorCombate.IntencaoAtaqueInimigos.Remove(inimigo);
        }

        private void ExecutarResolucaoAcao(Slot slotAtacante, Slot slotDefensor, bool atacanteEhAliado)
        {
            var atacante = slotAtacante.Combatente;
            var defensor = slotDefensor.Combatente;
            var habAtacante = slotAtacante.HabilidadePlanejada;

            if (atacante == null || defensor == null || habAtacante == null) return;

            // Checa se o alvo possui intenção de ataque ainda pendente
            bool alvoTemIntencao = _controladorCombate.VerificarAlvoNaoAtacouRodada(defensor) && !slotDefensor.JaAtacouNestaRodada && !slotDefensor.Defendendo;

            if (!alvoTemIntencao)
            {
                // =========================================================================
                // ATAQUE UNILATERAL (SEM OPOSIÇÃO)
                // =========================================================================
                int danoAplicado;
                if (!atacanteEhAliado && _controladorCombate.IntencaoAtaqueInimigos.ContainsKey(atacante))
                {
                    danoAplicado = _controladorCombate.InimigoAtacaSemOposicao(atacante);
                }
                else
                {
                    danoAplicado = _controladorCombate.Atacar(defensor, atacante, habAtacante);
                }

                var moedasAtacante = RolarMoedasDetalhado(atacante, habAtacante, out int poderAtacante);

                var resultado = new ResultadoEmbate(
                    atacante: atacante,
                    defensor: defensor,
                    habilidadeAtacante: habAtacante,
                    habilidadeDefensor: null,
                    moedasAtacante: moedasAtacante,
                    moedasDefensor: new List<bool>(),
                    poderFinalAtacante: poderAtacante,
                    poderFinalDefensor: 0,
                    vitoriaAtacante: true,
                    ehAtaqueUnilateral: true,
                    multiplicadorAfinidade: 1.0,
                    danoCausado: danoAplicado,
                    mensagemLog: $"[ATAQUE UNILATERAL] {atacante.Nome} desferiu {danoAplicado} de dano contra {defensor.Nome}!",
                    atacanteEhAliado: atacanteEhAliado
                );

                ExecutarAnimacaoEmbate(atacante, defensor, resultado, atacanteEhAliado);

                _logBatalha.Add($"[ATAQUE UNILATERAL] {atacante.Nome} usou '{habAtacante.Nome}' contra {defensor.Nome}!");
                _logBatalha.Add($"  -> Dano aplicado: {danoAplicado} HP em {defensor.Nome} (Vida Restante: {defensor.VidaAtual}/{defensor.VidaTotal})");

                if (atacanteEhAliado)
                {
                    DanoTotalCausadoPelaTripulacao += danoAplicado;
                    if (defensor.EstaMorto)
                    {
                        InimigosDerrotados++;
                        slotDefensor.HabilidadePlanejada = null;
                        slotDefensor.AlvoPlanejadoSlot = -1;
                        _controladorCombate.IntencaoAtaqueInimigos.Remove(defensor);
                        _logBatalha.Add($"[ALVO ABATIDO] {defensor.Nome} foi neutralizado!");
                    }
                }
            }
            else
            {
                // =========================================================================
                // EMBATE / CLASH (CONTROLADOR DE COMBATE COM MOEDAS)
                // =========================================================================
                var habDefensor = _controladorCombate.IntencaoAtaqueInimigos.TryGetValue(defensor, out var hDef) ? hDef : defensor.Habilidades.FirstOrDefault();
                _logBatalha.Add($"[EMBATE TÁTICO] {atacante.Nome} ({habAtacante.Nome}) VS {defensor.Nome} ({habDefensor?.Nome ?? "Ataque"})");

                Combatente vencedor;
                if (atacanteEhAliado)
                {
                    vencedor = _controladorCombate.RealizarEmbate(defensor, atacante, habAtacante);
                }
                else
                {
                    vencedor = _controladorCombate.RealizarEmbate(atacante, defensor, habDefensor);
                }

                bool atacanteVenceu = (vencedor == atacante);
                var perdedor = atacanteVenceu ? defensor : atacante;
                var habVencedor = atacanteVenceu ? habAtacante : habDefensor;
                var habPerdedor = atacanteVenceu ? habDefensor : habAtacante;

                int danoAplicado = _controladorCombate.Atacar(perdedor, vencedor, habVencedor);

                var moedasVencedor = RolarMoedasDetalhado(vencedor, habVencedor, out int poderVencedor);
                var moedasPerdedor = RolarMoedasDetalhado(perdedor, habPerdedor, out int poderPerdedor);

                var resultado = new ResultadoEmbate(
                    atacante: vencedor,
                    defensor: perdedor,
                    habilidadeAtacante: habVencedor,
                    habilidadeDefensor: habPerdedor,
                    moedasAtacante: moedasVencedor,
                    moedasDefensor: moedasPerdedor,
                    poderFinalAtacante: poderVencedor,
                    poderFinalDefensor: poderPerdedor,
                    vitoriaAtacante: true,
                    ehAtaqueUnilateral: false,
                    multiplicadorAfinidade: 1.0,
                    danoCausado: danoAplicado,
                    mensagemLog: $"[VITÓRIA NO EMBATE] {vencedor.Nome} superou {perdedor.Nome} causando {danoAplicado} HP!",
                    atacanteEhAliado: (vencedor == atacante ? atacanteEhAliado : !atacanteEhAliado)
                );

                ExecutarAnimacaoEmbate(vencedor, perdedor, resultado, resultado.AtacanteEhAliado);

                _logBatalha.Add($"  -> {vencedor.Nome} venceu o embate e desferiu {danoAplicado} HP em {perdedor.Nome}!");

                // O defensor gasta sua intenção de ataque no embate
                slotDefensor.JaAtacouNestaRodada = true;
                slotDefensor.HabilidadePlanejada = null;
                slotDefensor.AlvoPlanejadoSlot = -1;
                _controladorCombate.IntencaoAtaqueInimigos.Remove(defensor);

                if (resultado.AtacanteEhAliado)
                {
                    DanoTotalCausadoPelaTripulacao += danoAplicado;
                    if (perdedor.EstaMorto)
                    {
                        InimigosDerrotados++;
                        _logBatalha.Add($"[ALVO ABATIDO] {perdedor.Nome} foi destruído no embate!");
                    }
                }
            }

            Renderizar();
            Thread.Sleep(400);
        }

        private List<bool> RolarMoedasDetalhado(Combatente c, Habilidade h, out int poderTotal)
        {
            var moedas = new List<bool>();
            if (c == null || h == null)
            {
                poderTotal = 0;
                return moedas;
            }

            Random rnd = new Random();
            int threshold = c switch
            {
                Sentinela s => s.Adrenalina,
                Engenheiro e => e.Sobreaquecimento,
                Biomancer b => b.Mana,
                _ => 50
            };

            poderTotal = h.PoderBase;
            for (int i = 0; i < h.Moeda; i++)
            {
                bool cara = rnd.Next(threshold, 100) > 50;
                moedas.Add(cara);
                if (cara)
                {
                    poderTotal += h.PoderAdicionalMoeda;
                }
            }

            return moedas;
        }

        private void ExecutarAnimacaoEmbate(Combatente atacante, Combatente defensor, ResultadoEmbate resultado, bool atacanteEhAliado)
        {
            if (atacante != null && defensor != null && resultado != null)
            {
                int largura = ConfiguradorTela.ObterLarguraConsole();
                int altura = Math.Min(32, ConfiguradorTela.ObterAlturaConsole());
                var animador = new AnimadorEmbateConsole(largura, altura);
                animador.ExecutarAnimacao(atacante, defensor, resultado, atacanteEhAliado);
            }
        }

        private bool VerificarFimDeCombate(out bool vitoriaAliada)
        {
            bool fim = _controladorCombate.VerificarFimDeCombate();
            if (fim)
            {
                vitoriaAliada = _controladorCombate.CombatentesInimigos.All(i => i == null || i.EstaMorto);
                return true;
            }

            vitoriaAliada = false;
            return false;
        }

        private void ExibirFimDeCombate(bool vitoria)
        {
            Console.Clear();
            ConsoleColor corTema = vitoria ? ConsoleColor.Green : ConsoleColor.Red;
            string titulo = vitoria ? "VITÓRIA NO COMBATE ESPACIAL - SETOR LIMPO!" : "DERROTA - TODOS OS TRIPULANTES FORAM ABATIDOS";

            RenderizadorUI.DesenharCabecalho(titulo, 0, corTema);
            Console.WriteLine();

            RenderizadorUI.DesenharInicioSecao("RELATÓRIO PÓS-BATALHA", 0, corTema);
            if (vitoria)
            {
                try { BibliotecaDeMusicas.FanfarraDeConquista()?.Tocar(); } catch { }
                int expGanha = _ehChefe ? 150 : 60;
                RenderizadorUI.DesenharLinhaCentralizada("A tripulação neutralizou todas as ameaças com sucesso!", 0, ConsoleColor.Yellow, corTema);
                RenderizadorUI.DesenharLinhaCentralizada($"Recompensas: +{expGanha} EXP para todos os tripulantes vivos | +80 Créditos Espaciais", 0, ConsoleColor.White, corTema);

                foreach (var slot in _slotsAliados.Where(s => s.Combatente != null && !s.Combatente.EstaMorto))
                {
                    RenderizadorUI.DesenharLinhaConteudo($"  - {slot.Combatente.Nome}: EXP acumulada +{expGanha}!", 0, ConsoleColor.Green, corTema);
                }
            }
            else
            {
                RenderizadorUI.DesenharLinhaCentralizada("A nave Vanguarda foi cercada e neutralizada pelas forças inimigas.", 0, ConsoleColor.Red, corTema);
            }

            RenderizadorUI.DesenharFimSecao(0, corTema);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione ENTER, ESPAÇO ou ESC para prosseguir... ]");
            Console.ResetColor();
            if (!Console.IsInputRedirected)
            {
                try
                {
                    while (Console.KeyAvailable) Console.ReadKey(true);
                    while (true)
                    {
                        ConsoleKey k = Console.ReadKey(true).Key;
                        if (k == ConsoleKey.Enter || k == ConsoleKey.Spacebar || k == ConsoleKey.Escape) break;
                    }
                }
                catch { }
            }
        }

        public void Renderizar()
        {
            Limpar();
            RenderizadorUI.DesenharCabecalho($"ENGAJAMENTO TÁTICO 3V3 - {_nomeEncontro.ToUpper()} (RODADA {_rodadaAtual})", 0, ConsoleColor.Red);
            Console.WriteLine();

            // Cards dos Inimigos
            RenderizadorUI.DesenharInicioSecao("ESQUADRÃO INIMIGO", 0, ConsoleColor.DarkRed);
            RenderizadorUI.DesenharTresCardsCombatentes(_slotsInimigos, _combatenteAtivo, saoInimigos: true, _slotsAliados, _inimigoHovered);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkRed);
            Console.WriteLine();

            // Cards dos Aliados
            RenderizadorUI.DesenharInicioSecao("TRIPULAÇÃO DA NAVE VANGUARDA", 0, ConsoleColor.DarkCyan);
            RenderizadorUI.DesenharTresCardsCombatentes(_slotsAliados, _combatenteAtivo, saoInimigos: false, _slotsInimigos);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkCyan);
            Console.WriteLine();

            // Log de Batalha
            DesenharLogBatalha();
        }

        private void DesenharLogBatalha()
        {
            RenderizadorUI.DesenharInicioSecao("REGISTRO DE TELEMETRIA E EMBATES", 0, ConsoleColor.DarkGray);
            int ultimasLinhas = Math.Max(0, _logBatalha.Count - 4);
            for (int i = ultimasLinhas; i < _logBatalha.Count; i++)
            {
                RenderizadorUI.DesenharLinhaConteudo(_logBatalha[i], 0, ConsoleColor.Gray, ConsoleColor.DarkGray);
            }
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        private void DesenharCaixaEscolhaAcaoAliado(Combatente aliado, OpcaoMenuCombate opcaoSelecionada)
        {
            RenderizadorUI.DesenharInicioSecao($"TURNO TÁTICO: {aliado.Nome.ToUpper()} ({aliado.GetType().Name})", 0, ConsoleColor.Green);

            var opcoes = new (OpcaoMenuCombate Opcao, string Texto)[]
            {
                (OpcaoMenuCombate.Atacar, "[1] Atacar com Habilidade"),
                (OpcaoMenuCombate.UsarItem, "[2] Usar Item do Inventário"),
                (OpcaoMenuCombate.Defender, "[3] Postura Defensiva (+5% HP e Blindagem)")
            };

            foreach (var (opcao, texto) in opcoes)
            {
                bool sel = (opcao == opcaoSelecionada);
                string prefixo = sel ? "  ►► " : "     ";
                ConsoleColor cor = sel ? ConsoleColor.Yellow : ConsoleColor.White;
                RenderizadorUI.DesenharLinhaConteudo(prefixo + texto, 0, cor, ConsoleColor.Green);
            }

            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.Green);
            Console.WriteLine();
        }

        private void DesenharPainelSelecaoCartas(Combatente aliado, int indiceSelecionado)
        {
            RenderizadorUI.DesenharInicioSecao($"HABILIDADES DE COMBATE: {aliado.Nome.ToUpper()} (◄/► para escolher, ENTER para confirmar, ESC voltar)", 0, ConsoleColor.Yellow);
            RenderizadorUI.DesenharMaoCartasCombate(aliado, indiceSelecionado);
            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.Yellow);
            Console.WriteLine();
        }

        private void DesenharPainelSelecaoAlvoInimigo(List<Slot> inimigosVivos, int alvoSelecionado, Habilidade hab)
        {
            RenderizadorUI.DesenharInicioSecao($"ESCOLHA O ALVO PARA '{hab.Nome.ToUpper()}' [{hab.Afinidade}] (◄/► ou 1-{inimigosVivos.Count} para mirar, ENTER disparar, ESC voltar)", 0, ConsoleColor.Red);

            for (int i = 0; i < inimigosVivos.Count; i++)
            {
                var slotInimigo = inimigosVivos[i];
                var inimigo = slotInimigo.Combatente;
                bool sel = (i == alvoSelecionado);
                string prefixo = sel ? "  ►► " : "     ";
                ConsoleColor cor = sel ? ConsoleColor.Yellow : ConsoleColor.White;

                string texto = $"{prefixo}[{i + 1}] {inimigo.Nome} | HP: {inimigo.VidaAtual}/{inimigo.VidaTotal} | Blindagem: [{inimigo.Afinidade}] | Def: {inimigo.Defesa}";
                RenderizadorUI.DesenharLinhaConteudo(texto, 0, cor, ConsoleColor.Red);
            }

            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.Red);
            Console.WriteLine();
        }

        private void DesenharPainelSelecaoItem(int itemSelecionado)
        {
            RenderizadorUI.DesenharInicioSecao("COMPARTIMENTO DE ITENS DA TRIPULAÇÃO (▲/▼ para navegar, ENTER usar, ESC voltar)", 0, ConsoleColor.Yellow);

            for (int i = 0; i < _inventarioEquipe.Count; i++)
            {
                var item = _inventarioEquipe[i];
                bool sel = (i == itemSelecionado);
                string prefixo = sel ? "  ►► " : "     ";
                ConsoleColor cor = sel ? ConsoleColor.Yellow : ConsoleColor.White;

                string texto = $"{prefixo}[{i + 1}] {item.Nome} [{item.Raridade}] - Efeito: +{item.ValorEfeito} ({item.Tipo})";
                RenderizadorUI.DesenharLinhaConteudo(texto, 0, cor, ConsoleColor.Yellow);
            }

            RenderizadorUI.DesenharFimSecao(0, ConsoleColor.Yellow);
            Console.WriteLine();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
