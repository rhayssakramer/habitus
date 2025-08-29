# Habitus - Sistema de Organização Pessoal

## 📋 Visão Geral

O **Habitus** é um sistema completo de organização pessoal inspirado nos aplicativos **Organizador de rotina** e **TickTick**. O sistema oferece funcionalidades de gerenciamento de tarefas, hábitos, calendário e produtividade pessoal.

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
habitus/
├── backend/                    # API REST .NET 8
│   ├── Habitus.API/           # Camada de apresentação (Controllers, Middleware)
│   ├── Habitus.Application/   # Camada de aplicação (Use Cases, DTOs, Services)
│   ├── Habitus.Domain/        # Camada de domínio (Entidades, Interfaces, Regras de negócio)
│   ├── Habitus.Infrastructure/# Camada de infraestrutura (DbContext, Repositories)
│   └── Habitus.Tests/         # Testes unitários e de integração
├── frontend/                  # Aplicação Angular 17+
├── database/                  # Scripts SQL e migrations
└── docs/                      # Documentação do projeto
```

## 🎯 Funcionalidades Principais

### 📝 Gerenciamento de Tarefas (To-Do List)
- ✅ Criação, edição e exclusão de tarefas
- 🏷️ Categorização e tags
- ⏰ Prazos e lembretes
- 📊 Níveis de prioridade (Baixa, Média, Alta, Urgente)
- 🔄 Tarefas recorrentes
- 📎 Anexos e comentários
- 📈 Subtarefas e hierarquia

### 🔄 Sistema de Hábitos
- 🎯 Criação e acompanhamento de hábitos
- 📅 Frequência configurável (diária, semanal, mensal)
- 🔥 Sistema de streaks (sequências)
- 📊 Relatórios de progresso
- 🏆 Gamificação e conquistas

### 📅 Calendário Integrado
- 📆 Visualização mensal, semanal e diária
- 🔗 Integração com tarefas
- ⏰ Eventos e compromissos
- 🔔 Sistema de lembretes
- 🔄 Eventos recorrentes

### 👤 Gestão de Usuários
- 🔐 Autenticação JWT
- 👤 Perfis personalizáveis
- 🎨 Temas e personalização

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **MySQL** - Banco de dados
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados
- **MediatR** - Padrão Mediator
- **JWT Bearer** - Autenticação
- **Swagger** - Documentação da API

### Frontend
- **Angular 17+** - Framework SPA
- **TypeScript** - Linguagem
- **Angular Material** - Componentes UI
- **RxJS** - Programação reativa
- **NgRx** - Gerenciamento de estado

### Banco de Dados
- **MySQL 8.0+** - Sistema de gerenciamento

## 📊 Modelo de Dados

### Entidades Principais

#### User (Usuário)
```csharp
- Id: Guid
- FirstName: string
- LastName: string  
- Email: string
- PasswordHash: string
- ProfilePicture: string?
- IsEmailConfirmed: bool
- LastLoginAt: DateTime?
```

#### Task (Tarefa)
```csharp
- Id: Guid
- Title: string
- Description: string?
- DueDate: DateTime?
- Priority: TaskPriority (Low, Medium, High, Urgent)
- Status: TaskStatus (Todo, InProgress, Completed, Cancelled)
- IsRecurring: bool
- RecurrenceType: RecurrenceType
- Tags: string (JSON)
- EstimatedMinutes: int
- ActualMinutes: int
- UserId: Guid
- CategoryId: Guid?
- ParentTaskId: Guid?
```

#### Habit (Hábito)
```csharp
- Id: Guid
- Name: string
- Description: string?
- Color: string
- Icon: string?
- TargetFrequency: int
- FrequencyPeriod: RecurrenceType
- StartDate: DateTime
- EndDate: DateTime?
- IsActive: bool
- CurrentStreak: int
- BestStreak: int
- UserId: Guid
- CategoryId: Guid?
```

#### CalendarEvent (Evento)
```csharp
- Id: Guid
- Title: string
- Description: string?
- StartDate: DateTime
- EndDate: DateTime
- IsAllDay: bool
- Color: string
- Location: string?
- IsRecurring: bool
- RecurrenceRule: string?
- UserId: Guid
- TaskId: Guid?
```

## 🚀 Próximos Passos

### Fase 1: Backend - Fundação ✅
- [x] Estrutura do projeto
- [x] Entidades de domínio
- [x] Interfaces de repositório
- [ ] DbContext e configurações
- [ ] Repositories implementados
- [ ] Unit of Work pattern

### Fase 2: Backend - API
- [ ] Controllers base
- [ ] DTOs e AutoMapper
- [ ] Validações com FluentValidation
- [ ] Sistema de autenticação JWT
- [ ] Middleware de tratamento de erros
- [ ] Documentação Swagger

### Fase 3: Frontend - Base
- [ ] Projeto Angular
- [ ] Estrutura de módulos
- [ ] Serviços HTTP
- [ ] Guards e interceptors
- [ ] Componentes base

### Fase 4: Frontend - Funcionalidades
- [ ] Autenticação e login
- [ ] Dashboard principal
- [ ] CRUD de tarefas
- [ ] Sistema de hábitos
- [ ] Calendário integrado

### Fase 5: Melhorias
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Performance e otimização
- [ ] Deploy e CI/CD

## 📁 Estrutura de Arquivos Atual

```
backend/
├── Habitus.sln
├── Habitus.API/
├── Habitus.Application/
├── Habitus.Domain/
│   ├── Entities/
│   │   ├── BaseEntity.cs
│   │   ├── User.cs
│   │   ├── Task.cs
│   │   ├── Category.cs
│   │   ├── Habit.cs
│   │   ├── HabitLog.cs
│   │   ├── CalendarEvent.cs
│   │   ├── EventReminder.cs
│   │   ├── TaskComment.cs
│   │   └── TaskAttachment.cs
│   ├── Enums/
│   │   └── TaskEnums.cs
│   └── Interfaces/
│       ├── IRepository.cs
│       ├── IUnitOfWork.cs
│       └── IRepositories.cs
├── Habitus.Infrastructure/
│   └── Data/
│       └── HabitusDbContext.cs
└── Habitus.Tests/
```

## 🎨 Inspirações de Design

### Organizador de Rotina (Vadzim Shaulouski)
- Interface limpa e minimalista
- Foco na simplicidade
- Cores suaves e agradáveis

### TickTick (Appest Limited)
- Sistema robusto de tarefas
- Calendário integrado
- Funcionalidades avançadas de produtividade

## 💡 Funcionalidades Inovadoras Planejadas

- 🤖 **IA para sugestões de hábitos**
- 📊 **Analytics avançados de produtividade**
- 🎮 **Gamificação completa**
- 📱 **PWA (Progressive Web App)**
- 🔗 **Integrações com Google Calendar, Outlook**
- 📈 **Relatórios personalizados**
- 🌙 **Modo escuro/claro**
- 📴 **Modo offline**

---

**Status do Projeto**: 🚧 Em Desenvolvimento - Fase 1
**Última Atualização**: 28 de Agosto de 2025
