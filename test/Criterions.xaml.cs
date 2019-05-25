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
    /// Логика взаимодействия для Criterions.xaml
    /// </summary>
    public partial class Criterions : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        AddCriterion _addCriterion;
        public Criterions()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.GetListCriterionsCompleted += Client_GetListCriterionsCompleted;
            client.GetListCriterionsAsync();
            Loading.start();
        }
        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Client_GetListCriterionsCompleted(object sender, ServiceReference1.GetListCriterionsCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Error == null)
            {
                dgCriteries.ItemsSource = e.Result.ToList();
                //MessageBox.Show("Критерий добавлен");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void AddCriterionClick(object sender, RoutedEventArgs e)
        {
            _addCriterion = new AddCriterion();
            _addCriterion.Owner = this;
            if (_addCriterion.ShowDialog() == true)
            {
                Loading.start();
                client.GetListCriterionsAsync();
            }
            else
            {
                Loading.start();
                client.GetListCriterionsAsync();
            }
        }
        private void bt_criterion_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference1.criterions temp = dgCriteries.SelectedItem as ServiceReference1.criterions;
            _addCriterion = new AddCriterion();
            _addCriterion.Owner = this;
            
            _addCriterion.id_criterion = temp.id_criterion;
            _addCriterion.TextBoxNameCriterion.Text = temp.name;
            _addCriterion.SaveCriterion.Content = _addCriterion.edit_name;
            if (_addCriterion.ShowDialog() == true)
            {
                Loading.start();
                client.GetListCriterionsAsync();
            }
            else
            {
                Loading.start();
                client.GetListCriterionsAsync();
            }

        }
    }
}
