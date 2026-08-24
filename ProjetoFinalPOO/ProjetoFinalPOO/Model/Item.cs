using System;
using System.Collections.Generic;
using ProjetoFinalPOO.Combatentes;
using ProjetoFinalPOO.Enums;
using ProjetoFinalPOO.Model.Excecoes;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Representa um item utilizável em combate ou encontrado na exploração espacial.
    /// Implementa a interface IAplicavelEfeito.
    /// </summary>
    public class Item : IAplicavelEfeito
    {
        private int _id;
        private string _nome;
        private string _descricao;
        private Raridade _raridade;
        private TipoItem _tipo;
        private int _valorEfeito;

        public int Id { get => _id; set => _id = value; }
        public string Nome { get => _nome; set => _nome = value; }
        public string Descricao { get => _descricao; set => _descricao = value; }
        public Raridade Raridade { get => _raridade; set => _raridade = value; }
        public TipoItem Tipo { get => _tipo; set => _tipo = value; }
        public int ValorEfeito { get => _valorEfeito; set => _valorEfeito = Math.Max(0, value); }

        public Item(int id, string nome, Raridade raridade, TipoItem tipo, int valorEfeito, string descricao)
        {
            Id = id;
            Nome = nome;
            Raridade = raridade;
            Tipo = tipo;
            ValorEfeito = valorEfeito;
            Descricao = descricao;
        }

        public string AplicarEfeito(Combatente usuario, Combatente alvo, List<string> logs = null)
        {
            return Usar(usuario, alvo, logs);
        }

        public string Usar(Combatente usuario, Combatente alvo, List<string> logs = null)
        {
            if (usuario == null) throw new RegraJogoException("Usuário do item não pode ser nulo.", "USUARIO_NULO");
            if (alvo == null) throw new RegraJogoException("Alvo do item não pode ser nulo.", "ALVO_NULO");

            logs ??= new List<string>();
            string resultado = "";

            switch (Tipo)
            {
                case TipoItem.CuraVida:
                    alvo.Defender(); // Aciona recuperação tática nativa
                    resultado = $"{usuario.Nome} usou {Nome} em {alvo.Nome}, recuperando integridade e postura!";
                    break;

                case TipoItem.RecuperaEnergia:
                case TipoItem.RestauraSanidade:
                    alvo.AlterarModificador(ValorEfeito);
                    resultado = $"{usuario.Nome} usou {Nome} em {alvo.Nome}, potencializando seu recurso especial em +{ValorEfeito}!";
                    break;

                case TipoItem.EscudoEmergencial:
                    alvo.Defender();
                    resultado = $"{usuario.Nome} ativou {Nome} em {alvo.Nome}, assumindo postura defensiva reforçada!";
                    break;

                case TipoItem.GranadaFogo:
                    alvo.ReceberDano(ValorEfeito);
                    resultado = $"{usuario.Nome} arremessou {Nome} contra {alvo.Nome} causando {ValorEfeito} de dano de FOGO!";
                    break;

                case TipoItem.GranadaEletrica:
                    alvo.ReceberDano(ValorEfeito);
                    resultado = $"{usuario.Nome} arremessou {Nome} contra {alvo.Nome} causando {ValorEfeito} de dano ELÉTRICO!";
                    break;

                case TipoItem.GranadaAcido:
                    alvo.ReceberDano(ValorEfeito);
                    resultado = $"{usuario.Nome} arremessou {Nome} contra {alvo.Nome} causando {ValorEfeito} de dano de ÁCIDO!";
                    break;
            }

            logs.Add(resultado);
            return resultado;
        }

        public override string ToString()
        {
            return $"[{Raridade.ToString().ToUpper()}] {Nome} - {Descricao}";
        }
    }
}
