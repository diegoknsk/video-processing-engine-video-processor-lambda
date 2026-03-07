# Storie-11: Corrigir FFmpeg no Lambda — Sistema de Arquivos Somente Leitura

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do Video Processing Engine, quero que o Lambda use um diretório gravável para configuração ou download do FFmpeg na AWS, para que o processamento de vídeo funcione quando a função é publicada, sem falhar com "Read-only file system: '/var/task/.ffmpeg'".

## Objetivo
Garantir que, em ambiente AWS Lambda, o código não tente criar ou escrever em `/var/task` (somente leitura). O FFmpeg deve ser encontrado via Lambda Layer (`/opt/ffmpeg` ou `/opt/bin`) quando existir; caso o setup precise baixar ou usar um diretório próprio, esse diretório deve ser `/tmp` (único gravável no Lambda).

## Contexto
- **Problema:** Localmente a task 9 funciona; na AWS o handler falha com `IOException: Read-only file system : '/var/task/.ffmpeg'`.
- **Causa:** `FFmpegSetup.EnsureFFmpegInstalledAsync()` usa `Environment.SpecialFolder.UserProfile`, que no Lambda resolve para `/var/task` (read-only).
- **Solução:** Em ambiente Lambda, usar `/tmp` como base para o diretório `.ffmpeg`; manter prioridade para Layer em `/opt/ffmpeg` e `/opt/bin` no `Function.cs`.

## Escopo Técnico
- **Tecnologias:** .NET 10, AWS Lambda, Xabe.FFmpeg 5.2.6, Xabe.FFmpeg.Downloader 5.2.6
- **Arquivos afetados:**
  - `src/Core/VideoProcessor.Application/Services/FFmpegSetup.cs`
  - `src/InterfacesExternas/VideoProcessor.Lambda/Function.cs` (se necessário reforçar ordem de configuração)
- **Componentes:** FFmpegSetup (base path condicional), Function (detecção Layer vs fallback)
- **Pacotes/Dependências:** já existentes (Xabe.FFmpeg, Xabe.FFmpeg.Downloader); nenhum pacote novo

## Dependências e Riscos (para estimativa)
- **Dependências:** Storie-09 e Storie-10 (Lambda com processamento real e deploy já em uso).
- **Riscos:** Em Lambda sem Layer, download para `/tmp` pode consumir tempo e espaço (512 MB limite); recomendação continua sendo usar Layer com FFmpeg em `/opt/ffmpeg` ou `/opt/bin`.
- **Pré-condições:** Nenhuma; a correção deve manter comportamento local (UserProfile) inalterado.

## Subtasks
- [ ] [Subtask 01: Usar diretório gravável em Lambda no FFmpegSetup](./subtask/Subtask-01-FFmpegSetup_Diretorio_Gravar_Lambda.md)
- [ ] [Subtask 02: Garantir prioridade Layer e fallback no Function](./subtask/Subtask-02-Function_Prioridade_Layer_Fallback.md)
- [ ] [Subtask 03: Validar local e na AWS e atualizar documentação](./subtask/Subtask-03-Validar_Docs.md)

## Critérios de Aceite da História
- [ ] Em ambiente AWS Lambda, não ocorre `IOException` por "Read-only file system" ao invocar o handler (com ou sem Layer).
- [ ] Com Lambda Layer com FFmpeg em `/opt/ffmpeg` ou `/opt/bin`, o path é detectado no cold start e `EnsureFFmpegInstalledAsync` não tenta criar diretório em `/var/task`.
- [ ] Sem Layer, se o código precisar baixar FFmpeg, o diretório usado é `/tmp` (ou outro gravável), nunca `/var/task`.
- [ ] Comportamento local (Windows) permanece inalterado: uso de `%USERPROFILE%\.ffmpeg` quando aplicável.
- [ ] Documentação (README ou docs de deploy) menciona que em Lambda o único diretório gravável é `/tmp` e que o uso de Layer é recomendado.

## Rastreamento (dev tracking)
- **Início:** 07/03/2026, às 17:56 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
