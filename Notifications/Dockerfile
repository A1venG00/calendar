# Use the base ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Use the SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Notifications.csproj", "CalendarNotifications/"]
RUN dotnet restore "CalendarNotifications/Notifications.csproj"

WORKDIR "/src/CalendarNotifications"
# Copy the rest of the source code
COPY . .


# Publish the CalendarNotifications service
RUN dotnet publish "Notifications.csproj" -c Release -r linux-x64 -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Notifications.dll"]
