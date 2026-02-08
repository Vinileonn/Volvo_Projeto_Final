using AutoMapper;
using cinecore.modelos;
using cinecore.DTOs.ProdutoAlimento;

namespace cinecore.Mappings
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para ProdutoAlimento
    /// </summary>
    public class ProdutoAlimentoMappingProfile : Profile
    {
        public ProdutoAlimentoMappingProfile()
        {
            // Mapeamento de ProdutoAlimento para ProdutoAlimentoDto
            CreateMap<ProdutoAlimento, ProdutoAlimentoDto>();

            // Mapeamento de CriarProdutoAlimentoDto para ProdutoAlimento
            CreateMap<CriarProdutoAlimentoDto, ProdutoAlimento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

            // Mapeamento de AtualizarProdutoAlimentoDto para ProdutoAlimento
            CreateMap<AtualizarProdutoAlimentoDto, ProdutoAlimento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.EstoqueAtual, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
