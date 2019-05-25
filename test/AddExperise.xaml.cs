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
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls.Primitives;

namespace test
{
    /// <summary>
    /// Логика взаимодействия для AddExperise.xaml
    /// </summary>
    public partial class AddExperise : Window
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading Loading;
        string save_name = "Сохранить";
        string edit_name = "Изменить";
        bool type_expertise = false;//false - паттерн,true - поспелов
        ObservableCollection<Ccriteries> list_criteries_value = new ObservableCollection<Ccriteries>();
        List<int> list_id_experts = new List<int>();
        List<int> list_id_projects = new List<int>();
        DataTable dt = new DataTable();
        ObservableCollection<DataTable> ls_dt = new ObservableCollection<DataTable>();
        public AddExperise()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            dgCriteries_valie.ItemsSource = list_criteries_value;
            client.AddExperisePatternCompleted += Client_AddExperisePatternCompleted;
            client.AddExperisePospelovCompleted += Client_AddExperisePospelovCompleted;
            client.GetListExpertsCompleted += Client_GetListExpertsCompleted;
            client.GetListCriterionsCompleted += Client_GetListCriterionsCompleted;
            client.GetListProjectsCompleted += Client_GetListProjectsCompleted;
            client.GetListExpertsAsync();
            client.GetListCriterionsAsync();
            client.GetListProjectsAsync();
            Loading.start();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void Client_AddExperisePospelovCompleted(object sender, ServiceReference1.AddExperisePospelovCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                SaveExperise.Content = "Изменить";
                MessageBox.Show("Экспертиза добавлена");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_AddExperisePatternCompleted(object sender, ServiceReference1.AddExperisePatternCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Result != -1)
            {
                SaveExperise.Content = "Изменить";
                MessageBox.Show("Экспертиза добавлена");
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
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

        
       
        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        //сохраняем экспертизу
        private void SaveExperise_Click(object sender, RoutedEventArgs e)
        {
            if(type_expertise == false)
            {
                var t1 = list_criteries_value.Sum(c => c.weight);
                if (t1 == 1)
                {
                    if (list_id_experts.Count() != 0)
                    {
                        if (list_id_projects.Count() != 0)
                        {
                            Dictionary<int, float> weight_criteries = new Dictionary<int, float>();
                            foreach (var temp in list_criteries_value)
                            {
                                weight_criteries.Add(temp.id_criterion, temp.weight);
                            }
                            client.AddExperisePatternAsync(NameExpertise.Text, list_id_projects, list_id_experts, weight_criteries);
                            Loading.start();
                        }
                        else
                        {
                            MessageBox.Show("Не выбран не один проект!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не выбран не один эксперт!");
                    }
                }
                else
                {
                    MessageBox.Show("Сумма весов должны быть равна 1!");
                }
            }
            else
            {
                if (list_id_experts.Count() != 0)
                {
                    List<List<string>> ls_factors = new List<List<string>>();
                    int level = 0;
                    foreach (var temp in ls_dt)
                    {
                        List<string> ls = new List<string>();
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {
                            ls.Add(temp.Rows[i][0].ToString());
                        }
                        ls_factors.Add(ls);
                        level++;
                    }
                    client.AddExperisePospelovAsync(NameExpertise.Text, list_id_experts, ls_factors);
                }
                else
                {
                    MessageBox.Show("Не выбран не один эксперт!");
                }
                   
               
            }
            
           
              
            


        }
    
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            ServiceReference1.experts temp = dgExperts.SelectedItem as ServiceReference1.experts;
            var chkSelectAll = sender as CheckBox;
            if(chkSelectAll.IsChecked == true)
            {
                list_id_experts.Add(temp.id_expert);
            }
            else
            {
                int t = list_id_experts.FindIndex(c => c == temp.id_expert);
                list_id_experts.RemoveAt(t);
                
            }
           
        }
        
        private void checkBox_Checked_Criteries(object sender, RoutedEventArgs e)
        {
            
                ServiceReference1.criterions temp = dgCriteries.SelectedItem as ServiceReference1.criterions;
                var chkSelectAll = sender as CheckBox;
                if (chkSelectAll.IsChecked == true)
                {
                    Ccriteries ccriteries = new Ccriteries();
                    ccriteries.id_criterion = temp.id_criterion;
                    ccriteries.name = temp.name;
                    ccriteries.weight = 0;
                    list_criteries_value.Add(ccriteries);
                }
                else
                {
                    //int t = list_criteries_value.indexof(c => c.id_criterion == temp.id_criterion);
                    var q = list_criteries_value.IndexOf(list_criteries_value.Where(x => x.id_criterion == temp.id_criterion).FirstOrDefault());
                    if (q != -1)
                    {
                        list_criteries_value.RemoveAt(q);
                    }


                }

            
            

        }
        private void checkBox_Checked_Projects(object sender, RoutedEventArgs e)
        {

            ServiceReference1.projects temp = dgProjects.SelectedItem as ServiceReference1.projects;
            var chkSelectAll = sender as CheckBox;
            if (chkSelectAll.IsChecked == true)
            {
                list_id_projects.Add(temp.id_project);
            }
            else
            {
                int t = list_id_projects.FindIndex(c => c == temp.id_project);
                list_id_projects.RemoveAt(t);

            }

        }
        private void btnAddMore_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in ls_dt)
            {
                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    MessageBox.Show(temp.Rows[i][0].ToString());
                }

            }


        }
        //формируем цели
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Label label = new Label();
            label.Content = "Укажите цели";
            DataGrid dataGrid = new DataGrid();
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.AutoGenerateColumns = true;
            var MyStyle = new Style(typeof(DataGridCell))
            {
                Setters = {
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                          }
            };
            var MyStyle2 = new Style(typeof(DataGridColumnHeader))
            {
                Setters = {
                                new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center)
                          }
            };
            dataGrid.CellStyle = MyStyle;
            dataGrid.ColumnHeaderStyle = MyStyle2;
            
            dt = new DataTable();
            dt.Columns.Add("Цели");

            ls_dt.Add(dt);
            dataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);

            dataGrid.ItemsSource = dt.DefaultView;

            splMain.Children.Add(label);
            splMain.Children.Add(dataGrid);
            bt_aim.IsEnabled = false;
            bt_level.IsEnabled = true;

        }
        //формируем уровень
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Label label = new Label();
            label.Content = "Укажите факторы " + ls_dt.Count() + " уровня";
            DataGrid dataGrid = new DataGrid();
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.AutoGenerateColumns = true;
            var MyStyle = new Style(typeof(DataGridCell))
            {
                Setters = {
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                          }
            };
            var MyStyle2 = new Style(typeof(DataGridColumnHeader))
            {
                Setters = {
                                new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center)
                          }
            };
            dataGrid.CellStyle = MyStyle;
            dataGrid.ColumnHeaderStyle = MyStyle2;
            dt = new DataTable();

            dt.Columns.Add("факторы " + ls_dt.Count() + " уровня");

            ls_dt.Add(dt);


            dataGrid.ItemsSource = dt.DefaultView;
            dataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);

            splMain.Children.Add(label);
            splMain.Children.Add(dataGrid);
        }
        //переключатель метода
        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var selectedItem =  comboBox.SelectedIndex;
            if(selectedItem != -1)
            {
                //значит паттерн
                if(selectedItem == 0 )
                {
                    type_expertise = false;
                    tbc.SelectedIndex = 0;
                }
                //значит поспелов
                else
                {
                    type_expertise = true;
                    tbc.SelectedIndex = 1;
                }
            }
            
        }

       
    }
}
