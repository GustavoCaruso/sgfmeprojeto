const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
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
        error: function () {
            alert("Erro ao carregar os tipos de contato.");
        }
    });

    $("#btnlimpar").click(function () {
        limparFormulario();
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

        $.ajax({
            type: obj.id == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Medico" + (obj.id != "0" ? "/" + obj.id : ""),
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function () {
                limparFormulario();
                alert("Dados Salvos com sucesso!");
                carregarMedicos();
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
    });

    function limparFormulario() {
        $("#txtnomeCompleto").val('');
        $("#txtdataNascimento").val('');
        $("#txtcrm").val('');
        $("#txtid").val('0');
        contatos = [];
        atualizarTabelaContatos();
    }

    function carregarMedicos() {
        $.ajax({
            url: urlAPI + "api/Medico",
            method: "GET",
            success: function (data) {
                const tabela = $("#medicosTable tbody");
                tabela.empty();

                data.forEach(medico => {
                    const linha = `<tr>
                        <td>${medico.id}</td>
                        <td>${medico.nomeCompleto}</td>
                        <td>${new Date(medico.dataNascimento).toLocaleDateString()}</td>
                        <td>${medico.crm}</td>
                        <td><button type="button" class="btn btn-primary btn-editar" data-id="${medico.id}">Editar</button></td>
                    </tr>`;
                    tabela.append(linha);
                });

                $(".btn-editar").click(function () {
                    const id = $(this).data("id");
                    editarMedico(id);
                });
            },
            error: function () {
                alert("Erro ao carregar médicos.");
            }
        });
    }

    function editarMedico(id) {
        $.ajax({
            url: urlAPI + "api/Medico/" + id,
            method: "GET",
            success: function (data) {
                $("#txtid").val(data.id);
                $("#txtnomeCompleto").val(data.nomeCompleto);
                $("#txtdataNascimento").val(new Date(data.dataNascimento).toISOString().split('T')[0]);
                $("#txtcrm").val(data.crm);

                contatos = data.contato.map(c => ({
                    idTipoContato: c.idTipoContato,
                    tipo: c.tipocontato.nome, // Acesso ao nome do tipo de contato
                    valor: c.valor
                }));
                atualizarTabelaContatos();
            },
            error: function () {
                alert("Erro ao carregar os dados do médico.");
            }
        });
    }

    carregarMedicos();
});
