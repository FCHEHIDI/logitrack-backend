# Production-ready Dockerfile for ASP.NET Core LogiTrack API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["LogiTrack/LogiTrack.csproj", "LogiTrack/"]
RUN dotnet restore "LogiTrack/LogiTrack.csproj"
COPY . .
WORKDIR "/src/LogiTrack"
RUN dotnet publish "LogiTrack.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "LogiTrack.dll"]
