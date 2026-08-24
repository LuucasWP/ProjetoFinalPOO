using System;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Representa uma posição tática no grid de batalha (Slots 1, 2 e 3).
    /// Armazena o combatente posicionado, sua intenção planejada (preview de ação - README 21),
    /// estado de defesa (README 19) e controle de ação por rodada (README 20).
    /// </summary>
    public class Slot
    {
        private int _numeroSlot;
        private Combatente _combatente;
        private Habilidade _habilidadePlanejada;
        private int _alvoPlanejadoSlot;
        private bool _defendendo;
        private bool _jaAtacouNestaRodada;

        public int NumeroSlot { get => _numeroSlot; set => _numeroSlot = value; }
        public Combatente Combatente { get => _combatente; set => _combatente = value; }
        public Habilidade HabilidadePlanejada { get => _habilidadePlanejada; set => _habilidadePlanejada = value; }
        public int AlvoPlanejadoSlot { get => _alvoPlanejadoSlot; set => _alvoPlanejadoSlot = value; }
        public bool Defendendo { get => _defendendo; set => _defendendo = value; }
        public bool JaAtacouNestaRodada { get => _jaAtacouNestaRodada; set => _jaAtacouNestaRodada = value; }

        public Slot(int numeroSlot, Combatente combatente = null)
        {
            NumeroSlot = numeroSlot;
            Combatente = combatente;
            HabilidadePlanejada = null;
            AlvoPlanejadoSlot = 0;
            Defendendo = false;
            JaAtacouNestaRodada = false;
        }

        public void ResetarTurno()
        {
            Defendendo = false;
            JaAtacouNestaRodada = false;
            HabilidadePlanejada = null;
        }

        public override string ToString()
        {
            string nome = Combatente != null ? Combatente.Nome : "[VAZIO]";
            string status = Defendendo ? " [DEFENDENDO]" : "";
            return $"Slot {NumeroSlot}: {nome}{status}";
        }
    }
}
