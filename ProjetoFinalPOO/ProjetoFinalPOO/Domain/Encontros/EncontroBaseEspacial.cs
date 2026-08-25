using System;

namespace ProjetoFinalPOO.EncontroPlaneta
{
    public class EncontroBaseEspacial : IEncontro
    {
        public void Executar()
        {
            Console.WriteLine("Você chegou a base espacial!");
            while (true)
            {
                Console.WriteLine("1 - Curar.");
                Console.WriteLine("2 - Mudar Habilidade.");
                Console.WriteLine("3 - Alterar Item.");
                Console.WriteLine("0 - Sair do planeta");
                int menu = Convert.ToInt32(Console.ReadLine());

                if (menu == 0) break;

                switch (menu)
                {
                    case 1:
                        Curar();
                        break;
                    case 2:
                        MudarHabilidades();
                        break;
                    case 3:
                        AlterarItem();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Curar()
        {
            Console.WriteLine("Você foi curado em 30%: ");
            //Efeito de cura
        }

        private void MudarHabilidades()
        {
            Console.WriteLine("Para qual habilidade você deseja alterar: ");
            //Alterar a habilidade
        }

        private void AlterarItem()
        {
            Console.WriteLine("Para qual item você deseja alterar: ");
            //Alterar o item
        }
    }
}