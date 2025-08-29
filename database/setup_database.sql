-- Script para configurar o banco de dados Habitus
-- Execute este script no MySQL Workbench ou via comando mysql

-- 1. Criar o banco de dados
DROP DATABASE IF EXISTS habitus_db;
CREATE DATABASE habitus_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 2. Criar usuário específico para a aplicação
DROP USER IF EXISTS 'habitus_user'@'localhost';
CREATE USER 'habitus_user'@'localhost' IDENTIFIED BY 'HabitusPass123!';

-- 3. Dar permissões completas ao usuário
GRANT ALL PRIVILEGES ON habitus_db.* TO 'habitus_user'@'localhost';
FLUSH PRIVILEGES;

-- 4. Usar o banco criado
USE habitus_db;

-- 5. Verificar se está tudo configurado
SELECT 'Banco de dados habitus_db criado com sucesso!' as Status;
SHOW TABLES;
