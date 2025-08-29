// Configuração da API
const API_BASE_URL = 'https://localhost:7001/api';

// Estado do formulário
let currentStep = 1;
const totalSteps = 4;
let formData = {};

// Inicialização
document.addEventListener('DOMContentLoaded', function() {
    initializeForm();
    setupEventListeners();
    updateProgress();
});

function initializeForm() {
    // Configurar máscaras de input
    setupInputMasks();
    
    // Configurar validação em tempo real
    setupRealTimeValidation();
    
    // Configurar upload de imagem
    setupImageUpload();
    
    // Configurar busca de CEP
    setupCepLookup();
}

function setupEventListeners() {
    // Form submission
    document.getElementById('registerForm').addEventListener('submit', handleFormSubmit);
    
    // Enter key navigation
    document.addEventListener('keypress', function(e) {
        if (e.key === 'Enter' && e.target.tagName !== 'TEXTAREA') {
            e.preventDefault();
            if (currentStep < totalSteps) {
                nextStep();
            } else {
                document.getElementById('registerForm').dispatchEvent(new Event('submit'));
            }
        }
    });
}

function setupInputMasks() {
    // Máscara para telefone
    const phoneInput = document.getElementById('phone');
    phoneInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length <= 11) {
            value = value.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
            if (value.length < 14) {
                value = value.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
            }
        }
        e.target.value = value;
    });

    // Máscara para CEP
    const cepInput = document.getElementById('zipCode');
    cepInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length <= 8) {
            value = value.replace(/(\d{5})(\d{3})/, '$1-$2');
        }
        e.target.value = value;
    });
}

function setupRealTimeValidation() {
    const inputs = document.querySelectorAll('input[required]');
    
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateField(this);
        });
        
        input.addEventListener('input', function() {
            if (this.classList.contains('is-invalid')) {
                validateField(this);
            }
        });
    });

    // Validação especial para confirmação de senha
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    
    confirmPasswordInput.addEventListener('input', function() {
        if (this.value !== passwordInput.value) {
            setFieldInvalid(this, 'As senhas não coincidem');
        } else {
            setFieldValid(this);
        }
    });

    // Validação de email em tempo real
    const emailInput = document.getElementById('email');
    let emailTimeout;
    
    emailInput.addEventListener('input', function() {
        clearTimeout(emailTimeout);
        const email = this.value;
        
        if (email && isValidEmail(email)) {
            emailTimeout = setTimeout(() => {
                checkEmailAvailability(email);
            }, 1000);
        }
    });
}

function setupImageUpload() {
    const fileInput = document.getElementById('profilePicture');
    const preview = document.querySelector('.profile-preview');
    
    fileInput.addEventListener('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            if (file.size > 5 * 1024 * 1024) {
                showAlert('Arquivo muito grande. Máximo 5MB.', 'warning');
                return;
            }
            
            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
            if (!allowedTypes.includes(file.type)) {
                showAlert('Tipo de arquivo não permitido. Use JPG, PNG ou GIF.', 'warning');
                return;
            }
            
            const reader = new FileReader();
            reader.onload = function(e) {
                preview.innerHTML = `<img src="${e.target.result}" alt="Preview">`;
                formData.profilePicture = file;
            };
            reader.readAsDataURL(file);
        }
    });
}

function setupCepLookup() {
    const cepInput = document.getElementById('zipCode');
    
    cepInput.addEventListener('blur', function() {
        const cep = this.value.replace(/\D/g, '');
        if (cep.length === 8) {
            lookupCep(cep);
        }
    });
}

async function lookupCep(cep) {
    try {
        const response = await fetch(`${API_BASE_URL}/registration/cep/${cep}`);
        if (response.ok) {
            const data = await response.json();
            
            document.getElementById('street').value = data.street || '';
            document.getElementById('neighborhood').value = data.neighborhood || '';
            document.getElementById('city').value = data.city || '';
            document.getElementById('state').value = data.state || '';
            
            // Focar no campo número
            document.getElementById('number').focus();
        }
    } catch (error) {
        console.log('Erro ao buscar CEP:', error);
    }
}

async function checkEmailAvailability(email) {
    try {
        const response = await fetch(`${API_BASE_URL}/registration/check-email/${encodeURIComponent(email)}`);
        if (response.ok) {
            const data = await response.json();
            const emailInput = document.getElementById('email');
            
            if (!data.available) {
                setFieldInvalid(emailInput, 'Este email já está em uso');
            } else {
                setFieldValid(emailInput);
            }
        }
    } catch (error) {
        console.log('Erro ao verificar email:', error);
    }
}

