// Configuração da API
const API_BASE_URL = 'https://localhost:7001/api';

// Estado da aplicação
let userProfile = null;
let originalData = {};
let isEditMode = {
    personal: false,
    address: false
};

// Inicialização
document.addEventListener('DOMContentLoaded', function() {
    checkAuthentication();
    loadUserProfile();
    setupEventListeners();
});

function checkAuthentication() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = 'login.html';
        return;
    }
    
    // TODO: Verificar se o token é válido
}

function setupEventListeners() {
    // Form submissions
    document.getElementById('personalForm').addEventListener('submit', handlePersonalFormSubmit);
    document.getElementById('addressForm').addEventListener('submit', handleAddressFormSubmit);
    document.getElementById('passwordForm').addEventListener('submit', handlePasswordFormSubmit);
    document.getElementById('preferencesForm').addEventListener('submit', handlePreferencesFormSubmit);
    
    // Photo upload
    document.getElementById('photoUpload').addEventListener('change', handlePhotoUpload);
    
    // CEP lookup
    document.getElementById('zipCodeProfile').addEventListener('blur', function() {
        if (isEditMode.address) {
            const cep = this.value.replace(/\D/g, '');
            if (cep.length === 8) {
                lookupCep(cep);
            }
        }
    });
    
    // Phone mask
    document.getElementById('phone').addEventListener('input', function(e) {
        if (!e.target.readOnly) {
            applyPhoneMask(e.target);
        }
    });
    
    // CEP mask
    document.getElementById('zipCodeProfile').addEventListener('input', function(e) {
        if (!e.target.readOnly) {
            applyCepMask(e.target);
        }
    });
}

async function loadUserProfile() {
    try {
        showLoading(true);
        
        const token = localStorage.getItem('authToken');
        const response = await fetch(`${API_BASE_URL}/registration/profile`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });
        
        if (response.ok) {
            userProfile = await response.json();
            populateProfileData();
            updateNavigationBar();
        } else if (response.status === 401) {
            // Token inválido
            localStorage.removeItem('authToken');
            window.location.href = 'login.html';
        } else {
            throw new Error('Erro ao carregar perfil');
        }
        
    } catch (error) {
        console.error('Erro ao carregar perfil:', error);
        showAlert('Erro ao carregar dados do perfil', 'danger');
    } finally {
        showLoading(false);
    }
}

function populateProfileData() {
    if (!userProfile) return;
    
    // Header
    document.getElementById('profileName').textContent = userProfile.fullName || 'Nome não informado';
    document.getElementById('profileEmail').textContent = userProfile.email;
    document.getElementById('profileMemberSince').textContent = `Membro desde: ${formatDate(userProfile.createdAt)}`;
    
    // Age
    if (userProfile.age) {
        document.getElementById('profileAge').textContent = userProfile.age + ' anos';
    } else {
        document.getElementById('profileAge').textContent = 'Não informado';
    }
    
    // Profile completion
    const completionPercentage = calculateProfileCompletion();
    document.getElementById('profileProgress').style.width = `${completionPercentage}%`;
    document.getElementById('profileCompletionText').textContent = `${completionPercentage}%`;
    
    // Profile picture
    if (userProfile.profilePicture) {
        document.getElementById('profileAvatar').src = userProfile.profilePicture;
        document.getElementById('profileAvatar').style.display = 'block';
        document.getElementById('profilePlaceholder').style.display = 'none';
    }
    
    // Personal data
    document.getElementById('firstName').value = userProfile.firstName || '';
    document.getElementById('lastName').value = userProfile.lastName || '';
    document.getElementById('email').value = userProfile.email || '';
    document.getElementById('phone').value = userProfile.phone || '';
    document.getElementById('dateOfBirth').value = userProfile.dateOfBirth ? userProfile.dateOfBirth.split('T')[0] : '';
    
    // Account status
    document.getElementById('accountStatus').textContent = userProfile.isActive ? 'Ativa' : 'Inativa';
    document.getElementById('accountStatus').className = `badge ${userProfile.isActive ? 'bg-success' : 'bg-danger'} me-2`;
    
    document.getElementById('emailStatus').textContent = userProfile.isEmailConfirmed ? 'Email Confirmado' : 'Email Pendente';
    document.getElementById('emailStatus').className = `badge ${userProfile.isEmailConfirmed ? 'bg-info' : 'bg-warning'}`;
    
    // Address data
    document.getElementById('zipCodeProfile').value = userProfile.zipCode || '';
    document.getElementById('streetProfile').value = userProfile.street || '';
    document.getElementById('numberProfile').value = userProfile.number || '';
    document.getElementById('complementProfile').value = userProfile.complement || '';
    document.getElementById('neighborhoodProfile').value = userProfile.neighborhood || '';
    document.getElementById('cityProfile').value = userProfile.city || '';
    document.getElementById('stateProfile').value = userProfile.state || '';
    document.getElementById('countryProfile').value = userProfile.country || 'Brasil';
}

