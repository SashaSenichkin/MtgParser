docker-compose down
docker rmi $(docker images --filter "dangling=true" -q --no-trunc)

docker compose --env-file .env up -d