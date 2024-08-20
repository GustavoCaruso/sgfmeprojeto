const urlAPI = "https://localhost:7034/";

let statusOptions = '';
let sexoOptions = '';
let corRacaOptions = '';
let profissaoOptions = '';
let estadoCivilOptions = '';
let ufOptions = '';
let tipoContatoOptions = '';
let tipoEnderecoOptions = '';
let contatos = [];
let enderecos = [];
let houveAlteracao = false;

let contatoEmEdicao = null;
let enderecoEmEdicao = null;

$(document).ready(async function () {
    await carregarDadosSelecoes();

    if ($("#tabela").length > 0) {
        carregarRepresentantes();
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

    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        console.log("Clique no botão alterar. Código:", codigo);
        window.location.href = "/RepresentanteCadastro?id=" + codigo;
    });

    $(document).on("change", ".alterar-status", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        let novoStatus = $(elemento.target).val();

        if (novoStatus === "0") {
            alert("Seleção inválida! Por favor, escolha um status válido.");
            // Reverte a seleção para o valor anterior
            $(elemento.target).val($(elemento.target).data('original-value'));
        } else {
            console.log("Mudança de status. Código:", codigo, "Novo Status:", novoStatus);
            mudarStatus(codigo, novoStatus);
        }
    });

    $(document).on("focus", ".alterar-status", function () {
        // Armazena o valor original antes da mudança
        $(this).data('original-value', $(this).val());
    });

    $("#selectNaturalidadeUf").change(function () {
        const ufSelecionada = $(this).val();
        console.log("UF Naturalidade selecionada:", ufSelecionada);
        if (ufSelecionada !== "0") {
            carregarMunicipios(ufSelecionada, $("#selectNaturalidadeCidade"));
        } else {
            $("#selectNaturalidadeCidade").empty().append('<option value="0">Selecione uma cidade</option>');
        }
    });

    $("#selectEstado").change(function () {
        const ufSelecionada = $(this).val();
        console.log("UF selecionada para endereço:", ufSelecionada);
        if (ufSelecionada !== "0") {
            carregarMunicipios(ufSelecionada, $("#selectMunicipio"));
        } else {
            $("#selectMunicipio").empty().append('<option value="0">Selecione uma cidade</option>');
        }
    });

    $("#txtdataNascimento").on("input", function () {
        const dataNascimento = new Date($(this).val());
        if (!isNaN(dataNascimento)) {
            const idade = calcularIdade(dataNascimento);
            console.log("Data de nascimento inserida:", dataNascimento, "Idade calculada:", idade);
            $("#txtidade").val(idade);
        } else {
            $("#txtidade").val('');
        }
    });

    $(document).off("click", ".btn-danger[data-type='contato']").on("click", ".btn-danger[data-type='contato']", function () {
        const index = $(this).data("index");
        console.log("Clique para excluir contato. Índice:", index);
        const confirmDelete = confirm("Você tem certeza que deseja excluir este contato?");
        if (confirmDelete) {
            console.log("Confirmação de exclusão de contato. Índice:", index);
            contatos.splice(index, 1);
            atualizarTabelaContatos();
        }
    });

    $(document).off("click", ".btn-danger[data-type='endereco']").on("click", ".btn-danger[data-type='endereco']", function () {
        const index = $(this).data("index");
        console.log("Clique para excluir endereço. Índice:", index);
        const confirmDelete = confirm("Você tem certeza que deseja excluir este endereço?");
        if (confirmDelete) {
            console.log("Confirmação de exclusão de endereço. Índice:", index);
            enderecos.splice(index, 1);
            atualizarTabelaEnderecos();
        } else {
            console.log("Exclusão de endereço cancelada.");
        }
    });

    $(".form-control").on("input change", function () {
        if (!houveAlteracao) {
            houveAlteracao = true;
        }
    });

    configurarMascaraCPF();
    configurarMascaraCEP();
});

