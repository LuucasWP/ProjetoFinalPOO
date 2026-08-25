using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Registra o resultado detalhado de um embate (Clash) ou ataque unilateral.
    /// Atende às regras do README (Limbus Company, moedas, recursos, afinidades e ataques sem embate) e OO-10.
    /// </summary>
    public class ResultadoEmbate
    {
        public Combatente Atacante { get; set; }
        public Combatente Defensor { get; set; }
        public Habilidade HabilidadeAtacante { get; set; }
        public Habilidade HabilidadeDefensor { get; set; }
        public List<bool> MoedasAtacante { get; set; } = new List<bool>();
        public List<bool> MoedasDefensor { get; set; } = new List<bool>();
        public int PoderFinalAtacante { get; set; }
        public int PoderFinalDefensor { get; set; }
        public bool VitoriaAtacanteNoEmbate { get; set; }
        public bool EhAtaqueUnilateral { get; set; }
        public double MultiplicadorAfinidade { get; set; }
        public int DanoCausado { get; set; }
        public string MensagemLog { get; set; }
        public bool AtacanteEhAliado { get; set; } = true;
        public bool EhDistancia => false;

        public string NomeAtacante => Atacante?.Nome ?? "Atacante";
        public string NomeDefensor => Defensor?.Nome ?? "Defensor";
        public string NomeCarta => HabilidadeAtacante?.Nome ?? "Ataque";
        public int PoderBase => HabilidadeAtacante?.PoderBase ?? 0;
        public int ValorPorMoeda => HabilidadeAtacante?.PoderAdicionalMoeda ?? 0;
        public int PoderFinal => PoderFinalAtacante;
        public List<bool> MoedasResultado => MoedasAtacante;
        public bool VitoriaNoEmbate => VitoriaAtacanteNoEmbate;

        public ResultadoEmbate() { }

        public ResultadoEmbate(
            Combatente atacante,
            Combatente defensor,
            Habilidade habilidadeAtacante,
            Habilidade habilidadeDefensor,
            List<bool> moedasAtacante,
            List<bool> moedasDefensor,
            int poderFinalAtacante,
            int poderFinalDefensor,
            bool vitoriaAtacante,
            bool ehAtaqueUnilateral,
            double multiplicadorAfinidade,
            int danoCausado,
            string mensagemLog = "",
            bool atacanteEhAliado = true)
        {
            Atacante = atacante;
            Defensor = defensor;
            HabilidadeAtacante = habilidadeAtacante;
            HabilidadeDefensor = habilidadeDefensor;
            MoedasAtacante = moedasAtacante ?? new List<bool>();
            MoedasDefensor = moedasDefensor ?? new List<bool>();
            PoderFinalAtacante = poderFinalAtacante;
            PoderFinalDefensor = poderFinalDefensor;
            VitoriaAtacanteNoEmbate = vitoriaAtacante;
            EhAtaqueUnilateral = ehAtaqueUnilateral;
            MultiplicadorAfinidade = multiplicadorAfinidade;
            DanoCausado = danoCausado;
            MensagemLog = mensagemLog;
            AtacanteEhAliado = atacanteEhAliado;
        }

        public override string ToString()
        {
            if (EhAtaqueUnilateral)
            {
                return $"[ATAQUE UNILATERAL] {NomeAtacante} -> {NomeDefensor}: {DanoCausado} dano (Poder: {PoderFinalAtacante})";
            }
            string vencedor = VitoriaAtacanteNoEmbate ? NomeAtacante : NomeDefensor;
            return $"[EMBATE] {NomeAtacante} ({PoderFinalAtacante}) vs {NomeDefensor} ({PoderFinalDefensor}) -> Vencedor: {vencedor}, Dano: {DanoCausado}";
        }
    }
}
