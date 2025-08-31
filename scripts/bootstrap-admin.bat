@echo off
REM HairAI - Bootstrap First SuperAdmin Account
REM This script creates the first SuperAdmin account for the system

echo 🚀 HairAI Admin Bootstrap Script
echo =================================

REM Check if API is running
set API_URL=http://localhost:5000
echo 📡 Checking if HairAI API is running at %API_URL%...

curl -s -f "%API_URL%/health" >nul 2>&1
if errorlevel 1 (
    echo ❌ Error: HairAI API is not running at %API_URL%
    echo Please start the backend first:
    echo    cd Backend ^&^& dotnet run --project HairAI.Api
    pause
    exit /b 1
)

echo ✅ API is running!
echo.

REM Bootstrap the admin account
echo 🔐 Creating first SuperAdmin account...
echo.

curl -X POST "%API_URL%/auth/bootstrap-admin" ^
  -H "Content-Type: application/json" ^
  -w "%%{http_code}" ^
  -o response.tmp

set /p http_code=<response.tmp
if %http_code%==200 (
    echo ✅ SuperAdmin account created successfully!
    echo.
    echo 📋 Login Credentials:
    echo    Email: admin@hairai.com
    echo    Password: SuperAdmin123!
    echo.
    echo 🌐 Access Points:
    echo    Main Frontend: http://localhost:3000
    echo    AdminDashboard: http://localhost:3001
    echo.
    echo ⚠️  Important: Change the default password after first login!
    echo.
    echo 📊 Response:
    type response.tmp
) else (
    echo ❌ Failed to create SuperAdmin account
    echo HTTP Status: %http_code%
    echo Response:
    type response.tmp
    echo.
    echo ℹ️  If admin already exists, you can login with:
    echo    Email: admin@hairai.com
    echo    Password: SuperAdmin123!
)

del response.tmp >nul 2>&1

echo.
echo 🏥 Next Steps:
echo 1. Login to AdminDashboard: http://localhost:3001
echo 2. Create clinics using the Clinics page
echo 3. Create additional users (ClinicAdmin/Doctors)
echo 4. Assign users to clinics as needed

pause

