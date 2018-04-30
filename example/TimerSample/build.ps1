dotnet publish -c Release -o ../publish
docker build --tag timersample .
docker build --tag timersamplewin --file Dockerfile.WindowsContainer .
