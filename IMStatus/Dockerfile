FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IMStatus/IMStatus.csproj", "IMStatus/"]
RUN dotnet restore "IMStatus/IMStatus.csproj"
COPY . .
WORKDIR "/src/IMStatus"
RUN dotnet build "IMStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IMStatus.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IMStatus.dll"]
