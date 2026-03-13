# Subtask-01: Adicionar coverlet.msbuild aos Projetos de Teste

## Descrição
Adicionar o pacote `coverlet.msbuild` (v6.0.4) a ambos os projetos de teste (`VideoProcessor.Tests.Unit` e `VideoProcessor.Tests.Bdd`), com as propriedades corretas de `IncludeAssets` e `PrivateAssets`, para permitir a coleta de cobertura no formato OpenCover via flags MSBuild durante o `dotnet test`.

## Passos de Implementação
1. Abrir `tests/VideoProcessor.Tests.Unit/VideoProcessor.Tests.Unit.csproj` e adicionar dentro do `<ItemGroup>` existente de `PackageReference`:
   ```xml
   <PackageReference Include="coverlet.msbuild" Version="6.0.4">
     <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
     <PrivateAssets>all</PrivateAssets>
   </PackageReference>
   ```
   Verificar também se o `coverlet.collector` existente está com `PrivateAssets=all` e `IncludeAssets` corretos — ajustar se necessário.

2. Abrir `tests/VideoProcessor.Tests.Bdd/VideoProcessor.Tests.Bdd.csproj` e repetir a adição do `coverlet.msbuild` com os mesmos atributos. Verificar se já existe `coverlet.collector` e, se não, adicioná-lo também.

3. Rodar `dotnet restore` localmente para confirmar que os pacotes são resolvidos sem erros:
   ```bash
   dotnet restore tests/VideoProcessor.Tests.Unit/VideoProcessor.Tests.Unit.csproj
   dotnet restore tests/VideoProcessor.Tests.Bdd/VideoProcessor.Tests.Bdd.csproj
   ```

4. Validar coleta de cobertura localmente executando:
   ```bash
   dotnet test tests/VideoProcessor.Tests.Unit \
     --configuration Release \
     /p:CollectCoverage=true \
     /p:CoverletOutputFormat=opencover \
     /p:CoverletOutput=./TestResults/coverage.opencover.xml
   ```
   Confirmar que o arquivo `tests/VideoProcessor.Tests.Unit/TestResults/coverage.opencover.xml` é gerado.

## Formas de Teste
1. **Verificação do arquivo:** após `dotnet test` com as flags, confirmar existência de `coverage.opencover.xml` em `tests/VideoProcessor.Tests.Unit/TestResults/`.
2. **Conteúdo do XML:** abrir o arquivo gerado e verificar que contém elementos `<Module>` com `<Method>` e `sequenceCoverage` > 0 para as classes do projeto principal.
3. **Build limpo:** executar `dotnet build --no-incremental` para garantir que a adição do pacote não introduz erros de compilação.

## Critérios de Aceite
- [x] `coverlet.msbuild` 6.0.4 presente em `VideoProcessor.Tests.Unit.csproj` com `PrivateAssets=all`
- [x] `coverlet.msbuild` 6.0.4 presente em `VideoProcessor.Tests.Bdd.csproj` com `PrivateAssets=all`
- [x] `dotnet restore` conclui sem erros em ambos os projetos
- [x] Arquivo `coverage.opencover.xml` é gerado em `TestResults/` após `dotnet test` com as flags MSBuild
