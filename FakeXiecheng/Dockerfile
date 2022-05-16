FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FakeXieCheng.API.csproj", "./"]
RUN dotnet restore "./FakeXieCheng.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FakeXieCheng.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FakeXieCheng.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FakeXieCheng.API.dll"]
