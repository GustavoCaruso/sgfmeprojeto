const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
    if ($("#tabela").length > 0) {
        carregarCid();
    } else if ($("#txtid").length > 0) {
        let params = new URLSearchParams(window.location.search);
        let id = params.get('id');
        if (id) {
            visualizar(id);
        }
    }

    $("#btnlimpar").click(function () {
        $("#txtdescricao").val('');
        $("#txtcodigocid").val('');
        $("#txtid").val('0');
        $(".form-control").removeClass('is-invalid');
    });

    $("#tabela").on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).parent().parent().find(".codigo").text();
        window.location.href = "/CidCadastro?id=" + codigo;
    });

    $("#tabela").on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).parent().parent().find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtcodigocid").val().trim()) {
            $("#txtcodigocid").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtdescricao").val().trim()) {
            $("#txtdescricao").addClass('is-invalid');
            isValid = false;
        }
        return isValid;
    }

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                descricao: $("#txtdescricao").val(),
                codigo: $("#txtcodigocid").val(),
            };

            $.ajax({
                type: $("#txtid").val() == "0" ? "POST" : "PUT",
                url: urlAPI + "api/Cid",
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify(obj),
                dataType: "json",
                success: function () {
                    $("#txtdescricao").val('');
                    $("#txtcodigocid").val('');
                    $("#txtid").val('0');
                    alert("Dados Salvos com sucesso!");
                    if ($("#tabela").length > 0) {
                        carregarCid();
                    }
                },
                error: function (jqXHR) {
                    if (jqXHR.status === 400) {
                        var mensagem = "";
                        $(jqXHR.responseJSON.errors).each(function (index, elemento) {
                            mensagem = mensagem + elemento.errorMessage + "\n";
                        });
                        alert(mensagem);
                    } else {
                        alert("Erro ao salvar os dados.");
                    }
                }
            });
        }
    });
});

function carregarCid() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/cid",
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#tabela").empty();
            $.each(jsonResult, function (index, item) {
                var linha = $("#linhaExemplo").clone();
                $(linha).find(".codigo").html(item.id);
                $(linha).find(".descricao").html(item.descricao);
                $(linha).show();
                $("#tabela").append(linha);
            });

            // Inicializar DataTables após carregar os dados
            $('#tabelaCid').DataTable({
                language: {
                    url: '/js/pt-BR.json'
                },
                destroy: true  // Adicionar destroy para garantir que a tabela seja inicializada novamente corretamente
            });
        },
        error: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function excluir(codigo) {
    $.ajax({
        type: "DELETE",
        url: urlAPI + "api/Cid/" + codigo,
        contentType: "application/json;charset=utf-8",
        success: function () {
            alert('Exclusão efetuada!');
            carregarCid();
        },
        error: function (xhr, textStatus, errorThrown) {
            alert("Erro ao excluir o CID: " + errorThrown);
        }
    });
}

function visualizar(codigo) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Cid/" + codigo,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#txtid").val(jsonResult.id);
            $("#txtdescricao").val(jsonResult.descricao);
            $("#txtcodigocid").val(jsonResult.codigo);
        },
        error: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}
