namespace ProjetoFinalPOO.Mapa
{
    public class Aresta
    {
        public int Id { get; set; }
        public Vertice Destino { get; set; }
        public int Peso { get; set; }
        
        public Aresta(Vertice destino, int peso)
        {
            this.Id = 0;
            this.Destino = destino;
            this.Peso = peso;
        }
    }
}