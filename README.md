# Presidio_Training


# Packages used so far:

dotnet add package AutoMapper --version 12.0.1

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0

dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0

dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0

dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.0

dotnet add package Moq --version 4.20.72

dotnet add package Microsoft.Extensions.Logging.Log4Net.AspNetCore --version 8.0.0

dotnet add package Microsoft.AspNetCore.Authentication.Google --version 9.0.0

# Unit Testing:

dotnet new xunit -n [ProjectName].Tests

dotnet add [ProjectName].Tests reference [ProjectName]/[ProjectName].csproj

dotnet test