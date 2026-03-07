# Subtask 01: Criar port IS3VideoStorage no Domain

## Descrição
Definir a interface (port) `IS3VideoStorage` na camada Domain, em `VideoProcessor.Domain/Ports/`, representando o contrato para as duas operações S3 necessárias ao processamento de chunk: download do vídeo de origem para um arquivo temporário local, e upload de frames para o bucket/prefixo de destino. O port deve ser agnóstico a AWS — sem nenhuma referência ao AWSSDK.

## Passos de implementação
1. Criar a pasta `src/Core/VideoProcessor.Domain/Ports/` se não existir.
2. Criar `src/Core/VideoProcessor.Domain/Ports/IS3VideoStorage.cs` com dois métodos assíncronos:
   - `Task<string> DownloadToTempAsync(string bucket, string key, string localTempPath, CancellationToken ct = default)` — baixa o objeto do bucket/key para o caminho local informado e retorna o caminho local onde foi gravado.
   - `Task<IReadOnlyList<string>> UploadFramesAsync(string bucket, string prefix, IEnumerable<string> localFramePaths, CancellationToken ct = default)` — faz upload de todos os frames locais para o bucket no prefixo informado e retorna as S3 keys dos objetos criados.
3. Adicionar documentação XML em cada membro: descrever parâmetros, retorno e comportamento esperado em caso de arquivo não encontrado no S3 (deve lançar `FileNotFoundException` ou exceção de aplicação descritiva).
4. Verificar que o arquivo gerado não importa nenhum namespace `Amazon.*` — o port deve ser 100% agnóstico de infraestrutura.

## Formas de teste
- Compilar `VideoProcessor.Domain` isoladamente (`dotnet build src/Core/VideoProcessor.Domain`) e confirmar zero erros e zero dependências externas de infraestrutura.
- Revisão de código: confirmar que o port está em `Domain/Ports/`, que os tipos são todos do BCL (string, IReadOnlyList, CancellationToken) e que não há `using Amazon.*`.
- Criar mock/stub manual da interface nos testes da Subtask 06 e confirmar que o mock compila corretamente sem precisar de AWSSDK no projeto de testes.

## Critérios de aceite da subtask
- [ ] Arquivo `src/Core/VideoProcessor.Domain/Ports/IS3VideoStorage.cs` criado no namespace `VideoProcessor.Domain.Ports`.
- [ ] Interface expõe exatamente dois métodos: `DownloadToTempAsync` e `UploadFramesAsync`, com assinaturas compatíveis com o uso em `ProcessChunkUseCase`.
- [ ] Nenhuma referência a `Amazon.*` ou qualquer biblioteca de infra no arquivo.
- [ ] Documentação XML presente em cada método descrevendo parâmetros e comportamento de erro.
- [ ] `dotnet build src/Core/VideoProcessor.Domain` conclui sem erros.
