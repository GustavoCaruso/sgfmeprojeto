const urlAPI = "https://localhost:7034/";

let statusProcessoOptions = '';
let tipoProcessoOptions = '';
let medicamentos = [];
let houveAlteracao = false;
let medicamentoEmEdicao = null;

$(document).ready(async function () {
    await carregarDadosSelecoes();

    if ($("#tabela").length > 0) {
        carregarDispensacoes();
    } else if ($("#txtid").length > 0) {
        let params = new URLSearchParams(window.location.search);
        let id = params.get('id');
        if (id) {
            visualizar(id);
        } else {
            let dataAtual = new Date().toISOString().split('T')[0];
            $("#txtinicioApac").val(dataAtual);
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
        let codigo = $(this).closest("tr").find(".codigo").text();
        console.log("Clique no botão alterar. Código:", codigo);
        window.location.href = "/DispensacaoCadastro?id=" + codigo;
    });

    $(document).on("change", ".alterar-status", function (elemento) {
        let codigo = $(this).closest("tr").find(".codigo").text();
        let novoStatus = $(this).val();

        if (novoStatus === "0") {
            alert("Seleção inválida! Por favor, escolha um status válido.");
            $(this).val($(this).data('original-value'));
        } else {
            console.log("Mudança de status. Código:", codigo, "Novo Status:", novoStatus);
            mudarStatus(codigo, novoStatus);
        }
    });

    $(document).on("focus", ".alterar-status", function () {
        $(this).data('original-value', $(this).val());
    });

    $("#btnAdicionarMedicamento").click(function () {
        adicionarMedicamento();
    });

    $(document).off("click", ".btn-danger[data-type='medicamento']").on("click", ".btn-danger[data-type='medicamento']", function () {
        const index = $(this).data("index");
        const confirmDelete = confirm("Você tem certeza que deseja excluir este medicamento?");
        if (confirmDelete) {
            medicamentos.splice(index, 1);
            atualizarTabelaMedicamentos();
        }
    });

    $("#btnSalvar").click(function () {
        salvarDispensacao(); // Chama a função quando o botão for clicado
    });
});

function carregarDadosSelecoes() {
    return Promise.all([
        carregarOpcoesStatusProcesso(),
        carregarOpcoesTipoProcesso(),
        carregarPacientes(),
        carregarCids(),
        carregarMedicamentos(), // Agora carregando todos os medicamentos disponíveis
    ]);
}

function carregarPacientes() {
    return $.ajax({
        url: urlAPI + "api/Paciente", // Confirme se este é o endpoint correto para obter os pacientes
        method: "GET",
        success: function (data) {
            let pacienteOptions = '<option value="0">Selecione um paciente</option>';
            data.forEach(item => {
                pacienteOptions += `<option value="${item.id}">${item.nomeCompleto}</option>`;
            });
            $("#selectPaciente").html(pacienteOptions);
        },
        error: function () {
            alert("Erro ao carregar pacientes.");
        }
    });
}

function carregarCids() {
    return $.ajax({
        url: urlAPI + "api/Cid", // Confirme se este é o endpoint correto para obter os CIDs
        method: "GET",
        success: function (data) {
            let cidOptions = '<option value="0">Selecione um CID</option>';
            data.forEach(item => {
                cidOptions += `<option value="${item.id}">${item.descricao}</option>`;
            });
            $("#selectCid").html(cidOptions);
        },
        error: function () {
            alert("Erro ao carregar CIDs.");
        }
    });
}

function carregarOpcoesStatusProcesso() {
    return $.ajax({
        url: urlAPI + "api/Dispensacao/tipoStatusProcesso", // Endpoint da API para carregar status
        method: "GET",
        success: function (data) {
            console.log("Dados recebidos: ", data); // Verificar os dados recebidos

            let statusProcessoOptions = '<option value="0">Selecione um status de processo</option>';

            // Itera sobre o array de status e cria opções para o select
            data.forEach(item => {
                statusProcessoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });

            // Atualiza o select com as opções de status

            $("#selectStatusProcesso").html(statusProcessoOptions);
        },
        error: function () {
            alert("Erro ao carregar os status de processo.");
        }
    });
}


