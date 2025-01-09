using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserData user; // Declare user at the class level
        string fileName = "UserData.json";

        public MainWindow(UserData user)
        {
            InitializeComponent();
            this.user = user; // Assign the passed user to the class-level variable
            getUserData(user);
        }

        private Button addNewNote(string title, string text, DateTimeOffset date)
        {
            Button button = new()
            {
                Margin = new Thickness(20, 20, 0, 0),
                Width = 220,
                MaxWidth = 220,
                MaxHeight = 220,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Top,
                Background = new SolidColorBrush(Colors.White),
            };

            TextBlock textblock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
                Height = 100,
                Width = 220,
            };

            button.Click += (sender, e) =>
            {
                var dialog = new NoteEditor();
                dialog.TitleResponse = title;
                dialog.TextResponse = text;
                if (dialog.ShowDialog() == true)
                {
                    Note noteToEdit = user.Notes.FirstOrDefault(n => n.Title == title);

                    if (noteToEdit != null && string.IsNullOrEmpty(dialog.TitleResponse))
                    {
                        RemoveNoteFromUserData(user, noteToEdit);
                    }

                    if (noteToEdit != null)
                    {
                        noteToEdit.Title = dialog.TitleResponse;
                        noteToEdit.Text = dialog.TextResponse;
                        noteToEdit.Date = DateTimeOffset.Now;
                        SaveUserDataToJson(user);
                        RefreshNoteStack(user);
                    }
                    else
                    {
                        AddNoteToUserData(user, dialog.TitleResponse, dialog.TextResponse, DateTimeOffset.Now);
                        SaveUserDataToJson(user);
                        RefreshNoteStack(user);
                    }
                    }
                };

            button.Content = textblock;
            textblock.Text = title + " - " + date + " \n\n " + text;

            return button;
        }

        private void getUserData(UserData user)
        {
            foreach (Note i in user.Notes)
            {
                if (i.Title != null && i.Text != null)
                {
                    if (NoteStack.Children.Count > 0)
                    {
                        int IndexToAddTo = NoteStack.Children.Count;
                        NoteStack.Children.Insert(IndexToAddTo, addNewNote(i.Title, i.Text, i.Date));
                    }
                    else
                    {
                        NoteStack.Children.Add(addNewNote(i.Title, i.Text, i.Date));
                    }
                }
            }
        }
        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NoteEditor();
            if (dialog.ShowDialog() == true)
            {
                if (user.Notes.Any(n => n.Title == dialog.TitleResponse))
                {
                    MessageBox.Show("A note with this title already exists. Please choose a different title.");
                    return;
                }
                AddNoteToUserData(user, dialog.TitleResponse, dialog.TextResponse, DateTimeOffset.Now);
                RefreshNoteStack(user);
            }
        }

        private void AddNoteToUserData(UserData user, string title, string text, DateTimeOffset date)
        {
            Note newNote = new Note
            {
                Title = title,
                Text = text,
                Date = date
            };

            user.Notes.Add(newNote);
            SaveUserDataToJson(user);
        }

        private void RemoveNoteFromUserData(UserData user, Note noteToRemove)
        {
            if (user.Notes.Contains(noteToRemove))
            {
                user.Notes.Remove(noteToRemove);
                RefreshNoteStack(user);
            }
        }

        private void RefreshNoteStack(UserData user)
        {
            NoteStack.Children.Clear();
            getUserData(user);
            this.user = user;
        }
        private void SaveUserDataToJson(UserData user)
        {
            List<UserData> existingUsers = new();

            if (File.Exists(fileName))
            {
                string existingJson = File.ReadAllText(fileName);
                existingUsers = JsonSerializer.Deserialize<List<UserData>>(existingJson) ?? new List<UserData>();
            }

            var existingUser = existingUsers.FirstOrDefault(u => u.Name == user.Name);
            if (existingUser != null)
            {
                existingUsers.Remove(existingUser);
            }
            existingUsers.Add(user);

            string json = JsonSerializer.Serialize(existingUsers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginwindow = new LoginWindow();
            loginwindow.Show();
            this.Close();
        }
    }
}
