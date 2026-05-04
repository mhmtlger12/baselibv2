$(document).ready(function () {
    var detailsModal = new bootstrap.Modal(document.getElementById('detailsModal'));

    $('.btn-view-details').on('click', function () {
        var rawData = $(this).attr('data-details');
        try {
            // JSON formatında ise güzel (pretty) şekilde göster
            var parsed = JSON.parse(rawData);
            var pretty = JSON.stringify(parsed, null, 4);
            $('#jsonDetails').text(pretty);
        } catch (e) {
            // Normal metin ise doğrudan yaz
            $('#jsonDetails').text(rawData);
        }
        
        detailsModal.show();
    });
});
