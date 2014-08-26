namespace JobWorker
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using JobWorker.Labs;

    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            Trace.TraceInformation("JobWorker has been started");

            return result;
        }

        public override void Run()
        {
            Trace.TraceInformation("JobWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var lab = new Lab1Worker();
                //// var lab = new Lab2Worker(); 
                //// var lab = new Lab3Worker(); 
                await lab.RunAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                Trace.TraceError(exp.ToString());
                throw;
            }
        }

        public override void OnStop()
        {
            Trace.TraceInformation("JobWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("JobWorker has stopped");
        }
    }
}
