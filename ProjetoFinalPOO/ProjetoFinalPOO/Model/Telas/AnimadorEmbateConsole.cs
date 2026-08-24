using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Model;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Renderiza uma animação cinematográfica de combate no console com buffer duplo e controle de câmera 3D.
    /// Foca dinamicamente no Atacante e Defensor durante o avanço, impacto, Screen Shake, faíscas e recuo.
    /// Suporta redimensionamento automático de tela conforme o tamanho da janela do console.
    /// </summary>
    public class AnimadorEmbateConsole
    {
        private readonly int _largura;
        private readonly int _altura;
        private readonly char[,] _bufferChar;
        private readonly ConsoleColor[,] _bufferCor;
        private readonly CameraCombate _camera;

        public AnimadorEmbateConsole(int largura = 0, int altura = 0)
        {
            _largura = largura > 0 ? largura : ConfiguradorTela.ObterLarguraConsole();
            _altura = altura > 0 ? altura : Math.Min(32, ConfiguradorTela.ObterAlturaConsole());

            _largura = Math.Max(80, _largura);
            _altura = Math.Max(24, _altura);

            _bufferChar = new char[_altura, _largura];
            _bufferCor = new ConsoleColor[_altura, _largura];
            _camera = new CameraCombate(_largura, _altura);
        }

        public void ExecutarAnimacao(Combatente atacante, Combatente defensor, ResultadoEmbate resultado, bool? atacanteEhAliado = null)
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

            // Se for aliado atacando: Atacante começa na esquerda (-3.5, verde) e Defensor na direita (3.5, vermelho)
            // Se for inimigo atacando: Atacante começa na direita (3.5, vermelho) e Defensor na esquerda (-3.5, verde)
            Vetor3D posOriginalAtacante = ehAliado ? new Vetor3D(-3.5, 0, 0) : new Vetor3D(3.5, 0, 0);
            Vetor3D posOriginalDefensor = ehAliado ? new Vetor3D(3.5, 0, 0) : new Vetor3D(-3.5, 0, 0);

            ConsoleColor corAtacante = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;
            ConsoleColor corDefensor = ehAliado ? ConsoleColor.Red : ConsoleColor.Green;
            ConsoleColor corProjetil = ObterCorProjetil(afinidade, ehAliado);

            Vetor3D posAtacante = posOriginalAtacante;
            Vetor3D posDefensor = posOriginalDefensor;
            Vetor3D posProjetil = new Vetor3D(0, 0, 0);
            bool projetilAtivo = false;

            int totalFrames = 75;
            double fps = 20.0;
            double dt = 1.0 / fps;

            List<(Vetor3D Pos, Vetor3D Vel, char Ch, ConsoleColor Cor)> particulas = new List<(Vetor3D, Vetor3D, char, ConsoleColor)>();
            Random rand = new Random();

            for (int f = 0; f < totalFrames; f++)
            {
                // Permite ao jogador pular a animação a qualquer momento pressionando qualquer tecla
                if (!Console.IsInputRedirected)
                {
                    try
                    {
                        if (Console.KeyAvailable)
                        {
                            Console.ReadKey(true);
                            break;
                        }
                    }
                    catch { }
                }

                // ==========================================
                // 1. ATUALIZAÇÃO DA FÍSICA E POSIÇÃO DOS SPRITES / PROJÉTIL
                // ==========================================
                string statusMsg = "";
                ConsoleColor statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                if (ehDistancia)
                {
                    // =======================================================
                    // FLUXO DE ATAQUE À DISTÂNCIA (DISPARO DE PROJÉTIL)
                    // =======================================================
                    if (f < 22)
                    {
                        // Fase 1: Mira & Carregamento de Energia no cano / mãos
                        string nomeHab = resultado.NomeCarta.ToUpper();
                        string tagOrigem = ehAliado ? "" : " [INIMIGO]";
                        statusMsg = ehAliado
                            ? $">> {atacante.Nome.ToUpper()} MIRANDO COM [{nomeHab}] (DISPARO À DISTÂNCIA) >>"
                            : $"<< {atacante.Nome.ToUpper()}{tagOrigem} MIRANDO COM [{nomeHab}] (DISPARO À DISTÂNCIA) <<";
                        statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                        _camera.FocarEntidade(posAtacante, -4.0);
                        // Leve recuo para firmar a mira
                        posAtacante.X = posOriginalAtacante.X + (ehAliado ? -1 : 1) * (f / 22.0) * 0.2;

                        // Partículas de carregamento na ponta da arma
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
                        // Fase 2: Voo do Projétil pelo Campo
                        string tagOrigem = ehAliado ? "" : " INIMIGO";
                        statusMsg = ehAliado
                            ? $"[>> DISPARO DE PROJÉTIL EM ALTA VELOCIDADE CONTRA {defensor.Nome.ToUpper()}! >>]"
                            : $"[<< PROJÉTIL{tagOrigem} DISPARADO CONTRA {defensor.Nome.ToUpper()}! <<]";
                        statusCor = ehAliado ? ConsoleColor.Green : ConsoleColor.Red;

                        // Coice leve na arma do atacante que se estabiliza
                        double coice = (1.0 - (f - 22) / 18.0) * 0.3;
                        posAtacante.X = posOriginalAtacante.X + (ehAliado ? -coice : coice);

                        // Trajetória do Projétil
                        double progresso = (f - 22) / 18.0;
                        double startX = ehAliado ? posOriginalAtacante.X + 0.8 : posOriginalAtacante.X - 0.8;
                        double endX = ehAliado ? posOriginalDefensor.X - 0.3 : posOriginalDefensor.X + 0.3;

                        posProjetil.X = startX + (endX - startX) * progresso;
                        posProjetil.Y = 0.5 + Math.Sin(progresso * Math.PI) * 0.35; // Arco balístico suave
                        posProjetil.Z = 0;
                        projetilAtivo = true;

                        // Rastro de faíscas/fumaça atrás do projétil
                        particulas.Add((new Vetor3D(posProjetil.X + (ehAliado ? -0.4 : 0.4), posProjetil.Y, 0),
                                        new Vetor3D((rand.NextDouble() * 2 - 1) * 0.5, (rand.NextDouble() * 2 - 1) * 0.5, 0),
                                        (f % 2 == 0 ? '.' : '~'), corProjetil));

                        // Câmera acompanha o avanço do projétil
                        _camera.EnquadrarCombatentes(posAtacante, posProjetil, margemZoom: 1.1);
                    }
                    else if (f < 60)
                    {
                        // Fase 3: IMPACTO DO PROJÉTIL & EXPLOSÃO
                        projetilAtivo = false;
                        string tipoAcao = resultado.EhAtaqueUnilateral ? "DISPARO UNILATERAL" : "EMBATE À DISTÂNCIA";
                        statusMsg = $"[💥 {tipoAcao}! {resultado.PoderFinal} PODER -> DANO: {resultado.DanoCausado} HP EM {defensor.Nome.ToUpper()}!]";
                        statusCor = ehAliado ? ConsoleColor.Yellow : ConsoleColor.Red;

                        if (f == 40)
                        {
                            _camera.AdicionarImpacto(1.3); // Trepidação de tela

                            // Explosão de estilhaços / energia no defensor
                            for (int i = 0; i < 26; i++)
                            {
                                double ang = rand.NextDouble() * Math.PI * 2;
                                double spd = 3.0 + rand.NextDouble() * 5.0;
                                char ch = (i % 3 == 0) ? '*' : (i % 2 == 0 ? '+' : (ehAliado ? '/' : '\\'));
                                ConsoleColor corPart = (i % 2 == 0) ? ConsoleColor.Yellow : corProjetil;
                                particulas.Add((new Vetor3D(posOriginalDefensor.X + (ehAliado ? -0.3 : 0.3), 0.6, 0),
                                                new Vetor3D(Math.Cos(ang) * spd, Math.Sin(ang) * spd, 0), ch, corPart));
                            }
                        }

                        // Defensor sofre recuo (knockback) da explosão
                        double progressoImpacto = (f - 40) / 20.0;
                        posDefensor.X = posOriginalDefensor.X + (ehAliado ? 1.0 : -1.0) * Math.Sin(progressoImpacto * Math.PI) * 1.0;
                        _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 0.9);
                    }
                    else
                    {
                        // Fase 4: Dissipação e Reorganização
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
                    // =======================================================
                    // FLUXO DE ATAQUE CORPO-A-CORPO (INVESTIDA / DASH)
                    // =======================================================
                    if (ehAliado)
                    {
                        if (f < 22)
                        {
                            // Fase 1: Wind-up (Aliado preparando golpe corpo-a-corpo)
                            string nomeHab = resultado.NomeCarta.ToUpper();
                            statusMsg = $">> {atacante.Nome.ToUpper()} PREPARANDO [{nomeHab}] (Poder Base: {resultado.PoderBase}) >>";
                            statusCor = ConsoleColor.Green;
                            _camera.FocarEntidade(posAtacante, -4.0);
                            posAtacante.X = posOriginalAtacante.X - (f / 22.0) * 0.4;
                        }
                        else if (f < 40)
                        {
                            // Fase 2: Investida Rápida (Dash) para a direita
                            statusMsg = $"[>> AVANÇO RÁPIDO EM DIREÇÃO A {defensor.Nome.ToUpper()}! >>]";
                            statusCor = ConsoleColor.Green;
                            double progresso = (f - 22) / 18.0;
                            posAtacante.X = (posOriginalAtacante.X - 0.4) + progresso * (posOriginalDefensor.X - posOriginalAtacante.X - 1.2);
                            _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 1.1);
                        }
                        else if (f < 60)
                        {
                            // Fase 3: IMPACTO / GOLPE CORPO-A-CORPO
                            string tipoAcao = resultado.EhAtaqueUnilateral ? "ATAQUE UNILATERAL" : "EMBATE TÁTICO";
                            statusMsg = $"[💥 {tipoAcao}! {resultado.PoderFinal} PODER -> DANO: {resultado.DanoCausado} HP EM {defensor.Nome.ToUpper()}!]";
                            statusCor = ConsoleColor.Yellow;

                            if (f == 40)
                            {
                                _camera.AdicionarImpacto(1.2);

                                for (int i = 0; i < 22; i++)
                                {
                                    double ang = rand.NextDouble() * Math.PI * 2;
                                    double spd = 2.5 + rand.NextDouble() * 4.5;
                                    char ch = (i % 2 == 0) ? '*' : (i % 3 == 0 ? '/' : '+');
                                    ConsoleColor cor = (i % 2 == 0) ? ConsoleColor.Yellow : ConsoleColor.Green;
                                    particulas.Add((new Vetor3D(posDefensor.X - 0.5, 0.6, 0), new Vetor3D(Math.Cos(ang) * spd, Math.Sin(ang) * spd, 0), ch, cor));
                                }
                            }

                            double progressoImpacto = (f - 40) / 20.0;
                            posDefensor.X = posOriginalDefensor.X + Math.Sin(progressoImpacto * Math.PI) * 1.0;
                            _camera.EnquadrarCombatentes(posAtacante, posDefensor, margemZoom: 0.9);
                        }
                        else
                        {
                            // Fase 4: Retorno aos Slots
                            statusMsg = $"[DANO APLICADO EM {defensor.Nome.ToUpper()}! RETORNANDO AOS POSTOS...]";
                            statusCor = ConsoleColor.Yellow;
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
                        // Inimigo atacando corpo-a-corpo (direita para esquerda)
                        if (f < 22)
                        {
                            string nomeHab = resultado.NomeCarta.ToUpper();
                            statusMsg = $"<< {atacante.Nome.ToUpper()} [INIMIGO] PREPARANDO [{nomeHab}] (Poder: {resultado.PoderFinal}) <<";
                            statusCor = ConsoleColor.Red;
                            _camera.FocarEntidade(posAtacante, -4.0);
                            posAtacante.X = posOriginalAtacante.X + (f / 22.0) * 0.4;
                        }
                        else if (f < 40)
                        {
                            statusMsg = $"[<< INVESTIDA INIMIGA EM DIREÇÃO A {defensor.Nome.ToUpper()}! <<]";
                            statusCor = ConsoleColor.Red;
                            double progresso = (f - 22) / 18.0;
                            posAtacante.X = (posOriginalAtacante.X + 0.4) - progresso * (posOriginalAtacante.X - posOriginalDefensor.X - 1.2);
                            _camera.EnquadrarCombatentes(posDefensor, posAtacante, margemZoom: 1.1);
                        }
                        else if (f < 60)
                        {
                            string tipoAcao = resultado.EhAtaqueUnilateral ? "ATAQUE UNILATERAL INIMIGO" : "EMBATE TÁTICO INIMIGO";
                            statusMsg = $"[💥 {tipoAcao}! {resultado.PoderFinal} PODER -> DANO: {resultado.DanoCausado} HP NA TRIPULAÇÃO!]";
                            statusCor = ConsoleColor.Red;

                            if (f == 40)
                            {
                                _camera.AdicionarImpacto(1.2);

                                for (int i = 0; i < 22; i++)
                                {
                                    double ang = rand.NextDouble() * Math.PI * 2;
                                    double spd = 2.5 + rand.NextDouble() * 4.5;
                                    char ch = (i % 2 == 0) ? '*' : (i % 3 == 0 ? '\\' : '+');
                                    ConsoleColor cor = (i % 2 == 0) ? ConsoleColor.Yellow : ConsoleColor.Red;
                                    particulas.Add((new Vetor3D(posDefensor.X + 0.5, 0.6, 0), new Vetor3D(Math.Cos(ang) * spd, Math.Sin(ang) * spd, 0), ch, cor));
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

                // Atualizar partículas de faísca
                for (int i = particulas.Count - 1; i >= 0; i--)
                {
                    var p = particulas[i];
                    p.Pos.X += p.Vel.X * dt;
                    p.Pos.Y += p.Vel.Y * dt;
                    p.Vel.Y -= 9.8 * dt; // Gravidade
                    particulas[i] = p;
                }

                // Atualizar a câmera com Lerp
                _camera.Atualizar(dt);

                // ==========================================
                // 2. RENDERIZAÇÃO NO BUFFER DO CONSOLE
                // ==========================================
                LimparBuffer();

                DesenharGradeChao();
                DesenharSpriteProjetado(posAtacante, spriteAtacante, atacante.Nome, corAtacante);
                DesenharSpriteProjetado(posDefensor, spriteDefensor, defensor.Nome, corDefensor);

                if (projetilAtivo)
                {
                    DesenharProjetil(posProjetil, afinidade, ehAliado, corProjetil);
                }

                // Desenhar partículas de faísca
                foreach (var part in particulas)
                {
                    var (px, py, vis) = _camera.ProjetarParaConsole(part.Pos);
                    if (vis)
                    {
                        PlotarCaractere(px, py, part.Ch, part.Cor);
                    }
                }

                // HUD Superior e Inferior
                ConsoleColor corHud = ehAliado ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed;
                DesenharTextoNoBuffer(2, 0, $"=== SIMULAÇÃO CINEMATOGRÁFICA // EMBATE EM TEMPO REAL ===", corHud);
                DesenharTextoNoBuffer(2, 1, $"CÂMERA 3D // {atacante.Nome} ➔ {defensor.Nome} | ZOOM: {_camera.PosZ:F1} | [Pressione ESPAÇO para Pular]", ConsoleColor.DarkGray);
                
                string tipoAlcance = ehDistancia ? "DISPARO" : "CORPO-A-CORPO";
                string linhaAcao = $"AÇÃO: {atacante.Nome} ativou '{resultado.NomeCarta}' [{tipoAlcance}] (Poder: {resultado.PoderFinal}) | Alvo: {defensor.Nome}";
                DesenharTextoNoBuffer(2, _altura - 3, linhaAcao, ConsoleColor.Gray);
                DesenharTextoNoBuffer(2, _altura - 2, statusMsg, statusCor);

                if (f >= 40 && f < 68)
                {
                    // Dano flutuante sobre o defensor
                    var (hx, hy, hvis) = _camera.ProjetarParaConsole(new Vetor3D(posDefensor.X, 1.9, 0));
                    if (hvis)
                    {
                        string txtDano = $">> -{resultado.DanoCausado} HP! <<";
                        DesenharTextoNoBuffer(Math.Max(0, hx - txtDano.Length / 2), hy, txtDano, ConsoleColor.Red);
                    }
                }

                DesenharBordas();
                DespejarBufferNoConsole();

                Thread.Sleep((int)(dt * 1000));
            }
        }

        private void DesenharProjetil(Vetor3D pos, AfinidadeAtaque afinidade, bool ehAliado, ConsoleColor cor)
        {
            var (px, py, visivel) = _camera.ProjetarParaConsole(pos);
            if (!visivel) return;

            string glifo = ObterGlifoProjetil(afinidade, ehAliado);
            int offsetX = px - glifo.Length / 2;

            for (int i = 0; i < glifo.Length; i++)
            {
                int cx = offsetX + i;
                if (cx >= 0 && cx < _largura && py >= 0 && py < _altura)
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
                    AfinidadeAtaque.Eletrico => "≈⚡►",
                    AfinidadeAtaque.Acido => "◈═►",
                    _ => "══►"
                };
            }
            else
            {
                return afinidade switch
                {
                    AfinidadeAtaque.Fogo => "◄▲═",
                    AfinidadeAtaque.Eletrico => "◄⚡≈",
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
                AfinidadeAtaque.Fogo => ConsoleColor.Yellow,
                AfinidadeAtaque.Eletrico => ConsoleColor.Cyan,
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
            // Linha de horizonte/chão no espaço 3D projetada pela câmera
            for (double x = -9; x <= 9; x += 1.5)
            {
                var (tx, ty, vis) = _camera.ProjetarParaConsole(new Vetor3D(x, -0.4, 0));
                if (vis)
                {
                    PlotarCaractere(tx, ty, '.', ConsoleColor.DarkGreen);
                }
            }
        }

        private void DesenharSpriteProjetado(Vetor3D pos, string[] sprite, string nome, ConsoleColor cor)
        {
            var (baseX, baseY, visivel) = _camera.ProjetarParaConsole(pos);
            if (!visivel) return;

            int spriteLargura = (sprite != null && sprite.Length > 0) ? sprite[0].Length : 10;
            int offsetX = baseX - spriteLargura / 2;
            int offsetY = baseY - (sprite != null ? sprite.Length : 4);

            // Nome acima do sprite
            DesenharTextoNoBuffer(Math.Max(0, offsetX), Math.Max(0, offsetY - 1), nome, cor);

            // Linhas do sprite ASCII
            if (sprite != null)
            {
                for (int l = 0; l < sprite.Length; l++)
                {
                    int py = offsetY + l;
                    if (py < 0 || py >= _altura) continue;

                    string linha = sprite[l];
                    for (int c = 0; c < linha.Length; c++)
                    {
                        int px = offsetX + c;
                        if (px >= 0 && px < _largura && linha[c] != ' ')
                        {
                            _bufferChar[py, px] = linha[c];
                            _bufferCor[py, px] = cor;
                        }
                    }
                }
            }

            // Sombra no chão
            var (sx, sy, svis) = _camera.ProjetarParaConsole(new Vetor3D(pos.X, -0.2, 0));
            if (svis)
            {
                DesenharTextoNoBuffer(Math.Max(0, sx - 3), sy, "(====)", ConsoleColor.DarkGray);
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

        private void DesenharBordas()
        {
            for (int x = 0; x < _largura; x++)
            {
                PlotarCaractere(x, 0, '=', ConsoleColor.DarkCyan);
                PlotarCaractere(x, _altura - 1, '=', ConsoleColor.DarkCyan);
            }
            for (int y = 0; y < _altura; y++)
            {
                PlotarCaractere(0, y, '|', ConsoleColor.DarkCyan);
                PlotarCaractere(_largura - 1, y, '|', ConsoleColor.DarkCyan);
            }
        }

        private void DespejarBufferNoConsole()
        {
            try
            {
                Console.SetCursorPosition(0, 0);
            }
            catch
            {
                // Fallback para terminais sem suporte a SetCursorPosition
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
