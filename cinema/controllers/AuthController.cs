using cinema.models.UsuarioModel;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class AuthController
    {
        private readonly AuthServices authServices;
        private readonly UsuarioServices usuarioServices;

        public AuthController(AuthServices authServices, UsuarioServices usuarioServices)
        {
            this.authServices = authServices;
            this.usuarioServices = usuarioServices;
        }

        public (Usuario? usuario, string mensagem) Login(string email, string senha)
        {
            try
            {
                var usuario = authServices.Login(email, senha);
                return (usuario, "Login realizado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (null, $"Dados inválidos: {ex.Message}");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Erro: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao realizar login.");
            }
        }

        public (bool sucesso, string mensagem) RegistrarCliente(Cliente cliente)
        {
            try
            {
                usuarioServices.RegistrarCliente(cliente);
                return (true, "Cadastro realizado com sucesso.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
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
                usuarioServices.AlterarSenha(usuarioId, senhaAtual, senhaNova);
                return (true, "Senha alterada com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Usuário não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
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
