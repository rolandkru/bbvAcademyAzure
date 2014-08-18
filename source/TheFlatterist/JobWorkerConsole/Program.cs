namespace JobWorkerConsole
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using JobWorker.Labs;

    internal class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private static readonly ManualResetEvent RunCompleteEvent = new ManualResetEvent(false);

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            try
            {
                RunAsync(CancellationTokenSource.Token).Wait();
            }
            finally
            {
                RunCompleteEvent.Set();
            }
        }

        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var lab = new Lab1();
                await lab.RunAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                Trace.TraceError(exp.ToString());
                throw;
            }
        }
    }
}