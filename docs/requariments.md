### Database
PostgresDB -> localhost:5432
user: postgres_user
password: postgres_password

# Cache
1. Redis -> localhost:6379,connectTimeout=2000,connectRetry=0,syncTimeout=2000,asyncTimeout=2000
2. use LRU cache policy
   


# Names and layers
1. presentation -> GRPC and GRPC.Contacts for contracts
2. application -> Application -> buisness logic
3. infrastructure -> DAL and infrastructure services

# Data source
1. GRPC data source
grpc://server-api.pilot.svc.dev.sweedkube.com:5006 - CartAssitantService/GetOptionSetList

request - > docs/data-request.json
response - > docs/data-response.json


As a user I want to get cart assistant data from the GRPC data source and merge it with the database data and get complete object with all properties

# Acceptence criteria:
1. Service should get data from the GRPC data source
2. Service should merge data from the GRPC data source and the database
3. Service should return merged data example: docs/response.json
4. request should be docs/request.json 
5. for request to datasource get max count from the settings
6. for image you need to use next file: docs/image.json use fuzzy serch to match imagename with name property
7. if score of matching is low use DefaultImageUrl


# Additional info
<add key="gitlab" value="https://gitlab.walli.com/api/v4/projects/266/packages/nuget/index.json"/>

1. GRPC use code first approach - CartAssitantService/GetOptionSetList -> nuget - >  SD.Ecom.CartAssistant.Grpc.Contracts
   