namespace cinema.exceptions
{
    /// <summary>
    /// Exceção lançada quando dados inválidos são fornecidos (null, vazio, formato inválido)
    /// </summary>
    public class DadosInvalidosException : Exception
    {
        public DadosInvalidosException(string mensagem) : base(mensagem) { }

        public DadosInvalidosException(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}
