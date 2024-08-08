namespace WebPanel.Misc.TaskTimer
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public static bool _isTaskExecuted;

        public SchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _isTaskExecuted = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int countExecute =0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var tempNow = DateTime.Now;
                var now = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                var scheduledTimeTemp = new DateTime(tempNow.Year, tempNow.Month, tempNow.Day, 21, 39, 0); // Задайте желаемое время
                var scheduledTime = scheduledTimeTemp.Hour + ":" + scheduledTimeTemp.Minute;

                if (_isTaskExecuted)
                {
                    countExecute++;
                    if (countExecute >= 3)
                    {
                        countExecute = 0;
                        _isTaskExecuted = false;
                    }
                }


                if (now == scheduledTime && !_isTaskExecuted)
                {
                    _isTaskExecuted = true;

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduledTasks = scope.ServiceProvider.GetRequiredService<ScheduledTask>();
                        scheduledTasks.RunTask();
                    }

                    // Дополнительная логика, если необходимо

                   // break; // Завершаем выполнение после вызова метода
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Проверяем каждые 30 секунд
            }
        }
    }
}
