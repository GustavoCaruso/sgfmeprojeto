const urlAPI = "https://localhost:7034/";
let statusOptions = '';
let versaoCidOptions = ''; // Adiciona a variável para armazenar as opções de versão do CID

$(document).ready(function () {
    carregarOpcoesStatus(() => {
        carregarOpcoesVersaoCid(() => {
            if ($("#txtid").length > 0) {
                let params = new URLSearchParams(window.location.search);
                let id = params.get('id');
                if (id) {
                    visualizar(id);
                }
            }
        });
    });

    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    if ($("#tabela").length > 0) {
        carregarCids();
    }

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/CidCadastro?id=" + codigo;
    });

    $(document).on("change", ".alterar-status", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        let novoStatus = $(elemento.target).val();
        mudarStatus(codigo, novoStatus);
    });

    function carregarOpcoesStatus(callback) {
        $.ajax({
            url: urlAPI + "api/Cid/tipoStatus",
            method: "GET",
            success: function (data) {
                statusOptions = '';  // Reinicializa a variável statusOptions
                data.forEach(item => {
                    statusOptions += `<option value="${item.id}">${item.nome}</option>`;
                });
                $("#selectStatus").html(statusOptions);  // Preenche o select com as opções de status
                if (callback) callback(); // Chama o callback após carregar as opções
            },
            error: function () {
                alert("Erro ao carregar os status.");
            }
        });
    }

    function carregarOpcoesVersaoCid(callback) {
        $.ajax({
            url: urlAPI + "api/Cid/tipoVersaoCid",
            method: "GET",
            success: function (data) {
                versaoCidOptions = '';  // Reinicializa a variável versaoCidOptions
                data.forEach(item => {
                    versaoCidOptions += `<option value="${item.id}">${item.nome}</option>`;
                });
                $("#selectVersaoCid").html(versaoCidOptions);  // Preenche o select com as opções de versão do CID
                if (callback) callback(); // Chama o callback após carregar as opções
            },
            error: function () {
                alert("Erro ao carregar as versões do CID.");
            }
        });
    }

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtcodigo").val().trim()) {
            $("#txtcodigo").addClass('is-invalid');
            $("#txtcodigo").siblings('.invalid-feedback').text('O código do CID é obrigatório.');
            isValid = false;
        }

        if (!$("#txtdescricao").val().trim() || $("#txtdescricao").val().trim().length > 100) {
            $("#txtdescricao").addClass('is-invalid');
            let feedbackMessage = $("#txtdescricao").val().trim().length > 100 ?
                'A descrição não pode exceder 100 caracteres.' :
                'A descrição é obrigatória.';
            $("#txtdescricao").siblings('.invalid-feedback').text(feedbackMessage);
            isValid = false;
        }

        if (!$("#selectStatus").val().trim() || $("#selectStatus").val() === "0") {
            $("#selectStatus").addClass('is-invalid');
            $("#selectStatus").siblings('.invalid-feedback').text('O status é obrigatório.');
            isValid = false;
        }

        if (!$("#selectVersaoCid").val().trim() || $("#selectVersaoCid").val() === "0") {
            $("#selectVersaoCid").addClass('is-invalid');
            $("#selectVersaoCid").siblings('.invalid-feedback').text('A versão do CID é obrigatória.');
            isValid = false;
        }

        return isValid;
    }

    $(".form-control").on("input change", function () {
        $(this).removeClass('is-invalid');
    });

    function carregarOpcoes(apiEndpoint, selectElement, mensagemPadrao, callback) {
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
                if (callback) callback();
            },
            error: function () {
                alert("Erro ao carregar os dados.");
            }
        });
    }

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                codigo: $("#txtcodigo").val(),
                descricao: $("#txtdescricao").val(),
                idStatus: $("#selectStatus").val(),
                idVersaoCid: $("#selectVersaoCid").val()
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
                    $(linha).find(".versaoCid").html(item.versaocid ? item.versaocid.nome : "Não Definido");
                    $(linha).find(".alterar-status").html(statusOptions);
                    $(linha).find(".alterar-status").val(item.idStatus);

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

    function mudarStatus(codigo, novoStatus) {
        console.log("Alterando status para CID:", codigo, "Novo Status:", novoStatus);
        $.ajax({
            type: "PATCH",
            url: urlAPI + "api/Cid/" + codigo + "/mudarStatus",
            contentType: "application/json",
            data: JSON.stringify(novoStatus),  // Enviando apenas o valor do status
            dataType: "json",
            success: function () {
                alert('Status alterado com sucesso!');
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Erro:", xhr.responseText);
                alert("Erro ao alterar o status do CID: " + xhr.responseText);
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
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }
});
