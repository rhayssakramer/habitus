# 🎯 Habitus - Sistema de Organização Pessoal

![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Angular](https://img.shields.io/badge/Angular-17+-red)
![MySQL](https://img.shields.io/badge/MySQL-8.0+-orange)

## 📋 Sobre o Projeto

**Habitus** é um sistema completo de organização pessoal desenvolvido com .NET 8, Angular 17+ e MySQL. Inspirado nos aplicativos **Organizador de rotina** (Vadzim Shaulouski) e **TickTick** (Appest Limited), o sistema oferece funcionalidades avançadas para gerenciamento de:

- ✅ **Tarefas** (To-Do List completo)
- 🔄 **Hábitos** (Tracking e streaks)
- 📅 **Calendário** (Eventos e lembretes)
- 📊 **Produtividade** (Analytics e relatórios)

## 🏗️ Arquitetura

```
📁 habitus/
├── 🔧 backend/           # API REST .NET 8 (Clean Architecture)
├── 🎨 frontend/          # Angular 17+ SPA
├── 🗄️ database/         # Scripts MySQL e migrations
└── 📚 docs/             # Documentação completa
```

## 🚀 Status Atual

### ✅ Concluído (Fase 1 + 2A)
- [x] Estrutura base do projeto
- [x] Entidades de domínio completas
- [x] **Sistema de Autenticação JWT**
- [x] **Controllers Auth e Admin**
- [x] **Tela de Login funcional**
- [x] **Dashboard Administrativo**
- [x] Configuração JWT e CORS

### 🔄 Em Desenvolvimento
- [ ] Implementação dos serviços (UserService, AuthService)
- [ ] Implementação dos repositories
- [ ] Migration e testes da API

## 🛠️ Tecnologias

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **MySQL** - Banco de dados
- **AutoMapper** - Mapeamento de objetos
- **MediatR** - Padrão Mediator
- **FluentValidation** - Validação
- **JWT** - Autenticação

### Frontend (Planejado)
- **Angular 17+** - Framework SPA
- **Angular Material** - UI Components
- **TypeScript** - Linguagem
- **RxJS** - Programação reativa

## 📚 Documentação

- [📖 **Documentação Completa**](./docs/README.md) - Visão geral detalhada
- [🗺️ **Roadmap**](./docs/ROADMAP.md) - Plano de desenvolvimento
- [🗄️ **Banco de Dados**](./database/README.md) - Configuração MySQL

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK
- MySQL 8.0+
- Node.js 18+ (para o frontend)

### Backend
```bash
cd backend
dotnet restore
dotnet build
dotnet run --project Habitus.API
```

### Banco de Dados
```sql
CREATE DATABASE habitus_db;
-- Ver database/README.md para configuração completa
```

## 📅 Roadmap

| Fase | Descrição | Prazo Estimado |
|------|-----------|----------------|
| **Fase 1** | Backend - Fundação | 1 semana ✅ |
| **Fase 2** | API REST Completa | 3 semanas |
| **Fase 3** | Frontend Base | 4 semanas |
| **Fase 4** | Funcionalidades UI | 5 semanas |
| **Fase 5** | Otimização & Deploy | 3 semanas |

## 🎯 Funcionalidades Principais

### 📝 Gerenciamento de Tarefas
- Criação e organização de tarefas
- Categorização e prioridades
- Subtarefas e hierarquia
- Anexos e comentários
- Tarefas recorrentes

### 🔄 Sistema de Hábitos
- Tracking de hábitos diários
- Sistema de streaks
- Relatórios de progresso
- Gamificação

### 📅 Calendário Integrado
- Visualizações múltiplas
- Eventos e compromissos
- Integração com tarefas
- Lembretes automáticos

## 🤝 Contribuição

Este é um projeto em desenvolvimento ativo. Sugestões e contribuições são bem-vindas!

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

**Desenvolvido com ❤️ para aumentar a produtividade pessoal**

**Última Atualização**: 28 de Agosto de 2025
Sitemas de Organização Pessoal
