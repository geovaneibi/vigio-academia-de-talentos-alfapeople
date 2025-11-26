using System;
using Microsoft.Xrm.Sdk;
using vigio;

namespace Vigio.Plugins
{
    public class OcorrenciaPostUpdate : IPlugin
    {
        private readonly OcorrenciaBusinessLogic _businessLogic;

        public OcorrenciaPostUpdate()
        {
        }

        internal OcorrenciaPostUpdate(OcorrenciaBusinessLogic businessLogic)
        {
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = null;
            ITracingService tracing = null;

            try
            {
                context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var service = factory.CreateOrganizationService(context.UserId);

                if (!context.InputParameters.Contains("Target") ||
                    !(context.InputParameters["Target"] is Entity target))
                    return;

                if (!string.Equals(context.PrimaryEntityName, OcorrenciaBusinessLogic.ENTITY_NAME_OCORRENCIA, StringComparison.OrdinalIgnoreCase))
                    return;

                if (!string.Equals(context.MessageName, "Create", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase))
                    return;

                if (context.Depth > 1)
                    return;

                var dao = new OcorrenciaDao(service);
                var businessLogic = _businessLogic ?? new OcorrenciaBusinessLogic(dao);
                businessLogic.DefinirPrazoSla(target, tracing);
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracing?.Trace($"Erro de execução controlada no plugin Vigio.OcorrenciaPostUpdate: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                var msg =
                    $"ERRO CRÍTICO NO PLUGIN Vigio.OcorrenciaPostUpdate\n" +
                    $"Mensagem: {ex.Message}\n" +
                    $"StackTrace: {ex.StackTrace}\n" +
                    $"Entidade: {context?.PrimaryEntityName}\n" +
                    $"Mensagem CRM: {context?.MessageName}\n" +
                    $"Depth: {context?.Depth}\n" +
                    $"UserId: {context?.UserId}";

                tracing?.Trace(msg);
                throw new InvalidPluginExecutionException("Ocorreu um erro inesperado ao processar a Ocorrência. Verifique o log para mais detalhes.");
            }
        }
    }
}
