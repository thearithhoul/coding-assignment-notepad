FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY notepad-backend.csproj .
RUN dotnet restore notepad-backend.csproj

COPY . .
RUN dotnet publish notepad-backend.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "notepad-backend.dll"]
