#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

ENV clientSecret = ""
ENV clientId = ""
ENV tenantId = ""
ENV subscriptionId = ""
ENV resourceGroupName = ""
ENV dnsZoneName = ""
ENV aRecordName = ""
ENV DebugLogging = "false"
ENV RunContinuously = "true"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/MyIp/MyIp.csproj", "src/MyIp/"]
RUN dotnet restore "src/MyIp/MyIp.csproj"
COPY . .
WORKDIR "/src/src/MyIp"
RUN dotnet build "MyIp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyIp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyIp.dll"]