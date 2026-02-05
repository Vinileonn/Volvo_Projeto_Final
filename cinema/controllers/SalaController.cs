using cinema.models;
using cinema.services;
using cinema.exceptions;

namespace cinema.controllers
{
    public class SalaController
    {
        private readonly SalaService salaService;

        public SalaController(SalaService salaService)
        {
            this.salaService = salaService;
        }

        // CREATE - cadastrar nova sala e gerar assentos
        public (bool sucesso, string mensagem) CriarSala(Sala sala)
        {
            try
            {
                salaService.CriarSala(sala);
                return (true, $"Sala '{sala.Nome}' cadastrada com sucesso com {sala.Assentos.Count} assentos.");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao cadastrar sala.");
            }
        }

        // READ - obter sala por id
        public (Sala? sala, string mensagem) ObterSala(int id)
        {
            try
            {
                var sala = salaService.ObterSala(id);
                return (sala, "Sala obtida com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (null, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (null, "Erro inesperado ao obter sala.");
            }
        }

        // READ - listar todas as salas
        public (List<Sala> salas, string mensagem) ListarSalas()
        {
            try
            {
                var salas = salaService.ListarSalas();
                if (salas.Count == 0)
                {
                    return (salas, "Nenhuma sala cadastrada.");
                }
                return (salas, $"{salas.Count} sala(s) encontrada(s).");
            }
            catch (Exception)
            {
                return (new List<Sala>(), "Erro inesperado ao listar salas.");
            }
        }

        // UPDATE - atualizar dados da sala
        public (bool sucesso, string mensagem) AtualizarSala(int id, string? nome = null, int? capacidade = null)
        {
            try
            {
                salaService.AtualizarSala(id, nome, capacidade);
                return (true, "Sala atualizada com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (OperacaoNaoPermitidaException ex)
            {
                return (false, $"Operação não permitida: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao atualizar sala.");
            }
        }

        // UTIL - gerar assentos para sala
        public (bool sucesso, string mensagem) GerarAssentosParaSala(int id)
        {
            try
            {
                salaService.GerarAssentosParaSala(id);
                return (true, "Assentos gerados com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (DadosInvalidosException ex)
            {
                return (false, $"Dados inválidos: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao gerar assentos.");
            }
        }

        // DELETE - remover sala
        public (bool sucesso, string mensagem) DeletarSala(int id)
        {
            try
            {
                salaService.DeletarSala(id);
                return (true, "Sala deletada com sucesso.");
            }
            catch (RecursoNaoEncontradoException ex)
            {
                return (false, $"Recurso não encontrado: {ex.Message}");
            }
            catch (Exception)
            {
                return (false, "Erro inesperado ao deletar sala.");
            }
        }
    }
}
