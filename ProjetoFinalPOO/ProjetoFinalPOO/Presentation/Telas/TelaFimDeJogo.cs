using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Música;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Tela de encerramento da campanha (Vitória da Missão ou Derrota da Tripulação).
    /// Apresenta o relatório da Carga 73, estatísticas completas de dano, inimigos derrotados e evolução dos personagens.
    /// </summary>
    public class TelaFimDeJogo : ITela
    {
        private readonly bool _vitoria;
        private readonly int _saltosRealizados;
        private readonly int _combatesVencidos;
        private readonly int _danoTotalCausado;
        private readonly int _creditosObtidos;
        private readonly List<Combatente> _tripulacao;

        public TelaFimDeJogo(
            bool vitoria,
            int saltosRealizados,
            int combatesVencidos,
            int danoTotalCausado,
            int creditosObtidos,
            List<Combatente> tripulacao = null)
        {
            _vitoria = vitoria;
            _saltosRealizados = saltosRealizados;
            _combatesVencidos = combatesVencidos;
            _danoTotalCausado = danoTotalCausado;
            _creditosObtidos = creditosObtidos;
            _tripulacao = tripulacao ?? new List<Combatente>();
        }

        public void Entrar() { }
        public void Atualizar() { }
        public void Sair() { }

        public void Executar()
        {
            if (_vitoria)
            {
                try { BibliotecaDeMusicas.TemaEspacialOriginal()?.Tocar(); } catch { }
            }
            Renderizar();
            Console.ReadKey(true);
        }

        public void Renderizar()
        {
            Limpar();

            ConsoleColor corTema = _vitoria ? ConsoleColor.Green : ConsoleColor.Red;
            string titulo = _vitoria
                ? "MISSÃO CONCLUÍDA COM SUCESSO - A CARGA 73 ESTÁ SEGURA!"
                : "MISSÃO FRACASSADA - A NAVE VANGUARDA FOI DESTRUÍDA NO SETOR";

            RenderizadorUI.DesenharCabecalho(titulo, RenderizadorUI.LarguraPadrao, corTema);
            Console.WriteLine();

            DesenharPainelFinalComArte(corTema);
            DesenharStatusFinalTripulacao(corTema);
            DesenharRodape();
        }

        private void DesenharPainelFinalComArte(ConsoleColor corTema)
        {
            string[] arteFim = BancoSprites.ObterArteFimDeJogo(_vitoria);
            string tituloArte = _vitoria ? "TROFÉU DE EXTRAÇÃO ESTELAR" : "REGISTRO DE QUEDA DA NAVE";

            List<string> linhasInfo = new List<string>();
            if (_vitoria)
            {
                linhasInfo.Add("A nave Vanguarda atravessou com êxito as 6 galáxias e rompeu o cerco da Frota Sindical.");
                linhasInfo.Add("A misteriosa Carga 73 foi entregue em segurança no ponto de encontro com a Aliança Livre.");
                linhasInfo.Add("----------------------------------------------------------------------------------------");
                linhasInfo.Add($"Resultado da Campanha: [ VITÓRIA SUPREMA ]  |  Saltos Hiperespaciais: {_saltosRealizados}");
                linhasInfo.Add($"Combates Táticos Vencidos: {_combatesVencidos} vitórias | Dano Total Aplicado: {_danoTotalCausado} HP");
                linhasInfo.Add($"Créditos Espaciais Adquiridos: {_creditosObtidos} EC | Sobreviventes: {_tripulacao.FindAll(c => !c.EstaMorto).Count}/{_tripulacao.Count}");
                linhasInfo.Add("A tripulação mercenária agora é lendária em todo o setor espacial.");
            }
            else
            {
                linhasInfo.Add("Os sistemas da nave entraram em colapso crítico e a tripulação foi sobrepujada.");
                linhasInfo.Add("A Carga 73 foi capturada pelas forças da Frota Sindical. O sinal de socorro cessou...");
                linhasInfo.Add("----------------------------------------------------------------------------------------");
                linhasInfo.Add($"Resultado da Campanha: [ MISSÃO FRACASSADA ] | Saltos Realizados: {_saltosRealizados}");
                linhasInfo.Add($"Combates Vencidos: {_combatesVencidos} | Dano Total Desferido: {_danoTotalCausado} HP");
                linhasInfo.Add($"Créditos Restantes: {_creditosObtidos} EC | Sobreviventes: {_tripulacao.FindAll(c => !c.EstaMorto).Count}/{_tripulacao.Count}");
                linhasInfo.Add("Os registros da Vanguarda foram arquivados como aviso aos navegadores.");
            }

            RenderizadorUI.DesenharPainelCenarioComTexto(
                arteFim,
                tituloArte,
                linhasInfo,
                "RELATÓRIO FINAL DA DIRETRIZ MERCENÁRIA",
                corCenario: corTema,
                corTexto: ConsoleColor.White,
                corBorda: corTema
            );
            Console.WriteLine();
        }

        private void DesenharStatusFinalTripulacao(ConsoleColor corTema)
        {
            if (_tripulacao.Count == 0) return;

            RenderizadorUI.DesenharInicioSecao("REGISTRO FINAL DOS COMBATENTES MERCENÁRIOS", RenderizadorUI.LarguraPadrao, corTema);

            foreach (var c in _tripulacao)
            {
                bool vivo = !c.EstaMorto;
                string statusVivo = vivo ? $"[VIVO - HP: {c.VidaAtual}/{c.VidaTotal}]" : "[ABATIDO EM COMBATE]";
                ConsoleColor corStatus = vivo ? ConsoleColor.Green : ConsoleColor.Red;

                string modInfo = c switch
                {
                    Sentinela s => $"Adrenalina: {s.Adrenalina}/45",
                    Engenheiro e => $"Sobreaquecimento: {e.Sobreaquecimento}/45",
                    Biomancer b => $"Mana: {b.Mana}/45",
                    _ => $"Defesa: {c.Defesa}"
                };

                string esq = $"  - {c.Nome,-22} | Classe: {c.GetType().Name,-12} | Nível: {c.Level,2}/10 (EXP: {c.Exp})";
                string dir = $"{statusVivo} | {modInfo}";

                RenderizadorUI.DesenharLinhaDupla(esq, dir, RenderizadorUI.LarguraPadrao, ConsoleColor.White, corStatus, corTema);
            }

            RenderizadorUI.DesenharFimSecao(RenderizadorUI.LarguraPadrao, corTema);
            Console.WriteLine();
        }

        private void DesenharRodape()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ Pressione qualquer tecla para retornar ao Menu Principal... ]");
            Console.ResetColor();
        }

        public void Limpar()
        {
            Console.Clear();
        }
    }
}
