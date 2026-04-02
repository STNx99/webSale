param(
    [string]$SchemaFile = "railway-schema.sql"
)

$requiredVars = @("MYSQLHOST", "MYSQLPORT", "MYSQLUSER", "MYSQLPASSWORD", "MYSQLDATABASE")
$missing = @()

foreach ($var in $requiredVars) {
    if ([string]::IsNullOrWhiteSpace($env:$var)) {
        $missing += $var
    }
}

if ($missing.Count -gt 0) {
    Write-Error "Missing environment variables: $($missing -join ', ')"
    exit 1
}

if (-not (Test-Path $SchemaFile)) {
    Write-Error "Schema file not found: $SchemaFile"
    exit 1
}

Get-Content $SchemaFile -Raw |
        docker run --rm -i mysql:8 `
            mysql `
            --host=$env:MYSQLHOST `
            --port=$env:MYSQLPORT `
            --user=$env:MYSQLUSER `
            --password=$env:MYSQLPASSWORD `
      $env:MYSQLDATABASE

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to push schema to Railway MySQL."
    exit $LASTEXITCODE
}

Write-Host "Schema pushed successfully to Railway MySQL." -ForegroundColor Green