using System.Collections.Generic;
using ProjetoFinalPOO.EncontroPlaneta;
using ProjetoFinalPOO.Mapa;

namespace ProjetoFinalPOO
{
    public class Mapa_Jogo
    {
        private int Id { get; set; }
        private List<Encontro> _encontros { get; set; }
        private List<Vertice> _vertices { get; set; }

    }
}