
const urlAPI = "https://localhost:44309/"
$(document).ready(function () {

    // Essa função vai fazer o botão limpar o formulário
    $("#btnlimpar").click(function () {
        $("#txtid").val('0');
        $("#txtnomeCompleto").val('');
        $("#txtsexo").val('');
        $("#txtrg").val('');
        $("#txtcpf").val('');
        $("#txtcns").val('');
        $("#txtpeso").val('0');
        $("#txtaltura").val('0');
        $("#txtdataNascimento").val('');
        $("#txtnaturalidade").val('');
        $("#txtufNaturalidade").val('');
        $("#txtcorRaca").val('');
        $("#txtestadoCivil").val('');
        $("#txtnomeMae").val('');
    });

    // Essa função vai fazer o botão salvar os dados do formulário
    $("#btnsalvar").click(function () {
        // Aqui é para validar os campos
        const obj = {
            id: $("#txtid").val(),
            nomeCompleto: $("#txtnomeCompleto").val(),
            sexo: $("#txtsexo").val(),
            rg: $("#txtrg").val(),
            cpf: $("#txtcpf").val(),
            cns: $("#txtcns").val(),
            peso: $("#txtpeso").val(),
            altura: $("#txtaltura").val(),
            dataNascimento: $("#txtdataNascimento").val(),
            naturalidade: $("#txtnaturalidade").val(),
            ufNaturalidade: $("#txtufNaturalidade").val(),
            corRaca: $("#txtcorRaca").val(),
            estadoCivil: $("#txtestadoCivil").val(),
            nomeMae: $("#txtnomeMae").val()
        };

        console.log(JSON.stringify(obj))

        $.ajax({
            type: $("#txtid").val() == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Paciente",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {
                console.log(jsonResult);
                $("#txtid").val('0');
                $("#txtnomeCompleto").val('');
                $("#txtsexo").val('');
                $("#txtrg").val('');
                $("#txtcpf").val('');
                $("#txtcns").val('');
                $("#txtpeso").val('0');
                $("#txtaltura").val('0');
                $("#txtdataNascimento").val('');
                $("#txtnaturalidade").val('');
                $("#txtufNaturalidade").val('');
                $("#txtcorRaca").val('');
                $("#txtestadoCivil").val('');
                $("#txtnomeMae").val('');
                alert("Dados Salvos com sucesso!");

            },
            failure: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    });

});
