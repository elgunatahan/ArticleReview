# ArticleReview

# Running 

- install docker
- run `docker compose up` in terminal 

# Auth API
- AuthApi API will start at port 7004
- For swagger `http://localhost:7004/swagger/index.html`

# Article API  
- Article API will start at port 7000
- For swagger `http://localhost:7000/swagger/index.html`

# Review API  
- Review API will start at port 7002
- For swagger `http://localhost:7002/swagger/index.html`

# Tests
- UnitTests

# Technical Details
- .NET 8.0
- MongoDB
- Redis
- JWT
- OData
- Docker
- Serilog
 
# Practices
- DDD (Domain-driven design)
- CQRS
- Clean Architecture
- Unit Test


# AuthApi Details
```sh 
Login as Admin. Given role to all endoints at Article and Review api. Write-Read
curl -X 'POST' \
  'http://localhost:7004/api/v1/Auth' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "Admin",
  "password": "kloia"
}'

 
Login as Member. Given role to get endoints at Article and Review api. Read Only
curl -X 'POST' \
  'http://localhost:7004/api/v1/Auth' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "Admin",
  "password": "kloia"
}'
 
Login as ArticleApiUser. Given role to all endoints at Article api. Only Article Api
curl -X 'POST' \
  'http://localhost:7004/api/v1/Auth' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "ArticleApiUser",
  "password": "kloia"
}'
 
Login as ReviewApiUser. Given role to all endoints at Review api. Only Review Api. Throw exception at Post endpoint, because not able to inner call to Article api.

curl -X 'POST' \
  'http://localhost:7004/api/v1/Auth' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "ReviewApiUser",
  "password": "kloia"
}'


```
