using System;
using System.Collections.Generic;

namespace ProjetoFinalPOO.Model
{
    /// <summary>
    /// Gerencia a pilha e o ciclo de vida das telas ativas no jogo (State Pattern).
    /// Permite empilhar telas (para menus sobrepostos/modais) e alternar entre cenas completas.
    /// </summary>
    public class GerenciadorTelas
    {
        private static GerenciadorTelas _instancia;
        private readonly Stack<ITela> _pilhaTelas;
        private bool _jogandoAtivo;

        public static GerenciadorTelas Instancia => _instancia ??= new GerenciadorTelas();

        public GerenciadorTelas()
        {
            _pilhaTelas = new Stack<ITela>();
            _jogandoAtivo = true;
        }

        public void AlterarTela(ITela novaTela)
        {
            if (novaTela == null)
                throw new ArgumentNullException(nameof(novaTela));

            if (_pilhaTelas.Count > 0)
            {
                var telaAnterior = _pilhaTelas.Pop();
                telaAnterior.Sair();
            }

            _pilhaTelas.Push(novaTela);
            novaTela.Entrar();
            novaTela.Renderizar();
        }

        public void EmpilharTela(ITela telaSobreposicao)
        {
            if (telaSobreposicao == null)
                throw new ArgumentNullException(nameof(telaSobreposicao));

            _pilhaTelas.Push(telaSobreposicao);
            telaSobreposicao.Entrar();
            telaSobreposicao.Renderizar();
        }

        public void DesempilharTela()
        {
            if (_pilhaTelas.Count > 0)
            {
                var telaRemovida = _pilhaTelas.Pop();
                telaRemovida.Sair();
            }

            if (_pilhaTelas.Count > 0)
            {
                _pilhaTelas.Peek().Renderizar();
            }
        }

        public void RenderizarTelaAtual()
        {
            if (_pilhaTelas.Count > 0)
            {
                _pilhaTelas.Peek().Renderizar();
            }
        }

        public void AtualizarTelaAtual()
        {
            if (_pilhaTelas.Count > 0)
            {
                _pilhaTelas.Peek().Atualizar();
            }
        }

        public void Sair()
        {
            while (_pilhaTelas.Count > 0)
            {
                _pilhaTelas.Pop().Sair();
            }
            _jogandoAtivo = false;
        }

        public bool EstaAtivo() => _jogandoAtivo;

        public ITela GetTelaAtual() => _pilhaTelas.Count > 0 ? _pilhaTelas.Peek() : null;
    }
}
