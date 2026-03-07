# Teste local do CLI — Video Processor

Guia rápido para rodar o CLI no modo **local** (vídeo em disco) e no modo **AWS/S3** (vídeo no S3 → frames no S3).

---

## Pré-requisitos

- **Modo local:** FFmpeg disponível (o CLI baixa via Xabe se necessário). Vídeo em disco.
- **Modo AWS:** Credenciais AWS configuradas (`aws configure` ou variáveis `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_REGION`). Nenhum `appsettings` no projeto — só ambiente.

### Validar AWS

```bash
aws sts get-caller-identity
aws s3 ls
```

Se ambos funcionarem, o CLI em modo AWS usa as mesmas credenciais.

---

## Modo local (vídeo em disco → pasta local)

Comportamento igual ao anterior: não usa S3.

```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --video "C:\Projetos\Fiap\Videos\entrada\teste1.mp4" --interval 2 --output "C:\Projetos\Fiap\Videos\saida" --start 0 --end 10
```

Ou com `--mode local` (opcional):

```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --mode local --video "C:\Projetos\Fiap\Videos\entrada\teste1.mp4" --interval 2 --output "C:\Projetos\Fiap\Videos\saida" --start 0 --end 10
```

---

## Modo AWS (vídeo no S3 → frames no S3)

Baixa o vídeo do bucket de origem, extrai frames e envia para o bucket de destino. Usa o mesmo `ProcessChunkUseCase` da Lambda.

### Argumentos obrigatórios

| Argumento           | Descrição                                      |
|---------------------|------------------------------------------------|
| `--mode`            | `aws`                                          |
| `--video-id`        | Identificador do vídeo (ex.: UUID)             |
| `--chunk-id`        | Identificador do chunk (ex.: `chunk-001`)       |
| `--interval`        | Intervalo em segundos entre frames (ex.: 2)    |
| `--start`           | Início do trecho em segundos (ex.: 0)           |
| `--end`             | Fim do trecho em segundos (ex.: 10)             |
| `--source-bucket`   | Bucket S3 onde está o vídeo                     |
| `--source-key`      | **Key exata** do objeto no S3 (caminho completo)|
| `--target-bucket`   | Bucket S3 para gravar os frames                 |
| `--target-prefix`   | Prefixo/pasta no bucket de destino              |

### Importante: key do vídeo no S3

A `--source-key` deve ser **exatamente** a key do objeto no S3. No S3 não existem pastas de verdade; a key é o “caminho” completo do arquivo.

Para descobrir a key correta:

```bash
aws s3 ls s3://SEU-BUCKET-ORIGEM/caminho/do/video/ --recursive
```

Use a key que aparecer (ex.: o objeto pode se chamar `original` sem extensão `.mp4`).

### Exemplo — ambiente dev (video-processing-engine)

Bucket de vídeos: `video-processing-engine-dev-videos`  
Bucket de imagens: `video-processing-engine-dev-images`  
Objeto no S3: key `videos/34e88498-3031-7041-d464-584dc2c71918/860e4513-e3e7-40e2-8a03-2381c45f3530/original` (arquivo sem extensão no nome).

```bash
dotnet run --project src/InterfacesExternas/VideoProcessor.CLI -- --mode aws --video-id "860e4513-e3e7-40e2-8a03-2381c45f3530" --chunk-id "chunk-001" --interval 2 --start 0 --end 10 --source-bucket "video-processing-engine-dev-videos" --source-key "videos/34e88498-3031-7041-d464-584dc2c71918/860e4513-e3e7-40e2-8a03-2381c45f3530/original" --target-bucket "video-processing-engine-dev-images" --target-prefix "processed/860e4513-e3e7-40e2-8a03-2381c45f3530/chunk-001/frames/"
```

**Verificar frames no S3:**

```bash
aws s3 ls s3://video-processing-engine-dev-images/processed/860e4513-e3e7-40e2-8a03-2381c45f3530/chunk-001/frames/ --recursive
```

---

## Resumo

- **Só ambiente:** não é necessário `appsettings` no CLI; AWS usa credenciais do ambiente.
- **Modo local:** mesmo comando de sempre; opcional `--mode local`.
- **Modo AWS:** bucket e **key exata** do objeto; conferir com `aws s3 ls ... --recursive` quando der “vídeo não encontrado”.

Mais exemplos e payloads da Lambda: `docs/payload-examples.md`.
