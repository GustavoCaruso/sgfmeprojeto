const urlAPI = "https://localhost:7034/";

$(document).ready(function () {
    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    if ($("#tabela").length > 0) {
        carregarCids();
    } else if ($("#txtid").length > 0) {
        let params = new URLSearchParams(window.location.search);
        let id = params.get('id');
        if (id) {
            visualizar(id);
        } else {
            let dataAtual = new Date().toISOString().split('T')[0];
            $("#txtdataCadastro").val(dataAtual);
        }
    }

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/CidCadastro?id=" + codigo;
    });

    $(document).on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtcodigo").val().trim()) {
            $("#txtcodigo").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtdescricao").val().trim() || $("#txtdescricao").val().trim().length > 100) {
            $("#txtdescricao").addClass('is-invalid');
            $("#txtdescricao").siblings('.invalid-feedback').text('A descrição é obrigatória e não pode exceder 100 caracteres.');
            isValid = false;
        }
        if (!$("#selectStatus").val().trim() || $("#selectStatus").val() === "0") {
            $("#selectStatus").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectVersaoCid").val().trim() || $("#selectVersaoCid").val() === "0") {
            $("#selectVersaoCid").addClass('is-invalid');
            isValid = false;
        }

        return isValid;
    }

    $(".form-control").on("input change", function () {
        $(this).removeClass('is-invalid');
    });

    function carregarOpcoes(apiEndpoint, selectElement, mensagemPadrao) {
        $.ajax({
            url: urlAPI + apiEndpoint,
            method: "GET",
            success: function (data) {
                selectElement.empty();
                selectElement.append(`<option value="">${mensagemPadrao}</option>`);
                data.forEach(item => {
                    const option = `<option value="${item.id}">${item.nome}</option>`;
                    selectElement.append(option);
                });
            },
            error: function () {
                alert("Erro ao carregar os dados.");
            }
        });
    }

    carregarOpcoes("api/Cid/tipoStatus", $("#selectStatus"), "Selecione um status");
    carregarOpcoes("api/Cid/tipoVersaoCid", $("#selectVersaoCid"), "Selecione uma versão");

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                codigo: $("#txtcodigo").val(),
                descricao: $("#txtdescricao").val(),
                idStatus: $("#selectStatus").val(),
                idVersaoCid: $("#selectVersaoCid").val(),
                dataCadastro: $("#txtdataCadastro").val()
            };

            $.ajax({
                type: obj.id == "0" ? "POST" : "PUT",
                url: urlAPI + "api/Cid" + (obj.id != "0" ? "/" + obj.id : ""),
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify(obj),
                dataType: "json",
                success: function () {
                    limparFormulario();
                    alert("Dados Salvos com sucesso!");

                    if ($("#tabela").length > 0) {
                        carregarCids();
                    }
                },
                error: function (jqXHR, textStatus) {
                    if (jqXHR.status === 400) {
                        var errors = jqXHR.responseJSON.errors;
                        var message = "";
                        for (var key in errors) {
                            if (errors.hasOwnProperty(key)) {
                                errors[key].forEach(function (errorMessage) {
                                    message += errorMessage + "\n";
                                });
                            }
                        }
                        alert(message);
                    } else {
                        alert("Erro ao salvar os dados: " + textStatus);
                    }
                }
            });
        }
    });

    function limparFormulario() {
        $("#txtcodigo").val('');
        $("#txtdescricao").val('');
        $("#selectStatus").val('');
        $("#selectVersaoCid").val('');
        $("#txtid").val('0');
        $("#txtdataCadastro").val(new Date().toISOString().split('T')[0]);
    }

    function carregarCids() {
        $.ajax({
            url: urlAPI + "api/Cid/todosCids",
            method: "GET",
            success: function (data) {
                $("#tabela").empty();
                $.each(data, function (index, item) {
                    var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                    $(linha).find(".codigo").html(item.id);
                    $(linha).find(".codigoCid").html(item.codigo);
                    $(linha).find(".descricao").html(item.descricao);
                    $(linha).find(".status").html(item.status ? item.status.nome : "Não Definido");
                    $(linha).find(".versaoCid").html(item.versaocid ? item.versaocid.nome : "Não Definido");

                    $(linha).show();
                    $("#tabela").append(linha);
                });

                $('#tabelaCid').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar CIDs.");
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
                location.reload();
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
                $("#txtcodigo").val(jsonResult.codigo);
                $("#txtdescricao").val(jsonResult.descricao);
                $("#selectStatus").val(jsonResult.idStatus);
                $("#selectVersaoCid").val(jsonResult.idVersaoCid);
                $("#txtdataCadastro").val(new Date(jsonResult.dataCadastro).toISOString().split('T')[0]);
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }

    carregarCids();
});
