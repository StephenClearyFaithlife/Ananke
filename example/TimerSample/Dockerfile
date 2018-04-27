FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app
COPY publish .
ENTRYPOINT ["dotnet", "TimerSample.dll"]
