using Microsoft.Xrm.Sdk;
using System;

namespace vigio
{
    public class OcorrenciaBusinessLogic
    {
        public const string ENTITY_NAME_OCORRENCIA = "vigio_ocorrencia";
        private const string FIELD_SLA_HORAS = "vigio_slahoras";
        private const string FIELD_PRAZO_SLA = "vigio_prazosla";
        private const string TIMEZONE_BRASILIA_ID = "E. South America Standard Time";

        private readonly OcorrenciaDao _dao;

        public OcorrenciaBusinessLogic(OcorrenciaDao dao)
        {
            _dao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        public void DefinirPrazoSla(Entity ocorrencia, ITracingService tracing)
        {
            if (ocorrencia == null)
                throw new ArgumentNullException(nameof(ocorrencia));
            if (!string.Equals(ocorrencia.LogicalName, ENTITY_NAME_OCORRENCIA, StringComparison.OrdinalIgnoreCase))
            {
                tracing?.Trace("DefinirPrazoSla: entidade diferente de {0} ({1})", ENTITY_NAME_OCORRENCIA, ocorrencia.LogicalName);
                return;
            }

            int slaHoras = 1;
            if (ocorrencia.Contains(FIELD_SLA_HORAS) && ocorrencia[FIELD_SLA_HORAS] != null)
            {
                if (ocorrencia[FIELD_SLA_HORAS] is int sla)
                {
                    slaHoras = sla;
                }
                else if (int.TryParse(ocorrencia[FIELD_SLA_HORAS].ToString(), out var slaParsed))
                {
                    slaHoras = slaParsed;
                }
            }
            else
            {
                ocorrencia[FIELD_SLA_HORAS] = slaHoras;
            }

            if (ocorrencia.Contains(FIELD_PRAZO_SLA) && ocorrencia[FIELD_PRAZO_SLA] != null)
            {
                tracing?.Trace("DefinirPrazoSla: Prazo SLA já preenchido, nenhum cálculo realizado.");
                return;
            }

            var agoraUtc = DateTime.UtcNow;
            var tzBrasil = TimeZoneInfo.FindSystemTimeZoneById(TIMEZONE_BRASILIA_ID);
            var agoraBrasil = TimeZoneInfo.ConvertTimeFromUtc(agoraUtc, tzBrasil);
            var prazoSla = agoraBrasil.AddHours(slaHoras);

            ocorrencia[FIELD_PRAZO_SLA] = prazoSla;
            _dao.Atualizar(ocorrencia);

            tracing?.Trace("DefinirPrazoSla: SLA = {0}h | Agora(BR) = {1:o} | PrazoSLA = {2:o}", slaHoras, agoraBrasil, prazoSla);
        }
    }
}
