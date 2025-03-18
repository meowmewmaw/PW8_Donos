using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PW8_Donos
{
    public partial class Form1 : Form
    {
        public interface ICommand
        {
            void Execute(object? arg);
            bool CanExecute(object? arg);
            event EventHandler? CanExecuteChanged;
        }
        public class MainCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            Action<object?> action;
            public MainCommand(Action<object?> action)
            {
                this.action = action;
            }
            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter) => action?.Invoke(parameter);
        }
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public int Age { get; set; }
            public override string ToString() => Name;
        }
        public class MainViewModel : INotifyPropertyChanged
        {
            static int id = 0; // äëÿ ãåíåðàöèè id

            // äàííûå äëÿ íîâîãî îáúåêòà
            string name = "";
            int age;

            // êîìàíäà äëÿ äîáàâëåíèÿ
            public ICommand AddCommand { get; set; }
            // êîìàíäà äëÿ óäàëåíèÿ
            public ICommand RemoveCommand { get; set; }
            // äàííûå äëÿ îòîáðàæåíèÿ â ñïèñêå
            public BindingList<Person> People { get; }
            public string Name
            {
                get => name;
                set
                {
                    if (name != value)
                    {
                        name = value;
                        OnPropertyChanged();
                    }
                }
            }
            public int Age
            {
                get => age;
                set
                {
                    if (age != value)
                    {
                        age = value;
                        OnPropertyChanged();
                    }
                }
            }
            public MainViewModel()
            {
                People = new()
         {

             new Person {Id=++id, Name="Tom", Age=38 },
             new Person {Id=++id, Name ="Bob", Age = 42},
             new Person {Id=++id, Name = "Sam", Age = 25}
         };
                // îïðåäåëÿåì êîìàíäó äîáàâëåíèÿ
                AddCommand = new MainCommand(_ =>
                {
                    People.Add(new Person { Id = ++id, Name = this.Name, Age = this.Age });
                    Name = ""; Age = 0;
                });
                // îïðåäåëÿåì êîìàíäó óäàëåíèÿ
                RemoveCommand = new MainCommand(obj =>
                {
                    // èùåì îáúåêò ïî id
                    if (obj is int id)
                    {
                        var person = People.FirstOrDefault(p => p.Id == id);
                        // åñëè îáúåêò íàéäåí, óäàëÿåì åãî
                        if (person != null) { People.Remove(person); }
                    }
                });
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public Form1()
        {
            InitializeComponent();

            ListBox peopleListBox = new ListBox();
            peopleListBox.Dock = DockStyle.Left;
            peopleListBox.SelectionMode = SelectionMode.One;
            Controls.Add(peopleListBox);


            Panel personForm = new Panel { Padding = new Padding(10), Width = 260 };
            personForm.Dock = DockStyle.Right;
            TextBox nameBox = new TextBox();
            nameBox.Location = new Point(12, 10);
            nameBox.Size = new Size(230, 27);
            personForm.Controls.Add(nameBox);

            NumericUpDown ageBox = new NumericUpDown { Minimum = 0, Maximum = 100 };
            ageBox.Location = new Point(12, 50);
            ageBox.Size = new Size(230, 27);
            personForm.Controls.Add(ageBox);

            Button addButton = new Button { Text = "Save", AutoSize = true };
            addButton.Location = new Point(12, 80);
            personForm.Controls.Add(addButton);

            Button removeButton = new Button { Text = "Remove", AutoSize = true };
            removeButton.Location = new Point(12, 110);
            personForm.Controls.Add(removeButton);
            Controls.Add(personForm);

            // óñòàíàâëèâàåì ìîäåëü ïðåäñòàâëåíèÿ
            DataContext = new MainViewModel();

            peopleListBox.DataBindings.Add(new Binding("DataSource", DataContext, "People"));
            peopleListBox.DisplayMember = "Name";
            peopleListBox.ValueMember = "Id";

            nameBox.DataBindings.Add(new Binding("Text", DataContext, "Name", true, DataSourceUpdateMode.OnPropertyChanged));
            ageBox.DataBindings.Add(new Binding("Value", DataContext, "Age", true, DataSourceUpdateMode.OnPropertyChanged));

            // ïðèâÿçêà ê êîìàíäå äîáàâëåíèÿ
            addButton.DataBindings.Add(new Binding("Command", DataContext, "AddCommand", true));
            // ïðèâÿçêà ê êîìàíäå óäàëåíèÿ
            removeButton.DataBindings.Add(new Binding("Command", DataContext, "RemoveCommand", true));
            // ïðèâÿçêà ê ïàðàìåòðó êîìàíäû
            removeButton.DataBindings.Add(new Binding("CommandParameter", peopleListBox, "SelectedValue"));
        }
    }
}