using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;


namespace MauiBlazorHybrid.Services
{
    public class SyncBackgroundService
    {
        private readonly GlobalSyncService _syncService;

        public SyncBackgroundService(GlobalSyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(30)); // cada 30s

            while (await timer.WaitForNextTickAsync(token))
            {
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _syncService.SyncAllAsync();
                }
            }
        }
    }
}
