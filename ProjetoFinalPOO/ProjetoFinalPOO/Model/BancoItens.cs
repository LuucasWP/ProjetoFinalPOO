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
            return new List<Item>
            {
                new Item(1, "Medikit Nanomédico", Raridade.Comum, TipoItem.CuraVida, 35, "Restaura instantaneamente +35 de Vida em um aliado."),
                new Item(2, "Injeção de Éter Estabilizadora", Raridade.Comum, TipoItem.RestauraSanidade, 25, "Estabiliza e restaura +25 de Adrenalina, Superaquecimento ou Mana."),
                new Item(3, "Bateria de Plasma Concentrada", Raridade.Comum, TipoItem.RecuperaEnergia, 30, "Recarrega +30 pontos de Energia tática."),
                new Item(4, "Emissor de Escudo Cinético", Raridade.Incomum, TipoItem.EscudoEmergencial, 40, "Gera uma barreira protetora temporária de +40 de Escudo."),
                new Item(5, "Granada de Fogo Termobárica", Raridade.Incomum, TipoItem.GranadaFogo, 30, "Explosão térmica que causa 30 de dano de FOGO (eficaz contra Biológicos)."),
                new Item(6, "Granada de Pulso Eletrostático", Raridade.Incomum, TipoItem.GranadaEletrica, 30, "Sobrecarga de alta voltagem que causa 30 de dano ELÉTRICO (eficaz contra Mecânicos)."),
                new Item(7, "Frasco de Ácido Quântico", Raridade.Incomum, TipoItem.GranadaAcido, 30, "Substância corrosiva que causa 30 de dano de ÁCIDO (eficaz contra Armaduras)."),
                new Item(8, "Célula Regenerativa Militar", Raridade.Raro, TipoItem.CuraVida, 70, "Nanomáquinas de alta potência que curam +70 de Vida."),
                new Item(9, "Catalisador de Vácuo Arcano", Raridade.Epico, TipoItem.RestauraSanidade, 50, "Artefato que restaura +50 de Adrenalina, Superaquecimento ou Mana em combate."),
                new Item(10, "Bateria de Antimatéria Experimental", Raridade.Lendario, TipoItem.RecuperaEnergia, 75, "Restaura +75 de Energia para habilidades de alto calibre.")
            };
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
            return new List<Item>
            {
                new Item(1, "Medikit Nanomédico", Raridade.Comum, TipoItem.CuraVida, 35, "Restaura +35 de Vida em um aliado."),
                new Item(2, "Injeção de Éter Estabilizadora", Raridade.Comum, TipoItem.RestauraSanidade, 25, "Recupera +25 de Recursos Especiais."),
                new Item(3, "Bateria de Plasma Concentrada", Raridade.Comum, TipoItem.RecuperaEnergia, 30, "Recarrega +30 de Energia.")
            };
        }
    }
}
