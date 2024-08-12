const urlAPI = "https://localhost:7034/";
let statusOptions = '';

$(document).ready(function () {
    carregarOpcoesStatus(() => {
        if ($("#txtid").length > 0) {
            let params = new URLSearchParams(window.location.search);
            let id = params.get('id');
            if (id) {
                visualizar(id);
            }
        }
    });

    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    if ($("#tabela").length > 0) {
        carregarMedicamentos();
    }

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/MedicamentoCadastro?id=" + codigo;
    });

    $(document).on("change", ".alterar-status", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        let novoStatus = $(elemento.target).val();
        mudarStatus(codigo, novoStatus);
    });

    function carregarOpcoesStatus(callback) {
        $.ajax({
            url: urlAPI + "api/Medicamento/tipoStatus",
            method: "GET",
            success: function (data) {
                statusOptions = '';  // Reinicializa a variável statusOptions
                data.forEach(item => {
                    statusOptions += `<option value="${item.id}">${item.nome}</option>`;
                });
                $("#selectStatus").html(statusOptions);  // Preenche o select no formulário de cadastro
                if (callback) callback(); // Chama o callback após carregar as opções
            },
            error: function () {
                alert("Erro ao carregar os status.");
            }
        });
    }

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtnome").val().trim()) {
            $("#txtnome").addClass('is-invalid');
            isValid = false;
        } else if ($("#txtnome").val().trim().length > 100) {
            $("#txtnome").addClass('is-invalid');
            $("#txtnome").siblings('.invalid-feedback').text('O nome não pode exceder 100 caracteres.');
            isValid = false;
        }
        if (!$("#selectStatus").val().trim() || $("#selectStatus").val() === "0") {
            $("#selectStatus").addClass('is-invalid');
            isValid = false;
        }

        return isValid;
    }

    $(".form-control").on("input change", function () {
        $(this).removeClass('is-invalid');
    });

    function carregarMedicamentos() {
        $.ajax({
            url: urlAPI + "api/Medicamento/todosMedicamentos",
            method: "GET",
            success: function (data) {
                $("#tabela").empty();
                $.each(data, function (index, item) {
                    var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                    $(linha).find(".codigo").html(item.id);
                    $(linha).find(".nome").html(item.nome);
                    $(linha).find(".status select").html(statusOptions);  // Preenche o select com as opções de status
                    $(linha).find(".status select").val(item.idStatus);  // Define o status atual

                    $(linha).show();
                    $("#tabela").append(linha);
                });

                $('#tabelaMedicamento').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar medicamentos.");
            }
        });
    }

    function mudarStatus(codigo, novoStatus) {
        console.log("Alterando status para Medicamento:", codigo, "Novo Status:", novoStatus);
        $.ajax({
            type: "PATCH",
            url: urlAPI + "api/Medicamento/" + codigo + "/mudarStatus",
            contentType: "application/json",
            data: JSON.stringify(novoStatus),  // Enviando apenas o valor do status
            dataType: "json",
            success: function () {
                alert('Status alterado com sucesso!');
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Erro:", xhr.responseText);
                alert("Erro ao alterar o status do medicamento: " + xhr.responseText);
            }
        });
    }

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                nome: $("#txtnome").val(),
                idStatus: $("#selectStatus").val()
            };

            $.ajax({
                type: obj.id == "0" ? "POST" : "PUT",
                url: urlAPI + "api/Medicamento" + (obj.id != "0" ? "/" + obj.id : ""),
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify(obj),
                dataType: "json",
                success: function () {
                    limparFormulario();
                    alert("Dados Salvos com sucesso!");

                    if ($("#tabela").length > 0) {
                        carregarMedicamentos();
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
        $("#txtnome").val('');
        $("#txtid").val('0');
        $("#selectStatus").val('');
    }

    function visualizar(codigo) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Medicamento/" + codigo,
            contentType: "application/json;charset=utf-8",
            data: {},
            dataType: "json",
            success: function (jsonResult) {
                $("#txtid").val(jsonResult.id);
                $("#txtnome").val(jsonResult.nome);
                $("#selectStatus").val(jsonResult.idStatus);
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }
});
