#!/bin/bash

# HairAI - Bootstrap First SuperAdmin Account
# This script creates the first SuperAdmin account for the system

echo "🚀 HairAI Admin Bootstrap Script"
echo "================================="

# Check if API is running
API_URL="http://localhost:5000"
echo "📡 Checking if HairAI API is running at $API_URL..."

if ! curl -s -f "$API_URL/health" > /dev/null 2>&1; then
    echo "❌ Error: HairAI API is not running at $API_URL"
    echo "Please start the backend first:"
    echo "   cd Backend && dotnet run --project HairAI.Api"
    exit 1
fi

echo "✅ API is running!"
echo ""

# Bootstrap the admin account
echo "🔐 Creating first SuperAdmin account..."
echo ""

response=$(curl -s -X POST "$API_URL/auth/bootstrap-admin" \
  -H "Content-Type: application/json" \
  -w "\n%{http_code}")

# Extract response body and status code
http_code=$(echo "$response" | tail -n1)
response_body=$(echo "$response" | head -n -1)

if [ "$http_code" = "200" ]; then
    echo "✅ SuperAdmin account created successfully!"
    echo ""
    echo "📋 Login Credentials:"
    echo "   Email: admin@hairai.com"
    echo "   Password: SuperAdmin123!"
    echo ""
    echo "🌐 Access Points:"
    echo "   Main Frontend: http://localhost:3000"
    echo "   AdminDashboard: http://localhost:3001"
    echo ""
    echo "⚠️  Important: Change the default password after first login!"
    echo ""
    echo "📊 Full Response:"
    echo "$response_body" | jq . 2>/dev/null || echo "$response_body"
else
    echo "❌ Failed to create SuperAdmin account"
    echo "HTTP Status: $http_code"
    echo "Response: $response_body"
    
    # Check if admin already exists
    if echo "$response_body" | grep -q "SuperAdmin already exists"; then
        echo ""
        echo "ℹ️  It looks like a SuperAdmin already exists."
        echo "   You can login with: admin@hairai.com"
        echo "   Default password: SuperAdmin123!"
    fi
fi

echo ""
echo "🏥 Next Steps:"
echo "1. Login to AdminDashboard: http://localhost:3001"
echo "2. Create clinics using the Clinics page"
echo "3. Create additional users (ClinicAdmin/Doctors)"
echo "4. Assign users to clinics as needed"

