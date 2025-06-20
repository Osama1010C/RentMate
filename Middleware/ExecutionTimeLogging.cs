namespace RentMateAPI.Middleware
{
    using System.Diagnostics;

    public class ExecutionTimeLogging
    {
        private readonly RequestDelegate _next;
        private readonly string _logFilePath = "Logs/execution_times.log";
        private readonly string _largeExecutionTime = "Logs/Exceed_execution_times.log";

        public ExecutionTimeLogging(RequestDelegate next)
        {
            _next = next;

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(_largeExecutionTime)!);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            var logEntry = $"{egyptTime:yyyy-MM-dd hh:mm:ss tt} \n {context.Request.Method} {context.Request.Path} \n {elapsedMs} ms\n\n";

            await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);

            if(elapsedMs > 1000) await File.AppendAllTextAsync(_largeExecutionTime, logEntry + Environment.NewLine);


        }
    }

}
