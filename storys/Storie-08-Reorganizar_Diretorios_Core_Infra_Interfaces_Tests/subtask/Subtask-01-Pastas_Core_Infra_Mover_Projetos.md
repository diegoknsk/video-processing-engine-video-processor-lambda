# Subtask 01: Criar pastas físicas e mover projetos Core e Infra

## Descrição
Criar as pastas `src/Core/` e `src/Infra/` e mover os projetos de núcleo (Domain, Application) para Core e o projeto de infraestrutura (Infra) para Infra, sem alterar conteúdo dos arquivos — apenas localização.

## Passos de implementação
1. Criar pasta `src/Core/`.
2. Mover `src/VideoProcessor.Domain` para `src/Core/VideoProcessor.Domain` (arrastar pasta ou `git mv`).
3. Mover `src/VideoProcessor.Application` para `src/Core/VideoProcessor.Application`.
4. Criar pasta `src/Infra/`.
5. Mover `src/VideoProcessor.Infra` para `src/Infra/VideoProcessor.Infra`.
6. Verificar que não restaram pastas vazias em `src/` além das novas (Core, Infra); InterfacesExternas será criada na Subtask 02.

## Formas de teste
- Listar diretórios: `src/Core` deve conter `VideoProcessor.Domain` e `VideoProcessor.Application`; `src/Infra` deve conter `VideoProcessor.Infra`.
- Os `.csproj` ainda apontam para os paths antigos na solution — o build pode falhar até a Subtask 03; não é objetivo desta subtask que o build passe.
- Confirmar com `git status` que apenas movimentação de arquivos foi registrada (renames/moves).

## Critérios de aceite
- [x] Existe `src/Core/VideoProcessor.Domain` e `src/Core/VideoProcessor.Application` com todos os arquivos do projeto.
- [x] Existe `src/Infra/VideoProcessor.Infra` com todos os arquivos do projeto.
- [x] Os projetos originais não existem mais em `src/VideoProcessor.Domain`, `src/VideoProcessor.Application`, `src/VideoProcessor.Infra`.
