using AutoMapper;
using cinecore.Models;
using cinecore.DTOs.Sala;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Sala
    /// </summary>
    public class SalaMappingProfile : Profile
    {
        public SalaMappingProfile()
        {
            // Mapeamento de Sala para SalaDto
            CreateMap<Sala, SalaDto>()
                .ForMember(dest => dest.CinemaId, opt => opt.MapFrom(src => src.Cinema != null ? src.Cinema.Id : 0))
                .ForMember(dest => dest.QuantidadeAssentos, opt => opt.MapFrom(src => src.Assentos.Count));

            // Mapeamento de CriarSalaDto para Sala
            CreateMap<CriarSalaDto, Sala>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cinema, opt => opt.Ignore())
                .ForMember(dest => dest.Assentos, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarSalaDto para Sala
            CreateMap<AtualizarSalaDto, Sala>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cinema, opt => opt.Ignore())
                .ForMember(dest => dest.Assentos, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
