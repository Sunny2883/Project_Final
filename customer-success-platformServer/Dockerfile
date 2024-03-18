FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
RUN dotnet tool install -g Volo.Abp.Cli
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR ./Promact.CustomerSuccess.Platform
COPY --from=build-env /App/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Promact.CustomerSuccess.Platform.dll"]