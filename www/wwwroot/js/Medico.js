const urlAPI = "https://localhost:7034/";

$(document).ready(function () {
    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    let contatos = [];
    let enderecos = [];
    let medicoDados;

    if ($("#tabela").length > 0) {
        carregarMedicos();
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
        window.location.href = "/MedicoCadastro?id=" + codigo;
    });

    $(document).on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtnomeCompleto").val().trim() || $("#txtnomeCompleto").val().length > 100) {
            $("#txtnomeCompleto").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtdataNascimento").val().trim()) {
            $("#txtdataNascimento").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtcrm").val().trim()) {
            $("#txtcrm").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtnomeMae").val().trim() || $("#txtnomeMae").val().length > 100) {
            $("#txtnomeMae").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtrgNumero").val().trim() || $("#txtrgNumero").val().length !== 9) {
            $("#txtrgNumero").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtrgDataEmissao").val().trim()) {
            $("#txtrgDataEmissao").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtrgOrgaoExpedidor").val().trim()) {
            $("#txtrgOrgaoExpedidor").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectRgUfEmissao").val().trim() || $("#selectRgUfEmissao").val() === "0") {
            $("#selectRgUfEmissao").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtcnsNumero").val().trim() || $("#txtcnsNumero").val().length !== 15) {
            $("#txtcnsNumero").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtcpfNumero").val().trim() || $("#txtcpfNumero").val().length !== 11) {
            $("#txtcpfNumero").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectStatus").val().trim() || $("#selectStatus").val() === "0") {
            $("#selectStatus").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectSexo").val().trim() || $("#selectSexo").val() === "0") {
            $("#selectSexo").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectCorRaca").val().trim() || $("#selectCorRaca").val() === "0") {
            $("#selectCorRaca").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectEstadoCivil").val().trim() || $("#selectEstadoCivil").val() === "0") {
            $("#selectEstadoCivil").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectNaturalidadeUf").val().trim() || $("#selectNaturalidadeUf").val() === "0") {
            $("#selectNaturalidadeUf").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#selectNaturalidadeCidade").val().trim() || $("#selectNaturalidadeCidade").val() === "0") {
            $("#selectNaturalidadeCidade").addClass('is-invalid');
            isValid = false;
        }

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

    $(".form-control").on("input", function () {
        $(this).removeClass('is-invalid');
    });

    function carregarEstados(selectElement) {
        return $.ajax({
            url: "https://servicodados.ibge.gov.br/api/v1/localidades/estados",
            method: "GET",
            success: function (data) {
                selectElement.empty();
                selectElement.append('<option value="">Selecione um estado</option>');
                data.sort((a, b) => a.sigla.localeCompare(b.sigla));
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
                    selectElement.append('<option value="">Selecione um município</option>');
                    data.forEach(municipio => {
                        const option = `<option value="${municipio.nome}">${municipio.nome}</option>`;
                        selectElement.append(option);
                    });
                    if (cidadeSelecionada) {
                        selectElement.val(cidadeSelecionada);
                    }
                },
                error: function () {
                    alert("Erro ao carregar os municípios.");
                }
            });
        } else {
            selectElement.empty();
        }
    }

    carregarEstados($("#selectNaturalidadeUf"));
    $("#selectNaturalidadeUf").change(function () {
        carregarMunicipios($(this).val(), $("#selectNaturalidadeCidade"));
    });

    carregarEstados($("#selectRgUfEmissao"));
    carregarEstados($("#selectEstado"));

    $("#selectEstado").change(function () {
        carregarMunicipios($(this).val(), $("#selectMunicipio"));
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

    carregarOpcoes("api/Medico/tipoContato", $("#selectTipoContato"), "Selecione um tipo de contato");
    carregarOpcoes("api/Medico/tipoEndereco", $("#selectTipoEndereco"), "Selecione um tipo de endereço");
    carregarOpcoes("api/Medico/tipoStatus", $("#selectStatus"), "Selecione um status");
    carregarOpcoes("api/Medico/tipoSexo", $("#selectSexo"), "Selecione um sexo");
    carregarOpcoes("api/Medico/tipoCorRaca", $("#selectCorRaca"), "Selecione uma cor/raça");
    carregarOpcoes("api/Medico/tipoEstadoCivil", $("#selectEstadoCivil"), "Selecione um estado civil");

    $("#btnAdicionarContato").click(function () {
        const tipoContato = $("#selectTipoContato option:selected").text();
        const valorContato = $("#txtValorContato").val();
        const idTipoContato = $("#selectTipoContato").val();

        if (!idTipoContato || idTipoContato === "0") {
            alert("Por favor, selecione um tipo de contato.");
            return;
        }

        if (!valorContato.trim()) {
            alert("Por favor, insira um valor de contato válido.");
            return;
        }

        if (valorContato && tipoContato) {
            contatos.push({ idTipoContato: idTipoContato, tipo: tipoContato, valor: valorContato });
            atualizarTabelaContatos();
            $("#txtValorContato").val('');
        } else {
            alert("Por favor, selecione um tipo de contato e insira um valor.");
        }
    });

    $("#btnAdicionarEndereco").click(function () {
        const idTipoEndereco = $("#selectTipoEndereco").val();
        const tipoEndereco = $("#selectTipoEndereco option:selected").text();
        const logradouro = $("#txtLogradouro").val();
        const numero = $("#txtNumero").val();
        const complemento = $("#txtComplemento").val();
        const bairro = $("#txtBairro").val();
        const cidade = $("#selectMunicipio option:selected").text();
        const uf = $("#selectEstado option:selected").text();
        const cep = $("#txtCep").val();
        const pontoReferencia = $("#txtPontoReferencia").val();

        if (!idTipoEndereco || idTipoEndereco === "0") {
            alert("Por favor, selecione um tipo de endereço.");
            return;
        }

        if (!logradouro.trim() || logradouro.length > 100) {
            alert("Por favor, insira um logradouro válido (máximo 100 caracteres).");
            return;
        }

        if (!numero.trim() || numero.length > 10) {
            alert("Por favor, insira um número válido (máximo 10 caracteres).");
            return;
        }

        if (complemento && complemento.length > 30) {
            alert("O complemento deve ter no máximo 30 caracteres.");
            return;
        }

        if (!bairro.trim() || bairro.length > 70) {
            alert("Por favor, insira um bairro válido (máximo 70 caracteres).");
            return;
        }

        if (!cidade.trim() || cidade === "0") {
            alert("Por favor, selecione uma cidade.");
            return;
        }

        if (!uf.trim() || uf === "0") {
            alert("Por favor, selecione um estado.");
            return;
        }

        if (!cep.trim() || cep.length !== 8) {
            alert("Por favor, insira um CEP válido (8 caracteres).");
            return;
        }

        if (pontoReferencia && pontoReferencia.length > 100) {
            alert("O ponto de referência deve ter no máximo 100 caracteres.");
            return;
        }

        enderecos.push({ idTipoEndereco, tipo: tipoEndereco, logradouro, numero, complemento, bairro, cidade, uf, cep, pontoReferencia });
        atualizarTabelaEnderecos();
        $("#txtLogradouro").val('');
        $("#txtNumero").val('');
        $("#txtComplemento").val('');
        $("#txtBairro").val('');
        $("#selectMunicipio").val('');
        $("#selectEstado").val('');
        $("#txtCep").val('');
        $("#txtPontoReferencia").val('');
    });

    function atualizarTabelaContatos() {
        const tabela = $("#contatoTable tbody");
        tabela.empty();

        contatos.forEach((contato, index) => {
            const linha = `<tr>
                <td>${contato.tipo}</td>
                <td>${contato.valor}</td>
                <td><button type="button" class="btn btn-danger excluir-contato" data-index="${index}">Excluir</button></td>
            </tr>`;
            tabela.append(linha);
        });

        $(".excluir-contato").click(function () {
            const index = $(this).data("index");
            if (confirm("Tem certeza de que deseja excluir este contato?")) {
                contatos.splice(index, 1);
                atualizarTabelaContatos();
            }
        });
    }

    function atualizarTabelaEnderecos() {
        const tabela = $("#enderecoTable tbody");
        tabela.empty();

        enderecos.forEach((endereco, index) => {
            const linha = `<tr>
                <td>${endereco.tipo}</td>
                <td>${endereco.logradouro}</td>
                <td>${endereco.numero}</td>
                <td>${endereco.complemento}</td>
                <td>${endereco.bairro}</td>
                <td>${endereco.cidade}</td>
                <td>${endereco.uf}</td>
                <td>${endereco.cep}</td>
                <td>${endereco.pontoReferencia}</td>
                <td><button type="button" class="btn btn-danger excluir-endereco" data-index="${index}">Excluir</button></td>
            </tr>`;
            tabela.append(linha);
        });

        $(".excluir-endereco").click(function () {
            const index = $(this).data("index");
            if (confirm("Tem certeza de que deseja excluir este endereço?")) {
                enderecos.splice(index, 1);
                atualizarTabelaEnderecos();
            }
        });
    }

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                nomeCompleto: $("#txtnomeCompleto").val(),
                dataNascimento: $("#txtdataNascimento").val(),
                crm: $("#txtcrm").val(),
                dataCadastro: $("#txtdataCadastro").val(),
                idStatus: $("#selectStatus").val(),
                idSexo: $("#selectSexo").val(),
                idCorRaca: $("#selectCorRaca").val(),
                idEstadoCivil: $("#selectEstadoCivil").val(),
                nomeConjuge: $("#txtnomeConjuge").val(),
                naturalidadeUf: $("#selectNaturalidadeUf").val(),
                naturalidadeCidade: $("#selectNaturalidadeCidade").val(),
                rgNumero: $("#txtrgNumero").val(),
                rgDataEmissao: $("#txtrgDataEmissao").val(),
                rgOrgaoExpedidor: $("#txtrgOrgaoExpedidor").val(),
                rgUfEmissao: $("#selectRgUfEmissao").val(),
                cnsNumero: $("#txtcnsNumero").val(),
                cpfNumero: $("#txtcpfNumero").val(),
                nomeMae: $("#txtnomeMae").val(),
                contato: contatos,
                endereco: enderecos
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

                    if ($("#tabela").length > 0) {
                        carregarMedicos();
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
        $("#txtnomeCompleto").val('');
        $("#txtdataNascimento").val('');
        $("#txtcrm").val('');
        $("#txtdataCadastro").val(new Date().toISOString().split('T')[0]);
        $("#selectStatus").val('');
        $("#selectSexo").val('');
        $("#selectCorRaca").val('');
        $("#selectEstadoCivil").val('');
        $("#txtnomeConjuge").val('');
        $("#selectNaturalidadeUf").val('');
        $("#selectNaturalidadeCidade").val('');
        $("#txtrgNumero").val('');
        $("#txtrgDataEmissao").val('');
        $("#txtrgOrgaoExpedidor").val('');
        $("#selectRgUfEmissao").val('');
        $("#txtcnsNumero").val('');
        $("#txtcpfNumero").val('');
        $("#txtnomeMae").val('');
        contatos = [];
        enderecos = [];
        atualizarTabelaContatos();
        atualizarTabelaEnderecos();
    }

    function carregarMedicos() {
        $.ajax({
            url: urlAPI + "api/Medico/todosMedicosComContatosEEnderecos",
            method: "GET",
            success: function (data) {
                $("#tabela").empty();
                $.each(data, function (index, item) {
                    var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                    $(linha).find(".codigo").html(item.id);
                    $(linha).find(".nomeCompleto").html(item.nomeCompleto);
                    $(linha).find(".crm").html(item.crm);
                    $(linha).find(".dataCadastro").html(new Date(item.dataCadastro).toLocaleDateString());
                    $(linha).find(".status").html(item.status ? item.status.nome : "Não Definido");

                    var contatosHTML = item.contato.map(c => {
                        var tipoContatoNome = c.tipocontato ? c.tipocontato.nome : "Tipo de Contato Desconhecido";
                        return `${tipoContatoNome}: ${c.valor}`;
                    }).join("<br>");
                    $(linha).find(".contatos").html(contatosHTML);

                    var enderecosHTML = item.endereco.map(e => {
                        var tipoEnderecoNome = e.tipoendereco ? e.tipoendereco.nome : "Tipo de Endereço Desconhecido";
                        return `${tipoEnderecoNome}: ${e.logradouro}, ${e.numero}, ${e.complemento}, ${e.bairro}, ${e.cidade}, ${e.uf}, ${e.cep}, ${e.pontoReferencia}`;
                    }).join("<br>");
                    $(linha).find(".enderecos").html(enderecosHTML);

                    $(linha).show();
                    $("#tabela").append(linha);
                });

                $('#tabelaMedico').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar médicos.");
            }
        });
    }

    function excluir(codigo) {
        if (confirm("Tem certeza de que deseja excluir este médico?")) {
            $.ajax({
                type: "DELETE",
                url: urlAPI + "api/Medico/" + codigo,
                contentType: "application/json;charset=utf-8",
                success: function () {
                    alert('Exclusão efetuada!');
                    location.reload();
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("Erro ao excluir o médico: " + errorThrown);
                }
            });
        }
    }

    function visualizar(codigo) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Medico/" + codigo,
            contentType: "application/json;charset=utf-8",
            data: {},
            dataType: "json",
            success: function (jsonResult) {
                medicoDados = jsonResult;
                $("#txtid").val(jsonResult.id);
                $("#txtnomeCompleto").val(jsonResult.nomeCompleto);
                $("#txtdataNascimento").val(jsonResult.dataNascimento.split('T')[0]);
                $("#txtcrm").val(jsonResult.crm);
                $("#txtdataCadastro").val(new Date(jsonResult.dataCadastro).toISOString().split('T')[0]);
                $("#selectStatus").val(jsonResult.idStatus);
                $("#selectSexo").val(jsonResult.idSexo);
                $("#selectCorRaca").val(jsonResult.idCorRaca);
                $("#selectEstadoCivil").val(jsonResult.idEstadoCivil);
                $("#txtnomeConjuge").val(jsonResult.nomeConjuge);
                $("#selectNaturalidadeUf").val(jsonResult.naturalidadeUf);
                carregarMunicipios(jsonResult.naturalidadeUf, $("#selectNaturalidadeCidade"), jsonResult.naturalidadeCidade);
                $("#txtrgNumero").val(jsonResult.rgNumero);
                $("#txtrgDataEmissao").val(jsonResult.rgDataEmissao.split('T')[0]);
                $("#txtrgOrgaoExpedidor").val(jsonResult.rgOrgaoExpedidor);
                $("#selectRgUfEmissao").val(jsonResult.rgUfEmissao);
                $("#txtcnsNumero").val(jsonResult.cnsNumero);
                $("#txtcpfNumero").val(jsonResult.cpfNumero);
                $("#txtnomeMae").val(jsonResult.nomeMae);

                contatos = jsonResult.contato.map(c => ({
                    idTipoContato: c.idTipoContato,
                    tipo: c.tipocontato.nome,
                    valor: c.valor
                }));
                atualizarTabelaContatos();

                enderecos = jsonResult.endereco.map(e => ({
                    idTipoEndereco: e.idTipoEndereco,
                    tipo: e.tipoendereco.nome,
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
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }

    function calcularIdade(dataNascimento) {
        let hoje = new Date();
        let nascimento = new Date(dataNascimento);
        let idade = hoje.getFullYear() - nascimento.getFullYear();
        let m = hoje.getMonth() - nascimento.getMonth();

        if (m < 0 || (m === 0 && hoje.getDate() < nascimento.getDate())) {
            idade--;
        }
        return idade;
    }

    $("#txtdataNascimento").on("change", function () {
        let idade = calcularIdade($(this).val());
        $("#txtidade").val(idade);
    });

    if ($("#txtdataNascimento").val()) {
        let idade = calcularIdade($("#txtdataNascimento").val());
        $("#txtidade").val(idade);
    }

    carregarMedicos();
});
