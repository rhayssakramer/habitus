# 🎯 Habitus Frontend Angular

Sistema de Organização Pessoal desenvolvido em Angular 17+ com Bootstrap 5 e TypeScript.

## 🚀 Como Executar

### Pré-requisitos
- Node.js 18+ 
- npm ou yarn
- Angular CLI 17+

### Instalação

1. **Instalar Node.js** (se não tiver):
   - Baixar de: https://nodejs.org/
   - Versão recomendada: LTS (18+)

2. **Instalar Angular CLI**:
   ```bash
   npm install -g @angular/cli
   ```

3. **Instalar dependências**:
   ```bash
   cd frontend-angular
   npm install
   ```

4. **Executar o projeto**:
   ```bash
   ng serve
   # ou
   npm start
   ```

5. **Acessar**:
   - Frontend: http://localhost:4200
   - O backend deve estar rodando em: https://localhost:7001

## 📁 Estrutura do Projeto

```
src/
├── app/
│   ├── components/          # Componentes da aplicação
│   │   ├── login/          # Tela de login
│   │   ├── register/       # Tela de cadastro (3 steps)
│   │   ├── profile/        # Perfil do usuário
│   │   └── dashboard/      # Dashboard principal
│   ├── services/           # Serviços Angular
│   │   └── auth.service.ts # Autenticação e API
│   ├── models/             # Interfaces TypeScript
│   │   └── user.model.ts   # Modelos de usuário
│   ├── guards/             # Guards de rota
│   │   └── auth.guard.ts   # Proteção de rotas
│   └── app.module.ts       # Módulo principal
├── environments/           # Configurações de ambiente
├── assets/                 # Arquivos estáticos
└── styles.scss            # Estilos globais
```

## 🎨 Funcionalidades Implementadas

### ✅ Autenticação
- [x] Login com email/senha
- [x] Cadastro multi-etapas (3 steps)
- [x] Logout
- [x] Proteção de rotas
- [x] Refresh token automático

### ✅ Perfil de Usuário
- [x] Visualização de perfil
- [x] Edição de dados pessoais
- [x] Edição de endereço
- [x] Upload de foto de perfil
- [x] Alteração de senha

### ✅ Recursos Avançados
- [x] Busca automática de CEP
- [x] Máscaras para telefone e CEP
- [x] Validações em tempo real
- [x] Feedback visual (loading, erros, sucesso)
- [x] Design responsivo
- [x] Tema personalizado

## 🔧 Configuração

### Environment
Edite `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',  // URL da API
  appName: 'Habitus',
  version: '1.0.0'
};
```

### Rotas
As rotas estão configuradas em `app.module.ts`:

- `/login` - Tela de login
- `/register` - Cadastro multi-etapas
- `/profile` - Perfil do usuário (protegida)
- `/dashboard` - Dashboard (protegida)

## 🎨 Design System

### Cores
- Primary: `#667eea`
- Secondary: `#764ba2`
- Gradiente: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`

### Componentes
- Bootstrap 5.3
- Font Awesome 6.5
- Fonte: Inter (Google Fonts)
- Cards com shadow suave
- Botões com hover effects
- Formulários com validação visual

## 📱 Responsividade

- ✅ Mobile First
- ✅ Tablet friendly
- ✅ Desktop otimizado
- ✅ Touch friendly

## 🔒 Segurança

- JWT Token storage
- Refresh token automático
- Guards de rota
- Validação client-side
- HTTPS ready

## 🚧 Próximas Funcionalidades

- [ ] Dashboard com estatísticas
- [ ] Sistema de tarefas
- [ ] Calendário integrado
- [ ] Notificações
- [ ] Modo escuro
- [ ] PWA (Progressive Web App)

## 📝 Scripts Disponíveis

```bash
# Desenvolvimento
ng serve                    # Rodar em modo dev
ng serve --open            # Rodar e abrir navegador

# Build
ng build                   # Build para produção
ng build --watch          # Build com watch mode

# Testes
ng test                    # Rodar testes unitários
ng e2e                     # Testes end-to-end

# Análise
ng lint                    # Verificar código
ng analyze                 # Analisar bundle
```

## 🔗 Integração com Backend

O frontend se comunica com a API .NET através dos endpoints:

- `POST /api/auth/login` - Login
- `POST /api/registration` - Cadastro
- `GET /api/registration/profile` - Buscar perfil
- `PUT /api/registration/profile` - Atualizar perfil
- `POST /api/registration/profile/picture` - Upload foto
- `GET /api/registration/cep/{cep}` - Buscar CEP

## 🎯 Como Usar

1. **Primeiro acesso**: Clique em "Cadastre-se"
2. **Preencha Step 1**: Dados pessoais e senha
3. **Preencha Step 2**: Endereço completo (CEP busca automática)
4. **Step 3**: Adicione foto (opcional)
5. **Faça login** com email e senha
6. **Gerencie perfil**: Edite dados, foto, senha

## 💡 Dicas

- CEP é preenchido automaticamente
- Telefone usa máscara brasileira
- Senhas devem ter 6+ caracteres
- Fotos limitadas a 5MB
- Interface totalmente em português

---

**Desenvolvido com ❤️ usando Angular + Bootstrap**
