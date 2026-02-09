namespace cinecore.DTOs.Usuario
{
    /// <summary>
    /// DTO para retornar dados de usuário
    /// </summary>
    public class UsuarioDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public string TipoUsuario { get; set; } = "Usuario";
    }

    /// <summary>
    /// DTO para cliente (usuário com pontos de fidelidade)
    /// </summary>
    public class ClienteDto : UsuarioDto
    {
        public required string CPF { get; set; }
        public int PontosFidelidade { get; set; }
    }

    /// <summary>
    /// DTO para administrador
    /// </summary>
    public class AdministradorDto : UsuarioDto
    {
    }
}
