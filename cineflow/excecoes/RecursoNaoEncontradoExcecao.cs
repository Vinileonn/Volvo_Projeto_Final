namespace cineflow.excecoes
{
    /// <summary>
    /// Exceção lançada quando um recurso (usuário, filme, assento, etc) não é encontrado
    /// </summary>
    public class RecursoNaoEncontradoExcecao : Exception
    {
        public RecursoNaoEncontradoExcecao(string mensagem) : base(mensagem) { }

        public RecursoNaoEncontradoExcecao(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}

