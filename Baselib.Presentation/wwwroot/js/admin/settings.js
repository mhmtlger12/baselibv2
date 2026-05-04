$(document).ready(function () {
    var settingModal = new bootstrap.Modal(document.getElementById('settingModal'));

    $('.btn-edit-setting').on('click', function () {
        var id = $(this).data('id');
        var key = $(this).data('key');
        var value = $(this).data('value');

        $('#SettingId').val(id);
        $('#SettingKeyDisplay').text(key);
        $('#SettingValue').val(value);

        settingModal.show();
    });

    $('#settingForm').on('submit', function (e) {
        e.preventDefault();

        var id = $('#SettingId').val();
        var value = $('#SettingValue').val();

        var $btn = $('#btnSaveSetting');
        var originalText = $btn.html();
        $btn.html('<span class="spinner-border spinner-border-sm"></span> Kaydediliyor...').prop('disabled', true);

        api.put('/api/settings/' + id, { Value: value })
            .then(function (response) {
                showToast(response.Message || 'Ayar başarıyla güncellendi.', 'success');
                settingModal.hide();
                setTimeout(function () {
                    location.reload();
                }, 1000);
            })
            .catch(function (error) {
                var message = error.message || 'Bir hata oluştu.';
                showToast(message, 'error');
            })
            .finally(function () {
                $btn.html(originalText).prop('disabled', false);
            });
    });
});
