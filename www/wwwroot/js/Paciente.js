const urlAPI = "https://localhost:44309/";


$(document).ready(function () {
    carregarTiposContato();
    carregarTiposEndereco();
    carregarPacientes();

    $("#tabela").on("click", ".excluir", function (e) {
        let codigo = $(e.target).parent().parent().find(".codigo").text();
        excluirPacienteId(codigo);
    });

    $("#tabela").on("click", ".alterar", function (e) {
        let codigo = $(e.target).parent().parent().find(".codigo").text();
        carregarPacienteId(codigo);
    });

    $("#btnsalvar").click(function () {
        //validar
        const obj = {
            id: $("#txtid").val() === "0" ? null : parseInt($("#txtid").val()),
            nomeCompleto: $("#txtnomecompleto").val(),
            peso: parseFloat($("#txtpeso").val()),
            altura: parseFloat($("#txtaltura").val()),
            dataNascimento: $("#txtdatanascimento").val(),
            idade: parseInt($("#txtidade").val()),
            nomeMae: $("#txtnomemae").val(),
            rgNumero: $("#txtrgnumero").val(),
            rgDataEmissao: $("#txtrgdataemissao").val(),
            rgOrgaoExpedidor: $("#txtrgorgaoexpedidor").val(),
            rgUfEmissao: $("#txtrgufemissao").val(),
            cnsNumero: $("#txtcnsnumero").val(),
            cpfNumero: $("#txtcpfnumero").val(),
            nomeConjuge: $("#txtnomeconjuge").val(),
            dataCadastro: new Date().toISOString(),
            contato: [{
                idTipoContato: parseInt($("#txttipocontato").val()),
                valor: $("#txtvalor").val(),
                // Outros campos necessários como id, idPaciente, paciente, etc.
            }],
            endereco: [{
                idTipoEndereco: parseInt($("#txttipoendereco").val()),
                logradouro: $("#txtlogradouro").val(),
                numero: $("#txtnumero").val(),
                complemento: $("#txtcomplemento").val(),
                bairro: $("#txtbairro").val(),
                cidade: $("#txtcidade").val(),
                uf: $("#txtuf").val(),
                cep: $("#txtcep").val(),
                pontoReferencia: $("#txtpontoreferencia").val(),
                // Outros campos necessários como id, idPaciente, paciente, etc.
            }]
        };

        $.ajax({
            type: $("#txtid").val() === "0" ? "POST" : "PUT",
            url: urlAPI + "api/Paciente",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {
                if (jsonResult.statusCode === 500) {
                    alert(jsonResult.value.detail);
                } else {
                    $("#txtid").val('0');
                    limparCampos();
                    alert("Dados Salvos com sucesso!");
                    carregarPacientes();
                }
            },
            failure: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });


    });
});

function limparCampos() {
    $("#txtnomecompleto").val('');
    $("#txtpeso").val('');
    $("#txtaltura").val('');
    $("#txtdatanascimento").val('');
    $("#txtidade").val('');
    $("#txtnomemae").val('');
    $("#txtrgnumero").val('');
    $("#txtrgdataemissao").val('');
    $("#txtorgaoexpedidor").val('');
    $("#txtufemissao").val('');
    $("#txtcnsnumero").val('');
    $("#txtcpfnumero").val('');
    $("#txtnomeconjuge").val('');
    $("#txtcontato").val('');
    $("#txttipocontato").val('');
    $("#txtlogradouro").val('');
    $("#txtnumero").val('');
    $("#txtcomplemento").val('');
    $("#txtbairro").val('');
    $("#txtcidade").val('');
    $("#txtuf").val('');
    $("#txtcep").val('');
    $("#txtpontoreferencia").val('');
}

function carregarPacientes() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente",
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#tabela").empty();
            $.each(jsonResult, function (index, item) {
                var linha = $("#linhaModelo").clone();
                linha.find(".codigo").html(item.id);
                linha.find(".descricao").html(item.nomeCompleto);
                $("#tabela").append(linha);
            });
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function carregarPacienteId(id) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/" + id,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#txtid").val(jsonResult.id);
            $("#txtnomecompleto").val(jsonResult.nomeCompleto);
            $("#txtpeso").val(jsonResult.peso);
            $("#txtaltura").val(jsonResult.altura);
            $("#txtdatanascimento").val(jsonResult.dataNascimento.substring(0, 10));
            $("#txtidade").val(jsonResult.idade);
            $("#txtnomemae").val(jsonResult.nomeMae);
            $("#txtrgnumero").val(jsonResult.rgNumero);
            $("#txtrgdataemissao").val(jsonResult.rgDataEmissao.substring(0, 10));
            $("#txtorgaoexpedidor").val(jsonResult.orgaoExpedidor);
            $("#txtufemissao").val(jsonResult.ufEmissao);
            $("#txtcnsnumero").val(jsonResult.cnsNumero);
            $("#txtcpfnumero").val(jsonResult.cpfNumero);
            $("#txtnomeconjuge").val(jsonResult.nomeConjuge);

            carregarContatosPaciente(jsonResult.id);
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function carregarContatosPaciente(idPaciente) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Contato/PorPaciente/" + idPaciente,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            if (jsonResult.length > 0) {
                $("#txtcontato").val(jsonResult[0].valor);
                $("#txttipocontato").val(jsonResult[0].idTipoContato);
            } else {
                $("#txtcontato").val('');
                $("#txttipocontato").val('');
            }
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function carregarTiposContato() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/api/TipoContato",
        contentType: "application/json",
        success: function (data) {
            $("#txttipocontato").empty();
            data.forEach(function (item) {
                $("#txttipocontato").append(new Option(item.nome, item.id));
            });
        },
        error: function (xhr) {
            console.error("Erro ao carregar tipos de contato: ", xhr.status, xhr.statusText);
        }
    });
}

function carregarTiposEndereco() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/api/TipoEndereco",
        contentType: "application/json",
        success: function (data) {
            $("#txttipoendereco").empty();
            data.forEach(function (item) {
                $("#txttipoendereco").append(new Option(item.nome, item.id));
            });
        },
        error: function (xhr) {
            console.error("Erro ao carregar tipos de endereço: ", xhr.status, xhr.statusText);
        }
    });
}







function excluirPacienteId(id) {
    $.ajax({
        type: "DELETE",
        url: urlAPI + "api/Paciente/" + id,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            if (jsonResult === true) {
                alert('Dados excluídos');
                carregarPacientes();
            } else {
                alert('Dados não foram excluídos');
            }
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}
