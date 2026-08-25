using System;
using System.Collections.Generic;
using ProjetoFinalPOO.EncontroPlaneta;

namespace ProjetoFinalPOO.Mapa
{
    public class MapaRPGBuilder
    {
        private Grafo _grafo;
        private List<string> _ultimoPlaneta = new List<string>();
        private Random _random = new Random();
        
        public MapaRPGBuilder()
        {
            _grafo = new Grafo();
        }

        public MapaRPGBuilder GerarInicio()
        {
            _grafo.AdicionarVertice("Inicio");
            _ultimoPlaneta.Add("Inicio");
            return this;
        }

        public MapaRPGBuilder AdicionarGalaxia(int numeroGalaxia)
        {
            List<string> planetaAtual = new List<string>();
            int indiceBaseEspacial = _random.Next(0, 3);
            for (var i = 0; i < 3; i++)
            {
                Vertice verticeAtual = _grafo.AdicionarVertice($"Galáxia {numeroGalaxia}, Planeta {i + 1}");
                planetaAtual.Add($"Galáxia {numeroGalaxia}, Planeta {i + 1}");
                if (i == indiceBaseEspacial)
                {
                    verticeAtual.Encontro = new Encontro { Comportamento = new EncontroBaseEspacial() };
                }
                else
                {
                    verticeAtual.Encontro = new Encontro { Comportamento = new EncontroBatalha() };
                }
            }

            foreach (var planeta in planetaAtual)
            {
                foreach (var ultimo in _ultimoPlaneta)
                {
                    _grafo.AdicionarAresta(ultimo, planeta, 1);
                }
            }
            _ultimoPlaneta = planetaAtual;
            return this;
        }
        
        public Grafo Construir()
        {
            return _grafo;
        }
    }
}