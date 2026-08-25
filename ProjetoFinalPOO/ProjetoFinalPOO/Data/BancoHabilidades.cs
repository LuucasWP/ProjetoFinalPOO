using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Catálogo e gerador balanceado de todas as habilidades de cada classe e inimigos.
    /// Utiliza o novo modelo de Habilidade (8 parâmetros) e instâncias de Combatentes.
    /// </summary>
    public static class BancoHabilidades
    {
        public static List<Habilidade> ObterHabilidadesSentinela()
        {
            return CarregadorDadosJogo.ObterHabilidadesClasse("Sentinela");
        }

        public static List<Habilidade> ObterHabilidadesEngenheiro()
        {
            return CarregadorDadosJogo.ObterHabilidadesClasse("Engenheiro");
        }

        public static List<Habilidade> ObterHabilidadesBiomancer()
        {
            return CarregadorDadosJogo.ObterHabilidadesClasse("Biomancer");
        }

        public static List<Habilidade> ObterHabilidadesInimigo(string tipoInimigo, int nivel = 1)
        {
            return CarregadorDadosJogo.ObterHabilidadesInimigo(tipoInimigo, nivel);
        }

        public static Combatente CriarSentinela()
        {
            var sentinela = Sentinela.Instancia();
            if (sentinela.Habilidades.Count == 0)
            {
                sentinela.AdicionarHabilidade(ObterHabilidadesSentinela());
            }
            return sentinela;
        }

        public static Combatente CriarEngenheiro()
        {
            var engenheiro = Engenheiro.Instancia();
            if (engenheiro.Habilidades.Count == 0)
            {
                engenheiro.AdicionarHabilidade(ObterHabilidadesEngenheiro());
            }
            return engenheiro;
        }

        public static Combatente CriarBiomancer()
        {
            var biomancer = Biomancer.Instancia();
            if (biomancer.Habilidades.Count == 0)
            {
                biomancer.AdicionarHabilidade(ObterHabilidadesBiomancer());
            }
            return biomancer;
        }

        public static List<Combatente> CriarTripulacaoPadrao()
        {
            return new List<Combatente>
            {
                CriarSentinela(),
                CriarEngenheiro(),
                CriarBiomancer()
            };
        }
    }
}
