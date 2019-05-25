using System;
using System.Collections.Generic;
using System.Data;
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
using System.Collections.ObjectModel;
namespace test
{
    /// <summary>
    /// Логика взаимодействия для ExpertiseWorkPospelov.xaml
    /// </summary>
    public partial class ExpertiseWorkPospelov : Window
    {
        public ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        ServiceReference1.view_expertise_pospelov v = new ServiceReference1.view_expertise_pospelov();
        ObservableCollection<DataTable> ls_dt = new ObservableCollection<DataTable>();
        CLoading Loading;
        public int id_expertises;
        public int id_expert;
        public ExpertiseWorkPospelov()
        {
            InitializeComponent();
            Loading = new CLoading(circle);
            client.GetListExpertisePospelovCompleted += Client_GetListExpertisePospelovCompleted;
            client.AddMarkPospelovCompleted += Client_AddMarkPospelovCompleted;
            client.AddStatusExpertiseExpertCompleted += Client_AddStatusExpertiseExpertCompleted;
        }

        private void Client_AddMarkPospelovCompleted(object sender, ServiceReference1.AddMarkPospelovCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("Все записано");
                client.AddStatusExpertiseExpertAsync(id_expertises, id_expert);
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_AddStatusExpertiseExpertCompleted(object sender, ServiceReference1.AddStatusExpertiseExpertCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if(e.Result != -1)
                {
                    MessageBox.Show("Вы были последним");
                    this.DialogResult = true;
                }
                
               
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_GetListExpertisePospelovCompleted(object sender, ServiceReference1.GetListExpertisePospelovCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Error == null)
            {
                v.id_expertise = e.Result.id_expertise;
                v.list_factors = e.Result.list_factors;
                v.list_ribs = e.Result.list_ribs;
                v.list_marks_ribs = e.Result.list_marks_ribs;
                v.list_marks_factors = e.Result.list_marks_factors;
                var list_priority = v.list_factors.Select(o=>o.priority).GroupBy(o=>o).OrderBy(o=>o.Key).ToList();
                for (int i = 0; i < list_priority.Count()-1; i++)
                {
                    DataGrid dataGrid = new DataGrid();
                    dataGrid.CanUserAddRows = false;
                    dataGrid.IsReadOnly = false;
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
                    DataTable dt = new DataTable();
                    dt.Columns.Add("факторы");
                    int number_row = 0;
                    for(int j=0;j<v.list_factors.Count();j++)
                    {
                        if (v.list_factors[j].priority == list_priority[i].Key)
                        {
                            dt.Columns.Add(v.list_factors[j].name);
                        }
                        else if(v.list_factors[j].priority == list_priority[i].Key+1 )
                        {
                            DataRow r = dt.NewRow();
                            dt.Rows.Add(r);
                            dt.Rows[number_row][0] = v.list_factors[j].name;
                            for(int k=1;k<dt.Columns.Count;k++)
                            {
                                int id_fact_col = v.list_factors.Where(o => o.name == dt.Columns[k].ColumnName).FirstOrDefault().id_factor;
                                int id_rib = v.list_ribs.Where(o => o.id_factor_from == id_fact_col && o.id_factor_in == v.list_factors[j].id_factor).FirstOrDefault().id_rib;
                                if(v.list_marks_ribs.Count !=0)
                                {
                                    var value_rib = v.list_marks_ribs.Where(o => o.id_rib == id_rib).FirstOrDefault().value;
                                    if (value_rib != -1)
                                    {
                                        dt.Rows[number_row][k] = Math.Round(value_rib * 100);
                                    }
                                }
                            }
                            number_row++;
                        }
                    }
                    ls_dt.Add(dt);
                    dataGrid.ItemsSource = dt.DefaultView;
                    splMain.Children.Add(dataGrid);
                }
                Button button = new Button();
                button.Content = "сохранить";
                button.Click += btnSave_click;
                splMain.Children.Add(button);
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void btnSave_click(object sender, RoutedEventArgs e)
        {
            bool triger = true;
            for(int k=0;k<ls_dt.Count();k++)
            {
                if(triger == false)
                {
                    break;
                }
                for (int i = 1; i < ls_dt[k].Columns.Count; i++)
                {
                    float count = 0;
                    for (int j = 0; j < ls_dt[k].Rows.Count; j++)
                    {
                        count += float.Parse(ls_dt[k].Rows[j][i].ToString());
                        //ls_dt[k].Rows[j][i] = float.Parse(ls_dt[k].Rows[j][i].ToString()) / 100;
                        if (count > 100)
                        {
                            MessageBox.Show("В столбце " + ls_dt[k].Rows[i] + " сумма весов больше 100");
                            triger = false;
                            break;
                        }
                    }
                    if (count < 100)
                    {
                        triger = false;
                        MessageBox.Show("В столбце " + ls_dt[k].Columns[i] + " сумма весов меньше 100");
                        break;
                    }
                }
            }
            if (triger == true)
            {
                Dictionary<int, float> dic_ribs_val = new Dictionary<int, float>();
                Dictionary<int, float> dic_fact_val = new Dictionary<int, float>();
                for (int k = 0; k < ls_dt.Count(); k++)
                {
                    for (int i = 0; i < ls_dt[k].Rows.Count; i++)
                    {
                        float value = 0;
                        int id_fact_in = v.list_factors.Where(o => o.name == ls_dt[k].Rows[i][0].ToString()).First().id_factor;
                        for (int j = 1; j < ls_dt[k].Columns.Count; j++)
                        {
                            int id_fact_from = v.list_factors.Where(o => o.name == ls_dt[k].Columns[j].ToString()).First().id_factor;
                            int prior = v.list_factors.Where(o => o.name == ls_dt[k].Columns[j].ToString()).First().priority;
                            if(prior == 0)
                            {
                                float t = ls_dt[k].Columns.Count - 1;
                                float weght = 1 / t;
                                if(dic_fact_val.ContainsKey(id_fact_from) == false)
                                {
                                    dic_fact_val.Add(id_fact_from, weght);
                                }
                                int id_rib = v.list_ribs.Where(o => o.id_factor_from == id_fact_from && o.id_factor_in == id_fact_in).First().id_rib;
                                float temp_value = float.Parse(ls_dt[k].Rows[i][j].ToString()) / 100;
                                dic_ribs_val.Add(id_rib, temp_value);
                                value += (temp_value*weght);
                            }
                            else
                            {
                                float weght = dic_fact_val[id_fact_from];
                                int id_rib = v.list_ribs.Where(o => o.id_factor_from == id_fact_from && o.id_factor_in == id_fact_in).First().id_rib;
                                float temp_value = float.Parse(ls_dt[k].Rows[i][j].ToString()) / 100;
                                dic_ribs_val.Add(id_rib, temp_value);
                                value += (temp_value * weght);
                            }
                        }
                        dic_fact_val.Add(id_fact_in, value);
                    }
                }
                client.AddMarkPospelovAsync(id_expertises, id_expert, dic_ribs_val, dic_fact_val);

            }

        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
    }
}
