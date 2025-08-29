import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterUserDto, CepResponse } from '../../models/user.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  currentStep = 1;
  totalSteps = 3;
  loading = false;
  error = '';
  success = '';
  selectedFile: File | null = null;
  photoPreview: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.registerForm = this.formBuilder.group({
      // Dados pessoais
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      phone: ['', [Validators.pattern(/^\(\d{2}\) \d{4,5}-\d{4}$/)]],
      dateOfBirth: [''],
      
      // Endereço
      zipCode: ['', [Validators.pattern(/^\d{5}-\d{3}$/)]],
      street: [''],
      number: [''],
      complement: [''],
      neighborhood: [''],
      city: [''],
      state: [''],
      country: ['Brasil']
    }, {
      validators: this.passwordMatchValidator
    });
  }

  // Validador customizado para confirmar senha
  private passwordMatchValidator(control: AbstractControl): { [key: string]: any } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    
    return null;
  }

  get f() {
    return this.registerForm.controls;
  }

  get progressPercentage(): number {
    return (this.currentStep / this.totalSteps) * 100;
  }

  // Navegação entre steps
  nextStep(): void {
    if (this.isCurrentStepValid()) {
      this.currentStep++;
    } else {
      this.markCurrentStepAsTouched();
    }
  }

  prevStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  private isCurrentStepValid(): boolean {
    const step1Fields = ['firstName', 'lastName', 'email', 'password', 'confirmPassword'];
    const step2Fields = ['zipCode', 'street', 'number', 'city', 'state'];
    
    let fieldsToValidate: string[] = [];
    
    switch (this.currentStep) {
      case 1:
        fieldsToValidate = step1Fields;
        break;
      case 2:
        fieldsToValidate = step2Fields.filter(field => 
          this.registerForm.get(field)?.hasError('required')
        );
        break;
      case 3:
        return true; // Step 3 é opcional (foto)
    }
    
    // Verificar se há erros nos campos obrigatórios
    for (const field of fieldsToValidate) {
      const control = this.registerForm.get(field);
      if (control && control.invalid) {
        return false;
      }
    }
    
    // Verificar se as senhas coincidem
    if (this.currentStep === 1 && this.registerForm.hasError('passwordMismatch')) {
      return false;
    }
    
    return true;
  }

  private markCurrentStepAsTouched(): void {
    const step1Fields = ['firstName', 'lastName', 'email', 'password', 'confirmPassword'];
    const step2Fields = ['zipCode', 'street', 'number', 'city', 'state'];
    
    let fieldsToMark: string[] = [];
    
    switch (this.currentStep) {
      case 1:
        fieldsToMark = step1Fields;
        break;
      case 2:
        fieldsToMark = step2Fields;
        break;
    }
    
    fieldsToMark.forEach(field => {
      this.registerForm.get(field)?.markAsTouched();
    });
  }

  // Máscaras e formatação
  applyPhoneMask(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length <= 11) {
      value = value.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
      if (value.length < 14) {
        value = value.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
      }
    }
    this.registerForm.patchValue({ phone: value });
  }

  applyCepMask(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length <= 8) {
      value = value.replace(/(\d{5})(\d{3})/, '$1-$2');
    }
    this.registerForm.patchValue({ zipCode: value });
  }

  // Busca de CEP
  onCepBlur(): void {
    const cep = this.f['zipCode'].value?.replace(/\D/g, '');
    if (cep && cep.length === 8) {
      this.authService.lookupCep(cep).subscribe({
        next: (data: CepResponse) => {
          this.registerForm.patchValue({
            street: data.street,
            neighborhood: data.neighborhood,
            city: data.city,
            state: data.state
          });
        },
        error: (error) => {
          console.log('Erro ao buscar CEP:', error);
        }
      });
    }
  }

  // Upload de foto
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validações
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'Arquivo muito grande. Máximo 5MB.';
        return;
      }
      
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
      if (!allowedTypes.includes(file.type)) {
        this.error = 'Tipo de arquivo não permitido. Use JPG, PNG ou GIF.';
        return;
      }
      
      this.selectedFile = file;
      
      // Preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.photoPreview = e.target?.result as string;
      };
      reader.readAsDataURL(file);
      
      this.error = '';
    }
  }

  removePhoto(): void {
    this.selectedFile = null;
    this.photoPreview = null;
  }

  // Validação de email
  async checkEmailExists(): Promise<void> {
    const email = this.f['email'].value;
    if (email && this.f['email'].valid) {
      this.authService.checkEmailExists(email).subscribe({
        next: (result) => {
          if (result.exists) {
            this.f['email'].setErrors({ emailExists: true });
          }
        },
        error: (error) => {
          console.log('Erro ao verificar email:', error);
        }
      });
    }
  }

  // Submit do formulário
  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const registerData: RegisterUserDto = {
      firstName: this.f['firstName'].value,
      lastName: this.f['lastName'].value,
      email: this.f['email'].value,
      password: this.f['password'].value,
      confirmPassword: this.f['confirmPassword'].value,
      phone: this.f['phone'].value,
      dateOfBirth: this.f['dateOfBirth'].value ? new Date(this.f['dateOfBirth'].value) : undefined,
      street: this.f['street'].value,
      number: this.f['number'].value,
      complement: this.f['complement'].value,
      neighborhood: this.f['neighborhood'].value,
      city: this.f['city'].value,
      state: this.f['state'].value,
      zipCode: this.f['zipCode'].value,
      country: this.f['country'].value
    };

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.success = 'Cadastro realizado com sucesso! Redirecionando para login...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        this.error = error.message || 'Erro ao realizar cadastro';
        this.loading = false;
      }
    });
  }

  private markAllFieldsAsTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}
