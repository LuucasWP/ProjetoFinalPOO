namespace ProjetoFinalPOO.Enums
{
    /// <summary>
    /// Tipos de encontros nos nós do mapa estelar estilo Slay the Spire.
    /// </summary>
    public enum TipoEncontro
    {
        CombateComum,
        CombateElite,
        Chefe,
        EstacaoReparo,
        Comercio,
        Bazar = Comercio,
        EventoAnomalia,
        Anomalia = EventoAnomalia
    }
}
