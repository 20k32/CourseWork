using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelAgencyHRD
{
    public partial class LogInformationForm : Form
    {
        public ILoggable logAbility;
        public LogInformationForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            logAbility.DgCellClick(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            logAbility.DeleteOne();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            logAbility.DeleteAll();
        }
    }
}
