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
            return new List<Habilidade>
            {
                // 3 Básicas
                new Habilidade("Pancada Brutal", "Golpe pesado com manopla aquecida que causa impacto ígneo.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Fogo, 10, 1, 4),
                new Habilidade("Postura Inabalável", "Ergue o escudo de liga pesada preparando-se para o embate.", CategoriaHabilidade.Basica, 3, AfinidadeAtaque.Eletrico, 8, 2, 3),
                new Habilidade("Lança-Granada Ácida", "Dispara cápsula corrosiva altamente eficaz contra armaduras pesadas.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Acido, 11, 2, 4),

                // 2 Avançadas
                new Habilidade("Impacto Demolidor", "Avanço devastador com propulsores térmicos que rompe as defesas.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Fogo, 16, 2, 5),
                new Habilidade("Barreira de Titânio", "Projeta um campo de contenção que reduz dano e reforça a blindagem.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Acido, 14, 2, 4),

                // 1 Especialista
                new Habilidade("Fúria do Colosso", "Ataque sísmico avassalador que esmaga o alvo com fluido corrosivo concentrado.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Acido, 22, 3, 6)
            };
        }

        public static List<Habilidade> ObterHabilidadesEngenheiro()
        {
            return new List<Habilidade>
            {
                // 3 Básicas
                new Habilidade("Disparo Eletrostático", "Feixe elétrico concentrado que causa curto-circuito em robôs.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Eletrico, 9, 2, 4),
                new Habilidade("Nanorobôs de Reparo", "Enxame de nanorobôs que recupera integridade e calibra sistemas.", CategoriaHabilidade.Basica, 4, AfinidadeAtaque.Eletrico, 10, 1, 5),
                new Habilidade("Solda de Plasma", "Tocha térmica de plasma que queima componentes biológicos.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Fogo, 10, 1, 5),

                // 2 Avançadas
                new Habilidade("Sobrecarga de Circuito", "Pulso eletromagnético em cadeia que sobrecarrega sistemas inimigos.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Eletrico, 15, 3, 4),
                new Habilidade("Campo de Força Defensivo", "Escudo de partículas eletrostáticas que anula projéteis.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Eletrico, 12, 2, 5),

                // 1 Especialista
                new Habilidade("Canhão Orbital de Partículas", "Sinaliza um bombardeio de feixe de partículas em alta voltagem.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Eletrico, 24, 3, 7)
            };
        }

        public static List<Habilidade> ObterHabilidadesBiomancer()
        {
            return new List<Habilidade>
            {
                // 3 Básicas
                new Habilidade("Dardo Celular Cáustico", "Projétil de enzimas biológicas condensadas e altamente corrosivas.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Acido, 9, 2, 4),
                new Habilidade("Pulso Bio-Psíquico", "Canaliza bio-frequências para estabilizar o foco e a mana.", CategoriaHabilidade.Basica, 4, AfinidadeAtaque.Eletrico, 8, 2, 3),
                new Habilidade("Combustão Orgânica", "Reação exotérmica celular de alta voltagem que calcina alvos.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Fogo, 11, 2, 4),

                // 2 Avançadas
                new Habilidade("Nuvem Bio-Corrosiva", "Bruma tóxica de esporos cáusticos que derrete blindagens e tecidos.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Acido, 16, 3, 5),
                new Habilidade("Distorção Bio-Cinética", "Membrana bio-elástica que dissipa o impacto de ataques no embate.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Fogo, 14, 2, 5),

                // 1 Especialista
                new Habilidade("Vórtice Celular Absoluto", "Abre uma hiper-reação de biomassa devoradora que calcina em chamas.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Fogo, 25, 3, 7)
            };
        }

        public static List<Habilidade> ObterHabilidadesInimigo(string tipoInimigo)
        {
            string chave = tipoInimigo.ToLowerInvariant();

            if (chave.Contains("chefe") || chave.Contains("titã") || chave.Contains("capitania"))
            {
                return new List<Habilidade>
                {
                    new Habilidade("Canhão de Fusão Titânica", "Disparo pesado da bateria principal do Dreadnought.", CategoriaHabilidade.Especialista, 0, AfinidadeAtaque.Fogo, 20, 3, 5),
                    new Habilidade("Pulso Eletromagnético", "Onda de choque que sobrecarrega escudos.", CategoriaHabilidade.Avancada, 0, AfinidadeAtaque.Eletrico, 17, 2, 5),
                    new Habilidade("Salva de Torpedos de Ácido", "Torpedos com ogivas de biocorrosão.", CategoriaHabilidade.Avancada, 0, AfinidadeAtaque.Acido, 18, 2, 5)
                };
            }

            if (chave.Contains("drone") || chave.Contains("mecanico"))
            {
                return new List<Habilidade>
                {
                    new Habilidade("Metralhadora Eletrostática", "Rajada de disparos de alta voltagem.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 9, 2, 3),
                    new Habilidade("Laser de Corte Térmico", "Feixe concentrado para corte e penetração.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Fogo, 10, 1, 4),
                    new Habilidade("Descarga Elétrica de Choque", "Descarga de alta amperagem.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 8, 1, 3)
                };
            }

            return new List<Habilidade>
            {
                new Habilidade("Ataque Ácido", "Disparo de fluido corrosivo.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Acido, 10, 2, 3),
                new Habilidade("Lâmina Térmica", "Golpe cortante com espada superaquecida.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Fogo, 11, 2, 3),
                new Habilidade("Ataque Elétrico", "Pulso de choque elétrico direto.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 9, 1, 4)
            };
        }

        public static Combatente CriarSentinela()
        {
            var sentinela = Sentinela.Instancia();
            if (sentinela.Habilidades.Count == 0)
            {
                sentinela.AdcionarHabilidade(ObterHabilidadesSentinela());
            }
            return sentinela;
        }

        public static Combatente CriarEngenheiro()
        {
            var engenheiro = Engenheiro.Instancia();
            if (engenheiro.Habilidades.Count == 0)
            {
                engenheiro.AdcionarHabilidade(ObterHabilidadesEngenheiro());
            }
            return engenheiro;
        }

        public static Combatente CriarBiomancer()
        {
            var biomancer = Biomancer.Instancia();
            if (biomancer.Habilidades.Count == 0)
            {
                biomancer.AdcionarHabilidade(ObterHabilidadesBiomancer());
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
