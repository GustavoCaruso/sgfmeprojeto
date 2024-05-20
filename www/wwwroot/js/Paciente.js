const urlAPI = "https://localhost:44309/";

$(document).ready(function () {
    // Função para carregar os tipos de contato
    function carregarTiposContato() {
        $.ajax({
            url: urlAPI + "api/TipoContato",
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                // Limpar o select
                $('#tipoContatoSelect').empty();

                // Adicionar uma opção padrão
                $('#tipoContatoSelect').append($('<option>', {
                    value: '',
                    text: 'Selecione o tipo de contato'
                }));

                // Adicionar os tipos de contato ao select
                $.each(data, function (index, tipo) {
                    $('#tipoContatoSelect').append($('<option>', {
                        value: tipo.id,
                        text: tipo.nome
                    }));
                });
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar tipos de contato:', error);
            }
        });
    }

    // Chamar a função para carregar os tipos de contato quando a página carregar
    carregarTiposContato();

    // Evento de mudança no select de tipo de contato
    $('#tipoContatoSelect').change(function () {
        // Aqui você pode adicionar lógica adicional se necessário
    });
});
