# Subtask 03: Documentar Layer FFmpeg e variáveis da Lambda

## Descrição
A Lambda da Story 09 usa FFmpeg (Xabe.FFmpeg) e procura binários em `/opt/bin`, `/opt/ffmpeg` ou na variável de ambiente `FFMPEG_PATH`. A action de deploy não empacota o binário FFmpeg; a função na AWS deve ter um Layer com FFmpeg ou a variável configurada. Esta subtask garante que isso esteja documentado e, se aplicável, que a action ou a doc indiquem como anexar o layer.

## Passos de implementação
1. Documentar no README ou em `docs/deploy-lambda.md` (ou equivalente) que a função Lambda espera FFmpeg em um dos caminhos: `FFMPEG_PATH` (env), `/opt/bin` ou `/opt/ffmpeg`, tipicamente fornecidos por um **Lambda Layer** com FFmpeg para Amazon Linux 2.
2. Incluir na documentação: como anexar um layer à função (console AWS ou AWS CLI), ou referência a um layer público de FFmpeg compatível com .NET Lambda (se houver).
3. Documentar variáveis de ambiente úteis para a Lambda: `FFMPEG_PATH` (opcional, se o layer expõe o binário em outro path); e quaisquer variáveis já usadas pela aplicação (ex.: configuração de buckets via env, se existir).
4. Se for desejado que o **workflow** configure o layer automaticamente (ex.: via `aws lambda update-function-configuration --layers`), adicionar passo opcional no job de deploy usando variável do repositório com o ARN do layer; caso contrário, deixar explícito na doc que o layer deve ser configurado manualmente na função.

## Formas de teste
- Ler a documentação e seguir os passos para anexar um layer à função; invocar a Lambda e verificar que o processamento com FFmpeg funciona (ou que o erro é por falta de vídeo/S3, não por FFmpeg não encontrado).
- Verificar que a action não quebra se a função já tiver layer configurado (update-function-code não remove layers).

## Critérios de aceite da subtask
- [x] Existe documentação que explica onde a Lambda espera o FFmpeg (FFMPEG_PATH, /opt/bin, /opt/ffmpeg) e que o uso de Lambda Layer é a abordagem recomendada na AWS.
- [x] A documentação descreve como anexar o layer à função (manual ou referência a ARN/configuração).
- [x] Variáveis de ambiente relevantes para a Lambda (FFMPEG_PATH e outras usadas pelo código) estão listadas na documentação.
- [x] O workflow de deploy não remove nem sobrescreve a configuração de layers da função de forma indesejada (apenas update-function-code; layers permanecem se já configurados).
