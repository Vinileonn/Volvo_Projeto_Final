namespace cineflow.excecoes
{
    /// <summary>
    /// Exceção lançada quando um recurso (usuário, filme, assento, etc) não é encontrado
    /// </summary>
    // Renomeado de RecursoNaoEncontradoException para RecursoNaoEncontradoExcecao.
    public class RecursoNaoEncontradoExcecao : Exception
    {
        public RecursoNaoEncontradoExcecao(string mensagem) : base(mensagem) { }

        public RecursoNaoEncontradoExcecao(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}





