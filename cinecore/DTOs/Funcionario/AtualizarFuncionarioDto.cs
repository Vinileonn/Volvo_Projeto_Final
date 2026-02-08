using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.Funcionario
{
    /// <summary>
    /// DTO para atualizacao de um Funcionario existente
    /// </summary>
    public class AtualizarFuncionarioDto
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public string? Nome { get; set; }

        public CargoFuncionario? Cargo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do cinema deve ser maior que zero")]
        public int? CinemaId { get; set; }
    }
}
