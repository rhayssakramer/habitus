# 🚀 Plano de Commits Progressivos - Habitus

## 📋 Estratégia de Versionamento

Este documento descreve como fazer commits progressivos que simulam o desenvolvimento natural do projeto.

## 🎯 Commits Planejados (16 commits)

### **1️⃣ SETUP INICIAL (Commits 1-3)**
```
1. feat: setup inicial do projeto habitus
2. docs: adiciona documentação inicial e roadmap  
3. chore: configura estrutura de pastas e gitignore
```

### **2️⃣ BACKEND FOUNDATION (Commits 4-8)**
```
4. feat: estrutura backend clean architecture com .NET 8
5. feat: adiciona entidades de domínio (User, BaseEntity)
6. feat: implementa repository pattern e unit of work
7. feat: configura Entity Framework e migrations
8. feat: setup dependency injection e configurações
```

### **3️⃣ AUTENTICAÇÃO (Commits 9-11)**
```
9. feat: implementa autenticação JWT e controllers
10. feat: adiciona sistema de registro de usuários
11. feat: endpoints de perfil e upload de imagens
```

### **4️⃣ FRONTEND HTML (Commits 12-13)**
```
12. feat: frontend html com login e registro
13. feat: sistema de perfil e upload de fotos
```

### **5️⃣ FRONTEND ANGULAR (Commits 14-16)**
```
14. feat: setup projeto angular com typescript
15. feat: componentes angular para auth e registro
16. feat: integração completa frontend angular com backend
```

## 🔧 Comandos para Executar

Execute os comandos abaixo na ordem:

### Commit 1: Setup Inicial
```bash
git add README.md
git commit -m "feat: setup inicial do projeto habitus

- Sistema de organização pessoal inspirado no TickTick
- Estrutura inicial do repositório
- Tecnologias: .NET 8, Angular 17+, MySQL"
```

### Commit 2: Documentação
```bash
git add docs/
git commit -m "docs: adiciona documentação inicial e roadmap

- Documentação da arquitetura do sistema
- Status de implementação
- Guias de desenvolvimento"
```

### Commit 3: Estrutura
```bash
git add database/ COMO_RODAR.md *.ps1 *.config
git commit -m "chore: configura estrutura de pastas e scripts

- Scripts de configuração do banco MySQL
- Documentação de como rodar o projeto
- Configurações do NuGet e PowerShell"
```

### Commit 4: Backend Base
```bash
git add backend/Habitus.sln backend/*/Habitus.*.csproj
git commit -m "feat: estrutura backend clean architecture com .NET 8

- Clean Architecture com Domain, Application, Infrastructure, API
- Configuração de projetos .NET 8
- Estrutura modular e escalável"
```

### Commit 5: Entidades
```bash
git add backend/Habitus.Domain/Entities/ backend/Habitus.Domain/Enums/
git commit -m "feat: adiciona entidades de domínio (User, BaseEntity)

- BaseEntity com propriedades comuns
- User com dados completos (endereço, foto, etc)
- Enums para tipagem forte"
```

### Commit 6: Repository Pattern
```bash
git add backend/Habitus.Domain/Interfaces/ backend/Habitus.Infrastructure/
git commit -m "feat: implementa repository pattern e unit of work

- Interfaces para repositories e services
- Implementação do padrão Repository
- Unit of Work para transações"
```

### Commit 7: Entity Framework
```bash
git add backend/Habitus.Infrastructure/Data/
git commit -m "feat: configura Entity Framework e migrations

- DbContext configurado para MySQL
- Mapeamento de entidades
- Configurações de relacionamentos"
```

### Commit 8: DI e Config
```bash
git add backend/Habitus.Application/ backend/Habitus.API/
git commit -m "feat: setup dependency injection e configurações

- Configuração de serviços no DI container
- Configurações de JWT, CORS, Swagger
- Estrutura da API"
```

### Commit 9: Autenticação
```bash
git add backend/Habitus.API/Controllers/AuthController.cs
git commit -m "feat: implementa autenticação JWT e controllers

- AuthController com login/logout
- Geração e validação de JWT tokens
- Middleware de autenticação"
```

### Commit 10: Registro
```bash
git add backend/Habitus.Application/DTOs/ backend/Habitus.API/Controllers/RegistrationController.cs
git commit -m "feat: adiciona sistema de registro de usuários

- DTOs para registro e perfil
- Controller de registro completo
- Validações de dados de entrada"
```

### Commit 11: Perfil e Upload
```bash
git add backend/Habitus.Application/Services/
git commit -m "feat: endpoints de perfil e upload de imagens

- UserService com CRUD completo
- Upload de fotos de perfil
- Busca de CEP integrada"
```

### Commit 12: Frontend HTML
```bash
git add frontend/login.html frontend/register.html frontend/*.js
git commit -m "feat: frontend html com login e registro

- Telas de login e registro responsivas
- Formulário multi-etapas para cadastro
- Integração com API via JavaScript"
```

### Commit 13: Sistema de Perfil
```bash
git add frontend/profile.html frontend/profile.js
git commit -m "feat: sistema de perfil e upload de fotos

- Tela de perfil com edição inline
- Upload de fotos com preview
- Máscaras e validações client-side"
```

### Commit 14: Setup Angular
```bash
git add frontend-angular/package.json frontend-angular/angular.json frontend-angular/tsconfig*.json frontend-angular/src/environments/
git commit -m "feat: setup projeto angular com typescript

- Projeto Angular 17+ configurado
- TypeScript, Bootstrap 5, Font Awesome
- Estrutura modular para escalabilidade"
```

### Commit 15: Componentes Angular
```bash
git add frontend-angular/src/app/
git commit -m "feat: componentes angular para auth e registro

- LoginComponent e RegisterComponent
- AuthService com gerenciamento de estado
- Guards para proteção de rotas"
```

### Commit 16: Integração Final
```bash
git add frontend-angular/src/ frontend-angular/README.md
git commit -m "feat: integração completa frontend angular com backend

- Serviços integrados com API REST
- Componentes de perfil e dashboard
- Sistema completo funcional"
```

## 🎯 Benefícios desta Estratégia

1. **📚 História Clara**: Cada commit conta parte da história
2. **🔄 Rollback Fácil**: Pode voltar a qualquer ponto
3. **👥 Colaboração**: Outros devs entendem a evolução
4. **📝 Documentação**: Commits servem como documentação
5. **🎯 Features Isoladas**: Cada funcionalidade em commits separados

## 📋 Próximos Passos

1. Execute os commits na ordem apresentada
2. Teste cada etapa antes de commitar
3. Use mensagens descritivas e consistentes
4. Adicione tags para versões importantes

---

**💡 Dica**: Use `git log --oneline --graph` para visualizar a história!
