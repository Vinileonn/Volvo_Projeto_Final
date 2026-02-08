namespace cineflow.excecoes
{
    /// <summary>
    /// Exceção lançada quando dados inválidos são fornecidos (null, vazio, formato inválido)
    /// </summary>
    public class DadosInvalidosExcecao : Exception
    {
        public DadosInvalidosExcecao(string mensagem) : base(mensagem) { }

        public DadosInvalidosExcecao(string mensagem, Exception innerException) 
            : base(mensagem, innerException) { }
    }
}





