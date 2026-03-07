# Subtask 05: Ajustar e validar workflows de CI

## Descrição
Atualizar os workflows de CI (ex.: GitHub Actions) que utilizam paths dos projetos — em especial o path do projeto Lambda para publish/package — e validar que o pipeline passa.

## Passos de implementação
1. Localizar todos os arquivos de workflow em `.github/workflows/` (ex.: `deploy-lambda.yml`).
2. Em cada workflow, buscar referências a paths de projetos, por exemplo:
   - `-pl src/VideoProcessor.Lambda` → alterar para `-pl src/InterfacesExternas/VideoProcessor.Lambda`
   - Qualquer `dotnet build`, `dotnet test`, `dotnet publish` ou `dotnet lambda package` que receba path explícito de projeto ou pasta.
3. Atualizar os paths para a nova estrutura: `src/Core/*`, `src/Infra/*`, `src/InterfacesExternas/*`, `tests/*`.
4. Se houver jobs que listem projetos por glob (ex.: `**/*.csproj`), verificar se o resultado ainda inclui todos os projetos desejados.
5. Fazer commit das alterações e validar o workflow (push para branch ou `workflow_dispatch` se disponível), garantindo que build e test passam e que o package do Lambda é gerado no path esperado.

## Formas de teste
- Executar o workflow no GitHub (push ou manual dispatch) e conferir que o job de build-and-test conclui com sucesso.
- Conferir que o job de deploy (ou package) usa o projeto Lambda no path `src/InterfacesExternas/VideoProcessor.Lambda` e que o artefato é produzido corretamente.
- Revisar os logs do workflow para garantir que não há erro de "project not found" ou path inválido.

## Critérios de aceite
- [x] Todos os workflows em `.github/workflows/` que referenciam paths de projetos foram atualizados para a nova estrutura.
- [x] O workflow de build/test (e deploy, se aplicável) executa com sucesso após as alterações.
- [x] O comando de package da Lambda (ex.: `dotnet lambda package ... -pl ...`) usa o path `src/InterfacesExternas/VideoProcessor.Lambda` (ou equivalente).
