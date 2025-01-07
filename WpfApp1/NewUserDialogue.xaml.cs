using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for newuserdialogue.xaml
    /// </summary>
    public partial class NewUserDialogue : Window
    {
        public NewUserDialogue()
        {
            InitializeComponent();
        }

        public string UsernameResponse{
            get { return UsernameTextBox.Text; }
            set { UsernameTextBox.Text = value;}
        }

        public string PasswordResponse
        {
            get { return PasswordBox.Password; }
            set { PasswordBox.Password = value; }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
