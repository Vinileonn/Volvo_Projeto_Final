using AutoMapper;
using cinecore.modelos;
using cinecore.DTOs.AluguelSala;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Aluguel de Sala
    /// </summary>
    public class AluguelSalaMappingProfile : Profile
    {
        public AluguelSalaMappingProfile()
        {
            // Mapeamento de AluguelSala para AluguelSalaDto
            CreateMap<AluguelSala, AluguelSalaDto>()
                .ForMember(dest => dest.SalaId, opt => opt.MapFrom(src => src.Sala != null ? src.Sala.Id : 0))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Id : (int?)null));

            // Mapeamento de CriarAluguelSalaDto para AluguelSala
            CreateMap<CriarAluguelSalaDto, AluguelSala>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Sala, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarAluguelSalaDto para AluguelSala
            CreateMap<AtualizarAluguelSalaDto, AluguelSala>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Sala, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
