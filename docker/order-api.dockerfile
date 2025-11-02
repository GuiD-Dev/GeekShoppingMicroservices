FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY ./OrderAPI /home/app
WORKDIR /home/app
RUN dotnet restore OrderAPI.csproj && dotnet publish -c release -o /build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV ASPNETCORE_URLS=http://+:4008
WORKDIR /home/app
COPY --from=build /build .
EXPOSE 4008
ENTRYPOINT ["dotnet", "OrderAPI.dll"]