const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
    carregarTiposContato();
    carregarTiposEndereco();
    carregarPacientes();

    // Função para carregar os tipos de contato em um select
    function carregarTiposContatoSelect(selectElement) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Paciente/api/TipoContato",
            contentType: "application/json",
            success: function (data) {
                selectElement.empty();
                data.forEach(function (item) {
                    selectElement.append(new Option(item.nome, item.id));
                });
            },
            error: function (xhr) {
                console.error("Erro ao carregar tipos de contato: ", xhr.status, xhr.statusText);
            }
        });
    }

    function carregarTiposEnderecoSelect(selectElement) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/Paciente/api/TipoEndereco",
            contentType: "application/json",
            success: function (data) {
                selectElement.empty();
                data.forEach(function (item) {
                    selectElement.append(new Option(item.nome, item.id));
                });
            },
            error: function (xhr) {
                console.error("Erro ao carregar tipos de endereço: ", xhr.status, xhr.statusText);
            }
        });
    }

    // Adicionar mais contatos
    $("#btnAdicionarContato").click(function () {
        let novoContato = `
        <div class="contato">
            <div class="col-md-4">
                <label class="form-label">Contato</label>
                <input type="text" class="form-control" placeholder="Contato" required>
            </div>
            <div class="col-md-4">
                <label class="form-label">Tipo de Contato</label>
                <select class="form-select" required></select>
            </div>
            <div class="col-md-4">
                <button type="button" class="btn btn-danger btnRemoverContato">Remover</button>
            </div>
        </div>
        `;
        let novoElemento = $(novoContato);
        $("#contatos-container").append(novoElemento);
        carregarTiposContatoSelect(novoElemento.find('select'));
    });

    $("#contatos-container").on("click", ".btnRemoverContato", function () {
        $(this).closest(".contato").remove();
    });

    // Adicionar mais endereços
    $("#btnAdicionarEndereco").click(function () {
        let novoEndereco = `
        <div class="endereco">
            <div class="col-md-4">
                <label class="form-label">Tipo de Endereço</label>
                <select class="form-select" required></select>
            </div>
            <div class="col-md-4">
                <label class="form-label">Logradouro</label>
                <input type="text" class="form-control" placeholder="Logradouro" >
            </div>
            <div class="col-md-2">
                <label class="form-label">Número</label>
                <input type="text" class="form-control" placeholder="Número" >
            </div>
            <div class="col-md-3">
                <label class="form-label">Complemento</label>
                <input type="text" class="form-control" placeholder="Complemento">
            </div>
            <div class="col-md-3">
                <label class="form-label">Bairro</label>
                <input type="text" class="form-control" placeholder="Bairro" >
            </div>
            <div class="col-md-4">
                <label class="form-label">Cidade</label>
                <input type="text" class="form-control" placeholder="Cidade" >
            </div>
            <div class="col-md-2">
                <label class="form-label">UF</label>
                <input type="text" class="form-control" placeholder="UF" >
            </div>
            <div class="col-md-3">
                <label class="form-label">CEP</label>
                <input type="text" class="form-control" placeholder="CEP" >
            </div>
            <div class="col-md-5">
                <label class="form-label">Ponto de Referência</label>
                <input type="text" class="form-control" placeholder="Ponto de Referência">
            </div>
            <div class="col-md-4">
                <button type="button" class="btn btn-danger btnRemoverEndereco">Remover</button>
            </div>
        </div>
    `;
        let novoElemento = $(novoEndereco);
        $("#enderecos-container").append(novoElemento);
        carregarTiposEnderecoSelect(novoElemento.find('select'));
    });

    $("#enderecos-container").on("click", ".btnRemoverEndereco", function () {
        $(this).closest(".endereco").remove();
    });


    $("#enderecos-container").on("click", ".btnRemoverEndereco", function () {
        $(this).closest(".endereco").remove();
    });

    $("#tabela").on("click", ".excluir", function (e) {
        let codigo = $(e.target).parent().parent().find(".codigo").text();
        excluirPacienteId(codigo);
    });

    $("#tabela").on("click", ".alterar", function (e) {
        let codigo = $(e.target).parent().parent().find(".codigo").text();
        carregarPacienteId(codigo);
    });

    $("#btnsalvar").click(function () {
        const contatos = [];
        $("#contatos-container .contato").each(function () {
            const idTipoContato = parseInt($(this).find('select').val());
            const valor = $(this).find('input').val();
            contatos.push({ idTipoContato, valor });
        });

        const enderecos = [];
        $("#enderecos-container .endereco").each(function () {
            const idTipoEndereco = parseInt($(this).find('select').val());
            console.log("ID do Tipo de Endereço:", idTipoEndereco);

            const logradouro = $(this).find('input').eq(0).val();
            console.log("Logradouro:", logradouro);

            const numero = $(this).find('input').eq(1).val();
            console.log("Número:", numero);

            const complemento = $(this).find('input').eq(2).val();
            console.log("Complemento:", complemento);

            const bairro = $(this).find('input').eq(3).val();
            console.log("Bairro:", bairro);

            const cidade = $(this).find('input').eq(4).val();
            console.log("Cidade:", cidade);

            const uf = $(this).find('input').eq(5).val();
            console.log("UF:", uf);

            const cep = $(this).find('input').eq(6).val();
            console.log("CEP:", cep);

            const pontoReferencia = $(this).find('input').eq(7).val();
            console.log("Ponto de Referência:", pontoReferencia);

            enderecos.push({
                idTipoEndereco,
                logradouro,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                cep,
                pontoReferencia
            });
        });


        const obj = {
            id: $("#txtid").val() === "0" ? null : parseInt($("#txtid").val()),
            nomeCompleto: $("#txtnomecompleto").val(),
            peso: parseFloat($("#txtpeso").val()),
            altura: parseFloat($("#txtaltura").val()),
            dataNascimento: $("#txtdatanascimento").val(),
            idade: parseInt($("#txtidade").val()),
            nomeMae: $("#txtnomemae").val(),
            rgNumero: $("#txtrgnumero").val(),
            rgDataEmissao: $("#txtrgdataemissao").val(),
            rgOrgaoExpedidor: $("#txtrgorgaoexpedidor").val(),
            rgUfEmissao: $("#txtrgufemissao").val(),
            cnsNumero: $("#txtcnsnumero").val(),
            cpfNumero: $("#txtcpfnumero").val(),
            nomeConjuge: $("#txtnomeconjuge").val(),
            dataCadastro: new Date().toISOString(),
            contato: contatos,
            endereco: enderecos
        };
        console.log("Objeto a ser enviado:", obj);
        $.ajax({
            type: $("#txtid").val() === "0" ? "POST" : "PUT",
            url: urlAPI + "api/Paciente",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {
                if (jsonResult.statusCode === 500) {
                    alert(jsonResult.value.detail);
                } else {
                    $("#txtid").val('0');
                    limparCampos();
                    alert("Dados Salvos com sucesso!");
                    carregarPacientes();
                }
            },
            failure: function (response) {
                alert("Erro ao carregar os dados: " + response);
            }
        });
    });
});

