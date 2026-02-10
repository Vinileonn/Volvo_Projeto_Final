using cinecore.Enums;

namespace cinecore.DTOs.Funcionario
{
    /// <summary>
    /// DTO para retorno de dados de Funcionario
    /// </summary>
    public class FuncionarioDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public CargoFuncionario Cargo { get; set; }
        public int CinemaId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
