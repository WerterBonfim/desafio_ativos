# Werter FinTracker


## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- Versão 8 do dotnet
- Docker


# Execução

## Depurar/Debugar via IDE

Se você quiser depurar/debbugar o projeto via VisualStudio, Rider ou VSCode. Você precisa levar o banco de dados que esta registrado no arquivo docker-compose-dev.yml

Esse arquivo tem somente o serviço de banco de dados Sql Server Express

No terminal:
```bash

cd FinTracker/Infra

docker compose -p fin_trackr_dev -f docker-compose-dev.yml up -d

```

