# Subtask 02: Implementar Function Handler Puro e Bootstrap de DI

## Descrição
Criar handler Lambda (`Function.cs`) com método `FunctionHandler` puro (sem AddAWSLambdaHosting), configurar DI no construtor usando Microsoft.Extensions.DependencyInjection, e implementar retorno mockado para smoke test.

## Passos de Implementação
1. No projeto `VideoProcessor.Lambda`, instalar pacotes:
   ```bash
   dotnet add package Amazon.Lambda.Core --version 2.3.0
   dotnet add package Amazon.Lambda.Serialization.SystemTextJson --version 2.4.3
   dotnet add package Microsoft.Extensions.DependencyInjection --version 10.0.0
   dotnet add package Microsoft.Extensions.Configuration --version 10.0.0
   dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables --version 10.0.0
   ```
2. Criar `Function.cs`:
   - Atributo `[assembly: LambdaSerializer(...)]` para System.Text.Json
   - Classe `Function` com campo `IServiceProvider`
   - Construtor padrão (sem parâmetros) que chama `ConfigureServices()` e constrói ServiceProvider
   - Método privado `ConfigureServices()` registra serviços (Application, Infra)
   - Método público `FunctionHandler(JsonDocument input, ILambdaContext context)` retorna `Task<JsonDocument>` mockado
3. No método `FunctionHandler`:
   - Logar input recebido (usando `context.Logger`)
   - Retornar JSON mockado: `{ "status": "SUCCEEDED", "message": "Mock response from bootstrap" }`
4. Criar método `ConfigureServices()` que registra:
   - Serviços de Application (preparar para próximas stories)
   - Serviços de Infra (preparar para próximas stories)
   - Usar `IServiceCollection` e retornar `ServiceProvider`

## Formas de Teste
1. **Compilação:** `dotnet build src/VideoProcessor.Lambda` deve compilar sem erros
2. **Teste local:** criar console app que instancia `Function` e chama `FunctionHandler` com JSON mockado
3. **Smoke test:** handler retorna JSON válido com estrutura esperada

## Critérios de Aceite da Subtask
- [x] `Function.cs` implementado com construtor padrão e DI configurado
- [x] Atributo `[assembly: LambdaSerializer]` configurado para System.Text.Json
- [x] Método `FunctionHandler` recebe `JsonDocument` e retorna `Task<JsonDocument>`
- [x] Resposta mockada retorna JSON válido com campo `status: "SUCCEEDED"`
- [x] `ConfigureServices()` prepara ServiceProvider (pode estar vazio neste momento)
- [x] Projeto compila sem warnings relacionados a Lambda runtime
