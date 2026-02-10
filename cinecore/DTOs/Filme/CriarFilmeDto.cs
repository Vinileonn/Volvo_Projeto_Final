using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.DTOs.Filme
{
    /// <summary>
    /// DTO para criação de um novo Filme
    /// </summary>
    public class CriarFilmeDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 255 caracteres")]
        public required string Titulo { get; set; }

        [Required(ErrorMessage = "A duração é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que zero")]
        public int Duracao { get; set; }

        [Required(ErrorMessage = "O gênero é obrigatório")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O gênero deve ter entre 1 e 100 caracteres")]
        public required string Genero { get; set; }

        [Required(ErrorMessage = "O ano de lançamento é obrigatório")]
        public DateTime AnoLancamento { get; set; }

        public bool Eh3D { get; set; } = false;

        [Required(ErrorMessage = "A classificação indicativa é obrigatória")]
        public ClassificacaoIndicativa Classificacao { get; set; } = ClassificacaoIndicativa.Livre;
    }
}