function validateField(field) {
    const value = field.value.trim();
    let isValid = true;
    let message = '';

    // Validação básica de required
    if (field.hasAttribute('required') && !value) {
        isValid = false;
        message = 'Este campo é obrigatório';
    }
    
    // Validações específicas por tipo
    if (value && !isValid) {
        switch (field.type) {
            case 'email':
                if (!isValidEmail(value)) {
                    isValid = false;
                    message = 'Email inválido';
                }
                break;
            case 'password':
                if (value.length < 6) {
                    isValid = false;
                    message = 'A senha deve ter pelo menos 6 caracteres';
                }
                break;
            case 'tel':
                if (value && !isValidPhone(value)) {
                    isValid = false;
                    message = 'Telefone inválido';
                }
                break;
        }
    }
    
    // Validações especiais
    if (field.id === 'confirmPassword') {
        const password = document.getElementById('password').value;
        if (value !== password) {
            isValid = false;
            message = 'As senhas não coincidem';
        }
    }
    
    if (field.id === 'zipCode' && value) {
        if (!/^\d{5}-?\d{3}$/.test(value)) {
            isValid = false;
            message = 'CEP deve estar no formato 00000-000';
        }
    }

    if (isValid) {
        setFieldValid(field);
    } else {
        setFieldInvalid(field, message);
    }

    return isValid;
}

function setFieldValid(field) {
    field.classList.remove('is-invalid');
    field.classList.add('is-valid');
    
    const feedback = field.parentNode.querySelector('.invalid-feedback');
    if (feedback) feedback.textContent = '';
    
    const icon = field.parentNode.querySelector('.validation-icon');
    if (icon) {
        icon.className = 'fas fa-check validation-icon';
    }
}

