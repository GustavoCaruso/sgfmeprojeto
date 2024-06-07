const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
    // Apenas números nos campos CPF, CNS e RG
    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    let contatos = [];
    let enderecos = [];
    let pacienteDados;
    let tiposEndereco = {};

    // Adicionar "Selecione uma opção" aos selects
    const defaultOption = '<option value="">Selecione uma opção</option>';

    if ($("#tabela").length > 0) {
        carregarPacientes();
    } else if ($("#txtid").length > 0) {
        let params = new URLSearchParams(window.location.search);
        let id = params.get('id');
        if (id) {
            visualizar(id);
        } else {
            // Alimenta o campo dataCadastro com a data atual
            let dataAtual = new Date().toISOString().split('T')[0];
            $("#txtdataCadastro").val(dataAtual);
        }
    }

    $("#txtdataNascimento").change(function () {
        let dataNascimento = new Date($(this).val());
        let idade = calcularIdade(dataNascimento);
        $("#txtidade").val(idade);
    });

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/PacienteCadastro?id=" + codigo;
    });

    $(document).on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        const campos = [
            { id: "#txtnomeCompleto", maxLength: 100 },
            { id: "#txtdataNascimento" },
            { id: "#selectStatus" },
            { id: "#selectSexo" },
            { id: "#selectCorRaca" },
            { id: "#selectEstadoCivil" },
            { id: "#txtnomeConjuge", maxLength: 100, optional: true },
            { id: "#selectNaturalidadeUf" },
            { id: "#selectNaturalidadeCidade" },
            { id: "#txtrgNumero", maxLength: 9 },
            { id: "#txtrgDataEmissao" },
            { id: "#txtrgOrgaoExpedidor" },
            { id: "#selectRgUfEmissao" },
            { id: "#txtcnsNumero", maxLength: 15 },
            { id: "#txtcpfNumero", maxLength: 11 },
            { id: "#selectProfissao" },
            { id: "#txtnomeMae", maxLength: 100 },
            { id: "#txtpeso", maxLength: 5, optional: true },
            { id: "#txtaltura", maxLength: 3, optional: true }
        ];

        campos.forEach(campo => {
            const element = $(campo.id);
            if (element.length === 0) return;

            const value = element.val().trim();
            if ((value.length === 0 && !campo.optional) || (campo.maxLength && value.length > campo.maxLength)) {
                element.addClass('is-invalid');
                isValid = false;
            }
        });

        // Validação específica para "Naturalidade - Cidade"
        const naturalidadeUf = $("#selectNaturalidadeUf").val().trim();
        const naturalidadeCidade = $("#selectNaturalidadeCidade").val().trim();
        if (naturalidadeUf && naturalidadeCidade === "") {
            $("#selectNaturalidadeCidade").addClass('is-invalid');
            isValid = false;
        }

        if (contatos.length === 0) {
            $("#mensagemValidacao").text("Por favor, adicione pelo menos um contato.");
            isValid = false;
        } else {
            $("#mensagemValidacao").text(""); // Limpa a mensagem se a validação for bem-sucedida
        }

        if (enderecos.length === 0) {
            $("#mensagemValidacaoEndereco").text("Por favor, adicione pelo menos um endereço.");
            isValid = false;
        } else {
            $("#mensagemValidacaoEndereco").text(""); // Limpa a mensagem se a validação for bem-sucedida
        }

        return isValid;
    }

    // Função para carregar os estados do IBGE
    function carregarEstados(selectElement) {
        return $.ajax({
            url: "https://servicodados.ibge.gov.br/api/v1/localidades/estados",
            method: "GET",
            success: function (data) {
                selectElement.empty().append(defaultOption);
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
                    selectElement.empty().append(defaultOption);
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
            selectElement.empty().append(defaultOption);
        }
    }

    carregarEstados($("#selectEstado"));
    carregarEstados($("#selectNaturalidadeUf"));
    carregarEstados($("#selectRgUfEmissao"));

    $("#selectEstado").change(function () {
        carregarMunicipios($(this).val(), $("#selectMunicipio"));
    });

    $("#selectNaturalidadeUf").change(function () {
        carregarMunicipios($(this).val(), $("#selectNaturalidadeCidade"));
    });

    function carregarOpcoes(apiEndpoint, selectElement) {
        $.ajax({
            url: urlAPI + apiEndpoint,
            method: "GET",
            success: function (data) {
                selectElement.empty().append(defaultOption);
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

    function carregarOpcoesEndereco(apiEndpoint, selectElement) {
        $.ajax({
            url: urlAPI + apiEndpoint,
            method: "GET",
            success: function (data) {
                selectElement.empty().append(defaultOption);
                data.forEach(item => {
                    tiposEndereco[item.id] = item.nome;
                    const option = `<option value="${item.id}">${item.nome}</option>`;
                    selectElement.append(option);
                });
            },
            error: function () {
                alert("Erro ao carregar os dados.");
            }
        });
    }

    carregarOpcoes("api/Paciente/tipoContato", $("#selectTipoContato"));
    carregarOpcoesEndereco("api/Paciente/tipoEndereco", $("#selectTipoEndereco"));
    carregarOpcoes("api/Paciente/tipoStatus", $("#selectStatus"));
    carregarOpcoes("api/Paciente/tipoSexo", $("#selectSexo"));
    carregarOpcoes("api/Paciente/tipoProfissao", $("#selectProfissao"));
    carregarOpcoes("api/Paciente/tipoCorRaca", $("#selectCorRaca"));
    carregarOpcoes("api/Paciente/tipoEstadoCivil", $("#selectEstadoCivil"));

    $("#btnAdicionarContato").click(function () {
        const tipoContato = $("#selectTipoContato option:selected").text();
        const valorContato = $("#txtValorContato").val();
        const idTipoContato = $("#selectTipoContato").val();

        if (idTipoContato && valorContato.trim() && valorContato.length <= 100) {
            contatos.push({ idTipoContato: idTipoContato, tipo: tipoContato, valor: valorContato });
            atualizarTabelaContatos();
            $("#txtValorContato").val('');
        } else {
            alert("Por favor, selecione um tipo de contato e insira um valor válido (máximo 100 caracteres).");
        }
    });

    $("#btnAdicionarEndereco").click(function () {
        const idTipoEndereco = $("#selectTipoEndereco").val();
        const logradouro = $("#txtLogradouro").val();
        const numero = $("#txtNumero").val();
        const complemento = $("#txtComplemento").val();
        const bairro = $("#txtBairro").val();
        const cidade = $("#selectMunicipio option:selected").text();
        const uf = $("#selectEstado option:selected").text();
        const cep = $("#txtCep").val();
        const pontoReferencia = $("#txtPontoReferencia").val();

        if (idTipoEndereco && logradouro.trim() && logradouro.length <= 100 &&
            numero.trim() && numero.length <= 10 &&
            /^[A-Za-z ]*$/.test(complemento) && complemento.length <= 30 &&
            bairro.trim() && bairro.length <= 70 &&
            cidade && uf &&
            cep.trim().length === 8 && !isNaN(cep) &&
            pontoReferencia.length <= 100) {

            enderecos.push({ idTipoEndereco, logradouro, numero, complemento, bairro, cidade, uf, cep, pontoReferencia });
            atualizarTabelaEnderecos();
            $("#txtLogradouro").val('');
            $("#txtNumero").val('');
            $("#txtComplemento").val('');
            $("#txtBairro").val('');
            $("#selectMunicipio").val('');
            $("#selectEstado").val('');
            $("#txtCep").val('');
            $("#txtPontoReferencia").val('');
        } else {
            alert("Por favor, preencha todos os campos obrigatórios do endereço corretamente.");
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

    function atualizarTabelaEnderecos() {
        const tabela = $("#enderecoTable tbody");
        tabela.empty();

        enderecos.forEach((endereco, index) => {
            const linha = `<tr>
                <td>${tiposEndereco[endereco.idTipoEndereco]}</td>
                <td>${endereco.logradouro}</td>
                <td>${endereco.numero}</td>
                <td>${endereco.complemento}</td>
                <td>${endereco.bairro}</td>
                <td>${endereco.cidade}</td>
                <td>${endereco.uf}</td>
                <td>${endereco.cep}</td>
                <td>${endereco.pontoReferencia}</td>
                <td><button type="button" class="btn btn-danger" data-index="${index}">Excluir</button></td>
            </tr>`;
            tabela.append(linha);
        });

        $(".btn-danger").click(function () {
            const index = $(this).data("index");
            enderecos.splice(index, 1);
            atualizarTabelaEnderecos();
        });
    }

    $("#btnsalvar").click(function () {
        if (validarCampos()) {
            const obj = {
                id: $("#txtid").val(),
                nomeCompleto: $("#txtnomeCompleto").val(),
                dataNascimento: $("#txtdataNascimento").val(),
                rgNumero: $("#txtrgNumero").val(),
                rgDataEmissao: $("#txtrgDataEmissao").val(),
                rgOrgaoExpedidor: $("#txtrgOrgaoExpedidor").val(),
                rgUfEmissao: $("#selectRgUfEmissao").val(),
                cnsNumero: $("#txtcnsNumero").val(),
                cpfNumero: $("#txtcpfNumero").val(),
                nomeMae: $("#txtnomeMae").val(),
                nomeConjuge: $("#txtnomeConjuge").val(),
                naturalidadeCidade: $("#selectNaturalidadeCidade").val(),
                naturalidadeUf: $("#selectNaturalidadeUf").val(),
                peso: $("#txtpeso").val(),
                altura: $("#txtaltura").val(),
                dataCadastro: $("#txtdataCadastro").val(),  // não pode ser modificado pelo usuário
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
                url: urlAPI + "api/Paciente" + (obj.id != "0" ? "/" + obj.id : ""),
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify(obj),
                dataType: "json",
                success: function () {
                    limparFormulario();
                    alert("Dados Salvos com sucesso!");

                    if ($("#tabela").length > 0) {
                        carregarPacientes();
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
        $("#txtpeso").val('');
        $("#txtaltura").val('');
        $("#selectStatus").val('');
        $("#selectSexo").val('');
        $("#selectProfissao").val('');
        $("#selectCorRaca").val('');
        $("#selectEstadoCivil").val('');
        contatos = [];
        enderecos = [];
        atualizarTabelaContatos();
        atualizarTabelaEnderecos();
    }

    function carregarPacientes() {
        $.ajax({
            url: urlAPI + "api/Paciente/todosPacientesComContatosEEnderecos",
            method: "GET",
            success: function (data) {
                $("#tabela").empty();
                $.each(data, function (index, item) {
                    var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                    $(linha).find(".codigo").html(item.id);
                    $(linha).find(".nomeCompleto").html(item.nomeCompleto);
                    $(linha).find(".dataNascimento").html(new Date(item.dataNascimento).toLocaleDateString()); // Formata a data
                    $(linha).find(".rgNumero").html(item.rgNumero);
                    $(linha).find(".status").html(item.status ? item.status.nome : "Não Definido");

                    // Construir o HTML para exibir os contatos
                    var contatosHTML = item.contato.map(c => {
                        var tipoContatoNome = c.tipocontato ? c.tipocontato.nome : "Tipo de Contato Desconhecido";
                        return `${tipoContatoNome}: ${c.valor}`;
                    }).join("<br>");
                    $(linha).find(".contatos").html(contatosHTML);

                    // Construir o HTML para exibir os endereços
                    var enderecosHTML = item.endereco.map(e => {
                        return `${e.tipoendereco ? e.tipoendereco.nome : 'Desconhecido'}: ${e.logradouro}, ${e.numero}, ${e.complemento}, ${e.bairro}, ${e.cidade}, ${e.uf}, ${e.cep}, ${e.pontoReferencia}`;
                    }).join("<br>");
                    $(linha).find(".enderecos").html(enderecosHTML);

                    $(linha).show();
                    $("#tabela").append(linha);
                });

                $('#tabelaPaciente').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar pacientes.");
            }
        });
    }

    function excluir(codigo) {
        $.ajax({
            type: "DELETE",
            url: urlAPI + "api/Paciente/" + codigo,
            contentType: "application/json;charset=utf-8",
            success: function () {
                alert('Exclusão efetuada!');
                location.reload(); // Recarrega a página para atualizar a tabela
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("Erro ao excluir o paciente: " + errorThrown);
            }
        });
    }

    function visualizar(codigo) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Paciente/" + codigo,
            contentType: "application/json;charset=utf-8",
            data: {},
            dataType: "json",
            success: function (jsonResult) {
                pacienteDados = jsonResult;
                $("#txtid").val(jsonResult.id);
                $("#txtnomeCompleto").val(jsonResult.nomeCompleto);
                // Formatar e definir a data de nascimento
                var dataNascimento = new Date(jsonResult.dataNascimento);
                var formattedDate = dataNascimento.toISOString().split('T')[0];
                $("#txtdataNascimento").val(formattedDate);
                $("#txtrgNumero").val(jsonResult.rgNumero);
                $("#txtrgDataEmissao").val(new Date(jsonResult.rgDataEmissao).toISOString().split('T')[0]);
                $("#txtrgOrgaoExpedidor").val(jsonResult.rgOrgaoExpedidor);
                $("#selectRgUfEmissao").val(jsonResult.rgUfEmissao);
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
                $("#selectNaturalidadeUf").val(jsonResult.naturalidadeUf);
                $("#selectNaturalidadeCidade").val(jsonResult.naturalidadeCidade);
                $("#txtpeso").val(jsonResult.peso);
                $("#txtaltura").val(jsonResult.altura);

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

                // Carregar os municípios depois de definir o estado
                carregarMunicipios(jsonResult.naturalidadeUf, $("#selectNaturalidadeCidade"), jsonResult.naturalidadeCidade);

                // Calcular idade e definir no campo
                let idade = calcularIdade(dataNascimento);
                $("#txtidade").val(idade);
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }

    function calcularIdade(dataNascimento) {
        let hoje = new Date();
        let idade = hoje.getFullYear() - dataNascimento.getFullYear();
        let m = hoje.getMonth() - dataNascimento.getMonth();

        if (m < 0 || (m === 0 && hoje.getDate() < dataNascimento.getDate())) {
            idade--;
        }
        return idade;
    }

    // Ocultar mensagens de validação ao começar a digitar ou selecionar
    $(".form-control").on("input change", function () {
        $(this).removeClass('is-invalid');
    });

    carregarPacientes();
});
