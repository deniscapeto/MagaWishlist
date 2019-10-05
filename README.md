# MagaWishlist

Magawish is an API that allow developers to create, modify, delete and query customers and add products to their wishlist.

# Installation

Note: This API was designed to use MySql database. If you don't have access to a mtsql database please change the configuration in appsettings.json file as follow:

```
    "DatabaseType": "sqlite",
```

### Install Entity Framework command line tool

`dotnet tool install --global dotnet-ef --version 2.1`

### Make Migrations

`dotnet ef migrations add InitialCreate`

### Apply Migrations

`dotnet ef database update`

Note: If you are using MySql Database, don't create the schema. Migrations process will do it.

# How to Run

### From the terminal

Navigate to API project folder:

`cd MagaWishlist`

Since the project is using https run this command to generate a developer certificate:

`dotnet dev-certs https -t`

Run:

`dotnet run`

# How to Use

## Autentication

This is API is using jwt for authentication.
Make a POST request to the endpoint `/api/auth` with the body:

```
{
  "username": "teste",
  "accesskey": "teste"
}
```

*The generated token is necessary for all other requests* (It expires in 60 minutes by default)

## Customer

use the endpoit `/api/customer` as follows:

### Get Customer informations:

GET `/api/customer/123` where 123 is the customer ID

### Create new Customer:

POST `/api/customer` with the body (Content-Type application/json):

```
{
	"name": "Miles David",
	"email": "miles.davis@luizalabs.com"
}
```

### Update Customer Information:

PUT `/api/customer/123`

body (Content-Type application/json):

```
{
	"name": "Miles Davis Jr",
	"email": "miles.davis@luizalabs.com"
}
```

### Delete Customer (also the wishlist):

DELETE `/api/customer/123` 

## Wishlist

### Get Customer Wishlist

GET `/api/customer/123/wishlist` where 123 is the customer ID 

The response body, should be something as follows:

```
[
    {
        "productId": "1bf0f365-fbdd-4e21-9786-da459d78dd1f",
        "price": "1604767081",
        "image": "http://images.luizalabs.com/products/1bf0f365-fbdd-4e21-9786-da459d78dd1f",
        "title": "Cadeira para Auto Iseos..."
    }
]
```

### Add new Product to Customer Wishlist

POST `/api/customer/{customerId}/wishlist/{productId}`

### Remove Product from Customer Wishlist

DELETE `/api/customer/{customerId}/wishlist/{productId}`

# Testing

Use .net deafult test command:

## Run Unit Tests

`dotnet test`

## Check Code Coverage

`dotnet tool install --global coverlet.console --version 1.6.0`

`dotnet build` caso ainda não tenha feito o build

`coverlet MagaWishlist.UnitTests/bin/Debug/netcoreapp2.2/MagaWishlist.UnitTests.dll --target "dotnet" --targetargs "test --no-build"`

# Technical Overview

### Features

   - Circuit Breaker for external API calls
   - Timeout (For dependencies) using Polly
   - Dapper (for better performance). Entity Framework is used for migrations purpose only
   - Support for MySql Database and SqlLite
   - HttpClient Factory for best performance using TCP Connections (making use of pool)
