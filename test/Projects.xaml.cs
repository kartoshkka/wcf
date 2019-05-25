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
    /// Логика взаимодействия для Projects.xaml
    /// </summary>
    public partial class Projects : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        AddProject _AddProject;
        public Projects()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.GetListProjectsCompleted += Client_GetListProjectsCompleted;
            client.GetListProjectsAsync();
            Loading.start();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void Client_GetListProjectsCompleted(object sender, ServiceReference1.GetListProjectsCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Error == null)
            {
                dgProjects.ItemsSource = e.Result.ToList();
                //MessageBox.Show("Критерий добавлен");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void bt_Project_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference1.projects temp = dgProjects.SelectedItem as ServiceReference1.projects;
            _AddProject = new AddProject();
            _AddProject.Owner = this;

            _AddProject.TextBoxNameProject.Text = temp.name;
            _AddProject.SaveProject.Content = _AddProject.edit_name;
            _AddProject.id_project = temp.id_project;
            if (_AddProject.ShowDialog() == true)
            {
                Loading.start();
                client.GetListProjectsAsync();
            }
            else
            {
                Loading.start();
                client.GetListProjectsAsync();
            }
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }

        private void AddProjectClick(object sender, RoutedEventArgs e)
        {
            _AddProject = new AddProject();
            _AddProject.Owner = this;
            if (_AddProject.ShowDialog() == true)
            {
                Loading.start();
                client.GetListProjectsAsync();
            }
            else
            {
                Loading.start();
                client.GetListProjectsAsync();
            }
        }
    }
}
