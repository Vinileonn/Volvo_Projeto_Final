using AutoMapper;
using cinecore.Models;
using cinecore.DTOs.Ingresso;

namespace cinecore.Mappings
{
    public class IngressoMappingProfile : Profile
    {
        public IngressoMappingProfile()
        {
            CreateMap<Models.Ingresso, IngressoDto>()
                .ForMember(dest => dest.SessaoId, opt => opt.MapFrom(src => src.Sessao != null ? src.Sessao.Id : 0))
                .ForMember(dest => dest.SessaoDataHorario, opt => opt.MapFrom(src => src.Sessao != null ? src.Sessao.DataHorario : DateTime.MinValue))
                .ForMember(dest => dest.FilmeTitulo, opt => opt.MapFrom(src => src.Sessao != null && src.Sessao.Filme != null ? src.Sessao.Filme.Titulo : null))
                .ForMember(dest => dest.SalaNome, opt => opt.MapFrom(src => src.Sessao != null && src.Sessao.Sala != null ? src.Sessao.Sala.Nome : null))
                .ForMember(dest => dest.TipoSessao, opt => opt.MapFrom(src => src.Sessao != null ? src.Sessao.Tipo : Enums.TipoSessao.Regular))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Id : 0))
                .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Nome : null))
                .ForMember(dest => dest.TipoAssento, opt => opt.MapFrom(src => src.Assento != null ? src.Assento.Tipo : Enums.TipoAssento.Normal))
                .ForMember(dest => dest.AssentoPreferencial, opt => opt.MapFrom(src => src.Assento != null && src.Assento.Preferencial));
        }
    }
}
