using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;

namespace TravelAgencyHRD
{
    public partial class MainForm : Form
    {
        private TablePerson person;
        private TablePersonsEducation personsEducation;
        private TablePost post;
        private TableCabinet cabinet;
        private TableSeats seats;
        private TableAppointments appointments;
        private TableLogJournal logJournal;
        private Table tempTable;
        private ApplicationDbContext appDbContext;
        private SubForm subForm;
        private AuthorizationForm authorizationForm;
        private Font LabelFont;
        private Font LabelFontOnHover;
        private TablePersonsInformation personsInformation;
        private TableSeatsInformation seatsInformation;
        private TableAppointmentsInformation appointmentsInformation;
        private LogInformationForm logForm;
        private ToolStripMenuItem[] menuItems;

        public MainForm()
        {
            InitializeComponent();
            Table.Lables = new[] { label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13, label14, label15, label16, label17, label18, label19, label20, label21, label22, label23 };
            Table.Textboxes = new[] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18 };
            Table.Buttons = new[] { button5, button6, button7, button8, button2 };
            Table.ComboBoxes = new[] { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5 };
            Table.currentDG = dataGridView1;
            menuItems = new[] { PersonMenuItem, PersonsEducationMenuItem, PostMenuItem, CabinetMenuItem, SeatsMenuItem, AppointmentsMenuItem, LogJournalMenuItem, InformationMenuItem };
        }
        private void MakeVisibleButtons(bool visibility)
        {
            SearchButton.Visible = visibility;
            ResetButton.Visible = visibility;
            FilterButton.Visible = visibility;
        }
        private void MakeVisibleMenuItems(ToolStripMenuItem[] items, bool visibility, params int[] numbers)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Visible = !visibility;
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (i == numbers[j] - 1)
                    {
                        items[i].Visible = visibility;
                    }
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            appDbContext = new ApplicationDbContext();
            subForm = new SubForm();
            appDbContext.GenerateValues();
            LoadUser();
            LabelFont = new Font(label1.Font, FontStyle.Regular);
            LabelFontOnHover = new Font(label1.Font, FontStyle.Underline);
        }
        private void LoadUser()
        {
            authorizationForm = new AuthorizationForm() { AppDbContextToUse = appDbContext };
            authorizationForm.ShowDialog();
            label1.Text = $"{authorizationForm.UserStatus}: {authorizationForm.UserName}";
            switch (authorizationForm.UserStatus)
            {
                case "admin":
                    {
                        pictureBox1.BackgroundImage = Image.FromFile($@"Pictures\SytemPictures\a.png");
                        CRUDpanel.Visible = true;
                        MakeVisibleMenuItems(menuItems, true, 7);
                        logJournal = new TableLogJournal(appDbContext);
                        LogJournalMenuItem_Click(null, null);
                    }
                    break;
                case "user":
                    {
                        pictureBox1.BackgroundImage = Image.FromFile($@"Pictures\SytemPictures\u.png");
                        CRUDpanel.Visible = false;
                        SearchPanel.Visible = true;
                        MakeVisibleMenuItems(menuItems, true, 1, 2, 3, 4);
                        person = new TablePerson(appDbContext, subForm, subForm.dataGridView1, personsInformation);
                        personsEducation = new TablePersonsEducation(appDbContext);
                        post = new TablePost(appDbContext);
                        cabinet = new TableCabinet(appDbContext);
                        PersonMenuItem_Click(null, null);
                    }
                    break;
                case "worker":
                    {
                        pictureBox1.BackgroundImage = Image.FromFile($@"Pictures\SytemPictures\w.png");
                        CRUDpanel.Visible = true;
                        SearchPanel.Visible = true;
                        MakeVisibleMenuItems(menuItems, true, 1, 2, 3, 4, 5, 6, 8);
                        logForm = new LogInformationForm();
                        seatsInformation = new TableSeatsInformation(appDbContext, logForm, logForm.dataGridView1);
                        appointmentsInformation = new TableAppointmentsInformation(appDbContext, logForm, logForm.dataGridView1);
                        personsInformation = new TablePersonsInformation(appDbContext, logForm, logForm.dataGridView1);
                        person = new TablePerson(appDbContext, subForm, subForm.dataGridView1, personsInformation);
                        personsEducation = new TablePersonsEducation(appDbContext);
                        post = new TablePost(appDbContext);
                        cabinet = new TableCabinet(appDbContext);
                        seats = new TableSeats(appDbContext, subForm, subForm.dataGridView1, seatsInformation);
                        appointments = new TableAppointments(appDbContext, subForm, subForm.dataGridView1, appointmentsInformation);
                        SeatsMenuItem_Click(null, null);
                    }
                    break;
                default:
                    {
                        pictureBox1.BackgroundImage = Image.FromFile($@"Pictures\SytemPictures\un.png");
                        string[] info = new string[] { "You canceled the authorization, please go through it to the end.", "If you don't have an account, try to register new one.", "If your access level doesn't match your position, ask the administrator to change your status.", "You also can log in as a user with username 'user' and password 'user' in any time." };
                        dataGridView1.DataSource = info.Select(x => new {Information = x} ).ToList();
                        CRUDpanel.Visible = false;
                        SearchPanel.Visible = false;
                        MakeVisibleMenuItems(menuItems, false, 1, 2, 3, 4, 5, 6, 7, 8);
                        MakeVisibleButtons(false);
                        label1.Text = "Undefined: not a user";
                    }
                    break;
            }

        }
        private void PersonMenuItem_Click(object sender, EventArgs e)
        {
            person.MenuItem_Click();
            tempTable = person;
            MakeVisibleButtons(true);
        }
        private void PersonsEducationMenuItem_Click(object sender, EventArgs e)
        {
            personsEducation.MenuItem_Click();
            tempTable = personsEducation;
            MakeVisibleButtons(true);
        }
        private void PostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            post.MenuItem_Click();
            tempTable = post;
            MakeVisibleButtons(true);
        }
        private void CabinetMenuItem_Click(object sender, EventArgs e)
        {
            cabinet.MenuItem_Click();
            tempTable = cabinet;
            MakeVisibleButtons(true);
        }
        private void SeatsMenuItem_Click(object sender, EventArgs e)
        {
            seats.MenuItem_Click();
            tempTable = seats;
            MakeVisibleButtons(true);
        }
        private void AppointmentsMenuItem_Click(object sender, EventArgs e)
        {
            appointments.MenuItem_Click();
            tempTable = appointments;
            MakeVisibleButtons(true);
        }
        private void LogJournalMenuItem_Click(object sender, EventArgs e)
        {
            logJournal.MenuItem_Click();
            tempTable = logJournal;
            MakeVisibleButtons(false);
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tempTable?.DataGridCellClick(sender, e);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            tempTable.Add();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            tempTable.Remove();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            tempTable.Update();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            subForm.lookupField = (ILookupField)tempTable;
            tempTable.FormShow(tempTable == seats ? post : person);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            subForm.lookupField = (ILookupField)tempTable;
            tempTable.FormShow(tempTable == person ? person : cabinet);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            subForm.lookupField = (ILookupField)tempTable;
            tempTable.FormShow(seats);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            LoadUser();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            tempTable.Search();
        }
        private void FilterButton_Click(object sender, EventArgs e)
        {
            tempTable.Filter();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            subForm.lookupField = (ILookupField)tempTable;
            tempTable.ShowFormFromFilter(tempTable == person ? person : tempTable == seats ? cabinet : seats);
        }
        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.Font = LabelFontOnHover;
            label1.ForeColor = Color.Blue;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.Font = LabelFont;
            label1.ForeColor = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            subForm.lookupField = (ILookupField)tempTable;
            tempTable.ShowFormFromFilter(tempTable == seats ? post : person);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            tempTable.MenuItem_Click();
        }

        private void PersonsInformationMenuItem_Click(object sender, EventArgs e)
        {
            logForm.logAbility = personsInformation;
            personsInformation.MenuItem_Click();
        }

        private void SeatsInformationMenuItem_Click(object sender, EventArgs e)
        {
            logForm.logAbility = seatsInformation;
            seatsInformation.MenuItem_Click();
        }

        private void AppointmentsInformationMenuItem_Click(object sender, EventArgs e)
        {
            logForm.logAbility = appointmentsInformation;
            appointmentsInformation.MenuItem_Click();
        }
    }
}
