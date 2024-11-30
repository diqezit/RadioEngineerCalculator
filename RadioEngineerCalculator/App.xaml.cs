using System;
using System.IO;
using System.Windows;
using Serilog;

namespace RadioEngineerCalculator
{
    public partial class App : Application
    {
        private const string LogFileName = "logs/latest.txt";

        public App()
        {
            ConfigureLogging();
        }

        /// <summary>
        /// Настройка логгера Serilog.
        /// </summary>
        private void ConfigureLogging()
        {
            try
            {
                // Убедимся, что директория для логов существует
                var logDirectory = Path.GetDirectoryName(LogFileName);
                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Настройка Serilog для записи логов только в файл
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug() // Уровень логирования
                    .WriteTo.File(LogFileName, rollingInterval: RollingInterval.Infinite) // Лог в файл
                    .CreateLogger();

                Log.Information("Логгер успешно настроен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка настройки логгирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Действия при запуске приложения.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Log.Information("Приложение успешно стартовало.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Фатальная ошибка при запуске приложения.");
            }
        }

        /// <summary>
        /// Действия при завершении приложения.
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                Log.Information("Приложение завершено.");
            }
            finally
            {
                Log.CloseAndFlush(); // Завершаем логгер
            }
        }
    }
}
