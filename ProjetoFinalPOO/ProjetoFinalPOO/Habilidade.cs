using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO
{
    public class Habilidade
    {
        private int Id { get; set; }
        private string _nome { get; set; }
        private string _descricao { get; set; }
        private CategoriaHabilidade _categoria { get; set; }
        private int _custo { get; set; }
        private Afinidade _afinidade { get; set; }
        private int _danoBase { get; set; }
        private int _moeda { get; set; }
        private int _danoAdiconalMoeda { get; set; }
    }
}