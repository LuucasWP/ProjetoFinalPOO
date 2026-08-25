namespace ProjetoFinalPOO.Música
{
    public class Nota
    {
        public int Frequencia { get; set; }
        public int DuracaoMs { get; set; }

        public Nota(int frequencia, int duracaoMs)
        {
            Frequencia = frequencia;
            DuracaoMs = duracaoMs;
        }
    }
}