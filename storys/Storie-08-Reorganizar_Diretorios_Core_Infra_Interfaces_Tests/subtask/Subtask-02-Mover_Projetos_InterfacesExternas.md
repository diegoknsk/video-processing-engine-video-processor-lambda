# Subtask 02: Mover projetos InterfacesExternas

## Descrição
Criar a pasta `src/InterfacesExternas/` e mover os projetos de entrada (CLI e Lambda) para dentro dela, mantendo nomes dos projetos inalterados.

## Passos de implementação
1. Criar pasta `src/InterfacesExternas/`.
2. Mover `src/VideoProcessor.CLI` para `src/InterfacesExternas/VideoProcessor.CLI`.
3. Mover `src/VideoProcessor.Lambda` para `src/InterfacesExternas/VideoProcessor.Lambda`.
4. Verificar que em `src/` restam apenas as pastas `Core`, `Infra` e `InterfacesExternas`.

## Formas de teste
- Listar diretórios: `src/InterfacesExternas` deve conter `VideoProcessor.CLI` e `VideoProcessor.Lambda`.
- A solution ainda referencia paths antigos; build pode falhar até a Subtask 03.
- `git status` deve mostrar apenas movimentação (moves) para CLI e Lambda.

## Critérios de aceite
- [x] Existe `src/InterfacesExternas/VideoProcessor.CLI` e `src/InterfacesExternas/VideoProcessor.Lambda` com todos os arquivos.
- [x] Os projetos não existem mais em `src/VideoProcessor.CLI` e `src/VideoProcessor.Lambda`.
- [x] Estrutura física de `src/` é apenas: `src/Core/`, `src/Infra/`, `src/InterfacesExternas/`.
