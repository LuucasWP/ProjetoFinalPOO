using System;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Fornece artes ASCII (sprites, retratos, cenários, ícones de itens e molduras/placeholders de imagem)
    /// padronizadas para exibição visual nas interfaces do console.
    /// Permite uma interface rica visualmente e deixa espaços e molduras pré-configurados para futuras imagens gráficas.
    /// </summary>
    public static class BancoSprites
    {
        /// <summary>
        /// Retorna um sprite compacto (4 linhas) ideal para cards de batalha 3v3.
        /// </summary>
        public static string[] ObterSprite(string nomeOuTipo)
        {
            if (string.IsNullOrEmpty(nomeOuTipo))
                return ObterSpritePadrao();

            string chave = nomeOuTipo.ToLowerInvariant();

            if (chave.Contains("optimus") || chave.Contains("sentinela") || chave.Contains("brutamontes") || chave.Contains("marcus") || chave.Contains("tanque") || chave.Contains("armadura"))
            {
                return new string[]
                {
                    @"  ◢████████◣  ",
                    @" █▓▒░[■]  [■]░▒▓█",
                    @" █▓▌ ▐████▌ ▐▒▓█",
                    @"  ◥████████◤  "
                };
            }
            if (chave.Contains("asimov") || chave.Contains("engenheiro") || chave.Contains("elena") || chave.Contains("suporte") || chave.Contains("mecanico"))
            {
                return new string[]
                {
                    @"  ◢▀▀████▀▀◣  ",
                    @" █▓░ (◎)  (◎) ░▓█",
                    @" █▓▌ ◄◈══◈► ▐▒▓█",
                    @"  ◥▄▄████▄▄◤  "
                };
            }
            if (chave.Contains("pasteur") || chave.Contains("biomancer") || chave.Contains("eter-mancer") || chave.Contains("éter-mancer") || chave.Contains("kaelen") || chave.Contains("mancer") || chave.Contains("biologico"))
            {
                return new string[]
                {
                    @"   ◢██████◣   ",
                    @"  █▓░ ◄(◈)► ░▓█ ",
                    @" █▓▌ ⬡ ✦  ✦ ⬡ ▐▓█",
                    @"  ◥██████████◤  "
                };
            }
            if (chave.Contains("chefe") || chave.Contains("titã") || chave.Contains("capitania") || chave.Contains("dreadnought"))
            {
                return new string[]
                {
                    @" ◢████████████◣ ",
                    @"█▓▒ [X] TITÃ [X] ▒▓█",
                    @"█▓▌ ▐████████▌ ▐▒▓█",
                    @" ◥████████████◤ "
                };
            }
            if (chave.Contains("drone") || chave.Contains("patrulha") || chave.Contains("robo") || chave.Contains("mecanico"))
            {
                return new string[]
                {
                    @"  ◢▀▀◄(◎)►▀▀◣  ",
                    @" █▓▒ ▐████▌ ▒▓█ ",
                    @"  ◥██ ▀██▀ ██◤  ",
                    @"    ◥██████◤    "
                };
            }
            if (chave.Contains("ciborgue") || chave.Contains("elite") || chave.Contains("corsario") || chave.Contains("algoz") || chave.Contains("pirata"))
            {
                return new string[]
                {
                    @"  ◢██████████◣  ",
                    @" █▓▒ [!]    [!] ▒▓█",
                    @" █▓▌  ◄█══█►  ▐▒▓█",
                    @"  ◥██████████◤  "
                };
            }

            return ObterSpritePadrao();
        }

        /// <summary>
        /// Retorna um retrato detalhado em arte ASCII/Unicode (8 a 9 linhas) para telas de perfil, descanso e armeria com perspectiva e sombreamento.
        /// </summary>
        public static string[] ObterRetratoGrande(string nomeOuTipo)
        {
            if (string.IsNullOrEmpty(nomeOuTipo))
                return ObterRetratoPadrao();

            string chave = nomeOuTipo.ToLowerInvariant();

            if (chave.Contains("optimus") || chave.Contains("sentinela") || chave.Contains("marcus") || chave.Contains("brutamontes") || chave.Contains("tanque"))
            {
                return new string[]
                {
                    @"   ◢████████████████◣   ",
                    @"  █▓▒░ [OPTIMUS-V4] ░▒▓█  ",
                    @" █▓▒ ◢████████████◣ ▒▓█ ",
                    @" █▓▌█▓  [■]    [■]  ▓█▐▒▓█",
                    @" █▓▌█▓   ▄████▄   ▓█▐▒▓█",
                    @" █▓▌◥██ ▐██████▌ ██◤▐▒▓█",
                    @" █▓▒ ◢██ ▀████▀ ██◣ ▒▓█ ",
                    @"  ◥██████████████████◤  "
                };
            }

            if (chave.Contains("asimov") || chave.Contains("elena") || chave.Contains("engenheiro") || chave.Contains("suporte"))
            {
                return new string[]
                {
                    @"   ◢████████████████◣   ",
                    @"  █▓▒░  [ASIMOV-SYS] ░▒▓█  ",
                    @" █▓▒ ◢▀▀▀▀████▀▀▀▀◣ ▒▓█ ",
                    @" █▓▌█▓  (◎)    (◎)  ▓█▐▒▓█",
                    @" █▓▌█▓    ◄▲▲►    ▓█▐▒▓█",
                    @" █▓▌◥██ ◄[▓▓▓▓]► ██◤▐▒▓█",
                    @" █▓▒ ◢██ ▀████▀ ██◣ ▒▓█ ",
                    @"  ◥██████████████████◤  "
                };
            }

            if (chave.Contains("pasteur") || chave.Contains("biomancer") || chave.Contains("kaelen") || chave.Contains("eter-mancer") || chave.Contains("mancer"))
            {
                return new string[]
                {
                    @"   ◢████████████████◣   ",
                    @"  █▓▒░ [PASTEUR-BIO]░▒▓█  ",
                    @" █▓▒    ◢██████◣    ▒▓█ ",
                    @" █▓▌ █▓ ◄((◈))► ▓█  ▐▒▓█",
                    @" █▓▌  ◥██ ✦  ✦ ██◤  ▐▒▓█",
                    @" █▓▌   ◢██████◣   ▐▒▓█",
                    @" █▓▒  █▓▒░MANA░▒▓█  ▒▓█ ",
                    @"  ◥██████████████████◤  "
                };
            }

            if (chave.Contains("dreadnought") || chave.Contains("chefe") || chave.Contains("capitania"))
            {
                return new string[]
                {
                    @"  ◢██████████████████████◣  ",
                    @" █▓▒░ [DREADNOUGHT TITÃ] ░▒▓█ ",
                    @" █▓▒ ◢████████████████◣ ▒▓█ ",
                    @" █▓▌█▓ [X]  CAPITÂNIA [X] ▓█▐▒▓█",
                    @" █▓▌█▓  ◢██████████◣  ▓█▐▒▓█",
                    @" █▓▌◥██ ▐██████████▌ ██◤▐▒▓█",
                    @" █▓▒ ◢██ ▀████████▀ ██◣ ▒▓█ ",
                    @"  ◥██████████████████████◤  "
                };
            }

            return ObterRetratoPadrao();
        }

        /// <summary>
        /// Retorna uma ilustração ASCII de cenário / planeta / estação / anomalia (6 a 8 linhas).
        /// </summary>
        public static string[] ObterArteCenario(string tipoOuNome)
        {
            string chave = (tipoOuNome ?? "").ToLowerInvariant();

            if (chave.Contains("chefe") || chave.Contains("dreadnought") || chave.Contains("galáxia 6") || chave.Contains("fortaleza"))
            {
                return new string[]
                {
                    @"          /============\          ",
                    @"       <=|  [X]    [X]  |=>       ",
                    @"     <===|   ========   |===>     ",
                    @"   <=====|  [########]  |=====>   ",
                    @"     <===|   ========   |===>     ",
                    @"       <=|  \========/  |=>       ",
                    @"          \____________/          "
                };
            }

            if (chave.Contains("descanso") || chave.Contains("reparo") || chave.Contains("estacao") || chave.Contains("estação") || chave.Contains("armeria"))
            {
                return new string[]
                {
                    @"              |---|               ",
                    @"         ---| | O | |---          ",
                    @"        [===| |===| |===]         ",
                    @"       <====| [PORTO] |====>      ",
                    @"        [===| |===| |===]         ",
                    @"         ---| | O | |---          ",
                    @"              |---|               "
                };
            }

            if (chave.Contains("anomalia") || chave.Contains("fenda") || chave.Contains("eter") || chave.Contains("éter") || chave.Contains("nebulosa"))
            {
                return new string[]
                {
                    @"           .---'''''---.          ",
                    @"        .-'  ~ ~ ~ ~ ~  '-.       ",
                    @"       /  ~   <( @ )>  ~   \      ",
                    @"      | ~ ~    ~~~~~   ~ ~  |     ",
                    @"       \  ~   <( @ )>  ~   /      ",
                    @"        '-.  ~ ~ ~ ~ ~  .-'       ",
                    @"           '---.....---'          "
                };
            }

            if (chave.Contains("comercio") || chave.Contains("bazar") || chave.Contains("mercado") || chave.Contains("sucateiro"))
            {
                return new string[]
                {
                    @"            .--------.            ",
                    @"          /  [ BAZAR ] \          ",
                    @"        /==============\          ",
                    @"       |  [$$]    [$$]  |         ",
                    @"       |   /========\   |         ",
                    @"       |  | SUCATAS  |  |         ",
                    @"        \==============/          "
                };
            }

            if (chave.Contains("inicio") || chave.Contains("base") || chave.Contains("partida"))
            {
                return new string[]
                {
                    @"             .------.             ",
                    @"            /  BASE  \            ",
                    @"          /============\          ",
                    @"         |  [VANGUARDA] |         ",
                    @"         |  ==========  |         ",
                    @"         | CARGA 73: OK |         ",
                    @"          \____________/          "
                };
            }

            // Planeta Padrão / Vulcânico / Mineral
            return new string[]
            {
                @"             .------.             ",
                @"          .-'  ..    '-.          ",
                @"        /   (..)  (..)   \        ",
                @"       |   (....)  ..     |       ",
                @"       |    ..   (....)   |       ",
                @"        \      '....'    /        ",
                @"          '-.        .-'          "
            };
        }

        /// <summary>
        /// Retorna a arte ASCII da Nave Mercenária Vanguarda (7 linhas).
        /// </summary>
        public static string[] ObterArteNaveVanguarda()
        {
            return new string[]
            {
                @"              /¨¨\            ",
                @"            //    \\          ",
                @"           || [==] ||         ",
                @"          //|======|\\        ",
                @"         // |CARGA | \\       ",
                @"        <===\__73__/===>      ",
                @"          [PROPULSORES]       "
            };
        }

        /// <summary>
        /// Retorna um ícone/sprite ASCII de 4 linhas para um determinado tipo de item.
        /// </summary>
        public static string[] ObterArteItem(TipoItem tipo)
        {
            return tipo switch
            {
                TipoItem.CuraVida => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  _[]_  │  ",
                    @"  │ [ +  ] │  ",
                    @"  └────────┘  "
                },
                TipoItem.RecuperaEnergia => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  [||]  │  ",
                    @"  │ [EN++] │  ",
                    @"  └────────┘  "
                },
                TipoItem.RestauraSanidade => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  (..)  │  ",
                    @"  │ <@_@>  │  ",
                    @"  └────────┘  "
                },
                TipoItem.EscudoEmergencial => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  /--\  │  ",
                    @"  │ |DEF+| │  ",
                    @"  └────────┘  "
                },
                TipoItem.GranadaFogo => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  (o)   │  ",
                    @"  │ /FOGO\ │  ",
                    @"  └────────┘  "
                },
                TipoItem.GranadaEletrica => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  (o)   │  ",
                    @"  │ /ELET\ │  ",
                    @"  └────────┘  "
                },
                TipoItem.GranadaAcido => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  (o)   │  ",
                    @"  │ /ACID\ │  ",
                    @"  └────────┘  "
                },
                _ => new string[]
                {
                    @"  ┌────────┐  ",
                    @"  │  [??]  │  ",
                    @"  │  (::)  │  ",
                    @"  └────────┘  "
                }
            };
        }

        /// <summary>
        /// Retorna a arte de encerramento para Vitória ou Derrota.
        /// </summary>
        public static string[] ObterArteFimDeJogo(bool vitoria)
        {
            if (vitoria)
            {
                return new string[]
                {
                    @"         ___________         ",
                    @"        '._==_==_=_.'        ",
                    @"        .-\:      /-.        ",
                    @"       | (|:.     |) |       ",
                    @"        '-|:.     |-'        ",
                    @"          '---------'          ",
                    @"     [ VITÓRIA ESTELAR ]     "
                };
            }
            else
            {
                return new string[]
                {
                    @"            .---.            ",
                    @"           /     \           ",
                    @"          | () () |          ",
                    @"           \  ^  /           ",
                    @"            |||||            ",
                    @"            |||||            ",
                    @"         .---'''---.         ",
                    @"        /  COLAPSO  \        ",
                    @"       |  SINAL SOS  |       ",
                    @"        \___________/        ",
                    @"     [ MISSÃO FRACASSADA ]   "
                };
            }
        }

        /// <summary>
        /// Gera uma moldura/placeholder visual de espaço reservado para imagens futuras ou representações gráficas.
        /// </summary>
        public static string[] ObterPlaceholderImagem(string rotulo, int largura = 34, int altura = 7)
        {
            if (largura < 10) largura = 10;
            if (altura < 3) altura = 3;

            string[] linhas = new string[altura];
            linhas[0] = "┌" + new string('─', largura - 2) + "┐";
            linhas[altura - 1] = "└" + new string('─', largura - 2) + "┘";

            int linhaCentral = altura / 2;
            string textoRotulo = $"[ FOTO: {rotulo} ]";
            if (textoRotulo.Length > largura - 4)
            {
                textoRotulo = textoRotulo.Substring(0, largura - 4);
            }

            for (int i = 1; i < altura - 1; i++)
            {
                if (i == linhaCentral)
                {
                    int padEsq = (largura - 2 - textoRotulo.Length) / 2;
                    int padDir = largura - 2 - textoRotulo.Length - padEsq;
                    linhas[i] = "│" + new string(' ', padEsq) + textoRotulo + new string(' ', padDir) + "│";
                }
                else if (i == 1 || i == altura - 2)
                {
                    string interior = " . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . ";
                    if (interior.Length > largura - 2) interior = interior.Substring(0, largura - 2);
                    else interior = interior.PadRight(largura - 2);
                    linhas[i] = "│" + interior + "│";
                }
                else
                {
                    linhas[i] = "│" + new string(' ', largura - 2) + "│";
                }
            }

            return linhas;
        }

        private static string[] ObterSpritePadrao()
        {
            return new string[]
            {
                @"   ◢██████◣   ",
                @"  █▓▒ [..] ▒▓█  ",
                @"  █▓▌ ▐██▌ ▐▓█  ",
                @"   ◥██████◤   "
            };
        }

        private static string[] ObterRetratoPadrao()
        {
            return new string[]
            {
                @"   ◢████████████████◣   ",
                @"  █▓▒░  [COMBATENTE]░▒▓█  ",
                @" █▓▒    ◢██████◣    ▒▓█ ",
                @" █▓▌ █▓   [..]   ▓█  ▐▒▓█",
                @" █▓▌ █▓   ▄██▄   ▓█  ▐▒▓█",
                @" █▓▌ ◥██ ▐████▌ ██◤  ▐▒▓█",
                @" █▓▒  ◢██ ▀██▀ ██◣  ▒▓█ ",
                @"  ◥██████████████████◤  "
            };
        }
    }
}
