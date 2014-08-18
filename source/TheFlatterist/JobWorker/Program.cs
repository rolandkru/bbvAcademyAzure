namespace JobWorker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WorkerRole workerRole = new WorkerRole();
            workerRole.Run();
        }
    }
}