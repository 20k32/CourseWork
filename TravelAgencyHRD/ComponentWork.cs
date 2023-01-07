using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TravelAgencyHRD
{
    public interface ITableOperation
    {
        void Add();
        void Remove();
        void Update();
    }
    public interface ISelectable : ITableOperation
    {
        void Show();
        void FormShow(Table table);
    }
    public interface ILookupField
    {
        Form LookupForm { get; set; }
        DataGridView DgView { get; set; }
        void DgViewToUseCellClick(DataGridViewCellEventArgs e);
        string Id { get; set; }
        void FormShow(Table table);
    }
    public interface IFilterSearch
    {
        void ShowFormFromFilter(Table table);
        void Filter();
        void Search();
    }
    public interface ILoggable
    {
        DataGridView LogDgView { get; set; }
        Form LogForm { get; set; }
        public int IdToDelete { get; set; }
        void DgCellClick(DataGridViewCellEventArgs e);
        void DeleteOne();
        void DeleteAll();
    }

    public class Table : ISelectable, IFilterSearch
    {
        public static TextBox[] Textboxes;
        public static Label[] Lables;
        public static Button[] Buttons;
        public static ComboBox[] ComboBoxes;
        public static DataGridView currentDG;
        protected void MakeVisible(Control[] controls, int form, int to, bool visible)
        {
            int actualTo = to + 1;
            for (int i = form; i < actualTo; i++)
            {
                controls[i].Visible = visible;
                controls[i].Text = "";
            }
            if (actualTo != controls.Length)
                MakeVisible(controls, to, controls.Length - 1, !visible);
        }
        protected void MakeVisible(Control[] controls, bool visibility, params int[] numbers)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].Visible = !visibility;
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (i == numbers[j] - 1)
                    {
                        controls[i].Visible = visibility;
                    }
                }
            }
        }
        protected void SetupComboBox(ComboBox comboBox, params string[] strings)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(strings);
            comboBox.SelectedIndex = 0;
        }
        public ApplicationDbContext appDbContext { get; private set; }
        public Table(ApplicationDbContext context)
        {
            appDbContext = context;
        }
        public virtual void Search() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void Show() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void Filter() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void ShowFormFromFilter(Table table) { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void FormShow(Table table) { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void Add() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void Remove() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void Update() { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void DataGridCellClick(object sender, DataGridViewCellEventArgs e) { throw new NotImplementedException("To use this method you must override it in derived class"); }
        public virtual void MenuItem_Click() { throw new NotImplementedException("To use this method you must override it in derived class"); }

    }
    public class TablePerson : Table, ILookupField
    {
        public Form LookupForm { get; set; }
        public DataGridView DgView { get; set; }
        public string Id { get; set; }

        private TablePersonsInformation LogTable;
        public TablePerson(ApplicationDbContext context, Form LookUpFormToUse, DataGridView dataGridViewToUse, TablePersonsInformation logTable) : base(context)
        {
            LookupForm = LookUpFormToUse;
            DgView = dataGridViewToUse;
            LogTable = logTable;
        }
        private bool IsInTable()
        {
            try
            {
                var person = appDbContext.Person.Single(p => p.PhoneNumber == Textboxes[6].Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                var person = appDbContext.Person.Single(p => p.PhoneNumber == Textboxes[6].Text && p.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override void Add()
        {
            try
            {
                if (IsInTable() || new Regex("^[+]\\d{12}$").Matches(Textboxes[6].Text.Trim()).Count == 0)
                {
                    throw new Exception("The person's phone number must be unique.");
                }
                if (Textboxes[7].Text != "" && new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$").Matches(Textboxes[7].Text.Trim()).Count == 0)
                {
                    throw new Exception("The person's email is incorrect.");
                }
                var obj = appDbContext.Person.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    var education = appDbContext.PersonsEducation.Find(int.Parse(Textboxes[2].Text));
                    if (education == null) throw new Exception("There is no such education in list.");
                    appDbContext.Person.Add(new Person()
                    {
                        Initials = Textboxes[0].Text,
                        Nationality = Textboxes[1].Text,
                        Education = education,
                        Sex = char.Parse(Textboxes[3].Text),
                        DateOfBirth = DateTime.Parse(Textboxes[4].Text),
                        FamilyStatus = Textboxes[5].Text,
                        PhoneNumber = Textboxes[6].Text,
                        Email = Textboxes[7].Text,
                        IsPhotoAvaliable = bool.Parse(Textboxes[8].Text),
                        ChildrensCount = byte.Parse(Textboxes[9].Text)
                    });
                    LogTable.Add(Textboxes[0].Text, "Added", DateTime.Now, "none");
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var person = appDbContext.Person.Find(int.Parse(Textboxes[0].Text));
                if (person == null || person.IsDeleted == true)
                {
                    return;
                }
                LogTable.Add(person.Initials, "Fired", DateTime.Now, "none");
                appDbContext.Person.Remove(person);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var PersonQuery =
            appDbContext.Person
            .Select(person => new
            {
                Id = person.Id,
                Initials = person.Initials,
                Nationality = person.Nationality,
                Education = person.Education.Profession == null ? "NULL" : person.Education.Profession,
                Sex = person.Sex,
                DateOfBirth = person.DateOfBirth,
                FamilyStatus = person.FamilyStatus,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email,
                IsPhotoAvaliable = person.IsPhotoAvaliable,
                ChildrensCount = person.ChildrensCount
            });
            currentDG.DataSource = PersonQuery.ToList();
            appDbContext.Person.Load();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept() || new Regex("^[+]\\d{12}$").Matches(Textboxes[6].Text.Trim()).Count == 0)
                {
                    throw new Exception("The person's phone number must be unique.");
                }
                if (Textboxes[7].Text != "" && new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$").Matches(Textboxes[7].Text.Trim()).Count == 0)
                {
                    throw new Exception("The person's email is incorrect.");
                }
                var person = appDbContext.Person.Find(int.Parse(Lables[0].Text));
                var education = appDbContext.PersonsEducation.Find(int.Parse(Textboxes[2].Text));
                if (person != null && education != null)
                {
                    LogTable.Add(person.Initials, "Changed", DateTime.Now, Textboxes[0].Text);
                    person.Initials = Textboxes[0].Text;
                    person.Nationality = Textboxes[1].Text;
                    person.Education = education;
                    person.Sex = char.Parse(Textboxes[3].Text);
                    person.DateOfBirth = DateTime.Parse(Textboxes[4].Text);
                    person.FamilyStatus = Textboxes[5].Text;
                    person.PhoneNumber = Textboxes[6].Text;
                    person.Email = Textboxes[7].Text;
                    person.IsPhotoAvaliable = bool.Parse(Textboxes[8].Text);
                    person.ChildrensCount = byte.Parse(Textboxes[9].Text);
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Update(int Id)
        {
            try
            {
                var person = appDbContext.Person.Find(Id);
                var education = appDbContext.PersonsEducation.Find(int.Parse(Textboxes[2].Text));
                if (person != null && education != null)
                {
                    person.Initials = Textboxes[0].Text;
                    person.Nationality = Textboxes[1].Text;
                    person.Education = education;
                    person.Sex = char.Parse(Textboxes[3].Text);
                    person.DateOfBirth = DateTime.Parse(Textboxes[4].Text);
                    person.FamilyStatus = Textboxes[5].Text;
                    person.PhoneNumber = Textboxes[6].Text;
                    person.Email = Textboxes[7].Text;
                    person.IsPhotoAvaliable = bool.Parse(Textboxes[8].Text);
                    person.ChildrensCount = byte.Parse(Textboxes[9].Text);
                    person.IsDeleted = false;
                };
                LogTable.Add(Textboxes[0].Text, "Added", DateTime.Now, "none");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 21, true);
            MakeVisible(Textboxes, 0, 17, true);
            MakeVisible(ComboBoxes, 0, 4, true);
            MakeVisible(Buttons, true, 2, 4);
            SetupComboBox(ComboBoxes[0], "PhoneNumber", "Id");
            SetupComboBox(ComboBoxes[1], "Both", "Male", "Female");
            SetupComboBox(ComboBoxes[2], "Both", "Married", "Unmarried");
            SetupComboBox(ComboBoxes[3], "Both", "True", "False");
            SetupComboBox(ComboBoxes[4], "=", "<", ">", "<=", ">=");
            Lables[0].Text = "Id";
            Lables[1].Text = "Initials";
            Lables[2].Text = "Nationality";
            Lables[3].Text = "Education";
            Lables[4].Text = "Sex";
            Lables[5].Text = "DateOfBirth";
            Lables[6].Text = "FamilyStatus";
            Lables[7].Text = "PhoneNumber";
            Lables[8].Text = "Email";
            Lables[9].Text = "IsPhotoAvaliable";
            Lables[10].Text = "ChildrensCount";
            Lables[11].Text = Lables[1].Text;
            Lables[12].Text = Lables[2].Text;
            Lables[13].Text = Lables[3].Text;
            Lables[14].Text = Lables[4].Text;
            Lables[15].Text = Lables[5].Text;
            Lables[16].Text = "From";
            Lables[17].Text = "To";
            Lables[18].Text = Lables[6].Text;
            Lables[19].Text = Lables[8].Text;
            Lables[20].Text = Lables[9].Text;
            Lables[21].Text = Lables[10].Text;
        }
        public override void Filter()
        {
            try
            {
                var filterQuery =
                appDbContext.Person
                .Select(person => new
                {
                    Id = person.Id,
                    Initials = person.Initials,
                    Nationality = person.Nationality,
                    Education = person.Education.Profession == null ? "NULL" : person.Education.Profession,
                    Sex = person.Sex,
                    DateOfBirth = person.DateOfBirth,
                    FamilyStatus = person.FamilyStatus,
                    PhoneNumber = person.PhoneNumber,
                    Email = person.Email,
                    IsPhotoAvaliable = person.IsPhotoAvaliable,
                    ChildrensCount = person.ChildrensCount
                });
                if (Textboxes[11].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.Initials.Contains(Textboxes[11].Text));
                }
                if (Textboxes[12].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.Nationality.Contains(Textboxes[12].Text));
                }
                if (Textboxes[13].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.Education.Contains(Textboxes[13].Text));
                }
                if (Textboxes[14].Text != string.Empty && Textboxes[15].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.DateOfBirth > DateTime.Parse(Textboxes[14].Text) && person.DateOfBirth < DateTime.Parse(Textboxes[15].Text));
                }
                else if (Textboxes[14].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.DateOfBirth > DateTime.Parse(Textboxes[14].Text));
                }
                if (Textboxes[16].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(person => person.Email.Contains(Textboxes[16].Text));
                }
                if (Textboxes[17].Text != string.Empty)
                {
                    switch (ComboBoxes[4].SelectedItem)
                    {
                        case ">": filterQuery = filterQuery.Where(person => person.ChildrensCount > int.Parse(Textboxes[17].Text)); break;
                        case "<": filterQuery = filterQuery.Where(person => person.ChildrensCount < int.Parse(Textboxes[17].Text)); break;
                        case "=": filterQuery = filterQuery.Where(person => person.ChildrensCount == int.Parse(Textboxes[17].Text)); break;
                        case ">=": filterQuery = filterQuery.Where(person => person.ChildrensCount >= int.Parse(Textboxes[17].Text)); break;
                        case "<=": filterQuery = filterQuery.Where(person => person.ChildrensCount <= int.Parse(Textboxes[17].Text)); break;
                    }
                }
                switch (ComboBoxes[1].SelectedItem)
                {
                    case "Both": filterQuery = filterQuery; break;
                    case "Male": filterQuery = filterQuery.Where(person => person.Sex == 'M'); break;
                    case "Female": filterQuery = filterQuery.Where(person => person.Sex == 'F'); break;
                }
                switch (ComboBoxes[2].SelectedItem)
                {
                    case "Both": filterQuery = filterQuery; break;
                    case "Married": filterQuery = filterQuery.Where(person => person.FamilyStatus == "married"); break;
                    case "Unmarried": filterQuery = filterQuery.Where(person => person.FamilyStatus == "unmarried"); break;
                }
                switch (ComboBoxes[3].SelectedItem)
                {
                    case "Both": filterQuery = filterQuery; break;
                    case "True": filterQuery = filterQuery.Where(person => person.IsPhotoAvaliable == true); break;
                    case "False": filterQuery = filterQuery.Where(person => person.IsPhotoAvaliable == false); break;
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Search()
        {
            try
            {
                var filterQuery =
                    appDbContext.Person
                    .Select(person => new
                    {
                        Id = person.Id,
                        Initials = person.Initials,
                        Nationality = person.Nationality,
                        Education = person.Education.Profession == null ? "NULL" : person.Education.Profession,
                        Sex = person.Sex,
                        DateOfBirth = person.DateOfBirth,
                        FamilyStatus = person.FamilyStatus,
                        PhoneNumber = person.PhoneNumber,
                        Email = person.Email,
                        IsPhotoAvaliable = person.IsPhotoAvaliable,
                        ChildrensCount = person.ChildrensCount
                    });
                if (Textboxes[10].Text != "")
                {
                    switch (ComboBoxes[0].SelectedItem)
                    {
                        case "Id": filterQuery = filterQuery.Where(person => person.Id == int.Parse(Textboxes[10].Text)); break;
                        case "PhoneNumber": filterQuery = filterQuery.Where(person => person.PhoneNumber == Textboxes[10].Text); break;
                    }
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 10; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
            }
            catch { }
        }
        public override void FormShow(Table _)
        {
            DgView.DataSource = appDbContext.PersonsEducation.ToList();
            LookupForm.ShowDialog();
            if (LookupForm.DialogResult == DialogResult.OK)
            {
                string s = Id?.ToString();
                Textboxes[2].Text = s == null ? "NULL" : s;
            }
        }
        public override void ShowFormFromFilter(Table _)
        {
            DgView.DataSource = appDbContext.PersonsEducation.ToList();
            LookupForm.ShowDialog();
            if (LookupForm.DialogResult == DialogResult.OK)
            {
                string s = "";
                try
                {
                    s = appDbContext.PersonsEducation.Find(int.Parse(Id)).Profession;
                }
                catch { s = "NULL"; }
                Textboxes[13].Text = s;
            }
        }
        public void DgViewToUseCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = DgView.Rows[ind];
                Id = selectedRows.Cells[0].Value?.ToString();
            }
            catch { }
        }
    }
    public class TablePersonsEducation : Table
    {
        private bool IsInTable()
        {
            try
            {
                var person = appDbContext.PersonsEducation.Single(p => p.Profession == Textboxes[0].Text && p.ACDegree == Textboxes[1].Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                var person = appDbContext.PersonsEducation.Single(p => p.Profession == Textboxes[0].Text && p.ACDegree == Textboxes[1].Text && p.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public TablePersonsEducation(ApplicationDbContext context) : base(context) { }
        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The person's education profession and ACDegree must be unique.");
                }
                var obj = appDbContext.PersonsEducation.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    appDbContext.PersonsEducation.Add(new PersonsEducation()
                    {
                        Profession = Textboxes[0].Text,
                        LanguageSkills = Textboxes[2].Text,
                        ACDegree = Textboxes[1].Text
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var education = appDbContext.PersonsEducation.Find(int.Parse(Textboxes[0].Text));
                appDbContext.PersonsEducation.Remove(education);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var PersonsEducationQuery =
            appDbContext.PersonsEducation
            .Select(education => new
            {
                Id = education.Id,
                Profession = education.Profession,
                ACDegree = education.ACDegree,
                LanguageSkills = education.LanguageSkills,
            });
            currentDG.DataSource = PersonsEducationQuery.ToList();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept())
                {
                    throw new Exception("The person's education profession and ACDegree must be unique.");
                }
                var personsEducation = appDbContext.PersonsEducation.Find(int.Parse(Lables[0].Text));
                if (personsEducation != null)
                {
                    personsEducation.Profession = Textboxes[0].Text;
                    personsEducation.LanguageSkills = Textboxes[2].Text;
                    personsEducation.ACDegree = Textboxes[1].Text;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Update(int Id)
        {
            try
            {
                var personsEducation = appDbContext.PersonsEducation.Find(Id);
                if (personsEducation != null)
                {
                    personsEducation.Profession = Textboxes[0].Text;
                    personsEducation.LanguageSkills = Textboxes[2].Text;
                    personsEducation.ACDegree = Textboxes[1].Text;
                    personsEducation.IsDeleted = false;
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 4, true);
            MakeVisible(Lables, 11, 14, true);
            MakeVisible(Textboxes, 0, 3, true);
            MakeVisible(Textboxes, 10, 14, true);
            MakeVisible(ComboBoxes, 0, 1, true);
            MakeVisible(Buttons, false, 1, 2, 3, 4, 5);
            SetupComboBox(ComboBoxes[0], "Id");

            Lables[0].Text = "Id";
            Lables[1].Text = "Profession";
            Lables[3].Text = "LanguageSkills";
            Lables[2].Text = "ACDegree";
            Lables[11].Text = Lables[1].Text;
            Lables[12].Text = "LanguageSkills";
            Lables[13].Text = "ACDegree";
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 3; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
            }
            catch { }
        }
        public override void Filter()
        {
            try
            {
                var filterQuery =
                appDbContext.PersonsEducation
                .Select(education => new
                {
                    Id = education.Id,
                    Profession = education.Profession,
                    LanguageSkills = education.LanguageSkills,
                    ACDegree = education.ACDegree
                });
                if (Textboxes[11].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(education => education.Profession.Contains(Textboxes[11].Text));
                }
                if (Textboxes[12].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(education => education.LanguageSkills.Contains(Textboxes[12].Text));
                }
                if (Textboxes[13].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(education => education.ACDegree.Contains(Textboxes[13].Text));
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Search()
        {
            try
            {
                var filterQuery =
                 appDbContext.PersonsEducation
                 .Select(education => new
                 {
                     Id = education.Id,
                     Profession = education.Profession,
                     LanguageSkills = education.LanguageSkills,
                     ACDegree = education.ACDegree
                 });
                if (Textboxes[10].Text != "")
                {
                    filterQuery = filterQuery.Where(education => education.Id == int.Parse(Textboxes[10].Text));
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    public class TablePost : Table
    {
        public TablePost(ApplicationDbContext context) : base(context) { }
        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The name must be unique");
                }
                var obj = appDbContext.Post.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    appDbContext.Post.Add(new Post()
                    {
                        Name = Textboxes[0].Text
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
        }
        private void Update(int Id)
        {
            try
            {
                var post = appDbContext.Post.Find(Id);
                if (post != null)
                {
                    post.Name = Textboxes[0].Text;
                    post.IsDeleted = false;
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool IsInTable()
        {
            try
            {
                var post = appDbContext.Post.Single(p => p.Name == Textboxes[0].Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override void Remove()
        {
            try
            {
                var post = appDbContext.Post.Find(int.Parse(Textboxes[0].Text));
                appDbContext.Post.Remove(post);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var PostQuery =
                appDbContext.Post
                .Select(post => new
                {
                    Id = post.Id,
                    Name = post.Name
                });
            currentDG.DataSource = PostQuery.ToList();
        }
        public override void Update()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The name must be unique");
                }
                var post = appDbContext.Post.Find(int.Parse(Lables[0].Text));
                if (post != null)
                {
                    post.Name = Textboxes[0].Text;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 2, true);
            MakeVisible(Lables, 11, 12, true);
            MakeVisible(Textboxes, 0, 1, true);
            MakeVisible(Textboxes, 10, 12, true);
            MakeVisible(Buttons, false, 1, 2, 3, 4, 5);
            MakeVisible(ComboBoxes, 0, 1, true);
            SetupComboBox(ComboBoxes[0], "Name", "Id");
            Lables[0].Text = "Id";
            Lables[1].Text = "Name";
            Lables[11].Text = Lables[1].Text;
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                Textboxes[0].Text = selectedRows.Cells[1].Value?.ToString();
            }
            catch { }
        }
        public override void Filter()
        {
            try
            {
                var filterQuery =
                appDbContext.Post
                .Select(post => new
                {
                    Id = post.Id,
                    Name = post.Name
                });
                if (Textboxes[11].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(education => education.Name.Contains(Textboxes[11].Text));
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Search()
        {
            try
            {
                var filterQuery =
                 appDbContext.Post
                .Select(post => new
                {
                    Id = post.Id,
                    Name = post.Name
                });
                if (Textboxes[10].Text != "")
                {
                    switch (ComboBoxes[0].SelectedItem)
                    {
                        case "Name": filterQuery = filterQuery.Where(post => post.Name == Textboxes[10].Text); break;
                        case "Id": filterQuery = filterQuery.Where(post => post.Id == int.Parse(Textboxes[10].Text)); break;
                    }
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    public class TableCabinet : Table
    {
        public TableCabinet(ApplicationDbContext context) : base(context) { }
        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The cabinet name must be unique.");
                }
                var obj = appDbContext.Cabinet.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    int number = int.Parse(Textboxes[1].Text), floor = int.Parse(Textboxes[2].Text);
                    appDbContext.Cabinet.Add(new Cabinet()
                    {
                        Name = Textboxes[0].Text,
                        CabinetNumber = number <= 0 ? 1 : number,
                        Floor = floor < 0 ? 0 : floor
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool IsInTable()
        {
            try
            {
                var cabinet = appDbContext.Cabinet.Single(p => p.Name == Textboxes[0].Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                var cabinet = appDbContext.Cabinet.Single(p => p.Name == Textboxes[0].Text && p.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Update(int Id)
        {
            try
            {
                var cabinet = appDbContext.Cabinet.Find(Id);
                if (cabinet != null)
                {
                    int number = int.Parse(Textboxes[1].Text), floor = int.Parse(Textboxes[2].Text);
                    cabinet.Name = Textboxes[0].Text;
                    cabinet.CabinetNumber = number <= 0 ? 1 : number;
                    cabinet.Floor = floor < 0 ? 0 : floor;
                    cabinet.IsDeleted = false;
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var cabinet = appDbContext.Cabinet.Find(int.Parse(Textboxes[0].Text));
                appDbContext.Cabinet.Remove(cabinet);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var CabinetQuery =
                appDbContext.Cabinet
                .Select(cabinet => new
                {
                    Id = cabinet.Id,
                    Name = cabinet.Name,
                    CabinetNumber = cabinet.CabinetNumber,
                    Floor = cabinet.Floor
                });
            currentDG.DataSource = CabinetQuery.ToList();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept())
                {
                    throw new Exception("The cabinet name must be unique.");
                }
                var cabinet = appDbContext.Cabinet.Find(int.Parse(Lables[0].Text));
                if (cabinet != null)
                {
                    int number = int.Parse(Textboxes[1].Text), floor = int.Parse(Textboxes[2].Text);
                    cabinet.Name = Textboxes[0].Text;
                    cabinet.CabinetNumber = number <= 0 ? 1 : number;
                    cabinet.Floor = floor < 0 ? 0 : floor; ;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 4, true);
            MakeVisible(Lables, 11, 14, true);
            MakeVisible(Textboxes, 0, 3, true);
            MakeVisible(Textboxes, 10, 14, true);
            MakeVisible(Buttons, false, 1, 2, 3, 4, 5);
            MakeVisible(ComboBoxes, 0, 1, true);
            SetupComboBox(ComboBoxes[0], "Name", "Id");
            Lables[0].Text = "Id";
            Lables[1].Text = "Name";
            Lables[2].Text = "CabinetNumber";
            Lables[3].Text = "Floor";
            Lables[11].Text = Lables[1].Text;
            Lables[12].Text = Lables[2].Text;
            Lables[13].Text = Lables[3].Text;
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 3; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
            }
            catch { }
        }
        public override void Filter()
        {
            try
            {
                var filterQuery =
                appDbContext.Cabinet
                .Select(cabinet => new
                {
                    Id = cabinet.Id,
                    Name = cabinet.Name,
                    CabinetNumber = cabinet.CabinetNumber,
                    Floor = cabinet.Floor
                });
                if (Textboxes[11].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(cabinet => cabinet.Name.Contains(Textboxes[11].Text));
                }
                if (Textboxes[12].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(cabinet => cabinet.CabinetNumber == int.Parse(Textboxes[12].Text));
                }
                if (Textboxes[13].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(cabinet => cabinet.Floor == int.Parse(Textboxes[13].Text));
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Search()
        {
            try
            {
                var filterQuery =
                appDbContext.Cabinet
                .Select(cabinet => new
                {
                    Id = cabinet.Id,
                    Name = cabinet.Name,
                    CabinetNumber = cabinet.CabinetNumber,
                    Floor = cabinet.Floor
                });
                if (Textboxes[10].Text != "")
                {
                    switch (ComboBoxes[0].SelectedItem)
                    {
                        case "Name": filterQuery = filterQuery.Where(cabinet => cabinet.Name == Textboxes[10].Text); break;
                        case "Id": filterQuery = filterQuery.Where(cabinet => cabinet.Id == int.Parse(Textboxes[10].Text)); break;
                    }
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    public class TableSeats : Table, ILookupField
    {
        public Form LookupForm { get; set; }
        public DataGridView DgView { get; set; }
        public string Id { get; set; }

        private TableSeatsInformation LogTable;
        public TableSeats(ApplicationDbContext context, Form LookUpFormToUse, DataGridView dataGridViewToUse, TableSeatsInformation logTable) : base(context)
        {
            LookupForm = LookUpFormToUse;
            DgView = dataGridViewToUse;
            LogTable = logTable;
        }
        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The Post Id (Name) and Cabinet Id (Name) in Seats must be unique.");
                }
                var obj = appDbContext.Seats.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    var post = appDbContext.Post.Find(int.Parse(Textboxes[0].Text));
                    var cabinet = appDbContext.Cabinet.Find(int.Parse(Textboxes[2].Text));
                    decimal salary = decimal.Parse(Textboxes[3].Text);
                    if (post == null || cabinet == null) throw new Exception("There is no post or cabinet with this ID");
                    byte count = byte.Parse(Textboxes[1].Text);
                    appDbContext.Seats.Add(new Seats()
                    {
                        Post = post,
                        PostCount = (byte)(count >= 10 ? 9 : count),
                        Cabinet = cabinet,
                        Salary = salary <= 0 ? 0.1m : salary,
                    });
                    LogTable.Add(post.Name, cabinet.Name ,"Added", DateTime.Now, "none");

                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool IsInTable()
        {
            try
            {
                var cabinet = appDbContext.Seats.Single(p => p.Post.Id == int.Parse(Textboxes[0].Text) && p.CabinetId == int.Parse(Textboxes[2].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                var cabinet = appDbContext.Seats.Single(p => p.Post.Id == int.Parse(Textboxes[0].Text) && p.CabinetId == int.Parse(Textboxes[2].Text) && p.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Update(int Id)
        {
            try
            {
                var seat = appDbContext.Seats.Find(Id);
                var post = appDbContext.Post.Find(int.Parse(Textboxes[0].Text));
                var cabinet = appDbContext.Cabinet.Find(int.Parse(Textboxes[2].Text));
                decimal salary = decimal.Parse(Textboxes[3].Text);
                if (seat != null && post != null || cabinet != null)
                {
                    byte count = byte.Parse(Textboxes[1].Text);
                    seat.Post = post;
                    seat.PostCount = (byte)(count >= 10 ? 9 : count);
                    seat.Cabinet = cabinet;
                    seat.Salary = salary <= 0 ? 0.1m : salary;
                    seat.IsDeleted = false;
                    LogTable.Add(seat.Post.Name, seat.Cabinet.Name, "Added", DateTime.Now, "none");
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var seat = appDbContext.Seats.Find(int.Parse(Textboxes[0].Text));
                if (seat == null || seat.IsDeleted == true)
                {
                    return;
                }
                LogTable.Add(seat.PostId.ToString(), seat.CabinetId.ToString(), "Removed", DateTime.Now, "none");
                appDbContext.Seats.Remove(seat);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);}
        }
        public override void Show()
        {
            var postCabinetQueryable = appDbContext.Seats.Select(seat => new
            {
                Id = seat.Id,
                Post = seat.Post.Name == null ? "NULL" : seat.Post.Name,
                PostCount = seat.PostCount,
                Cabinet = seat.Cabinet.Name == null ? "NULL" : seat.Cabinet.Name,
                Salary = seat.Salary
            });
            currentDG.DataSource = postCabinetQueryable.ToList();
            appDbContext.Seats.Load();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept())
                {
                    throw new Exception("The Post Id (Name) and Cabinet Id (Name) in Seats must be unique.");
                }
                var seat = appDbContext.Seats.Find(int.Parse(Lables[0].Text));
                var post = appDbContext.Post.Find(int.Parse(Textboxes[0].Text));
                var cabinet = appDbContext.Cabinet.Find(int.Parse(Textboxes[2].Text));
                decimal salary = decimal.Parse(Textboxes[3].Text);
                if (seat != null && post != null && cabinet != null)
                {
                    byte count = byte.Parse(Textboxes[1].Text);
                    LogTable.Add(seat.PostId.ToString(), seat.CabinetId.ToString(), "Changed", DateTime.Now, post.Name + " " + cabinet.Name);
                    seat.Post = post;
                    seat.PostCount = (byte)(count >= 10 ? 9 : count);
                    seat.Cabinet = cabinet;
                    seat.Salary = salary <= 0 ? 0.1m : salary;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 5, true);
            MakeVisible(Lables, 11, 14, true);
            MakeVisible(Textboxes, 0, 4, true);
            MakeVisible(Textboxes, 10, 14, true);
            MakeVisible(Buttons, true, 1, 2, 4, 5);
            MakeVisible(ComboBoxes, true, 1, 5);
            Lables[21].Visible = true;
            Textboxes[17].Visible = true;
            SetupComboBox(ComboBoxes[0], "Post", "Id");
            SetupComboBox(ComboBoxes[4], "=", "<", ">", "<=", ">=");
            Lables[0].Text = "Id";
            Lables[1].Text = "Post";
            Lables[2].Text = "PostCount";
            Lables[3].Text = "Cabinet";
            Lables[4].Text = "Salary";
            Lables[11].Text = Lables[1].Text;
            Lables[12].Text = Lables[2].Text;
            Lables[13].Text = Lables[3].Text;
            Lables[21].Text = Lables[4].Text;
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 4; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
            }
            catch { }
        }
        public override void FormShow(Table table)
        {
            if (table is TablePost)
            {
                DgView.DataSource = appDbContext.Post.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = Id?.ToString();
                    Textboxes[0].Text = s == null ? "NULL" : s;
                }
            }
            else if (table is TableCabinet)
            {
                DgView.DataSource = appDbContext.Cabinet.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = Id?.ToString();
                    Textboxes[2].Text = s == null ? "NULL" : s;
                }
            }
        }
        public void DgViewToUseCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = DgView.Rows[ind];
                Id = selectedRows.Cells[0].Value?.ToString();
            }
            catch { }
        }
        public override void Filter()
        {
            try
            {
                var filterQuery = appDbContext.Seats.Select(seat => new
                {
                    Id = seat.Id,
                    Post = seat.Post.Name == null ? "NULL" : seat.Post.Name,
                    PostCount = seat.PostCount,
                    Cabinet = seat.Cabinet.Name == null ? "NULL" : seat.Cabinet.Name,
                    Salary = seat.Salary
                });
                if (Textboxes[11].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(seat => seat.Post.Contains(Textboxes[11].Text));
                }
                if (Textboxes[12].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(seat => seat.PostCount == int.Parse(Textboxes[12].Text));
                }
                if (Textboxes[13].Text != string.Empty)
                {
                    filterQuery = filterQuery.Where(seat => seat.Cabinet.Contains(Textboxes[13].Text));
                }
                if (Textboxes[17].Text != string.Empty)
                {
                    decimal inputSalary = decimal.Parse(Textboxes[17].Text);
                    switch (ComboBoxes[4].SelectedItem)
                    {
                        case ">": filterQuery = filterQuery.Where(seat => seat.Salary > inputSalary); break;
                        case "<": filterQuery = filterQuery.Where(seat => seat.Salary < inputSalary); break;
                        case "=": filterQuery = filterQuery.Where(seat => seat.Salary == inputSalary); break;
                        case ">=": filterQuery = filterQuery.Where(seat => seat.Salary >= inputSalary); break;
                        case "<=": filterQuery = filterQuery.Where(seat => seat.Salary <= inputSalary); break;
                    }
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void ShowFormFromFilter(Table table)
        {
            if (table is TablePost)
            {
                DgView.DataSource = appDbContext.Post.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = "";
                    try
                    {
                         s = appDbContext.Post.Find(int.Parse(Id)).Name;
                    }
                    catch 
                    {
                        s = "NULL";
                    }
                    Textboxes[11].Text = s;
                }
            }
            else if (table is TableCabinet)
            {
                DgView.DataSource = appDbContext.Cabinet.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = "";
                    try
                    {
                        s = appDbContext.Cabinet.Find(int.Parse(Id)).Name;
                    }
                    catch
                    {
                        s = "NULL";
                    }
                    Textboxes[13].Text = s;
                }
            }
        }
        public override void Search()
        {
            try
            {
                var filterQuery =
                appDbContext.Seats
                .Select(seat => new
                {
                    Id = seat.Id,
                    Post = seat.Post.Name == null ? "NULL" : seat.Post.Name,
                    PostCount = seat.PostCount,
                    Cabinet = seat.Cabinet.Name == null ? "NULL" : seat.Cabinet.Name,
                    Salary = seat.Salary
                });
                if (Textboxes[10].Text != "")
                {
                    switch (ComboBoxes[0].SelectedItem)
                    {
                        case "Post": filterQuery = filterQuery.Where(seat => seat.Post == Textboxes[10].Text); break;
                        case "Id": filterQuery = filterQuery.Where(seat => seat.Id == int.Parse(Textboxes[10].Text)); break;
                    }
                    
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    public class TableAppointments : Table, ILookupField
    {
        public Form LookupForm { get; set; }
        public DataGridView DgView { get; set; }
        public string Id { get; set; }
        private TableAppointmentsInformation LogTable;
        public TableAppointments(ApplicationDbContext context, Form LookUpFormToUse, DataGridView dataGridViewToUse, TableAppointmentsInformation logTable) : base(context)
        {
            LookupForm = LookUpFormToUse;
            DgView = dataGridViewToUse;
            LogTable = logTable;
        }
        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The profession of the same person must be unique.");
                }
                var obj = appDbContext.Appointments.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    var person = appDbContext.Person.Find(int.Parse(Textboxes[0].Text));
                    var seat = appDbContext.Seats.Find(int.Parse(Textboxes[4].Text));
                    decimal stake = decimal.Parse(Textboxes[1].Text);
                    if (person == null || seat == null) throw new Exception("There is no Person or Seat with this ID");
                    DateTime? deldate = null;
                    if (Textboxes[3].Text != "")
                        deldate = DateTime.Parse(Textboxes[3].Text);
                    appDbContext.Appointments.Add(new Appointments()
                    {
                        Person = person,
                        Stake = stake <= 0 ? 0.1m : stake,
                        InvokationDate = DateTime.Parse(Textboxes[2].Text),
                        DeletionDate = deldate,
                        DismissalReason = Textboxes[5]?.Text,
                        Seat = seat
                    });
                    LogTable.Add(person.Initials, Textboxes[1].Text, "Added", DateTime.Now, "none");
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool IsInTable()
        {
            try
            {
                appDbContext.Appointments.Single(sp => sp.Person.Id == int.Parse(Textboxes[0].Text) && sp.Seat.Id == int.Parse(Textboxes[4].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                var cabinet = appDbContext.Appointments.Single(sp => sp.Person.Id == int.Parse(Textboxes[0].Text) && sp.Seat.Id == int.Parse(Textboxes[4].Text) && sp.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Update(int Id)
        {
            try
            {
                var appointments = appDbContext.Appointments.Find(Id);
                var person = appDbContext.Person.Find(int.Parse(Textboxes[0].Text));
                var seat = appDbContext.Seats.Find(int.Parse(Textboxes[4].Text));
                decimal stake = decimal.Parse(Textboxes[1].Text);
                DateTime? deldate = null;
                if (Textboxes[3].Text != "")
                    deldate = DateTime.Parse(Textboxes[3].Text);
                if (appointments != null || person != null || seat != null)
                {
                    LogTable.Add(person.Initials, Textboxes[1].Text, "Added", DateTime.Now, "none");
                    appointments.Person = person;
                    appointments.Stake = stake <= 0 ? 0.1m : stake;
                    appointments.InvokationDate = DateTime.Parse(Textboxes[2].Text);
                    appointments.DeletionDate = deldate;
                    appointments.DismissalReason = Textboxes[5].Text;
                    appointments.Seat = seat;
                    appointments.IsDeleted = false;
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var appointments = appDbContext.Appointments.Find(int.Parse(Textboxes[0].Text));
                if (appointments == null || appointments.IsDeleted == true)
                {
                    return;
                }
                LogTable.Add(appointments.PersonId.ToString(), appointments.Stake.ToString(), "Deleted", DateTime.Now, "none");
                appDbContext.Appointments.Remove(appointments);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var postCabinetQueryable = appDbContext.Appointments.Select(appointments => new
            {
                Id = appointments.Id,
                Person = appointments.Person.Initials == null ? "NULL" : appointments.Person.Initials,
                Stake = appointments.Stake,
                InvokationDate = appointments.InvokationDate,
                DeletionDate = appointments.DeletionDate,
                DismissalReason = appointments.DismissalReason,
                Seats = appointments.Seat.Post.Name == null ? "NULL" : appointments.Seat.Post.Name
            });
            currentDG.DataSource = postCabinetQueryable.ToList();
            appDbContext.Appointments.Load();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept())
                {
                    throw new Exception("The profession of the same person must be unique.");
                }
                var appointments = appDbContext.Appointments.Find(int.Parse(Lables[0].Text));
                var person = appDbContext.Person.Find(int.Parse(Textboxes[0].Text));
                var seats = appDbContext.Seats.Find(int.Parse(Textboxes[4].Text));
                decimal stake = decimal.Parse(Textboxes[1].Text);
                DateTime? deldate = null;
                if (Textboxes[3].Text != "")
                    deldate = DateTime.Parse(Textboxes[3].Text);
                if (appointments != null && person != null && seats != null)
                {
                    LogTable.Add(appointments.PersonId.ToString(), appointments.Stake.ToString(), "Changed", DateTime.Now, person.Initials + " " + Textboxes[1].Text);
                    appointments.Person = person;
                    appointments.Stake = stake <= 0 ? 0.1m : stake;
                    appointments.InvokationDate = DateTime.Parse(Textboxes[2].Text);
                    appointments.DeletionDate = deldate;
                    appointments.DismissalReason = Textboxes[5].Text;
                    appointments.Seat = seats;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 7, true);
            MakeVisible(Lables, 11, 18, true);
            MakeVisible(Textboxes, 0, 6, true);
            MakeVisible(Textboxes, 10, 16, true);
            MakeVisible(ComboBoxes, true, 1, 5);
            MakeVisible(Buttons, true, 1, 3, 4, 5);
            SetupComboBox(ComboBoxes[0], "Id");
            SetupComboBox(ComboBoxes[4], "=", "<", ">", ">=", "<=");
            Lables[21].Visible = true;
            Textboxes[17].Visible = true;
            Lables[0].Text = "Id";
            Lables[1].Text = "Person";
            Lables[2].Text = "Stake";
            Lables[3].Text = "InvokationDate";
            Lables[4].Text = "DeletionDate";
            Lables[5].Text = "Seats";
            Lables[6].Text = "DismissalReason";
            Lables[11].Text = Lables[1].Text;
            Lables[12].Text = Lables[6].Text;
            Lables[13].Text = Lables[5].Text;
            Lables[15].Text = "Date";
            Lables[16].Text = "Inv";
            Lables[17].Text = "Del";
            Lables[21].Text = Lables[2].Text;
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 4; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
                Textboxes[4].Text = selectedRows.Cells[6].Value?.ToString();
                Textboxes[5].Text = selectedRows.Cells[5].Value?.ToString();
            }
            catch { }
        }
        public override void FormShow(Table table)
        {
            if (table is TablePerson)
            {
                var PersonQuery =
                appDbContext.Person
                .Select(person => new
                {
                    Id = person.Id,
                    Initials = person.Initials,
                    Nationality = person.Nationality,
                    Education = person.Education.Profession,
                    Sex = person.Sex,
                    DateOfBirth = person.DateOfBirth,
                    FamilyStatus = person.FamilyStatus,
                    PhoneNumber = person.PhoneNumber,
                    Email = person.Email,
                    IsPhotoAvaliable = person.IsPhotoAvaliable,
                    ChildrensCount = person.ChildrensCount
                }).Where(person => person.Education != null);
                DgView.DataSource = PersonQuery.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = Id?.ToString();
                    Textboxes[0].Text = s == null ? "NULL" : s;
                }
            }
            else if (table is TableSeats)
            {
                var postCabinetQueryable = appDbContext.Seats.Select(seats => new
                {
                    Id = seats.Id,
                    Post = seats.Post.Name,
                    PostCount = seats.PostCount,
                    Cabinet = seats.Cabinet.Name,
                    Salary = seats.Salary
                }).Where(seats => seats.Post != null && seats.Cabinet != null);
                DgView.DataSource = postCabinetQueryable.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = Id?.ToString();
                    Textboxes[4].Text = s == null ? "NULL" : s;
                }
            }
        }
        public void DgViewToUseCellClick(DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = DgView.Rows[ind];
                Id = selectedRows.Cells[0].Value?.ToString();
            }
            catch { }
        }
        public override void Filter()
        {
            try
            {
                var filterQuery = appDbContext.Appointments.Select(appointments => new
                {
                    Id = appointments.Id,
                    Person = appointments.Person.Initials == null ? "NULL" : appointments.Person.Initials,
                    Stake = appointments.Stake,
                    InvokationDate = appointments.InvokationDate,
                    DeletionDate = appointments.DeletionDate,
                    DismissalReason = appointments.DismissalReason,
                    Seats = appointments.Seat.Post.Name == null ? "NULL" : appointments.Seat.Post.Name
                });
                if (Textboxes[11].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.Person.Contains(Textboxes[11].Text));
                }
                if (Textboxes[12].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.DismissalReason.Contains(Textboxes[12].Text));
                }
                if (Textboxes[13].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.Seats.Contains(Textboxes[13].Text));
                }
                if (Textboxes[14].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.InvokationDate == DateTime.Parse(Textboxes[14].Text));
                }
                if (Textboxes[15].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.DeletionDate == DateTime.Parse(Textboxes[15].Text));
                }
                if (Textboxes[17].Text != "")
                {
                    decimal inputStake = decimal.Parse(Textboxes[17].Text);
                    switch (ComboBoxes[4].SelectedItem)
                    {
                        case ">": filterQuery = filterQuery.Where(appointments => appointments.Stake > inputStake); break;
                        case "<": filterQuery = filterQuery.Where(appointments => appointments.Stake < inputStake); break;
                        case "=": filterQuery = filterQuery.Where(appointments => appointments.Stake == inputStake); break;
                        case ">=": filterQuery = filterQuery.Where(appointments => appointments.Stake >= inputStake); break;
                        case "<=": filterQuery = filterQuery.Where(appointments => appointments.Stake <= inputStake); break;
                    }
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);}
        }
        public override void ShowFormFromFilter(Table table)
        {
            if (table is TablePerson)
            {
                var PersonQuery =
                appDbContext.Person
                .Select(person => new
                {
                    Id = person.Id,
                    Initials = person.Initials,
                    Nationality = person.Nationality,
                    Education = person.Education.Profession,
                    Sex = person.Sex,
                    DateOfBirth = person.DateOfBirth,
                    FamilyStatus = person.FamilyStatus,
                    PhoneNumber = person.PhoneNumber,
                    Email = person.Email,
                    IsPhotoAvaliable = person.IsPhotoAvaliable,
                    ChildrensCount = person.ChildrensCount
                }).Where(person => person.Education != null);
                DgView.DataSource = PersonQuery.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = "";
                    try
                    {
                        s = appDbContext.Person.Find(int.Parse(Id)).Initials;
                    }
                    catch
                    {
                        s = null;
                    }
                    Textboxes[11].Text = s;
                }
            }
            else if (table is TableSeats)
            {
                var postCabinetQueryable = appDbContext.Seats.Select(seats => new
                {
                    Id = seats.Id,
                    Post = seats.Post.Name,
                    PostCount = seats.PostCount,
                    Cabinet = seats.Cabinet.Name,
                    Salary = seats.Salary
                }).Where(seats => seats.Post != null && seats.Cabinet != null); ;
                DgView.DataSource = postCabinetQueryable.ToList();
                LookupForm.ShowDialog();
                if (LookupForm.DialogResult == DialogResult.OK)
                {
                    string s = "";
                    try
                    {
                        s = postCabinetQueryable.First(s => s.Id == int.Parse(Id)).Post;
                    }
                    catch
                    {
                        s = "NULL";
                    }
                    Textboxes[13].Text = s;
                }
            }
        }
        public override void Search()
        {
            try
            {
                var filterQuery = appDbContext.Appointments.Select(appointments => new
                {
                    Id = appointments.Id,
                    Person = appointments.Person.Initials == null ? "NULL" : appointments.Person.Initials,
                    Stake = appointments.Stake,
                    InvokationDate = appointments.InvokationDate,
                    DeletionDate = appointments.DeletionDate,
                    DismissalReason = appointments.DismissalReason,
                    Seats = appointments.Seat.Post.Name == null ? "NULL" : appointments.Seat.Post.Name
                });
                if (Textboxes[10].Text != "")
                {
                    filterQuery = filterQuery.Where(appointments => appointments.Id == int.Parse(Textboxes[10].Text));
                }
                currentDG.DataSource = filterQuery.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
    public class TableLogJournal : Table
    {
        public TableLogJournal(ApplicationDbContext context) : base(context) { }

        public override void Add()
        {
            try
            {
                if (IsInTable())
                {
                    throw new Exception("The user's name must be unique.");
                }
                var obj = appDbContext.LogJournal.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    Update(obj.Id);
                }
                else
                {
                    appDbContext.LogJournal.Add(new LogJournal()
                    {
                        Initials = Textboxes[0].Text,
                        Password = Textboxes[1].Text,
                        Role = Textboxes[2].Text
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void Add(string initials, string password, string role)
        {
            try
            {
                var obj = appDbContext.LogJournal.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    var logperson = appDbContext.LogJournal.Find(obj.Id);
                    if (logperson != null)
                    {
                        logperson.Initials = initials;
                        logperson.Password = password;
                        logperson.Role = role;
                        logperson.IsDeleted = false;
                    };
                }
                else
                {
                    appDbContext.LogJournal.Add(new LogJournal()
                    {
                        Initials = initials,
                        Password = password,
                        Role = role
                    });
                }
                appDbContext.SaveChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool IsInTable()
        {
            try
            {
                appDbContext.LogJournal.Single(l => l.Initials == Textboxes[0].Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsInTableExcept()
        {
            try
            {
                appDbContext.LogJournal.Single(l => l.Initials == Textboxes[0].Text && l.Id != int.Parse(Lables[0].Text));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Update(int Id)
        {
            try
            {
                var logperson = appDbContext.LogJournal.Find(Id);
                if (logperson != null)
                {
                    logperson.Initials = Textboxes[0].Text;
                    logperson.Password = Textboxes[1].Text;
                    logperson.Role = Textboxes[2].Text;
                    logperson.IsDeleted = false;
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Remove()
        {
            try
            {
                var logJorunalTuple = appDbContext.LogJournal.Find(int.Parse(Textboxes[0].Text));
                if (logJorunalTuple == null || logJorunalTuple.Id < 3) throw new Exception("You cannot delete the 1st admin or the 1st user");
                appDbContext.LogJournal.Remove(logJorunalTuple);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var LJQuery =
                appDbContext.LogJournal
                .Select(lj => new
                {
                    Id = lj.Id,
                    Initials = lj.Initials,
                    Password = lj.Password,
                    Role = lj.Role
                });
            currentDG.DataSource = LJQuery.ToList();
        }
        public override void Update()
        {
            try
            {
                if (IsInTableExcept())
                {
                    throw new Exception("The name must be unique");
                }
                var logperson = appDbContext.LogJournal.Find(int.Parse(Lables[0].Text));
                if (logperson != null && logperson.Id > 2)
                {
                    logperson.Initials = Textboxes[0].Text;
                    logperson.Password = Textboxes[1].Text;
                    logperson.Role = Textboxes[2].Text;
                    appDbContext.SaveChanges();
                    Show();
                };
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void MenuItem_Click()
        {
            Show();
            MakeVisible(Lables, 0, 4, true);
            MakeVisible(Textboxes, 0, 3, true);
            MakeVisible(Buttons, false, 1, 2, 3, 4, 5);
            MakeVisible(ComboBoxes, 0, 4, false);
            Lables[0].Text = "Id";
            Lables[1].Text = "Initials";
            Lables[2].Text = "Password";
            Lables[3].Text = "Role";
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = currentDG.Rows[ind];
                Lables[0].Text = selectedRows.Cells[0].Value?.ToString();
                for (int i = 0; i < 3; i++)
                {
                    Textboxes[i].Text = selectedRows.Cells[i + 1].Value?.ToString();
                }
            }
            catch { }
        }
    }
    public class TablePersonsInformation : Table, ILoggable
    {
        public DataGridView LogDgView { get; set; }
        public Form LogForm { get; set; }
        public int IdToDelete { get; set; }

        public TablePersonsInformation(ApplicationDbContext context, Form form, DataGridView dataGridView) : base(context) 
        {
            LogDgView = dataGridView;
            LogForm = form;
        }
        public void Add(string Initials, string Operation, DateTime Date, string Information)
        {
            try
            {
                var obj = appDbContext.PersonInformation.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    obj.Initials = Initials;
                    obj.Operation = Operation;
                    obj.Date = Date;
                    obj.Information = Information;
                    obj.IsDeleted = false;
                }
                else
                {
                    appDbContext.PersonInformation.Add(new PersonInformation()
                    {
                        Initials = Initials,
                        Operation = Operation,
                        Date = Date,
                        Information = Information,
                        IsDeleted = false,
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); throw ex; }
        }
        public override void Remove()
        {
            try
            {
                var PITuple = appDbContext.PersonInformation.Find(IdToDelete);
                appDbContext.PersonInformation.Remove(PITuple);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var PIQuery =
                appDbContext.PersonInformation
                .Select(pi => new
                {
                    Id = pi.Id,
                    Initials = pi.Initials,
                    Operation = pi.Operation,
                    Date = pi.Date,
                    Information = pi.Information
                });
            LogDgView.DataSource = PIQuery.ToList();
        }
        public override void MenuItem_Click()
        {
            Show();
            LogForm.ShowDialog();
        }
        public void Clear()
        {
            appDbContext.PersonInformation.RemoveRange(appDbContext.PersonInformation);
            appDbContext.SaveChanges();
            Show();
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = LogDgView.Rows[ind];
                IdToDelete = Convert.ToInt32(selectedRows.Cells[0].Value);
            }
            catch { }
        }
        public void DgCellClick(DataGridViewCellEventArgs e)
        {
            DataGridCellClick(null, e);
        }

        public void DeleteOne()
        {
            Remove();
        }

        public void DeleteAll()
        {
            Clear();
        }
    }
    public class TableSeatsInformation : Table, ILoggable
    {
        public DataGridView LogDgView { get; set; }
        public Form LogForm { get; set; }
        public int IdToDelete { get; set; }

        public TableSeatsInformation(ApplicationDbContext context, Form form, DataGridView dataGridView) : base(context)
        {
            LogDgView = dataGridView;
            LogForm = form;
        }
        public void Add(string PostName, string CabinetName, string Operation, DateTime Date, string Information)
        {
            try
            {
                var obj = appDbContext.SeatsInformation.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    obj.Post = PostName;
                    obj.Cabinet = CabinetName;
                    obj.Operation = Operation;
                    obj.Date = Date;
                    obj.Information = Information;
                    obj.IsDeleted = false;
                }
                else
                {
                    appDbContext.SeatsInformation.Add(new SeatsInformation()
                    {
                        Post = PostName,
                        Cabinet = CabinetName,
                        Operation = Operation,
                        Date = Date,
                        Information = Information,
                        IsDeleted = false,
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); throw ex; }
        }
        public override void Remove()
        {
            try
            {
                var PITuple = appDbContext.SeatsInformation.Find(IdToDelete);
                appDbContext.SeatsInformation.Remove(PITuple);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var SIQuery =
                appDbContext.SeatsInformation
                .Select(pi => new
                {
                    Id = pi.Id,
                    Post = pi.Post,
                    Cabinet = pi.Cabinet, 
                    Operation = pi.Operation,
                    Date = pi.Date,
                    Information = pi.Information
                });
            LogDgView.DataSource = SIQuery.ToList();
        }
        public override void MenuItem_Click()
        {
            Show();
            LogForm.ShowDialog();
        }
        public void Clear()
        {
            appDbContext.SeatsInformation.RemoveRange(appDbContext.SeatsInformation);
            appDbContext.SaveChanges();
            Show();
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = LogDgView.Rows[ind];
                IdToDelete = Convert.ToInt32(selectedRows.Cells[0].Value);
            }
            catch { }
        }
        public void DgCellClick(DataGridViewCellEventArgs e)
        {
            DataGridCellClick(null, e);
        }

        public void DeleteOne()
        {
            Remove();
        }

        public void DeleteAll()
        {
            Clear();
        }
    }
    public class TableAppointmentsInformation : Table, ILoggable
    {
        public DataGridView LogDgView { get; set; }
        public Form LogForm { get; set; }
        public int IdToDelete { get; set; }

        public TableAppointmentsInformation(ApplicationDbContext context, Form form, DataGridView dataGridView) : base(context)
        {
            LogDgView = dataGridView;
            LogForm = form;
        }
        public void Add(string Person, string Stake, string Operation, DateTime Date, string Information)
        {
            try
            {
                var obj = appDbContext.AppointmentsInformation.IgnoreQueryFilters().FirstOrDefault(p => p.IsDeleted == true);
                if (obj != null)
                {
                    obj.Person = Person;
                    obj.Stake = Stake;
                    obj.Operation = Operation;
                    obj.Date = Date;
                    obj.Information = Information;
                    obj.IsDeleted = false;
                }
                else
                {
                    appDbContext.AppointmentsInformation.Add(new AppointmentsInformation()
                    {
                        Person = Person,
                        Stake = Stake,
                        Operation = Operation,
                        Date = Date,
                        Information = Information,
                        IsDeleted = false,
                    });
                }
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); throw ex; }
        }
        public override void Remove()
        {
            try
            {
                var PITuple = appDbContext.AppointmentsInformation.Find(IdToDelete);
                appDbContext.AppointmentsInformation.Remove(PITuple);
                appDbContext.SaveChanges();
                Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override void Show()
        {
            var SIQuery =
                appDbContext.AppointmentsInformation
                .Select(pi => new
                {
                    Id = pi.Id,
                    Person = pi.Person,
                    Stake = pi.Stake,
                    Operation = pi.Operation,
                    Date = pi.Date,
                    Information = pi.Information
                });
            LogDgView.DataSource = SIQuery.ToList();
        }
        public override void MenuItem_Click()
        {
            Show();
            LogForm.ShowDialog();
        }
        public void Clear()
        {
            appDbContext.AppointmentsInformation.RemoveRange(appDbContext.AppointmentsInformation);
            appDbContext.SaveChanges();
            Show();
        }
        public override void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int ind = e.RowIndex;
                DataGridViewRow selectedRows = LogDgView.Rows[ind];
                IdToDelete = Convert.ToInt32(selectedRows.Cells[0].Value);
            }
            catch { }
        }
        public void DgCellClick(DataGridViewCellEventArgs e)
        {
            DataGridCellClick(null, e);
        }

        public void DeleteOne()
        {
            Remove();
        }

        public void DeleteAll()
        {
            Clear();
        }
    }
}
