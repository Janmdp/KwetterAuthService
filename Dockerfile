#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["KwetterAuthenticationService/KwetterAuthenticationService.csproj", "KwetterAuthenticationService/"]
COPY ["Models/Models.csproj", "KwetterAuthenticationService/"]
COPY ["Logic/Logic.csproj", "KwetterAuthenticationService/"]
COPY ["DataAccesLayer/DataAccesLayer.csproj", "KwetterAuthenticationService/"]

RUN dotnet restore "KwetterAuthenticationService/KwetterAuthenticationService.csproj"
COPY . .
WORKDIR "/src/KwetterAuthenticationService"
RUN dotnet build "KwetterAuthenticationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KwetterAuthenticationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KwetterAuthenticationService.dll"]

#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
#WORKDIR /app
#EXPOSE 81
#EXPOSE 444

#ENV ASPNETCORE_URLS = "http://localhost:5000"

#COPY ["KwetterAuthenticationService/KwetterAuthenticationService.csproj", "KwetterAuthenticationService/"]
#COPY ["Models/Models.csproj", "KwetterAuthenticationService/"]
#COPY ["Logic/Logic.csproj", "KwetterAuthenticationService/"]
#COPY ["DataAccesLayer/DataAccesLayer.csproj", "KwetterAuthenticationService/"]
#RUN dotnet restore "KwetterAuthenticationService/KwetterAuthenticationService.csproj"

#COPY . ./
#RUN dotnet publish -c Release -o out

#FROM mcr.microsoft.com/dotnet/aspnet:5.0
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "KwetterAuthenticationService.dll"]