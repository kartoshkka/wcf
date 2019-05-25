using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Test_windows.xaml
    /// </summary>
    public partial class Test_windows : Window
    {
        
        //ObservableCollection<ObservableCollection<CObject>> ls = new ObservableCollection<ObservableCollection<CObject>>();
       
        DataTable dt = new DataTable();
        //List<DataGrid> dataGrids = new List<DataGrid>();
        ObservableCollection<DataTable> ls_dt = new ObservableCollection<DataTable>();
        public Test_windows()
        {
            InitializeComponent();
        }


        private void btnAddMore_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in ls_dt)
            {
               for(int i =0;i<temp.Rows.Count;i++)
                {
                    MessageBox.Show(temp.Rows[i][0].ToString());
                }
                   
            }


        }
        //формируем цели
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.AutoGenerateColumns = true;
            dataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
            dt = new DataTable();
            dt.Columns.Add("Цели");
           
            ls_dt.Add(dt);
            

            dataGrid.ItemsSource = dt.DefaultView;
            splMain.Children.Add(dataGrid);
         
        }
        //формируем уровень
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.AutoGenerateColumns = true;

            dt = new DataTable();

            dt.Columns.Add("факторы " +ls_dt.Count() + " уровня");
           
            ls_dt.Add(dt);

        
            dataGrid.ItemsSource = dt.DefaultView;
            dataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
            
            splMain.Children.Add(dataGrid);
        }
    }
}