function carregarOpcoesTipoProcesso() {
    return $.ajax({
        url: urlAPI + "api/Dispensacao/tipoProcesso",
        method: "GET",
        success: function (data) {
            tipoProcessoOptions = '<option value="0">Selecione um tipo de processo</option>';
            data.forEach(item => {
                tipoProcessoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectTipoProcesso").html(tipoProcessoOptions);
        },
        error: function () {
            alert("Erro ao carregar os tipos de processo.");
        }
    });
}

// Função para carregar medicamentos no select corretamente
function carregarMedicamentos() {
    return $.ajax({
        url: urlAPI + "api/Medicamento", // Confirme se este é o endpoint correto para obter os medicamentos
        method: "GET",
        success: function (data) {
            let medicamentoOptions = '<option value="0">Selecione um medicamento</option>';
            data.forEach(item => {
                medicamentoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectMedicamento").html(medicamentoOptions);
            console.log("Medicamentos carregados:", data); // Log para verificar se os medicamentos estão sendo carregados corretamente
        },
        error: function () {
            alert("Erro ao carregar medicamentos.");
        }
    });
}

function carregarMedicamentosPorDispensacao() {
    const dispensacaoId = $("#txtid").val();
    if (!dispensacaoId || dispensacaoId === "0") return;

    return $.ajax({
        url: urlAPI + "api/Dispensacao/" + dispensacaoId + "/medicamentos",
        method: "GET",
        success: function (data) {
            let medicamentoOptions = '<option value="0">Selecione um medicamento</option>';
            data.forEach(item => {
                medicamentoOptions += `<option value="${item.idMedicamento}">${item.nomeMedicamento}</option>`;
            });
            $("#selectMedicamento").html(medicamentoOptions);
        },
        error: function () {
            alert("Erro ao carregar medicamentos.");
        }
    });
}

function adicionarMedicamento() {
    const idMedicamento = $("#selectMedicamento").val();
    const nomeMedicamento = $("#selectMedicamento option:selected").text();
    const quantidade = $("#txtQuantidade").val();

    // Corrigir a captura dos checkboxes
    const recibo = $("#txtRecibo").prop('checked') ? "Sim" : "Não";
    const receita = $("#txtReceita").prop('checked') ? "Sim" : "Não";
    const medicamentoChegou = $("#txtMedicamentoChegou").prop('checked') ? "Sim" : "Não";
    const medicamentoEntregue = $("#txtMedicamentoEntregue").prop('checked') ? "Sim" : "Não";
    const dataEntrega = $("#txtDataEntrega").val();

    if (idMedicamento === "0" || !quantidade || !dataEntrega) {
        alert("Preencha todos os campos obrigatórios de medicamento.");
        return;
    }

    const novoMedicamento = {
        idMedicamento,
        nomeMedicamento,
        quantidade,
        recibo,
        receita,
        medicamentoChegou,
        medicamentoEntregue,
        dataEntrega
    };

    if (medicamentoEmEdicao !== null) {
        medicamentos[medicamentoEmEdicao] = novoMedicamento;
        medicamentoEmEdicao = null;
    } else {
        medicamentos.push(novoMedicamento);
    }

    atualizarTabelaMedicamentos();
    limparCamposMedicamento();
}

function atualizarTabelaMedicamentos() {
    const tabela = $("#medicamentoTable tbody");
    tabela.empty();

    medicamentos.forEach((medicamento, index) => {
        const linha = `<tr>
            <td>${medicamento.nomeMedicamento}</td>
            <td>${medicamento.quantidade}</td>
            <td>${medicamento.recibo === "Sim" ? "Sim" : "Não"}</td> <!-- Ajuste -->
            <td>${medicamento.receita === "Sim" ? "Sim" : "Não"}</td> <!-- Ajuste -->
            <td>${medicamento.medicamentoChegou === "Sim" ? "Sim" : "Não"}</td> <!-- Ajuste -->
            <td>${medicamento.medicamentoEntregue === "Sim" ? "Sim" : "Não"}</td> <!-- Ajuste -->
            <td>${medicamento.dataEntrega}</td>
            <td>
                <button type="button" class="btn btn-warning btn-edit" data-index="${index}" data-type="medicamento">Editar</button>
                <button type="button" class="btn btn-danger" data-index="${index}" data-type="medicamento">Excluir</button>
            </td>
        </tr>`;
        tabela.append(linha);
    });

    $(".btn-edit[data-type='medicamento']").off("click").on("click", function () {
        const index = $(this).data("index");
        editarMedicamento(index);
    });
}

function editarMedicamento(index) {
    const medicamento = medicamentos[index];
    medicamentoEmEdicao = index;

    $("#selectMedicamento").val(medicamento.idMedicamento);
    $("#txtQuantidade").val(medicamento.quantidade);
    $("#txtRecibo").prop('checked', medicamento.recibo === "Sim"); // Ajuste aqui
    $("#txtReceita").prop('checked', medicamento.receita === "Sim"); // Ajuste aqui
    $("#txtMedicamentoChegou").prop('checked', medicamento.medicamentoChegou === "Sim"); // Ajuste aqui
    $("#txtMedicamentoEntregue").prop('checked', medicamento.medicamentoEntregue === "Sim"); // Ajuste aqui
    $("#txtDataEntrega").val(medicamento.dataEntrega);
}

function limparCamposMedicamento() {
    $("#selectMedicamento").val('0');
    $("#txtQuantidade").val('');
    $("#txtRecibo").prop('checked', false);  // Limpar checkbox Recibo
    $("#txtReceita").prop('checked', false); // Limpar checkbox Receita
    $("#txtMedicamentoChegou").prop('checked', false); // Limpar checkbox Chegou
    $("#txtMedicamentoEntregue").prop('checked', false); // Limpar checkbox Entregue
    $("#txtDataEntrega").val('');
    medicamentoEmEdicao = null;
}

function carregarDispensacoes() {
    $("#loading").show();

    $.ajax({
        url: urlAPI + "api/Dispensacao/dadosBasicos",
        method: "GET",
        success: function (data) {
            console.log(data);
            const tabela = $("#tabela");
            tabela.empty();

            const fragment = document.createDocumentFragment();

            $.each(data, function (index, item) {
                var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                $(linha).find(".codigo").html(item.id);
                $(linha).find(".nomePaciente").html(item.nomePaciente ? item.nomePaciente : "Nome não disponível");
                $(linha).find(".cid").html(item.nomeCid || "CID não informado");
                $(linha).find(".inicioApac").html(new Date(item.inicioApac).toLocaleDateString());
                $(linha).find(".fimApac").html(new Date(item.fimApac).toLocaleDateString());
                $(linha).find(".observacao").html(item.observacao ? item.observacao : "Sem observações");
                $(linha).find(".status").html(item.statusNome || "Status não disponível");

                $(linha).find(".acoes").html(`<button type="button" class="alterar btn btn-info btn-sm">Visualizar</button>`);

                fragment.appendChild(linha[0]);
            });

            tabela.append(fragment);

            $('#tabelaDispensacao').DataTable({
                language: {
                    url: '/js/pt-BR.json'
                },
                destroy: true
            });

            $("#loading").hide();
        },
        error: function () {
            alert("Erro ao carregar dispensações.");
            $("#loading").hide();
        }
    });
}

function salvarDispensacao() {
    if (!validarCampos()) {
        return;
    }

    // Função para formatar a data corretamente (mantendo o formato de data `YYYY-MM-DD` esperado pela API)
    function formatarData(data) {
        if (!data || data === "") return null; // Se o campo estiver vazio, retorna null
        return data; // Retorna o valor da data, já no formato adequado para a API
    }

    // Prepara os medicamentos no formato esperado
    const medicamentosFormatados = medicamentos.map(medicamento => ({
        id: medicamento.id || 0, // Se não tiver um ID, assume 0 para criação
        idMedicamento: parseInt(medicamento.idMedicamento), // Garante que seja numérico
        quantidade: parseInt(medicamento.quantidade), // Garante que seja numérico
        recibo: medicamento.recibo === true || medicamento.recibo === "Sim", // Booleano
        receita: medicamento.receita === true || medicamento.receita === "Sim", // Booleano
        medicamentoChegou: medicamento.medicamentoChegou === true || medicamento.medicamentoChegou === "Sim", // Booleano
        medicamentoEntregue: medicamento.medicamentoEntregue === true || medicamento.medicamentoEntregue === "Sim", // Booleano
        dataEntrega: formatarData(medicamento.dataEntrega) // Já está no formato `YYYY-MM-DD`, apenas retorna
    }));

    // Prepara o objeto conforme esperado pela API
    const obj = {
        id: $("#txtid").val() ? parseInt($("#txtid").val()) : 0, // Garante que seja numérico
        idPaciente: parseInt($("#selectPaciente").val()), // Garante que seja numérico
        idCid: parseInt($("#selectCid").val()), // Garante que seja numérico
        inicioApac: formatarData($("#txtinicioApac").val()) || new Date().toISOString(), // Se não houver, usa a data atual
        fimApac: formatarData($("#txtfimApac").val()) || new Date().toISOString(), // Se não houver, usa a data atual
        observacao: $("#txtObservacao").val(),
        dataRenovacao: formatarData($("#dataRenovacao").val()), // Usa a data diretamente
        dataSuspensao: formatarData($("#dataSuspensao").val()), // Usa a data diretamente
        idStatusProcesso: parseInt($("#selectStatusProcesso").val()), // Garante que seja numérico
        idTipoProcesso: parseInt($("#selectTipoProcesso").val()), // Garante que seja numérico
        medicamento: medicamentosFormatados // Medicamentos formatados
    };

    console.log("Dados enviados: ", obj); // Log para verificar os dados que estão sendo enviados

    $.ajax({
        type: obj.id === 0 ? "POST" : "PUT", // Verifica se é criação ou atualização
        url: urlAPI + "api/Dispensacao" + (obj.id !== 0 ? "/" + obj.id : ""),
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(obj), // Serializa o objeto em JSON
        dataType: "json",
        success: function () {
            limparFormulario();
            alert("Dados salvos com sucesso!");

            if ($("#tabela").length > 0) {
                carregarDispensacoes(); // Atualiza a tabela se necessário
            }
            houveAlteracao = false;
        },
        error: function (jqXHR, textStatus) {
            console.log("Erro ao salvar os dados: ", jqXHR.responseText); // Exibe a resposta de erro no console
            alert("Erro ao salvar os dados: " + textStatus);
        }
    });
}




function validarCampos() {
    let isValid = true;
    $(".form-control").removeClass('is-invalid');

    if ($("#selectPaciente").val() === "0") {
        $("#selectPaciente").addClass('is-invalid');
        isValid = false;
    }
    if ($("#selectCid").val() === "0") {
        $("#selectCid").addClass('is-invalid');
        isValid = false;
    }

    if (medicamentos.length === 0) {
        alert("Por favor, adicione pelo menos um medicamento.");
        isValid = false;
    }

    return isValid;
}

function limparFormulario() {
    $("#selectPaciente").val('0');
    $("#selectCid").val('0');
    $("#txtinicioApac").val('');
    $("#txtfimApac").val('');
    $("#txtObservacao").val('');
    $("#selectStatusProcesso").val('0');
    $("#selectTipoProcesso").val('0');
    medicamentos = [];
    atualizarTabelaMedicamentos();
}

function visualizar(id) {
    console.log("Chamando visualizar para o ID: " + id);

    $.ajax({
        url: urlAPI + "api/Dispensacao/" + id + "/dadosCompletos",
        method: "GET",
        success: function (data) {
            console.log("Dados recebidos:", data);

            if (data) {
                $("#txtid").val(data.id);
                $("#selectPaciente").val(data.idPaciente);
                $("#selectCid").val(data.idCid);
                $("#txtinicioApac").val(new Date(data.inicioApac).toISOString().split('T')[0]);
                $("#txtfimApac").val(new Date(data.fimApac).toISOString().split('T')[0]);
                $("#txtObservacao").val(data.observacao);
                $("#selectStatusProcesso").val(data.idStatusProcesso);
                $("#selectTipoProcesso").val(data.idTipoProcesso);

                medicamentos = data.medicamento || [];
                atualizarTabelaMedicamentos();
            } else {
                alert("Nenhuma informação encontrada para essa dispensação.");
            }
        },
        error: function (error) {
            console.log("Erro ao carregar os dados da dispensação:", error);
            alert("Erro ao carregar os dados da dispensação.");
        }
    });
}
