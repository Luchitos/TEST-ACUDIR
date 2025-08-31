FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Crear el archivo .env con las variables de entorno necesarias en el directorio Acudir.Test.Apis
RUN echo "JWT_SECRET_KEY=${JWT_SECRET_KEY}" > .env

# Copiar los archivos de cada proyecto y restaurar dependencias individualmente
COPY Acudir.Test.Apis/*.csproj Acudir.Test.Apis/
RUN dotnet restore Acudir.Test.Apis/Acudir.Test.Apis.csproj

COPY Domain/*.csproj Domain/
RUN dotnet restore Domain/Domain.csproj

COPY Data/*.csproj Data/
RUN dotnet restore Data/Data.csproj

COPY Application/*.csproj Application/
RUN dotnet restore Application/Application.csproj

# Copiar el resto de los archivos y construir la aplicaci�n
COPY . ./
RUN dotnet publish Acudir.Test.Apis/Acudir.Test.Apis.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./


ENTRYPOINT ["dotnet", "Acudir.Test.Apis.dll"]
