namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Interface que define o contrato do State Pattern para telas e cenas do jogo.
    /// Garante ciclo de vida padronizado: Entrar, Renderizar, Atualizar, Limpar e Sair.
    /// </summary>
    public interface ITela
    {
        void Entrar();
        void Renderizar();
        void Atualizar();
        void Limpar();
        void Sair();
    }
}
