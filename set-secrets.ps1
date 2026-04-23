# Set the project path
$Project = "GivingChampion\GivingChampion.AppHost\GivingChampion.AppHost.csproj"

# Set the secret values here
$googleClientId     = ""
$googleClientSecret = ""
$jwtKey             = ""
#$jwtIssuer          = "your-jwt-issuer"
#$jwtAudience        = "your-jwt-audience"
#$jwtAccessMinutes   = "60"
$defaultConnection  = ""

# Optional admin seed values (leave empty if not required)
#$seedAdminEmail    = "admin@example.com"
#$seedAdminPassword = "adminpassword"
#$seedAdminFullName = "Admin User"

# Set the .NET user-secrets for the values
dotnet user-secrets set "Authentication:Google:ClientId" $googleClientId --project $Project
dotnet user-secrets set "Authentication:Google:ClientSecret" $googleClientSecret --project $Project
dotnet user-secrets set "Jwt:Key" $jwtKey --project $Project
#dotnet user-secrets set "Jwt:Issuer" $jwtIssuer --project $ApiProject
#dotnet user-secrets set "Jwt:Audience" $jwtAudience --project $ApiProject
#dotnet user-secrets set "Jwt:AccessTokenMinutes" $jwtAccessMinutes --project $ApiProject
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $defaultConnection --project $Project

# Set admin user secrets if provided
#if ($seedAdminEmail) {
#    dotnet user-secrets set "DefaultAdminUser:Email" $seedAdminEmail --project $ApiProject
#}
#if ($seedAdminPassword) {
#    dotnet user-secrets set "DefaultAdminUser:Password" $seedAdminPassword --project $ApiProject
#}
#if ($seedAdminFullName) {
#    dotnet user-secrets set "DefaultAdminUser:FullName" $seedAdminFullName --project $ApiProject
#}

Write-Host "User-secrets have been updated!" -ForegroundColor Green