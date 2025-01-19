# SmartWebShop

A SmartWebShop application built with ASP .NET Technology. The frontend is an ASP .NET web app with Razor Pages, and the backend is an ASP .NET Core web API. The project is written in .NET 8.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Frontend Screens](#frontend-screens)
- [Backend API](#backend-api)
- [Setting up User Secrets](#setting-up-user-secrets)


## Installation

Instructions on how to install and set up the project.

```bash

# Install frontend dependencies
cd Contoso.WebApp
dotnet build

# Navigate to the backend directory
cd ../Contoso.Api

# Build the backend project
dotnet build

# Run the database using docker-compose
docker-compose up -d
```

## Usage

Instructions on how to use the project.

```bash
# Run the frontend project
cd Contoso.WebApp
dotnet run

# Run the backend project
cd ../Contoso.Api
dotnet run
```

## Frontend Screens

- `/Login` - Login screen
- `/Home` - Viewing products available in the webshop
- `/Product/id` - Viewing details of the selected product
- `/Cart` - Cart site with checkout functionality

## Backend API

The API uses a SQL Server database with Product, Order, OrderItem, and User tables. The database is currently deployed locally via docker-compose.
he API has the following controllers:

- `/api/Order` - Create, get one, get all orders
- `/api/Product` - CRUD for products
- `/api/Account` - Login, Register

## Setting up User Secrets

To securely store sensitive information such as connection strings, you can use the `dotnet user-secrets` tool. Follow the steps below to set up user secrets for the project:

1. Navigate to the backend project directory:

    ```bash
    cd ../Contoso.Api
    ```

2. Initialize user secrets for the project:

    ```bash
    dotnet user-secrets init
    ```

3. Set the connection string as a user secret:

    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING_FOR_SQL_SERVER"
    ```

By using user secrets, you ensure that sensitive information is not hard-coded in your source code and is kept secure.



