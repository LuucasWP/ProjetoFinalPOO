using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Responsável por preparar e padronizar o console entre diferentes sistemas operacionais (Windows, Linux e macOS).
    /// Detecta e adapta as dimensões dinamicamente conforme o tamanho da janela do console em tela cheia.
    /// </summary>
    public static class ConfiguradorTela
    {
        public const int LarguraAlvo = 200;
        public const int AlturaAlvo = 55;
        public const int LarguraMinima = 90;
        public const int LarguraMaxima = 320;
        public const int AlturaMinima = 26;
        public const int AlturaMaxima = 120;

        public static int LarguraAtual => ObterLarguraConsole();
        public static int AlturaAtual => ObterAlturaConsole();

        private static int _ultimaLargura = -1;
        private static int _ultimaAltura = -1;

        // Constantes para chamadas nativas do Windows
        private const int SwMaximize = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static void ConfigurarTelaCheia()
        {
            ConfigurarCodificacao();
            OcultarCursor();

            if (OperatingSystem.IsWindows())
            {
                ConfigurarWindows();
            }
            else
            {
                ConfigurarUnix();
            }

            Console.Clear();
        }

        public static void RestaurarTela()
        {
            try
            {
                Console.CursorVisible = true;
                Console.ResetColor();

                // Restaura o buffer de tela padrão em sistemas Unix/Linux/macOS
                if (!OperatingSystem.IsWindows())
                {
                    Console.Write("\x1b[?1049l");
                }
            }
            catch
            {
                // Ignora falhas em ambientes sem terminal interativo
            }
        }

        private static void ConfigurarCodificacao()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch
            {
                // Fallback silencioso se o console não permitir alteração de encoding
            }
        }

        private static void OcultarCursor()
        {
            try
            {
                Console.CursorVisible = false;
            }
            catch
            {
                // Ambientes de console redirecionados podem não suportar CursorVisible
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void ConfigurarWindows()
        {
            try
            {
                IntPtr ponteiroJanela = GetConsoleWindow();
                if (ponteiroJanela != IntPtr.Zero)
                {
                    ShowWindow(ponteiroJanela, SwMaximize);
                }

                int largura = Console.LargestWindowWidth > 0 ? Console.LargestWindowWidth : LarguraAlvo;
                int altura = Console.LargestWindowHeight > 0 ? Console.LargestWindowHeight : AlturaAlvo;

                largura = Math.Clamp(largura, LarguraMinima, LarguraMaxima);
                altura = Math.Clamp(altura, AlturaMinima, AlturaMaxima);

                if (largura > 0 && altura > 0)
                {
                    Console.SetWindowSize(largura, altura);
                    Console.SetBufferSize(largura, altura);
                }
            }
            catch
            {
                // Fallback caso permissões ou driver de vídeo restrinjam redimensionamento
            }
        }

        private static void ConfigurarUnix()
        {
            try
            {
                // 1. Ativa o "alternate screen buffer" (estilo vim/htop)
                Console.Write("\x1b[?1049h");

                // 2. Redimensiona o terminal para preencher a tela caso o terminal suporte
                int larguraDesejada = Console.WindowWidth > 0 ? Math.Max(Console.WindowWidth, LarguraAlvo) : LarguraAlvo;
                int alturaDesejada = Console.WindowHeight > 0 ? Math.Max(Console.WindowHeight, AlturaAlvo) : AlturaAlvo;
                Console.Write($"\x1b[8;{alturaDesejada};{larguraDesejada}t");

                // 3. Move o cursor para a origem
                Console.Write("\x1b[H");
            }
            catch
            {
                // Fallback para terminais simples
            }
        }

        /// <summary>
        /// Obtém a largura atual do console, respeitando os limites do terminal em tela cheia.
        /// </summary>
        public static int ObterLarguraConsole()
        {
            try
            {
                int w = Console.WindowWidth;
                if (w > 0)
                {
                    return Math.Clamp(w, LarguraMinima, LarguraMaxima);
                }
                return LarguraAlvo;
            }
            catch
            {
                return LarguraAlvo;
            }
        }

        /// <summary>
        /// Obtém a altura atual do console, respeitando os limites do terminal em tela cheia.
        /// </summary>
        public static int ObterAlturaConsole()
        {
            try
            {
                int h = Console.WindowHeight;
                if (h > 0)
                {
                    return Math.Clamp(h, AlturaMinima, AlturaMaxima);
                }
                return AlturaAlvo;
            }
            catch
            {
                return AlturaAlvo;
            }
        }

        /// <summary>
        /// Detecta se a janela do console foi redimensionada pelo usuário desde a última verificação.
        /// </summary>
        public static bool HouveRedimensionamento()
        {
            int atualW = ObterLarguraConsole();
            int atualH = ObterAlturaConsole();

            if (_ultimaLargura != atualW || _ultimaAltura != atualH)
            {
                _ultimaLargura = atualW;
                _ultimaAltura = atualH;
                return true;
            }

            return false;
        }
    }
}
