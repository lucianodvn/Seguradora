# ---------- Estágio 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Configurações globais do build (TreatWarningsAsErrors, analisadores)
COPY Directory.Build.props .editorconfig ./

# Copia só os .csproj primeiro: o restore fica em cache enquanto as dependências não mudarem
COPY Seguradora.Domain/Seguradora.Domain.csproj Seguradora.Domain/
COPY Seguradora.Application/Seguradora.Application.csproj Seguradora.Application/
COPY Seguradora.Infrastructure/Seguradora.Infrastructure.csproj Seguradora.Infrastructure/
COPY Seguradora.Api/Seguradora.Api.csproj Seguradora.Api/
RUN dotnet restore Seguradora.Api/Seguradora.Api.csproj

# Agora o código-fonte
COPY Seguradora.Domain/ Seguradora.Domain/
COPY Seguradora.Application/ Seguradora.Application/
COPY Seguradora.Infrastructure/ Seguradora.Infrastructure/
COPY Seguradora.Api/ Seguradora.Api/
RUN dotnet publish Seguradora.Api/Seguradora.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Estágio 2: imagem final (só o runtime) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Usuário sem privilégio (a imagem já traz um, definido em APP_UID)
USER $APP_UID

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Seguradora.Api.dll"]