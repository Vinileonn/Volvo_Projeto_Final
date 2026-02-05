namespace cinema.exceptions
{
    /// <summary>
    /// Exceção lançada quando uma operação não é permitida (duplicidade, restrição de negócio, etc)
    /// </summary>
    public class OperacaoNaoPermitidaException : Exception
    {
        public OperacaoNaoPermitidaException(string mensagem) : base(mensagem) { }

        public OperacaoNaoPermitidaException(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}
