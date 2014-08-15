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

            ////var clientLogic = new Lab1();
            ////var clientLogic = new Lab2();
            var clientLogic = new Lab3();

            clientLogic.Run(name);
        }
    }
}
