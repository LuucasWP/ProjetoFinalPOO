using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Combatentes
{
    public class Biomancer : Combatente
    {
        private static Biomancer _instancia;
        private int _mana { get; set; }
        public int Mana 
        {
            get => _mana;
            private set
            {
                if (value < 0)
                    _mana = 0;
                else if (value > 45)
                    _mana = 45;
                else
                    _mana = value;
            }
        }
        private Biomancer() : base()
        {
            _nome = "Pasteur";
            _mana = 45;
            _vidaTotal = 50;
            _vidaAtual = _vidaTotal;
            _defesa = 8;
            _agilidade = 15;
            _afinidade = AfinidadeDefesa.Biologico;
        }
        
        public static Biomancer Instancia()
        {
            if (_instancia == null)
                _instancia = new Biomancer();
            return _instancia;
        }

        internal override int CalcularPoderBase(Habilidade habilidade)
        {
            Random rnd = new Random();

            int poderHabilidadeFinal = habilidade.PoderBase;
            for (int i = 0; i < habilidade.Moeda; i++)
            {
                int aleatorio = rnd.Next(_mana, 100);
                if (aleatorio > 50)
                {
                    poderHabilidadeFinal += habilidade.PoderAdicionalMoeda;
                }
            }

            return poderHabilidadeFinal;
        }

        public override void AlterarModificador(int Modificador)
        {
            Mana -= Modificador;
        }

        internal override void Defender()
        {
            base.Defender();
            Mana += (int)(Mana * 0.30);
        }
    }
}
