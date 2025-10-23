FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY ./CouponAPI /home/app
WORKDIR /home/app
RUN dotnet restore CouponAPI.csproj && dotnet publish -c release -o /build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV ASPNETCORE_URLS=http://+:4006
WORKDIR /home/app
COPY --from=build /build .
EXPOSE 4006
ENTRYPOINT ["dotnet", "CouponAPI.dll"]