using cinecore.Models;
using cinecore.Exceptions;

namespace cinecore.Services
{
    public class AutenticacaoServico
    {
        private readonly UsuarioServico UsuarioServico;

        public AutenticacaoServico(UsuarioServico UsuarioServico)
        {
            this.UsuarioServico = UsuarioServico;
        }

        // Método para autenticar
        public Usuario? Autenticar(string email, string senha)
        {
            // Valida se email e senha foram informados
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new DadosInvalidosExcecao("Email ou senha não informados.");
            }

            // Busca o usuário através do UsuarioServico
            var usuario = UsuarioServico.ObterUsuarioPorCredenciais(email, senha);
            
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoExcecao("Email ou senha inválidos.");
            }

            return usuario;
        }

        // Método para validar credenciais
        public void ValidarCredenciais(string email, string senha)
        {
            // Valida se email e senha foram informados
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new DadosInvalidosExcecao("Email ou senha não informados.");
            }

            // Verifica através do UsuarioServico
            var usuario = UsuarioServico.ObterUsuarioPorCredenciais(email, senha);
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoExcecao("Email ou senha inválidos.");
            }
        }
    }   
}
