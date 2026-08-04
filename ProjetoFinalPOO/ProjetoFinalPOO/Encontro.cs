using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO
{
    public class Encontro
    {
        private int Id { get; set; }
        private string _nome { get; set; }
        private string _descricao { get; set; }
        private TipoEncontro _tipoEncontro { get; set; }
        private int _quantidadeLimite { get; set; }
    }
}