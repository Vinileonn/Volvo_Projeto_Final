namespace cineflow.excecoes
{
    /// <summary>
    /// Exceção lançada quando uma operação não é permitida (duplicidade, restrição de negócio, etc)
    /// </summary>
    public class OperacaoNaoPermitidaExcecao : Exception
    {
        public OperacaoNaoPermitidaExcecao(string mensagem) : base(mensagem) { }

        public OperacaoNaoPermitidaExcecao(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}





