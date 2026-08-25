using System;

namespace ProjetoFinalPOO.Música
{
    public class BibliotecaDeMusicas
    {
        public static Musica FanfarraDeConquista()
        {
            Musica musica = new Musica("Fanfarra de Conquista");
            musica.Notas.Add(new Nota(523, 120));  
            musica.Notas.Add(new Nota(659, 120));  
            musica.Notas.Add(new Nota(784, 120));  
            musica.Notas.Add(new Nota(1047, 250)); 
            return musica;
        }
        
        public static Musica TemaEspacialOriginal()
        {
            Musica musica = new Musica("Tema Espacial Original");
            musica.Notas.Add(new Nota(523, 250)); 
            musica.Notas.Add(new Nota(659, 250)); 
            musica.Notas.Add(new Nota(784, 250)); 
            musica.Notas.Add(new Nota(1047, 400));
            musica.Notas.Add(new Nota(784, 250)); 
            musica.Notas.Add(new Nota(659, 250)); 
            musica.Notas.Add(new Nota(523, 400)); 
            musica.Notas.Add(new Nota(587, 250)); 
            musica.Notas.Add(new Nota(698, 250)); 
            musica.Notas.Add(new Nota(880, 400)); 
            musica.Notas.Add(new Nota(698, 250)); 
            musica.Notas.Add(new Nota(587, 250)); 
            musica.Notas.Add(new Nota(523, 600)); 
            return musica;
        }
        
        public static Musica MusicaInicio()
        {
            Musica musicaInicio = new Musica("Musica de Inicio");
            musicaInicio.Notas.Add(new Nota(394, 320)); 
            musicaInicio.Notas.Add(new Nota(394, 320)); 
            musicaInicio.Notas.Add(new Nota(394, 320)); 
            musicaInicio.Notas.Add(new Nota(394, 320)); 
            musicaInicio.Notas.Add(new Nota(309, 450)); 
            musicaInicio.Notas.Add(new Nota(394, 350));
            musicaInicio.Notas.Add(new Nota(394, 350));
            musicaInicio.Notas.Add(new Nota(472, 350));
            musicaInicio.Notas.Add(new Nota(472, 350));
            musicaInicio.Notas.Add(new Nota(394, 350));
            musicaInicio.Notas.Add(new Nota(526, 500));
            return musicaInicio;

        }
    }
}