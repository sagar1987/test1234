#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG MICROSERVICE=''
WORKDIR /src
COPY ["Lakeshore.Service/Lakeshore.Service.csproj", "Lakeshore.Service/"]
COPY ["Lakeshore.Api/Lakeshore.Api.csproj", "Lakeshore.Api/"]
COPY ["Lakeshore.Domain/Lakeshore.Domain.csproj", "Lakeshore.Domain/"]
COPY ["Lakeshore.Dto/Lakeshore.Dto.csproj", "Lakeshore.Dto/"]
COPY ["Lakesore.Application/Lakeshore.Application.csproj", "Lakesore.Application/"]
COPY ["Lakeshore.Infrastructure/Lakeshore.Infrastructure.csproj", "Lakeshore.Infrastructure/"]
RUN dotnet restore -s "https://hq-artifactorypro.llmhq.com/artifactory/api/nuget/v3/nuget-local" -s "https://api.nuget.org/v3/index.json" "Lakeshore.Service/Lakeshore.Service.csproj"
RUN dotnet restore "Lakeshore.Service/Lakeshore.Service.csproj"
COPY . .
WORKDIR "/src/Lakeshore.Service"
RUN dotnet build "Lakeshore.Service.csproj" -c Release -o /app/build
FROM build AS publish
ARG MICROSERVICE=''
RUN dotnet publish "Lakeshore.Service.csproj" -c Release -o /app/publish
FROM base AS final
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
ARG MICROSERVICE=''
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lakeshore.Service.dll"]