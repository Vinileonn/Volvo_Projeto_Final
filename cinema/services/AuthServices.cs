using cinema.models.UsuarioModel;
using cinema.exceptions;

namespace cinema.services
{
    public class AuthServices
    {
        private readonly UsuarioServices usuarioServices;

        public AuthServices(UsuarioServices usuarioServices)
        {
            this.usuarioServices = usuarioServices;
        }

        // Método para realizar login
        public Usuario? Login(string email, string senha)
        {
            // Valida se email e senha foram informados
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new DadosInvalidosException("Email ou senha não informados.");
            }

            // Busca o usuário através do UsuarioServices
            var usuario = usuarioServices.ObterUsuarioPorCredenciais(email, senha);
            
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoException("Email ou senha inválidos.");
            }

            return usuario;
        }

        // Método para validar credenciais
        public void ValidarCredenciais(string email, string senha)
        {
            // Valida se email e senha foram informados
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new DadosInvalidosException("Email ou senha não informados.");
            }

            // Verifica através do UsuarioServices
            var usuario = usuarioServices.ObterUsuarioPorCredenciais(email, senha);
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoException("Email ou senha inválidos.");
            }
        }
    }   
}
