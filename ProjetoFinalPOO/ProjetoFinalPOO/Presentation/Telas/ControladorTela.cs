using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Mapa;
using ProjetoFinalPOO.Model;
using ProjetoFinalPOO.Model.Telas;

namespace ProjetoFinalPOO.Controladores
{
    /// <summary>
    /// Fábrica (Factory Method) para instanciação e inicialização padronizada de telas do sistema.
    /// Centraliza a criação de telas e desacopla os controladores das classes concretas de visualização.
    /// </summary>
    public static class ControladorTela
    {
        public static TelaMenu CriarTelaMenu()
        {
            return new TelaMenu();
        }

        public static TelaOpcoes CriarTelaOpcoes()
        {
            return new TelaOpcoes();
        }

        public static TelaCreditos CriarTelaCreditos()
        {
            return new TelaCreditos();
        }

        public static TelaMapa CriarTelaMapa(
            ProjetoFinalPOO.Mapa.Mapa mapa,
            Grafo grafo = null,
            List<Combatente> tripulacao = null,
            List<Item> inventarioEquipe = null)
        {
            return new TelaMapa(mapa, grafo, tripulacao, inventarioEquipe);
        }

        public static TelaEventoPlaneta CriarTelaEventoPlaneta(
            Vertice verticePlaneta,
            List<Combatente> tripulacao,
            List<Item> inventarioEquipe)
        {
            return new TelaEventoPlaneta(verticePlaneta, tripulacao, inventarioEquipe);
        }

        public static TelaCombate CriarTelaCombate(
            List<Combatente> tripulacao,
            List<Combatente> inimigos,
            List<Item> inventarioEquipe,
            string nomeEncontro = "Patrulha Espacial",
            bool ehChefe = false)
        {
            return new TelaCombate(tripulacao, inimigos, inventarioEquipe, nomeEncontro, ehChefe);
        }

        public static TelaTrocaHabilidades CriarTelaTrocaHabilidades(List<Combatente> tripulacao)
        {
            return new TelaTrocaHabilidades(tripulacao);
        }

        public static TelaEscolhaItem CriarTelaEscolhaItem(List<Item> itensDisponiveis, List<Item> inventarioEquipe)
        {
            return new TelaEscolhaItem(itensDisponiveis, inventarioEquipe);
        }

        public static TelaFimDeJogo CriarTelaFimDeJogo(
            bool vitoria,
            int saltosRealizados,
            int combatesVencidos,
            int danoTotalCausado,
            int creditosObtidos,
            List<Combatente> tripulacao = null)
        {
            return new TelaFimDeJogo(vitoria, saltosRealizados, combatesVencidos, danoTotalCausado, creditosObtidos, tripulacao);
        }
    }
}
