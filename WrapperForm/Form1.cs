namespace TestForm
{
    public partial class Form1 : Form
    {
        public dllTest dllTest = null;
        public Form1(dllTest _dllTest)
        {
            dllTest = _dllTest;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dllTest.Init(label1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dllTest.Update();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            dllTest.Shutdown();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dllTest.ShowOverlayForSection(UplayWrapper.Enums.UPC_OverlaySection.UPC_OverlaySection_Home);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dllTest.showbrowser();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dllTest.RefreshProductList();
        }
    }
}