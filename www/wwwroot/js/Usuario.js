const urlAPI = "https://localhost:7034/";

let statusOptions = '';
let perfilUsuarioOptions = '';
let houveAlteracao = false;

$(document).ready(async function () {
    await carregarDadosSelecoes();

    if ($("#tabela").length > 0) {
        carregarUsuarios();
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

    $(".form-control").on("input change", function () {
        if ($(this).hasClass('is-invalid')) {
            $(this).removeClass('is-invalid');
        }
    });

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/UsuarioCadastro?id=" + codigo;
    });

    $(document).on("change", ".alterar-status", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        let novoStatus = $(elemento.target).val();

        if (novoStatus === "0") {
            alert("Seleção inválida! Por favor, escolha um status válido.");
            $(elemento.target).val($(elemento.target).data('original-value'));
        } else {
            mudarStatus(codigo, novoStatus);
        }
    });

    $(document).on("focus", ".alterar-status", function () {
        $(this).data('original-value', $(this).val());
    });

    $("#selectEstabelecimentoSaude").change(function () {
        const estabelecimentoSelecionado = $(this).val();
        console.log("Estabelecimento selecionado:", estabelecimentoSelecionado);
        if (estabelecimentoSelecionado !== "0") {
            carregarFuncionariosPorEstabelecimento(estabelecimentoSelecionado);
        } else {
            $("#selectFuncionario").empty().append('<option value="0">Selecione um funcionário</option>');
        }
    });
});

// Função para carregar os funcionários baseados no estabelecimento selecionado
function carregarFuncionariosPorEstabelecimento(estabelecimentoId) {
    console.log("Carregando funcionários para o estabelecimento ID:", estabelecimentoId);
    return $.ajax({
        url: urlAPI + `api/Usuario/FuncionariosPorEstabelecimento/${estabelecimentoId}`,
        method: "GET",
        success: function (data) {
            console.log("Funcionários recebidos:", data);
            let options = '<option value="0">Selecione um funcionário</option>';
            data.forEach(item => {
                options += `<option value="${item.id}">${item.nomeCompleto}</option>`;
            });
            $("#selectFuncionario").html(options);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Erro ao carregar os funcionários:", jqXHR.status, textStatus, errorThrown);
            alert("Erro ao carregar os funcionários para o estabelecimento selecionado.");
        }
    });
}


// Função para carregar as opções de seleção na tela de cadastro
function carregarDadosSelecoes() {
    return Promise.all([
        carregarOpcoesStatus(),
        carregarOpcoesPerfilUsuario(),
        carregarEstabelecimentosSaude(),
    ]);
}

