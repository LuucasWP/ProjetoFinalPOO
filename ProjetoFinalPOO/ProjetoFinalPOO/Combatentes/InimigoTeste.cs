using ProjetoFinalPOO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinalPOO.Combatentes
{
    public class InimigoTeste: Combatente
    {

        public InimigoTeste(string nome)
        {
            _nome = nome;
            _vidaTotal = 100;
            _vidaAtual = _vidaTotal;
            _defesa = 10;
            _agilidade = 50;
            _afinidade = AfinidadeDefesa.Armadurado;
        }

        public override void AlterarModificador(int Modificador)
        {
            throw new NotImplementedException();
        }
    }
}
