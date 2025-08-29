# Configuração do Banco de Dados MySQL

## Pré-requisitos
- MySQL 8.0 ou superior
- .NET 8 SDK

## Configuração Inicial

### 1. Criar o banco de dados
```sql
CREATE DATABASE habitus_db;
CREATE USER 'habitus_user'@'localhost' IDENTIFIED BY 'HabitusPass123!';
GRANT ALL PRIVILEGES ON habitus_db.* TO 'habitus_user'@'localhost';
FLUSH PRIVILEGES;
```

### 2. String de Conexão
Adicionar no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=habitus_db;User=habitus_user;Password=HabitusPass123!;CharSet=utf8mb4;"
  }
}
```

### 3. Comandos de Migration
```bash
# Instalar ferramenta global do EF
dotnet tool install --global dotnet-ef

# Criar primeira migration
dotnet ef migrations add InitialCreate --project Habitus.Infrastructure --startup-project Habitus.API

# Aplicar migration ao banco
dotnet ef database update --project Habitus.Infrastructure --startup-project Habitus.API
```

## Estrutura das Tabelas

As seguintes tabelas serão criadas:
- `Users` - Usuários do sistema
- `Categories` - Categorias para organização
- `Tasks` - Tarefas do usuário
- `TaskComments` - Comentários das tarefas
- `TaskAttachments` - Anexos das tarefas
- `Habits` - Hábitos do usuário
- `HabitLogs` - Registro de execução dos hábitos
- `CalendarEvents` - Eventos do calendário
- `EventReminders` - Lembretes dos eventos
