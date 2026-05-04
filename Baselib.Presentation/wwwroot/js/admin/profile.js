$(document).ready(function () {
    $('#changePasswordForm').on('submit', function (e) {
        e.preventDefault();
        
        var currentPassword = $('#CurrentPassword').val();
        var newPassword = $('#NewPassword').val();
        var confirmPassword = $('#ConfirmPassword').val();
        
        if (newPassword !== confirmPassword) {
            $('#ConfirmPassword').addClass('is-invalid');
            return;
        }
        
        $('#ConfirmPassword').removeClass('is-invalid');
        
        var $btn = $('#btnChangePassword');
        var originalText = $btn.html();
        $btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Güncelleniyor...').prop('disabled', true);
        
        var payload = {
            CurrentPassword: currentPassword,
            NewPassword: newPassword
        };
        
        api.put('/api/profile/password', payload)
            .then(function (response) {
                showToast('Şifreniz başarıyla değiştirildi.', 'success');
                $('#changePasswordForm')[0].reset();
            })
            .catch(function (error) {
                var message = error.message || 'Şifre değiştirilirken bir hata oluştu.';
                showToast(message, 'error');
            })
            .finally(function () {
                $btn.html(originalText).prop('disabled', false);
            });
    });

    // Rol Değiştirme
    document.querySelectorAll('.role-switcher').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const roleId = this.getAttribute('data-role-id');
            const roleName = this.querySelector('h6').innerText;
            if (!roleId) return;

            if (!confirm(`'${roleName}' rolüne geçiş yapmak istediğinize emin misiniz?`)) return;

            showLoading();
            api.post('/api/auth/switch-role/' + roleId)
                .then(function (response) {
                    // Tokenları localStorage'a güncelle (Admin UI kullanıyorsa)
                    localStorage.setItem('accessToken', response.accessToken || response.AccessToken);
                    localStorage.setItem('refreshToken', response.refreshToken || response.RefreshToken);
                    localStorage.setItem('user', JSON.stringify(response.user || response.User));
                    
                    showToast('Rol başarıyla değiştirildi. Sayfa yenileniyor...', 'success');
                    setTimeout(() => {
                        window.location.reload();
                    }, 1000);
                })
                .catch(function (error) {
                    showToast(error.message || 'Rol değiştirilirken hata oluştu.', 'error');
                    hideLoading();
                });
        });
    });
    
    $('#ConfirmPassword').on('input', function() {
        if ($(this).val() === $('#NewPassword').val()) {
            $(this).removeClass('is-invalid');
        } else {
            $(this).addClass('is-invalid');
        }
    });
});
