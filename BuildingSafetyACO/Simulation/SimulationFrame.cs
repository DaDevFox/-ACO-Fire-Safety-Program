using BuildingSafetyACO.Data;

namespace BuildingSafetyACO.Simulation
{
    public enum State
    {
        Initializing,
        Active
    }

    public class SimulationFrame
    {
        /// <summary>
        /// Shouldn't be changed while active
        /// </summary>
        public string planID { get; set; }

        public Database database = new Database();

        
        public void Initialize()
        {
            database.Initialize();


        }





    }
}