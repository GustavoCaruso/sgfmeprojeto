const urlAPI = "https://localhost:7034/";

let statusProcessoOptions = '';
let tipoProcessoOptions = '';
let pacienteOptions = '';
let medicamentoOptions = '';
let medicamentos = [];
let houveAlteracao = false;
let medicamentoEmEdicao = null;
let idPaciente = null; // Variável global para armazenar o ID do paciente

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
            // Definir a data atual menos 3 horas para o campo de início APAC
            let dataAtual = new Date();
            dataAtual.setHours(dataAtual.getHours() - 3); // Subtrai 3 horas da data atual

            let dataAtualFormatada = dataAtual.toISOString().split('T')[0]; // Formata como YYYY-MM-DD
            $("#txtinicioApac").val(dataAtualFormatada); // Define no campo de início do APAC

            // Calcular a data de fim como 6 meses à frente
            let dataFim = new Date(dataAtual); // Cria uma cópia da data atual
            dataFim.setMonth(dataFim.getMonth() + 6); // Adiciona 6 meses

            // Verifica se a adição de 6 meses resultou em um mês inválido (isso pode acontecer ao adicionar meses em datas como 31)
            if (dataFim.getDate() !== dataAtual.getDate()) {
                dataFim.setDate(0); // Define o último dia do mês anterior
            }

            let dataFimFormatada = dataFim.toISOString().split('T')[0]; // Formata como YYYY-MM-DD
            $("#txtfimApac").val(dataFimFormatada); // Define no campo de fim do APAC
        
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

    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/DispensacaoCadastro?id=" + codigo;
    });

    $("#btnAdicionarMedicamento").click(function () {
        adicionarMedicamento();
    });

    $("#txtRgPaciente").on('blur', function () {
        const rgPaciente = $(this).val().trim();

        if (rgPaciente) {
            $.ajax({
                url: urlAPI + "api/Dispensacao/buscarPorRg/" + rgPaciente,
                method: "GET",
                success: function (data) {
                    if (data) {
                        // Preencher os campos do paciente
                        $("#txtNomePaciente").val(data.nomePaciente);
                        $("#txtDataNascimento").val(data.dataNascimento ? data.dataNascimento.split('T')[0] : '');
                        $("#txtCpfPaciente").val(data.cpfNumero);

                        // Armazenar o ID do paciente
                        idPaciente = data.idPaciente;  // Armazena o ID do paciente retornado

                        // Definir a data atual menos 3 horas para o campo de início APAC
                        let dataAtual = new Date();
                        dataAtual.setHours(dataAtual.getHours() - 3); // Subtrai 3 horas da data atual

                        let dataAtualFormatada = dataAtual.toISOString().split('T')[0]; // Extrai apenas a data no formato YYYY-MM-DD
                        $("#txtinicioApac").val(dataAtualFormatada); // Define no campo de início do APAC

                        // Calcular a data de fim como 6 meses à frente
                        let dataFim = new Date(dataAtual); // Cria uma cópia da data atual
                        dataFim.setMonth(dataFim.getMonth() + 6); // Adiciona 6 meses

                        // Verifica se a adição de 6 meses resultou em um mês inválido (exemplo, dia 31)
                        if (dataFim.getDate() !== dataAtual.getDate()) {
                            dataFim.setDate(0); // Define o último dia do mês anterior
                        }

                        let dataFimFormatada = dataFim.toISOString().split('T')[0]; // Extrai apenas a data no formato YYYY-MM-DD
                        $("#txtfimApac").val(dataFimFormatada); // Define no campo de fim do APAC

                        // Preencher a tabela de representantes
                        let tbody = $("#representanteTable tbody");
                        tbody.empty(); // Limpa a tabela

                        data.representante.forEach(rep => {
                            let dataNascimentoRepresentante = rep.dataNascimentoRepresentante ? new Date(rep.dataNascimentoRepresentante).toLocaleDateString('pt-BR') : '';

                            let row = `
                            <tr>
                                <td>${rep.nomeRepresentante}</td>
                                <td>${rep.rgRepresentante}</td>
                                <td>${rep.cpfRepresentante}</td>
                                <td>${dataNascimentoRepresentante}</td>
                            </tr>`;

                            tbody.append(row);
                        });

                        if (data.representante.length === 0) {
                            tbody.append('<tr><td colspan="4">Nenhum representante encontrado</td></tr>');
                        }
                    } else {
                        alert("Paciente não encontrado");
                        limparCamposPaciente();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error("Erro ao buscar o paciente:", textStatus, errorThrown);
                    alert("Erro ao buscar o paciente");
                    limparCamposPaciente();
                }
            });
        } else {
            alert("Por favor, insira um RG válido.");
            limparCamposPaciente();
        }
    });



});



