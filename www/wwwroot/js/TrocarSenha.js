$(document).ready(function () {
    $("#btnTrocarSenha").click(function () {
        trocarSenha();
    });
});

function trocarSenha() {
    const senhaAtual = $("#txtsenhaAtual").val();
    const novaSenha = $("#txtnovaSenha").val();
    const confirmarSenha = $("#txtconfirmarSenha").val();

    if (novaSenha !== confirmarSenha) {
        alert("A nova senha e a confirmação não correspondem.");
        return;
    }

    const resultado = {
        "idUsuario": getQueryParameter("id"),
        "senhaAtual": senhaAtual,
        "novaSenha": novaSenha
    };

    $.ajax({
        type: "POST",
        url: "https://localhost:7034/api/Seguranca/TrocarSenha",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(resultado),
        dataType: "json",
        success: function (response) {
            alert(response.mensagem); // Exibe a mensagem de sucesso
            window.location.href = "Login"; // Redirecionar para a página de login
        },
        error: function (jqXHR) {
            if (jqXHR.status === 400 || jqXHR.status === 404) {
                alert("Erro ao trocar a senha: " + jqXHR.responseJSON.mensagem);
            } else {
                alert("Erro inesperado: " + jqXHR.statusText);
            }
        }
    });
}

function getQueryParameter(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}
