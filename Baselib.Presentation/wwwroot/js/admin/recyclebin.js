$(document).ready(function () {
    $('.btn-restore').on('click', function () {
        var id = $(this).data('id');
        var type = $(this).data('type');
        var $btn = $(this);
        
        if (confirm("Bu '" + type + "' kaydını tekrar sisteme geri yüklemek istediğinize emin misiniz?")) {
            var originalText = $btn.html();
            $btn.html('<span class="spinner-border spinner-border-sm"></span> Yükleniyor...').prop('disabled', true);

            // API Endpoint: PUT /api/recyclebin/{type}/{id}/restore
            api.put('/api/recyclebin/' + encodeURIComponent(type) + '/' + id + '/restore', {})
                .then(function (response) {
                    showToast(response.Message || 'Kayıt başarıyla geri yüklendi.', 'success');
                    // Satırı tablodan kaldır (sayfayı yenilemeden)
                    $btn.closest('tr').fadeOut(400, function() { $(this).remove(); });
                })
                .catch(function (error) {
                    var message = error.message || 'Geri yükleme sırasında hata oluştu.';
                    showToast(message, 'error');
                    $btn.html(originalText).prop('disabled', false);
                });
        }
    });
});
