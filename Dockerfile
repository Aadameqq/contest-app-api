FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /home/app

COPY src/App/App.csproj ./src/App/
COPY src/App/packages.lock.json ./src/App/
RUN dotnet restore ./src/App/App.csproj --locked-mode

COPY src ./src

RUN dotnet publish ./src/App/App.csproj -c Release -o /home/app/out

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sandbox

WORKDIR /home/app

RUN dotnet tool install --global dotnet-ef --version 8.0.*
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=build /home/app/out ./app

COPY --from=build /home/app/src ./src

EXPOSE 8080

CMD ["dotnet", "/home/app/app/App.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production

WORKDIR /home/app

COPY --from=build /home/app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "App.dll"]