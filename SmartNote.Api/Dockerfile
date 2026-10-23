FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["SmartNote.Api/SmartNote.Api.csproj", "SmartNote.Api/"]

RUN dotnet restore "SmartNote.Api/SmartNote.Api.csproj"

COPY SmartNote.Api/ SmartNote.Api/

WORKDIR /src/SmartNote.Api

FROM build AS publish

RUN dotnet publish "SmartNote.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartNote.Api.dll"]