function removerMascara(valor, tipo) {
    if (tipo === "Celular" || tipo === "Telefone Fixo" || tipo === "CEP") {
        return valor.replace(/\D/g, ''); // Remove todos os caracteres que não são dígitos
    }
    return valor;
}

function carregarDadosSelecoes() {
    return Promise.all([
        carregarOpcoesStatus(),
        carregarOpcoesSexo(),
        carregarOpcoesCorRaca(),
        carregarOpcoesProfissao(),
        carregarOpcoesEstadoCivil(),
        carregarEstados($("#selectNaturalidadeUf")),
        carregarEstados($("#selectEstado")),
        carregarEstados($("#selectRgUfEmissao")),
        carregarOpcoes("api/Representante/tipoContato", $("#selectTipoContato")),
        carregarOpcoes("api/Representante/tipoEndereco", $("#selectTipoEndereco")),
    ]);
}

function configurarMascaraCPF() {
    $("#txtcpfNumero").off("input").on("input", function () {
        let valor = $(this).val();
        valor = valor.replace(/\D/g, "");
        if (valor.length <= 11) {
            valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
            valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
            valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
        }
        $(this).val(valor);
    });
}

function configurarMascaraCEP() {
    $("#txtCep").off("input").on("input", function () {
        let valor = $(this).val();
        valor = valor.replace(/\D/g, "");
        valor = valor.replace(/^(\d{5})(\d)/, "$1-$2");
        $(this).val(valor);
    });
}

