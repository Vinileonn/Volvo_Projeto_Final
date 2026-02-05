namespace cinema.exceptions
{
    /// <summary>
    /// Exceção lançada quando um recurso (usuário, filme, assento, etc) não é encontrado
    /// </summary>
    public class RecursoNaoEncontradoException : Exception
    {
        public RecursoNaoEncontradoException(string mensagem) : base(mensagem) { }

        public RecursoNaoEncontradoException(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}
