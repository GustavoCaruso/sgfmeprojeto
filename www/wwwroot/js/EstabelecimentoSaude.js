const urlAPI = "https://localhost:44309/"


$(document).ready(function () {
    CarregarEstabelecimentoSaude();
    CarregarTipoContato();



    // Função para carregar os tipos de contato em um select
    function CarregarTipoContatoSelect(selectElement) {
        $.ajax({
            type: "GET",
            url: urlAPI + "api/EstabelecimentoSaude/api/TipoContato",
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

    $("#contatos-container").on("click", ".btnRemoverContato", function () {
        $(this).closest(".contato").remove();
    });








    // Adicionar mais contatos
    $("#btnAdicionarContato").click(function () {
        let novoContato = `
        <div class="contato row mb-3">
           <div class="col-md-2">
                <label class="form-label">Contato</label>
                <input type="text" class="form-control" placeholder="Contato" required>
            </div>
            <div class="col-md-4">
                <label class="form-label">Tipo de Contato</label>
                <select class="form-select" required></select>
            </div>
             <div class="col-md-4 d-flex align-items-end">
            <button type="button" class="btn btn-danger btnRemoverContato mt-2">Remover</button>
        </div>
        </div>
        `;
        let novoElemento = $(novoContato);
        $("#contatos-container").append(novoElemento);
        CarregarTipoContatoSelect(novoElemento.find('select'));
    });









    $("#btnlimpar").click(function () {
        $("#txtid").val('0');
        $("#txtnomeFantasia").val('');
        $("#txtrazaoSocial").val('');
        $("#txtcnes").val('');
        $("#contatos-container").empty();
    });


    $("#tabela").on("click", ".alterar", function (elemento) {
        //alert('clicou visualizar!')
        //parent: retorna para elemmento pai
        //find: procura nos elementos filhos
        let codigo = $(elemento.target).parent().parent().find(".codigo").text()
        visualizar(codigo)

    })


    $("#tabela").on("click", ".excluir", function (elemento) {
        //alert('clicou excluir!')
        let codigo = $(elemento.target).parent().parent().find(".codigo").text()
        excluir(codigo)
    })


    $("#btnsalvar").click(function () {
        //Valida os campos
        const contatos = [];
        $("#contatos-container .contato").each(function () {
            const idTipoContato = parseInt($(this).find('select').val());
            const valor = $(this).find('input').val();
            contatos.push({ idTipoContato, valor });
        });
        const obj = {
            id: $("#txtid").val(),
            nomeFantasia: $("#txtnomeFantasia").val(),
            razaoSocial: $("#txtrazaoSocial").val(),
            cnes: $("#txtcnes").val(),
            contato: contatos
        }
        console.log(JSON.stringify(obj));

        $.ajax({
            type: $("#txtid").val() == "0" ? "POST" : "PUT",
            url: urlAPI + "api/EstabelecimentoSaude",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {

                console.log(jsonResult)
                $("#txtid").val('0');
                $("#txtnomeFantasia").val('');
                $("#txtrazaoSocial").val('');
                $("#txtcnes").val('');
                $("#contatos-container").empty(); // Limpa os contatos adicionados
                alert("Dados salvos com sucesso!");
                CarregarEstabelecimentoSaude();
                location.reload(); // Atualiza a página para limpar os campos dinamicos

            },
            error: function (jqXHR) {
                if (jqXHR.status === 400) {
                    var mensagem = "";
                    $(jqXHR.responseJSON.errors).each(function (index, elemento) {
                        mensagem = mensagem + elemento.errorMessage + "\n";
                    });
                    alert(mensagem);
                } else {
                    alert("Erro ao salvar os dados.");
                }
            }
        });
    });


});






function CarregarEstabelecimentoSaude() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/EstabelecimentoSaude",
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {
            console.log(jsonResult)
            $("#tabela").empty();
            $.each(jsonResult, function (index, item) {

                var linha = $("#linhaEstabelecimentoSaude").clone();
                $(linha).find(".codigo").html(item.id);
                $(linha).find(".nomeFantasia").html(item.nomeFantasia);
                $(linha).find(".razaoSocial").html(item.nomeFantasia)
                $(linha).find(".cnes").html(item.cnes)

                $("#tabela").append(linha)
            })
        },

        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}

function excluir(codigo) {
    $.ajax({
        type: "DELETE",
        url: urlAPI + "api/EstabelecimentoSaude/" + codigo,
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (jsonResult) {

            console.log(jsonResult);
            if (jsonResult) {
                alert('Exclusão efetuada!');
                CarregarEstabelecimentoSaude();
            }
            else
                alert("Exclusão não pode ser efetuada!");


        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
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
            $("#txtid").val(jsonResult.id);
            $("#txtnomeFantasia").val(jsonResult.nomeFantasia);
            $("#txtrazaoSocial").val(jsonResult.razaoSocial);
            $("#txtcnes").val(jsonResult.Cnes);

            // Preencher os contatos, se estiverem disponíveis nos dados retornados
            if (jsonResult.contato) {
                $("#contatos-container").empty();
                jsonResult.contato.forEach(function (contato) {
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
                    CarregarTipoContatoSelect(novoElemento.find('select'));
                    novoElemento.find('select').val(contato.idTipoContato);
                });
            }
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}



function CarregarTipoContato() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/EstabelecimentoSaude/api/TipoContato",
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

function CarregarContatoEstabelecimentoSaude(idEstabelecimentoSaude) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Contato/EstabelecimentoSaude/" + idEstabelecimentoSaude,
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
                CarregarTipoContatoSelect(novoElemento.find('select'));
                novoElemento.find('select').val(contato.idTipoContato);
            });
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}
