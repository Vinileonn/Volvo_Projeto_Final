using cinema.modelos.UsuarioModelo;
using cinema.servicos;
using cinema.excecoes;

namespace cinema.controladores
{
    // Renomeado de AuthController para AutenticacaoControlador.
    public class AutenticacaoControlador
    {
        private readonly AutenticacaoServico AutenticacaoServico;
        private readonly UsuarioServico UsuarioServico;

        public AutenticacaoControlador(AutenticacaoServico AutenticacaoServico, UsuarioServico UsuarioServico)
        {
            this.AutenticacaoServico = AutenticacaoServico;
            this.UsuarioServico = UsuarioServico;
        }

        public (Usuario? usuario, string mensagem) Autenticar(string email, string senha)
        {
            try
            {
                var usuario = AutenticacaoServico.Autenticar(email, senha);
                return (usuario, "Autenticacao realizada com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, $"Erro: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao autenticar.");
            }
        }

        public (bool sucesso, string mensagem) RegistrarCliente(Cliente cliente)
        {
            try
            {
                UsuarioServico.RegistrarCliente(cliente);
                return (true, "Cadastro realizado com sucesso.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar cliente.");
            }
        }

        public (bool sucesso, string mensagem) AlterarSenha(int usuarioId, string senhaAtual, string senhaNova)
        {
            try
            {
                UsuarioServico.AlterarSenha(usuarioId, senhaAtual, senhaNova);
                return (true, "Senha alterada com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, $"Usuário não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao alterar senha.");
            }
        }
    }
}





