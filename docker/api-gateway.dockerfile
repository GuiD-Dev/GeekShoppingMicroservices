FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY ./APIGateway /home/app
WORKDIR /home/app
RUN dotnet restore APIGateway.csproj && dotnet publish -c release -o /build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV ASPNETCORE_URLS=http://+:4000
WORKDIR /home/app
COPY --from=build /build .
EXPOSE 4000
ENTRYPOINT ["dotnet", "APIGateway.dll"]