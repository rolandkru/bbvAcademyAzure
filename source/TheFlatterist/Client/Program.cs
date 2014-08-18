// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Client
{
    using System;

    using Client.Labs;
    using Client.Labs.Lab2;
    using Client.Labs.Lab3;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentNullException("Argument missing: [Name]");
            }

            var name = args[0];

            //// var clientLogic = new Lab1Client();
            //// var clientLogic = new Lab2Client();
            var clientLogic = new Lab3Client();

            clientLogic.RunAsync(name).Wait();
        }
    }
}
