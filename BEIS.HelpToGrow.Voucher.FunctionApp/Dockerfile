FROM mcr.microsoft.com/dotnet/sdk:5.0 AS installer-env
# Build requires 3.1 SDK
COPY --from=mcr.microsoft.com/dotnet/core/sdk:3.1 /usr/share/dotnet /usr/share/dotnet

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app
RUN mkdir -p /home/site/wwwroot
#RUN dotnet publish *.csproj --output /home/site/wwwroot

WORKDIR /src
COPY BEIS.HelpToGrow.Voucher.FunctionApp/BEIS.HelpToGrow.Voucher.FunctionApp.csproj .
RUN dotnet restore "./BEIS.HelpToGrow.Voucher.FunctionApp.csproj"
COPY . .
WORKDIR "/src/BEIS.HelpToGrow.Voucher.FunctionApp"
RUN dotnet build -c Release -o app/build
FROM build AS publish
RUN dotnet publish "BEIS.HelpToGrow.Voucher.FunctionApp.csproj" -c Release -o /home/site/wwwroot

#RUN dotnet publish "BEIS.HelpToGrow.Voucher.FunctionApp/BEIS.HelpToGrow.Voucher.FunctionApp.csproj" -c Release -o /home/site/wwwroot

#COPY BEIS.HelpToGrow.Voucher.FunctionApp/BEIS.HelpToGrow.Voucher.FunctionApp.csproj .
#RUN dotnet restore "./BEIS.HelpToGrow.Voucher.FunctionApp.csproj"
#COPY . .
#RUN dotnet publish "BEIS.HelpToGrow.Voucher.FunctionApp.csproj" -c Release -o /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:3.0-dotnet-isolated5.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:3.0-dotnet-isolated5.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]