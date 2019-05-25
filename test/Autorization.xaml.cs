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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading cLoading;
        public int id_expert = -1;
        public Autorization()
        {
            InitializeComponent();
            cLoading = new CLoading(circle);
            client.GetIdExpertCompleted += Client_GetIdExpertCompleted;
        }

        private void Client_GetIdExpertCompleted(object sender, ServiceReference1.GetIdExpertCompletedEventArgs e)
        {
            cLoading.stop();
            if (e.Error == null)
            {
                if (e.Result.id_expert != -1)
                {
                    ((MainWindow)Application.Current.MainWindow).id_expert = e.Result.id_expert;
                    ((MainWindow)Application.Current.MainWindow).temp_experts = e.Result;
                    MessageBox.Show("Вы успешно авторизовались");
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Такой учетной записи не существует");
                }

            }
            else
                MessageBox.Show(e.Error.Message);
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        private void Autorization_Click(object sender, RoutedEventArgs e)
        {
            string pas = TextboxPassword.Password;
            string login = TextBoxLogin.Text;
            client.GetIdExpertAsync(login, pas);
            cLoading.start();
        }
    }
}
