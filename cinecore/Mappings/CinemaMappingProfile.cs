using AutoMapper;
using cinecore.modelos;
using cinecore.DTOs.Cinema;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Cinema
    /// </summary>
    public class CinemaMappingProfile : Profile
    {
        public CinemaMappingProfile()
        {
            // Mapeamento de Cinema para CinemaDto
            CreateMap<Cinema, CinemaDto>();

            // Mapeamento de CriarCinemaDto para Cinema
            CreateMap<CriarCinemaDto, Cinema>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarCinemaDto para Cinema
            CreateMap<AtualizarCinemaDto, Cinema>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
