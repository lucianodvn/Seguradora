using Seguradora.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Domain.Sinistros
{
    public sealed class Sinistro
    {
        public Guid Id { get; private set; }
        public Guid ApoliceId { get; private set; }
        public string Descricao { get; private set; }
        public decimal ValorEstimado { get; private set; }
        public DateTime DataOcorrencia { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public StatusSinistro Status { get; private set; }

        private Sinistro(Guid apoliceId, string descricao, decimal valorEstimado,
            DateTime dataOcorrencia, DateTime dataAbertura)
        {
            Id = Guid.NewGuid();
            ApoliceId = apoliceId;
            Descricao = descricao;
            ValorEstimado = valorEstimado;
            DataOcorrencia = dataOcorrencia;
            DataAbertura = dataAbertura;
            Status = StatusSinistro.Aberto;
        }

        internal static Sinistro Abrir(Guid apoliceId, string descricao, decimal valorEstimado,
            DateTime dataOcorrencia, DateTime dataAbertura)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new DomainException("A descrição do sinistro é obrigatória.");
            if (valorEstimado <= 0)
                throw new DomainException("O valor estimado deve ser maior que zero.");

            return new Sinistro(apoliceId, descricao.Trim(), valorEstimado, dataOcorrencia, dataAbertura);
        }
        public string? MotivoNegativa { get; private set; }

        public void IniciarAnalise()
        {
            GarantirStatus(StatusSinistro.Aberto, "Só é possível iniciar a análise de um sinistro aberto.");
            Status = StatusSinistro.EmAnalise;
        }

        public void Aprovar()
        {
            GarantirStatus(StatusSinistro.EmAnalise, "Só é possível aprovar um sinistro em análise.");
            Status = StatusSinistro.Aprovado;
        }

        public void Negar(string motivo)
        {
            GarantirStatus(StatusSinistro.EmAnalise, "Só é possível negar um sinistro em análise.");
            if (string.IsNullOrWhiteSpace(motivo))
                throw new DomainException("O motivo da negativa é obrigatório.");

            MotivoNegativa = motivo.Trim();
            Status = StatusSinistro.Negado;
        }

        private void GarantirStatus(StatusSinistro esperado, string mensagem)
        {
            if (Status != esperado)
                throw new DomainException(mensagem);
        }

    }
}
