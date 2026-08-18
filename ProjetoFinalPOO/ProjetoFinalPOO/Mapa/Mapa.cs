using System.Collections.Generic;

namespace ProjetoFinalPOO.Mapa
{
    public class Mapa
    {
        public string Nome { get; set; }
        public List<Encontro> _encontros { get; set; }
        public Mapa(string nome)
        {
            this.Nome = nome;
            this._encontros = new List<Encontro>();
        }
        
        public Grafo novoMapa = new MapaRPGBuilder()
            .GerarInicio()
            .AdicionarGalaxia(1)
            .AdicionarGalaxia(2)
            .AdicionarGalaxia(3)
            .AdicionarGalaxia(4)
            .AdicionarGalaxia(5)
            .AdicionarGalaxia(6)
            .Construir();
    }
}