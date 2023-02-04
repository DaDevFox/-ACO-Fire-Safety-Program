using BuildingSafetyACO.Simulation;
using System.Diagnostics;

namespace BuildingSafetyACO
{
    public static class ApplicaitonCore
    {
        public static SimulationFrame Simulation { get; set; } = new SimulationFrame();






        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainApplication());

            Debug.WriteLine("Starting");
            Simulation.Initialize();
        }
    }
}