namespace BuildingSafetyACO
{
    public partial class MainApplication : Form
    {
        public MainApplication()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplictionCore.Simulation.Activate();
        }
    }
}