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
    /// Логика взаимодействия для AddProject.xaml
    /// </summary>
    public partial class AddProject : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        public int id_project = -1;
        public string save_name = "Сохранить";
        public string edit_name = "Изменить";
        public AddProject()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.AddProjectCompleted += Client_AddProjectCompleted;
            client.EditProjectCompleted += Client_EditProjectCompleted;
        }

        private void Client_EditProjectCompleted(object sender, ServiceReference1.EditProjectCompletedEventArgs e)
        {

            Loading.stop();
            if (e.Result != -1)
            {
                id_project = e.Result;
                MessageBox.Show("Проект изменен");
            }
            else
            {
                MessageBox.Show("Не удалось сохранить изменения");
            }
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        private void Client_AddProjectCompleted(object sender, ServiceReference1.AddProjectCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                SaveProject.Content = edit_name;
                MessageBox.Show("Проект добавлен");
                id_project = e.Result;
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (SaveProject.Content.ToString() == save_name)
            {
                client.AddProjectAsync(TextBoxNameProject.Text);
                Loading.start();
            }
            else
           if (SaveProject.Content.ToString() == edit_name)
            {
                client.EditProjectAsync(id_project, TextBoxNameProject.Text);
            }
        }
    }
}
