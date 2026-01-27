# Script to open Swagger UI for all microservices
# Wait for services to be ready (adjust delay as needed)
Write-Host "Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

Write-Host "Opening Swagger UI for all APIs..." -ForegroundColor Green

# Open Basket API Swagger
Start-Process "http://localhost:6001/swagger"
Write-Host "✓ Opened Basket API Swagger: http://localhost:6001/swagger" -ForegroundColor Cyan

# Open Catalog API Swagger
Start-Process "http://localhost:6000/swagger"
Write-Host "✓ Opened Catalog API Swagger: http://localhost:6000/swagger" -ForegroundColor Cyan

Write-Host "`nAll Swagger UIs have been opened!" -ForegroundColor Green
