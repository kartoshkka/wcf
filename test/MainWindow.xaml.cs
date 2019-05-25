using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {
        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading cLoading;
        AddExperise _addExperise;
        Criterions _criterions;
        Experts _experts;
        Projects _projects;
        ExpertiseWork _expertiseWork;
        СompletedExpertises _completedExpertises;
       
        ExpertiseWorkPospelov _expertiseWorkPospelov;
        Autorization _autorization;
        public int id_expert;
        public ServiceReference1.experts temp_experts;
        public MainWindow()
        {
            InitializeComponent();
            
            cLoading = new CLoading(circle);
            client.GetListCurrentExpertisesCompleted += Client_GetListCurrentExpertisesCompleted;
           
            //client.GetListCurrentExpertisesAsync();
            Autorization();
            cLoading.start();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        //Метод авторизации
        public void Autorization()
        {
            try
            {
                _autorization = new Autorization();
                //_autorization.Owner = this;
                if (_autorization.ShowDialog() == true)
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                    this.Visibility = Visibility.Visible;
                    this.ShowInTaskbar = true;
                    if (temp_experts.access == 1)
                    {
                        label_fio.Content ="Вы авторизованы под учетной записью "+ temp_experts.second_name+ " " + temp_experts.first_name + " " + temp_experts.patronymic;
                        buton_criteries.IsEnabled = true;
                        buton_experts.IsEnabled = true;
                        buton_project.IsEnabled = true;
                        ButtonAddExperise.IsEnabled = true;
                    }
                    else
                    {
                        label_fio.Content = "Вы авторизованы под учетной записью " + temp_experts.second_name + " " + temp_experts.first_name + " " + temp_experts.patronymic;
                        buton_criteries.IsEnabled = false;
                        buton_experts.IsEnabled = false;
                        buton_project.IsEnabled = false;
                        ButtonAddExperise.IsEnabled = false;
                    }
                }
                else
                {
                    this.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Client_GetListCurrentExpertisesCompleted(object sender, ServiceReference1.GetListCurrentExpertisesCompletedEventArgs e)
        {
            cLoading.stop();
            if (e.Error == null)
            {
                dgCurrentExpertises.ItemsSource = e.Result.ToList();

            }
            else
                MessageBox.Show(e.Error.Message);
        }

       

       

      
        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        //создать новую экспертизу
        private void ButtonAddExperise_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _addExperise = new AddExperise();
                _addExperise.Owner = this;
                if (_addExperise.ShowDialog() == true)
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
                else
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
            }
        
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonAddCriterion_Click(object sender, RoutedEventArgs e)
        {
            _criterions = new Criterions();
            _criterions.Owner = this;
            _criterions.ShowDialog();
        }

        private void Expert_click(object sender, RoutedEventArgs e)
        {
            _experts = new Experts();
            _experts.Owner = this;
            _experts.ShowDialog();
        }

        private void project_click(object sender, RoutedEventArgs e)
        {
            _projects = new Projects();
            _projects.Owner = this;
            _projects.ShowDialog();
        }
        //начать проходить экспертизу
        private void bt_Expertise_Click(object sender, RoutedEventArgs e)
        {
            //var  dt = new DataTable();

            // for (int i = 0; i < 3; i++)
            //     dt.Columns.Add("col" + i.ToString());

            // for (int i = 0; i < 3; i++)
            // {

            //     DataRow r =dt.NewRow();
            //     r[0] = "a" + i.ToString();
            //     r[1] = "b" + i.ToString();
            //     r[2] = "c" + i.ToString();
            //     dt.Rows.Add(r);
            // }

            // dgCurrentExpertises.ItemsSource = dt.DefaultView;
            ServiceReference1.expertises temp = dgCurrentExpertises.SelectedItem as ServiceReference1.expertises;
            if(temp.type ==1)
            {
                _expertiseWork = new ExpertiseWork();
                _expertiseWork.id_expertises = temp.id_expertise;
                _expertiseWork.id_expert = id_expert;
                _expertiseWork.Owner = this;
                _expertiseWork.client.GetListExpertisesCriterionsAsync(temp.id_expertise, id_expert);


                if (_expertiseWork.ShowDialog() == true)
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
                else
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
            }
            else if(temp.type ==2)
            {

                _expertiseWorkPospelov = new ExpertiseWorkPospelov();
                _expertiseWorkPospelov.id_expertises = temp.id_expertise;
                _expertiseWorkPospelov.id_expert = id_expert;
                _expertiseWorkPospelov.Owner = this;
                _expertiseWorkPospelov.client.GetListExpertisePospelovAsync(temp.id_expertise, id_expert);


                if (_expertiseWorkPospelov.ShowDialog() == true)
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
                else
                {
                    cLoading.start();
                    client.GetListCurrentExpertisesAsync(id_expert);
                }
            }
            else
            {
                MessageBox.Show("Не возможно понять какой метод экспертизы используется.");
            }
        }

      
        //открыть завершенные экспертизы
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            _completedExpertises = new СompletedExpertises();
            _completedExpertises.Owner = this;
           
            if (_completedExpertises.ShowDialog() == true)
            {
                cLoading.start();
                client.GetListCurrentExpertisesAsync(id_expert);
            }
            else
            {
                cLoading.start();
                client.GetListCurrentExpertisesAsync(id_expert);
            }




        }

        private void bt_exit_click(object sender, RoutedEventArgs e)
        {
            temp_experts = null;
            id_expert = -1;
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            Autorization();
        }
    }
}
