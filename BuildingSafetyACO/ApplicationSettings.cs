namespace BuildingSafetyACO
{
    public enum ApplicationState
    {
        Preview,
        Simulation,
        EndStats
    }

    public class ApplicationSettings
    {
        public ApplicationState State { get; set; }

        public Controls controls = new Controls()
        {

        };

        






        public class Controls
        {
            public Keys SimulationRun = Keys.Space;
            public Keys SimulationHalt = Keys.Back;
            
            public MouseButtons UISelect = MouseButtons.Left;
            public MouseButtons UIContextMenu = MouseButtons.Right;

            public Keys UIBack = Keys.Escape; 
            
            public Keys UIUp = Keys.Up; 
            public Keys UIDown = Keys.Down;
            public Keys UILeft = Keys.Left;
            public Keys UIRight = Keys.Right;
        }
    }
}