function carregarOpcoesStatus() {
    const cachedStatus = localStorage.getItem('statusOptions');
    if (cachedStatus) {
        statusOptions = cachedStatus;
        $("#selectStatus").html(statusOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Representante/tipoStatus",
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

function carregarOpcoesSexo() {
    const cachedSexo = localStorage.getItem('sexoOptions');
    if (cachedSexo) {
        sexoOptions = cachedSexo;
        $("#selectSexo").html(sexoOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Representante/tipoSexo",
        method: "GET",
        success: function (data) {
            sexoOptions = '<option value="0">Selecione um sexo</option>';
            data.forEach(item => {
                sexoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectSexo").html(sexoOptions);
            localStorage.setItem('sexoOptions', sexoOptions);
        },
        error: function () {
            alert("Erro ao carregar os tipos de sexo.");
        }
    });
}

function carregarOpcoesEstadoCivil() {
    const cachedEstadoCivil = localStorage.getItem('estadoCivilOptions');
    if (cachedEstadoCivil) {
        estadoCivilOptions = cachedEstadoCivil;
        $("#selectEstadoCivil").html(estadoCivilOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Representante/tipoEstadoCivil",
        method: "GET",
        success: function (data) {
            estadoCivilOptions = '<option value="0">Selecione um estado civil</option>';
            data.forEach(item => {
                estadoCivilOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectEstadoCivil").html(estadoCivilOptions);
            localStorage.setItem('estadoCivilOptions', estadoCivilOptions);
        },
        error: function () {
            alert("Erro ao carregar os estados civis.");
        }
    });
}

function carregarOpcoesCorRaca() {
    const cachedCorRaca = localStorage.getItem('corRacaOptions');
    if (cachedCorRaca) {
        corRacaOptions = cachedCorRaca;
        $("#selectCorRaca").html(corRacaOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Representante/tipoCorRaca",
        method: "GET",
        success: function (data) {
            corRacaOptions = '<option value="0">Selecione uma cor/raça</option>';
            data.forEach(item => {
                corRacaOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectCorRaca").html(corRacaOptions);
            localStorage.setItem('corRacaOptions', corRacaOptions);
        },
        error: function () {
            alert("Erro ao carregar as opções de cor/raça.");
        }
    });
}

function carregarOpcoesProfissao() {
    const cachedProfissao = localStorage.getItem('profissaoOptions');
    if (cachedProfissao) {
        profissaoOptions = cachedProfissao;
        $("#selectProfissao").html(profissaoOptions);
        return Promise.resolve();
    }

    return $.ajax({
        url: urlAPI + "api/Representante/tipoProfissao",
        method: "GET",
        success: function (data) {
            profissaoOptions = '<option value="0">Selecione uma profissão</option>';
            data.forEach(item => {
                profissaoOptions += `<option value="${item.id}">${item.nome}</option>`;
            });
            $("#selectProfissao").html(profissaoOptions);
            localStorage.setItem('profissaoOptions', profissaoOptions);
        },
        error: function () {
            alert("Erro ao carregar as profissões.");
        }
    });
}

function carregarOpcoes(apiEndpoint, selectElement) {
    return $.ajax({
        url: urlAPI + apiEndpoint,
        method: "GET",
        success: function (data) {
            const defaultOption = '<option value="0">Selecione uma opção</option>';
            selectElement.html(defaultOption); // Define a opção padrão primeiro
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

function carregarEstados(selectElement) {
    return $.ajax({
        url: "https://servicodados.ibge.gov.br/api/v1/localidades/estados",
        method: "GET",
        success: function (data) {
            selectElement.empty();
            selectElement.append('<option value="">Selecione uma UF</option>');
            data.forEach(estado => {
                const option = `<option value="${estado.sigla}">${estado.sigla}</option>`;
                selectElement.append(option);
            });
        },
        error: function () {
            alert("Erro ao carregar os estados.");
        }
    });
}

function carregarMunicipios(estadoSigla, selectElement, cidadeSelecionada = null) {
    if (estadoSigla) {
        return $.ajax({
            url: `https://servicodados.ibge.gov.br/api/v1/localidades/estados/${estadoSigla}/municipios`,
            method: "GET",
            success: function (data) {
                selectElement.empty();
                selectElement.append('<option value="">Selecione uma Cidade</option>');
                data.forEach(municipio => {
                    const option = `<option value="${municipio.nome}">${municipio.nome}</option>`;
                    selectElement.append(option);
                });
                if (cidadeSelecionada) {
                    selectElement.val(cidadeSelecionada);
                }
            },
            error: function () {
                alert("Erro ao carregar as Cidades.");
            }
        });
    } else {
        selectElement.empty().append('<option value="">Selecione uma cidade</option>');
    }
}

function carregarRepresentantes() {
    $("#loading").show();

    $.ajax({
        url: urlAPI + "api/Representante/dadosBasicos",
        method: "GET",
        success: function (data) {
            const tabela = $("#tabela");
            tabela.empty();

            const fragment = document.createDocumentFragment();

            $.each(data, function (index, item) {
                var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                $(linha).find(".codigo").html(item.id);
                $(linha).find(".nomeCompleto").html(item.nomeCompleto);
                $(linha).find(".dataNascimento").html(new Date(item.dataNascimento).toLocaleDateString());
                $(linha).find(".rg").html(item.rgNumero);
                $(linha).find(".cpf").html(item.cpfNumero);

                var statusSelect = $("<select>")
                    .addClass("form-select alterar-status")
                    .html(statusOptions)
                    .val(item.idStatus);
                $(linha).find(".status").html(statusSelect);

                fragment.appendChild(linha[0]);
            });

            tabela.append(fragment);

            $('#tabelaRepresentante').DataTable({
                language: {
                    url: '/js/pt-BR.json'
                },
                destroy: true
            });

            $("#loading").hide();
        },
        error: function () {
            alert("Erro ao carregar representantes.");
            $("#loading").hide();
        }
    });
}

function validarCampos() {
    let isValid = true;
    $(".form-control").removeClass('is-invalid');

    const camposObrigatorios = [
        "#txtnomeCompleto",
        "#txtdataNascimento",
        "#txtrgNumero",
        "#txtrgDataEmissao",
        "#txtrgOrgaoExpedidor",
        "#selectRgUfEmissao",
        "#txtcnsNumero",
        "#txtcpfNumero",
        "#txtnomeMae",
        "#selectNaturalidadeCidade",
        "#selectNaturalidadeUf",
        
        "#selectStatus",
        "#selectSexo",
        "#selectProfissao",
        "#selectCorRaca",
        "#selectEstadoCivil"
    ];

    camposObrigatorios.forEach(function (campo) {
        let valor = $(campo).val().trim();

        if (valor === "" || valor === "0") {
            $(campo).addClass('is-invalid');
            isValid = false;
        }
    });

    if (contatos.length === 0) {
        $("#mensagemValidacao").text("Por favor, adicione pelo menos um contato.");
        isValid = false;
    } else {
        $("#mensagemValidacao").text("");
    }

    if (enderecos.length === 0) {
        $("#mensagemValidacaoEndereco").text("Por favor, adicione pelo menos um endereço.");
        isValid = false;
    } else {
        $("#mensagemValidacaoEndereco").text("");
    }

    return isValid;
}

$("#btnsalvar").click(function () {
    if ($("#txtid").val() !== "0" && houveAlteracao) {
        const confirmSave = confirm("Você fez alterações no formulário. Deseja salvar as alterações?");
        if (!confirmSave) {
            return;
        }
    }

    if (validarCampos()) {
        const rgNumero = removerMascara($("#txtrgNumero").val(), "RG");
        const cnsNumero = removerMascara($("#txtcnsNumero").val(), "CNS");
        const cpfNumero = removerMascara($("#txtcpfNumero").val(), "CPF");

        enderecos = enderecos.map(endereco => ({
            ...endereco,
            cep: removerMascara(endereco.cep, "CEP")
        }));

        const obj = {
            id: $("#txtid").val(),
            nomeCompleto: $("#txtnomeCompleto").val(),
            dataNascimento: $("#txtdataNascimento").val(),
            rgNumero: rgNumero,
            rgDataEmissao: $("#txtrgDataEmissao").val(),
            rgOrgaoExpedidor: $("#txtrgOrgaoExpedidor").val(),
            rgUfEmissao: $("#selectRgUfEmissao").val(),
            cnsNumero: cnsNumero,
            cpfNumero: cpfNumero,
            nomeMae: $("#txtnomeMae").val(),
            nomeConjuge: $("#txtnomeConjuge").val(),
            naturalidadeCidade: $("#selectNaturalidadeCidade").val(),
            naturalidadeUf: $("#selectNaturalidadeUf").val(),
           
            dataCadastro: $("#txtdataCadastro").val(),
            idStatus: $("#selectStatus").val(),
            idSexo: $("#selectSexo").val(),
            idProfissao: $("#selectProfissao").val(),
            idCorRaca: $("#selectCorRaca").val(),
            idEstadoCivil: $("#selectEstadoCivil").val(),
            contato: contatos,
            endereco: enderecos
        };

        $.ajax({
            type: obj.id == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Representante" + (obj.id != "0" ? "/" + obj.id : ""),
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function () {
                limparFormulario();
                alert("Dados Salvos com sucesso!");

                if ($("#tabela").length > 0) {
                    carregarRepresentantes();
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

function limparFormulario() {
    $("#txtnomeCompleto").val('');
    $("#txtdataNascimento").val('');
    $("#txtrgNumero").val('');
    $("#txtrgDataEmissao").val('');
    $("#txtrgOrgaoExpedidor").val('');
    $("#selectRgUfEmissao").val('');
    $("#txtcnsNumero").val('');
    $("#txtcpfNumero").val('');
    $("#txtid").val('0');
    $("#txtdataCadastro").val(new Date().toISOString().split('T')[0]);
    $("#txtidade").val('');
    $("#txtnomeMae").val('');
    $("#txtnomeConjuge").val('');
    $("#selectNaturalidadeCidade").val('');
    $("#selectNaturalidadeUf").val('');
    

    $("#selectStatus").val("0");
    $("#selectSexo").val("0");
    $("#selectProfissao").val("0");
    $("#selectCorRaca").val("0");
    $("#selectEstadoCivil").val("0");

    contatos = [];
    enderecos = [];
    atualizarTabelaContatos();
    atualizarTabelaEnderecos();

    houveAlteracao = false;
}

function mudarStatus(codigo, novoStatus) {
    console.log("Alterando status para Representante:", codigo, "Novo Status:", novoStatus);
    $.ajax({
        type: "PATCH",
        url: urlAPI + "api/Representante/" + codigo + "/mudarStatus",
        contentType: "application/json",
        data: JSON.stringify(novoStatus),
        dataType: "json",
        success: function () {
            alert('Status alterado com sucesso!');
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log("Erro:", xhr.responseText);
            alert("Erro ao alterar o status do representante: " + xhr.responseText);
        }
    });
}

async function visualizar(codigo) {
    try {
        $("#loading").show();

        const representantePromise = $.ajax({
            type: "GET",
            url: urlAPI + "api/Representante/" + codigo + "/dadosCompletos",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
        });

        const estadosPromise = carregarEstados($("#selectRgUfEmissao"));

        const [jsonResult, estados] = await Promise.all([representantePromise, estadosPromise]);

        $("#txtid").val(jsonResult.id);
        $("#txtnomeCompleto").val(jsonResult.nomeCompleto);

        const dataNascimento = new Date(jsonResult.dataNascimento);
        $("#txtdataNascimento").val(dataNascimento.toISOString().split('T')[0]);

        $("#txtrgNumero").val(jsonResult.rgNumero);
        $("#txtrgDataEmissao").val(new Date(jsonResult.rgDataEmissao).toISOString().split('T')[0]);
        $("#txtrgOrgaoExpedidor").val(jsonResult.rgOrgaoExpedidor);
        $("#selectRgUfEmissao").val(jsonResult.rgUfEmissao);

        await carregarEstados($("#selectNaturalidadeUf")).then(() => {
            $("#selectNaturalidadeUf").val(jsonResult.naturalidadeUf);
            carregarMunicipios(jsonResult.naturalidadeUf, $("#selectNaturalidadeCidade"), jsonResult.naturalidadeCidade);
        });

        $("#txtcnsNumero").val(jsonResult.cnsNumero);
        $("#txtcpfNumero").val(jsonResult.cpfNumero);
        $("#txtdataCadastro").val(new Date(jsonResult.dataCadastro).toISOString().split('T')[0]);

        $("#selectStatus").val(jsonResult.idStatus);
        $("#selectSexo").val(jsonResult.idSexo);
        $("#selectProfissao").val(jsonResult.idProfissao);
        $("#selectCorRaca").val(jsonResult.idCorRaca);
        $("#selectEstadoCivil").val(jsonResult.idEstadoCivil);

        $("#txtnomeMae").val(jsonResult.nomeMae);
        $("#txtnomeConjuge").val(jsonResult.nomeConjuge);

       

        contatos = jsonResult.contato.map(c => ({
            idTipoContato: c.idTipoContato,
            tipo: c.tipocontato.nome,
            valor: c.valor
        }));
        atualizarTabelaContatos();

        enderecos = jsonResult.endereco.map(e => ({
            idTipoEndereco: e.idTipoEndereco,
            logradouro: e.logradouro,
            numero: e.numero,
            complemento: e.complemento,
            bairro: e.bairro,
            cidade: e.cidade,
            uf: e.uf,
            cep: e.cep,
            pontoReferencia: e.pontoReferencia
        }));
        atualizarTabelaEnderecos();

        const idade = calcularIdade(dataNascimento);
        $("#txtidade").val(idade);

    } catch (error) {
        alert("Erro ao carregar os dados: " + error.responseText);
    } finally {
        $("#loading").hide();
    }
}

function atualizarTabelaContatos() {
    const tabela = $("#contatoTable tbody");
    tabela.empty();

    contatos.forEach((contato, index) => {
        let valorFormatado = contato.valor;

        if (contato.tipo === "Celular") {
            valorFormatado = aplicarMascara(valorFormatado, "Celular");
        } else if (contato.tipo === "Telefone Fixo") {
            valorFormatado = aplicarMascara(valorFormatado, "Telefone Fixo");
        }

        const linha = `<tr>
            <td>${contato.tipo}</td>
            <td>${valorFormatado}</td>
            <td>
                <button type="button" class="btn btn-warning btn-edit" data-index="${index}" data-type="contato">Editar</button>
                <button type="button" class="btn btn-danger" data-index="${index}" data-type="contato">Excluir</button>
            </td>
        </tr>`;
        tabela.append(linha);
    });

    $(".btn-edit[data-type='contato']").off("click").on("click", function () {
        const index = $(this).data("index");
        editarContato(index);
    });
}

function editarContato(index) {
    if (contatoEmEdicao !== null) {
        contatos[contatoEmEdicao.index] = contatoEmEdicao;
    }

    contatoEmEdicao = { ...contatos[index], index };

    let valorComMascara = contatoEmEdicao.valor;
    if (contatoEmEdicao.tipo === "Celular") {
        valorComMascara = aplicarMascara(valorComMascara, "Celular");
    } else if (contatoEmEdicao.tipo === "Telefone Fixo") {
        valorComMascara = aplicarMascara(valorComMascara, "Telefone Fixo");
    }

    $("#selectTipoContato").val(contatoEmEdicao.idTipoContato);
    $("#txtValorContato").val(valorComMascara);
}

function atualizarTabelaEnderecos() {
    const tabela = $("#enderecoTable tbody");
    tabela.empty();

    enderecos.forEach((endereco, index) => {
        let tipoEnderecoNome = $("#selectTipoEndereco option[value='" + endereco.idTipoEndereco + "']").text();
        let cepFormatado = aplicarMascara(endereco.cep, "CEP");

        const linha = `<tr>
            <td>${tipoEnderecoNome}</td>
            <td>${endereco.logradouro}</td>
            <td>${endereco.numero}</td>
            <td>${endereco.complemento}</td>
            <td>${endereco.bairro}</td>
            <td>${endereco.cidade}</td>
            <td>${endereco.uf}</td>
            <td>${cepFormatado}</td>
            <td>${endereco.pontoReferencia}</td>
            <td>
                <button type="button" class="btn btn-warning btn-edit" data-index="${index}" data-type="endereco">Editar</button>
                <button type="button" class="btn btn-danger" data-index="${index}" data-type="endereco">Excluir</button>
            </td>
        </tr>`;
        tabela.append(linha);
    });

    $(document).off("click", ".btn-edit[data-type='endereco']");
    $(document).off("click", ".btn-danger[data-type='endereco']");

    $(document).on("click", ".btn-edit[data-type='endereco']", function () {
        const index = $(this).data("index");
        editarEndereco(index);
    });

    $(document).on("click", ".btn-danger[data-type='endereco']", function () {
        const index = $(this).data("index");
        const confirmDelete = confirm("Você tem certeza que deseja excluir este endereço?");
        if (confirmDelete) {
            enderecos.splice(index, 1);
            atualizarTabelaEnderecos();
        }
    });
}

function salvarEnderecoEmEdicao() {
    if (enderecoEmEdicao !== null) {
        const enderecoAtualizado = {
            idTipoEndereco: $("#selectTipoEndereco").val(),
            logradouro: $("#txtLogradouro").val(),
            numero: $("#txtNumero").val(),
            complemento: $("#txtComplemento").val(),
            bairro: $("#txtBairro").val(),
            cidade: $("#selectMunicipio option:selected").text(),
            uf: $("#selectEstado option:selected").text(),
            cep: removerMascara($("#txtCep").val(), "CEP"),
            pontoReferencia: $("#txtPontoReferencia").val()
        };

        if (!enderecoAtualizado.idTipoEndereco || !enderecoAtualizado.logradouro || !enderecoAtualizado.numero ||
            !enderecoAtualizado.bairro || !enderecoAtualizado.cidade || !enderecoAtualizado.uf || !enderecoAtualizado.cep) {
            alert("Por favor, preencha todos os campos obrigatórios do endereço.");
            return false;
        }

        enderecos[enderecoEmEdicao] = enderecoAtualizado;
        enderecoEmEdicao = null;

        atualizarTabelaEnderecos();
        limparCamposEndereco();
    }
    return true;
}

function editarEndereco(index) {
    enderecoEmEdicao = index;

    let endereco = enderecos[index];
    let cepComMascara = aplicarMascara(endereco.cep, "CEP");

    $("#selectTipoEndereco").val(endereco.idTipoEndereco);
    $("#txtLogradouro").val(endereco.logradouro);
    $("#txtNumero").val(endereco.numero);
    $("#txtComplemento").val(endereco.complemento);
    $("#txtBairro").val(endereco.bairro);
    $("#selectEstado").val(endereco.uf);
    carregarMunicipios(endereco.uf, $("#selectMunicipio"), endereco.cidade);
    $("#txtCep").val(cepComMascara);
    $("#txtPontoReferencia").val(endereco.pontoReferencia);
}

function calcularIdade(dataNascimento) {
    const hoje = new Date();
    let idade = hoje.getFullYear() - dataNascimento.getFullYear();
    const m = hoje.getMonth() - dataNascimento.getMonth();

    if (m < 0 || (m === 0 && hoje.getDate() < dataNascimento.getDate())) {
        idade--;
    }
    return idade;
}

$("#selectTipoContato").change(function () {
    const tipoContato = $("#selectTipoContato option:selected").text();
    const inputContato = $("#txtValorContato");

    inputContato.off("input");
    inputContato.val('');

    if (tipoContato === "Celular") {
        inputContato.attr("maxlength", 11);
        inputContato.on("input", function () {
            this.value = this.value.replace(/\D/g, '');
            this.value = this.value.slice(0, 11);
            this.value = aplicarMascara(this.value, "Celular");
        });
    } else if (tipoContato === "Telefone Fixo") {
        inputContato.attr("maxlength", 10);
        inputContato.on("input", function () {
            this.value = this.value.replace(/\D/g, '');
            this.value = this.value.slice(0, 10);
            this.value = aplicarMascara(this.value, "Telefone Fixo");
        });
    } else if (tipoContato === "E-mail") {
        inputContato.attr("maxlength", 100);
        inputContato.on("input", function () {
            const email = this.value;
            if (!email.includes("@")) {
                this.setCustomValidity("E-mail inválido");
            } else {
                this.setCustomValidity("");
            }
        });
    } else {
        inputContato.removeAttr("maxlength");
    }
});

function aplicarMascara(valor, tipo) {
    if (tipo === "Celular") {
        return valor.replace(/^(\d{2})(\d{5})(\d{4})$/, "($1) $2-$3");
    } else if (tipo === "Telefone Fixo") {
        return valor.replace(/^(\d{2})(\d{4})(\d{4})$/, "($1) $2-$3");
    } else if (tipo === "CEP") {
        return valor.replace(/^(\d{5})(\d{3})$/, "$1-$2");
    }
    return valor;
}

$("#btnAdicionarContato").click(function () {
    const idTipoContato = $("#selectTipoContato").val();
    const tipoContato = $("#selectTipoContato option:selected").text();
    let valorContato = $("#txtValorContato").val();

    if (idTipoContato === "" || idTipoContato === null || tipoContato === "Selecione um Tipo de Contato") {
        alert("Por favor, selecione um tipo de contato válido.");
        return;
    }

    valorContato = removerMascara(valorContato, tipoContato);

    if (contatoEmEdicao !== null) {
        const isSameAsOld = (contatos[contatoEmEdicao.index].idTipoContato === idTipoContato &&
            contatos[contatoEmEdicao.index].valor === valorContato);

        if (!isSameAsOld) {
            const contatoDuplicado = contatos.some(contato =>
                contato.idTipoContato === idTipoContato && contato.valor === valorContato
            );

            if (contatoDuplicado) {
                alert("Este contato já foi adicionado.");
                return;
            }
        }

        contatos[contatoEmEdicao.index] = {
            idTipoContato: idTipoContato,
            tipo: tipoContato,
            valor: valorContato
        };
        contatoEmEdicao = null;
    } else {
        const contatoDuplicado = contatos.some(contato =>
            contato.idTipoContato === idTipoContato && contato.valor === valorContato
        );

        if (contatoDuplicado) {
            alert("Este contato já foi adicionado.");
            return;
        }

        if (contatos.length >= 3) {
            alert("Você pode adicionar no máximo 3 contatos.");
            return;
        }

        contatos.push({ idTipoContato: idTipoContato, tipo: tipoContato, valor: valorContato });
    }

    atualizarTabelaContatos();
    $("#txtValorContato").val('');
});

$("#btnAdicionarEndereco").click(function () {
    const idTipoEndereco = $("#selectTipoEndereco").val();
    const logradouro = $("#txtLogradouro").val();
    const numero = $("#txtNumero").val();
    const complemento = $("#txtComplemento").val();
    const bairro = $("#txtBairro").val();
    const cidade = $("#selectMunicipio option:selected").text();
    const uf = $("#selectEstado option:selected").text();
    const cep = removerMascara($("#txtCep").val(), "CEP");
    const pontoReferencia = $("#txtPontoReferencia").val();

    if (!idTipoEndereco || idTipoEndereco === "0") {
        alert("Por favor, selecione um tipo de endereço válido.");
        return;
    }

    if (!logradouro || !numero || !bairro || !cidade || !uf || !cep) {
        alert("Por favor, preencha todos os campos obrigatórios do endereço.");
        return;
    }

    const novoEndereco = {
        idTipoEndereco,
        logradouro,
        numero,
        complemento,
        bairro,
        cidade,
        uf,
        cep,
        pontoReferencia
    };

    if (enderecoEmEdicao !== null) {
        enderecos[enderecoEmEdicao] = novoEndereco;
        enderecoEmEdicao = null;
    } else {
        enderecos.push(novoEndereco);
    }

    atualizarTabelaEnderecos();
    limparCamposEndereco();

    $("#selectTipoEndereco").val('0');
});

function limparCamposEndereco() {
    $("#selectTipoEndereco").val('0');
    $("#txtLogradouro").val('');
    $("#txtNumero").val('');
    $("#txtComplemento").val('');
    $("#txtBairro").val('');
    $("#selectMunicipio").val('');
    $("#selectEstado").val('');
    $("#txtCep").val('');
    $("#txtPontoReferencia").val('');
    enderecoEmEdicao = null;
}

$("#txtcpfNumero").on("input", function () {
    let valor = $(this).val();

    valor = valor.replace(/\D/g, "");

    if (valor.length <= 11) {
        valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
        valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
        valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    }

    $(this).val(valor);
});

$("#txtrgNumero").on("input", function () {
    let valor = $(this).val();
    valor = valor.replace(/[^a-zA-Z0-9]/g, "");
    valor = valor.replace(/(\d{2})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\w{1,2})$/, "$1-$2");
    $(this).val(valor);
});

$("#txtcnsNumero").on("input", function () {
    let valor = $(this).val();
    valor = valor.replace(/\D/g, "");
    valor = valor.replace(/(\d{3})(\d)/, "$1 $2");
    valor = valor.replace(/(\d{4})(\d)/, "$1 $2");
    valor = valor.replace(/(\d{4})(\d)/, "$1 $2");
    $(this).val(valor);
});

$("#txtCep").on("input", function () {
    let valor = $(this).val();
    valor = valor.replace(/\D/g, "");
    valor = valor.replace(/^(\d{5})(\d)/, "$1-$2");
    $(this).val(valor);
});





$("#txtNumero").on("input", function () {
    let valor = $(this).val();

    valor = valor.replace(/\D/g, "");

    if (valor > 9999999999) {
        valor = 9999999999;
    }

    $(this).val(valor);
});

$("#txtNumero").on("blur", function () {
    let valor = $(this).val();

    if (valor === "" || valor === "0") {
        valor = 1;
    }

    $(this).val(valor);
});
