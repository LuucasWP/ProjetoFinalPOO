namespace ProjetoFinalPOO.EncontroPlaneta
{
    public class Encontro
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public int QuantidadeLimite { get; set; }
        public IEncontro _comportamentos { get; set; }  
    }
}