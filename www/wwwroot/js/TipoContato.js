


const urlAPI = "https://localhost:44309/"

$(document).ready(function () {


   

    $("#btnlimpar").click(function () {
        $("#txtnome").val('');

        $("#txtid").val('0');



    });




    

    $("#btnsalvar").click(function () {
        //validar
        const obj = {
            id: $("#txtid").val(),
            nome: $("#txtnome").val(),


        }

        console.log(JSON.stringify(obj))

        $.ajax({
            type: $("#txtid").val() == "0" ? "POST" : "PUT",
            url: urlAPI + "api/TipoContato",
            contentType: "application/json;charset=utf-8",
            /*
            headers: {
                "Authorization": "Bearer " + token
            },*/

            data: JSON.stringify(obj),
            dataType: "json",
            success: function (jsonResult) {

                console.log(jsonResult)
                $("#txtnome").val('');

                $("#txtid").val('0');


                alert("Dados Salvos com sucesso!")


                carregarStatusConsulta();
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


    })

});






