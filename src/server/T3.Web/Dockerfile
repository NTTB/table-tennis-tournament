FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["TTT.Web/TTT.Web.csproj", "TTT.Web/"]
RUN dotnet restore "TTT.Web/TTT.Web.csproj"
COPY . .
WORKDIR "/src/TTT.Web"
RUN dotnet build "TTT.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TTT.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TTT.Web.dll"]
