using System;

namespace ProjetoFinalPOO.Model.Excecoes
{
    /// <summary>
    /// Exceção personalizada para violações de regras do jogo (README e requisito OO-11).
    /// Lançada quando o jogador ou o motor tenta executar ações inválidas (ex: energia insuficiente,
    /// carta indisponível, salto para planeta não adjacente, inventário cheio, etc.).
    /// </summary>
    public class RegraJogoException : Exception
    {
        public string CodigoRegra { get; }

        public RegraJogoException(string mensagem, string codigoRegra = "REGRA_GERAL")
            : base(mensagem)
        {
            CodigoRegra = codigoRegra;
        }

        public RegraJogoException(string mensagem, Exception innerException, string codigoRegra = "REGRA_GERAL")
            : base(mensagem, innerException)
        {
            CodigoRegra = codigoRegra;
        }
    }
}
