# Subtask 01: Criar port IS3Service e implementação S3Service

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Criar interface (port) `IS3Service` no Domain definindo contrato para operações S3, e criar implementação `S3Service` em novo projeto Infra.S3 usando AWSSDK.S3.

## Tarefas
1. Criar `src/VideoProcessor.Domain/Ports/IS3Service.cs`:
   ```csharp
   public interface IS3Service
   {
       Task<string> UploadFileAsync(string bucket, string key, string filePath);
       Task<string> UploadJsonAsync<T>(string bucket, string key, T obj);
   }
   ```
2. Criar novo projeto Infra.S3:
   ```bash
   dotnet new classlib -n VideoProcessor.Infra.S3 -o src/VideoProcessor.Infra.S3
   dotnet sln add src/VideoProcessor.Infra.S3
   ```
3. Adicionar referências:
   - `VideoProcessor.Domain`
   - AWSSDK.S3 (já instalado no Lambda, adicionar aqui)
4. Criar `src/VideoProcessor.Infra.S3/Services/S3Service.cs` implementando `IS3Service`
5. Implementar `UploadFileAsync()` usando `PutObjectAsync()`
6. Implementar `UploadJsonAsync()` serializando objeto e fazendo upload

## Critérios de Aceite
- [ ] Interface `IS3Service` criada em Domain/Ports
- [ ] Projeto `VideoProcessor.Infra.S3` criado e adicionado à solution
- [ ] Classe `S3Service` implementa `IS3Service`
- [ ] `UploadFileAsync()` faz upload de arquivo local para S3
- [ ] `UploadJsonAsync<T>()` serializa objeto e faz upload como JSON
- [ ] Ambos os métodos retornam S3 key do objeto uploaded
- [ ] Código compila sem erros

## Notas Técnicas
- Usar `AmazonS3Client` injetado via construtor
- `PutObjectRequest` com `FilePath` para arquivos
- `PutObjectRequest` com `ContentBody` para JSON string
- Serializar JSON com `JsonSerializer.Serialize()` (System.Text.Json)
