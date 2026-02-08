using AutoMapper;
using cinecore.modelos;
using cinecore.DTOs;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Filme
    /// </summary>
    public class FilmeMappingProfile : Profile
    {
        public FilmeMappingProfile()
        {
            // Mapeamento de Filme para FilmeDto
            CreateMap<Filme, FilmeDto>();

            // Mapeamento de CriarFilmeDto para Filme
            CreateMap<CriarFilmeDto, Filme>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarFilmeDto para Filme
            CreateMap<AtualizarFilmeDto, Filme>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
