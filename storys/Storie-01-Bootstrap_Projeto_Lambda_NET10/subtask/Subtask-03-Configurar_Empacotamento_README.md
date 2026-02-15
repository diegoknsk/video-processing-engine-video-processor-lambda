# Subtask 03: Configurar Empacotamento ZIP e Criar README

## Descrição
Configurar empacotamento do Lambda em formato ZIP (via `dotnet lambda package` ou publish + zip), criar `.gitignore` para .NET, e documentar no README.md como buildar, testar e empacotar o projeto.

## Passos de Implementação
1. Instalar AWS Lambda Tools (se necessário): `dotnet tool install -g Amazon.Lambda.Tools`
2. Adicionar ao `.csproj` do Lambda propriedades para empacotamento:
   ```xml
   <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
   <AWSProjectType>Lambda</AWSProjectType>
   ```
3. Testar empacotamento:
   ```bash
   cd src/VideoProcessor.Lambda
   dotnet lambda package -o ../../artifacts/VideoProcessor.zip
   ```
   Alternativa (se lambda tools não disponível):
   ```bash
   dotnet publish -c Release -o publish
   cd publish && zip -r ../../artifacts/VideoProcessor.zip .
   ```
4. Criar `.gitignore` na raiz com padrões .NET:
   - `bin/`, `obj/`, `*.user`, `.vs/`, `.idea/`, `artifacts/`, `publish/`, etc.
5. Criar `README.md` com seções:
   - **Visão Geral:** Lambda Worker para processar chunks de vídeo (minimalista, handler puro)
   - **Pré-requisitos:** .NET 10 SDK, AWS CLI
   - **Como Buildar:** `dotnet build`
   - **Como Rodar Testes:** `dotnet test`
   - **Como Empacotar:** comando `dotnet lambda package` ou alternativa com `dotnet publish + zip`
   - **Estrutura de Pastas:** breve descrição de cada projeto (Domain, Application, Infra, Lambda, Tests)

## Formas de Teste
1. **Empacotamento:** comando gera `VideoProcessor.zip` em `artifacts/` sem erros
2. **Validação do ZIP:** descompactar e verificar presença de `.dll`, `.deps.json`, `.runtimeconfig.json`
3. **Teste de .gitignore:** criar arquivo em `bin/` ou `obj/`, verificar que `git status` não o lista

## Critérios de Aceite da Subtask
- [x] Comando `dotnet lambda package` (ou alternativa) gera ZIP funcional
- [x] ZIP contém `VideoProcessor.Lambda.dll` e todas as dependências
- [x] `.gitignore` configurado com padrões .NET (bin, obj, artifacts, etc.)
- [x] README.md documenta: build, testes, empacotamento, estrutura de pastas
- [x] Arquivo ZIP gerado tem tamanho < 50 MB (sanity check; sem dependências desnecessárias)
