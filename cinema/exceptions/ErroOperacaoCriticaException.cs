namespace cinema.exceptions
{
    /// <summary>
    /// Exceção lançada para erros críticos que indicam estado inconsistente do sistema
    /// </summary>
    public class ErroOperacaoCriticaException : Exception
    {
        public ErroOperacaoCriticaException(string mensagem) : base(mensagem) { }

        public ErroOperacaoCriticaException(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}
