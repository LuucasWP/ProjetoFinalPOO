using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetoFinalPOO.Mapa;
using ProjetoFinalPOO.Música;

namespace ProjetoFinalPOO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Grafo mapa = new MapaRPGBuilder()
                .GerarInicio()
                .AdicionarGalaxia(1)
                .AdicionarGalaxia(2)
                .AdicionarGalaxia(3)
                .AdicionarGalaxia(4)
                .AdicionarGalaxia(5)
                .AdicionarGalaxia(6)
                .Construir();

            foreach (var vertice in mapa.Vertices)
            {
                string tipoEncontro = vertice.Encontro == null
                    ? "Sem encontro"
                    : vertice.Encontro.Comportamento.GetType().Name;

                Console.WriteLine($"Vértice: {vertice.Nome} [{tipoEncontro}]");

                foreach (var aresta in vertice.Arestas)
                {
                    Console.WriteLine($"  -> {aresta.Destino.Nome} (peso {aresta.Peso})");
                }
            }

            Console.ReadLine();
        }
    }
}
