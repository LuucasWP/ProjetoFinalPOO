using System;

namespace ProjetoFinalPOO.Música
{
    public class BibliotecaDeMusicas
    {
        public static Musica TemaConquisataItem()
        {
            Musica musica = new Musica("Tema Conquista Item");
            musica.Notas.Add(new Nota(523, 120));  
            musica.Notas.Add(new Nota(659, 120));  
            musica.Notas.Add(new Nota(784, 120));  
            musica.Notas.Add(new Nota(1047, 250)); 
            return musica;
        }
        
        public static Musica TemaEspacialOriginal()
        {
            Musica musica = new Musica("Tema Espacial Original");
            musica.Notas.Add(new Nota(523, 200)); 
            musica.Notas.Add(new Nota(659, 200)); 
            musica.Notas.Add(new Nota(784, 200)); 
            musica.Notas.Add(new Nota(1047, 400));
            musica.Notas.Add(new Nota(784, 200)); 
            musica.Notas.Add(new Nota(659, 200)); 
            musica.Notas.Add(new Nota(523, 400)); 
            musica.Notas.Add(new Nota(587, 200)); 
            musica.Notas.Add(new Nota(698, 200)); 
            musica.Notas.Add(new Nota(880, 400)); 
            musica.Notas.Add(new Nota(698, 200)); 
            musica.Notas.Add(new Nota(587, 200)); 
            musica.Notas.Add(new Nota(523, 600)); 
            return musica;
        }
        
        public static Musica MusicaInicio()
        {
            Musica musicaInicio = new Musica("Musica de Inicio");
            musicaInicio.Notas.Add(new Nota(392, 250)); 
            musicaInicio.Notas.Add(new Nota(392, 250)); 
            musicaInicio.Notas.Add(new Nota(392, 250)); 
            musicaInicio.Notas.Add(new Nota(392, 250)); 
            musicaInicio.Notas.Add(new Nota(311, 500)); 
            musicaInicio.Notas.Add(new Nota(392, 200));
            musicaInicio.Notas.Add(new Nota(392, 200));
            musicaInicio.Notas.Add(new Nota(466, 200));
            musicaInicio.Notas.Add(new Nota(466, 200));
            musicaInicio.Notas.Add(new Nota(392, 200));
            musicaInicio.Notas.Add(new Nota(262, 500));
            return musicaInicio;

        }
    }
}