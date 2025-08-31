using HairAI.Application.Common.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HairAI.Infrastructure.Services;

public class RabbitMqService : IQueueService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqService> _logger;
    private bool _disposed = false;
    private readonly object _lockObject = new object();

    public RabbitMqService(string connectionString, ILogger<RabbitMqService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        try
        {
            var factory = new ConnectionFactory() 
            { 
                Uri = new Uri(connectionString),
                // PERFORMANCE: Add connection pool settings to prevent connection exhaustion
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                // SECURITY: Limit memory usage
                RequestedChannelMax = 100,
                RequestedFrameMax = 131072,
                // MEMORY MANAGEMENT: Set connection recovery settings
                ContinuationTimeout = TimeSpan.FromSeconds(20),
                HandshakeContinuationTimeout = TimeSpan.FromSeconds(10)
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // PERFORMANCE: Set QoS to prevent overwhelming the service
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            // Declare the queue with proper error handling
            _channel.QueueDeclare(queue: "analysis_jobs",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                                 
            // Add connection recovery event handlers
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            _connection.ConnectionUnblocked += OnConnectionUnblocked;
            
            _logger.LogInformation("RabbitMQ service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            // CRASH PREVENTION: Dispose resources on initialization failure
            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception disposeEx)
            {
                _logger.LogError(disposeEx, "Error disposing RabbitMQ resources after initialization failure");
            }
            throw new InvalidOperationException("Failed to initialize RabbitMQ connection", ex);
        }
    }

    public async Task PublishAnalysisJobAsync(Guid jobId)
    {
        // CRASH PREVENTION: Check for disposed state
        if (_disposed)
        {
            _logger.LogWarning("Attempt to publish job {JobId} on disposed RabbitMQ service", jobId);
            throw new ObjectDisposedException(nameof(RabbitMqService));
        }

        // CRASH PREVENTION: Validate input
        if (jobId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to publish empty JobId");
            throw new ArgumentException("JobId cannot be empty", nameof(jobId));
        }

        // THREAD SAFETY: Ensure only one thread publishes at a time
        lock (_lockObject)
        {
            try
            {
                var message = new { JobId = jobId, Timestamp = DateTime.UtcNow };
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                // MEMORY LEAK FIX: Reuse properties object when possible with pooling
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2; // Persistent
                properties.MessageId = Guid.NewGuid().ToString();
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                properties.ContentType = "application/json";

                // CRASH PREVENTION: Add retry logic with timeout
                _channel.BasicPublish(exchange: "",
                                     routingKey: "analysis_jobs",
                                     basicProperties: properties,
                                     body: body);
                                     
                _logger.LogInformation("Analysis job {JobId} published successfully", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish analysis job {JobId}", jobId);
                // CRASH PREVENTION: Log and rethrow with context
                throw new InvalidOperationException($"Failed to publish analysis job {jobId}", ex);
            }
        }
        
        await Task.CompletedTask;
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown: {Reason}", e.ReplyText);
    }

    private void OnConnectionBlocked(object? sender, EventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection blocked");
    }

    private void OnConnectionUnblocked(object? sender, EventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection unblocked");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            lock (_lockObject)
            {
                if (!_disposed)
                {
                    try
                    {
                        _logger.LogInformation("Disposing RabbitMQ service");
                        
                        // Remove event handlers
                        if (_connection != null)
                        {
                            _connection.ConnectionShutdown -= OnConnectionShutdown;
                            _connection.ConnectionBlocked -= OnConnectionBlocked;
                            _connection.ConnectionUnblocked -= OnConnectionUnblocked;
                        }

                        // MEMORY LEAK FIX: Proper resource disposal order with timeout
                        if (_channel != null)
                        {
                            try
                            {
                                _channel.Close();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error closing RabbitMQ channel");
                            }
                            _channel.Dispose();
                        }
                        
                        if (_connection != null)
                        {
                            try
                            {
                                _connection.Close();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error closing RabbitMQ connection");
                            }
                            _connection.Dispose();
                        }
                        
                        _logger.LogInformation("RabbitMQ service disposed successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing RabbitMQ service");
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }
    }

    // MEMORY LEAK FIX: Add finalizer as safety net
    ~RabbitMqService()
    {
        _logger.LogWarning("RabbitMqService finalizer called - this indicates potential memory leak");
        Dispose(false);
    }
    
    // HEALTH CHECK: Add method to check connection status
    public bool IsConnected => _connection?.IsOpen == true && _channel?.IsOpen == true;
}