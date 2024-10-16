# ArticleReview

# Running 
- install docker
- run `docker compose up` in terminal 

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


# Auth Api Details
- AuthApi API will start at port 7004
- For swagger `http://localhost:7004/swagger/index.html`
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
  "username": "Member",
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
 
Login as ReviewApiUser. Given role to all endoints at Review api. Only Review Api. Throw exception at Post Reviews endpoint, because not able to inner call to Article api to validate ArticleId is valid or not.
curl -X 'POST' \
  'http://localhost:7004/api/v1/Auth' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "ReviewApiUser",
  "password": "kloia"
}'


```


# Article Api Details
- Article API will start at port 7000
- For swagger `http://localhost:7000/swagger/index.html`
```sh 
Post => Create Article
curl --location 'http://localhost:7000/api/v1/Articles' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}' \
--data '{
  "author": "string",
  "articleContent": "string",
  "publishDate": "2024-10-16T01:08:43.867Z",
  "starCount": 2,
  "title": "string"
}'

PUT => Update Article
curl --location --request PUT 'http://localhost:7000/api/v1/Articles/0447b136-f0d9-4491-a871-ff35eba13ee5' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}' \
--data '{
  "author": "string",
  "articleContent": "string",
  "publishDate": "2024-10-16T01:08:43.867Z",
  "starCount": 2,
  "title": "string"
}'

DELETE => Soft Delete Article
curl --location --request DELETE 'http://localhost:7000/api/v1/Articles/0447b136-f0d9-4491-a871-ff35eba13ee5' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}'

GET => Can Search with filter, top, skip, orderby, select
curl --location 'http://localhost:7000/api/v1/articles?%24select=id&%24top=100' \
--header 'Authorization: Bearer {{token}}'

GET => Get By Id
curl --location 'http://localhost:7000/api/v1/articles/{{id}}' \
--header 'Authorization: Bearer {{token}}'
```



# Review API  
- Review API will start at port 7002
- For swagger `http://localhost:7002/swagger/index.html`
```sh 
POST => Create Review
curl --location 'http://localhost:7002/api/v1/reviews' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}' \
--data '{
  "articleId": "257f278a-46b1-4480-a5c9-fbb71a389570",
  "reviewer": "string",
  "reviewContent": "string"
}'

PUT => Update Review
curl --location --request PUT 'http://localhost:7002/api/v1/reviews/bcab0a51-6cbb-46d8-b590-1271384c07a7' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}' \
--data '{
  "articleId": "257f278a-46b1-4480-a5c9-fbb71a389570",
  "reviewer": "string",
  "reviewContent": "string"
}'

DELETE => Soft Delete Review
curl --location --request DELETE 'http://localhost:7002/api/v1/reviews/bcab0a51-6cbb-46d8-b590-1271384c07a7' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}'

GET => Can Search with filter, top, skip, orderby, select
curl --location 'http://localhost:7002/api/v1/reviews' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}'

GET => Get By Id
curl --location 'http://localhost:7002/api/v1/reviews/{{id}}' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer {{token}}'
```
