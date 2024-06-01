const urlAPI = "https://localhost:7034/";

$(document).ready(function () {
    let contatos = [];
    let enderecos = [];

    if ($("#tabela").length > 0) {
        carregarRepresentantes();
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
        window.location.href = "/RepresentanteCadastro?id=" + codigo;
    });

    $(document).on("click", ".excluir", function (elemento) {
        let codigo = $(elemento.target).closest("tr").find(".codigo").text();
        excluir(codigo);
    });

    function validarCampos() {
        let isValid = true;
        $(".form-control").removeClass('is-invalid');

        if (!$("#txtnomeCompleto").val().trim()) {
            $("#txtnomeCompleto").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtdataNascimento").val().trim()) {
            $("#txtdataNascimento").addClass('is-invalid');
            isValid = false;
        }
        if (!$("#txtrgNumero").val().trim()) {
            $("#txtrgNumero").addClass('is-invalid');
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

    // Chamada AJAX para carregar estados do IBGE
    $.ajax({
        url: "https://servicodados.ibge.gov.br/api/v1/localidades/estados",
        method: "GET",
        success: function (data) {
            const selectEstado = $("#selectEstado");
            data.forEach(estado => {
                const option = `<option value="${estado.sigla}">${estado.sigla}</option>`;
                selectEstado.append(option);
            });
        },
        error: function () {
            alert("Erro ao carregar os estados.");
        }
    });

    // Carregar municípios ao selecionar um estado
    $("#selectEstado").change(function () {
        const estadoSigla = $(this).val();
        if (estadoSigla) {
            const estado = $("#selectEstado option:selected").text();
            $.ajax({
                url: `https://servicodados.ibge.gov.br/api/v1/localidades/estados/${estadoSigla}/municipios`,
                method: "GET",
                success: function (data) {
                    const selectMunicipio = $("#selectMunicipio");
                    selectMunicipio.empty();
                    data.forEach(municipio => {
                        const option = `<option value="${municipio.id}">${municipio.nome}</option>`;
                        selectMunicipio.append(option);
                    });
                },
                error: function () {
                    alert("Erro ao carregar os municípios.");
                }
            });
        } else {
            $("#selectMunicipio").empty();
        }
    });

    $.ajax({
        url: urlAPI + "api/Representante/tipoContato",
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

    $.ajax({
        url: urlAPI + "api/Representante/tipoEndereco",
        method: "GET",
        success: function (data) {
            const selectTipoEndereco = $("#selectTipoEndereco");
            data.forEach(tipo => {
                const option = `<option value="${tipo.id}">${tipo.nome}</option>`;
                selectTipoEndereco.append(option);
            });
        },
        error: function () {
            alert("Erro ao carregar os tipos de endereço.");
        }
    });

    // Adicionar chamada AJAX para carregar os status
    $.ajax({
        url: urlAPI + "api/Representante/tipoStatus",
        method: "GET",
        success: function (data) {
            const selectStatus = $("#selectStatus");
            data.forEach(status => {
                const option = `<option value="${status.id}">${status.nome}</option>`;
                selectStatus.append(option);
            });
        },
        error: function () {
            alert("Erro ao carregar os status.");
        }
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

        if (logradouro && numero && bairro && cidade && uf && cep) {
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
                <td>${endereco.idTipoEndereco}</td>
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
                rgUfEmissao: $("#txtrgUfEmissao").val(),
                cnsNumero: $("#txtcnsNumero").val(),
                cpfNumero: $("#txtcpfNumero").val(),
                dataCadastro: $("#txtdataCadastro").val(),  // não pode ser modificado pelo usuário
                idStatus: $("#selectStatus").val(),  // Adiciona o status do representante
                contato: contatos,
                endereco: enderecos
            };

            if (contatos.length === 0) {
                $("#mensagemValidacao").text("Por favor, adicione pelo menos um contato.");
                return; // Não prossegue com o salvamento dos dados se a validação falhar
            }

            if (enderecos.length === 0) {
                $("#mensagemValidacaoEndereco").text("Por favor, adicione pelo menos um endereço.");
                return; // Não prossegue com o salvamento dos dados se a validação falhar
            }

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
        $("#txtrgUfEmissao").val('');
        $("#txtcnsNumero").val('');
        $("#txtcpfNumero").val('');
        $("#txtid").val('0');
        $("#txtdataCadastro").val(new Date().toISOString().split('T')[0]);
        $("#txtidade").val('');
        $("#selectStatus").val('');
        contatos = [];
        enderecos = [];
        atualizarTabelaContatos();
        atualizarTabelaEnderecos();
    }

    function carregarRepresentantes() {
        $.ajax({
            url: urlAPI + "api/Representante/todosRepresentantesComContatosEEnderecos",
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

                $('#tabelaRepresentante').DataTable({
                    language: {
                        url: '/js/pt-BR.json'
                    },
                    destroy: true
                });
            },
            error: function () {
                alert("Erro ao carregar representantes.");
            }
        });
    }

    function excluir(codigo) {
        $.ajax({
            type: "DELETE",
            url: urlAPI + "api/Representante/" + codigo,
            contentType: "application/json;charset=utf-8",
            success: function () {
                alert('Exclusão efetuada!');
                location.reload(); // Recarrega a página para atualizar a tabela
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("Erro ao excluir o representante: " + errorThrown);
            }
        });
    }

    function visualizar(codigo) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Representante/" + codigo,
            contentType: "application/json;charset=utf-8",
            data: {},
            dataType: "json",
            success: function (jsonResult) {
                $("#txtid").val(jsonResult.id);
                $("#txtnomeCompleto").val(jsonResult.nomeCompleto);
                // Formatar e definir a data de nascimento
                var dataNascimento = new Date(jsonResult.dataNascimento);
                var formattedDate = dataNascimento.toISOString().split('T')[0];
                $("#txtdataNascimento").val(formattedDate);
                $("#txtrgNumero").val(jsonResult.rgNumero);
                $("#txtrgDataEmissao").val(new Date(jsonResult.rgDataEmissao).toISOString().split('T')[0]);
                $("#txtrgOrgaoExpedidor").val(jsonResult.rgOrgaoExpedidor);
                $("#txtrgUfEmissao").val(jsonResult.rgUfEmissao);
                $("#txtcnsNumero").val(jsonResult.cnsNumero);
                $("#txtcpfNumero").val(jsonResult.cpfNumero);
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

    carregarRepresentantes();
});
