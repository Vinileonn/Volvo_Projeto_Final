using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.Funcionario
{
    /// <summary>
    /// DTO para criacao de um novo Funcionario
    /// </summary>
    public class CriarFuncionarioDto
    {
        [Required(ErrorMessage = "O nome e obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O cargo e obrigatorio")]
        public CargoFuncionario Cargo { get; set; }

        [Required(ErrorMessage = "O cinema e obrigatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do cinema deve ser maior que zero")]
        public int CinemaId { get; set; }
    }
}
