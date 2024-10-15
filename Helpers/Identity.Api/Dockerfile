# Important Notes:
# 1. The generated self-signed certificate is for development purposes only and should NOT be used in a production environment.
#    In production, use a certificate from a trusted Certificate Authority (CA).
# 2. The password for the certificate ("Test1234!") is hardcoded for demonstration simplicity. In a real-world scenario,
#    secrets like this should be managed securely, for example using environment variables, a secrets manager, or a vault.
# 3. This Dockerfile is designed to provide a simple way to run the Identity API locally, with HTTPS enabled, to simulate a real-world API.
#    However, many security considerations, such as certificate management, should be properly handled before using in any production-like environment.

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY Identity.Api.csproj .
RUN dotnet restore "Identity.Api.csproj"

# Copy the entire project and build the application
COPY . .
WORKDIR /src
RUN dotnet build "Identity.Api.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install OpenSSL to generate certificate
RUN apt-get update && apt-get install -y openssl

# Generate self-signed certificate and create .pfx file
RUN mkdir /https && \
    openssl req -x509 -newkey rsa:2048 -keyout /https/key.pem -out /https/cert.pem -days 365 -nodes -subj "/CN=localhost" && \
    openssl pkcs12 -export -out /https/cert.pfx -inkey /https/key.pem -in /https/cert.pem -passout pass:Test1234!

# Set environment variables for ASP.NET Core to use the generated certificate
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:5003;http://+:5002
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=Test1234!
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx

# Copy the published output from the build stage
COPY --from=build /app/publish .

EXPOSE 5002
EXPOSE 5003

# Set the entrypoint to run the application
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
