# Script para configurar e rodar o Frontend Angular
# Execute este script no PowerShell

Write-Host "🎯 CONFIGURANDO FRONTEND ANGULAR - HABITUS" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

# Verificar se Node.js está instalado
$nodeVersion = node --version 2>$null
if ($nodeVersion) {
    Write-Host "✅ Node.js $nodeVersion encontrado" -ForegroundColor Green
} else {
    Write-Host "❌ Node.js não encontrado" -ForegroundColor Red
    Write-Host "📥 Por favor, instale o Node.js 18+ em: https://nodejs.org/" -ForegroundColor Yellow
    Write-Host "   - Baixe a versão LTS (recomendada)" -ForegroundColor Cyan
    Write-Host "   - Execute o instalador" -ForegroundColor Cyan
    Write-Host "   - Reinicie o PowerShell após a instalação" -ForegroundColor Cyan
    Write-Host ""
    Read-Host "Pressione Enter após instalar o Node.js..."
    exit 1
}

# Verificar se Angular CLI está instalado
$ngVersion = ng version 2>$null
if ($ngVersion) {
    Write-Host "✅ Angular CLI encontrado" -ForegroundColor Green
} else {
    Write-Host "📦 Instalando Angular CLI..." -ForegroundColor Yellow
    npm install -g @angular/cli
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Angular CLI instalado com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "❌ Erro ao instalar Angular CLI" -ForegroundColor Red
        exit 1
    }
}

# Navegar para o diretório do frontend
Write-Host ""
Write-Host "📁 Navegando para frontend-angular..." -ForegroundColor Yellow
if (Test-Path "frontend-angular") {
    Set-Location "frontend-angular"
} else {
    Write-Host "❌ Diretório frontend-angular não encontrado" -ForegroundColor Red
    exit 1
}

# Instalar dependências
Write-Host ""
Write-Host "📦 Instalando dependências npm..." -ForegroundColor Yellow
npm install
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao instalar dependências" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Dependências instaladas com sucesso!" -ForegroundColor Green

# Instruções para execução
Write-Host ""
Write-Host "🚀 COMO EXECUTAR O PROJETO" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green
Write-Host ""
Write-Host "1. Para desenvolvimento:" -ForegroundColor Cyan
Write-Host "   ng serve" -ForegroundColor White
Write-Host "   # ou" -ForegroundColor Gray
Write-Host "   npm start" -ForegroundColor White
Write-Host ""
Write-Host "2. Para abrir automaticamente:" -ForegroundColor Cyan
Write-Host "   ng serve --open" -ForegroundColor White
Write-Host ""
Write-Host "3. URLs importantes:" -ForegroundColor Cyan
Write-Host "   Frontend: http://localhost:4200" -ForegroundColor Green
Write-Host "   Backend:  https://localhost:7001" -ForegroundColor Green
Write-Host ""

# Perguntar se quer executar agora
$runNow = Read-Host "Deseja executar o projeto agora? (s/n)"
if ($runNow -eq "s" -or $runNow -eq "S" -or $runNow -eq "sim") {
    Write-Host ""
    Write-Host "🚀 Iniciando servidor de desenvolvimento..." -ForegroundColor Green
    Write-Host "   Pressione Ctrl+C para parar" -ForegroundColor Yellow
    Write-Host "   O navegador abrirá automaticamente" -ForegroundColor Cyan
    Write-Host ""
    
    Start-Sleep -Seconds 2
    ng serve --open
}

Write-Host ""
Write-Host "✨ SETUP ANGULAR CONCLUÍDO! ✨" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Próximos passos:" -ForegroundColor Yellow
Write-Host "1. Certifique-se que o backend está rodando" -ForegroundColor Cyan
Write-Host "2. Execute: ng serve --open" -ForegroundColor Cyan
Write-Host "3. Acesse: http://localhost:4200" -ForegroundColor Cyan
Write-Host "4. Teste o cadastro e login" -ForegroundColor Cyan
