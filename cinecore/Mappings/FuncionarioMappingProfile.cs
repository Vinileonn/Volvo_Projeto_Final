using AutoMapper;
using cinecore.Models;
using cinecore.DTOs.Funcionario;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para Funcionario
    /// </summary>
    public class FuncionarioMappingProfile : Profile
    {
        public FuncionarioMappingProfile()
        {
            // Mapeamento de Funcionario para FuncionarioDto
            CreateMap<Funcionario, FuncionarioDto>()
                .ForMember(dest => dest.CinemaId, opt => opt.MapFrom(src => src.Cinema != null ? src.Cinema.Id : 0));

            // Mapeamento de CriarFuncionarioDto para Funcionario
            CreateMap<CriarFuncionarioDto, Funcionario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cinema, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarFuncionarioDto para Funcionario
            CreateMap<AtualizarFuncionarioDto, Funcionario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cinema, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