// Carregar as opções de status
function carregarOpcoesStatus() {
    const cachedStatus = localStorage.getItem('statusOptions');
    if (cachedStatus) {
        statusOptions = cachedStatus;
        $("#selectStatus").html(statusOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Usuario/tipoStatus",
        method: "GET",
        success: function (data) {
            statusOptions = '<option value="0">Selecione um status</option>';
            data.forEach(item => {
                statusOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectStatus").html(statusOptions);
            localStorage.setItem('statusOptions', statusOptions);
        },
        error: function () {
            alert("Erro ao carregar os status.");
        }
    });
}

// Carregar as opções de perfil de usuário
function carregarOpcoesPerfilUsuario() {
    const cachedPerfilUsuario = localStorage.getItem('perfilUsuarioOptions');
    if (cachedPerfilUsuario) {
        perfilUsuarioOptions = cachedPerfilUsuario;
        $("#selectPerfilUsuario").html(perfilUsuarioOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Usuario/tipoPerfilUsuario",
        method: "GET",
        success: function (data) {
            perfilUsuarioOptions = '<option value="0">Selecione um perfil de usuário</option>';
            data.forEach(item => {
                perfilUsuarioOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectPerfilUsuario").html(perfilUsuarioOptions);
            localStorage.setItem('perfilUsuarioOptions', perfilUsuarioOptions);
        },
        error: function () {
            alert("Erro ao carregar os perfis de usuário.");
        }
    });
}

// Carregar os estabelecimentos de saúde
function carregarEstabelecimentosSaude() {
    return $.ajax({
        url: urlAPI + "api/EstabelecimentoSaude",
        method: "GET",
        success: function (data) {
            let options = '<option value="0">Selecione um estabelecimento de saúde</option>';
            data.forEach(item => {
                options += `<option value="${item.id}">${item.nomeFantasia}</option>`;
            });
            $("#selectEstabelecimentoSaude").html(options);
        },
        error: function () {
            alert("Erro ao carregar os estabelecimentos de saúde.");
        }
    });
}

// Função para carregar os dados dos usuários na tela de gerenciamento
function carregarUsuarios() {
    $("#loading").show();

    $.ajax({
        url: urlAPI + "api/Usuario/dadosBasicos",
        method: "GET",
        success: function (data) {
            const tabela = $("#tabela");
            tabela.empty();

            const fragment = document.createDocumentFragment();

            $.each(data, function (index, item) {
                var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                $(linha).find(".codigo").html(item.id);
                $(linha).find(".nomeUsuario").html(item.nomeUsuario);
                $(linha).find(".perfilUsuario").html(item.perfilusuario);
                $(linha).find(".funcionarioNome").html(item.funcionarioNome);
                $(linha).find(".estabelecimentoSaudeNome").html(item.estabelecimentoSaudeNome);

                var statusSelect = $("<select>")
                    .addClass("form-select alterar-status")
                    .html(statusOptions)
                    .val(item.idStatus);
                $(linha).find(".status").html(statusSelect);

                fragment.appendChild(linha[0]);
            });

            tabela.append(fragment);

            $('#tabelaUsuario').DataTable({
                language: {
                    url: '/js/pt-BR.json'
                },
                destroy: true
            });

            $("#loading").hide();
        },
        error: function () {
            alert("Erro ao carregar usuários.");
            $("#loading").hide();
        }
    });
}



// Validação dos campos no formulário de cadastro de usuário
function validarCampos() {
    let isValid = true;
    $(".form-control").removeClass('is-invalid');

    const camposObrigatorios = [
        "#txtnomeUsuario",
        "#txtsenha",
        "#selectStatus",
        "#selectPerfilUsuario",
        "#selectFuncionario"
    ];

    camposObrigatorios.forEach(function (campo) {
        let valor = $(campo).val().trim();

        if (valor === "" || valor === "0") {
            $(campo).addClass('is-invalid');
            isValid = false;
        }
    });

    return isValid;
}

// Ação de salvar usuário no banco de dados
$("#btnsalvar").click(function () {
    if ($("#txtid").val() !== "0" && houveAlteracao) {
        const confirmSave = confirm("Você fez alterações no formulário. Deseja salvar as alterações?");
        if (!confirmSave) {
            return;
        }
    }

    if (validarCampos()) {
        const obj = {
            id: $("#txtid").val(),
            nomeUsuario: $("#txtnomeUsuario").val(),
            senha: $("#txtsenha").val(),
            idStatus: $("#selectStatus").val(),
            idPerfilUsuario: $("#selectPerfilUsuario").val(),
            idFuncionario: $("#selectFuncionario").val()
        };

        $.ajax({
            type: obj.id == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Usuario" + (obj.id != "0" ? "/" + obj.id : ""),
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function () {
                limparFormulario();
                alert("Dados Salvos com sucesso!");

                if ($("#tabela").length > 0) {
                    carregarUsuarios();
                }
                houveAlteracao = false;
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

// Limpar o formulário após salvar ou cancelar
function limparFormulario() {
    $("#txtnomeUsuario").val('');
    $("#txtsenha").val('');
    $("#txtid").val('0');
    $("#selectStatus").val("0");
    $("#selectPerfilUsuario").val("0");
    $("#selectFuncionario").val("0");
    houveAlteracao = false;
}

// Mudar o status do usuário
function mudarStatus(codigo, novoStatus) {
    $.ajax({
        type: "PATCH",
        url: urlAPI + "api/Usuario/" + codigo + "/mudarStatus",
        contentType: "application/json",
        data: JSON.stringify(novoStatus),
        dataType: "json",
        success: function () {
            alert('Status alterado com sucesso!');
        },
        error: function (xhr, textStatus, errorThrown) {
            alert("Erro ao alterar o status do usuário: " + xhr.responseText);
        }
    });
}

// Visualizar usuário existente para edição
async function visualizar(codigo) {
    try {
        $("#loading").show();

        const usuarioPromise = $.ajax({
            type: "GET",
            url: urlAPI + "api/Usuario/" + codigo + "/dadosCompletos",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
        });

        const [jsonResult] = await Promise.all([usuarioPromise]);

        if (jsonResult) {
            $("#txtid").val(jsonResult.id);
            $("#txtnomeUsuario").val(jsonResult.nomeUsuario);
            $("#txtsenha").val(jsonResult.senha);
            $("#selectStatus").val(jsonResult.idStatus);
            $("#selectPerfilUsuario").val(jsonResult.idPerfilUsuario);

            if (jsonResult.funcionario && jsonResult.funcionario.idEstabelecimentoSaude) {
                await carregarEstabelecimentosSaude().then(() => {
                    $("#selectEstabelecimentoSaude").val(jsonResult.funcionario.idEstabelecimentoSaude);
                    return carregarFuncionariosPorEstabelecimento(jsonResult.funcionario.idEstabelecimentoSaude);
                }).then(() => {
                    $("#selectFuncionario").val(jsonResult.idFuncionario);
                });

                // Adicionar nome do funcionário e nome do estabelecimento de saúde
                $("#nomeFuncionario").val(jsonResult.funcionario.nomeCompleto);
                $("#nomeEstabelecimentoSaude").val(jsonResult.funcionario.estabelecimentosaude.nomeFantasia);
            } else {
                console.error("Estabelecimento de Saúde ou Funcionário não definidos corretamente.");
            }
        } else {
            alert("Dados do usuário não encontrados.");
        }
    } catch (error) {
        console.error("Erro ao carregar os dados:", error);
        alert("Erro ao carregar os dados: " + error.responseText);
    } finally {
        $("#loading").hide();
    }
}


