using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace vigio
{
    public class OcorrenciaDao
    {
        private readonly IOrganizationService _service;
        public const string ENTITY_NAME_OCORRENCIA = "vigio_ocorrencia";

        public OcorrenciaDao(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }


        public void Atualizar(Entity ocorrencia)
        {
            if (ocorrencia == null)
                throw new ArgumentNullException(nameof(ocorrencia));
            if (!string.Equals(ocorrencia.LogicalName, ENTITY_NAME_OCORRENCIA, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Entidade inválida.");
            _service.Update(ocorrencia);
        }

      
    }
}
