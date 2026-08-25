using System;

namespace ProjetoFinalPOO.Enums
{
    /// <summary>
    /// Implementa a matriz de vantagens e desvantagens entre afinidades de ataque e defesa.
    /// Regras do README:
    /// - Armadura: fraca contra Ácido (2.0x), neutra contra Elétrico (1.0x), forte contra Fogo (0.5x).
    /// - Mecânico: fraco contra Elétrico (2.0x), neutro contra Fogo (1.0x), forte contra Ácido (0.5x).
    /// - Biológico: fraco contra Fogo (2.0x), neutro contra Ácido (1.0x), forte contra Elétrico (0.5x).
    /// </summary>
    public static class TabelaAfinidades
    {
        public static double ObterMultiplicador(AfinidadeAtaque ataque, AfinidadeDefesa defesa)
        {
            return (ataque, defesa) switch
            {
                // Armadurado
                (AfinidadeAtaque.Acido, AfinidadeDefesa.Armadurado) => 2.0,   // Fraca
                (AfinidadeAtaque.Eletrico, AfinidadeDefesa.Armadurado) => 1.0, // Neutra
                (AfinidadeAtaque.Fogo, AfinidadeDefesa.Armadurado) => 0.5,    // Forte

                // Mecânico
                (AfinidadeAtaque.Eletrico, AfinidadeDefesa.Mecanico) => 2.0, // Fraca
                (AfinidadeAtaque.Fogo, AfinidadeDefesa.Mecanico) => 1.0,     // Neutra
                (AfinidadeAtaque.Acido, AfinidadeDefesa.Mecanico) => 0.5,    // Forte

                // Biológico
                (AfinidadeAtaque.Fogo, AfinidadeDefesa.Biologico) => 2.0,     // Fraca
                (AfinidadeAtaque.Acido, AfinidadeDefesa.Biologico) => 1.0,    // Neutra
                (AfinidadeAtaque.Eletrico, AfinidadeDefesa.Biologico) => 0.5, // Forte

                // Neutro / Padrão
                _ => 1.0
            };
        }

        public static string ObterTextoEfetividade(AfinidadeAtaque ataque, AfinidadeDefesa defesa)
        {
            double mult = ObterMultiplicador(ataque, defesa);
            if (mult > 1.0) return "[FRAQUEZA ELEMENTAL! +100% DANO]";
            if (mult < 1.0) return "[RESISTENTE! -50% DANO]";
            return "[DANO NEUTRO]";
        }
    }
}
