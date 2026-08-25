using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;

namespace ProjetoFinalPOO.Model
{
    #region DTOs de Configuração

    public class ConfiguracaoJogoDto
    {
        public int LarguraConsole { get; set; } = 110;
        public int AlturaConsole { get; set; } = 32;
        public int CreditosIniciais { get; set; } = 250;
        public int IntegridadeNaveInicial { get; set; } = 100;
        public int ExpPorVitoriaBase { get; set; } = 30;
        public int ExpPorGalaxiaMultiplicador { get; set; } = 15;
        public int CapacidadeMaximaInventario { get; set; } = 10;
        public int NivelMaximoTripulacao { get; set; } = 10;
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Raridade { get; set; }
        public string TipoItem { get; set; }
        public int ValorEfeito { get; set; }
        public string Descricao { get; set; }
    }

    public class HabilidadeDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public int Modificador { get; set; }
        public string Afinidade { get; set; }
        public int PoderBase { get; set; }
        public int Moeda { get; set; }
        public int PoderAdicionalMoeda { get; set; }
    }

    public class HabilidadesRootDto
    {
        public List<HabilidadeDto> Sentinela { get; set; } = new List<HabilidadeDto>();
        public List<HabilidadeDto> Engenheiro { get; set; } = new List<HabilidadeDto>();
        public List<HabilidadeDto> Biomancer { get; set; } = new List<HabilidadeDto>();
        public Dictionary<string, List<HabilidadeDto>> Inimigos { get; set; } = new Dictionary<string, List<HabilidadeDto>>(StringComparer.OrdinalIgnoreCase);
    }

    public class InimigoModeloDto
    {
        public string Nome { get; set; }
        public int VidaBase { get; set; }
        public int EscalaVidaPorNivel { get; set; }
        public int DefesaBase { get; set; }
        public int Agilidade { get; set; }
        public string Afinidade { get; set; }
        public string ChaveHabilidade { get; set; }
    }

    public class InimigosRootDto
    {
        public Dictionary<string, InimigoModeloDto> Chefe { get; set; } = new Dictionary<string, InimigoModeloDto>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, InimigoModeloDto> Elite { get; set; } = new Dictionary<string, InimigoModeloDto>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, InimigoModeloDto> Comum { get; set; } = new Dictionary<string, InimigoModeloDto>(StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    /// <summary>
    /// Gerenciador central de carga de dados e configurações do jogo a partir de arquivos JSON externos.
    /// Implementa Graceful Fallback para garantir resiliência total mesmo se arquivos estiverem ausentes.
    /// </summary>
    public static class CarregadorDadosJogo
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        private static ConfiguracaoJogoDto _configuracao;
        private static List<Item> _itensCarregados;
        private static Dictionary<string, List<Habilidade>> _habilidadesClasses;
        private static Dictionary<string, List<HabilidadeDto>> _habilidadesInimigosDto;
        private static InimigosRootDto _inimigosRoot;
        private static bool _dadosCarregados = false;

        public static ConfiguracaoJogoDto Configuracao
        {
            get
            {
                GarantirDadosCarregados();
                return _configuracao ??= new ConfiguracaoJogoDto();
            }
        }

        public static void CarregarTodosDados()
        {
            if (_dadosCarregados) return;

            string pastaDados = ResolverPastaDados();

            CarregarConfiguracoes(pastaDados);
            CarregarItens(pastaDados);
            CarregarHabilidades(pastaDados);
            CarregarInimigos(pastaDados);

            _dadosCarregados = true;
        }

        private static void GarantirDadosCarregados()
        {
            if (!_dadosCarregados)
            {
                CarregarTodosDados();
            }
        }

        private static string ResolverPastaDados()
        {
            // Tenta caminhos comuns de execução e desenvolvimento
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] caminhosPossiveis = new[]
            {
                Path.Combine(baseDir, "Dados"),
                Path.Combine(baseDir, "..", "..", "..", "Dados"),
                Path.Combine(Directory.GetCurrentDirectory(), "Dados"),
                Path.Combine(Directory.GetCurrentDirectory(), "ProjetoFinalPOO", "Dados"),
                Path.Combine(baseDir, "..", "..", "..", "ProjetoFinalPOO", "Dados")
            };

            foreach (var caminho in caminhosPossiveis)
            {
                if (Directory.Exists(caminho))
                {
                    return Path.GetFullPath(caminho);
                }
            }

            return Path.Combine(baseDir, "Dados");
        }

        #region Carregamento Específico com Fallbacks

        private static void CarregarConfiguracoes(string pastaDados)
        {
            string caminhoArquivo = Path.Combine(pastaDados, "configuracoes.json");
            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    _configuracao = JsonSerializer.Deserialize<ConfiguracaoJogoDto>(json, _jsonOptions);
                }
                catch
                {
                    _configuracao = new ConfiguracaoJogoDto();
                }
            }
            else
            {
                _configuracao = new ConfiguracaoJogoDto();
            }
        }

        private static void CarregarItens(string pastaDados)
        {
            string caminhoArquivo = Path.Combine(pastaDados, "itens.json");
            _itensCarregados = new List<Item>();

            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    var dtos = JsonSerializer.Deserialize<List<ItemDto>>(json, _jsonOptions);
                    if (dtos != null && dtos.Count > 0)
                    {
                        foreach (var dto in dtos)
                        {
                            if (Enum.TryParse<Raridade>(dto.Raridade, true, out var raridade) &&
                                Enum.TryParse<TipoItem>(dto.TipoItem, true, out var tipoItem))
                            {
                                _itensCarregados.Add(new Item(dto.Id, dto.Nome, raridade, tipoItem, dto.ValorEfeito, dto.Descricao));
                            }
                        }
                    }
                }
                catch
                {
                    // Fallback
                }
            }

            if (_itensCarregados.Count == 0)
            {
                CarregarItensFallback();
            }
        }

        private static void CarregarItensFallback()
        {
            _itensCarregados = new List<Item>
            {
                new Item(1, "Medikit Nanomédico", Raridade.Comum, TipoItem.CuraVida, 35, "Restaura instantaneamente +35 de Vida em um aliado."),
                new Item(2, "Injeção de Éter Estabilizadora", Raridade.Comum, TipoItem.RestauraRecursoEspecial, 25, "Estabiliza e restaura +25 de Recursos Especiais."),
                new Item(3, "Bateria de Plasma Concentrada", Raridade.Comum, TipoItem.RecuperaEnergia, 30, "Recarrega +30 pontos de Energia tática."),
                new Item(4, "Emissor de Escudo Cinético", Raridade.Incomum, TipoItem.EscudoEmergencial, 40, "Gera uma barreira protetora temporária de +40 de Escudo."),
                new Item(5, "Granada de Fogo Termobárica", Raridade.Incomum, TipoItem.GranadaFogo, 30, "Explosão térmica que causa 30 de dano de FOGO."),
                new Item(6, "Granada de Pulso Eletrostático", Raridade.Incomum, TipoItem.GranadaEletrica, 30, "Sobrecarga que causa 30 de dano ELÉTRICO."),
                new Item(7, "Frasco de Ácido Quântico", Raridade.Incomum, TipoItem.GranadaAcido, 30, "Substância corrosiva que causa 30 de dano de ÁCIDO."),
                new Item(8, "Célula Regenerativa Militar", Raridade.Raro, TipoItem.CuraVida, 70, "Nanomáquinas de alta potência que curam +70 de Vida."),
                new Item(9, "Catalisador de Vácuo Arcano", Raridade.Epico, TipoItem.RestauraRecursoEspecial, 50, "Artefato que restaura +50 de Recursos Especiais."),
                new Item(10, "Bateria de Antimatéria Experimental", Raridade.Lendario, TipoItem.RecuperaEnergia, 75, "Restaura +75 de Energia para habilidades de alto calibre.")
            };
        }

        private static void CarregarHabilidades(string pastaDados)
        {
            string caminhoArquivo = Path.Combine(pastaDados, "habilidades.json");
            _habilidadesClasses = new Dictionary<string, List<Habilidade>>(StringComparer.OrdinalIgnoreCase);
            _habilidadesInimigosDto = new Dictionary<string, List<HabilidadeDto>>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    var root = JsonSerializer.Deserialize<HabilidadesRootDto>(json, _jsonOptions);
                    if (root != null)
                    {
                        if (root.Sentinela != null)
                            _habilidadesClasses["Sentinela"] = ConverterHabilidades(root.Sentinela);
                        if (root.Engenheiro != null)
                            _habilidadesClasses["Engenheiro"] = ConverterHabilidades(root.Engenheiro);
                        if (root.Biomancer != null)
                            _habilidadesClasses["Biomancer"] = ConverterHabilidades(root.Biomancer);

                        if (root.Inimigos != null)
                        {
                            foreach (var par in root.Inimigos)
                            {
                                _habilidadesInimigosDto[par.Key] = par.Value;
                            }
                        }
                    }
                }
                catch
                {
                    // Fallback
                }
            }

            if (_habilidadesClasses.Count == 0)
            {
                CarregarHabilidadesFallback();
            }
        }

        private static List<Habilidade> ConverterHabilidades(List<HabilidadeDto> dtos)
        {
            var lista = new List<Habilidade>();
            foreach (var dto in dtos)
            {
                Enum.TryParse<CategoriaHabilidade>(dto.Categoria, true, out var cat);
                Enum.TryParse<AfinidadeAtaque>(dto.Afinidade, true, out var afinidade);
                lista.Add(new Habilidade(dto.Nome, dto.Descricao, cat, dto.Modificador, afinidade, dto.PoderBase, dto.Moeda, dto.PoderAdicionalMoeda));
            }
            return lista;
        }

        private static void CarregarHabilidadesFallback()
        {
            _habilidadesClasses["Sentinela"] = new List<Habilidade>
            {
                new Habilidade("Pancada Brutal", "Golpe pesado com manopla aquecida que causa impacto ígneo.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Fogo, 10, 1, 4),
                new Habilidade("Postura Inabalável", "Ergue o escudo de liga pesada preparando-se para o embate.", CategoriaHabilidade.Basica, 3, AfinidadeAtaque.Eletrico, 8, 2, 3),
                new Habilidade("Lança-Granada Ácida", "Dispara cápsula corrosiva altamente eficaz contra armaduras pesadas.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Acido, 11, 2, 4),
                new Habilidade("Impacto Demolidor", "Avanço devastador com propulsores térmicos que rompe as defesas.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Fogo, 16, 2, 5),
                new Habilidade("Barreira de Titânio", "Projeta um campo de contenção que reduz dano e reforça a blindagem.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Acido, 14, 2, 4),
                new Habilidade("Fúria do Colosso", "Ataque sísmico avassalador que esmaga o alvo com fluido corrosivo concentrado.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Acido, 22, 3, 6)
            };

            _habilidadesClasses["Engenheiro"] = new List<Habilidade>
            {
                new Habilidade("Disparo Eletrostático", "Feixe elétrico concentrado que causa curto-circuito em robôs.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Eletrico, 9, 2, 4),
                new Habilidade("Nanorobôs de Reparo", "Enxame de nanorobôs que recupera integridade e calibra sistemas.", CategoriaHabilidade.Basica, 4, AfinidadeAtaque.Eletrico, 10, 1, 5),
                new Habilidade("Solda de Plasma", "Tocha térmica de plasma que queima componentes biológicos.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Fogo, 10, 1, 5),
                new Habilidade("Sobrecarga de Circuito", "Pulso eletromagnético em cadeia que sobrecarrega sistemas inimigos.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Eletrico, 15, 3, 4),
                new Habilidade("Campo de Força Defensivo", "Escudo de partículas eletrostáticas que anula projéteis.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Eletrico, 12, 2, 5),
                new Habilidade("Canhão Orbital de Partículas", "Sinaliza um bombardeio de feixe de partículas em alta voltagem.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Eletrico, 24, 3, 7)
            };

            _habilidadesClasses["Biomancer"] = new List<Habilidade>
            {
                new Habilidade("Dardo Celular Cáustico", "Projétil de enzimas biológicas condensadas e altamente corrosivas.", CategoriaHabilidade.Basica, 5, AfinidadeAtaque.Acido, 9, 2, 4),
                new Habilidade("Pulso Bio-Psíquico", "Canaliza bio-frequências para estabilizar o foco e a mana.", CategoriaHabilidade.Basica, 4, AfinidadeAtaque.Eletrico, 8, 2, 3),
                new Habilidade("Combustão Orgânica", "Reação exotérmica celular de alta voltagem que calcina alvos.", CategoriaHabilidade.Basica, 6, AfinidadeAtaque.Fogo, 11, 2, 4),
                new Habilidade("Nuvem Bio-Corrosiva", "Bruma tóxica de esporos cáusticos que derrete blindagens e tecidos.", CategoriaHabilidade.Avancada, 10, AfinidadeAtaque.Acido, 16, 3, 5),
                new Habilidade("Distorção Bio-Cinética", "Membrana bio-elástica que dissipa o impacto de ataques no embate.", CategoriaHabilidade.Avancada, 8, AfinidadeAtaque.Fogo, 14, 2, 5),
                new Habilidade("Vórtice Celular Absoluto", "Abre uma hiper-reação de biomassa devoradora que calcina em chamas.", CategoriaHabilidade.Especialista, 15, AfinidadeAtaque.Fogo, 25, 3, 7)
            };
        }

        private static void CarregarInimigos(string pastaDados)
        {
            string caminhoArquivo = Path.Combine(pastaDados, "inimigos.json");
            _inimigosRoot = null;

            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    _inimigosRoot = JsonSerializer.Deserialize<InimigosRootDto>(json, _jsonOptions);
                }
                catch
                {
                    // Fallback
                }
            }

            _inimigosRoot ??= new InimigosRootDto();
        }

        #endregion

        #region Métodos de Acesso Público

        public static List<Item> ObterItens()
        {
            GarantirDadosCarregados();
            return _itensCarregados.Select(i => new Item(i.Id, i.Nome, i.Raridade, i.Tipo, i.ValorEfeito, i.Descricao)).ToList();
        }

        public static List<Item> ObterInventarioInicial()
        {
            GarantirDadosCarregados();
            var todos = ObterItens();
            return todos.Take(3).ToList();
        }

        public static List<Habilidade> ObterHabilidadesClasse(string nomeClasse)
        {
            GarantirDadosCarregados();
            if (_habilidadesClasses.TryGetValue(nomeClasse, out var habilidades))
            {
                return habilidades.Select(h => new Habilidade(h.Nome, h.Descricao, h.Categoria, h.Modificador, h.Afinidade, h.PoderBase, h.Moeda, h.PoderAdicionalMoeda)).ToList();
            }

            return new List<Habilidade>();
        }

        public static List<Habilidade> ObterHabilidadesInimigo(string chaveTipo, int nivel = 1)
        {
            GarantirDadosCarregados();
            string chave = chaveTipo.ToLowerInvariant();
            int bonusPoder = Math.Max(0, (nivel - 1) * 2);
            int bonusMoeda = Math.Max(0, (nivel - 1) / 2);

            string categoriaChave = "Pirata";
            if (chave.Contains("chefe") || chave.Contains("titã") || chave.Contains("capitania"))
            {
                categoriaChave = "Chefe";
            }
            else if (chave.Contains("drone") || chave.Contains("mecanico"))
            {
                categoriaChave = "Drone";
            }

            if (_habilidadesInimigosDto.TryGetValue(categoriaChave, out var dtos) && dtos.Count > 0)
            {
                var lista = new List<Habilidade>();
                foreach (var dto in dtos)
                {
                    Enum.TryParse<CategoriaHabilidade>(dto.Categoria, true, out var cat);
                    Enum.TryParse<AfinidadeAtaque>(dto.Afinidade, true, out var afinidade);
                    lista.Add(new Habilidade(dto.Nome, dto.Descricao, cat, dto.Modificador, afinidade, dto.PoderBase + bonusPoder, dto.Moeda, dto.PoderAdicionalMoeda + bonusMoeda));
                }
                return lista;
            }

            // Fallback para Inimigo
            if (categoriaChave == "Chefe")
            {
                return new List<Habilidade>
                {
                    new Habilidade("Canhão de Fusão Titânica", "Disparo pesado da bateria principal do Dreadnought.", CategoriaHabilidade.Especialista, 0, AfinidadeAtaque.Fogo, 18 + bonusPoder, 3, 4 + bonusMoeda),
                    new Habilidade("Pulso Eletromagnético", "Onda de choque que sobrecarrega escudos.", CategoriaHabilidade.Avancada, 0, AfinidadeAtaque.Eletrico, 15 + bonusPoder, 2, 4 + bonusMoeda),
                    new Habilidade("Salva de Torpedos de Ácido", "Torpedos com ogivas de biocorrosão.", CategoriaHabilidade.Avancada, 0, AfinidadeAtaque.Acido, 16 + bonusPoder, 2, 4 + bonusMoeda)
                };
            }
            else if (categoriaChave == "Drone")
            {
                return new List<Habilidade>
                {
                    new Habilidade("Metralhadora Eletrostática", "Rajada de disparos de alta voltagem.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 8 + bonusPoder, 2, 3 + bonusMoeda),
                    new Habilidade("Laser de Corte Térmico", "Feixe concentrado para corte e penetração.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Fogo, 9 + bonusPoder, 1, 4 + bonusMoeda),
                    new Habilidade("Descarga Elétrica de Choque", "Descarga de alta amperagem.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 7 + bonusPoder, 2, 2 + bonusMoeda)
                };
            }

            return new List<Habilidade>
            {
                new Habilidade("Ataque Ácido", "Disparo de fluido corrosivo.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Acido, 8 + bonusPoder, 2, 3 + bonusMoeda),
                new Habilidade("Lâmina Térmica", "Golpe cortante com espada superaquecida.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Fogo, 9 + bonusPoder, 2, 3 + bonusMoeda),
                new Habilidade("Ataque Elétrico", "Pulso de choque elétrico direto.", CategoriaHabilidade.Basica, 0, AfinidadeAtaque.Eletrico, 8 + bonusPoder, 1, 4 + bonusMoeda)
            };
        }

        #endregion
    }
}
