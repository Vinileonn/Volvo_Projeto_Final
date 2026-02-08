using AutoMapper;
using cinecore.modelos;
using cinecore.DTOs.Sessao;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Sessão
    /// </summary>
    public class SessaoMappingProfile : Profile
    {
        public SessaoMappingProfile()
        {
            // Mapeamento de Sessao para SessaoDto
            CreateMap<Sessao, SessaoDto>()
                .ForMember(dest => dest.FilmeId, opt => opt.MapFrom(src => src.Filme!.Id))
                .ForMember(dest => dest.SalaId, opt => opt.MapFrom(src => src.Sala!.Id));

            // Mapeamento de CriarSessaoDto para Sessao
            CreateMap<CriarSessaoDto, Sessao>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.Filme, opt => opt.Ignore())
                .ForMember(dest => dest.Sala, opt => opt.Ignore())
                .ForMember(dest => dest.Ingressos, opt => opt.Ignore());

            // Mapeamento de AtualizarSessaoDto para Sessao
            CreateMap<AtualizarSessaoDto, Sessao>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.Filme, opt => opt.Ignore())
                .ForMember(dest => dest.Sala, opt => opt.Ignore())
                .ForMember(dest => dest.Ingressos, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
