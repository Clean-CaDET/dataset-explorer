# Stage 1: Build Stage
# Use .NET 5.0 SDK for building the application (matches project target framework)
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy csproj file and restore dependencies (for better layer caching)
COPY DataSetExplorer.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application in Release mode
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime Stage
# Use lightweight ASP.NET Core 5.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app

# Fix Debian package repository URLs
RUN sed -i 's|deb.debian.org|archive.debian.org|g' /etc/apt/sources.list && \
    sed -i 's|security.debian.org|archive.debian.org|g' /etc/apt/sources.list && \
    echo 'Acquire::Check-Valid-Until "false";' > /etc/apt/apt.conf.d/99no-check-valid

# Install system dependencies (Git, libgit2, Python 3, and pip)
RUN apt-get update && \
    apt-get install -y \
        git \
        libgit2-27 \
        python3 \
        python3-pip && \
    rm -rf /var/lib/apt/lists/*

# Copy published application from build stage
COPY --from=build /app/publish .

# Copy LibGit2Sharp native binaries to app root for easier loading
# Use debian.9-x64 version which is compatible with Debian Buster (OpenSSL 1.1)
RUN cp -f /app/runtimes/debian.9-x64/native/libgit2-106a5f2.so /app/ || true && \
    chmod +x /app/libgit2-106a5f2.so || true

# Install Python dependencies for annotation consistency and community detection
COPY Core/AnnotationConsistency/requirements.txt /tmp/requirements.txt
RUN pip3 install --no-cache-dir -r /tmp/requirements.txt && \
    pip3 install --no-cache-dir networkx infomap && \
    rm /tmp/requirements.txt

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Set library search path for LibGit2Sharp native library
ENV LD_LIBRARY_PATH=/app:/app/runtimes/debian.9-x64/native:/usr/lib/x86_64-linux-gnu

# Expose port 5000 for HTTP
EXPOSE 5000

# Create SSH directory
RUN mkdir -p /root/.ssh

ARG DEPLOY_KEY
ARG SSH_CONFIG

# Copy SSH key and config - local setup
#COPY deploy_key /root/.ssh/id_rsa
#COPY ssh_config /root/.ssh/config

# Fix permissions - deploy setup
RUN echo "$DEPLOY_KEY" > /root/.ssh/id_rsa \
 && echo "$SSH_CONFIG" > /root/.ssh/config \
 && chmod 700 /root/.ssh \
 && chmod 600 /root/.ssh/id_rsa \
 && chmod 600 /root/.ssh/config \
 && ssh-keyscan github.com >> /root/.ssh/known_hosts \
 && chmod 644 /root/.ssh/known_hosts

# Fix permissions - local setup
RUN chmod 700 /root/.ssh \
 && chmod 600 /root/.ssh/id_rsa \
 && chmod 600 /root/.ssh/config \
 && ssh-keyscan github.com >> /root/.ssh/known_hosts \
 && chmod 644 /root/.ssh/known_hosts

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "DataSetExplorer.dll"]
