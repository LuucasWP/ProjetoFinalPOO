# 🌌 Mercenários do Éter: A Escolta da Carga 73

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![Arquitetura em Camadas](https://img.shields.io/badge/Arquitetura-Clean%20Layers-blue?style=for-the-badge)](DOCUMENTACAO_SISTEMAS_JOGO.md)
[![Status](https://img.shields.io/badge/Status-Concluído-brightgreen?style=for-the-badge)](#)

> **Mercenários do Éter** é um RPG tático espacial baseado em turnos desenvolvido em **C# / .NET 10** para terminal console. O jogo combina a progressão estratégica de mapa em grafo (*Slay the Spire*) com o sistema de **Embates e Moedas (*Limbus Company*)**, onde você comanda uma tripulação mercenária encarregada de escoltar a misteriosa **Carga 73** através de 6 galáxias hostis controladas pela temida Frota Sindical.

---

## 📑 Sumário

- [Lore & História](#-lore--história)
- [Principais Mecânicas de Jogo](#-principais-mecânicas-de-jogo)
  - [Mapa Estelar & Exploração (Grafo 2D)](#1-mapa-estelar--exploração-grafo-2d)
  - [Combate Tático & Embates (Limbus Company)](#2-combate-tático--embates-limbus-company)
  - [Matriz Elemental de Afinidades](#3-matriz-elemental-de-afinidades)
- [Classes da Tripulação & Recursos Especiais](#-classes-da-tripulação--recursos-especiais)
- [Armeria, Baralhos e Itens Táticos](#-armeria-baralhos-e-itens-táticos)
- [Arquitetura & Configuração Externa (JSON)](#-arquitetura--configuração-externa-json)
- [Como Executar o Jogo](#-como-executar-o-jogo)
- [Controles do Jogo](#-controles-do-jogo)
- [Notas Originais de Brainstorming](#-notas-originais-de-brainstorming)

---

## 📜 Lore & História

No limiar do setor estelar Éter-Helios, uma aliança de corporações autoritárias — o *Sindicato Estelar* — impõe bloqueio total sobre as rotas de dobra espacial. A bordo da fragata mercenária **Vanguarda**, um trio de especialistas aceita a missão mais perigosa de suas vidas: escoltar uma cápsula criogênica contendo um indivíduo de valor incalculável, catalogado apenas como **Carga 73**, até o portal de evacuação na Galáxia 6.

Para alcançar a liberdade, a tripulação deverá atravessar setores repletos de patrulhas de drones, corsários cibernéticos, anomalias cósmicas e estações espaciais, culminando no confronto titânico contra a **Capitânia Dreadnought**.

---

## ⚔️ Principais Mecânicas de Jogo

### 1. Mapa Estelar & Exploração (Grafo 2D)
- **Navegação Não-Linear**: O universo é estruturado em um **Grafo Direcionado Ponderado** de 6 galáxias procedurais gerado pelo `MapaRPGBuilder`.
- **Rotas Adjacentes**: O jogador planeja sua rota saltando apenas entre nós planetários imediatamente conectados por corredores de dobra.
- **Tipos de Eventos Planetários**:
  - ⚔️ **Combate Comum / Elite**: Confrontos contra patrulhas sindicais e frotas de assalto.
  - 🛠️ **Estação de Apoio & Armeria**: Zonas neutras para descanso, reparo do casco e troca de habilidades do baralho.
  - 🌌 **Anomalias Cósmicas**: Fendas de éter e ruínas ancestrais para canalização de recursos e espólios raros.
  - 👑 **Confronto Final (Galáxia 6)**: Batalha contra o esquadrão titânico da Capitânia Sindical.

```
 [Início] ──> [G1-P1] ──> [G2-P1] ──> [G3-Estação] ──> ... ──> [G6-Chefe Final]
                 └──> [G1-P2] ──> [G2-P2] ──> [G3-Anomalia] ──/
```

---

### 2. Combate Tático & Embates (*Limbus Company*)
- **Iniciativa por Agilidade**: No início de cada rodada, a ordem de turnos é definida pela agilidade dos combatentes (`Sentinela: 8`, `Biomancer: 15`, `Engenheiro: 20`, `Inimigos: 7 a 23`).
- **Sensores de Intenção**: Os inimigos declaram suas cartas e alvos pretendidos antes do início dos turnos.
- **Resolução de Confrontos**:
  - **Embate (Clash)**: Ocorre quando um aliado ataca um inimigo que ainda possui intenção de ataque ativa. Ambos lançam suas **Moedas de Poder**:
    $$\text{Poder Final} = \text{Poder Base} + \sum (\text{Cara} \times \text{Poder Adicional Moeda})$$
    O combatente com maior poder vence o embate, anula o ataque adversário e desfere seu golpe!
  - **Ataque Sem Oposição (Unilateral)**: Ocorre quando o atacante alveja um oponente que já agiu na rodada ou está indefeso.
- **Câmera 3D & Animação no Terminal**: Animação em ASCII 3D com projeção em perspectiva, efeito *Screen Shake*, rastro de partículas e log dinâmico lateral.

---

### 3. Matriz Elemental de Afinidades

O dano final é influenciado pelo choque entre o elemento do ataque e o tipo de blindagem do defensor:

| Blindagem / Defesa | 🔥 Fogo | ⚡ Elétrico | 🧪 Ácido |
| :--- | :---: | :---: | :---: |
| **Armadurado** (Blindagem Pesada) | `0.5x` (Resistente) | `1.0x` (Neutro) | **`2.0x` (Vulnerável)** |
| **Mecânico** (Robôs e Drones) | `1.0x` (Neutro) | **`2.0x` (Vulnerável)** | `0.5x` (Resistente) |
| **Biológico** (Humanos e Ciborgues) | **`2.0x` (Vulnerável)** | `0.5x` (Resistente) | `1.0x` (Neutro) |

---

## 👥 Classes da Tripulação & Recursos Especiais

A tripulação mercenária da nave Vanguarda é formada por 3 especialistas com arquétipos e recursos distintos:

| Combatente | Classe | Blindagem | Recurso Especial | Mecânica de Moedas |
| :--- | :--- | :--- | :--- | :--- |
| **Optimus** | **Sentinela** | Armadurado | 🩸 **Adrenalina** $(0 \dots 45)$ | Acumula ao sofrer dano. Aumenta a chance de tirar Cara nos embates quando pressionado. |
| **Asimov** | **Engenheiro** | Mecânico | 🔥 **Superaquecimento** $(0 \dots 45)$ | Eleva-se a cada disparo e habilidade tecnológica utilizada, maximizando a taxa de acerto. |
| **Pasteur** | **Biomancer** | Biológico | 🧪 **Mana** $(0 \dots 45)$ | Energia bio-psíquica consumida em feitiços cáusticos e regenerada com descanso e itens. |

> **Progressão de Nível**: A cada vitória, os heróis recebem EXP ($30 + \text{Galáxia} \times 15$). Ao subir de nível (até o **Nível 10**), recebem $+12$ de HP Máximo, cura imediata, $+1$ de Defesa e bônus periódicos de Agilidade.

---

## 🎴 Armeria, Baralhos e Itens Táticos

- **Estrutura do Baralho de Cada Classe**:
  - 🟢 **3 Habilidades Básicas**: Ataques consistentes de baixo custo de modificador.
  - 🔵 **2 Habilidades Avançadas**: Golpes de alto impacto, quebra de guarda e escudos táticos.
  - 🟣 **1 Habilidade Especialista**: Cartas supremas com alto poder base e 3 moedas de multiplicador.
- **Armeria nas Estações**: Nas estações espaciais, o jogador pode trocar e equipar cartas do catálogo para otimizar suas táticas contra os chefes do setor.
- **Itens de Espólio**: Após combates e explorações, escolha 1 entre 3 consumíveis (Nanomédicos, Catalisadores de Éter, Baterias de Plasma e Granadas Elementais).

---

## 🏗️ Arquitetura & Configuração Externa (JSON)

O projeto segue princípios de **Clean Architecture** e padrões de projeto GoF (**Strategy**, **Factory Method**, **Singleton**, **State**, **Builder** e **Observer**):

```
ProjetoFinalPOO/
├── Controladores/              # Orquestração do ciclo de vida do jogo (ControladorJogo)
├── Dados/                      # Configurações externas JSON (Itens, Habilidades, Inimigos, Gerais)
├── Data/                       # Carregador dinâmico de dados com Fallback seguro e Repositórios
├── Domain/                     # Camada de Domínio pura
│   ├── Combate/                # ControladorCombate, Slots, ResultadoEmbate
│   ├── Combatentes/            # Combatente, Sentinela, Engenheiro, Biomancer, Inimigo
│   ├── Encontros/              # IEncontro, EncontroBatalha, EncontroBaseEspacial
│   ├── Excecoes/               # RegraJogoException
│   ├── Habilidades/            # Habilidade, TabelaAfinidades
│   ├── Itens/                  # Item, IAplicavelEfeito
│   └── Mapa/                   # Grafo, Vertice, Aresta, MapaRPGBuilder
├── Enums/                      # Enumerações compartilhadas
├── Media/Audio/                # Biblioteca de Músicas e Sintetizador de Notas
├── Presentation/               # Camada de Interface de Console
│   ├── Engine/                 # RenderizadorUI, BancoSprites, CameraCombate, AnimadorEmbateConsole
│   └── Telas/                  # Telas de UI (Menu, Mapa, Combate, Armeria, etc.)
└── Program.cs                  # Ponto de entrada
```

---

## 🚀 Como Executar o Jogo

### Pré-requisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior instalado.
- Terminal com suporte a ANSI / UTF-8 (Windows Terminal, Alacritty, GNOME Terminal, Kitty, etc.).

### Compilação e Execução

```bash
# Clone o repositório
git clone https://github.com/LuucasWP/ProjetoFinalPOO.git
cd ProjetoFinalPOO/ProjetoFinalPOO/ProjetoFinalPOO

# Restaurar dependências e compilar
dotnet build

# Iniciar o jogo
dotnet run
```

---

## 🎮 Controles do Jogo

| Tecla | Ação |
| :---: | :--- |
| `↑` / `↓` ou `W` / `S` | Navegação nos menus, opções e alvos |
| `←` / `→` ou `A` / `D` | Seleção de cartas e itens em grade horizontal |
| `Enter` / `Espaço` | Confirmar ação / Iniciar embate |
| `1` a `6` | Atalhos numéricos diretos para cartas e opções |
| `Esc` | Retornar / Cancelar seleção |

---

## 💡 Notas Originais de Brainstorming

<details>
<summary><b>Clique para expandir as 28 anotações originais do projeto</b></summary>

```text
1: jogo de exploração espacial
2: mapa estilo slay the spire com tipos de planetas e estações espaciais com efeitos diversos
3: história é um grupo de mercenários carregando uma pessoa como carga para fora de um espaço controlado por um outro grupo
4: boss final poderia ser um time fodão do grupo
5: batalha inspirada em limbus company em que o jogador seleciona as ações que o personagem irá executar e o embate se desenrola baseado em parâmetros e o vencedor irá efetivamente atacar e causar o dano
6: possivelmente adicionar mecânica que aumenta as chances de sucesso em um embate com base em algum atributo (recursos de classe: Adrenalina, Sobreaquecimento e Mana)
7: classes como: brutamontes, engenheiro, *-mancer
8: jogador pode selecionar as ações do personagem através de cartas
9: levels até level 10
10: atributos podem subir: aleatoriamente, conforme a classe, conforme a decisão do jogador *a decidir
11: dificuldade pode ser um enum multiplicador dos parâmetros base -> por enquanto não
12: código em português
13: jogo em português.
14: jogador tem acesso a uma lista com todas as habilidades do jogo e pode decidir trocar o conjunto de habilidades nas áreas de descanso.
15: habilidades iniciais são pré-definidas.
16: iniciar combate e definir as ordens por velocidade.
17: para atacar escolhe a habilidade da lista de cartas equipadas.
18: opção de usar itens.
19: opção de se defender.
20: se o inimigo já atacou, dá um ataque sem embate.
21: preview das ações dos inimigos.
22: nas áreas exploradas terá uma lista de itens e um poderá escolhido para levar com ele.
23: trocamos para afinidades de ataque e de defesa, cada qual com sua vantagem e desvantagem
24: armadura é fraca contra ácido, neutro contra elétrico e forte contra fogo
25: mecânico é fraco contra elétrico, neutro contra fogo e forte contra ácido
26: biológico é fraco contra fogo, neutro contra ácido e forte contra elétrico
27: os personagens terão 6 habilidades, sendo 3 básicas, 2 avançadas e 1 especialista.
28: a party é pré-definida com três personagens, um de cada classe diferente.
```
</details>

---

## 👥 Autores & Créditos

Projeto desenvolvido para a disciplina de **Programação Orientada a Objetos (POO)**.
- **Desenvolvedor**: Gabriel (Deki) & Equipe
- **Diagrama UML**: [diagrama_classes_uml.svg](diagrama_classes_uml.svg)
- **Documentação Técnica dos Sistemas**: [DOCUMENTACAO_SISTEMAS_JOGO.md](DOCUMENTACAO_SISTEMAS_JOGO.md)