function limparCampos() {
    $("#txtnomecompleto").val('');
    $("#txtpeso").val('');
    $("#txtaltura").val('');
    $("#txtdatanascimento").val('');
    $("#txtidade").val('');
    $("#txtnomemae").val('');
    $("#txtrgnumero").val('');
    $("#txtrgdataemissao").val('');
    $("#txtorgaoexpedidor").val('');
    $("#txtufemissao").val('');
    $("#txtcnsnumero").val('');
    $("#txtcpfnumero").val('');
    $("#txtnomeconjuge").val('');
    $("#contatos-container").empty();
    $("#enderecos-container").empty();
}

function carregarPacientes() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente",
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#tabela").empty();
            $.each(jsonResult, function (index, item) {
                var linha = $("#linhaModelo").clone();
                linha.find(".codigo").html(item.id);
                linha.find(".descricao").html(item.nomeCompleto);
                $("#tabela").append(linha);
            });
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function carregarPacienteId(id) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/" + id,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#txtid").val(jsonResult.id);
            $("#txtnomecompleto").val(jsonResult.nomeCompleto);
            $("#txtpeso").val(jsonResult.peso);
            $("#txtaltura").val(jsonResult.altura);
            $("#txtdatanascimento").val(jsonResult.dataNascimento.substring(0, 10));
            $("#txtidade").val(jsonResult.idade);
            $("#txtnomemae").val(jsonResult.nomeMae);
            $("#txtrgnumero").val(jsonResult.rgNumero);
            $("#txtrgdataemissao").val(jsonResult.rgDataEmissao.substring(0, 10));
            $("#txtorgaoexpedidor").val(jsonResult.orgaoExpedidor);
            $("#txtufemissao").val(jsonResult.ufEmissao);
            $("#txtcnsnumero").val(jsonResult.cnsNumero);
            $("#txtcpfnumero").val(jsonResult.cpfNumero);
            $("#txtnomeconjuge").val(jsonResult.nomeConjuge);

            carregarContatosPaciente(jsonResult.id);
            carregarEnderecoPaciente(jsonResult.id);
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function carregarContatosPaciente(idPaciente) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Contato/PorPaciente/" + idPaciente,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            $("#contatos-container").empty();
            jsonResult.forEach(function (contato) {
                let contatoHTML = `
                <div class="contato">
                    <div class="col-md-4">
                        <label class="form-label">Contato</label>
                        <input type="text" class="form-control" value="${contato.valor}" required>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Tipo de Contato</label>
                        <select class="form-select" required></select>
                    </div>
                    <div class="col-md-4">
                        <button type="button" class="btn btn-danger btnRemoverContato">Remover</button>
                    </div>
                </div>`;
                let novoElemento = $(contatoHTML);
                $("#contatos-container").append(novoElemento);
                carregarTiposContatoSelect(novoElemento.find('select'));
                novoElemento.find('select').val(contato.idTipoContato);
            });
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}
function carregarEnderecoPaciente(idPaciente) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Endereco/PorPaciente/" + idPaciente,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            if (jsonResult.length > 0) {
                const endereco = jsonResult[0];
                $("#txtlogradouro").val(endereco.logradouro);
                $("#txtnumero").val(endereco.numero);
                $("#txtcomplemento").val(endereco.complemento);
                $("#txtbairro").val(endereco.bairro);
                $("#txtcidade").val(endereco.cidade);
                $("#txtuf").val(endereco.uf);
                $("#txtcep").val(endereco.cep);
                $("#txtpontoreferencia").val(endereco.pontoReferencia);
                $("#txttipocontato").val(endereco.idTipoContato);
            } else {
                $("#txtlogradouro").val('');
                $("#txtnumero").val('');
                $("#txtcomplemento").val('');
                $("#txtbairro").val('');
                $("#txtcidade").val('');
                $("#txtuf").val('');
                $("#txtcep").val('');
                $("#txtpontoreferencia").val('');
                $("#txttipocontato").val('');
            }
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}


function carregarTiposContato() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/api/TipoContato",
        contentType: "application/json",
        success: function (data) {
            $("#txttipocontato").empty();
            data.forEach(function (item) {
                $("#txttipocontato").append(new Option(item.nome, item.id));
            });
        },
        error: function (xhr) {
            console.error("Erro ao carregar tipos de contato: ", xhr.status, xhr.statusText);
        }
    });
}

function carregarTiposEndereco() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Paciente/api/TipoEndereco",
        contentType: "application/json",
        success: function (data) {
            $("#txttipoendereco").empty();
            data.forEach(function (item) {
                $("#txttipoendereco").append(new Option(item.nome, item.id));
            });
        },
        error: function (xhr) {
            console.error("Erro ao carregar tipos de endereço: ", xhr.status, xhr.statusText);
        }
    });
}

function excluirPacienteId(id) {
    $.ajax({
        type: "DELETE",
        url: urlAPI + "api/Paciente/" + id,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            if (jsonResult === true) {
                alert('Dados excluídos');
                carregarPacientes();
            } else {
                alert('Dados não foram excluídos');
            }
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}
