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
    /// Логика взаимодействия для Experts.xaml
    /// </summary>
    public partial class Experts : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        AddExpert _AddExpert;
        public Experts()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.GetListExpertsCompleted += Client_GetListExpertsCompleted;
            client.GetListExpertsAsync();
            Loading.start();
        }

        private void Client_GetListExpertsCompleted(object sender, ServiceReference1.GetListExpertsCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Error == null)
            {
                dgExperts.ItemsSource = e.Result.ToList();
                //MessageBox.Show("Критерий добавлен");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void AddExpertClick(object sender, RoutedEventArgs e)
        {
            _AddExpert = new AddExpert();
            _AddExpert.Owner = this;
            _AddExpert.ShowDialog();
        }

        private void bt_Expert_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference1.experts temp = dgExperts.SelectedItem as ServiceReference1.experts;
            _AddExpert = new AddExpert();
            _AddExpert.Owner = this;
            
            _AddExpert.TextBoxFirstName.Text = temp.first_name;
            _AddExpert.TextBoxLastName.Text = temp.second_name;
            _AddExpert.TextBoxPatronimic.Text = temp.patronymic;
            _AddExpert.TextBoxLogin.Text = temp.login;
            _AddExpert.TextboxPassword.Password = temp.password;
            _AddExpert.SaveExpert.Content = _AddExpert.edit_name;
            _AddExpert.id_expert = temp.id_expert;
            if (_AddExpert.ShowDialog() == true)
            {
                Loading.start();
                client.GetListExpertsAsync();
            }
            else
            {
                Loading.start();
                client.GetListExpertsAsync();
            }
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
    }
}
