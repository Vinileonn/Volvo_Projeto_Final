using cinecore.dados;
using cinecore.modelos;
using cinecore.excecoes;
using Microsoft.EntityFrameworkCore;

namespace cinecore.servicos
{
    public class UsuarioServico
    {
        private readonly CineFlowContext _context;

        public UsuarioServico(CineFlowContext context)
        {
            _context = context;
        }

        // Método para inicializar com administrador padrão
        public void InicializarAdministrador(string email, string senha)
        {
            // Verifica se já existe um administrador
            if (!_context.Administradores.Any())
            {
                var admin = Administrador.Criar(0, "Administrador", email, senha);
                _context.Administradores.Add(admin);
                _context.SaveChanges();
            }
        }

        // Método interno para obter usuário por email e senha
        internal Usuario? ObterUsuarioPorCredenciais(string email, string senha)
        {
            var emailLower = email.ToLower();
            return _context.Usuarios.FirstOrDefault(u =>
                u != null &&
                u.Email.ToLower().Equals(emailLower) &&
                u.Senha == senha);
        }

        public void AdicionarUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new DadosInvalidosExcecao("Usuário nulo.");
            }
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        // Método para registrar um novo administrador
        public void RegistrarAdministrador(Administrador administrador)
        {
            if (administrador == null)
            {
                throw new DadosInvalidosExcecao("Administrador nulo.");
            }

            var camposVazios = new List<string>();
            if (string.IsNullOrWhiteSpace(administrador.Nome))
                camposVazios.Add("nome");
            if (string.IsNullOrWhiteSpace(administrador.Email))
                camposVazios.Add("email");
            if (string.IsNullOrWhiteSpace(administrador.Senha))
                camposVazios.Add("senha");

            if (camposVazios.Count > 0)
            {
                throw new DadosInvalidosExcecao($"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }

            if (_context.Usuarios.Any(u => u != null && u.Email.ToLower() == administrador.Email.ToLower()))
            {
                throw new OperacaoNaoPermitidaExcecao($"Email '{administrador.Email}' já cadastrado.");
            }

            administrador.DataCadastro = DateTime.Now;
            AdicionarUsuario(administrador);
        }

        // Método para registrar um novo cliente
        public void RegistrarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new DadosInvalidosExcecao("Cliente nulo.");
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
                throw new DadosInvalidosExcecao($"Campos obrigatórios faltando: {string.Join(", ", camposVazios)}.");
            }

            // Verifica duplicidade de email e CPF
            if (_context.Usuarios.Any(u => u != null && u.Email.ToLower() == cliente.Email.ToLower()))
            {
                throw new OperacaoNaoPermitidaExcecao($"Email '{cliente.Email}' já cadastrado.");
            }

            if (_context.Usuarios.OfType<Cliente>().Any(c => c.CPF == cliente.CPF))
            {
                throw new OperacaoNaoPermitidaExcecao($"CPF '{cliente.CPF}' já cadastrado.");
            }

            // Define a data de cadastro como a data atual
            cliente.DataCadastro = DateTime.Now;

            // Adiciona o cliente à lista
            AdicionarUsuario(cliente);
        }

        // Método para obter usuário por ID
        public Usuario ObterUsuario(int id)
        {
            var cliente = _context.Usuarios
                .OfType<Cliente>()
                .Include(c => c.Ingressos)
                .Include(c => c.Pedidos)
                .Include(c => c.Cortesias)
                .FirstOrDefault(c => c.Id == id);

            if (cliente != null)
            {
                return cliente;
            }

            var admin = _context.Usuarios
                .OfType<Administrador>()
                .FirstOrDefault(a => a.Id == id);

            if (admin == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Usuário com ID {id} não encontrado.");
            }

            return admin;
        }

        // Método para obter usuário por email
        public Usuario ObterUsuarioPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DadosInvalidosExcecao("Email não informado.");
            }

            var cliente = _context.Usuarios
                .OfType<Cliente>()
                .Include(c => c.Ingressos)
                .Include(c => c.Pedidos)
                .Include(c => c.Cortesias)
                .FirstOrDefault(c => c.Email.ToLower() == email.ToLower());

            if (cliente != null)
            {
                return cliente;
            }

            var admin = _context.Usuarios
                .OfType<Administrador>()
                .FirstOrDefault(a => a.Email.ToLower() == email.ToLower());

            if (admin == null)
            {
                throw new RecursoNaoEncontradoExcecao($"Usuário com email {email} não encontrado.");
            }

            return admin;
        }

        // Método para listar todos os usuários
        public List<Usuario> ListarUsuarios()
        {
            return _context.Usuarios.ToList();
        }

        // Método para listar apenas clientes
        public List<Cliente> ListarClientes()
        {
            return _context.Usuarios
                .OfType<Cliente>()
                .Include(c => c.Ingressos)
                .Include(c => c.Pedidos)
                .Include(c => c.Cortesias)
                .ToList();
        }

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
                if (_context.Usuarios.Any(u => u != null && u.Id != id &&
                    u.Email.ToLower() == email.ToLower()))
                {
                    throw new OperacaoNaoPermitidaExcecao($"Email '{email}' já está em uso por outro usuário.");
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
            _context.SaveChanges();
        }

        // Método para deletar usuário
        public void DeletarUsuario(int id)
        {
            var usuario = ObterUsuario(id);
            
            // Não permite deletar Administrador
            if (usuario is Administrador)
            {
                throw new OperacaoNaoPermitidaExcecao("Não é permitido deletar o usuário Administrador.");
            }

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
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
                    
                throw new DadosInvalidosExcecao($"Erro ao alterar senha: {string.Join(" e ", motivos)}.");
            }

            usuario.Senha = senhaNova;
            _context.SaveChanges();
        }
    }
}