function updateNavigationBar() {
    if (!userProfile) return;
    
    document.getElementById('navUserName').textContent = userProfile.firstName || 'Usuário';
    
    if (userProfile.profilePicture) {
        document.getElementById('navAvatar').src = userProfile.profilePicture;
        document.getElementById('navAvatar').style.display = 'inline';
        document.getElementById('navDefaultAvatar').style.display = 'none';
    }
}

function calculateProfileCompletion() {
    if (!userProfile) return 0;
    
    const fields = [
        userProfile.firstName,
        userProfile.lastName,
        userProfile.email,
        userProfile.phone,
        userProfile.dateOfBirth,
        userProfile.street,
        userProfile.number,
        userProfile.city,
        userProfile.state,
        userProfile.zipCode
    ];
    
    const completedFields = fields.filter(field => field && field.trim() !== '').length;
    return Math.round((completedFields / fields.length) * 100);
}

function toggleEditMode(section) {
    isEditMode[section] = !isEditMode[section];
    
    if (isEditMode[section]) {
        // Entrar em modo de edição
        enableEditMode(section);
        // Salvar dados originais
        originalData[section] = getCurrentFormData(section);
    } else {
        // Sair do modo de edição sem salvar
        disableEditMode(section);
        // Restaurar dados originais
        restoreFormData(section);
    }
}

function enableEditMode(section) {
    const form = document.getElementById(`${section}Form`);
    const inputs = form.querySelectorAll('input, select');
    const editBtn = document.getElementById(`edit${capitalizeFirst(section)}Btn`);
    const actions = document.getElementById(`${section}FormActions`);
    
    // Habilitar campos
    inputs.forEach(input => {
        if (input.id !== 'email') { // Email não deve ser editável
            input.readOnly = false;
            input.disabled = false;
        }
    });
    
    // Atualizar botão
    editBtn.innerHTML = '<i class="fas fa-times me-1"></i> Cancelar';
    editBtn.className = 'btn btn-outline-secondary btn-sm';
    
    // Mostrar ações
    actions.classList.remove('d-none');
    
    // Focar no primeiro campo
    const firstInput = form.querySelector('input:not([readonly]):not([disabled])');
    if (firstInput) firstInput.focus();
}

function disableEditMode(section) {
    const form = document.getElementById(`${section}Form`);
    const inputs = form.querySelectorAll('input, select');
    const editBtn = document.getElementById(`edit${capitalizeFirst(section)}Btn`);
    const actions = document.getElementById(`${section}FormActions`);
    
    // Desabilitar campos
    inputs.forEach(input => {
        input.readOnly = true;
        input.disabled = true;
    });
    
    // Restaurar botão
    editBtn.innerHTML = '<i class="fas fa-edit me-1"></i> Editar';
    editBtn.className = 'btn btn-outline-primary btn-sm';
    
    // Esconder ações
    actions.classList.add('d-none');
    
    isEditMode[section] = false;
}

function cancelEdit(section) {
    // Restaurar dados originais
    restoreFormData(section);
    // Sair do modo de edição
    disableEditMode(section);
}

function getCurrentFormData(section) {
    const form = document.getElementById(`${section}Form`);
    const formData = new FormData(form);
    const data = {};
    
    for (let [key, value] of formData.entries()) {
        data[key] = value;
    }
    
    return data;
}

function restoreFormData(section) {
    if (!originalData[section]) return;
    
    const form = document.getElementById(`${section}Form`);
    
    Object.keys(originalData[section]).forEach(key => {
        const input = form.querySelector(`[name="${key}"]`);
        if (input) {
            input.value = originalData[section][key];
        }
    });
}

async function handlePersonalFormSubmit(e) {
    e.preventDefault();
    
    try {
        const formData = getCurrentFormData('personal');
        const token = localStorage.getItem('authToken');
        
        const response = await fetch(`${API_BASE_URL}/registration/profile`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });
        
        if (response.ok) {
            showAlert('Dados pessoais atualizados com sucesso!', 'success');
            disableEditMode('personal');
            await loadUserProfile(); // Recarregar dados
        } else {
            const error = await response.json();
            showAlert(error.message || 'Erro ao atualizar dados', 'danger');
        }
        
    } catch (error) {
        console.error('Erro ao salvar dados pessoais:', error);
        showAlert('Erro de conexão. Tente novamente.', 'danger');
    }
}

