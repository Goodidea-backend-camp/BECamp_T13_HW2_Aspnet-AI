FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 5199

ENV ASPNETCORE_URLS=http://+:5199

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG configuration=Release
WORKDIR /source
COPY ["src/BECamp_T13_HW2_Aspnet-AI.csproj", "src/"]
RUN dotnet restore "src/BECamp_T13_HW2_Aspnet-AI.csproj"
COPY . .
WORKDIR "/source/src"
RUN dotnet build "BECamp_T13_HW2_Aspnet-AI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BECamp_T13_HW2_Aspnet-AI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BECamp_T13_HW2_Aspnet-AI.dll"]
