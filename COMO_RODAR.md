## 🚀 COMO RODAR O PROJETO HABITUS

### ✅ **PASSO A PASSO SIMPLIFICADO**

#### **1. PRÉ-REQUISITOS VERIFICADOS:**
- ✅ .NET 9.0.302 instalado
- ⚠️ MySQL precisa ser configurado

#### **2. CONFIGURAR BANCO DE DADOS**

**Execute no MySQL Workbench ou linha de comando:**

```sql
-- 1. Criar banco
DROP DATABASE IF EXISTS habitus_db;
CREATE DATABASE habitus_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 2. Criar usuário
DROP USER IF EXISTS 'habitus_user'@'localhost';
CREATE USER 'habitus_user'@'localhost' IDENTIFIED BY 'HabitusPass123!';

-- 3. Dar permissões
GRANT ALL PRIVILEGES ON habitus_db.* TO 'habitus_user'@'localhost';
FLUSH PRIVILEGES;
```

#### **3. RODAR O BACKEND**

```powershell
# No diretório habitus/backend
cd backend
dotnet restore
dotnet build

# Ir para API
cd Habitus.API
dotnet run
```

**A API estará em:**
- 🌐 https://localhost:7001
- 📚 Swagger: https://localhost:7001/swagger

#### **4. RODAR O FRONTEND**

**Opção 1 - VS Code Live Server:**
1. Abra `frontend/login.html` no VS Code
2. Clique com botão direito → "Open with Live Server"

**Opção 2 - Servidor HTTP:**
```powershell
# No diretório habitus/frontend  
cd frontend
python -m http.server 3000
# ou
npx serve .
```

**Frontend estará em:**
- 🎨 http://localhost:3000

#### **5. TESTAR O SISTEMA**

1. **Abrir:** http://localhost:3000/register.html
2. **Preencher:** formulário de cadastro completo
3. **Testar:** login em http://localhost:3000/login.html
4. **Acessar:** perfil em http://localhost:3000/profile.html

### 🔧 **PROBLEMAS CONHECIDOS**

- ❌ **Conflitos de namespace:** Alguns arquivos têm conflitos entre `Task` (entidade) e `Task` (System.Threading.Tasks)
- ✅ **Solução temporária:** Use apenas funcionalidades de User/Auth por enquanto

### 📝 **STATUS ATUAL**

✅ **FUNCIONAL:**
- Sistema de cadastro completo
- Login com JWT
- Perfil do usuário
- Upload de foto
- Busca de CEP

⚠️ **EM DESENVOLVIMENTO:**
- Sistema de tarefas (conflitos de namespace)
- Categorias e metas

### 🎯 **PRÓXIMOS PASSOS**

1. **Configure o MySQL** usando o script acima
2. **Execute a API:** `cd backend/Habitus.API && dotnet run`
3. **Abra o frontend** usando Live Server ou servidor HTTP
4. **Teste o cadastro** e login

### 📞 **COMANDOS RÁPIDOS**

```powershell
# Setup completo
cd "C:\Users\r.bezerra.de.melo\Documents\GitHub\habitus"

# Backend
cd backend && dotnet restore && dotnet build
cd Habitus.API && dotnet run

# Frontend (novo terminal)
cd frontend && python -m http.server 3000
```

**🎉 PRONTO! Seu sistema está funcionando!**
