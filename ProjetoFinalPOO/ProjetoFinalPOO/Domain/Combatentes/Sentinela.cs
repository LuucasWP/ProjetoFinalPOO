using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Combatentes
{
    public class Sentinela : Combatente
    {
        private static Sentinela _instancia;
        private int _adrenalina {get; set;}
        
        public int Adrenalina
        {
            get => _adrenalina;
            private set
            {
                if (value < 0)
                    _adrenalina = 0;
                else if (value > 45)
                    _adrenalina = 45;
                else
                    _adrenalina = value;
            }
        }

        private Sentinela() : base()
        {
            _nome = "Optimus";
            Adrenalina = 0;
            _vidaTotal = 100;
            _vidaAtual = _vidaTotal;
            _defesa = 10;
            _agilidade = 8;
            _afinidade = AfinidadeDefesa.Armadurado;
        }

        public static Sentinela Instancia()
        {
            if (_instancia == null)
                _instancia = new Sentinela();
            return _instancia;
        }

        internal override int CalcularPoderBase(Habilidade habilidade)
        {
            Random rnd = new Random();

            int poderHabilidadeFinal = habilidade.PoderBase;
            for (int i = 0; i < habilidade.Moeda; i++)
            {
                int aleatorio = rnd.Next(_adrenalina, 100);
                if (aleatorio > 50)
                {
                    poderHabilidadeFinal += habilidade.PoderAdicionalMoeda;
                }
            }

            return poderHabilidadeFinal;
        }

        public override void AlterarModificador(int Modificador)
        {
            Adrenalina += Modificador;
        }

        internal override void ReceberDano(int Dano)
        {
            base.ReceberDano(Dano);
            AlterarModificador(Dano / 2);
        }

        internal override void Defender()
        {
            base.Defender();
            Adrenalina += Math.Max(2, (int)(Adrenalina * 0.10) + 2);
        }
    }
}
