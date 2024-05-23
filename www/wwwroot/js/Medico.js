const urlAPI = "https://localhost:44309/";

$(document).ready(function () {


    //Início da validação dos campos no frontend
  

    //Fim da validação dos campos no frontend



























    let contatos = [];

    // Carregar tipos de contato
    $.ajax({
        url: urlAPI + "api/Medico/tipoContato",
        method: "GET",
        success: function (data) {
            const selectTipoContato = $("#selectTipoContato");
            data.forEach(tipo => {
                const option = `<option value="${tipo.id}">${tipo.nome}</option>`;
                selectTipoContato.append(option);
            });
        },
        error: function (jqXHR) {
            alert("Erro ao carregar os tipos de contato.");
        }
    });

    $("#btnlimpar").click(function () {
        $("#txtnomeCompleto").val('');
        $("#txtdataNascimento").val('');
        $("#txtcrm").val('');
        $("#txtid").val('0');
        contatos = [];
        atualizarTabelaContatos();
    });

    $("#btnAdicionarContato").click(function () {
        const tipoContato = $("#selectTipoContato option:selected").text();
        const valorContato = $("#txtValorContato").val();
        const idTipoContato = $("#selectTipoContato").val();

        if (tipoContato && valorContato) {
            contatos.push({ idTipoContato: idTipoContato, tipo: tipoContato, valor: valorContato });
            atualizarTabelaContatos();
            $("#txtValorContato").val('');
        } else {
            alert("Por favor, selecione um tipo de contato e insira um valor.");
        }
    });

    function atualizarTabelaContatos() {
        const tabela = $("#contatoTable tbody");
        tabela.empty();

        contatos.forEach((contato, index) => {
            const linha = `<tr>
                <td>${contato.tipo}</td>
                <td>${contato.valor}</td>
                <td><button type="button" class="btn btn-danger" data-index="${index}">Excluir</button></td>
            </tr>`;
            tabela.append(linha);
        });

        $(".btn-danger").click(function () {
            const index = $(this).data("index");
            contatos.splice(index, 1);
            atualizarTabelaContatos();
        });
    }

    $("#btnsalvar").click(function () {
        const obj = {
            id: $("#txtid").val(),
            nomeCompleto: $("#txtnomeCompleto").val(),
            dataNascimento: $("#txtdataNascimento").val(),
            crm: $("#txtcrm").val(),
            contato: contatos
        };
        console.log(JSON.stringify(obj));

        $.ajax({
            type: $("#txtid").val() == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Medico",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {
                console.log(jsonResult);
                $("#txtnomeCompleto").val('');
                $("#txtdataNascimento").val('');
                $("#txtcrm").val('');
                $("#txtid").val('0');
                contatos = [];
                atualizarTabelaContatos();
                alert("Dados Salvos com sucesso!");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status === 409) {
                    var errorMessage = JSON.parse(jqXHR.responseText);
                    alert("Erro ao salvar os dados: " + errorMessage.message);
                } else {
                    if (jqXHR.status === 400) {
                        var mensagem = "";
                        $(jqXHR.responseJSON.errors).each(function (index, elemento) {
                            mensagem = mensagem + elemento.errorMessage + "\n";
                        });
                        alert(mensagem);
                    } else {
                        alert("Erro ao salvar os dados: " + errorThrown);
                    }
                }
            }
        });
    });
});
