FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY ./PaymentAPI /home/app
WORKDIR /home/app
RUN dotnet restore PaymentAPI.csproj && dotnet publish -c release -o /build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV ASPNETCORE_URLS=http://+:4010
WORKDIR /home/app
COPY --from=build /build .
EXPOSE 4010
ENTRYPOINT ["dotnet", "PaymentAPI.dll"]