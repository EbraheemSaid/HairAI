using HairAI.Application.Common.Interfaces;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HairAI.Infrastructure.Services;

public class PaymobService : IPaymentGateway, IDisposable
{
    private readonly string _apiKey;
    private readonly string _integrationId;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymobService> _logger;

    public PaymobService(string apiKey, string integrationId, ILogger<PaymobService> logger)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _integrationId = integrationId ?? throw new ArgumentNullException(nameof(integrationId));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = new HttpClient { BaseAddress = new Uri("https://accept.paymob.com/api/") };
        
        // Add default headers
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "HairAI-Paymob-Client/1.0");
    }

    public async Task<string> CreateCheckoutSessionAsync(decimal amount, string currency, string successUrl, string cancelUrl)
    {
        try
        {
            _logger.LogInformation("Creating Paymob checkout session for amount {Amount} {Currency}", amount, currency);
            
            // Step 1: Get authentication token
            var authToken = await GetAuthTokenAsync();
            
            // Step 2: Create order
            var orderId = await CreateOrderAsync(authToken, amount, currency);
            
            // Step 3: Get payment key
            var paymentKey = await GetPaymentKeyAsync(authToken, orderId, amount, currency);
            
            _logger.LogInformation("Paymob checkout session created successfully with order ID {OrderId}", orderId);
            return paymentKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Paymob checkout session for amount {Amount} {Currency}", amount, currency);
            throw new InvalidOperationException($"Failed to create Paymob checkout session: {ex.Message}", ex);
        }
    }

    public async Task<bool> VerifyPaymentAsync(string paymentIntentId)
    {
        try
        {
            _logger.LogInformation("Verifying Paymob payment with transaction ID {TransactionId}", paymentIntentId);
            
            var authToken = await GetAuthTokenAsync();
            
            var response = await _httpClient.GetAsync($"acceptance/transactions/{paymentIntentId}?token={authToken}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transaction = JsonSerializer.Deserialize<PaymobTransaction>(content);
                
                var isSuccess = transaction?.Success == true && transaction?.Pending == false;
                _logger.LogInformation("Paymob payment verification result for transaction {TransactionId}: {IsSuccess}", 
                    paymentIntentId, isSuccess);
                return isSuccess;
            }
            
            _logger.LogWarning("Paymob payment verification failed for transaction {TransactionId}. Status: {StatusCode}", 
                paymentIntentId, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Paymob payment verification for transaction {TransactionId}", paymentIntentId);
            return false;
        }
    }

    private async Task<string> GetAuthTokenAsync()
    {
        try
        {
            var authRequest = new { api_key = _apiKey };
            
            var response = await _httpClient.PostAsJsonAsync("auth/tokens", authRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Paymob auth token request failed. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to get auth token. Status: {response.StatusCode}");
            }
            
            var authResponse = await response.Content.ReadFromJsonAsync<PaymobAuthResponse>();
            
            if (string.IsNullOrEmpty(authResponse?.Token))
            {
                _logger.LogError("Paymob auth token response missing token");
                throw new InvalidOperationException("Failed to get auth token - token is missing in response");
            }
            
            _logger.LogDebug("Paymob auth token retrieved successfully");
            return authResponse.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Paymob auth token retrieval");
            throw new InvalidOperationException($"Failed to get auth token: {ex.Message}", ex);
        }
    }

    private async Task<int> CreateOrderAsync(string authToken, decimal amount, string currency)
    {
        try
        {
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(amount * 100), // Convert to cents
                currency = currency.ToUpper(),
                items = new object[] { }
            };
            
            var response = await _httpClient.PostAsJsonAsync("ecommerce/orders", orderRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Paymob order creation failed. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to create order. Status: {response.StatusCode}");
            }
            
            var orderResponse = await response.Content.ReadFromJsonAsync<PaymobOrderResponse>();
            
            if (orderResponse?.Id == null || orderResponse.Id <= 0)
            {
                _logger.LogError("Paymob order creation response missing order ID");
                throw new InvalidOperationException("Failed to create order - order ID is missing in response");
            }
            
            _logger.LogDebug("Paymob order created successfully with ID {OrderId}", orderResponse.Id);
            return orderResponse.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Paymob order creation");
            throw new InvalidOperationException($"Failed to create order: {ex.Message}", ex);
        }
    }

    private async Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string currency)
    {
        try
        {
            var paymentRequest = new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                expiration = 3600, // 1 hour
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = "clinic@hairai.com",
                    floor = "NA",
                    first_name = "Clinic",
                    street = "NA",
                    building = "NA",
                    phone_number = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "Admin",
                    state = "Cairo"
                },
                currency = currency.ToUpper(),
                integration_id = int.Parse(_integrationId)
            };
            
            var response = await _httpClient.PostAsJsonAsync("acceptance/payment_keys", paymentRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Paymob payment key request failed. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to get payment key. Status: {response.StatusCode}");
            }
            
            var paymentResponse = await response.Content.ReadFromJsonAsync<PaymobPaymentResponse>();
            
            if (string.IsNullOrEmpty(paymentResponse?.Token))
            {
                _logger.LogError("Paymob payment key response missing token");
                throw new InvalidOperationException("Failed to get payment key - token is missing in response");
            }
            
            _logger.LogDebug("Paymob payment key retrieved successfully");
            return paymentResponse.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Paymob payment key retrieval");
            throw new InvalidOperationException($"Failed to get payment key: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            _httpClient?.Dispose();
            _logger.LogInformation("PaymobService disposed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing PaymobService");
        }
    }

    // DTOs for Paymob API responses
    private class PaymobAuthResponse
    {
        public string? Token { get; set; }
    }

    private class PaymobOrderResponse
    {
        public int Id { get; set; }
    }

    private class PaymobPaymentResponse
    {
        public string? Token { get; set; }
    }

    private class PaymobTransaction
    {
        public bool Success { get; set; }
        public bool Pending { get; set; }
    }
}