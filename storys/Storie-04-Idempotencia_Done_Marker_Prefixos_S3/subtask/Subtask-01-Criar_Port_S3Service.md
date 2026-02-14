# Subtask 01: Criar Port IS3Service e Implementação S3Service

## Descrição
Criar interface `IS3Service` no Domain (port) com métodos para verificar existência, ler e escrever JSONs no S3, e implementar `S3Service` na Infra usando AWSSDK.S3 com tratamento de erros.

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Ports/IS3Service.cs`:
   ```csharp
   public interface IS3Service
   {
       Task<bool> ExistsAsync(string bucket, string key, CancellationToken ct = default);
       Task<T?> GetJsonAsync<T>(string bucket, string key, CancellationToken ct = default);
       Task PutJsonAsync<T>(string bucket, string key, T obj, CancellationToken ct = default);
   }
   ```
2. Criar `src/VideoProcessor.Infra/Services/S3Service.cs`:
   - Injetar `IAmazonS3` via construtor primário
   - Implementar `ExistsAsync`:
     ```csharp
     try
     {
         await _s3Client.GetObjectMetadataAsync(bucket, key, ct);
         return true;
     }
     catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
     {
         return false;
     }
     ```
   - Implementar `GetJsonAsync<T>`:
     ```csharp
     var response = await _s3Client.GetObjectAsync(bucket, key, ct);
     using var stream = response.ResponseStream;
     return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: ct);
     ```
   - Implementar `PutJsonAsync<T>`:
     ```csharp
     var json = JsonSerializer.Serialize(obj);
     var request = new PutObjectRequest
     {
         BucketName = bucket,
         Key = key,
         ContentBody = json,
         ContentType = "application/json"
     };
     await _s3Client.PutObjectAsync(request, ct);
     ```
3. Registrar no DI (`Function.cs` → `ConfigureServices`):
   ```csharp
   services.AddSingleton<IAmazonS3>(new AmazonS3Client());
   services.AddSingleton<IS3Service, S3Service>();
   ```
4. Adicionar using: `using Amazon.S3;`, `using Amazon.S3.Model;`

## Formas de Teste
1. **Mock S3:** criar testes unitários com mock de `IAmazonS3` (usando Moq)
2. **Teste de existência:** mock retorna NotFound → `ExistsAsync` retorna false
3. **Teste de escrita:** verificar que `PutObjectAsync` foi chamado com bucket/key corretos

## Critérios de Aceite da Subtask
- [ ] Interface `IS3Service` criada no Domain com 3 métodos
- [ ] Implementação `S3Service` criada na Infra
- [ ] `ExistsAsync` trata `AmazonS3Exception` com StatusCode NotFound corretamente
- [ ] `GetJsonAsync` deserializa JSON do stream
- [ ] `PutJsonAsync` serializa objeto e sobe para S3 com ContentType "application/json"
- [ ] `IAmazonS3` e `IS3Service` registrados no DI
- [ ] Projeto compila sem erros