async function handleAddressFormSubmit(e) {
    e.preventDefault();
    
    try {
        const formData = getCurrentFormData('address');
        const token = localStorage.getItem('authToken');
        
        // Combinar com dados pessoais
        const fullData = {
            firstName: userProfile.firstName,
            lastName: userProfile.lastName,
            email: userProfile.email,
            ...formData
        };
        
        const response = await fetch(`${API_BASE_URL}/registration/profile`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(fullData)
        });
        
        if (response.ok) {
            showAlert('Endereço atualizado com sucesso!', 'success');
            disableEditMode('address');
            await loadUserProfile(); // Recarregar dados
        } else {
            const error = await response.json();
            showAlert(error.message || 'Erro ao atualizar endereço', 'danger');
        }
        
    } catch (error) {
        console.error('Erro ao salvar endereço:', error);
        showAlert('Erro de conexão. Tente novamente.', 'danger');
    }
}

async function handlePasswordFormSubmit(e) {
    e.preventDefault();
    
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmNewPassword = document.getElementById('confirmNewPassword').value;
    
    if (newPassword !== confirmNewPassword) {
        showAlert('A nova senha e a confirmação não coincidem', 'danger');
        return;
    }
    
    if (newPassword.length < 6) {
        showAlert('A nova senha deve ter pelo menos 6 caracteres', 'danger');
        return;
    }
    
    try {
        const token = localStorage.getItem('authToken');
        
        const response = await fetch(`${API_BASE_URL}/auth/change-password`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                currentPassword,
                newPassword
            })
        });
        
        if (response.ok) {
            showAlert('Senha alterada com sucesso!', 'success');
            document.getElementById('passwordForm').reset();
        } else {
            const error = await response.json();
            showAlert(error.message || 'Erro ao alterar senha', 'danger');
        }
        
    } catch (error) {
        console.error('Erro ao alterar senha:', error);
        showAlert('Erro de conexão. Tente novamente.', 'danger');
    }
}

async function handlePreferencesFormSubmit(e) {
    e.preventDefault();
    showAlert('Preferências salvas com sucesso!', 'success');
}

async function handlePhotoUpload(e) {
    const file = e.target.files[0];
    if (!file) return;
    
    // Validações
    if (file.size > 5 * 1024 * 1024) {
        showAlert('Arquivo muito grande. Máximo 5MB.', 'warning');
        return;
    }
    
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    if (!allowedTypes.includes(file.type)) {
        showAlert('Tipo de arquivo não permitido. Use JPG, PNG ou GIF.', 'warning');
        return;
    }
    
    try {
        const token = localStorage.getItem('authToken');
        const formData = new FormData();
        formData.append('file', file);
        
        const response = await fetch(`${API_BASE_URL}/registration/profile/picture`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });
        
        if (response.ok) {
            const result = await response.json();
            showAlert('Foto de perfil atualizada com sucesso!', 'success');
            
            // Atualizar preview
            const reader = new FileReader();
            reader.onload = function(e) {
                document.getElementById('profileAvatar').src = e.target.result;
                document.getElementById('profileAvatar').style.display = 'block';
                document.getElementById('profilePlaceholder').style.display = 'none';
                
                // Atualizar navbar também
                document.getElementById('navAvatar').src = e.target.result;
                document.getElementById('navAvatar').style.display = 'inline';
                document.getElementById('navDefaultAvatar').style.display = 'none';
            };
            reader.readAsDataURL(file);
            
        } else {
            const error = await response.json();
            showAlert(error.message || 'Erro ao fazer upload da foto', 'danger');
        }
        
    } catch (error) {
        console.error('Erro ao fazer upload:', error);
        showAlert('Erro de conexão. Tente novamente.', 'danger');
    }
}

async function lookupCep(cep) {
    try {
        const response = await fetch(`${API_BASE_URL}/registration/cep/${cep}`);
        if (response.ok) {
            const data = await response.json();
            
            document.getElementById('streetProfile').value = data.street || '';
            document.getElementById('neighborhoodProfile').value = data.neighborhood || '';
            document.getElementById('cityProfile').value = data.city || '';
            document.getElementById('stateProfile').value = data.state || '';
            
            // Focar no campo número
            document.getElementById('numberProfile').focus();
        }
    } catch (error) {
        console.log('Erro ao buscar CEP:', error);
    }
}

function triggerFileUpload() {
    document.getElementById('photoUpload').click();
}

function applyPhoneMask(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length <= 11) {
        value = value.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
        if (value.length < 14) {
            value = value.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
        }
    }
    input.value = value;
}

function applyCepMask(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length <= 8) {
        value = value.replace(/(\d{5})(\d{3})/, '$1-$2');
    }
    input.value = value;
}

function showLoading(show) {
    // TODO: Implementar indicador de loading
}

function showAlert(message, type) {
    const alertContainer = document.getElementById('alertContainer');
    
    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    alertContainer.innerHTML = '';
    alertContainer.appendChild(alert);
    
    // Auto dismiss after 5 seconds
    setTimeout(() => {
        if (alert.parentNode) {
            alert.remove();
        }
    }, 5000);
    
    // Scroll to top to show alert
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

function capitalizeFirst(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

// Utility functions para debug
window.debugProfile = () => console.log(userProfile);
window.debugEditMode = () => console.log(isEditMode);
