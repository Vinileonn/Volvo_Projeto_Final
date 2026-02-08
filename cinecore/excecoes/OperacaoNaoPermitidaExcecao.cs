namespace cinecore.excecoes
{
    public class OperacaoNaoPermitidaExcecao : Exception
    {
        public OperacaoNaoPermitidaExcecao(string mensagem) : base(mensagem) { }
    }
}
