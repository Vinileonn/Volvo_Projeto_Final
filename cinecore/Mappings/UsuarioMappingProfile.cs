using AutoMapper;
using cinecore.Models;
using cinecore.DTOs.Usuario;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Usuario
    /// </summary>
    public class UsuarioMappingProfile : Profile
    {
        public UsuarioMappingProfile()
        {
            // Mapeamento de Usuario para UsuarioDto
            CreateMap<Usuario, UsuarioDto>();

            // Mapeamento de Cliente para ClienteDto
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.PontosFidelidade, opt => opt.MapFrom(src => src.PontosFidelidade));

            // Mapeamento de Administrador para AdministradorDto
            CreateMap<Administrador, AdministradorDto>();

            // Mapeamento de CriarUsuarioDto para Cliente
            CreateMap<CriarUsuarioDto, Cliente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de CriarAdministradorDto para Administrador
            CreateMap<CriarAdministradorDto, Administrador>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarUsuarioDto para Usuario
            CreateMap<AtualizarUsuarioDto, Usuario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.Senha, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
