using System;
using System.Collections.Generic;

namespace ProjetoFinalPOO.Música
{
    public class Musica
    {
        public string Nome { get; set; }
        public List<Nota> Notas { get; set; } = new List<Nota>();

        public Musica(string nome)
        {
            Nome = nome;
        }

        public void Tocar()
        {
            foreach (var nota in Notas)
            {
                Console.Beep(nota.Frequencia, nota.DuracaoMs);
            }
        }
        
    }
}