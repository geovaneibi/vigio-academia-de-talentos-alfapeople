var Vigio = window.Vigio || {};
window.Vigio = Vigio;

Vigio.Inspecao = (function () {
    var FIELD_STATUS = "vigio_resultado";
    var FIELD_DATA_INSPECAO = "vigio_datadainspecao";

    function onLoad(executionContext) {
        var formContext = executionContext.getFormContext();

        var statusAttr = formContext.getAttribute(FIELD_STATUS);
        if (statusAttr) {
            statusAttr.addOnChange(onStatusChange);
        }

        aplicarRegra(formContext);
    }

    function onStatusChange(executionContext) {
        var formContext = executionContext.getFormContext();
        aplicarRegra(formContext);
    }

    function aplicarRegra(formContext) {
        var statusAttr = formContext.getAttribute(FIELD_STATUS);
        if (!statusAttr) return;

        var dataInspAttr = formContext.getAttribute(FIELD_DATA_INSPECAO);
        if (!dataInspAttr) return;

        var statusValue = statusAttr.getValue(); 

        if (statusValue === 1) {
            if (!dataInspAttr.getValue()) {
                dataInspAttr.setValue(new Date());
            }
            dataInspAttr.setRequiredLevel("required");
        } else {
            dataInspAttr.setValue(null);
            dataInspAttr.setRequiredLevel("none");
        }
    }

    return {
        onLoad: onLoad,
        onStatusChange: onStatusChange
    };
})();
