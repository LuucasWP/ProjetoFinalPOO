using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Model;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Renderiza uma animação cinematográfica de combate no console com tela dividida:
    /// - Painel Esquerdo: Exibição detalhada e sem cortes das Moedas e Log formatado do Embate Atual com quebra automática de linha.
    /// - Painel Direito: Viewport 3D da animação com controle de câmera, sprites ASCII, projéteis e Screen Shake.
    /// </summary>
    public class AnimadorEmbateConsole
    {
        private readonly int _largura;
        private readonly int _altura;
        private readonly int _larguraLog;
        private readonly int _offsetAnimacaoX;
        private readonly int _larguraAnimacao;
        private readonly char[,] _bufferChar;
        private readonly ConsoleColor[,] _bufferCor;
        private readonly CameraCombate _camera;

        public AnimadorEmbateConsole(int largura = 0, int altura = 0)
        {
            _largura = largura > 0 ? largura : ConfiguradorTela.ObterLarguraConsole();
            _altura = altura > 0 ? altura : Math.Min(32, ConfiguradorTela.ObterAlturaConsole());

            _largura = Math.Max(100, _largura);
            _altura = Math.Max(26, _altura);

            // Largura generosa para o painel esquerdo evitando qualquer corte de texto
            _larguraLog = Math.Max(52, Math.Min(62, _largura * 48 / 100));
            _offsetAnimacaoX = _larguraLog + 1;
            _larguraAnimacao = _largura - _offsetAnimacaoX;

            _bufferChar = new char[_altura, _largura];
            _bufferCor = new ConsoleColor[_altura, _largura];
            _camera = new CameraCombate(_larguraAnimacao, _altura);
        }

        public void ExecutarAnimacao(
            Combatente atacante,
            Combatente defensor,
            ResultadoEmbate resultado,
            bool? atacanteEhAliado = null)
        {
            if (atacante == null || defensor == null || resultado == null) return;

            Console.CursorVisible = false;

            bool ehAliado = atacanteEhAliado ?? resultado.AtacanteEhAliado;
            bool ehDistancia = resultado.EhDistancia ||
                               resultado.NomeCarta.Contains("Disparo", StringComparison.OrdinalIgnoreCase) ||
                               resultado.NomeCarta.Contains("Canhão", StringComparison.OrdinalIgnoreCase) ||
                               resultado.NomeCarta.Contains("Laser", StringComparison.OrdinalIgnoreCase) ||
                               resultado.NomeCarta.Contains("Granada", StringComparison.OrdinalIgnoreCase) ||
                               resultado.NomeCarta.Contains("Dardo", StringComparison.OrdinalIgnoreCase);
            AfinidadeAtaque afinidade = resultado.HabilidadeAtacante?.Afinidade ?? AfinidadeAtaque.Fogo;

            string[] spriteAtacante = BancoSprites.ObterSprite(atacante?.Nome);
            string[] spriteDefensor = BancoSprites.ObterSprite(defensor?.Nome);

            Vetor3D posOriginalAtacante = ehAliado ? new Vetor3D(-3.2, 0, 0) : new Vetor3D(3.2, 0, 0);
            Vetor3D posOriginalDefensor = ehAliado ? new Vetor3D(3.2, 0, 0) : new Vetor3D(-3.2, 0, 0);

            ConsoleColor corAtacante = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;
            ConsoleColor corDefensor = ehAliado ? ConsoleColor.Red : ConsoleColor.Green;
            ConsoleColor corProjetil = ObterCorProjetil(afinidade, ehAliado);

            Vetor3D posAtacante = posOriginalAtacante;
            Vetor3D posDefensor = posOriginalDefensor;
            Vetor3D posProjetil = new Vetor3D(0, 0, 0);
            bool projetilAtivo = false;

            int totalFrames = 75;
            double fps = 18.0;
            double dt = 1.0 / fps;

            List<(Vetor3D Pos, Vetor3D Vel, char Ch, ConsoleColor Cor)> particulas = new List<(Vetor3D, Vetor3D, char, ConsoleColor)>();
            Random rand = new Random();

            for (int f = 0; f < totalFrames; f++)
            {
                // Pular animação se tecla de confirmação for pressionada (Enter, Espaço ou ESC)
                if (!Console.IsInputRedirected)
                {
                    try
                    {
                        if (Console.KeyAvailable)
                        {
                            ConsoleKey teclaPular = Console.ReadKey(true).Key;
                            if (teclaPular == ConsoleKey.Enter || teclaPular == ConsoleKey.Spacebar || teclaPular == ConsoleKey.Escape)
                            {
                                break;
                            }
                        }
                    }
                    catch { }
                }

                // ==========================================
                // 1. FÍSICA E MOVIMENTO DOS SPRITES / PROJÉTIL
                // ==========================================
                string statusMsg = "";
                ConsoleColor statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                if (ehDistancia)
                {
                    if (f < 22)
                    {
                        string nomeHab = resultado.NomeCarta.ToUpper();
                        string tagOrigem = ehAliado ? "" : " [INIMIGO]";
                        statusMsg = ehAliado
                            ? $">> {atacante.Nome.ToUpper()} MIRANDO COM [{nomeHab}] >>"
                            : $"<< {atacante.Nome.ToUpper()}{tagOrigem} MIRANDO COM [{nomeHab}] <<";
                        statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                        _camera.FocarEntidade(posAtacante, -4.0);
                        posAtacante.X = posOriginalAtacante.X + (ehAliado ? -1 : 1) * (f / 22.0) * 0.2;

                        if (f % 2 == 0)
                        {
                            double sparkX = posAtacante.X + (ehAliado ? 0.7 : -0.7);
                            particulas.Add((new Vetor3D(sparkX, 0.5 + (rand.NextDouble() * 0.2 - 0.1), 0),
                                            new Vetor3D((rand.NextDouble() * 2 - 1) * 0.8, (rand.NextDouble() * 2 - 1) * 0.8, 0),
                                            '.', corProjetil));
                        }
                    }
                    else if (f < 40)
                    {
                        statusMsg = ehAliado
                            ? $"[>> DISPARO DE PROJÉTIL CONTRA {defensor.Nome.ToUpper()}! >>]"
                            : $"[<< PROJÉTIL INIMIGO DISPARADO CONTRA {defensor.Nome.ToUpper()}! <<]";
                        statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                        double coice = (1.0 - (f - 22) / 18.0) * 0.3;
                        posAtacante.X = posOriginalAtacante.X + (ehAliado ? -coice : coice);

                        double progresso = (f - 22) / 18.0;
                        double startX = ehAliado ? posOriginalAtacante.X + 0.8 : posOriginalAtacante.X - 0.8;
                        double endX = ehAliado ? posOriginalDefensor.X - 0.3 : posOriginalDefensor.X + 0.3;

                        posProjetil.X = startX + (endX - startX) * progresso;
                        posProjetil.Y = 0.5 + Math.Sin(progresso * Math.PI) * 0.35;
                        posProjetil.Z = 0;
                        projetilAtivo = true;

                        particulas.Add((new Vetor3D(posProjetil.X + (ehAliado ? -0.4 : 0.4), posProjetil.Y, 0),
                                        new Vetor3D((rand.NextDouble() * 2 - 1) * 0.5, (rand.NextDouble() * 2 - 1) * 0.5, 0),
                                        (f % 2 == 0 ? '.' : '~'), corProjetil));

                        _camera.EnquadrarCombatentes(posAtacante, posProjetil, margemZoom: 1.1);
                    }
                    else if (f < 60)
                    {
                        projetilAtivo = false;
                        string tipoAcao = resultado.EhAtaqueUnilateral ? "DISPARO UNILATERAL" : "EMBATE À DISTÂNCIA";
                        statusMsg = $"[💥 {tipoAcao}! {resultado.PoderFinal} PODER -> DANO: {resultado.DanoCausado} HP EM {defensor.Nome.ToUpper()}!]";
                        statusCor = ehAliado ? ConsoleColor.Yellow : ConsoleColor.Red;

                        if (f == 40)
                        {
                            _camera.AdicionarImpacto(1.3);
                            for (int i = 0; i < 24; i++)
                            {
                                double ang = rand.NextDouble() * Math.PI * 2;
                                double spd = 3.0 + rand.NextDouble() * 5.0;
                                char ch = (i % 3 == 0) ? '*' : (i % 2 == 0 ? '+' : (ehAliado ? '/' : '\\'));
                                ConsoleColor corPart = (i % 2 == 0) ? ConsoleColor.Yellow : corProjetil;
                                particulas.Add((new Vetor3D(posOriginalDefensor.X + (ehAliado ? -0.3 : 0.3), 0.6, 0),
                                                new Vetor3D(Math.Cos(ang) * spd, Math.Sin(ang) * spd, 0), ch, corPart));
                            }
                        }

                        double progressoImpacto = (f - 40) / 20.0;
                        posDefensor.X = posOriginalDefensor.X + (ehAliado ? 1.0 : -1.0) * Math.Sin(progressoImpacto * Math.PI) * 1.0;
                        _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 0.9);
                    }
                    else
                    {
                        statusMsg = $"[DISPARO CONCLUÍDO! RETORNANDO AOS POSTOS...]";
                        statusCor = ehAliado ? ConsoleColor.Yellow : ConsoleColor.Red;
                        posDefensor.X = posOriginalDefensor.X;
                        posAtacante.X = posOriginalAtacante.X;
                        _camera.AlvoX = 0;
                        _camera.AlvoY = 0;
                        _camera.AlvoZ = -6.5;
                    }
                }
                else
                {
                    // Corpo a corpo
                    if (ehAliado)
                    {
                        if (f < 20)
                        {
                            string nomeHab = resultado.NomeCarta.ToUpper();
                            statusMsg = $">> {atacante.Nome.ToUpper()} PREPARANDO [{nomeHab}] (Poder: {resultado.PoderFinal}) >>";
                            _camera.FocarEntidade(posAtacante, -4.0);
                            posAtacante.X = posOriginalAtacante.X - (f / 20.0) * 0.4;
                        }
                        else if (f < 40)
                        {
                            statusMsg = $"[>> INVESTIDA TÁTICA EM DIREÇÃO A {defensor.Nome.ToUpper()}! >>]";
                            double progresso = (f - 20) / 20.0;
                            posAtacante.X = (posOriginalAtacante.X - 0.4) + progresso * (posOriginalDefensor.X - posOriginalAtacante.X - 0.8);
                            _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 1.1);
                        }
                        else if (f < 60)
                        {
                            string tipoAcao = resultado.EhAtaqueUnilateral ? "ATAQUE UNILATERAL" : "VITÓRIA NO EMBATE";
                            statusMsg = $"[💥 {tipoAcao}! {resultado.PoderFinal} PODER -> DANO: {resultado.DanoCausado} HP EM {defensor.Nome.ToUpper()}!]";
                            statusCor = ConsoleColor.Yellow;

                            if (f == 40)
                            {
                                _camera.AdicionarImpacto(1.5);
                                for (int i = 0; i < 24; i++)
                                {
                                    double vX = (rand.NextDouble() - 0.5) * 8.0;
                                    double vY = rand.NextDouble() * 7.0 + 1.0;
                                    char ch = rand.NextDouble() > 0.5 ? '*' : '+';
                                    particulas.Add((new Vetor3D(posOriginalDefensor.X, 0.6, 0), new Vetor3D(vX, vY, 0), ch, ConsoleColor.Yellow));
                                }
                            }

                            double progressoImpacto = (f - 40) / 20.0;
                            posDefensor.X = posOriginalDefensor.X + Math.Sin(progressoImpacto * Math.PI) * 1.1;
                            _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 0.9);
                        }
                        else
                        {
                            statusMsg = $"[GOLPE APLICADO COM SUCESSO! RECUANDO...]";
                            double progressoRetorno = (f - 60) / 15.0;
                            posAtacante.X = (posOriginalDefensor.X - 1.2) + (posOriginalAtacante.X - (posOriginalDefensor.X - 1.2)) * progressoRetorno;
                            posDefensor.X = posOriginalDefensor.X;
                            _camera.AlvoX = 0;
                            _camera.AlvoY = 0;
                            _camera.AlvoZ = -6.5;
                        }
                    }
                    else
                    {
                        if (f < 20)
                        {
                            statusMsg = $"[{atacante.Nome.ToUpper()} INVESTE BRUTALMENTE CONTRA A TRIPULAÇÃO...]";
                            double progressoAvanco = f / 20.0;
                            posAtacante.X = posOriginalAtacante.X + (posOriginalDefensor.X + 1.2 - posOriginalAtacante.X) * progressoAvanco;
                            _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 1.1);
                        }
                        else if (f < 40)
                        {
                            statusMsg = $"[BLOQUEIO EM CURSO! {defensor.Nome.ToUpper()} SUSTENTA O IMPACTO!]";
                            statusCor = ConsoleColor.Yellow;
                            posAtacante.X = posOriginalDefensor.X + 1.2 - Math.Sin((f - 20) * 0.8) * 0.15;
                            posDefensor.X = posOriginalDefensor.X - Math.Sin((f - 20) * 0.8) * 0.15;
                            _camera.AlvoX = (posAtacante.X + posDefensor.X) / 2.0;
                            _camera.AlvoY = 0;
                            _camera.AlvoZ = -4.5;
                        }
                        else if (f < 60)
                        {
                            statusMsg = $"!!! AMEAÇA INIMIGA CAUSA {resultado.DanoCausado} HP DE DANO NA TRIPULAÇÃO !!!";
                            statusCor = ConsoleColor.Red;
                            if (f == 40)
                            {
                                _camera.AdicionarImpacto(0.5);
                                for (int i = 0; i < 20; i++)
                                {
                                    double vX = (rand.NextDouble() - 0.5) * 7.0;
                                    double vY = rand.NextDouble() * 6.0 + 1.0;
                                    char ch = rand.NextDouble() > 0.5 ? '!' : '*';
                                    particulas.Add((new Vetor3D(posOriginalDefensor.X, 0.8, 0), new Vetor3D(vX, vY, 0), ch, ConsoleColor.Red));
                                }
                            }

                            double progressoImpacto = (f - 40) / 20.0;
                            posDefensor.X = posOriginalDefensor.X - Math.Sin(progressoImpacto * Math.PI) * 1.0;
                            _camera.EnquadrarCombatentes(posDefensor, posAtacante, margemZoom: 0.9);
                        }
                        else
                        {
                            statusMsg = $"[DANO APLICADO NA TRIPULAÇÃO ({defensor.Nome.ToUpper()})! RETORNANDO AOS POSTOS...]";
                            statusCor = ConsoleColor.Red;
                            double progressoRetorno = (f - 60) / 15.0;
                            posAtacante.X = (posOriginalDefensor.X + 1.2) + (posOriginalAtacante.X - (posOriginalDefensor.X + 1.2)) * progressoRetorno;
                            posDefensor.X = posOriginalDefensor.X;
                            _camera.AlvoX = 0;
                            _camera.AlvoY = 0;
                            _camera.AlvoZ = -6.5;
                        }
                    }
                }

                // Atualizar partículas
                for (int i = particulas.Count - 1; i >= 0; i--)
                {
                    var p = particulas[i];
                    p.Pos.X += p.Vel.X * dt;
                    p.Pos.Y += p.Vel.Y * dt;
                    p.Vel.Y -= 9.8 * dt;
                    particulas[i] = p;
                }
                _camera.Atualizar(dt);

                // ==========================================
                // 2. RENDERIZAÇÃO NO BUFFER DIVIDIDO
                // ==========================================
                LimparBuffer();

                // Desenha Painel Esquerdo (Moedas e Log do Embate Formatado)
                DesenharPainelEsquerdoLogEMoedas(resultado, f);

                // Divisória Central
                DesenharDivisoriaCentral();

                // Desenha Painel Direito (Animação 3D)
                DesenharGradeChao();
                DesenharSpriteProjetado(posAtacante, spriteAtacante, atacante.Nome, corAtacante);
                DesenharSpriteProjetado(posDefensor, spriteDefensor, defensor.Nome, corDefensor);

                if (projetilAtivo)
                {
                    DesenharProjetil(posProjetil, afinidade, ehAliado, corProjetil);
                }

                foreach (var part in particulas)
                {
                    var (px, py, vis) = _camera.ProjetarParaConsole(part.Pos);
                    int consoleX = _offsetAnimacaoX + px;
                    if (vis && consoleX > _larguraLog && consoleX < _largura - 1)
                    {
                        PlotarCaractere(consoleX, py, part.Ch, part.Cor);
                    }
                }

                // HUD do Painel Direito
                ConsoleColor corHud = ehAliado ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed;
                DesenharTextoNoBuffer(_offsetAnimacaoX + 2, 1, $"=== SIMULAÇÃO // CÂMERA 3D ===", corHud);
                DesenharTextoNoBuffer(_offsetAnimacaoX + 2, 2, $"{atacante.Nome} ➔ {defensor.Nome} | [ESPAÇO/ESC: Pular]", ConsoleColor.DarkGray);

                string tipoAlcance = ehDistancia ? "DISPARO" : "CORPO-A-CORPO";
                string linhaAcao = $"AÇÃO: {atacante.Nome} '{resultado.NomeCarta}' [{tipoAlcance}] (Poder: {resultado.PoderFinal})";
                DesenharTextoNoBuffer(_offsetAnimacaoX + 2, _altura - 4, CortarTexto(linhaAcao, _larguraAnimacao - 4), ConsoleColor.Gray);
                DesenharTextoNoBuffer(_offsetAnimacaoX + 2, _altura - 3, CortarTexto(statusMsg, _larguraAnimacao - 4), statusCor);

                DesenharBordasExternas();
                DespejarBufferNoConsole();

                Thread.Sleep((int)(dt * 1000));
            }

            // ==========================================
            // 3. QUADRO FINAL COM PAUSA (READKEY)
            // ==========================================
            LimparBuffer();
            DesenharPainelEsquerdoLogEMoedas(resultado, 100);
            DesenharDivisoriaCentral();

            DesenharGradeChao();
            DesenharSpriteProjetado(posOriginalAtacante, spriteAtacante, atacante.Nome, corAtacante);
            DesenharSpriteProjetado(posOriginalDefensor, spriteDefensor, defensor.Nome, corDefensor);

            ConsoleColor corHudFim = ehAliado ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed;
            DesenharTextoNoBuffer(_offsetAnimacaoX + 2, 1, $"=== RESULTADO DO EMBATE TÁTICO ===", corHudFim);

            string resumoFinal = $"DESFECHO: {atacante.Nome} causou {resultado.DanoCausado} dano em {defensor.Nome} | HP: {defensor.VidaAtual}/{defensor.VidaTotal}";
            DesenharTextoNoBuffer(_offsetAnimacaoX + 2, _altura - 4, CortarTexto(resumoFinal, _larguraAnimacao - 4), ConsoleColor.White);

            string promptContinuar = "[ EMBATE CONCLUÍDO ] >> Pressione ENTER, ESPAÇO ou ESC para continuar <<";
            DesenharTextoNoBuffer(_offsetAnimacaoX + Math.Max(2, (_larguraAnimacao - promptContinuar.Length) / 2), _altura - 3, promptContinuar, ConsoleColor.Yellow);

            DesenharBordasExternas();
            DespejarBufferNoConsole();

            if (!Console.IsInputRedirected)
            {
                try
                {
                    while (Console.KeyAvailable) Console.ReadKey(true);
                    while (true)
                    {
                        ConsoleKey teclaFim = Console.ReadKey(true).Key;
                        if (teclaFim == ConsoleKey.Enter || teclaFim == ConsoleKey.Spacebar || teclaFim == ConsoleKey.Escape)
                        {
                            break;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Desenha no lado esquerdo o painel de moedas e o log formatado sem cortes nem truncamentos.
        /// </summary>
        private void DesenharPainelEsquerdoLogEMoedas(ResultadoEmbate resultado, int frame)
        {
            int larguraUtil = _larguraLog - 2;

            // 1. Moldura Superior Dinâmica
            string tituloMoedas = resultado.EhAtaqueUnilateral
                ? " RESULTADO: ATAQUE DIRETO "
                : " RESULTADO DAS MOEDAS [CLASH] ";
            string linhaTopo = "┌─" + tituloMoedas + new string('─', Math.Max(0, larguraUtil - tituloMoedas.Length - 2)) + "┐";
            DesenharTextoNoBuffer(1, 1, CortarTexto(linhaTopo, larguraUtil + 1), ConsoleColor.Cyan);

            // 2. Seção do Atacante (formatada em 3 linhas sem corte)
            DesenharTextoNoBuffer(2, 2, CortarTexto($"ATACANTE: {resultado.NomeAtacante}", larguraUtil - 2), ConsoleColor.Yellow);
            DesenharTextoNoBuffer(2, 3, CortarTexto($"  Habilidade: [{resultado.NomeCarta}] ({resultado.HabilidadeAtacante?.Afinidade})", larguraUtil - 2), ConsoleColor.White);

            string moedasAtacanteStr = FormatarMoedasString(resultado.HabilidadeAtacante, resultado.MoedasAtacante, resultado.PoderFinalAtacante);
            DesenharTextoNoBuffer(2, 4, CortarTexto($"  Moedas: {moedasAtacanteStr}", larguraUtil - 2), ConsoleColor.Yellow);

            // 3. Seção do Defensor (formatada em linhas dedicadas)
            if (resultado.EhAtaqueUnilateral || resultado.HabilidadeDefensor == null)
            {
                DesenharTextoNoBuffer(2, 5, CortarTexto($"DEFENSOR: {resultado.NomeDefensor} [Sem Oposição]", larguraUtil - 2), ConsoleColor.DarkGray);
                DesenharTextoNoBuffer(2, 6, CortarTexto($"  DANO APLICADO: {resultado.DanoCausado} HP", larguraUtil - 2), ConsoleColor.Green);
            }
            else
            {
                DesenharTextoNoBuffer(2, 5, CortarTexto($"DEFENSOR: {resultado.NomeDefensor}", larguraUtil - 2), ConsoleColor.Magenta);
                DesenharTextoNoBuffer(2, 6, CortarTexto($"  Habilidade: [{resultado.HabilidadeDefensor.Nome}] ({resultado.HabilidadeDefensor?.Afinidade})", larguraUtil - 2), ConsoleColor.White);

                string moedasDefensorStr = FormatarMoedasString(resultado.HabilidadeDefensor, resultado.MoedasDefensor, resultado.PoderFinalDefensor);
                DesenharTextoNoBuffer(2, 7, CortarTexto($"  Moedas: {moedasDefensorStr}", larguraUtil - 2), ConsoleColor.Magenta);

                string vencedorTag = resultado.VitoriaAtacanteNoEmbate
                    ? $"  >>> VITÓRIA: {resultado.NomeAtacante.ToUpper()} ({resultado.PoderFinalAtacante} > {resultado.PoderFinalDefensor}) <<<"
                    : $"  >>> VITÓRIA: {resultado.NomeDefensor.ToUpper()} ({resultado.PoderFinalDefensor} > {resultado.PoderFinalAtacante}) <<<";
                ConsoleColor corVencedor = resultado.VitoriaAtacanteNoEmbate ? ConsoleColor.Green : ConsoleColor.Red;
                DesenharTextoNoBuffer(2, 8, CortarTexto(vencedorTag, larguraUtil - 2), corVencedor);
            }

            // 4. Divisória Dinâmica de Moedas / Log
            int linhaDivisoria = (resultado.EhAtaqueUnilateral || resultado.HabilidadeDefensor == null) ? 7 : 9;
            string tituloLog = " LOG DO EMBATE ATUAL ";
            string linhaMeio = "├─" + tituloLog + new string('─', Math.Max(0, larguraUtil - tituloLog.Length - 2)) + "┤";
            DesenharTextoNoBuffer(1, linhaDivisoria, CortarTexto(linhaMeio, larguraUtil + 1), ConsoleColor.DarkCyan);

            // 5. Exibição do Log do Embate com Quebra de Linha Automática
            int linhaInicialLog = linhaDivisoria + 1;
            int totalLinhasLog = _altura - linhaInicialLog - 2;

            var logsFormatados = ObterLogsDoEmbateFormatados(resultado, frame, larguraUtil - 2);
            if (logsFormatados.Count > 0 && totalLinhasLog > 0)
            {
                int inicio = Math.Max(0, logsFormatados.Count - totalLinhasLog);
                int count = Math.Min(totalLinhasLog, logsFormatados.Count - inicio);

                for (int i = 0; i < count; i++)
                {
                    var itemLog = logsFormatados[inicio + i];
                    DesenharTextoNoBuffer(2, linhaInicialLog + i, CortarTexto(itemLog.Texto, larguraUtil - 1), itemLog.Cor);
                }
            }
        }

        private List<(string Texto, ConsoleColor Cor)> ObterLogsDoEmbateFormatados(ResultadoEmbate resultado, int frameAtual, int maxLargura)
        {
            var listaFinal = new List<(string, ConsoleColor)>();
            if (resultado == null) return listaFinal;

            bool ehUnilateral = resultado.EhAtaqueUnilateral || resultado.HabilidadeDefensor == null;

            // Frame >= 0: Abertura da ação
            if (frameAtual >= 0)
            {
                if (ehUnilateral)
                {
                    AdicionarLogFormatado(listaFinal, $"[AÇÃO DIRETA] {resultado.NomeAtacante} avança", ConsoleColor.Cyan, maxLargura);
                    AdicionarLogFormatado(listaFinal, $"• Carta: '{resultado.NomeCarta}' ({resultado.HabilidadeAtacante?.Afinidade})", ConsoleColor.White, maxLargura);
                    AdicionarLogFormatado(listaFinal, $"• Alvo: {resultado.NomeDefensor} (Sem oposição)", ConsoleColor.DarkGray, maxLargura);
                }
                else
                {
                    AdicionarLogFormatado(listaFinal, $"[EMBATE TÁTICO] {resultado.NomeAtacante} VS {resultado.NomeDefensor}", ConsoleColor.Cyan, maxLargura);
                    AdicionarLogFormatado(listaFinal, $"• {resultado.NomeAtacante}: '{resultado.NomeCarta}'", ConsoleColor.Yellow, maxLargura);
                    AdicionarLogFormatado(listaFinal, $"• {resultado.NomeDefensor}: '{resultado.HabilidadeDefensor?.Nome ?? "Ataque"}'", ConsoleColor.Magenta, maxLargura);
                }
            }

            // Frame >= 16: Lançamento de moedas
            if (frameAtual >= 16)
            {
                AdicionarLogFormatado(listaFinal, $"[LANÇAMENTO DE MOEDAS]", ConsoleColor.Yellow, maxLargura);
                string moedasAtac = FormatarMoedasString(resultado.HabilidadeAtacante, resultado.MoedasAtacante, resultado.PoderFinalAtacante);
                AdicionarLogFormatado(listaFinal, $"• {resultado.NomeAtacante}: {moedasAtac}", ConsoleColor.White, maxLargura);

                if (!ehUnilateral)
                {
                    string moedasDef = FormatarMoedasString(resultado.HabilidadeDefensor, resultado.MoedasDefensor, resultado.PoderFinalDefensor);
                    AdicionarLogFormatado(listaFinal, $"• {resultado.NomeDefensor}: {moedasDef}", ConsoleColor.White, maxLargura);

                    if (resultado.VitoriaAtacanteNoEmbate)
                    {
                        AdicionarLogFormatado(listaFinal, $"• Vencedor: {resultado.NomeAtacante} ({resultado.PoderFinalAtacante} > {resultado.PoderFinalDefensor})!", ConsoleColor.Green, maxLargura);
                    }
                    else
                    {
                        AdicionarLogFormatado(listaFinal, $"• Vencedor: {resultado.NomeDefensor} ({resultado.PoderFinalDefensor} > {resultado.PoderFinalAtacante})!", ConsoleColor.Red, maxLargura);
                    }
                }
            }

            // Frame >= 38: Impacto e dano
            if (frameAtual >= 38)
            {
                AdicionarLogFormatado(listaFinal, $"[IMPACTO & DANO]", ConsoleColor.Yellow, maxLargura);
                AdicionarLogFormatado(listaFinal, $"• Dano Aplicado: {resultado.DanoCausado} HP", ConsoleColor.White, maxLargura);
                if (resultado.MultiplicadorAfinidade != 1.0 && resultado.MultiplicadorAfinidade > 0)
                {
                    AdicionarLogFormatado(listaFinal, $"• Afinidade: {resultado.MultiplicadorAfinidade:F1}x multiplicador", ConsoleColor.Cyan, maxLargura);
                }
            }

            // Frame >= 58: Desfecho
            if (frameAtual >= 58)
            {
                AdicionarLogFormatado(listaFinal, $"[DESFECHO]", ConsoleColor.Cyan, maxLargura);
                if (resultado.Defensor != null)
                {
                    AdicionarLogFormatado(listaFinal, $"• HP de {resultado.NomeDefensor}: {resultado.Defensor.VidaAtual}/{resultado.Defensor.VidaTotal}", ConsoleColor.White, maxLargura);
                    if (resultado.Defensor.EstaMorto)
                    {
                        AdicionarLogFormatado(listaFinal, $"• [ALVO ABATIDO] {resultado.NomeDefensor} neutralizado!", ConsoleColor.Red, maxLargura);
                    }
                }
            }

            return listaFinal;
        }

        private static void AdicionarLogFormatado(List<(string Texto, ConsoleColor Cor)> lista, string texto, ConsoleColor cor, int maxLargura)
        {
            if (string.IsNullOrEmpty(texto)) return;

            if (texto.Length <= maxLargura)
            {
                lista.Add((texto, cor));
                return;
            }

            string[] palavras = texto.Split(' ');
            StringBuilder linhaAtual = new StringBuilder();

            foreach (var palavra in palavras)
            {
                if (linhaAtual.Length + palavra.Length + 1 > maxLargura)
                {
                    if (linhaAtual.Length > 0)
                    {
                        lista.Add((linhaAtual.ToString(), cor));
                        linhaAtual.Clear();
                        linhaAtual.Append("  "); // Indentação de continuação
                    }
                }

                if (linhaAtual.Length > 0 && linhaAtual.ToString() != "  ")
                    linhaAtual.Append(" ");

                linhaAtual.Append(palavra);
            }

            if (linhaAtual.Length > 0)
            {
                lista.Add((linhaAtual.ToString(), cor));
            }
        }

        private string FormatarMoedasString(Habilidade hab, List<bool> moedas, int poderFinal)
        {
            if (hab == null) return $"Poder Total: {poderFinal}";

            StringBuilder sb = new StringBuilder();
            sb.Append($"Base: {hab.PoderBase} | ");

            if (moedas != null && moedas.Count > 0)
            {
                for (int i = 0; i < moedas.Count; i++)
                {
                    if (moedas[i])
                        sb.Append($"[● +{hab.PoderAdicionalMoeda}] ");
                    else
                        sb.Append("[○ +0] ");
                }
            }
            else
            {
                for (int i = 0; i < hab.Moeda; i++)
                {
                    sb.Append($"[● +{hab.PoderAdicionalMoeda}] ");
                }
            }

            sb.Append($"= {poderFinal}");
            return sb.ToString();
        }

        private void DesenharDivisoriaCentral()
        {
            for (int y = 1; y < _altura - 1; y++)
            {
                PlotarCaractere(_larguraLog, y, '║', ConsoleColor.DarkCyan);
            }
            PlotarCaractere(_larguraLog, 0, '╦', ConsoleColor.DarkCyan);
            PlotarCaractere(_larguraLog, _altura - 1, '╩', ConsoleColor.DarkCyan);
        }

        private void DesenharProjetil(Vetor3D pos, AfinidadeAtaque afinidade, bool ehAliado, ConsoleColor cor)
        {
            var (px, py, visivel) = _camera.ProjetarParaConsole(pos);
            if (!visivel) return;

            string glifo = ObterGlifoProjetil(afinidade, ehAliado);
            int offsetX = _offsetAnimacaoX + px - glifo.Length / 2;

            for (int i = 0; i < glifo.Length; i++)
            {
                int cx = offsetX + i;
                if (cx > _larguraLog && cx < _largura - 1 && py >= 1 && py < _altura - 1)
                {
                    _bufferChar[py, cx] = glifo[i];
                    _bufferCor[py, cx] = cor;
                }
            }
        }

        private static string ObterGlifoProjetil(AfinidadeAtaque afinidade, bool ehAliado)
        {
            if (ehAliado)
            {
                return afinidade switch
                {
                    AfinidadeAtaque.Fogo => "═▲►",
                    AfinidadeAtaque.Eletrico => "≈*►",
                    AfinidadeAtaque.Acido => "◈═►",
                    _ => "══►"
                };
            }
            else
            {
                return afinidade switch
                {
                    AfinidadeAtaque.Fogo => "◄▲═",
                    AfinidadeAtaque.Eletrico => "◄*≈",
                    AfinidadeAtaque.Acido => "◄═◈",
                    _ => "◄══"
                };
            }
        }

        private static ConsoleColor ObterCorProjetil(AfinidadeAtaque afinidade, bool ehAliado)
        {
            if (!ehAliado) return ConsoleColor.Red;

            return afinidade switch
            {
                AfinidadeAtaque.Fogo => ConsoleColor.Red,
                AfinidadeAtaque.Eletrico => ConsoleColor.DarkYellow,
                AfinidadeAtaque.Acido => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }

        private void LimparBuffer()
        {
            for (int y = 0; y < _altura; y++)
            {
                for (int x = 0; x < _largura; x++)
                {
                    _bufferChar[y, x] = ' ';
                    _bufferCor[y, x] = ConsoleColor.Black;
                }
            }
        }

        private void DesenharGradeChao()
        {
            for (double x = -8; x <= 8; x += 1.6)
            {
                var (tx, ty, vis) = _camera.ProjetarParaConsole(new Vetor3D(x, -0.4, 0));
                int consoleX = _offsetAnimacaoX + tx;
                if (vis && consoleX > _larguraLog && consoleX < _largura - 1 && ty >= 1 && ty < _altura - 1)
                {
                    PlotarCaractere(consoleX, ty, '.', ConsoleColor.DarkGreen);
                }
            }
        }

        private void DesenharSpriteProjetado(Vetor3D pos, string[] sprite, string nome, ConsoleColor cor)
        {
            var (baseX, baseY, visivel) = _camera.ProjetarParaConsole(pos);
            if (!visivel) return;

            int spriteLargura = (sprite != null && sprite.Length > 0) ? sprite[0].Length : 10;
            int offsetX = _offsetAnimacaoX + baseX - spriteLargura / 2;
            int offsetY = baseY - (sprite != null ? sprite.Length : 4);

            // Nome acima do sprite
            int nomeX = Math.Max(_offsetAnimacaoX + 1, offsetX);
            if (offsetY - 1 >= 1 && offsetY - 1 < _altura - 1 && nomeX < _largura - 1)
            {
                DesenharTextoNoBuffer(nomeX, offsetY - 1, CortarTexto(nome, _largura - nomeX - 1), cor);
            }

            // Linhas do sprite ASCII
            if (sprite != null)
            {
                for (int l = 0; l < sprite.Length; l++)
                {
                    int py = offsetY + l;
                    if (py < 1 || py >= _altura - 1) continue;

                    string linha = sprite[l];
                    for (int c = 0; c < linha.Length; c++)
                    {
                        int px = offsetX + c;
                        if (px > _larguraLog && px < _largura - 1 && linha[c] != ' ')
                        {
                            _bufferChar[py, px] = linha[c];
                            _bufferCor[py, px] = cor;
                        }
                    }
                }
            }

            // Sombra no chão
            var (sx, sy, svis) = _camera.ProjetarParaConsole(new Vetor3D(pos.X, -0.2, 0));
            int shadowX = _offsetAnimacaoX + sx - 3;
            if (svis && shadowX > _larguraLog && shadowX < _largura - 7 && sy >= 1 && sy < _altura - 1)
            {
                DesenharTextoNoBuffer(shadowX, sy, "(====)", ConsoleColor.DarkGray);
            }
        }

        private void DesenharTextoNoBuffer(int x, int y, string texto, ConsoleColor cor)
        {
            if (y < 0 || y >= _altura || string.IsNullOrEmpty(texto)) return;

            for (int i = 0; i < texto.Length; i++)
            {
                int px = x + i;
                if (px >= 0 && px < _largura)
                {
                    _bufferChar[y, px] = texto[i];
                    _bufferCor[y, px] = cor;
                }
            }
        }

        private void PlotarCaractere(int x, int y, char ch, ConsoleColor cor)
        {
            if (x >= 0 && x < _largura && y >= 0 && y < _altura)
            {
                _bufferChar[y, x] = ch;
                _bufferCor[y, x] = cor;
            }
        }

        private void DesenharBordasExternas()
        {
            for (int x = 0; x < _largura; x++)
            {
                PlotarCaractere(x, 0, '═', ConsoleColor.DarkCyan);
                PlotarCaractere(x, _altura - 1, '═', ConsoleColor.DarkCyan);
            }
            for (int y = 0; y < _altura; y++)
            {
                PlotarCaractere(0, y, '║', ConsoleColor.DarkCyan);
                PlotarCaractere(_largura - 1, y, '║', ConsoleColor.DarkCyan);
            }
            PlotarCaractere(0, 0, '╔', ConsoleColor.DarkCyan);
            PlotarCaractere(_largura - 1, 0, '╗', ConsoleColor.DarkCyan);
            PlotarCaractere(0, _altura - 1, '╚', ConsoleColor.DarkCyan);
            PlotarCaractere(_largura - 1, _altura - 1, '╝', ConsoleColor.DarkCyan);
        }

        private static string CortarTexto(string texto, int maxTam)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            if (maxTam <= 0) return "";
            return texto.Length > maxTam ? texto.Substring(0, maxTam) : texto;
        }

        private void DespejarBufferNoConsole()
        {
            try
            {
                Console.SetCursorPosition(0, 0);
            }
            catch
            {
                // Fallback para ambientes sem suporte a SetCursorPosition
            }

            ConsoleColor corAtual = ConsoleColor.White;
            Console.ForegroundColor = corAtual;

            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < _altura; y++)
            {
                for (int x = 0; x < _largura; x++)
                {
                    ConsoleColor cor = _bufferCor[y, x];
                    if (cor != corAtual)
                    {
                        if (sb.Length > 0)
                        {
                            Console.Write(sb.ToString());
                            sb.Clear();
                        }
                        Console.ForegroundColor = cor;
                        corAtual = cor;
                    }
                    sb.Append(_bufferChar[y, x]);
                }
                sb.AppendLine();
            }

            if (sb.Length > 0)
            {
                Console.Write(sb.ToString());
            }
            Console.ResetColor();
        }
    }
}
