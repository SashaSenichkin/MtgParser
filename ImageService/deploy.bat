az container delete --name image-service -g rs-20221116232858 --yes
az container create --environment-variables ConnectionStrings__MtgContext="Server=mtg-server.database.windows.net,1433;Initial Catalog=mtg;Persist Security Info=False;User ID=MainUser;Password=LAbaexRCEFPLGS9;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30" ^
-g rs-20221116232858 ^
--image "mtgregistry.azurecr.io/imageservice" ^
--registry-username "mtgregistry" ^
--registry-password "iJFFI0QXEWtOP2SPFCxKnpG8FpftnyJqxdgQ8fEi34+ACRCdlmBs" ^
--name image-service ^
--ports 80 ^
--ip-address Public ^
--dns-name-label image-service ^
--location westeurope