# Video Processor Lambda

Lambda Worker para processar chunks individuais de vídeo no pipeline da Step Functions. Projeto minimalista com handler puro em .NET 10 (sem AddAWSLambdaHosting).

## Pré-requisitos

- .NET 10 SDK
- AWS CLI (para testes locais com S3)

## Como Buildar

```bash
dotnet build
```

## Como Rodar Testes

```bash
dotnet test
```

## Como Empacotar

Com AWS Lambda Tools instalado (`dotnet tool install -g Amazon.Lambda.Tools`):

```bash
cd src/VideoProcessor.Lambda
dotnet lambda package -o ../../artifacts/VideoProcessor.zip
```

Alternativa (sem Lambda Tools):

```bash
dotnet publish src/VideoProcessor.Lambda -c Release -o publish
cd publish && zip -r ../artifacts/VideoProcessor.zip .
```

## Estrutura de Pastas

| Projeto | Descrição |
|---------|-----------|
| `src/VideoProcessor.Domain` | Entidades e ports (camada de domínio) |
| `src/VideoProcessor.Application` | Use cases e regras de negócio |
| `src/VideoProcessor.Infra` | Implementações (S3, etc.) |
| `src/VideoProcessor.Lambda` | Handler Lambda e bootstrap de DI |
| `tests/VideoProcessor.Tests.Unit` | Testes unitários (xUnit) |
| `tests/VideoProcessor.Tests.Bdd` | Testes BDD (SpecFlow + xUnit) |
