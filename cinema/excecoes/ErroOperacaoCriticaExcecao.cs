namespace cinema.excecoes
{
    /// <summary>
    /// Exceção lançada para erros críticos que indicam estado inconsistente do sistema
    /// </summary>
    // Renomeado de ErroOperacaoCriticaException para ErroOperacaoCriticaExcecao.
    public class ErroOperacaoCriticaExcecao : Exception
    {
        public ErroOperacaoCriticaExcecao(string mensagem) : base(mensagem) { }

        public ErroOperacaoCriticaExcecao(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}





