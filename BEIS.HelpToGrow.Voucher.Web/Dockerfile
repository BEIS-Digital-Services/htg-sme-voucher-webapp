FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY BEIS.HelpToGrow.Voucher.Web/Beis.HelpToGrow.Voucher.Web.csproj .
RUN dotnet restore "./Beis.HelpToGrow.Voucher.Web.csproj"
COPY . .
WORKDIR "./Beis.HelpToGrow.Voucher.Web"
RUN dotnet build "Beis.HelpToGrow.Voucher.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Beis.HelpToGrow.Voucher.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Beis.HelpToGrow.Voucher.Web.dll"]