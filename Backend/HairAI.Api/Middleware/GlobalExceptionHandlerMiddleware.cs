using FluentValidation;
using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace HairAI.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // SECURITY: Log full exception details but return sanitized response
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}", 
                context.TraceIdentifier, context.Request.Path, context.Request.Method);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, shouldExposeDetails) = GetErrorResponse(exception);
        context.Response.StatusCode = statusCode;

        var response = new
        {
            success = false,
            message = message,
            traceId = context.TraceIdentifier,
            errors = GetErrorDetails(exception, shouldExposeDetails)
        };

        try
        {
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
        catch (Exception serializationEx)
        {
            // CRASH PREVENTION: Fallback if JSON serialization fails
            _logger.LogError(serializationEx, "Failed to serialize error response");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("{\"success\":false,\"message\":\"Internal server error\"}");
        }
    }

    private (int StatusCode, string Message, bool ShouldExposeDetails) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException => (400, "Validation failed", true),
            ArgumentNullException => (400, "Required parameter is missing", _environment.IsDevelopment()),
            ArgumentException => (400, "Invalid request parameters", _environment.IsDevelopment()),
            UnauthorizedAccessException => (401, "Access denied", false),
            KeyNotFoundException => (404, "Resource not found", false),
            NotSupportedException => (405, "Operation not supported", false),
            TimeoutException => (408, "Request timeout", false),
            
            // Database-specific exceptions (must come before InvalidOperationException)
            NpgsqlException npgsqlEx when npgsqlEx.SqlState == "23505" => (409, "Resource already exists", false),
            NpgsqlException npgsqlEx when npgsqlEx.SqlState == "23503" => (409, "Referenced resource not found", false),
            NpgsqlException => (503, "Database service unavailable", false),
            
            // Invalid operation exceptions (must come after NpgsqlException)
            InvalidOperationException => (409, "Invalid operation", _environment.IsDevelopment()),
            
            // Memory-related exceptions
            OutOfMemoryException => (507, "Server memory limit exceeded", false),
            StackOverflowException => (500, "Server processing limit exceeded", false),
            
            // Task cancellation exceptions (TaskCanceledException inherits from OperationCanceledException)
            TaskCanceledException => (408, "Request was cancelled", false),
            OperationCanceledException => (408, "Operation was cancelled", false),
            
            _ => (500, "An internal server error occurred", _environment.IsDevelopment())
        };
    }

    private List<string> GetErrorDetails(Exception exception, bool shouldExposeDetails)
    {
        var errors = new List<string>();

        if (exception is ValidationException validationException)
        {
            // SECURITY: Safe to expose validation errors
            errors.AddRange(validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        }
        else if (shouldExposeDetails)
        {
            // SECURITY: Only expose details in development
            errors.Add(exception.Message);
            
            if (exception.InnerException != null)
            {
                errors.Add($"Inner: {exception.InnerException.Message}");
            }
        }
        else
        {
            // SECURITY: Generic error message for production
            errors.Add("An error occurred while processing your request");
        }

        return errors;
    }
}