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
    /// Логика взаимодействия для AddCriterion.xaml
    /// </summary>
    public partial class AddCriterion : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        public int id_criterion = -1;
        public string save_name = "Сохранить";
        public string edit_name = "Изменить";
        public AddCriterion()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.AddCriterionCompleted += Client_AddCriterionCompleted;
            client.EditCriterionCompleted += Client_EditCriterionCompleted;
        }

        private void Client_EditCriterionCompleted(object sender, ServiceReference1.EditCriterionCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                id_criterion = e.Result;
                MessageBox.Show("Критерий изменен");
            }
            else
            {
                MessageBox.Show("Не удалось сохранить изменения");
            }
        }

        private void Client_AddCriterionCompleted(object sender, ServiceReference1.AddCriterionCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                SaveCriterion.Content = edit_name;
                MessageBox.Show("Критерий добавлен");
                id_criterion = e.Result;
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

       

        private void SaveCriterion_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCriterion.Content.ToString() == save_name)
            {
                client.AddCriterionAsync(TextBoxNameCriterion.Text);
                Loading.start();
            }
            else
           if (SaveCriterion.Content.ToString() == edit_name)
            {
                client.EditCriterionAsync(id_criterion, TextBoxNameCriterion.Text);
            }
           
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
    }
}
