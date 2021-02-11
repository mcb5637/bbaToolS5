using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5xTool
{
    public partial class FilePeek : Form
    {
        internal FilePeek()
        {
            InitializeComponent();
        }

        private string data;

        internal void ShowFilePeek(string s)
        {
            s = s.Replace("\n", "\r\n");
            if (TB_Data == null)
            {
                data = s;
            }
            else
            {
                TB_Data.Text = s;
                data = null;
            }
            ShowDialog();
        }

        private void FilePeek_Shown(object sender, EventArgs e)
        {
            if (data != null)
                TB_Data.Text = data;
        }
    }
}
