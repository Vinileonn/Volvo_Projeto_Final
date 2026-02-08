using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.DTOs.Filme
{
    /// <summary>
    /// DTO para atualização de um Filme existente
    /// </summary>
    public class AtualizarFilmeDto
    {
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 255 caracteres")]
        public string? Titulo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que zero")]
        public int? Duracao { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "O gênero deve ter entre 1 e 100 caracteres")]
        public string? Genero { get; set; }

        public DateTime? AnoLancamento { get; set; }

        public bool? Eh3D { get; set; }

        public ClassificacaoIndicativa? Classificacao { get; set; }
    }
}
