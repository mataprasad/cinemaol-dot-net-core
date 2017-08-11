FROM microsoft/aspnetcore:1.0
WORKDIR /app
COPY . .
#Copy sqlite db file to root diectory of published file and change the setting in appsetting.json
#In your dev m/c run bellow commands
#npm install -g bower
#npm install -g gulp
#dotnet restore
#dotnet build
#dotnet publish
ENTRYPOINT ["dotnet", "cinema.dll"]
