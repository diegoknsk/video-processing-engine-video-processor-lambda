# Subtask 01: Criar Lambda Layer com FFmpeg binário

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [ ] Concluído

## Descrição
Baixar binários FFmpeg compilados para Amazon Linux 2 (arquitetura x86_64 ou arm64 conforme Lambda), criar estrutura de diretórios do Layer, empacotar como ZIP, e publicar na AWS.

## Tarefas
1. Baixar FFmpeg para Lambda:
   - Opção 1: Usar build pré-compilado de https://johnvansickle.com/ffmpeg/ (Linux static build)
   - Opção 2: Usar Layer público existente (ex: arn:aws:lambda:us-east-1:...:layer:ffmpeg)
2. Criar estrutura de diretórios local:
   ```
   lambda-layers/
   └── ffmpeg/
       └── ffmpeg/
           ├── ffmpeg
           └── ffprobe
   ```
3. Validar que binários têm permissão de execução: `chmod +x ffmpeg ffprobe`
4. Criar ZIP:
   ```bash
   cd lambda-layers/ffmpeg
   zip -r ffmpeg-layer.zip .
   ```
5. Publicar Layer na AWS:
   ```bash
   aws lambda publish-layer-version \
     --layer-name ffmpeg \
     --zip-file fileb://ffmpeg-layer.zip \
     --compatible-runtimes dotnet8 \
     --compatible-architectures x86_64
   ```
6. Anexar Layer à função Lambda (Console ou CLI)

## Critérios de Aceite
- [ ] Binários FFmpeg baixados para Amazon Linux
- [ ] Estrutura de diretórios criada: `ffmpeg/ffmpeg/` contendo `ffmpeg` e `ffprobe`
- [ ] ZIP criado com estrutura correta
- [ ] Layer publicado na AWS com nome `ffmpeg`
- [ ] Layer anexado à função Lambda `video-processor-lambda`
- [ ] ARN do Layer documentado em `docs/LAMBDA_LAYER_FFMPEG.md`

## Notas Técnicas
- Layer extrai conteúdo para `/opt/` no Lambda
- Estrutura `ffmpeg/ffmpeg/` resulta em `/opt/ffmpeg/ffmpeg` e `/opt/ffmpeg/ffprobe`
- Usar Amazon Linux 2 (não Ubuntu) para compatibilidade
- Tamanho do Layer: ~80-100MB (FFmpeg completo)
- Alternativa: usar Layer público já existente (mais rápido)
