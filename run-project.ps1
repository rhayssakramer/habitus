# Script para rodar o projeto Habitus completo
# Execute este script no PowerShell como Administrador

Write-Host "🚀 INICIANDO SETUP DO PROJETO HABITUS" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# 1. Verificar pré-requisitos
Write-Host "`n📋 1. VERIFICANDO PRÉ-REQUISITOS..." -ForegroundColor Yellow

# Verificar .NET
$dotnetVersion = dotnet --version 2>$null
if ($dotnetVersion) {
    Write-Host "✅ .NET $dotnetVersion instalado" -ForegroundColor Green
} else {
    Write-Host "❌ .NET não encontrado. Instale o .NET 8 ou superior" -ForegroundColor Red
    exit 1
}

# Verificar MySQL
$mysqlService = Get-Service -Name "MySQL*" -ErrorAction SilentlyContinue
if ($mysqlService) {
    Write-Host "✅ MySQL encontrado: $($mysqlService.DisplayName)" -ForegroundColor Green
} else {
    Write-Host "⚠️  MySQL não encontrado. Certifique-se de que está instalado e rodando" -ForegroundColor Yellow
}

# 2. Configurar backend
Write-Host "`n🔧 2. CONFIGURANDO BACKEND..." -ForegroundColor Yellow
Set-Location "backend"

# Restaurar pacotes
Write-Host "📦 Restaurando pacotes NuGet..."
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao restaurar pacotes" -ForegroundColor Red
    exit 1
}

# Compilar projeto
Write-Host "🔨 Compilando projeto..."
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro na compilação" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Backend configurado com sucesso!" -ForegroundColor Green

# 3. Instruções para o banco
Write-Host "`n🗄️  3. CONFIGURAÇÃO DO BANCO DE DADOS" -ForegroundColor Yellow
Write-Host "Para configurar o banco, execute os seguintes comandos no MySQL:" -ForegroundColor Cyan
Write-Host ""
Write-Host "mysql -u root -p" -ForegroundColor White
Write-Host "source ../database/setup_database.sql" -ForegroundColor White
Write-Host ""

# 4. Como rodar a API
Write-Host "`n🌐 4. COMO RODAR A API" -ForegroundColor Yellow
Write-Host "Execute os comandos:" -ForegroundColor Cyan
Write-Host ""
Write-Host "cd Habitus.API" -ForegroundColor White
Write-Host "dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "A API estará disponível em:" -ForegroundColor Cyan
Write-Host "- https://localhost:7001" -ForegroundColor Green
Write-Host "- http://localhost:5001" -ForegroundColor Green
Write-Host "- Swagger: https://localhost:7001/swagger" -ForegroundColor Green

# 5. Como rodar o frontend
Write-Host "`n🎨 5. COMO RODAR O FRONTEND" -ForegroundColor Yellow
Write-Host "Para testar o frontend, você pode:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Usar Live Server no VS Code:" -ForegroundColor White
Write-Host "   - Abra frontend/login.html" -ForegroundColor Gray
Write-Host "   - Clique com botão direito -> 'Open with Live Server'" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Ou usar um servidor HTTP simples:" -ForegroundColor White
Write-Host "   cd ../frontend" -ForegroundColor Gray
Write-Host "   python -m http.server 3000" -ForegroundColor Gray
Write-Host "   # ou npx serve ." -ForegroundColor Gray
Write-Host ""

# 6. URLs importantes
Write-Host "`n🔗 6. URLS IMPORTANTES" -ForegroundColor Yellow
Write-Host "Backend API: https://localhost:7001" -ForegroundColor Green
Write-Host "Swagger UI: https://localhost:7001/swagger" -ForegroundColor Green
Write-Host "Frontend: http://localhost:3000" -ForegroundColor Green
Write-Host ""

# 7. Próximos passos
Write-Host "`n📝 7. PRÓXIMOS PASSOS" -ForegroundColor Yellow
Write-Host "1. Configure o banco de dados usando o script em database/setup_database.sql" -ForegroundColor Cyan
Write-Host "2. Execute a API: cd Habitus.API && dotnet run" -ForegroundColor Cyan
Write-Host "3. Abra o frontend em um servidor HTTP" -ForegroundColor Cyan
Write-Host "4. Teste o cadastro e login" -ForegroundColor Cyan
Write-Host ""

Write-Host "✨ SETUP CONCLUÍDO! Boa codificação! ✨" -ForegroundColor Green

# Voltar para diretório raiz
Set-Location ".."
