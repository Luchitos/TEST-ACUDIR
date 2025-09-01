# build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY Acudir.Test.Apis/*.csproj Acudir.Test.Apis/
COPY Domain/*.csproj Domain/
COPY Data/*.csproj Data/
COPY Application/*.csproj Application/
COPY Infrastructure/*.csproj Infrastructure/

RUN dotnet restore Acudir.Test.Apis/Acudir.Test.Apis.csproj
RUN dotnet restore Domain/Domain.csproj
RUN dotnet restore Data/Data.csproj
RUN dotnet restore Application/Application.csproj
RUN dotnet restore Infrastructure/Infrastructure.csproj

COPY . ./
RUN dotnet publish Acudir.Test.Apis/Acudir.Test.Apis.csproj -c Release -o out

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "Acudir.Test.Apis.dll"]
