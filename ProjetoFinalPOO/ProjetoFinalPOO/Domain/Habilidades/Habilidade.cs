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
        private int _moedaMaxima { get; set; }
        private int _poderAdicionalMoeda { get; set; }

        public string Nome => _nome;
        public string Descricao => _descricao;
        public CategoriaHabilidade Categoria => _categoria;
        public int Modificador => _modificador;
        public AfinidadeAtaque Afinidade => _afinidade;
        public int PoderBase => _poderBase;
        public int Moeda => _moeda;
        public int MoedaMaxima => _moedaMaxima;
        public int PoderAdicionalMoeda => _poderAdicionalMoeda;

        public Habilidade(string nome, string descricao, CategoriaHabilidade categoria, int modificador, AfinidadeAtaque afinidade, int poderBase, int moeda, int poderAdicionalMoeda)
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
            _moedaMaxima = moeda;
            _poderAdicionalMoeda = poderAdicionalMoeda;
        }

        internal void RemoverMoeda()
        {
            if (_moeda > 0)
                _moeda--;
        }

        internal void ResetarMoeda()
        {
            _moeda = _moedaMaxima;
        }

        public Habilidade Clonar()
        {
            var clone = new Habilidade(_nome, _descricao, _categoria, _modificador, _afinidade, _poderBase, _moedaMaxima, _poderAdicionalMoeda);
            clone.Id = this.Id;
            return clone;
        }
    }
}