// Função para limpar os campos do paciente
function limparCamposPaciente() {
    $("#txtNomePaciente").val('');
    $("#txtDataNascimento").val('');
    $("#txtCpfPaciente").val('');
    $("#representanteTable tbody").empty();
}



function carregarDadosSelecoes() {
    return Promise.all([
        carregarOpcoesStatusProcesso(),
        carregarOpcoesTipoProcesso(),
        carregarOpcoesPaciente(),
        carregarOpcoesMedicamento(),
        carregarOpcoesCid() // Adiciona o carregamento de CID
    ]);
}


function carregarOpcoesStatusProcesso() {
    return $.ajax({
        url: urlAPI + "api/StatusProcesso",
        method: "GET",
        success: function (data) {
            console.log("StatusProcesso carregado:", data); // Log para depuração
            statusProcessoOptions = '<option value="0">Selecione um status</option>';
            data.forEach(item => {
                statusProcessoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectStatusProcesso").html(statusProcessoOptions);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Erro ao carregar os status do processo:", textStatus, errorThrown);
            alert("Erro ao carregar os status do processo.");
        }
    });
}

function carregarOpcoesTipoProcesso() {
    return $.ajax({
        url: urlAPI + "api/TipoProcesso",
        method: "GET",
        success: function (data) {
            console.log("TipoProcesso carregado:", data); // Log para depuração
            tipoProcessoOptions = '<option value="0">Selecione um tipo de processo</option>';
            data.forEach(item => {
                tipoProcessoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectTipoProcesso").html(tipoProcessoOptions);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Erro ao carregar os tipos de processo:", textStatus, errorThrown);
            alert("Erro ao carregar os tipos de processo.");
        }
    });
}


function carregarOpcoesPaciente() {
    return $.ajax({
        url: urlAPI + "api/Paciente",
        method: "GET",
        success: function (data) {
            pacienteOptions = '<option value="0">Selecione um paciente</option>';
            data.forEach(item => {
                pacienteOptions += `<option value="${item.id}">${item.nomeCompleto}</option>`;
            });
            $("#selectPaciente").html(pacienteOptions);
        },
        error: function () {
            alert("Erro ao carregar os pacientes.");
        }
    });
}

function carregarOpcoesMedicamento() {
    return $.ajax({
        url: urlAPI + "api/Medicamento",
        method: "GET",
        success: function (data) {
            medicamentoOptions = '<option value="0">Selecione um medicamento</option>';
            data.forEach(item => {
                medicamentoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectMedicamento").html(medicamentoOptions);
        },
        error: function () {
            alert("Erro ao carregar os medicamentos.");
        }
    });
}



function carregarOpcoesCid() {
    return $.ajax({
        url: urlAPI + "api/Cid",
        method: "GET",
        success: function (data) {
            let cidOptions = '<option value="0">Selecione um CID</option>';
            data.forEach(item => {
                cidOptions += `<option value="${item.id}">${item.codigo} - ${item.descricao}</option>`;
            });
            $("#selectCid").html(cidOptions);
        },
        error: function () {
            alert("Erro ao carregar os CIDs.");
        }
    });
}





function carregarDispensacoes() {
    $.ajax({
        url: urlAPI + "api/Dispensacao/dadosBasicos",
        method: "GET",
        success: function (data) {
            const tabela = $("#tabela");
            tabela.empty();
            data.forEach(item => {
                let linha = `<tr>
                    <td class="codigo">${item.id}</td>
                    <td>${item.nomePaciente}</td>
                    <td>${item.nomeCid}</td>
                    <td>${new Date(item.inicioApac).toLocaleDateString()}</td>
                    <td>${new Date(item.fimApac).toLocaleDateString()}</td>
                    <td>${item.statusNome}</td>
                    <td>${item.tipoProcessoNome}</td>
                    <td><button class="btn btn-primary alterar">Alterar</button></td>
                </tr>`;
                tabela.append(linha);
            });
        },
        error: function () {
            alert("Erro ao carregar dispensações.");
        }
    });
}

function adicionarMedicamento() {
    const idMedicamento = $("#selectMedicamento").val();
    const nomeMedicamento = $("#selectMedicamento option:selected").text();
    const quantidade = $("#txtQuantidade").val();
    const dataEntrega = $("#txtDataEntrega").val();
    const recibo = $("#txtRecibo").is(":checked");
    const receita = $("#txtReceita").is(":checked");
    const medicamentoChegou = $("#txtmedicamentoChegou").is(":checked");
    const medicamentoEntregue = $("#txtmedicamentoEntregue").is(":checked");

    if (!idMedicamento || idMedicamento === "0") {
        alert("Por favor, selecione um medicamento.");
        return;
    }

    if (!quantidade || !dataEntrega) {
        alert("Por favor, preencha a quantidade e a data de entrega.");
        return;
    }

    const novoMedicamento = {
        idMedicamento,
        nomeMedicamento,
        quantidade,
        dataEntrega,
        recibo,
        receita,
        medicamentoChegou,
        medicamentoEntregue
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
            <td>${new Date(medicamento.dataEntrega).toLocaleDateString()}</td>
            <td>${medicamento.recibo ? "Sim" : "Não"}</td>
            <td>${medicamento.receita ? "Sim" : "Não"}</td>
            <td>${medicamento.medicamentoChegou ? "Sim" : "Não"}</td> <!-- Adiciona campo medicamentoChegou -->
            <td>${medicamento.medicamentoEntregue ? "Sim" : "Não"}</td> <!-- Adiciona campo medicamentoEntregue -->
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
    medicamentoEmEdicao = index;

    let medicamento = medicamentos[index];
    $("#selectMedicamento").val(medicamento.idMedicamento);
    $("#txtQuantidade").val(medicamento.quantidade);
    $("#txtDataEntrega").val(new Date(medicamento.dataEntrega).toISOString().split('T')[0]);
    $("#txtRecibo").prop('checked', medicamento.recibo);
    $("#txtReceita").prop('checked', medicamento.receita);
    $("#txtmedicamentoChegou").prop('checked', medicamento.medicamentoChegou); // Carrega medicamentoChegou
    $("#txtmedicamentoEntregue").prop('checked', medicamento.medicamentoEntregue); // Carrega medicamentoEntregue
}


function limparCamposMedicamento() {
    $("#selectMedicamento").val("0");
    $("#txtQuantidade").val('');
    $("#txtDataEntrega").val('');
    $("#txtRecibo").prop('checked', false);
    $("#txtReceita").prop('checked', false);
    medicamentoEmEdicao = null;
}

async function visualizar(id) {
    try {
        const dispensacao = await $.ajax({
            url: `${urlAPI}api/Dispensacao/${id}/dadosCompletos`,
            method: "GET"
        });

        // Preenchendo os campos com os dados retornados
        $("#txtid").val(dispensacao.id);
        $("#selectPaciente").val(dispensacao.idPaciente);
        $("#selectCid").val(dispensacao.idCid);
        $("#txtinicioApac").val(new Date(dispensacao.inicioApac).toISOString().split('T')[0]);
        $("#txtfimApac").val(new Date(dispensacao.fimApac).toISOString().split('T')[0]);
        $("#selectStatusProcesso").val(dispensacao.idStatusProcesso);
        $("#selectTipoProcesso").val(dispensacao.idTipoProcesso);
        $("#txtObservacao").val(dispensacao.observacao);

        // Preenchendo a data de renovação e suspensão, se existirem
        if (dispensacao.dataRenovacao) {
            $("#txtdataRenovacao").val(new Date(dispensacao.dataRenovacao).toISOString().split('T')[0]);
        } else {
            $("#txtdataRenovacao").val(''); // Limpa se não existir
        }

        if (dispensacao.dataSuspensao) {
            $("#txtdataSuspensao").val(new Date(dispensacao.dataSuspensao).toISOString().split('T')[0]);
        } else {
            $("#txtdataSuspensao").val(''); // Limpa se não existir
        }

        // Verifica se o nome do medicamento não está preenchido
        for (const m of dispensacao.medicamento) {
            if (!m.nomeMedicamento || m.nomeMedicamento === "undefined") {
                // Busca o nome do medicamento pelo idMedicamento
                const medicamentoDetalhes = await $.ajax({
                    url: `${urlAPI}api/Medicamento/${m.idMedicamento}`,
                    method: "GET"
                });
                m.nomeMedicamento = medicamentoDetalhes.nome;  // Preenche o nome do medicamento
            }
        }

        // Preenchendo a tabela de medicamentos
        medicamentos = dispensacao.medicamento.map(m => ({
            idMedicamento: m.idMedicamento,
            nomeMedicamento: m.nomeMedicamento,  // Agora o nome será preenchido corretamente
            quantidade: m.quantidade,
            dataEntrega: m.dataEntrega,
            recibo: m.recibo,
            receita: m.receita,
            medicamentoChegou: m.medicamentoChegou,
            medicamentoEntregue: m.medicamentoEntregue
        }));
        atualizarTabelaMedicamentos();

    } catch (error) {
        alert("Erro ao carregar os dados da dispensação.");
    }
}


function validarCampos() {
    let isValid = true;
    $(".form-control").removeClass('is-invalid');

    const camposObrigatorios = [
        "#selectPaciente",
        "#txtinicioApac",
        "#txtfimApac",
        "#selectStatusProcesso",
        "#selectTipoProcesso",
        "#selectCid" // Adiciona o CID na validação
    ];

    camposObrigatorios.forEach(function (campo) {
        let campoElemento = $(campo);

        // Verifica se o campo existe e não está vazio ou não selecionado
        if (campoElemento.length > 0) {
            let valor = campoElemento.val() ? campoElemento.val().trim() : "";

            if (valor === "" || valor === "0") {
                campoElemento.addClass('is-invalid');
                isValid = false;
            }
        }
    });

    if (medicamentos.length === 0) {
        $("#mensagemValidacao").text("Por favor, adicione pelo menos um medicamento.");
        isValid = false;
    } else {
        $("#mensagemValidacao").text("");
    }

    return isValid;
}


$("#btnsalvar").click(function () {
    if (validarCampos()) {
        const obj = {
            idPaciente: idPaciente, // Usando o ID do paciente armazenado
            idCid: $("#selectCid").val(),
            inicioApac: $("#txtinicioApac").val(),
            fimApac: $("#txtfimApac").val(),
            observacao: $("#txtObservacao").val(),
            idStatusProcesso: $("#selectStatusProcesso").val(),
            idTipoProcesso: $("#selectTipoProcesso").val(),
            dataRenovacao: $("#txtdataRenovacao").val(),
            dataSuspensao: $("#txtdataSuspensao").val(),
            medicamento: medicamentos.map(medicamento => ({
                idMedicamento: medicamento.idMedicamento,
                nomeMedicamento: medicamento.nomeMedicamento,
                quantidade: medicamento.quantidade,
                dataEntrega: medicamento.dataEntrega,
                recibo: medicamento.recibo,
                receita: medicamento.receita,
                medicamentoChegou: medicamento.medicamentoChegou,
                medicamentoEntregue: medicamento.medicamentoEntregue
            }))
        };

        console.log("Objeto a ser enviado:", obj);

        $.ajax({
            type: "POST",
            url: urlAPI + "api/Dispensacao",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function () {
                limparFormulario();
                alert("Dados salvos com sucesso!");
                houveAlteracao = false;
            },
            error: function (jqXHR, textStatus) {
                alert("Erro ao salvar os dados: " + textStatus);
            }
        });
    }
});



function limparFormulario() {
    $("#selectPaciente").val('0');
    $("#txtinicioApac").val('');
    $("#txtfimApac").val('');
    $("#selectStatusProcesso").val('0');
    $("#selectTipoProcesso").val('0');
    $("#txtObservacao").val('');
    medicamentos = [];
    atualizarTabelaMedicamentos();
}
