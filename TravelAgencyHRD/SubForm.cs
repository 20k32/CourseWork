namespace TravelAgencyHRD
{
    public partial class SubForm : Form
    {
        public ILookupField lookupField;
        public SubForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lookupField.DgViewToUseCellClick(e);
        }
    }
}
