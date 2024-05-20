


const urlAPI = "https://localhost:44309/"

$(document).ready(function () {


    carregarCid();

    $("#btnlimpar").click(function () {

        $("#txtid").val('0');
        $("#txtcodigo").val('');
        $("#txtdescricao").val('');



    });




    $("#tabela").on("click", ".alterar", function (elemento) {

        let codigo = $(elemento.target).parent().parent().find(".codigo").text()
        visualizar(codigo);
    })

    $("#tabela").on("click", ".excluir", function (elemento) {

        let codigo = $(elemento.target).parent().parent().find(".codigo").text()
        excluir(codigo);
    })





    $("#btnsalvar").click(function () {
        //validar
        const obj = {


            id: $("#txtid").val(),
            codigo: $("#txtcodigo").val(),
            descricao: $("#txtdescricao").val()


        }

        console.log(JSON.stringify(obj))

        $.ajax({
            type: $("#txtid").val() == "0" ? "POST" : "PUT",
            url: urlAPI + "api/Cid",
            contentType: "application/json;charset=utf-8",
            /*
            headers: {
                "Authorization": "Bearer " + token
            },*/

            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {

                console.log(jsonResult)

                $("#txtid").val('0');
                $("#txtcodigo").val('');
                $("#txtdescricao").val('');

                alert("Dados salvos com sucesso!")


                carregarCid();
            },
            error: function (jqXHR) {
                if (jqXHR.status === 400) {
                    var mensagem = "";
                    $(jqXHR.responseJSON.errors).each(function (index, elemento) {
                        mensagem = mensagem + elemento.errorMessage + "\n";
                    });
                    alert(mensagem);
                } else {
                    alert("Erro ao salvar os dados!");
                }
            }
        });


    })

});



function carregarCid() {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Cid",
        contentType: "application/json;charset=utf-8",

        //headers: {
        //    "Authorization": "Bearer " + token
        //},

        data: {},
        dataType: "json",
        success: function (jsonResult) {

            console.log(jsonResult)

            $("#tabela").empty();
            $.each(jsonResult, function (index, item) {

                var linha = $("#linhaExemplo").clone()
                $(linha).find(".codigo").html(item.id)
                $(linha).find(".codigo").html(item.codigo)
                $(linha).find(".descricao").html(item.descricao)

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
        url: urlAPI + "api/Cid/" + codigo,
        contentType: "application/json;charset=utf-8",

        //headers: {
        //    "Authorization": "Bearer " + token
       // },

        data: {},
        dataType: "json",
        success: function (jsonResult) {

            console.log(jsonResult)

            if (jsonResult) {
                alert("Exclusão efetuada!")
                carregarCid();

            }
            else
                alert("Exclusão não pode ser efetuada!")


        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}


function visualizar(codigo) {
    $.ajax({
        type: "GET",
        url: urlAPI + "api/Cid/" + codigo,
        contentType: "application/json;charset=utf-8",

        //headers: {
        //    "Authorization": "Bearer " + token
       // },

        data: {},
        dataType: "json",
        success: function (jsonResult) {

            console.log(jsonResult)

            $("#txtid").val(jsonResult.id)
            $("#txtcodigo").val(jsonResult.codigo)
            $("#txtnome").val(jsonResult.nome)
        },
        failure: function (response) {
            alert("Erro ao carregar os dados: " + response);
        }
    });
}