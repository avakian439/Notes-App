using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Diagnostics;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public class UserData
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public required List<Note> Notes { get; set; }
    }

    public class Note
    {
        public DateTimeOffset Date { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }

    }

    public partial class LoginWindow : Window
    {
        public Button? lastClickedButton;
        public string FileName = "UserData.json";

        public LoginWindow()
        {
            InitializeComponent();
            getUserData();
        }


        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewUserDialogue();
            if (dialog.ShowDialog() == true)
            {
                List<UserData> existingUsers = new();
                if (File.Exists(FileName))
                {
                    string existingJson = File.ReadAllText(FileName);
                    existingUsers = JsonSerializer.Deserialize<List<UserData>>(existingJson) ?? new List<UserData>();
                }

                var username = dialog.UsernameResponse;
                username = username.ToLower();
                var user = existingUsers.Find(x => x.Name == username);
                if (user != null)
                {
                    if (user.Password == dialog.PasswordResponse && lastClickedButton != null)
                    {
                        existingUsers.Remove(user);
                        string JsonString = JsonSerializer.Serialize(existingUsers, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(FileName, JsonString);
                            
                        lastClickedButton.Content = null;
                        AccountStack.Children.Remove(lastClickedButton);
                        AccountLoginButton.IsEnabled = false;
                        lastClickedButton = null;
                        getUserData();
                        MessageBox.Show("Account removed successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Invalid Password");
                    }
                }
                else
                {
                    var newUser = new UserData { Name = username, Password = dialog.PasswordResponse, Notes = new List<Note>() };
                    existingUsers.Add(newUser);
                    string JsonString = JsonSerializer.Serialize(existingUsers, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(FileName, JsonString);
                    getUserData();
                }
            }
        }

        private void AccountLoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastClickedButton != null)
            {
                var dialog = new PasswordDialogue();
                if (dialog.ShowDialog() == true)
                {
                    List<UserData> existingUsers = new();
                    if (File.Exists(FileName))
                    {
                        string existingJson = File.ReadAllText(FileName);
                        existingUsers = JsonSerializer.Deserialize<List<UserData>>(existingJson) ?? new List<UserData>();
                    }

                    var username = ((Label)((Grid)lastClickedButton.Content).Children[1]).Content.ToString();
                    var user = existingUsers.Find(x => x.Name == username);
                    if (user != null && user.Password == dialog.PasswordResponse)
                    {
                        var mainWindow = new MainWindow(user);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Password");
                    }
                }
            }
        }

        private Button CreateNewAccountButton(String username)
        {
            Button button = new Button
            {
                Background = null,
                BorderBrush = null,
                Height = 183,
            };

            Grid grid = new Grid {
                Height = 147
            };

            Image image = new Image
            {
                Height = 130,
                Width = 130,
                Margin = new Thickness(25, 0, 25, 0),
                Source = new BitmapImage(new Uri("pack://application:,,,/userpicture.png"))
            };

            Label label = new Label
            {
                Foreground = new SolidColorBrush(Colors.White),
                VerticalContentAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 21, 0 ,-21),
                FontSize = 16,
                Content = username,
            };

            grid.Children.Add(image);
            grid.Children.Add(label);

            button.Content = grid;
            button.Click += (sender, e) =>
            {
                lastClickedButton = button;
                AccountLoginButton.IsEnabled = true;
            };

            return button;
        }

        private void getUserData()
        {
            List<UserData> existingUsers = new();

            if (File.Exists(FileName))
            {
                string existingJson = File.ReadAllText(FileName);
                existingUsers = JsonSerializer.Deserialize<List<UserData>>(existingJson) ?? new List<UserData>();
            }

            foreach (var user in existingUsers)
            {
                if (user.Name != null)
                {
                    AccountStack.Children.Insert(AccountStack.Children.Count - 1, CreateNewAccountButton(user.Name));
                }
            }
        }


    }
}