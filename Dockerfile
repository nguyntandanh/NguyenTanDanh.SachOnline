﻿# ------------ Build stage ------------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the full source code and build
COPY . ./
RUN dotnet publish -c Release -o out

# ------------ Runtime stage ------------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Run the application
ENTRYPOINT ["dotnet", "NguyenTanDanh.SachOnline.dll"]
