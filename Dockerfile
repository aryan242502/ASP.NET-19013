# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution
COPY *.sln ./

# Copy project folder
COPY DoctorAppointmentSystem/*.csproj DoctorAppointmentSystem/

# Restore
RUN dotnet restore

# Copy everything
COPY . .

# Publish
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DoctorAppointment.dll"]
