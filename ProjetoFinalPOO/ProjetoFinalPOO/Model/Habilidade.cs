using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO
{
    public class Habilidade
    {
        private static int _contadorID = 0;
        public int Id { get; private set; }
        private string _nome { get; set; }
        private string _descricao { get; set; }
        private CategoriaHabilidade _categoria { get; set; }
        private int _modificador { get; set; }
        private AfinidadeAtaque _afinidade { get; set; }
        private int _poderBase { get; set; }
        private int _moeda { get; set; }
        private int _poderAdiconalMoeda { get; set; }

        public string Nome => _nome;
        public string Descricao => _descricao;
        public CategoriaHabilidade Categoria => _categoria;
        public int Modificador => _modificador;
        public AfinidadeAtaque Afinidade => _afinidade;
        public int PoderBase => _poderBase;
        public int Moeda => _moeda;
        public int PoderAdicionalMoeda => _poderAdiconalMoeda;

        public Habilidade(string nome, string descricao, CategoriaHabilidade categoria, int modificador, AfinidadeAtaque afinidade, int poderBase, int moeda, int poderAdiconalMoeda)
        {
            _contadorID++;
            Id = _contadorID;
            _nome = nome;
            _descricao = descricao;
            _categoria = categoria;
            _modificador = modificador;
            _afinidade = afinidade;
            _poderBase = poderBase;
            _moeda = moeda;
            _poderAdiconalMoeda = poderAdiconalMoeda;
        }

        internal void RemoverMoeda()
        {
            _moeda--;
        }
    }
}