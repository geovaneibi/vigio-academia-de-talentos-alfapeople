function finalizarOcorrencias(primaryControl) {
    console.log("Função finalizarOcorrencias iniciada.");

    const selectedItems = primaryControl.getGrid().getSelectedRows();
    console.log("Quantidade de registros selecionados:", selectedItems.getLength());

    if (selectedItems.getLength() === 0) {
        console.warn("Nenhum registro selecionado.");
        Xrm.Navigation.openAlertDialog({ text: "Selecione ao menos uma ocorrência para finalizar." });
        return;
    }

    const promises = [];

    selectedItems.forEach(function (row, index) {
        console.log("Processando registro índice:", index);

        const id = row.getData().entity.getId().replace("{", "").replace("}", "");
        console.log("ID capturado:", id);

        const updateData = {
            vigio_statusdaocorrencia: 4 
        };
        console.log("Objeto updateData:", updateData);

        const promise = Xrm.WebApi.updateRecord("vigio_ocorrencia", id, updateData)
            .then(function (result) {
                console.log("Update OK para ID:", id, "Resultado:", result);
            })
            .catch(function (error) {
                console.error("Erro no update para ID:", id, "Detalhes:", error);
            });

        promises.push(promise);
    });

    Promise.all(promises)
        .then(function () {
            console.log("Promise.all concluído com sucesso.");
            Xrm.Navigation.openAlertDialog({ text: "Ocorrência(s) finalizada(s) com sucesso." });
        })
        .catch(function (error) {
            console.error("Erro geral no Promise.all:", error);
            Xrm.Navigation.openErrorDialog({ message: "Erro ao finalizar ocorrência: " + error.message });
        });
}
