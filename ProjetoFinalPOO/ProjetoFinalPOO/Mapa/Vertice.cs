using System.Collections.Generic;

namespace ProjetoFinalPOO.Mapa
{
    public class Vertice
    {
        public string Nome { get; set; }
        public List<Aresta> Arestas { get; set; } = new List<Aresta>();
        public Vertice(string nome) => this.Nome = nome;
    }
}