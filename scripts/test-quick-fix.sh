#!/bin/bash

# Quick test for authentication fix
BASE_URL="http://localhost:5000/api"

echo "🔧 Testing HairAI API Authentication Fix"
echo "======================================="

# Test health endpoint first
echo "1. Testing health endpoint..."
health_response=$(curl -s "$BASE_URL/health")
if echo "$health_response" | grep -q "healthy"; then
    echo "✅ API is running and healthy"
else
    echo "❌ API is not responding"
    echo "Make sure to:"
    echo "1. cd Backend/HairAI.Api"
    echo "2. dotnet restore"
    echo "3. dotnet run"
    exit 1
fi

# Test registration
echo ""
echo "2. Testing user registration..."
register_data='{
    "firstName": "Test",
    "lastName": "User", 
    "email": "test@hairai.com",
    "password": "TestPassword123!"
}'

register_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d "$register_data" \
    "$BASE_URL/auth/register")

if echo "$register_response" | grep -q '"success":true'; then
    echo "✅ User registration successful"
else
    echo "❌ User registration failed"
    echo "Response: $register_response"
fi

# Test login
echo ""
echo "3. Testing user login..."
login_data='{
    "email": "test@hairai.com",
    "password": "TestPassword123!"
}'

login_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d "$login_data" \
    "$BASE_URL/auth/login")

if echo "$login_response" | grep -q '"success":true'; then
    echo "✅ User login successful"
    
    # Extract token
    token=$(echo "$login_response" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
    echo "Token obtained: ${token:0:50}..."
    
    # Test authenticated endpoint
    echo ""
    echo "4. Testing authenticated endpoint (Create Clinic)..."
    clinic_data='{"name": "Test Clinic"}'
    
    clinic_response=$(curl -s -X POST \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $token" \
        -d "$clinic_data" \
        "$BASE_URL/clinics")
    
    if echo "$clinic_response" | grep -q '"success":true'; then
        echo "✅ Authenticated endpoint working!"
        echo "🎉 JWT Authentication is working correctly!"
    else
        echo "❌ Authenticated endpoint failed"
        echo "Response: $clinic_response"
    fi
else
    echo "❌ User login failed"
    echo "Response: $login_response"
fi

echo ""
echo "======================================="
echo "Authentication fix test completed!" 