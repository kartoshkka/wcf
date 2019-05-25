using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IService1
    {
        #region Методы получения данных с БД
        //Получить всех экспертов
        [OperationContract]
        List<experts> GetListExperts();
        //Получить все критерии
        [OperationContract]
        List<criterions> GetListCriterions();
        //Получить все проекты
        [OperationContract]
        List<projects> GetListProjects();
        //Получить всех текущие экспертиз
        [OperationContract]
        List<expertises> GetListCurrentExpertises(int id_expert);
        //Получить все завершенные экспертизы
        [OperationContract]
        List<expertises> GetListCompletedExpertises();
        //Получить экспертизу для ее прохождения
        [OperationContract]
        view_expertise_criterios GetListExpertisesCriterions(int id_expertise, int id_expert);
        //Получить экспертизу для вывода ее отчета
        [OperationContract]
        view_completed_expertise GetListExpertiseReport(int id_expertise);
        //Получить экспертизу для вывода ее отчета pospelov
        [OperationContract]
        view_completed_expertise_pospelov GetListExpertiseReportPospelov(int id_expertise);
        //Получить экспертизу Поспелова для ее прохождения
        [OperationContract]
        view_expertise_pospelov GetListExpertisePospelov(int id_expertise,int id_expert);
        //Получить экспертизу Поспелова для ее прохождения
        [OperationContract]
        experts GetIdExpert(string login,string password);
        #endregion

        #region Методы добавления в БД
        //Добавить новую экспертизу pattern
        [OperationContract]
        int AddExperisePattern(string name,List<int> ls_id_projects,List<int> ls_id_experts,Dictionary<int,float> dc_weight_criteries);
        //Добавить новую экспертизу поспелов
       [ServiceKnownType(typeof(List<List<string>>))]
         [OperationContract]
        int AddExperisePospelov(string name, List<int> ls_id_experts,List<List<string>> ls_fac );
        //Добавить нового эксперта
        [OperationContract]
        int AddExpert(string first_name, string second_name, string patronymic, string login, string password,int accses);
        //Добавить новый критерий
        [OperationContract]
        int AddCriterion(string name);
        //Добавить новый проект
        [OperationContract]
        int AddProject(string name);
        //Добавить новую оценку
        [OperationContract]
        int AddMark(int id_expertise, int id_expert, int id_project, int id_criterion, double value);
        //Добавить новую оценку проекта
        [OperationContract]
        int AddResultExpert(int id_expertise, int id_expert, int id_project, double value);
        //Добавить статус эксперта в экспертизе
        [OperationContract]
        int AddStatusExpertiseExpert(int id_expertise, int id_expert);
        //Добавить статуса в экспертизе
        [OperationContract]
        int AddStatusExpertise(int id_expertise);
        //Добавить новую оценку эксперта поспелов
        [OperationContract]
        int AddMarkPospelov(int id_expertise, int id_expert, Dictionary<int,float> dic_ribs_val, Dictionary<int, float> dic_facs_val);
        //Добавить статус экспертизе поспелов
        [OperationContract]
        int AddResultMarkPospelov(int id_expertise);
        #endregion

        #region Методы Редактирования в БД
        //Изменить критерий
        [OperationContract]
        int EditCriterion(int id_criterion,string name);
        //Изменить эксперта
        [OperationContract]
        int EditExpert(int id_expert, string first_name, string second_name, string patronymic, string login, string password,int accses);
        //Изменить эксперта
        [OperationContract]
        int EditProject(int id_project, string name);
        #endregion
        


        // TODO: Добавьте здесь операции служб
    }
    

    // Используйте контракт данных, как показано в примере ниже, чтобы добавить составные типы к операциям служб.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    
    //класс для отображения экспертизы Pattern
     public class view_expertise_criterios
    {
        public int id_expertise { get; set; }
        public List<projects_expertises> list_pr_ex { get; set; }
        public List<expertises_criterions> list_ex_cr { get; set; }
        public List<marks> list_marks { get; set; }
    }
    //класс для отображения экспертизы поспелова
    public class view_expertise_pospelov
    {
        public int id_expertise { get; set; }
        public List<factors> list_factors { get; set; }
        public List<ribs> list_ribs { get; set; }
        public List<marks_factors> list_marks_factors { get; set; }
        public List<marks_ribs> list_marks_ribs { get; set; }
    }
    //класс для отображения завершенной экспертизы поспелова

    public class view_completed_expertise_pospelov
    {
        public int id_expertise { get; set; }
        public expertises expertises { get; set; }
        public List<marks_factors> list_marks_factors { get; set; }
        public List<marks_ribs> list_marks_ribs { get; set; }
        public List<results_factors> list_results_factors { get; set; }
        public List<results_ribs> list_results_ribs { get; set; }

    }
    //класс для отображения завершенной экспертизы паттерн
    public class view_completed_expertise
    {
        public int id_expertise { get; set; }
        public List<results_experts> list_res_ex { get; set; }
        public List<marks> list_marks { get; set; }
        public List<results_expertises_criterions> list_res_ex_cr { get; set; }
        public List<results_expertise> list_res_exppertise { get; set; }

    }
    
}