function setFieldInvalid(field, message) {
    field.classList.remove('is-valid');
    field.classList.add('is-invalid');
    
    const feedback = field.parentNode.querySelector('.invalid-feedback');
    if (feedback) feedback.textContent = message;
    
    const icon = field.parentNode.querySelector('.validation-icon');
    if (icon) {
        icon.className = 'fas fa-times validation-icon';
    }
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidPhone(phone) {
    const phoneRegex = /^\(\d{2}\)\s\d{4,5}-\d{4}$/;
    return phoneRegex.test(phone);
}

function nextStep() {
    if (validateCurrentStep()) {
        collectCurrentStepData();
        
        if (currentStep < totalSteps) {
            // Esconder step atual
            document.querySelector(`.form-step[data-step="${currentStep}"]`).classList.remove('active');
            
            // Marcar como completo
            document.querySelector(`.step[data-step="${currentStep}"]`).classList.add('completed');
            
            // Avançar step
            currentStep++;
            
            // Mostrar próximo step
            document.querySelector(`.form-step[data-step="${currentStep}"]`).classList.add('active');
            document.querySelector(`.step[data-step="${currentStep}"]`).classList.add('active');
            
            // Atualizar progresso
            updateProgress();
            
            // Se for o último step, mostrar resumo
            if (currentStep === totalSteps) {
                showSummary();
            }
            
            // Scroll para o topo
            document.querySelector('.register-card').scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    }
}

function prevStep() {
    if (currentStep > 1) {
        // Esconder step atual
        document.querySelector(`.form-step[data-step="${currentStep}"]`).classList.remove('active');
        document.querySelector(`.step[data-step="${currentStep}"]`).classList.remove('active');
        
        // Voltar step
        currentStep--;
        
        // Mostrar step anterior
        document.querySelector(`.form-step[data-step="${currentStep}"]`).classList.add('active');
        document.querySelector(`.step[data-step="${currentStep}"]`).classList.remove('completed');
        
        // Atualizar progresso
        updateProgress();
        
        // Scroll para o topo
        document.querySelector('.register-card').scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }
}

function validateCurrentStep() {
    const currentStepElement = document.querySelector(`.form-step[data-step="${currentStep}"]`);
    const requiredFields = currentStepElement.querySelectorAll('input[required], select[required]');
    let isValid = true;

    requiredFields.forEach(field => {
        if (!validateField(field)) {
            isValid = false;
        }
    });

    // Validações especiais por step
    if (currentStep === 1) {
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        
        if (password !== confirmPassword) {
            setFieldInvalid(document.getElementById('confirmPassword'), 'As senhas não coincidem');
            isValid = false;
        }
    }

    if (currentStep === 4) {
        const acceptTerms = document.getElementById('acceptTerms').checked;
        const acceptPrivacy = document.getElementById('acceptPrivacy').checked;
        
        if (!acceptTerms) {
            document.getElementById('acceptTerms').classList.add('is-invalid');
            isValid = false;
        }
        
        if (!acceptPrivacy) {
            document.getElementById('acceptPrivacy').classList.add('is-invalid');
            isValid = false;
        }
    }

    if (!isValid) {
        showAlert('Por favor, corrija os erros antes de continuar.', 'danger');
    }

    return isValid;
}

function collectCurrentStepData() {
    const currentStepElement = document.querySelector(`.form-step[data-step="${currentStep}"]`);
    const inputs = currentStepElement.querySelectorAll('input, select');
    
    inputs.forEach(input => {
        if (input.type === 'checkbox') {
            formData[input.name] = input.checked;
        } else if (input.type !== 'file') {
            formData[input.name] = input.value;
        }
    });
}

function showSummary() {
    const summaryContainer = document.getElementById('userSummary');
    
    const summaryHTML = `
        <div class="col-md-6">
            <p><strong>Nome:</strong> ${formData.firstName} ${formData.lastName}</p>
            <p><strong>Email:</strong> ${formData.email}</p>
            ${formData.phone ? `<p><strong>Telefone:</strong> ${formData.phone}</p>` : ''}
            ${formData.dateOfBirth ? `<p><strong>Data de Nascimento:</strong> ${formatDate(formData.dateOfBirth)}</p>` : ''}
        </div>
        <div class="col-md-6">
            ${formData.street ? `<p><strong>Endereço:</strong> ${formData.street}, ${formData.number || 'S/N'}</p>` : ''}
            ${formData.city ? `<p><strong>Cidade:</strong> ${formData.city}/${formData.state}</p>` : ''}
            ${formData.zipCode ? `<p><strong>CEP:</strong> ${formData.zipCode}</p>` : ''}
        </div>
    `;
    
    summaryContainer.innerHTML = summaryHTML;
}

function updateProgress() {
    const progressFill = document.getElementById('progressFill');
    const progress = (currentStep / totalSteps) * 100;
    progressFill.style.width = `${progress}%`;
}

async function handleFormSubmit(e) {
    e.preventDefault();
    
    if (!validateCurrentStep()) {
        return;
    }
    
    // Coletar dados do último step
    collectCurrentStepData();
    
    // Mostrar loading
    showLoading(true);
    
    try {
        // Preparar dados para envio
        const registrationData = {
            firstName: formData.firstName,
            lastName: formData.lastName,
            email: formData.email,
            password: formData.password,
            confirmPassword: formData.confirmPassword,
            phone: formData.phone || null,
            dateOfBirth: formData.dateOfBirth || null,
            street: formData.street || null,
            number: formData.number || null,
            complement: formData.complement || null,
            neighborhood: formData.neighborhood || null,
            city: formData.city || null,
            state: formData.state || null,
            zipCode: formData.zipCode || null,
            country: formData.country || 'Brasil',
            acceptTerms: formData.acceptTerms,
            acceptPrivacyPolicy: formData.acceptPrivacy,
            newsletterOptIn: formData.newsletterOptIn || false
        };
        
        // Enviar dados
        const response = await fetch(`${API_BASE_URL}/registration/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(registrationData)
        });
        
        const data = await response.json();
        
        if (response.ok) {
            // Salvar token se fornecido
            if (data.token) {
                localStorage.setItem('authToken', data.token);
                localStorage.setItem('refreshToken', data.refreshToken);
                localStorage.setItem('user', JSON.stringify(data.user));
            }
            
            // Upload da foto se houver
            if (formData.profilePicture) {
                await uploadProfilePicture(data.token);
            }
            
            showAlert('Conta criada com sucesso! Redirecionando...', 'success');
            
            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 2000);
            
        } else {
            showAlert(data.message || 'Erro ao criar conta', 'danger');
        }
        
    } catch (error) {
        console.error('Erro ao registrar:', error);
        showAlert('Erro de conexão. Tente novamente.', 'danger');
    } finally {
        showLoading(false);
    }
}

async function uploadProfilePicture(token) {
    try {
        const formData = new FormData();
        formData.append('file', formData.profilePicture);
        
        await fetch(`${API_BASE_URL}/registration/profile/picture`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });
    } catch (error) {
        console.log('Erro ao fazer upload da foto:', error);
    }
}

function showLoading(show) {
    const form = document.getElementById('registerForm');
    const loading = document.getElementById('loadingSpinner');
    
    if (show) {
        form.style.display = 'none';
        loading.style.display = 'block';
    } else {
        form.style.display = 'block';
        loading.style.display = 'none';
    }
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
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
}

// Utility functions para debug
window.debugFormData = () => console.log(formData);
window.debugStep = () => console.log('Current step:', currentStep);
