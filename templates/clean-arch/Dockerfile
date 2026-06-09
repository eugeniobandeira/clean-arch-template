FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["01 - aspire/02 - ServiceDefaults/CleanArch.ServiceDefaults/CleanArch.ServiceDefaults.csproj", "01 - aspire/02 - ServiceDefaults/CleanArch.ServiceDefaults/"]
COPY ["02 - src/03 - Domain/CleanArch.Domain/CleanArch.Domain.csproj", "02 - src/03 - Domain/CleanArch.Domain/"]
COPY ["02 - src/02 - Application/CleanArch.Application/CleanArch.Application.csproj", "02 - src/02 - Application/CleanArch.Application/"]
COPY ["02 - src/05 - Infrastructure/CleanArch.Infrastructure/CleanArch.Infrastructure.csproj", "02 - src/05 - Infrastructure/CleanArch.Infrastructure/"]
COPY ["02 - src/04 - IoC/CleanArch.IoC/CleanArch.IoC.csproj", "02 - src/04 - IoC/CleanArch.IoC/"]
COPY ["02 - src/01 - Api/CleanArch.Api/CleanArch.Api.csproj", "02 - src/01 - Api/CleanArch.Api/"]

RUN dotnet restore "02 - src/01 - Api/CleanArch.Api/CleanArch.Api.csproj"

COPY . .

RUN dotnet publish "02 - src/01 - Api/CleanArch.Api/CleanArch.Api.csproj" \
    --no-restore \
    --configuration Release \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CleanArch.Api.dll"]
