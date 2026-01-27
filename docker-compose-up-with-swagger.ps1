# Docker Compose script that opens Swagger automatically
Write-Host "Starting Docker Compose services..." -ForegroundColor Yellow
Write-Host "Swagger will open automatically after services are ready.`n" -ForegroundColor Cyan

# Start docker compose in background
$job = Start-Job -ScriptBlock {
    docker compose up
}

# Wait a bit for services to start, then open Swagger
Start-Sleep -Seconds 20

Write-Host "`nOpening Swagger UI for all APIs..." -ForegroundColor Green

# Open Basket API Swagger
Start-Process "http://localhost:6001/swagger"
Write-Host "✓ Opened Basket API Swagger: http://localhost:6001/swagger" -ForegroundColor Cyan

# Open Catalog API Swagger  
Start-Process "http://localhost:6000/swagger"
Write-Host "✓ Opened Catalog API Swagger: http://localhost:6000/swagger" -ForegroundColor Cyan

Write-Host "`nSwagger UIs opened! Docker Compose is running in the background." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the services.`n" -ForegroundColor Yellow

# Wait for the job to complete
Wait-Job $job | Out-Null
Receive-Job $job
