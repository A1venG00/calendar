# Use the base ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Use the SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["lab2.csproj", "lab2/"]
RUN dotnet restore "lab2/lab2.csproj"

WORKDIR "/src/lab2"

# Copy the rest of the source code
COPY . .


# Publish the CalendarEvents service
RUN dotnet publish "lab2.csproj" -c Release -r linux-x64 -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "lab2.dll"]
