using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO
{
    public class Combatente
    {
        private int ID { get; set; }
        private string _nome { get; set; }
        private int _Level { get; set; }
        private int _exp { get; set; }
        private int _vidaTotal { get; set; }
        private int _vidaAtual { get; set; }
        private int _ataque { get; set; }
        private int _defesa { get; set; }
        private int _agilidade { get; set; }
        private Afinidade _afinidade { get; set; }
    }
}