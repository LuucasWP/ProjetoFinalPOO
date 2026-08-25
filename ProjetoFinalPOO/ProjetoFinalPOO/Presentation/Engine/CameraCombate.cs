using System;

namespace ProjetoFinalPOO.Model.Telas
{
    /// <summary>
    /// Representa um ponto ou vetor 3D no espaço do jogo.
    /// </summary>
    public struct Vetor3D
    {
        public double X;
        public double Y;
        public double Z;

        public Vetor3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vetor3D operator +(Vetor3D a, Vetor3D b) => new Vetor3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vetor3D operator -(Vetor3D a, Vetor3D b) => new Vetor3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vetor3D operator *(Vetor3D a, double s) => new Vetor3D(a.X * s, a.Y * s, a.Z * s);
    }

    /// <summary>
    /// Simula uma câmera dinâmica no console que rastreia e enquadra entidades móveis (Atacante e Defensor).
    /// Suporta projeção perspectiva (3D/2.5D), interpolação suave (Lerp), zoom dinâmico por distância e Screen Shake.
    /// </summary>
    public class CameraCombate
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }

        public double AlvoX { get; set; }
        public double AlvoY { get; set; }
        public double AlvoZ { get; set; }

        public double AnguloRotacao { get; set; }
        public double AlvoAngulo { get; set; }

        public double VelocidadeSuavizacao { get; set; } = 8.0; // Fator de Lerp
        public double TraumaTremor { get; set; } = 0.0;        // Intensidade do Screen Shake
        public int LarguraTela { get; set; } = 80;
        public int AlturaTela { get; set; } = 24;

        private readonly Random _random = new Random();

        public CameraCombate(int largura = 0, int altura = 0)
        {
            LarguraTela = largura > 0 ? largura : ConfiguradorTela.ObterLarguraConsole();
            AlturaTela = altura > 0 ? altura : ConfiguradorTela.ObterAlturaConsole();
            Resetar();
        }

        public void Resetar()
        {
            PosX = 0;
            PosY = 0;
            PosZ = -6.0;
            AlvoX = 0;
            AlvoY = 0;
            AlvoZ = -6.0;
            AnguloRotacao = 0;
            AlvoAngulo = 0;
            TraumaTremor = 0;
        }

        /// <summary>
        /// Aplica trauma para produzir efeito de impacto (Screen Shake).
        /// </summary>
        public void AdicionarImpacto(double intensidade = 1.0)
        {
            TraumaTremor = Math.Min(1.0, TraumaTremor + intensidade);
        }

        /// <summary>
        /// Enquadra automaticamente o atacante e o defensor, calculando o ponto médio e o zoom ideal.
        /// </summary>
        public void EnquadrarCombatentes(Vetor3D posAtacante, Vetor3D posDefensor, double margemZoom = 1.2)
        {
            // 1. Ponto Médio (Centro de Atenção)
            AlvoX = (posAtacante.X + posDefensor.X) * 0.5;
            AlvoY = (posAtacante.Y + posDefensor.Y) * 0.5;

            // 2. Distância entre os dois
            double distancia = Math.Abs(posAtacante.X - posDefensor.X);

            // 3. Zoom Dinâmico: quanto mais próximos, mais a câmera se aproxima (Z menos negativo)
            // Quanto mais distantes, mais a câmera se afasta (Z mais negativo)
            double zoomDesejado = -(2.8 + distancia * 0.75 * margemZoom);
            AlvoZ = Math.Max(-10.0, Math.Min(-3.5, zoomDesejado));
        }

        /// <summary>
        /// Foca a câmera diretamente em uma única entidade com zoom aproximado.
        /// </summary>
        public void FocarEntidade(Vetor3D pos, double zoomZ = -4.2)
        {
            AlvoX = pos.X;
            AlvoY = pos.Y;
            AlvoZ = zoomZ;
        }

        /// <summary>
        /// Atualiza a física da câmera e a suavização (Lerp) em cada quadro.
        /// </summary>
        public void Atualizar(double dt)
        {
            // Interpolação suave (Damping exponencial)
            double t = 1.0 - Math.Exp(-VelocidadeSuavizacao * dt);
            PosX += (AlvoX - PosX) * t;
            PosY += (AlvoY - PosY) * t;
            PosZ += (AlvoZ - PosZ) * t;
            AnguloRotacao += (AlvoAngulo - AnguloRotacao) * t;

            // Decaimento do Screen Shake
            if (TraumaTremor > 0)
            {
                TraumaTremor = Math.Max(0, TraumaTremor - 2.5 * dt);
            }
        }

        /// <summary>
        /// Rotaciona um ponto no plano XZ (Yaw da câmera).
        /// </summary>
        public Vetor3D RotacionarXZ(Vetor3D p, double angulo)
        {
            double c = Math.Cos(angulo);
            double s = Math.Sin(angulo);
            return new Vetor3D(
                p.X * c - p.Z * s,
                p.Y,
                p.X * s + p.Z * c
            );
        }

        /// <summary>
        /// Projeta um ponto do mundo 3D/2.5D para coordenadas de caracteres do Console.
        /// </summary>
        public (int TelaX, int TelaY, bool Visivel) ProjetarParaConsole(Vetor3D pontoMundo)
        {
            // 1. Deslocamento com Screen Shake
            double shakeX = 0;
            double shakeY = 0;
            if (TraumaTremor > 0)
            {
                double forca = TraumaTremor * TraumaTremor * 0.5;
                shakeX = (_random.NextDouble() * 2 - 1) * forca;
                shakeY = (_random.NextDouble() * 2 - 1) * forca;
            }

            // 2. Transformação de Espaço de Câmera (View Space)
            double xRel = pontoMundo.X - (PosX + shakeX);
            double yRel = pontoMundo.Y - (PosY + shakeY);
            double zRel = pontoMundo.Z - PosZ; // Z relativo à câmera (positivo à frente)

            if (zRel <= 0.1) // Atrás ou muito perto da câmera
                return (0, 0, false);

            // 3. Rotação de Câmera
            var rot = RotacionarXZ(new Vetor3D(xRel, yRel, zRel), -AnguloRotacao);

            // 4. Projeção Perspectiva: x / z e y / z
            double fovFator = 2.0;
            // Proporção de caracteres no console (um caractere costuma ser ~2x mais alto que largo)
            double proporcaoAspecto = (double)LarguraTela / (AlturaTela * 2.0);

            double ndcX = (rot.X * fovFator / proporcaoAspecto) / rot.Z;
            double ndcY = (rot.Y * fovFator) / rot.Z;

            // 5. Mapeamento para células do console [-1..1] -> [0..Largura, 0..Altura]
            int telaX = (int)Math.Round((ndcX + 1.0) * 0.5 * LarguraTela);
            int telaY = (int)Math.Round((1.0 - (ndcY + 1.0) * 0.5) * AlturaTela);

            bool visivel = telaX >= 0 && telaX < LarguraTela && telaY >= 0 && telaY < AlturaTela;
            return (telaX, telaY, visivel);
        }
    }
}
