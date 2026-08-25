using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Interface para objetos que aplicam efeitos sobre combatentes (habilidades, cartas, itens).
    /// Cumpre o requisito OO-06 ao ser implementada por hierarquias distintas de entidades.
    /// </summary>
    public interface IAplicavelEfeito
    {
        string Nome { get; }
        string Descricao { get; }
        string AplicarEfeito(Combatente usuario, Combatente alvo, List<string> logs = null);
    }
}
