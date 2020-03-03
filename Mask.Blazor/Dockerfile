#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV PublicKey MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDFZLucigIvl/AAliSrlP0QI8vxB11C9iAEsvvZto3A/yh9MIlCoKVFbUvqAEuLpxJxMqTDDJA4C7xoukAcyXJTEiEILeqBbqSxDlsxh+L3msaim+ZKKoUnJvxuekJyFOi9H0seZbS/WytkqKhKmATOe0w94JMHFkFFON4QyERehwIDAQAB
ENV ConnectionString dummy
ENV HangfireDashboardUrl dummy
ENV ThreadNum 30
ENV SleepTime 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Mask.Blazor/Mask.Blazor.csproj", "Mask.Blazor/"]
RUN dotnet restore "Mask.Blazor/Mask.Blazor.csproj"
COPY . .
WORKDIR "/src/Mask.Blazor"
RUN dotnet build "Mask.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mask.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mask.Blazor.dll"]