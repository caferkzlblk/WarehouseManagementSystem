using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WarehouseManagementSystem.Data;

public class OrderStatusUpdaterService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OrderStatusUpdaterService> _logger;

    public OrderStatusUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<OrderStatusUpdaterService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Servis devre dışı bırakıldı
        return Task.CompletedTask;
    }


   

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
