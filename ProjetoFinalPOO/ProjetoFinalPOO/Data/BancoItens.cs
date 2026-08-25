using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Fornece o catálogo de itens utilizáveis e gerador de espólios para exploração espacial (README 18 e 22).
    /// </summary>
    public static class BancoItens
    {
        public static List<Item> ObterTodosItens()
        {
            return CarregadorDadosJogo.ObterItens();
        }

        public static List<Item> GerarEscolhaTresItens(Random rng = null)
        {
            rng ??= new Random();
            var todos = ObterTodosItens();
            var escolhidos = new List<Item>();

            // Seleciona 3 itens aleatórios distintos
            var indices = new HashSet<int>();
            while (indices.Count < 3 && indices.Count < todos.Count)
            {
                indices.Add(rng.Next(todos.Count));
            }

            foreach (var idx in indices)
            {
                escolhidos.Add(todos[idx]);
            }

            return escolhidos;
        }

        public static List<Item> ObterInventarioInicial()
        {
            return CarregadorDadosJogo.ObterInventarioInicial();
        }
    }
}
