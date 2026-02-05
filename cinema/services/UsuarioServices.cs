using cinema.models.UsuarioModel;
using cinema.exceptions;

namespace cinema.services
{
    public class UsuarioServices
    {
        private List<Usuario> usuarios;

        public UsuarioServices()
        {
            usuarios = new List<Usuario>();
            // Cria um admin por padrão
            usuarios.Add(new Admin(1, "Administrador"));
        }

        // Método interno para obter usuário por email e senha
        internal Usuario? ObterUsuarioPorCredenciais(string email, string senha)
        {
            return usuarios.FirstOrDefault(u =>
                u != null &&
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Senha == senha);
        }

        // Create - 
        // Método para registrar um novo cliente
        public void RegistrarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new DadosInvalidosException("Cliente nulo.");
            }

            // Valida campos obrigatórios juntos
            var camposVazios = new List<string>();
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                camposVazios.Add("nome");
            if (string.IsNullOrWhiteSpace(cliente.Email))
                camposVazios.Add("email");
            if (string.IsNullOrWhiteSpace(cliente.Senha))
                camposVazios.Add("senha");
            if (string.IsNullOrWhiteSpace(cliente.CPF))
                camposVazios.Add("CPF");
            
            if (camposVazios.Count > 0)
            {
                throw new DadosInvalidosException($"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }

            // Verifica duplicidade de email e CPF
            if (usuarios.Any(u => u != null && u.Email.Equals(cliente.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new OperacaoNaoPermitidaException($"Email '{cliente.Email}' já cadastrado.");
            }

            if (usuarios.OfType<Cliente>().Any(c => c.CPF == cliente.CPF))
            {
                throw new OperacaoNaoPermitidaException($"CPF '{cliente.CPF}' já cadastrado.");
            }

            // Gera um novo ID para o cliente
            cliente.Id = usuarios.Count > 0 ? usuarios.Max(u => u.Id) + 1 : 1;
            
            // Define a data de cadastro como a data atual
            cliente.DataCadastro = DateTime.Now;

            // Adiciona o cliente à lista
            usuarios.Add(cliente);
        }

        // Read -
        // Método para obter usuário por ID
        public Usuario ObterUsuario(int id)
        {
            var usuario = usuarios.FirstOrDefault(u => u != null && u.Id == id);
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoException($"Usuário com ID {id} não encontrado.");
            }
            return usuario;
        }

        // Método para obter usuário por email
        public Usuario ObterUsuarioPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DadosInvalidosException("Email não informado.");
            }

            var usuario = usuarios.FirstOrDefault(u =>
                u != null && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (usuario == null)
            {
                throw new RecursoNaoEncontradoException($"Usuário com email {email} não encontrado.");
            }
            return usuario;
        }

        // Método para listar todos os usuários
        public List<Usuario> ListarUsuarios()
        {
            return new List<Usuario>(usuarios);
        }

        // Método para listar apenas clientes
        public List<Cliente> ListarClientes()
        {
            return usuarios.OfType<Cliente>().ToList();
        }

        // UPDATE -
        // Método para atualizar dados do usuário
        public void AtualizarUsuario(int id, string? nome = null, string? email = null, 
                                      string? telefone = null, string? endereco = null)
        {
            var usuario = ObterUsuario(id);

            // Atualiza apenas os campos fornecidos
            if (!string.IsNullOrWhiteSpace(nome))
                usuario.Nome = nome;

            if (!string.IsNullOrWhiteSpace(email))
            {
                // Verifica se o novo email já está em uso por outro usuário
                if (usuarios.Any(u => u != null && u.Id != id &&
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new OperacaoNaoPermitidaException($"Email '{email}' já está em uso por outro usuário.");
                }
                usuario.Email = email;
            }

            // Atualiza dados específicos de Cliente
            if (usuario is Cliente cliente)
            {
                if (!string.IsNullOrWhiteSpace(telefone))
                    cliente.Telefone = telefone;

                if (!string.IsNullOrWhiteSpace(endereco))
                    cliente.Endereco = endereco;
            }
        }

        // DELETE -
        // Método para deletar usuário
        public void DeletarUsuario(int id)
        {
            var usuario = ObterUsuario(id);
            
            // Não permite deletar admin
            if (usuario is Admin)
            {
                throw new OperacaoNaoPermitidaException("Não é permitido deletar o usuário Administrador.");
            }

            usuarios.Remove(usuario);
        }

        // Método para alterar senha
        public void AlterarSenha(int id, string senhaAtual, string senhaNova)
        {
            var usuario = ObterUsuario(id);
            
            // Valida senhas juntas
            if (usuario.Senha != senhaAtual || string.IsNullOrWhiteSpace(senhaNova))
            {
                var motivos = new List<string>();
                if (usuario.Senha != senhaAtual)
                    motivos.Add("senha atual incorreta");
                if (string.IsNullOrWhiteSpace(senhaNova))
                    motivos.Add("nova senha inválida");
                    
                throw new DadosInvalidosException($"Erro ao alterar senha: {string.Join(" e ", motivos)}.");
            }

            usuario.Senha = senhaNova;
            }
    }
}
