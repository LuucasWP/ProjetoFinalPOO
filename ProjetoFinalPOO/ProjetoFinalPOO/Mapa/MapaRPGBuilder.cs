using System.Collections.Generic;

namespace ProjetoFinalPOO.Mapa
{
    public class MapaRPGBuilder
    {
        private Grafo _grafo;
        private List<string> _ultimoPlaneta = new List<string>();
        
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
            List<string> _PlanetaAtual = new List<string>();
            for (var i = 0; i < 3; i++)
            {
                _grafo.AdicionarVertice($"Galáxia {numeroGalaxia}, Planeta {i + 1}");
                _PlanetaAtual.Add($"Galáxia {numeroGalaxia}, Planeta {i + 1}");
            }

            foreach (var _planeta in _PlanetaAtual)
            {
                foreach (var _ultimo in _ultimoPlaneta)
                {
                    _grafo.AdicionarAresta(_ultimo, _planeta, 1);
                }
            }
            _ultimoPlaneta = _PlanetaAtual;
            return this;
        }
        
        public Grafo Construir()
        {
            return _grafo;
        }
    }
}