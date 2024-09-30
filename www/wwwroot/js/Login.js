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
            if (jsonResult.necessitaTrocarSenha) {
                // Redirecionar para a página de troca de senha
                window.location.href = "TrocarSenha?id=" + jsonResult.idUsuario;
            } else {
                console.log("Login bem-sucedido");
                // Armazenar o token e o nome do usuário no sessionStorage
                sessionStorage.setItem('token', jsonResult.token);
                sessionStorage.setItem('nomeUsuario', resultado.nomeUsuario); // Armazenar o nome de usuário
                window.location.href = "Index"; // Redirecionar para a página inicial ou dashboard
            }
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
