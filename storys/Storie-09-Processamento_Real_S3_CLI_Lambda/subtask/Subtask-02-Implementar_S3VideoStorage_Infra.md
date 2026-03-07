# Subtask 02: Implementar S3VideoStorage no Infra

## Descrição
Implementar `S3VideoStorage` no projeto `VideoProcessor.Infra`, satisfazendo o port `IS3VideoStorage`. Esta é a única classe do projeto que terá dependência direta de `AWSSDK.S3`. A implementação deve suportar: download de objetos S3 para arquivo temporário local (usado pela Lambda e CLI modo AWS), e upload em paralelo controlado de múltiplos frames para S3.

## Passos de implementação
1. Adicionar o pacote `AWSSDK.S3` ao `VideoProcessor.Infra.csproj`:
   ```xml
   <PackageReference Include="AWSSDK.S3" Version="3.7.*" />
   ```
   Usar o comando `dotnet add src/Infra/VideoProcessor.Infra package AWSSDK.S3` para obter a versão mais recente estável.

2. Criar a pasta `src/Infra/VideoProcessor.Infra/S3/` e criar o arquivo `S3VideoStorage.cs` no namespace `VideoProcessor.Infra.S3`.

3. Implementar `DownloadToTempAsync`:
   - Receber `bucket`, `key`, `localTempPath` e `CancellationToken`.
   - Garantir que o diretório pai de `localTempPath` existe (`Directory.CreateDirectory`).
   - Usar `AmazonS3Client.GetObjectAsync` para obter o stream do objeto.
   - Gravar o stream no arquivo local via `FileStream` com `CopyToAsync`.
   - Em caso de `AmazonS3Exception` com `StatusCode == 404`, relançar como `FileNotFoundException` com mensagem descritiva (ex.: `"Vídeo não encontrado no S3: s3://{bucket}/{key}"`).

4. Implementar `UploadFramesAsync`:
   - Receber `bucket`, `prefix`, `IEnumerable<string> localFramePaths` e `CancellationToken`.
   - Garantir que o prefix termina em `/` (normalizar se necessário).
   - Fazer upload em paralelo com `SemaphoreSlim(8)` para limitar concorrência e evitar throttling S3.
   - Para cada frame: derivar a S3 key como `$"{prefix}{Path.GetFileName(localPath)}"`.
   - Usar `PutObjectRequest` com `FilePath` (upload de arquivo local).
   - Retornar `IReadOnlyList<string>` com todas as S3 keys geradas, na mesma ordem dos frames.

5. Registrar `S3VideoStorage` como implementação de `IS3VideoStorage` via extensão de DI, ou documentar o registro para ser feito no bootstrap da Lambda e do CLI.

## Notas técnicas
- `AmazonS3Client` deve ser injetado via construtor (DI) para permitir mock em testes. Não usar `new AmazonS3Client()` diretamente na classe.
- Para testes unitários (Subtask 06), o `IAmazonS3` (interface do AWSSDK) pode ser mockado com Moq.
- O upload paralelo com `SemaphoreSlim` evita `503 SlowDown` do S3 em lotes grandes de frames.
- `ContentType` dos frames deve ser `"image/jpeg"`.

## Formas de teste
- Teste unitário (Subtask 06): mockar `IAmazonS3`, chamar `DownloadToTempAsync` e verificar que `GetObjectAsync` foi chamado com bucket/key corretos; verificar que `FileNotFoundException` é lançado em resposta a 404.
- Teste unitário (Subtask 06): mockar `IAmazonS3`, chamar `UploadFramesAsync` com lista de 3 frames e verificar que `PutObjectAsync` foi chamado 3 vezes com as keys corretas.
- Teste manual (Subtask 07): executar CLI modo AWS com bucket e key reais e confirmar que o vídeo é baixado e os frames aparecem no bucket de destino via Console AWS.

## Critérios de aceite da subtask
- [ ] Classe `S3VideoStorage` em `src/Infra/VideoProcessor.Infra/S3/S3VideoStorage.cs` implementa `IS3VideoStorage`.
- [ ] `AWSSDK.S3` adicionado ao `VideoProcessor.Infra.csproj` com versão registrada.
- [ ] `DownloadToTempAsync` usa `AmazonS3Client.GetObjectAsync`, grava o arquivo localmente e lança `FileNotFoundException` quando o objeto não existe no S3 (404).
- [ ] `UploadFramesAsync` usa upload paralelo com `SemaphoreSlim` e retorna todas as S3 keys geradas.
- [ ] `IAmazonS3` é recebido via construtor (injetável/mockável).
- [ ] `dotnet build` da solution completa passa sem erros.
