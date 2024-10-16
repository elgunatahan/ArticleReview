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


# AuthApi Details
```sh 
Post => Create Article
curl --location 'http://localhost:7000/api/v1/Articles' \
--header 'accept: */*' \
--header 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' \
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5MDg4ZGQwNy1jMWM0LTQxYmMtOGY0OC0zZjI0OTQ5NTI0NmQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkMTQ3Zjg0LTk1NDItNGRmYS05NWY0LTdlOWM5OGZjMDZjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwiZXhwIjoxNzI5MDQxMDg5LCJpc3MiOiJLbG9pYSJ9.M14aBJEr3J8z2JkiubKtiNQq2jw8ZbmWACIJqPoYvJYx_TNdvJQfkJE6XMSuLCEFlPlIMLKHp1TOcd-S2JNKPqJaM8ie_y0q4JivJ3iYztDrU6z9N47STMkethKPaLn2x53oWmSw63nYirTJsnUOlYsZXankhMpi4SW890OXZxIhbxbmYodtrBSQygYXIaGD63vJPI5qUvPjWkUSsFhSSUUL4vj0duuZkjst4c80DTc5Wezte3ophhZup0eDFQCpJ3HBbg_RvbNqk63nVIJZ4bmq8delXDaEp5JwhtI5GKAxMsOCVqIcc9OYewsX9O72H-beGSwohq8NtznadkfEvQ' \
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
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5MDg4ZGQwNy1jMWM0LTQxYmMtOGY0OC0zZjI0OTQ5NTI0NmQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkMTQ3Zjg0LTk1NDItNGRmYS05NWY0LTdlOWM5OGZjMDZjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwiZXhwIjoxNzI5MDQxMDg5LCJpc3MiOiJLbG9pYSJ9.M14aBJEr3J8z2JkiubKtiNQq2jw8ZbmWACIJqPoYvJYx_TNdvJQfkJE6XMSuLCEFlPlIMLKHp1TOcd-S2JNKPqJaM8ie_y0q4JivJ3iYztDrU6z9N47STMkethKPaLn2x53oWmSw63nYirTJsnUOlYsZXankhMpi4SW890OXZxIhbxbmYodtrBSQygYXIaGD63vJPI5qUvPjWkUSsFhSSUUL4vj0duuZkjst4c80DTc5Wezte3ophhZup0eDFQCpJ3HBbg_RvbNqk63nVIJZ4bmq8delXDaEp5JwhtI5GKAxMsOCVqIcc9OYewsX9O72H-beGSwohq8NtznadkfEvQ' \
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
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5MDg4ZGQwNy1jMWM0LTQxYmMtOGY0OC0zZjI0OTQ5NTI0NmQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkMTQ3Zjg0LTk1NDItNGRmYS05NWY0LTdlOWM5OGZjMDZjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwiZXhwIjoxNzI5MDQxMDg5LCJpc3MiOiJLbG9pYSJ9.M14aBJEr3J8z2JkiubKtiNQq2jw8ZbmWACIJqPoYvJYx_TNdvJQfkJE6XMSuLCEFlPlIMLKHp1TOcd-S2JNKPqJaM8ie_y0q4JivJ3iYztDrU6z9N47STMkethKPaLn2x53oWmSw63nYirTJsnUOlYsZXankhMpi4SW890OXZxIhbxbmYodtrBSQygYXIaGD63vJPI5qUvPjWkUSsFhSSUUL4vj0duuZkjst4c80DTc5Wezte3ophhZup0eDFQCpJ3HBbg_RvbNqk63nVIJZ4bmq8delXDaEp5JwhtI5GKAxMsOCVqIcc9OYewsX9O72H-beGSwohq8NtznadkfEvQ'

GET => Get All with filter, top, skip, orderby, select
curl --location 'http://localhost:7000/api/v1/articles?%24select=id&%24top=100' \
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIzODQwNTI1MS1hN2ZmLTQxZTgtYjEyMS05YTczOWVjZDE3NzgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkMTQ3Zjg0LTk1NDItNGRmYS05NWY0LTdlOWM5OGZjMDZjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwiZXhwIjoxNzI5MDQwMzMyLCJpc3MiOiJLbG9pYSJ9.LsRypZcezh5RYhWMDa06q8pdVVn-jDPUcbGodEQNHy1h0Qy0g53tdHVBG14EjXHomFYGmUEBr0BQCbbBuII23ueXS1uqN354xYZT-mVtlyaYnOWMd9N7fdYRBcGvJo55yELBVoeB3lsCiCFUshZBywgaH-zLtzaaCHoP9VQ9lYDNT8FvIT1u0F91cz5CDd9dXli0oW0UWZnC1jRLpAI7brpk2fxZ-sZq4Ge-9KTh9nhiuP3w0KfJBMwhhg-Z2JBfxWHjoXnR-rFFNY7nlkVVib9W_18VwziQgS_ociogj4bWzwXE-mvifDKSIHaCAQ_2nmGfOxadvuWTDitPghdUlw'

GET => Get By Id
curl --location 'http://localhost:7000/api/v1/articles/bcd29dd7-04d9-4fc5-9129-d013fb57cb56' \
--header 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIzODQwNTI1MS1hN2ZmLTQxZTgtYjEyMS05YTczOWVjZDE3NzgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRkMTQ3Zjg0LTk1NDItNGRmYS05NWY0LTdlOWM5OGZjMDZjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwiZXhwIjoxNzI5MDQwMzMyLCJpc3MiOiJLbG9pYSJ9.LsRypZcezh5RYhWMDa06q8pdVVn-jDPUcbGodEQNHy1h0Qy0g53tdHVBG14EjXHomFYGmUEBr0BQCbbBuII23ueXS1uqN354xYZT-mVtlyaYnOWMd9N7fdYRBcGvJo55yELBVoeB3lsCiCFUshZBywgaH-zLtzaaCHoP9VQ9lYDNT8FvIT1u0F91cz5CDd9dXli0oW0UWZnC1jRLpAI7brpk2fxZ-sZq4Ge-9KTh9nhiuP3w0KfJBMwhhg-Z2JBfxWHjoXnR-rFFNY7nlkVVib9W_18VwziQgS_ociogj4bWzwXE-mvifDKSIHaCAQ_2nmGfOxadvuWTDitPghdUlw'
```
