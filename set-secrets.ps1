# Set the project path
$Project = "GivingChampion\GivingChampion.AppHost\GivingChampion.AppHost.csproj"

# Set the secret values here
$googleClientId     = "your-google-client-id"
$googleClientSecret = "your-google-client-secret"
$jwtKey             = "your-jwt-key"
$defaultConnection  = "your-postgres-connection-string"

# Optional admin seed values (leave empty if not required)
#$seedAdminEmail    = "admin@example.com"
#$seedAdminPassword = "adminpassword"
#$seedAdminFullName = "Admin User"

# Set the .NET user-secrets for the values
dotnet user-secrets set "Authentication:Google:ClientId" $googleClientId --project $Project
dotnet user-secrets set "Authentication:Google:ClientSecret" $googleClientSecret --project $Project
dotnet user-secrets set "Jwt:Key" $jwtKey --project $Project
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $defaultConnection --project $Project

# Set admin user secrets if provided
#if ($seedAdminEmail) {
#    dotnet user-secrets set "DefaultAdminUser:Email" $seedAdminEmail --project $Project
#}
#if ($seedAdminPassword) {
#    dotnet user-secrets set "DefaultAdminUser:Password" $seedAdminPassword --project $Project
#}
#if ($seedAdminFullName) {
#    dotnet user-secrets set "DefaultAdminUser:FullName" $seedAdminFullName --project $Project
#}

# Set environment variables for the same secrets
[System.Environment]::SetEnvironmentVariable("Authentication__Google__ClientId", $googleClientId, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("Authentication__Google__ClientSecret", $googleClientSecret, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("Jwt__Key", $jwtKey, [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", $defaultConnection, [System.EnvironmentVariableTarget]::User)

# Set admin environment variables if provided
#if ($seedAdminEmail) {
#    [System.Environment]::SetEnvironmentVariable("DefaultAdminUser__Email", $seedAdminEmail, [System.EnvironmentVariableTarget]::User)
#}
#if ($seedAdminPassword) {
#    [System.Environment]::SetEnvironmentVariable("DefaultAdminUser__Password", $seedAdminPassword, [System.EnvironmentVariableTarget]::User)
#}
#if ($seedAdminFullName) {
#    [System.Environment]::SetEnvironmentVariable("DefaultAdminUser__FullName", $seedAdminFullName, [System.EnvironmentVariableTarget]::User)
#}

Write-Host "User-secrets and environment variables have been updated!" -ForegroundColor Green