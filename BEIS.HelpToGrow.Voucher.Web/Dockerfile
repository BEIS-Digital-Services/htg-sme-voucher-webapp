FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY beis-htg-sme-voucher-webapp/BEIS.HelpToGrow.Voucher.Web.csproj .
RUN dotnet restore "./BEIS.HelpToGrow.Voucher.Web.csproj"
COPY . .
WORKDIR "/src/beis-htg-sme-voucher-webapp"
RUN dotnet build "BEIS.HelpToGrow.Voucher.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BEIS.HelpToGrow.Voucher.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BEIS.HelpToGrow.Voucher.Web.dll"]