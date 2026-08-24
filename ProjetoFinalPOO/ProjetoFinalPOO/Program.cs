using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //NÃO FAZER COMMIT DO CODIGO ABAIXO NA MAIN
            //O CODIGO ABAIXO É SOMENTE PARA TESTES DURANTE O DESVENVOLVIMENTO DO PROJETO

            Sentinela p1 = Sentinela.Instancia();
            Engenheiro p2 = Engenheiro.Instancia();
            Biomancer p3 = Biomancer.Instancia();


            InimigoTeste i1 = new InimigoTeste("Inimigo 1");
            InimigoTeste i2 = new InimigoTeste("Inimigo 2");
            InimigoTeste i3 = new InimigoTeste("Inimigo 3");


            List<Habilidade> habilidades = new List<Habilidade>()
            {
                new Habilidade("Habilidade basica 1", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade basica 2", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade basica 3", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade Avancada 1", "desc", Enums.CategoriaHabilidade.Avancada, 10,
                    Enums.AfinidadeAtaque.Eletrico, 10, 2, 5),
                new Habilidade("Habilidade Avancada 2", "desc", Enums.CategoriaHabilidade.Avancada, 10,
                    Enums.AfinidadeAtaque.Eletrico, 10, 2, 5),
                new Habilidade("Habilidade Especialista 1", "desc", Enums.CategoriaHabilidade.Especialista, 10,
                    Enums.AfinidadeAtaque.Acido, 13, 2, 5)
            };

            List<Habilidade> h1 = new List<Habilidade>()
            {
                new Habilidade("Habilidade Inimigo 1", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade Inimigo 2", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade Inimigo 3", "desc", Enums.CategoriaHabilidade.Basica, 10,
                    Enums.AfinidadeAtaque.Fogo, 8, 3, 3),
                new Habilidade("Habilidade Inimigo 1", "desc", Enums.CategoriaHabilidade.Avancada, 10,
                    Enums.AfinidadeAtaque.Eletrico, 10, 2, 5),
                new Habilidade("Habilidade Inimigo 2", "desc", Enums.CategoriaHabilidade.Avancada, 10,
                    Enums.AfinidadeAtaque.Eletrico, 10, 2, 5),
                new Habilidade("Habilidade Inimigo 1", "desc", Enums.CategoriaHabilidade.Especialista, 10,
                    Enums.AfinidadeAtaque.Acido, 13, 2, 5)
            };

            p1.AdcionarHabilidade(habilidades);
            p2.AdcionarHabilidade(habilidades);
            p3.AdcionarHabilidade(habilidades);

            i1.AdcionarHabilidade(h1);
            i2.AdcionarHabilidade(h1);
            i3.AdcionarHabilidade(h1);

            List<Combatente> inimigos = new List<Combatente>
            {
                i1,
                i2,
                i3
            };

            /*
            combate.IniciarCombate(inimigos);
            combate.IniciarRodada();

            foreach (var item in combate.IntencaoAtaqueInimigos)
            {
                Console.WriteLine($"id: {item.Key} | Habilidade: {item.Value.Nome}");
            }
            */

            // Simulução de um combate | Exemplo de como usar os metodos do controlador de combate
            ControladorCombate combate = ControladorCombate.Instancia();
            combate.IniciarCombate(inimigos);
            do
            {
                combate.IniciarRodada();
                while (combate.VerificarFimDeRodada())
                {
                    var (combatenteAtual, eAliado) = combate.AcaoProximoCombatente();

                    if (eAliado)
                    {
                        OpcaoMenuCombate opcaoMenu = SelecionarAcao();
                        int idAlvo = SelecionarAlvo();
                        Combatente alvoAcao = combate.CombatentesInimigos.Find(c => c.Id == idAlvo);

                        switch (opcaoMenu)
                        {
                            case OpcaoMenuCombate.Atacar:
                                int idHabilidadeSelecionada = SelecionarHabilidade(combatenteAtual);
                                if (combate.VerificarAlvoNaoAtacouRodada(alvoAcao))
                                {

                                    Combatente VencedorEmbate = combate.RealizarEmbate(alvoAcao, combatenteAtual,
                                        combatenteAtual.HabilidadesDisponiveis.Find(h => h.Id == idHabilidadeSelecionada));
                                    //Console.WriteLine($"Vencedor do embate: {VencedorEmbate.Nome}");
                                    if (combate.CombatentesAliados.Contains(VencedorEmbate))
                                    {
                                        combate.Atacar(alvoAcao, combatenteAtual, combatenteAtual.HabilidadesDisponiveis.Find(h => h.Id == idHabilidadeSelecionada));
                                    }
                                    else
                                    {
                                        combate.Atacar(combatenteAtual, VencedorEmbate, combate.IntencaoAtaqueInimigos[alvoAcao]);
                                    }
                                    //Console.WriteLine($"{VencedorEmbate.Nome} causou {danoCausado}");
                                }

                                break;
                            case OpcaoMenuCombate.UsarItem:

                                break;
                            case OpcaoMenuCombate.Defender:
                                combatenteAtual.Defender();
                                break;
                        }

                    }
                    else
                    {
                        combate.InimigoAtacaSemOposicao(combatenteAtual);
                    }
                }
            } while (combate.VerificarFimDeCombate());


            // Fim da simulação de combate
        }



        //Metodos criados somente para auxiliar na simulação de combate
        //-------------------------------------------------------------------------------------------------------
        public static OpcaoMenuCombate SelecionarAcao()
        {
            Console.WriteLine("Selecionar ação:");
            Console.WriteLine("1) Atacar");
            Console.WriteLine("2) Usar Item");
            Console.WriteLine("3) Defender");
            OpcaoMenuCombate opcao;
            while (!Enum.TryParse(Console.ReadLine(), out opcao) || !Enum.IsDefined(typeof(OpcaoMenuCombate), opcao))
            {
                Console.WriteLine("Entrada Invalida");
            }

            return opcao;
        }

        public static int SelecionarAlvo()
        {
            ListarAliados();
            ListarInimigos();

            Console.WriteLine("Informe o id do Alvo:");
            int idAlvo;
            while (!int.TryParse(Console.ReadLine(), out idAlvo))
            {
                Console.WriteLine("Entrada invalida, informe o id do alvo novamente");
            }

            return idAlvo;
        }

        public static int SelecionarHabilidade(Combatente combatente)
        {
            ListarHabilidades(combatente);

            Console.WriteLine("Informe o id da Habilidade:");
            int idHabilidade;
            while (!int.TryParse(Console.ReadLine(), out idHabilidade))
            {
                Console.WriteLine("Entrada invalida, informe o id da Habilidade novamente");
            }

            return idHabilidade;
        }

        public static void ListarAliados()
        {
            Console.WriteLine("-------------------------------------------------Aliados-------------------------------------------------");
            foreach (Combatente combatentesAliado in ControladorCombate.Instancia().CombatentesAliados)
            {
                Console.WriteLine($"ID:{combatentesAliado.Id} | {combatentesAliado.Nome}");
            }

            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
        }

        public static void ListarInimigos()
        {
            Console.WriteLine("-------------------------------------------------Inimigos-------------------------------------------------");
            foreach (Combatente combatentesInimigo in ControladorCombate.Instancia().CombatentesInimigos)
            {
                Console.WriteLine($"ID:{combatentesInimigo.Id} | {combatentesInimigo.Nome}");
            }

            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
        }

        public static void ListarHabilidades(Combatente combatente)
        {
            Console.WriteLine("-------------------------------------------------Habilidade-------------------------------------------------");
            foreach (Habilidade habilidade in combatente.HabilidadesDisponiveis)
            {
                Console.WriteLine($"ID:{habilidade.Id} | Nome: {habilidade.Nome} | Poder Base: {habilidade.PoderBase} | Moedas: {habilidade.Moeda} | Adicional Moeda: {habilidade.PoderAdicionalMoeda}");
            }

            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
        }



        //-------------------------------------------------------------------------------------------------------
    }
}

