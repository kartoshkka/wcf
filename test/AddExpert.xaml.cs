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

namespace test
{
    /// <summary>
    /// Логика взаимодействия для AddExpert.xaml
    /// </summary>
    public partial class AddExpert : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        public int id_expert = -1;
        public string save_name = "Сохранить";
        public string edit_name = "Изменить";
        public AddExpert()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.AddExpertCompleted += Client_AddExpertCompleted;
            client.EditExpertCompleted += Client_EditExpertCompleted;
        }

        private void Client_EditExpertCompleted(object sender, ServiceReference1.EditExpertCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                id_expert = e.Result;
                MessageBox.Show("Эксперт изменен");
            }
            else
            {
                MessageBox.Show("Не удалось сохранить изменения");
            }
        }

        private void Client_AddExpertCompleted(object sender, ServiceReference1.AddExpertCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                SaveExpert.Content = edit_name;
                MessageBox.Show("Эксперт добавлен");
                id_expert = e.Result;
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void SaveExpert_Click(object sender, RoutedEventArgs e)
        {
            if (SaveExpert.Content.ToString() == save_name)
            {
                int acces = -1;
                if(CheckBoxAccses.IsChecked ==true)
                {
                    acces = 1;
                }
                else
                {
                    acces = 2;
                }
                client.AddExpertAsync(TextBoxFirstName.Text,TextBoxLastName.Text, TextBoxPatronimic.Text, TextBoxLogin.Text, TextboxPassword.Password, acces);
                Loading.start();
            }
            else
          if (SaveExpert.Content.ToString() == edit_name)
            {
                int acces = -1;
                if (CheckBoxAccses.IsChecked == true)
                {
                    acces = 1;
                }
                else
                {
                    acces = 2;
                }
                client.EditExpertAsync(id_expert, TextBoxFirstName.Text, TextBoxLastName.Text, TextBoxPatronimic.Text, TextBoxLogin.Text, TextboxPassword.Password,acces);
            }
        }
        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
    }
}
