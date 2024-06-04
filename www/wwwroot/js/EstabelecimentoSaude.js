const urlAPI = "https://localhost:44309/";

$(document).ready(function () {

    // Apenas números nos campos CNES
    $(".numeric-only").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    let contatos = [];
    let enderecos = [];
    let estabelecimentoDados;

    if ($("#tabela").length > 0) {
        carregarEstabelecimentos();
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

    $("#btnlimpar").click(function () {
        limparFormulario();
    });

    $(document).on("click", ".alterar", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        window.location.href = "/EstabelecimentoSaudeCadastro?id=" + codigo;
    });

    $(document).on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtnomeFantasia").val().trim()) {
            $("#txtnomeFantasia").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtrazaoSocial").val().trim()) {
            $("#txtrazaoSocial").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtcnes").val().trim()) {
            $("#txtcnes").addClass('is-invalid');
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
                selectElement.empty();
                selectElement.append('<option value="0">Selecione um estado</option>');
                data.sort((a, b) => a.sigla.localeCompare(b.sigla)).forEach(estado => {
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
                    selectElement.append('<option value="0">Selecione um município</option>');
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

    carregarEstados($("#selectEstado"));

    $("#selectEstado").change(function () {
        carregarMunicipios($(this).val(), $("#selectMunicipio"));
    });

    function carregarOpcoes(apiEndpoint, selectElement) {
        $.ajax({
            url: urlAPI + apiEndpoint,
            method: "GET",
            success: function (data) {
                selectElement.empty();
                selectElement.append('<option value="0">Selecione uma opção</option>');
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

    carregarOpcoes("api/EstabelecimentoSaude/tipoContato", $("#selectTipoContato"));
    carregarOpcoes("api/EstabelecimentoSaude/tipoEndereco", $("#selectTipoEndereco"));
    carregarOpcoes("api/EstabelecimentoSaude/tipoStatus", $("#selectStatus"));

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

        if (logradouro && numero && bairro && cidade && uf && cep) {
            enderecos.push({ idTipoEndereco, tipoEndereco, logradouro, numero, complemento, bairro, cidade, uf, cep, pontoReferencia });
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
            alert("Por favor, preencha todos os campos obrigatórios do endereço.");
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
                <td>${endereco.tipoEndereco}</td>
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
                nomeFantasia: $("#txtnomeFantasia").val(),
                razaoSocial: $("#txtrazaoSocial").val(),
                cnes: $("#txtcnes").val(),
                idStatus: $("#selectStatus").val(),
                contato: contatos,
                endereco: enderecos
            };

            $.ajax({
                type: obj.id == "0" ? "POST" : "PUT",
                url: urlAPI + "api/EstabelecimentoSaude" + (obj.id != "0" ? "/" + obj.id : ""),
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify(obj),
                dataType: "json",
                success: function () {
                    limparFormulario();
                    alert("Dados Salvos com sucesso!");

                    if ($("#tabela").length > 0) {
                        carregarEstabelecimentos();
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
        $("#txtnomeFantasia").val('');
        $("#txtrazaoSocial").val('');
        $("#txtcnes").val('');
        $("#txtid").val('0');
        $("#txtdataCadastro").val(new Date().toISOString().split('T')[0]);
        $("#selectStatus").val('0');
        contatos = [];
        enderecos = [];
        atualizarTabelaContatos();
        atualizarTabelaEnderecos();
    }

    function carregarEstabelecimentos() {
        $.ajax({
            url: urlAPI + "api/EstabelecimentoSaude/todos",
            method: "GET",
            success: function (data) {
                $("#tabela").empty();
                $.each(data, function (index, item) {
                    var linha = $("#linhaExemplo").clone().removeAttr("id").removeAttr("style");
                    $(linha).find(".codigo").html(item.id);
                    $(linha).find(".nomeFantasia").html(item.nomeFantasia);
                    $(linha).find(".razaoSocial").html(item.razaoSocial);
                    $(linha).find(".cnes").html(item.cnes);
                    $(linha).find(".status").html(item.status ? item.status.nome : "Não Definido");

                    // Construir o HTML para exibir os contatos
                    var contatosHTML = item.contato.map(c => {
                        var tipoContatoNome = c.tipocontato ? c.tipocontato.nome : "Tipo de Contato Desconhecido";
                        return `${tipoContatoNome}: ${c.valor}`;
                    }).join("<br>");
                    $(linha).find(".contatos").html(contatosHTML);

                    // Construir o HTML para exibir os endereços
                    var enderecosHTML = item.endereco.map(e => {
                        var tipoEnderecoNome = e.tipoendereco ? e.tipoendereco.nome : "Tipo de Endereço Desconhecido";
                        return `${tipoEnderecoNome}: ${e.logradouro}, ${e.numero}, ${e.complemento}, ${e.bairro}, ${e.cidade}, ${e.uf}, ${e.cep}, ${e.pontoReferencia}`;
                    }).join("<br>");
                    $(linha).find(".enderecos").html(enderecosHTML);

                    $(linha).show();
                    $("#tabela").append(linha);
                });

                $('#tabelaEstabelecimentoSaude').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar estabelecimentos.");
            }
        });
    }

    function excluir(codigo) {
        $.ajax({
            type: "DELETE",
            url: urlAPI + "api/EstabelecimentoSaude/" + codigo,
            contentType: "application/json;charset=utf-8",
            success: function () {
                alert('Exclusão efetuada!');
                location.reload(); // Recarrega a página para atualizar a tabela
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("Erro ao excluir o estabelecimento: " + errorThrown);
            }
        });
    }

    function visualizar(codigo) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/EstabelecimentoSaude/" + codigo,
            contentType: "application/json;charset=utf-8",
            data: {},
            dataType: "json",
            success: function (jsonResult) {
                estabelecimentoDados = jsonResult;
                $("#txtid").val(jsonResult.id);
                $("#txtnomeFantasia").val(jsonResult.nomeFantasia);
                $("#txtrazaoSocial").val(jsonResult.razaoSocial);
                $("#txtcnes").val(jsonResult.cnes);
                $("#txtdataCadastro").val(new Date(jsonResult.dataCadastro).toISOString().split('T')[0]);
                $("#selectStatus").val(jsonResult.idStatus);

                contatos = jsonResult.contato.map(c => ({
                    idTipoContato: c.idTipoContato,
                    tipo: c.tipocontato.nome,
                    valor: c.valor
                }));
                atualizarTabelaContatos();

                enderecos = jsonResult.endereco.map(e => ({
                    idTipoEndereco: e.idTipoEndereco,
                    tipoEndereco: e.tipoendereco.nome,
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
                carregarMunicipios(jsonResult.uf, $("#selectMunicipio"), jsonResult.cidade);
            },
            error: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    }

    carregarEstabelecimentos();
});
