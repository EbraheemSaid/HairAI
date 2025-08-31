#!/bin/bash

# Test script for bootstrap-admin endpoint
BASE_URL="http://localhost:5000/api"

echo "🔧 Testing HairAI API Bootstrap Admin Endpoint"
echo "============================================="

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

# Test bootstrap-admin endpoint
echo ""
echo "2. Testing bootstrap-admin endpoint..."
bootstrap_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    "$BASE_URL/auth/bootstrap-admin")

if echo "$bootstrap_response" | grep -q '"success":true'; then
    echo "✅ Bootstrap admin successful"
    echo "Response: $bootstrap_response"
    
    # Extract admin details
    email=$(echo "$bootstrap_response" | grep -o '"email":"[^"]*"' | cut -d'"' -f4)
    user_id=$(echo "$bootstrap_response" | grep -o '"userId":"[^"]*"' | cut -d'"' -f4)
    default_password=$(echo "$bootstrap_response" | grep -o '"defaultPassword":"[^"]*"' | cut -d'"' -f4)
    
    echo ""
    echo "Admin User Details:"
    echo "  Email: $email"
    echo "  User ID: $user_id"
    echo "  Default Password: $default_password"
    echo ""
    echo "🎉 You can now login with these credentials!"
else
    echo "❌ Bootstrap admin failed"
    echo "Response: $bootstrap_response"
fi

echo ""
echo "============================================="
echo "Bootstrap admin test completed!"
