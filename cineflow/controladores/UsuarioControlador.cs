using cineflow.modelos.UsuarioModelo;
using cineflow.servicos;
using cineflow.excecoes;

namespace cineflow.controladores
{
    public class UsuarioControlador
    {
        private readonly UsuarioServico UsuarioServico;

        public UsuarioControlador(UsuarioServico usuarioServico)
        {
            this.UsuarioServico = usuarioServico;
        }

        // CRIAR - Registrar novo cliente
        public (bool sucesso, string mensagem) RegistrarCliente(Cliente cliente)
        {
            try
            {
                UsuarioServico.RegistrarCliente(cliente);
                return (true, "Cliente registrado com sucesso.");
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
                return (false, "Erro inesperado ao registrar cliente.");
            }
        }

        // CRIAR - Adicionar usuário (genérico)
        public (bool sucesso, string mensagem) AdicionarUsuario(Usuario usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return (false, "Usuário inválido.");
                }

                if (usuario is Cliente cliente)
                {
                    UsuarioServico.RegistrarCliente(cliente);
                }
                else
                {
                    // Para outros tipos de usuário (futuramente)
                    throw new OperacaoNaoPermitidaExcecao("Tipo de usuário não suportado para registro.");
                }

                return (true, "Usuário adicionado com sucesso.");
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
                return (false, "Erro inesperado ao adicionar usuário.");
            }
        }

        // LER - Obter usuário por ID
        public (Usuario? usuario, string mensagem) ObterUsuario(int id)
        {
            try
            {
                var usuario = UsuarioServico.ObterUsuario(id);
                return (usuario, "Usuário encontrado.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, ex.Message);
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao buscar usuário.");
            }
        }

        // LER - Obter usuário por email
        public (Usuario? usuario, string mensagem) ObterUsuarioPorEmail(string email)
        {
            try
            {
                var usuario = UsuarioServico.ObterUsuarioPorEmail(email);
                return (usuario, "Usuário encontrado.");
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (null, ex.Message);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (null, ex.Message);
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao buscar usuário.");
            }
        }

        // LER - Listar todos os usuários
        public (List<Usuario> usuarios, string mensagem) ListarUsuarios()
        {
            try
            {
                var usuarios = UsuarioServico.ListarUsuarios();
                if (usuarios.Count == 0)
                {
                    return (usuarios, "Nenhum usuário cadastrado.");
                }
                return (usuarios, $"{usuarios.Count} usuário(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Usuario>(), "Erro inesperado ao listar usuários.");
            }
        }

        // LER - Listar apenas clientes
        public (List<Cliente> clientes, string mensagem) ListarClientes()
        {
            try
            {
                var clientes = UsuarioServico.ListarClientes();
                if (clientes.Count == 0)
                {
                    return (clientes, "Nenhum cliente cadastrado.");
                }
                return (clientes, $"{clientes.Count} cliente(s) encontrado(s).");
            }
            catch (Exception)
            {
                return (new List<Cliente>(), "Erro inesperado ao listar clientes.");
            }
        }

        // ATUALIZAR - Atualizar dados do usuário
        public (bool sucesso, string mensagem) AtualizarUsuario(int id, string? nome = null, 
            string? email = null, string? telefone = null, string? endereco = null)
        {
            try
            {
                UsuarioServico.AtualizarUsuario(id, nome, email, telefone, endereco);
                return (true, "Usuário atualizado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, ex.Message);
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar usuário.");
            }
        }

        // ATUALIZAR - Alterar senha
        public (bool sucesso, string mensagem) AlterarSenha(int id, string senhaAtual, string senhaNova)
        {
            try
            {
                UsuarioServico.AlterarSenha(id, senhaAtual, senhaNova);
                return (true, "Senha alterada com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, ex.Message);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao alterar senha.");
            }
        }

        // DELETAR - Deletar usuário
        public (bool sucesso, string mensagem) DeletarUsuario(int id)
        {
            try
            {
                UsuarioServico.DeletarUsuario(id);
                return (true, "Usuário deletado com sucesso.");
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return (false, ex.Message);
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar usuário.");
            }
        }
    }
}
