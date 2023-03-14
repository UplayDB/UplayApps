using System.ComponentModel;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public dllTest dllTest = null;
        public Form1(dllTest _dllTest)
        {
            dllTest = _dllTest;
            InitializeComponent();
            backgroundWorker1.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dllTest.Init(label1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dllTest.GetAchImg();
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
            dllTest.GetAchList();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dllTest.RefreshProductList();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            dllTest.Update();
            Thread.Sleep(10);
            backgroundWorker1.ReportProgress(0);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(e);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }
            dllTest.TestErrors(int.Parse(textBox1.Text));
        }
    }
}