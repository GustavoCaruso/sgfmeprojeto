

$(document).ready(function () {
    
    $("#btnentrar").click(function () {
        
        realizarLogin();
    });
});

function realizarLogin() {
    const resultado = {
        "nomeUsuario": $("#txtnomeUsuario").val(),
        "senha": $("#txtsenha").val()
    };

    $.ajax({
        type: "POST",
        url: "https://localhost:7034/api/Seguranca/validaLogin",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(resultado),
        dataType: "json",
        success: function (jsonResult) {
            console.log("Login bem-sucedido");
            sessionStorage.setItem('token', jsonResult.token);
            window.location.href = "Index"; // Redirecionar para a página inicial ou dashboard após o login bem-sucedido
        },
        error: function (jqXHR) {
            if (jqXHR.status === 400) {
                alert("Dados de login inválidos.");
            } else if (jqXHR.status === 401) {
                alert("Usuário ou senha incorretos.");
            } else {
                alert("Erro inesperado: " + jqXHR.statusText);
            }
        }
    });
}

