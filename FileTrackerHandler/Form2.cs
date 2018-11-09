using System;
using System.Windows.Forms;

namespace FileTrackerHandler
{
    public partial class Form2 : Form
    {
        private int _n;
        private DemoClient _dct;
        private string _url;
        private readonly int _ppid;
        public Form2(String dir,int tid)
        {
            InitializeComponent();
            this.Text = dir;
            _url = dir;
            _ppid = tid;
        }

        private void _mtimerClientReadFromPipe_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_dct._mFromServerQueue.Count > 0)
                {
                    if (listView1.InvokeRequired)
                    {
                        listView1.Invoke((MethodInvoker)delegate()
                        {
                            var ot = _dct._mFromServerQueue.Dequeue();
                            var item = new ListViewItem(ot);
                            item.SubItems.Add(_n.ToString());
                            item.SubItems.Add(DateTime.Now.ToLongTimeString());
                            _n++;
                            listView1.Items.Add(item);
                            listView1.EnsureVisible(listView1.Items.Count - 1);
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                _mtimerClientReadFromPipe.Enabled = false;
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dct.StopClient();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            _dct = new DemoClient("qursaan"+_ppid);
            _dct.StartClient();
            if (_dct.IsClientStarted())
            {
                tsss.Text = @"Active...@qursaan"+_ppid;
            }
        }

    }
}
