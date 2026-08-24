using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Combatentes
{
    public class Engenheiro : Combatente
    {
        private static Engenheiro _instancia;
        private int _sobreaquecimento { get; set; }

        public int Sobreaquecimento
        {
            get => _sobreaquecimento;
            private set
            {
                if (value < 0)
                    _sobreaquecimento = 0;
                if (value > 45)
                    _sobreaquecimento = 45;
                _sobreaquecimento = value;
            }
        }

        private Engenheiro() : base()
        {
            _nome = "Asimov";
            Sobreaquecimento = 0;
            _vidaTotal = 70;
            _vidaAtual = _vidaTotal;
            _defesa = 13;
            _agilidade = 20;
            _afinidade = AfinidadeDefesa.Mecanico;
        }

        public static Engenheiro Instancia()
        {
            if (_instancia == null)
                _instancia = new Engenheiro();
            return _instancia;
        }

        internal override int CalcularPoderBase(Habilidade habilidade)
        {
            Random rnd = new Random();

            int poderHabilidadeFinal = habilidade.PoderBase;
            for (int i = 0; i < habilidade.Moeda; i++)
            {
                int aleatorio = rnd.Next(_sobreaquecimento, 100);
                if (aleatorio > 50)
                {
                    poderHabilidadeFinal += habilidade.PoderAdicionalMoeda;
                }
            }

            return poderHabilidadeFinal;
        }

        public override void AlterarModificador(int Modificador)
        {
            Sobreaquecimento += Modificador;
        }

        internal override void Defender()
        {
            base.Defender();
            Sobreaquecimento += (2 / 100) * Sobreaquecimento;
        }
    }
}