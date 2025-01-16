# SmartWebShop

A SmartWebShop application built with ASP .NET Technology. The frontend is an ASP .NET web app with Razor Pages, and the backend is an ASP .NET Core web API. The project is written in .NET 8.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Frontend Screens](#frontend-screens)
- [Backend API](#backend-api)

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


