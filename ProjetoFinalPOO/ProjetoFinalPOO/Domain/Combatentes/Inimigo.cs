using ProjetoFinalPOO.Enums;
using System;

namespace ProjetoFinalPOO.Combatentes
{
    public class Inimigo : Combatente
    {
        public Inimigo(string nome, int vidaTotal = 100, int defesa = 10, int agilidade = 10, AfinidadeDefesa afinidade = AfinidadeDefesa.Armadurado) : base()
        {
            _nome = nome;
            _vidaTotal = vidaTotal;
            _vidaAtual = _vidaTotal;
            _defesa = defesa;
            _agilidade = agilidade;
            _afinidade = afinidade;
        }

        public override void AlterarModificador(int Modificador)
        {
            // Inimigos possuem estabilidade padrão e não acumulam recursos especiais da tripulação
        }
    }
}
