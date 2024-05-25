const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
    let contatos = [];
    let tiposEndereco = [];
    let enderecos = [];

    // Carregar tipos de contato
    $.ajax({
        url: urlAPI + "api/Paciente/tipoContato",
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

    // Carregar tipos de endereço
    $.ajax({
        url: urlAPI + "api/Paciente/tipoEndereco",
        method: "GET",
        success: function (data) {
            tiposEndereco = data;
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

    $("#btnAdicionarEndereco").click(function () {
        const tipoEndereco = $("#selectTipoEndereco option:selected").text();
        const idTipoEndereco = $("#selectTipoEndereco").val(); // Capturando o ID do tipo de endereço
        const endereco = {
            idTipoEndereco: idTipoEndereco, // Incluindo o ID do tipo de endereço
            tipoEndereco: tipoEndereco, // Incluindo o nome do tipo de endereço
            logradouro: $("#txtlogradouro").val(),
            numero: $("#txtnumero").val(),
            complemento: $("#txtcomplemento").val(),
            bairro: $("#txtbairro").val(),
            cidade: $("#txtcidade").val(),
            uf: $("#txtuf").val(),
            cep: $("#txtcep").val(),
            pontoReferencia: $("#txtpontoreferencia").val()
        };

        console.log('Endereço a adicionar:', endereco);

        if (endereco.logradouro && endereco.numero && endereco.bairro && endereco.cidade && endereco.uf && endereco.cep) {
            enderecos.push(endereco);
            atualizarTabelaEnderecos();
            limparCamposEndereco();
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
                <td><button type="button" class="btn btn-danger btn-excluir-contato" data-index="${index}">Excluir</button></td>
            </tr>`;
            tabela.append(linha);
        });

        $(".btn-excluir-contato").click(function () {
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
            <td>${endereco.tipoEndereco}</td> <!-- Apenas exibir o nome do tipo de endereço -->
            <td>${endereco.logradouro}</td>
            <td>${endereco.numero}</td>
            <td>${endereco.complemento}</td>
            <td>${endereco.bairro}</td>
            <td>${endereco.cidade}</td>
            <td>${endereco.uf}</td>
            <td>${endereco.cep}</td>
            <td>${endereco.pontoReferencia}</td>
            <td><button type="button" class="btn btn-danger btn-excluir-endereco" data-index="${index}">Excluir</button></td>
        </tr>`;
            tabela.append(linha);
        });

        $(".btn-excluir-endereco").click(function () {
            const index = $(this).data("index");
            enderecos.splice(index, 1);
            atualizarTabelaEnderecos();
        });
    }



    function limparFormulario() {
        console.log("Limpando formulário...");
        $("#txtnomecompleto").val('');
        $("#txtdatanascimento").val('');
        $("#txtcpf").val('');
        $("#txtid").val('0');
        $("#txtpeso").val('');
        $("#txtaltura").val('');
        $("#txtidade").val('');
        $("#txtnomemae").val('');
        $("#txtrgnumero").val('');
        $("#txtrgdataemissao").val('');
        $("#txtrgorgaoexpedidor").val('');
        $("#txtrgufemissao").val('');
        $("#txtcnsnumero").val('');
        $("#txtnomeconjuge").val('');
        $("#txtdatacadastro").val('');

        contatos = [];
        enderecos = [];
        atualizarTabelaContatos();
        atualizarTabelaEnderecos();
    }


    function limparCamposEndereco() {
        $("#txtlogradouro").val('');
        $("#txtnumero").val('');
        $("#txtcomplemento").val('');
        $("#txtbairro").val('');
        $("#txtcidade").val('');
        $("#txtuf").val('');
        $("#txtcep").val('');
        $("#txtpontoreferencia").val('');
    }

    $("#btnsalvar").click(function () {
        const obj = {
            id: $("#txtid").val(),
            nomeCompleto: $("#txtnomecompleto").val(),
            peso: $("#txtpeso").val(),
            altura: $("#txtaltura").val(),
            dataNascimento: $("#txtdatanascimento").val(),
            idade: $("#txtidade").val(),
            nomeMae: $("#txtnomemae").val(),
            rgNumero: $("#txtrgnumero").val(),
            rgDataEmissao: $("#txtrgdataemissao").val(),
            rgOrgaoExpedidor: $("#txtrgorgaoexpedidor").val(),
            rgUfEmissao: $("#txtrgufemissao").val(),
            cnsNumero: $("#txtcnsnumero").val(),
            cpfNumero: $("#txtcpfnumero").val(),
            nomeConjuge: $("#txtnomeconjuge").val(),
            dataCadastro: $("#txtdatacadastro").val(),
            contato: contatos,
            endereco: enderecos // Endereços agora incluem idTipoEndereco
        };

        console.log('Payload a ser enviado:', JSON.stringify(obj));

        $.ajax({
            type: obj.id == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Paciente" + (obj.id != "0" ? "/" + obj.id : ""),
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function () {
                limparFormulario();
                alert("Dados Salvos com sucesso!");
                carregarPacientes();
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



    function carregarPacientes() {
        $.ajax({
            url: urlAPI + "api/Paciente",
            method: "GET",
            success: function (data) {
                const tabela = $("#pacientesTable tbody");
                tabela.empty();

                data.forEach(paciente => {
                    const linha = `<tr>
                        <td>${paciente.id}</td>
                        <td>${paciente.nomeCompleto}</td>
                        <td>${new Date(paciente.dataNascimento).toLocaleDateString()}</td>
                        <td>${paciente.cpf}</td>
                        <td><button type="button" class="btn btn-primary btn-editar" data-id="${paciente.id}">Editar</button></td>
                    </tr>`;
                    tabela.append(linha);
                });

                $(".btn-editar").click(function () {
                    const id = $(this).data("id");
                    editarPaciente(id);
                });
            },
            error: function () {
                alert("Erro ao carregar pacientes.");
            }
        });
    }

    function editarPaciente(id) {
        $.ajax({
            url: urlAPI + "api/Paciente/" + id,
            method: "GET",
            success: function (data) {
                $("#txtid").val(data.id);
                $("#txtnomeCompleto").val(data.nomeCompleto);
                $("#txtdataNascimento").val(new Date(data.dataNascimento).toISOString().split('T')[0]);
                $("#txtcpf").val(data.cpf);

                contatos = data.contato.map(c => ({
                    idTipoContato: c.idTipoContato,
                    tipo: c.tipocontato.nome,
                    valor: c.valor
                }));
                enderecos = data.endereco;
                atualizarTabelaContatos();
                atualizarTabelaEnderecos();
            },
            error: function () {
                alert("Erro ao carregar os dados do paciente.");
            }
        });
    }

    carregarPacientes();
});

