using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace test
{
    /// <summary>
    /// Логика взаимодействия для СompletedExpertises.xaml
    /// </summary>
    public partial class СompletedExpertises : System.Windows.Window
    {
        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        CLoading cLoading;
        public int id_expertises;
        public СompletedExpertises()
        {
            InitializeComponent();
            cLoading = new CLoading(circle);
            client.GetListCompletedExpertisesCompleted += Client_GetListCompletedExpertisesCompleted;
            client.GetListCompletedExpertisesAsync();
            client.GetListExpertiseReportCompleted += Client_GetListExpertiseReportCompleted;
            client.GetListExpertiseReportPospelovCompleted += Client_GetListExpertiseReportPospelovCompleted;
            cLoading.start();
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        //формируем отчет в ексель по методу поспелова
        private void Client_GetListExpertiseReportPospelovCompleted(object sender, ServiceReference1.GetListExpertiseReportPospelovCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                Excel.Application xlApp = new Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet = null;
                object misValue = System.Reflection.Missing.Value;
                xlWorkBook = xlApp.Workbooks.Add();
               
                var experts = e.Result.list_marks_factors.GroupBy(o => o.id_expert).ToList();
                for (int i = 0; i < experts.Count(); i++)
                {
                    int id_expert = experts[i].Key;
                    if (i == 0)
                    {
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                        xlWorkSheet.Name = "Отчет эксперта " + e.Result.list_marks_factors.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name;
                        //e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name + " " + e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.second_name + " " + e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.patronymic;

                    }
                    else
                    {
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Type.Missing, xlWorkSheet, Type.Missing, Type.Missing);
                        xlWorkSheet.Name = "Отчет эксперта " + e.Result.list_marks_factors.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name;

                    }
                    var ls_marks_factors_expert = e.Result.list_marks_factors.Where(o => o.id_expert == id_expert).OrderBy(o=>o.factors.priority).ToList();
                    var ls_marks_ribs_expert = e.Result.list_marks_ribs.Where(o => o.id_expert == id_expert).OrderBy(o => o.ribs.factors.priority).ToList();
                    var list_priority = e.Result.list_marks_factors.Select(o => o.factors.priority).GroupBy(o => o).OrderBy(o => o.Key).ToList();
                    int number_row = 0;
                    

                    for (int j = 0; j < list_priority.Count()-1; j++)
                    {
                        int number_col = 1;
                        number_row++ ;
                        int count_pr_row = ls_marks_factors_expert.Where(o => o.factors.priority == list_priority[j].Key + 1).Count();
                        for (int t = 0; t < ls_marks_factors_expert.Count(); t++)
                        {
                            //if (ls_marks_factors_expert[t].factors.priority == 0)
                            //{
                            //    number_col++;
                            //    xlWorkSheet.Cells[1,number_col] = ls_marks_factors_expert[t].factors.name
                                
                            //}
                             if(ls_marks_factors_expert[t].factors.priority == list_priority[j].Key )
                            {
                                number_col++;
                                xlWorkSheet.Cells[number_row , number_col ] = ls_marks_factors_expert[t].factors.name + "\n"+ " " + Math.Round(ls_marks_factors_expert[t].value * 100);

                                //number_row++;
                            }
                            else if (ls_marks_factors_expert[t].factors.priority == list_priority[j].Key + 1)
                            {
                                number_row++;
                                //xlWorkSheet.Cells.Style.WrapText = true;
                                xlWorkSheet.Cells[number_row, 1] = ls_marks_factors_expert[t].factors.name + "\n" + Math.Round(ls_marks_factors_expert[t].value * 100);
                                var values = ls_marks_ribs_expert.Where(o => o.ribs.id_factor_in == ls_marks_factors_expert[t].factors.id_factor).ToList();
                                for(int l = 0; l < values.Count(); l++)
                                {
                                    xlWorkSheet.Cells[number_row, 2 + l] = Math.Round(values[l].value * 100);
                                }
                               //var mark_rib = ls_marks_ribs_expert.Where(o=>o.ribs.factors.name == )
                                
                            }
                        }
                        number_row++;
                    }

                    //Постройка диаграммы
                    int last_priority2 = list_priority.Max(o => o.Key);
                    var ls_res_fact_last2 = e.Result.list_marks_factors.Where(o => o.factors.priority == last_priority2 && o.id_expert == id_expert).OrderBy(o => o.factors.priority).ToList();
                    List<double> masY2 = new List<double>();
                    List<string> masX2 = new List<string>();
                    for (int t = 0; t < ls_res_fact_last2.Count(); t++)
                    {
                        masX2.Add(ls_res_fact_last2[t].factors.name);
                        masY2.Add(Math.Round(ls_res_fact_last2[t].value * 100));
                    }
                    //строим диаграмму
                    Excel.ChartObjects _xlCharts2 = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject _myChart2 = (Excel.ChartObject)_xlCharts2.Add(10 * last_priority2, 80, 300, 250);
                    Excel.Chart _chartPage2 = _myChart2.Chart;

                    SeriesCollection _seriesCollection2 = (SeriesCollection)_chartPage2.SeriesCollection(Type.Missing);
                    Series _series2 = _seriesCollection2.NewSeries();
                    _series2.XValues = masX2.ToArray();
                    _series2.Values = masY2.ToArray();
                    _chartPage2.ApplyLayout(2, _chartPage2.ChartType);
                    _chartPage2.Legend.LegendEntries(_chartPage2.Legend.LegendEntries().Count).Delete();
                    _chartPage2.ChartTitle.Text = "Результат";
                }
                //Строим итоговую таблицу
                var ls_results_factors = e.Result.list_results_factors.OrderBy(o => o.factors.priority).ToList();
                var ls_results_ribs_expert = e.Result.list_results_ribs.OrderBy(o => o.ribs.factors.priority).ToList();
                var list_priority_result = e.Result.list_results_factors.Select(o => o.factors.priority).GroupBy(o => o).OrderBy(o => o.Key).ToList();
                int number_row2 = 0;
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Type.Missing, xlWorkSheet, Type.Missing, Type.Missing);
                xlWorkSheet.Name = "Отчет общий ";
                for (int j = 0; j < list_priority_result.Count() - 1; j++)
                {
                    int number_col = 1;
                    number_row2++;
                    int count_pr_row = ls_results_factors.Where(o => o.factors.priority == list_priority_result[j].Key + 1).Count();
                    for (int t = 0; t < ls_results_factors.Count(); t++)
                    {
                        if (ls_results_factors[t].factors.priority == list_priority_result[j].Key)
                        {
                            number_col++;
                            xlWorkSheet.Cells[number_row2, number_col] = ls_results_factors[t].factors.name + "\n" +  Math.Round(ls_results_factors[t].value * 100);

                            //number_row++;
                        }
                        else if (ls_results_factors[t].factors.priority == list_priority_result[j].Key + 1)
                        {
                            number_row2++;
                            //xlWorkSheet.Cells.Style.WrapText = true;
                            xlWorkSheet.Cells[number_row2, 1] = ls_results_factors[t].factors.name + "\n" + Math.Round(ls_results_factors[t].value * 100);
                            var values = ls_results_ribs_expert.Where(o => o.ribs.id_factor_in == ls_results_factors[t].factors.id_factor).ToList();
                            for (int l = 0; l < values.Count(); l++)
                            {
                                xlWorkSheet.Cells[number_row2, 2 + l] = Math.Round(values[l].value * 100);
                            }
                            //var mark_rib = ls_marks_ribs_expert.Where(o=>o.ribs.factors.name == )

                        }
                    }
                    number_row2++;
                }
                
               
                int last_priority = list_priority_result.Max(o=>o.Key);
                var ls_res_fact_last = e.Result.list_results_factors.Where(o => o.factors.priority == last_priority).OrderBy(o => o.factors.priority).ToList();
                List<double> masY = new List<double>();
                List<string> masX = new List<string>();
                for (int i=0;i<ls_res_fact_last.Count();i++)
                {
                    masX.Add(ls_res_fact_last[i].factors.name);
                    masY.Add(Math.Round(ls_res_fact_last[i].value * 100));
                }
                //строим диаграмму
                Excel.ChartObjects _xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                Excel.ChartObject _myChart = (Excel.ChartObject)_xlCharts.Add(10* last_priority, 80, 300, 250);
                Excel.Chart _chartPage = _myChart.Chart;

                SeriesCollection _seriesCollection = (SeriesCollection)_chartPage.SeriesCollection(Type.Missing);
                Series _series = _seriesCollection.NewSeries();
                _series.XValues = masX.ToArray();
                _series.Values = masY.ToArray();
                _chartPage.ApplyLayout(2, _chartPage.ChartType);
                _chartPage.Legend.LegendEntries(_chartPage.Legend.LegendEntries().Count).Delete();
                _chartPage.ChartTitle.Text = "Результат";
                //chartPage.ApplyDataLabels(XlDataLabelsType.xlDataLabelsShowValue, false, true, false, false, false, false, true, false, false);
                //_chartPage.ChartType = Excel.XlChartType.xlPie;


                //Excel.Range cellRange = (Excel.Range)xlWorkSheet.Cells[1, 1];
                //Excel.Range rowRange = cellRange.EntireRow;
                //rowRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
                //xlWorkSheet.Cells[1,1] = "awdwad";
                //xlWorkSheet.Columns.AutoFit();
                xlWorkSheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlWorkSheet.Cells.Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                SaveFileDialog sFile = new SaveFileDialog();
                sFile.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                try
                {

                    if (sFile.ShowDialog() == true)
                    {
                        string path = sFile.FileName;
                        xlWorkBook.Application.DisplayAlerts = false;
                        //MessageBox.Show(path);
                        xlWorkBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        //xlApp.Visible = true;
                        xlWorkBook.Close(true, misValue, misValue);
                        xlApp.Quit();
                        MessageBox.Show("Ексель файл создан. Вы можете найти его по пути  " + path);
                    }
                    Marshal.ReleaseComObject(xlWorkSheet);
                    Marshal.ReleaseComObject(xlWorkBook);
                    Marshal.ReleaseComObject(xlApp);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.Message);
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
            cLoading.stop();
        }
        //формируем отчет в ексель по методу паттерна
        private void Client_GetListExpertiseReportCompleted(object sender, ServiceReference1.GetListExpertiseReportCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Excel.Application xlApp = new Excel.Application();
                
                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet = null;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add();
                var experts = e.Result.list_marks.GroupBy(o => o.id_expert).ToList();
                int count_project = e.Result.list_marks.GroupBy(o => o.id_project).ToList().Count();
                var count_criterion = e.Result.list_marks.GroupBy(o => o.id_criterion).ToList().Count();
                for (int i = 0; i < experts.Count(); i++)
                {
                   
                    int id_expert = experts[i].Key;
                    if (i == 0)
                    {
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                        xlWorkSheet.Name = "Отчет эксперта "+ e.Result.list_marks.Where(o=>o.id_expert == id_expert).FirstOrDefault().experts.first_name;
                        //e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name + " " + e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.second_name + " " + e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.patronymic;

                    }
                    else
                    {
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Type.Missing, xlWorkSheet , Type.Missing, Type.Missing);
                        xlWorkSheet.Name = "Отчет эксперта " + e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name;

                    }
                    var ls_result_expert = e.Result.list_res_ex.Where(o => o.id_expert == id_expert).OrderBy(o=>o.id_project).ToList();
                    var ls_marks = e.Result.list_marks.Where(o => o.id_expert == id_expert).ToList();
                    var ls_criterion = e.Result.list_marks.Where(o => o.id_expert == id_expert).OrderBy(o => o.id_criterion).GroupBy(o => o.id_criterion).ToList();
                    
                    for(int j = 0; j< ls_criterion.Count(); j++)
                    {
                        int id_criterion = ls_criterion[j].Key;
                        if(j == 0)
                        {
                            xlWorkSheet.Cells[1, 1] = "Критерии";
                            var projects = ls_marks.OrderBy(o => o.id_project).GroupBy(o => o.id_project).ToList();
                            for(int k=0;k<projects.Count();k++)
                            {
                                string name_project = ls_marks.Where(o => o.id_project == projects[k].Key).FirstOrDefault().projects.name;
                                xlWorkSheet.Cells[1, k+1+1] = name_project;
                               
                            }
                        }
                        string name_criterion = ls_marks.Where(o => o.id_criterion == id_criterion).FirstOrDefault().criterions.name;
                        var values = ls_marks.Where(o => o.id_criterion == id_criterion).OrderBy(o => o.id_project).ToList();
                        for(int k = 0; k<values.Count(); k++)
                        {
                            xlWorkSheet.Cells[j + 2, 2+k] = Math.Round(values[k].value * 100);
                        }
                        xlWorkSheet.Cells[j+2, 1] = name_criterion;
                    }
                    
                    xlWorkSheet.Cells[count_criterion + 2, 1] = "ИТОГ";
                    for (int j = 0; j < ls_result_expert.Count(); j++)
                    {
                        xlWorkSheet.Cells[count_criterion + 2, j + 2] = Math.Round(ls_result_expert[j].value * 100); 
                    }
                    xlWorkSheet.Columns.AutoFit();
                    //строим диаграмму
                    Excel.ChartObjects _xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject _myChart = (Excel.ChartObject)_xlCharts.Add(10, 80, 300, 250);
                    Excel.Chart _chartPage = _myChart.Chart;

                    SeriesCollection _seriesCollection = (SeriesCollection)_chartPage.SeriesCollection(Type.Missing);
                    Series _series = _seriesCollection.NewSeries();
                    _series.XValues = xlWorkSheet.get_Range("B1", xlWorkSheet.Cells[1, 1 + count_project] as Range);
                    _series.Values = xlWorkSheet.get_Range(xlWorkSheet.Cells[2 + count_criterion, 2] as Range, xlWorkSheet.Cells[2 + count_criterion, 1 + count_project] as Range);
                    _chartPage.ApplyLayout(2, _chartPage.ChartType);
                    _chartPage.ChartTitle.Text = "Отчет("+ e.Result.list_marks.Where(o => o.id_expert == id_expert).FirstOrDefault().experts.first_name+")";
                    //chartPage.ApplyDataLabels(XlDataLabelsType.xlDataLabelsShowValue, false, true, false, false, false, false, true, false, false);
                    _chartPage.ChartType = Excel.XlChartType.xlPie;

                }
                //строим итоговую таблицу
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(Type.Missing, xlWorkSheet, Type.Missing, Type.Missing);
                xlWorkSheet.Name = "Отчет общий";
                
                var ls_expertise_crt = e.Result.list_res_ex_cr.ToList();
                for(int i = 0;i < ls_expertise_crt.Count(); i++)
                {
                    xlApp.Columns.AutoFit();
                    var ls_criterion = ls_expertise_crt.OrderBy(o => o.id_criterion).GroupBy(o => o.id_criterion).ToList();

                    for (int j = 0; j < ls_criterion.Count(); j++)
                    {
                        int id_criterion = ls_criterion[j].Key;
                        if (j == 0)
                        {
                            xlWorkSheet.Cells[1, 1] = "Критерии";
                            var projects = ls_expertise_crt.OrderBy(o => o.id_project).GroupBy(o => o.id_project).ToList();
                            for (int k = 0; k < projects.Count(); k++)
                            {
                                string name_project = ls_expertise_crt.Where(o => o.id_project == projects[k].Key).FirstOrDefault().projects.name;
                                xlWorkSheet.Cells[1, k + 1 + 1] = name_project;

                            }
                        }
                        string name_criterion = ls_expertise_crt.Where(o => o.id_criterion == id_criterion).FirstOrDefault().criterions.name;
                        var values = ls_expertise_crt.Where(o => o.id_criterion == id_criterion).OrderBy(o => o.id_project).ToList();
                        for (int k = 0; k < values.Count(); k++)
                        {
                            xlWorkSheet.Cells[j + 2, 2 + k] = Math.Round(values[k].value * 100);
                        }
                        xlWorkSheet.Cells[j + 2, 1] = name_criterion;
                    }
                    var ls_result_all = e.Result.list_res_exppertise.ToList();
                    
                    xlWorkSheet.Cells[count_criterion + 2, 1] = "ИТОГ";
                    for (int j = 0; j < ls_result_all.Count(); j++)
                    {
                        xlWorkSheet.Cells[count_criterion + 2, j + 2] = Math.Round(ls_result_all[j].value * 100);
                    }
                }

                Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
                Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, count_criterion * 25, 300, 250);
                Excel.Chart chartPage = myChart.Chart;

               

                SeriesCollection seriesCollection = (SeriesCollection)chartPage.SeriesCollection(Type.Missing);
                Series series = seriesCollection.NewSeries();
                series.XValues = xlWorkSheet.get_Range("B1", xlWorkSheet.Cells[1, 1 + count_project] as Range);
                series.Values = xlWorkSheet.get_Range(xlWorkSheet.Cells[2 + count_criterion, 2] as Range, xlWorkSheet.Cells[2 + count_criterion, 1 + count_project] as Range);
                chartPage.ApplyLayout(2, chartPage.ChartType);
                chartPage.ChartTitle.Text = "Отчет(Общий)";
                //chartPage.ApplyDataLabels(XlDataLabelsType.xlDataLabelsShowValue, false, true, false, false, false, false, true, false, false);
                chartPage.ChartType = Excel.XlChartType.xlPie;
                SaveFileDialog sFile = new SaveFileDialog();

                
                sFile.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                try
                {

                    if (sFile.ShowDialog() == true)
                    {
                        string  path = sFile.FileName;
                        xlWorkBook.Application.DisplayAlerts = false;
                        //MessageBox.Show(path);
                        xlWorkBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        //xlApp.Visible = true;
                        xlWorkBook.Close(true, misValue, misValue);
                        xlApp.Quit();
                        MessageBox.Show("Ексель файл создан. Вы можете найти его по пути  " + path);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.Message);
                }


                //xlWorkBook.Close(false, false, false);
                //xlApp.Quit();
                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();


            }
            else
                MessageBox.Show(e.Error.Message);
            cLoading.stop();
        }

        private void Client_GetListCompletedExpertisesCompleted(object sender, ServiceReference1.GetListCompletedExpertisesCompletedEventArgs e)
        {
            cLoading.stop();
            if (e.Error == null)
            {
                dgCompletedExpertises.ItemsSource = e.Result.ToList();

            }
            else
                MessageBox.Show(e.Error.Message);
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }
        private void bt_Report_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference1.expertises temp = dgCompletedExpertises.SelectedItem as ServiceReference1.expertises;
            if(temp.type == 1)
            {
                cLoading.start();
                client.GetListExpertiseReportAsync(temp.id_expertise);
            }
            else
            {
                cLoading.start();
                client.GetListExpertiseReportPospelovAsync(temp.id_expertise);
            }

        }
    }
}
