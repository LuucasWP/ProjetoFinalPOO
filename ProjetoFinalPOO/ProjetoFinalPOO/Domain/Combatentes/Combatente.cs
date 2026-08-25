using ProjetoFinalPOO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinalPOO.Combatentes
{
    public abstract class Combatente
    {
        private static int _contadorID = 0;
        public int Id { get; private set; }
        internal string _nome { get; set; }
        internal int _level { get; set; }
        internal int _exp { get; set; }
        internal int _vidaTotal { get; set; }
        internal int _vidaAtual { get; set; }
        internal int _defesa { get; set; }
        internal int _agilidade { get; set; }
        internal AfinidadeDefesa _afinidade { get; set; }
        internal List<Habilidade> _habilidades { get; set; }
        internal List<Habilidade> _habilidadesDisponiveis { get; set; }
        internal bool _estaDefendendo { get; set; }

        public string Nome => _nome;
        public int Level => _level;
        public int Exp => _exp;
        public int VidaTotal
        {
            get => _vidaTotal;
            private set
            {
                if (value < 25)
                    _vidaTotal = 25;
                else
                    _vidaTotal = value;
            }
        }
        public int VidaAtual
        {
            get => _vidaAtual;
            private set
            {
                if (value < 0)
                    _vidaAtual = 0;
                else if (value > VidaTotal)
                    _vidaAtual = VidaTotal;
                else
                    _vidaAtual = value;
            }
        }
        public int Defesa => _defesa;
        public int Agilidade => _agilidade;
        public AfinidadeDefesa Afinidade => _afinidade;
        public List<Habilidade> Habilidades => _habilidades;
        public List<Habilidade> HabilidadesDisponiveis => _habilidadesDisponiveis;

        public bool EstaMorto => VidaAtual <= 0;
        public bool EstaDefendendo => _estaDefendendo;
        protected Combatente()
        {
            _contadorID++;
            Id = _contadorID;
            _level = 1;
            _exp = 0;
            _habilidades = new List<Habilidade>();
            _habilidadesDisponiveis = new List<Habilidade>();
        }

        public void AdicionarHabilidade(List<Habilidade> habilidadesAdicionadas)
        {
            foreach (var habilidade in habilidadesAdicionadas)
            {
                int contadorHabilidade = 0;

                if (_habilidades.Count == 6)
                    return;

                contadorHabilidade = _habilidades.FindAll(x => x.Categoria == habilidade.Categoria).Count();

                switch (habilidade.Categoria)
                {
                    case CategoriaHabilidade.Basica:
                        if (contadorHabilidade < 3)
                            _habilidades.Add(habilidade);
                        break;
                    case CategoriaHabilidade.Avancada:
                        if (contadorHabilidade < 2)
                            _habilidades.Add(habilidade);
                        break;
                    case CategoriaHabilidade.Especialista:
                        if (contadorHabilidade < 1)
                            _habilidades.Add(habilidade);
                        break;
                    default:
                        break;
                }
            }
        }

        public void AdicionarHabilidadesDisponiveis(List<Habilidade> habilidadesAdicionadas)
        {
            foreach (var habilidade in habilidadesAdicionadas)
            {
                int contadorHabilidade = 0;

                contadorHabilidade = _habilidadesDisponiveis.FindAll(x => x.Categoria == habilidade.Categoria).Count();

                switch (habilidade.Categoria)
                {
                    case CategoriaHabilidade.Basica:
                        if (contadorHabilidade < 3)
                            _habilidadesDisponiveis.Add(habilidade);
                        break;
                    case CategoriaHabilidade.Avancada:
                        if (contadorHabilidade < 2)
                            _habilidadesDisponiveis.Add(habilidade);
                        break;
                    case CategoriaHabilidade.Especialista:
                        if (contadorHabilidade < 1)
                            _habilidadesDisponiveis.Add(habilidade);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool SubstituirHabilidade(Habilidade habilidadeSubstituida, Habilidade novaHabilidade)
        {
            if (habilidadeSubstituida.Categoria != novaHabilidade.Categoria)
                return false;

            int indexHabilidadeSubstituida = _habilidades.FindIndex(h => h == habilidadeSubstituida);
            _habilidades.Insert(indexHabilidadeSubstituida, novaHabilidade);
            return true;
        }

        internal virtual int CalcularPoderBase(Habilidade habilidade)
        {
            Random rnd = new Random();

            int poderHabilidadeFinal = habilidade.PoderBase;
            for (int i = 0; i < habilidade.Moeda; i++)
            {
                int aleatorio = rnd.Next(1, 100);
                if (aleatorio > 50)
                {
                    poderHabilidadeFinal += habilidade.PoderAdicionalMoeda;
                }
            }
            return poderHabilidadeFinal;
        }

        internal void RemoverMoeda(Habilidade habilidade)
        {
            _habilidadesDisponiveis.Find(h => h.Id == habilidade.Id).RemoverMoeda();
        }

        internal virtual void ReceberDano(int Dano)
        {
            if (Dano <= 0)
                Dano = 1;
            VidaAtual -= Dano;
        }

        internal virtual void Defender()
        {
            VidaAtual += Math.Max(1, (int)(_vidaTotal * 0.05));
            _estaDefendendo = true;
        }

        public abstract void AlterarModificador(int Modificador);

    }
}