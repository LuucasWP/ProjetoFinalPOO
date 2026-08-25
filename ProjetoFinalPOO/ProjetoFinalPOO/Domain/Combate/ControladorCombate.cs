using System;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinalPOO
{
    public class ControladorCombate
    {
        private static ControladorCombate _instancia;
        private List<Combatente> _combatentesAliados { get; set; }
        private List<Combatente> _combatentesInimigos { get; set; }
        private int _rodada { get; set; }
        private List<Combatente> _ordem { get; set; }
        private Dictionary<Combatente, Habilidade> _intencaoAtaqueInimigos { get; set; }
        private Dictionary<Combatente, Combatente> _intencaoAlvoAtaqueInimigos { get; set; }

        public List<Combatente> CombatentesAliados => _combatentesAliados;
        public List<Combatente> CombatentesInimigos => _combatentesInimigos;
        public List<Combatente> Ordem => _ordem;
        public int Rodada => _rodada;
        public Dictionary<Combatente, Habilidade> IntencaoAtaqueInimigos => _intencaoAtaqueInimigos;
        public Dictionary<Combatente, Combatente> IntencaoAlvoAtaqueInimigos => _intencaoAlvoAtaqueInimigos;

        private ControladorCombate()
        {
            _combatentesInimigos = new List<Combatente>();
            _combatentesAliados = new List<Combatente>()
            {
                Sentinela.Instancia(),
                Biomancer.Instancia(),
                Engenheiro.Instancia()
            };
            _ordem = new List<Combatente>();
            _intencaoAtaqueInimigos = new Dictionary<Combatente, Habilidade>();
            _intencaoAlvoAtaqueInimigos = new Dictionary<Combatente, Combatente>();
        }

        public static ControladorCombate Instancia()
        {
            if (_instancia == null)
                _instancia = new ControladorCombate();
            return _instancia;
        }

        public void IniciarCombate(List<Combatente> inimigos)
        {
            _combatentesInimigos.Clear();
            _combatentesInimigos.AddRange(inimigos);
            _rodada = 0;
        }

        public (Combatente CombatenteAtual, bool eAliado) AcaoProximoCombatente()
        {
            Combatente CombatenteAtual = _ordem.First();

            if (_combatentesAliados.Exists(c => c == CombatenteAtual))
            {
                RemoverDaOrdem(CombatenteAtual);
                return (CombatenteAtual, true);
            }

            RemoverDaOrdem(CombatenteAtual);
            return (CombatenteAtual, false);
        }

        public void IniciarRodada()
        {
            _rodada++;
            OrdenarOrdemCombatentes();
            AdicionarHabilidadesDisponiveis();
            CriarIntencaoAtaqueInimigos();
            ResetarDefesa();
        }

        private void OrdenarOrdemCombatentes()
        {
            _ordem.Clear();
            List<Combatente> ordem = new List<Combatente>();

            ordem.AddRange(_combatentesAliados.Where(c => !c.EstaMorto));
            ordem.AddRange(_combatentesInimigos.Where(c => !c.EstaMorto));
            _ordem = ordem.OrderByDescending(c => c.Agilidade).ToList();
        }

        private void AdicionarHabilidadesDisponiveis()
        {
            foreach (Combatente combatente in _ordem)
            {
                if (combatente.HabilidadesDisponiveis.Count == 0)
                    combatente.AdicionarHabilidadesDisponiveis(combatente.Habilidades);
            }
        }

        private void CriarIntencaoAtaqueInimigos()
        {
            foreach (Combatente inimigo in _combatentesInimigos.Where(i => !i.EstaMorto))
            {
                Random rnd = new Random();
                int indexHabilidade = rnd.Next(inimigo._habilidadesDisponiveis.Count);
                _intencaoAtaqueInimigos.Add(inimigo, inimigo.HabilidadesDisponiveis[indexHabilidade]);
                _intencaoAlvoAtaqueInimigos.Add(inimigo, DecidirAlvoAtaqueInimigo());
            }
        }

        private void ResetarDefesa()
        {
            foreach (Combatente combatente in _ordem)
                combatente._estaDefendendo = false;
        }

        private void RemoverIntencaoAtaqueInimigo(Combatente inimigo)
        {
            if (!IntencaoAtaqueInimigos.ContainsKey(inimigo))
                return;
            if (!IntencaoAlvoAtaqueInimigos.ContainsKey(inimigo))
                return;

            _intencaoAtaqueInimigos.Remove(inimigo);
            _intencaoAlvoAtaqueInimigos.Remove(inimigo);

            RemoverDaOrdem(inimigo);
        }


        public Combatente RealizarEmbate(Combatente alvo, Combatente aliadoAtacando, Habilidade habilidadeSelecionada)
        {
            Habilidade habilidadeDoAlvo = IntencaoAtaqueInimigos[alvo];

            do
            {
                int PoderHabilidadeAlvo = alvo.CalcularPoderBase(habilidadeDoAlvo);
                int PoderHabilidadeSelecionada = aliadoAtacando.CalcularPoderBase(habilidadeSelecionada);

                if (PoderHabilidadeSelecionada == PoderHabilidadeAlvo)
                    continue;
                else if (PoderHabilidadeSelecionada > PoderHabilidadeAlvo)
                {
                    alvo.RemoverMoeda(habilidadeDoAlvo);
                }
                else
                    aliadoAtacando.RemoverMoeda(habilidadeSelecionada);
            }
            while (alvo.HabilidadesDisponiveis.Find(h => h.Id == habilidadeDoAlvo.Id).Moeda > 0 &&
                    aliadoAtacando.HabilidadesDisponiveis.Find(h => h.Id == habilidadeSelecionada.Id).Moeda > 0);

            if (alvo.HabilidadesDisponiveis.Find(h => h.Id == habilidadeDoAlvo.Id).Moeda > 0 &&
                    aliadoAtacando.HabilidadesDisponiveis.Find(h => h.Id == habilidadeSelecionada.Id).Moeda == 0)
            {
                RemoverIntencaoAtaqueInimigo(alvo);
                return alvo;
            }

            aliadoAtacando.AlterarModificador(habilidadeSelecionada.Modificador);
            RemoverIntencaoAtaqueInimigo(alvo);
            return aliadoAtacando;
        }

        public int Atacar(Combatente alvo, Combatente atacador, Habilidade habilidade)
        {
            int danoFinal = 0;

            int poderHabilidade = atacador.CalcularPoderBase(habilidade);

            decimal multiplicadorAfinidade = MultiplicadorAfinidade(alvo.Afinidade, habilidade.Afinidade);

            int defesaAlvo = alvo.Defesa;

            bool estaDefendendo = alvo.EstaDefendendo;

            danoFinal = (int)(poderHabilidade * multiplicadorAfinidade) - (estaDefendendo ? 2 * defesaAlvo : defesaAlvo);
            if (danoFinal <= 0)
                danoFinal = 1;
            alvo.ReceberDano(danoFinal);

            RemoverHabilidadeUtilizada(habilidade, atacador);
            atacador.AlterarModificador(habilidade.Modificador);

            return danoFinal;
        }


        private void RemoverHabilidadeUtilizada(Habilidade habilidade, Combatente combatente)
        {
            combatente.HabilidadesDisponiveis.Remove(habilidade);
        }

        public int InimigoAtacaSemOposicao(Combatente atacador)
        {
            Habilidade habilidadeAtacador = IntencaoAtaqueInimigos[atacador];
            Combatente alvo = IntencaoAlvoAtaqueInimigos[atacador];
            RemoverIntencaoAtaqueInimigo(atacador);

            return Atacar(alvo, atacador, habilidadeAtacador);
        }

        private Combatente DecidirAlvoAtaqueInimigo()
        {
            Random rnd = new Random();
            int aliadosVivos = CombatentesAliados.FindAll(c => !c.EstaMorto).Count;
            int aleatorio = rnd.Next(aliadosVivos);

            return CombatentesAliados[aleatorio];
        }

        private decimal MultiplicadorAfinidade(AfinidadeDefesa afinidadeDefesa, AfinidadeAtaque afinidadeAtaque)
        {
            decimal multiplicadorFinal = 0;
            switch (afinidadeDefesa)
            {
                case AfinidadeDefesa.Armadurado:
                    switch (afinidadeAtaque)
                    {
                        case AfinidadeAtaque.Acido:
                            multiplicadorFinal = 2;
                            break;
                        case AfinidadeAtaque.Eletrico:
                            multiplicadorFinal = 1;
                            break;
                        case AfinidadeAtaque.Fogo:
                            multiplicadorFinal = 0.5m;
                            break;
                    }
                    break;
                case AfinidadeDefesa.Mecanico:
                    switch (afinidadeAtaque)
                    {
                        case AfinidadeAtaque.Eletrico:
                            multiplicadorFinal = 2;
                            break;
                        case AfinidadeAtaque.Fogo:
                            multiplicadorFinal = 1;
                            break;
                        case AfinidadeAtaque.Acido:
                            multiplicadorFinal = 0.5m;
                            break;
                    }
                    break;
                case AfinidadeDefesa.Biologico:
                    switch (afinidadeAtaque)
                    {
                        case AfinidadeAtaque.Fogo:
                            multiplicadorFinal = 2;
                            break;
                        case AfinidadeAtaque.Acido:
                            multiplicadorFinal = 1;
                            break;
                        case AfinidadeAtaque.Eletrico:
                            multiplicadorFinal = 0.5m;
                            break;
                    }
                    break;
            }

            return multiplicadorFinal;
        }

        private void RemoverDaOrdem(Combatente combatente)
        {
            _ordem.Remove(combatente);
        }

        public bool VerificarFimDeRodada()
        {
            return _ordem.Any();
        }

        public bool VerificarAlvoNaoAtacouRodada(Combatente combatente)
        {
            return IntencaoAtaqueInimigos.ContainsKey(combatente);
        }

        public bool VerificarFimDeCombate()
        {
            if (_combatentesAliados.All(a => a.EstaMorto))
                return true;
            if (_combatentesInimigos.All(a => a.EstaMorto))
                return true;

            return false;
        }
    }
}