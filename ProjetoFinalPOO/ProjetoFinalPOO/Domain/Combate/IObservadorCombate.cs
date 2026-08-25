using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Contrato de Observer para que telas e interfaces recebam notificações de eventos e atualizações de estado do combate.
    /// </summary>
    public interface IObservadorCombate
    {
        void AtualizarEstado(Slot[] slotsJogador, Slot[] slotsInimigo, int rodadaAtual);
        void AtualizarIniciativa(List<Combatente> ordemIniciativa, Combatente combatenteAtivo);
        void ExibirEmbate(ResultadoEmbate resultado);
        void AtualizarLog(string mensagem);
    }
}
