using System;
using System.Threading;
using System.Windows.Forms;

namespace FileTrackerHandler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            

        }

        private DemoClient _dc;

        private void bttnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtURL.Text = folderDialog.SelectedPath;
                }
            }
        }

        private int _currentWindow = 0;

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            
            _dc.Client.Write("NEW");
            _dc.Client.Write(_currentWindow.ToString());
            _dc.Client.Write(txtURL.Text);
            var f2 = new Form2(txtURL.Text,_currentWindow);
            var t = new Thread(() => f2.ShowDialog()) {IsBackground = true};
            t.Start();
            _currentWindow++;
            this.txtURL.Text = "";
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            this.btnStart.Enabled = (txtURL.Text != String.Empty);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _dc = new DemoClient("qursaan");
            _dc.StartClient();
            if (_dc.IsClientStarted())
            {
                tsss.Text = @"Active...";
            }
        }


        private void _mtimerClientReadFromPipe_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_dc._mFromServerQueue.Count <= 0) return;
                var ot = _dc._mFromServerQueue.Dequeue();
                tsss.Text = ot;
            }
            catch (Exception ex)
            {
                _mtimerClientReadFromPipe.Enabled = false;
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dc.StopClient();
            Environment.Exit(Environment.ExitCode);

        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            _dc.StopClient();
            _dc.StartClient();
            if (_dc.IsClientStarted())
            {
                tsss.Text = @"Active...";
            }
        }
    }
}
