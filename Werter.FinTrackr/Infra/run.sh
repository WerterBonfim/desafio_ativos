#!/bin/bash


echo "Levantando ambiente..."

docker compose down
docker compose -p fin_trackr_dev -f docker-compose-dev.yml up -d 

echo "Pronto! Acesse o visualizador de logs através do seguinte link: https://localhost:7777"
