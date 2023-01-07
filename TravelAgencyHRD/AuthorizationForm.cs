using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TravelAgencyHRD
{
    public partial class AuthorizationForm : Form
    {
        public string UserStatus, UserName;
        private ApplicationDbContext appDbContext;
        public ApplicationDbContext AppDbContextToUse;

        public AuthorizationForm()
        {
            InitializeComponent();
        }
        private async Task RegisterAsync()
        {
            LoginButton.Click -= LoginButton_Click;
            RegisterButton.Click -= RegisterButton_Click;
            RegisterButton.Text = "";
            RegisterButton.Image = Image.FromFile($@"Pictures\SytemPictures\loading2.gif");
            await Task.Run(() =>
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Input data was incorrect");
                    return;
                }
                var tempUser = appDbContext.LogJournal.SingleOrDefault(p => p.Initials == textBox1.Text);
                if (tempUser == null)
                {
                    UserStatus = "user";
                    UserName = textBox1.Text;
                    var LogJournal1 = new LogJournal() { Initials = UserName, Password = textBox2.Text, Role = UserStatus };
                    new TableLogJournal(appDbContext).Add(textBox1.Text, textBox2.Text, UserStatus);
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
                else
                {
                    MessageBox.Show("There is such user. Try to log in.");
                }
            });
            RegisterButton.Text = "Register";
            RegisterButton.Image = null;
            LoginButton.Click += LoginButton_Click;
            RegisterButton.Click += RegisterButton_Click;
        }
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterAsync();
        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {
            appDbContext = AppDbContextToUse;
        }

        private async Task LoginAsync()
        {
            LoginButton.Text = "";
            LoginButton.Image = Image.FromFile($@"Pictures\SytemPictures\loading1.gif");
            LoginButton.Click -= LoginButton_Click;
            RegisterButton.Click -= RegisterButton_Click;
            await Task.Run(() =>
            {
                var user = appDbContext.LogJournal.SingleOrDefault(x => x.Initials == textBox1.Text && x.Password == textBox2.Text);
                if(textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Input data was incorrect");
                    return;
                }
                if (user != null)
                {
                    UserStatus = user.Role;
                    UserName = user.Initials;
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
                else
                {
                    MessageBox.Show("There is no such user in system. Please, try to rewrite password or user name.");
                }
            });
            LoginButton.Text = "Login";
            LoginButton.Image = null;
            LoginButton.Click += LoginButton_Click;
            RegisterButton.Click += RegisterButton_Click;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginAsync();
        }
    }
}