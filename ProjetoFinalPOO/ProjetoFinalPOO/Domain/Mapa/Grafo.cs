using System.Collections.Generic;

namespace ProjetoFinalPOO.Mapa
{
    public class Grafo
    {
        public List<Vertice> Vertices { get; set; } = new List<Vertice>();
        
        public Vertice AdicionarVertice(string nome)
        {
            Vertice novoVertice = new Vertice(nome);
            Vertices.Add(novoVertice);
            return novoVertice;
        }
        public void AdicionarAresta(string origemNome, string destinoNome, int peso)
        {
            Vertice origem = Vertices.Find(v => v.Nome == origemNome);
            Vertice destino = Vertices.Find(v => v.Nome == destinoNome);

            if(origem != null && destino != null)
            {
                origem.Arestas.Add(new Aresta(destino, peso));
            }
        }
    }
}