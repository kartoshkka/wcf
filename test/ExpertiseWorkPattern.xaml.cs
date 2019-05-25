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

namespace test
{
    /// <summary>
    /// Логика взаимодействия для ExpertiseWork.xaml
    /// </summary>
    
    public partial class ExpertiseWork : Window
    {
        public ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        ServiceReference1.view_expertise_criterios v = new ServiceReference1.view_expertise_criterios();
        DataTable dt = new DataTable();
        CLoading Loading;
        public int id_expertises;
        public int id_expert;
        public List<int> ls_int = new List<int>();
        public ExpertiseWork()
        {
            InitializeComponent();
            
            Loading = new CLoading(circle);
            client.GetListExpertisesCriterionsCompleted += Client_GetListExpertisesCriterionsCompleted;
            client.AddMarkCompleted += Client_AddMarkCompleted;
            client.AddResultExpertCompleted += Client_AddResultExpertCompleted;
            client.AddStatusExpertiseExpertCompleted += Client_AddStatusExpertiseExpertCompleted;
            client.AddStatusExpertiseCompleted += Client_AddStatusExpertiseCompleted;
            Loading.start();
        }

        private void Client_AddStatusExpertiseCompleted(object sender, ServiceReference1.AddStatusExpertiseCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if(e.Result != -1)
                {
                    MessageBox.Show("Вы были последним кто прошел эту экспертизу. Экспертиза завершена всеми экспертами.");
                    this.DialogResult = true;
                }
               
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
                if (e.Result != -1)
                {
                    MessageBox.Show("Вы были последним кто прошел эту экспертизу. Экспертиза завершена всеми экспертами.");
                    this.DialogResult = true;
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_AddResultExpertCompleted(object sender, ServiceReference1.AddResultExpertCompletedEventArgs e)
        {
            if (e.Error == null)
            {

            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_AddMarkCompleted(object sender, ServiceReference1.AddMarkCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ls_int.Add(e.Result);
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void Client_GetListExpertisesCriterionsCompleted(object sender, ServiceReference1.GetListExpertisesCriterionsCompletedEventArgs e)
        {
            Loading.stop();
            if (e.Error == null)
            {
                dt = new DataTable();
                
                v.id_expertise = e.Result.id_expertise;
                v.list_ex_cr = e.Result.list_ex_cr;
                v.list_pr_ex = e.Result.list_pr_ex;
                v.list_marks = e.Result.list_marks;
                //MessageBox.Show(v.list_pr_ex[0].projects.name);
                dt.Columns.Add("критерии");
                for (int i = 0; i < v.list_pr_ex.Count(); i++)
                {
                    dt.Columns.Add(v.list_pr_ex[i].projects.name);
                }
                //dt.Columns.Add("id_pr" + v.list_pr_ex[i].id_project);

                for (int i = 0; i < v.list_ex_cr.Count(); i++)
                {

                    DataRow r = dt.NewRow();
                    r[0] = v.list_ex_cr[i].criterions.name;
                    for(int j = 1; j < v.list_pr_ex.Count()+1;j++)
                    {
                        var value = v.list_marks.Where(o => o.id_project == v.list_pr_ex[j-1].id_project && o.id_criterion == v.list_ex_cr[i].id_criterion).FirstOrDefault();
                        if(value != null)
                        {
                            r[j] =Math.Round(value.value*100);
                        }
                        else
                        {
                            r[j] = 0;
                        }
                    }
                   
                    dt.Rows.Add(r);
                }

                dgExpertises.ItemsSource = dt.DefaultView;
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

        private void SaveResultClick(object sender, RoutedEventArgs e)
        {
            bool trigger = true;
            for (int i=0;i<dt.Rows.Count;i++)
            {
                float count = 0;
               
                for(int j=1;j<dt.Columns.Count;j++)
                {
                    count += float.Parse(dt.Rows[i][j].ToString());
                    dt.Rows[i][j] = float.Parse(dt.Rows[i][j].ToString()) / 100;
                    if (count >100)
                    {
                        MessageBox.Show("В строке критерия "+ dt.Rows[i][0]+ " сумма весов больше 100");
                        trigger = false;
                        break;
                    }
                }
                if(count <100)
                {
                    trigger = false;
                    MessageBox.Show("В строке критерия " + dt.Rows[i][0] + " сумма весов меньше 100");
                }
                if(trigger == false)
                {
                    break;
                }
            }

            if (trigger == true)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    int id_project = v.list_pr_ex.Where(o => o.projects.name == dt.Columns[j].ToString()).First().id_project;
                    float sum = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int id_criterion = v.list_ex_cr.Where(o => o.criterions.name == dt.Rows[i][0].ToString()).First().id_criterion;
                        float weght = float.Parse(v.list_ex_cr.Where(o => o.criterions.name == dt.Rows[i][0].ToString()).First().weight.ToString());
                        float value = float.Parse(dt.Rows[i][j].ToString());
                        sum += weght * value;
                        client.AddMarkAsync(id_expertises, id_expert, id_project, id_criterion, value);
                    }
                    client.AddResultExpertAsync(id_expertises, id_expert, id_project, sum);
                }
                client.AddStatusExpertiseExpertAsync(id_expertises, id_expert);
            }
            client.GetListExpertisesCriterionsAsync(id_expertises, id_expert);
            //client.AddStatusExpertiseAsync(id_expertises);
            Loading.start();
            
        }
    }
}
