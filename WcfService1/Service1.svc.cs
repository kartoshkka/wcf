using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service1 : IService1
    {
        public db_expertiseEntities db_Expertises = new db_expertiseEntities();
        #region Методы добавленния в БД
        //Метод добавления экспертизы pattern
        public int AddExperisePattern(string name, List<int> ls_id_projects, List<int> ls_id_experts, Dictionary<int, float> dc_weight_criteries)
        {
            try
            {
                
                expertises _expertises = new expertises();
                _expertises.name = name;
                _expertises.id_status = 1;
                _expertises.type = 1;
                db_Expertises.expertises.Add(_expertises);
                db_Expertises.SaveChanges();

                foreach(int temp in ls_id_projects)
                {
                    projects_expertises pr_ex = new projects_expertises();
                    pr_ex.id_expertise = _expertises.id_expertise;
                    pr_ex.id_project = temp;
                    db_Expertises.projects_expertises.Add(pr_ex);
                    db_Expertises.SaveChanges();
                }
                foreach (int temp in ls_id_experts)
                {
                    experts_expertises ex_ex = new experts_expertises();
                    ex_ex.id_expertise = _expertises.id_expertise;
                    ex_ex.id_expert = temp;
                    ex_ex.id_status = 1;
                    db_Expertises.experts_expertises.Add(ex_ex);
                    db_Expertises.SaveChanges();
                }
                foreach (var temp in dc_weight_criteries)
                {
                    expertises_criterions ex_cr = new expertises_criterions();
                    ex_cr.id_expertise = _expertises.id_expertise;
                    ex_cr.id_criterion = temp.Key;
                    ex_cr.weight = temp.Value;
                    db_Expertises.expertises_criterions.Add(ex_cr);
                    db_Expertises.SaveChanges();
                }

                return _expertises.id_expertise;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //Метод добавления экспертизы pospelov
        public int AddExperisePospelov(string name, List<int> ls_id_experts, List<List<string>> ls_fac /*List<int> ls_prior, List<string> ls_name_fact*/)
        {
            try
            {
                expertises _expertises = new expertises();
                _expertises.name = name;
                _expertises.id_status = 1;
                _expertises.type = 2;
                db_Expertises.expertises.Add(_expertises);
                db_Expertises.SaveChanges();

               
                foreach (int temp in ls_id_experts)
                {
                    experts_expertises ex_ex = new experts_expertises();
                    ex_ex.id_expertise = _expertises.id_expertise;
                    ex_ex.id_expert = temp;
                    ex_ex.id_status = 1;
                    db_Expertises.experts_expertises.Add(ex_ex);
                    db_Expertises.SaveChanges();
                }
                int level = 0;
                foreach (var temp in ls_fac)
                {
                    for(int i=0;i<temp.Count();i++)
                    {
                        factors fc = new factors();
                        fc.id_expertise = _expertises.id_expertise;
                        fc.name = temp[i];
                        fc.priority = level;
                        db_Expertises.factors.Add(fc);
                        db_Expertises.SaveChanges();
                    }
                    level++;
                }
                int max_priority = ls_fac.Count();
                level = 0;
                foreach (var temp in ls_fac)
                {
                    if(level != max_priority)
                    {
                        for (int i = 0; i < temp.Count(); i++)
                        {
                            string name_fact = temp[i];
                            var id_factor = db_Expertises.factors.Where(o => o.id_expertise == _expertises.id_expertise && o.name == name_fact).FirstOrDefault().id_factor;

                            List<factors> tmpFactors = db_Expertises.factors.Where(o => o.priority == level + 1 && o.id_expertise == _expertises.id_expertise).ToList();
                            foreach (var fact in tmpFactors)
                            {
                                ribs r = new ribs();
                                r.id_factor_from = id_factor;
                                r.id_factor_in = fact.id_factor;
                                db_Expertises.ribs.Add(r);
                                db_Expertises.SaveChanges();
                            }

                        }
                        level++;
                    }
                   
                    
                }
                return _expertises.id_expertise;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        //Метод добавления критерия
        public int AddCriterion(string name)
        {
            try
            {
                criterions C = new criterions();
                C.name = name;

                db_Expertises.criterions.Add(C);
                db_Expertises.SaveChanges();
                return C.id_criterion;
            }
            catch
            {
                return -1;
            }
        }
        //Метод добавления эксперта
        public int AddExpert(string first_name, string second_name, string patronymic, string login, string password,int accses)
        {
            try
            {
                experts exp = new experts();
                exp.first_name = first_name;
                exp.second_name = second_name;
                exp.patronymic =patronymic;
                exp.login = login;
                exp.access = accses;
                exp.password = password;

                db_Expertises.experts.Add(exp);
                db_Expertises.SaveChanges();
                return exp.id_expert;
            }
            catch
            {
                return -1;
            }
        }

        //Метод добавления эксперта
        public int AddProject(string name)
        {
            try
            {
                projects _projects = new projects();
                _projects.name = name;
               

                db_Expertises.projects.Add(_projects);
                db_Expertises.SaveChanges();
                return _projects.id_project;
            }
            catch
            {
                return -1;
            }
        }

        //Метод добавления оценки
        public int AddMark(int id_expertise, int id_expert, int id_project, int id_criterion, double value)
        {
            try
            {
                experts_expertises experts_Expertises = db_Expertises.experts_expertises.Where(o => o.id_expert == id_expert && o.id_expertise == id_expertise).FirstOrDefault();
                if(experts_Expertises != null && experts_Expertises.id_status == 2)
                {
                    marks _mark = db_Expertises.marks.Where(o => o.id_expert == id_expert && o.id_expertise == id_expertise && o.id_project == id_project && o.id_criterion == id_criterion).FirstOrDefault();
                    if (_mark != null)
                    {
                        _mark.id_expertise = id_expertise;
                        _mark.id_expert = id_expert;
                        _mark.id_project = id_project;
                        _mark.id_criterion = id_criterion;
                        _mark.value = value;

                        db_Expertises.SaveChanges();
                        return _mark.id_mark;
                    }
                    else
                    {
                        _mark = new marks();
                        _mark.id_expertise = id_expertise;
                        _mark.id_expert = id_expert;
                        _mark.id_project = id_project;
                        _mark.id_criterion = id_criterion;
                        _mark.value = value;
                        db_Expertises.marks.Add(_mark);
                        db_Expertises.SaveChanges();
                        return _mark.id_mark;
                        
                    }

                }
                else
                {
                    marks _mark = new marks();
                    _mark.id_expertise = id_expertise;
                    _mark.id_expert = id_expert;
                    _mark.id_project = id_project;
                    _mark.id_criterion = id_criterion;
                    _mark.value = value;
                    db_Expertises.marks.Add(_mark);
                    db_Expertises.SaveChanges();
                    return _mark.id_mark;
                }
               


                
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

         //Метод добавления оценки по проекту экспертом
        public int AddResultExpert(int id_expertise, int id_expert, int id_project, double value)
        {
            try
            {
                experts_expertises experts_Expertises = db_Expertises.experts_expertises.Where(o => o.id_expert == id_expert && o.id_expertise == id_expertise).FirstOrDefault();
                if (experts_Expertises != null && experts_Expertises.id_status == 2)
                {
                    results_experts res = db_Expertises.results_experts.Where(o => o.id_expert == id_expert && o.id_expertise == id_expertise && o.id_project == id_project).FirstOrDefault();
                    if(res != null)
                    {
                        res.id_expertise = id_expertise;
                        res.id_expert = id_expert;
                        res.id_project = id_project;
                        res.value = value;

                        db_Expertises.SaveChanges();
                        return res.id_result_expert;
                    }
                    else
                    {
                        res = new results_experts();
                        res.id_expertise = id_expertise;
                        res.id_expert = id_expert;
                        res.id_project = id_project;
                        res.value = value;
                        db_Expertises.results_experts.Add(res);
                        db_Expertises.SaveChanges();
                        return res.id_result_expert;
                    }
                    
                }
                else
                {
                    results_experts res = new results_experts();
                    res.id_expertise = id_expertise;
                    res.id_expert = id_expert;
                    res.id_project = id_project;
                    res.value = value;
                    db_Expertises.results_experts.Add(res);
                    db_Expertises.SaveChanges();
                    return res.id_result_expert;
                }
                   


                
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //Метод добавления статуса экспертизы для эксперта
        public int AddStatusExpertiseExpert(int id_expertise, int id_expert)
        {
            try
            {
                experts_expertises experts_Expertises = db_Expertises.experts_expertises.Where(o => o.id_expert == id_expert && o.id_expertise == id_expertise).FirstOrDefault();
                if (experts_Expertises != null && experts_Expertises.id_status == 1)
                {
                    experts_Expertises.id_status = 2;
                    db_Expertises.SaveChanges();
                    
                    int res = AddStatusExpertise(experts_Expertises.id_expertise);
                    return res;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //Метод добавления статуса экспертизы pattern
        public int AddStatusExpertise(int id_expertise)
        {
            try
            {
                List<experts_expertises> experts_Expertises = db_Expertises.experts_expertises.Where(o =>  o.id_expertise == id_expertise).ToList();
                if (experts_Expertises != null)
                {
                    bool triger = true;
                    foreach (experts_expertises ex_status in experts_Expertises)
                    {
                        if(ex_status.id_status == 1)
                        {
                            triger = false;
                        }
                    }
                    if(triger == true)
                    {
                        expertises _expertises = db_Expertises.expertises.Where(o => o.id_expertise == id_expertise).FirstOrDefault();
                        _expertises.id_status = 2;
                        db_Expertises.SaveChanges();
                        if(_expertises.type == 1)
                        {
                            List<expertises_criterions> ls_ex_cr = db_Expertises.expertises_criterions.Where(o => o.id_expertise == id_expertise).ToList();
                            List<results_experts> ls_rs_ex = db_Expertises.results_experts.Where(o => o.id_expertise == id_expertise).ToList();
                            List<marks> ls_marks = db_Expertises.marks.Where(o => o.id_expertise == id_expertise).ToList();

                            var criterions = db_Expertises.marks.Where(o => o.id_expertise == id_expertise).GroupBy(o => o.id_criterion).ToList();
                            var projects = db_Expertises.marks.Where(o => o.id_expertise == id_expertise).GroupBy(o => o.id_project).ToList();
                            int count_experts = db_Expertises.experts_expertises.Where(o => o.id_expertise == id_expertise).ToList().Count();
                            for (int i = 0; i < criterions.Count(); i++)
                            {
                                int id_criterion = criterions[i].Key;
                                double value = 0;
                                for (int j = 0; j < projects.Count(); j++)
                                {
                                    value = 0;
                                    int id_project = projects[j].Key;
                                    foreach (marks mark in ls_marks)
                                    {
                                        if (mark.id_project == id_project && mark.id_criterion == id_criterion)
                                        {
                                            value += mark.value;
                                        }
                                    }
                                    value = value / count_experts;
                                    results_expertises_criterions res = new results_expertises_criterions();
                                    res.id_criterion = id_criterion;
                                    res.id_expertise = id_expertise;
                                    res.id_project = id_project;
                                    res.value = value;
                                    db_Expertises.results_expertises_criterions.Add(res);
                                    db_Expertises.SaveChanges();
                                }
                            }
                            List<results_expertises_criterions> ls_res_ex_cr = db_Expertises.results_expertises_criterions.Where(o => o.id_expertise == id_expertise).ToList();
                            for (int i = 0; i < projects.Count(); i++)
                            {
                                int id_project = projects[i].Key;
                                double value = 0;
                                for (int j = 0; j < criterions.Count(); j++)
                                {
                                    int id_criterion = criterions[j].Key;
                                    var value_criterion = ls_res_ex_cr.Where(o => o.id_criterion == id_criterion && o.id_project == id_project).FirstOrDefault().value;
                                    double weight_criterion = ls_ex_cr.Where(o => o.id_criterion == id_criterion).FirstOrDefault().weight;
                                    value += value_criterion * weight_criterion;
                                }
                                results_expertise res = new results_expertise();
                                res.id_expertise = id_expertise;
                                res.id_project = id_project;
                                res.value = value;
                                db_Expertises.results_expertise.Add(res);
                                db_Expertises.SaveChanges();
                            }
                            return _expertises.id_expertise;
                        }
                        else
                        {
                            List<marks_factors> ls_fact = db_Expertises.marks_factors.Where(o => o.id_expertise == id_expertise).ToList();
                            if (ls_fact != null)
                            {
                                var ls_fact_group = ls_fact.GroupBy(o => o.id_factor).ToList();
                                
                                for (int i = 0; i < ls_fact_group.Count(); i++)
                                {
                                    int number_fact = 0;
                                    double value = 0;
                                    foreach (var fact in ls_fact)
                                    {
                                        if (fact.id_factor == ls_fact_group[i].Key)
                                        {
                                            value += fact.value;
                                            number_fact++;
                                        }
                                    }
                                    value = value / number_fact;
                                    results_factors res = new results_factors();
                                    res.id_expertise = id_expertise;
                                    res.id_factor = ls_fact_group[i].Key;
                                    res.value = value;
                                    db_Expertises.results_factors.Add(res);
                                    db_Expertises.SaveChanges();
                                }
                            }
                            List<marks_ribs> ls_ribs = db_Expertises.marks_ribs.Where(o => o.id_expertise == id_expertise).ToList();
                            if (ls_ribs != null)
                            {
                                var ls_fact_ribs = ls_ribs.GroupBy(o => o.id_rib).ToList();

                                for (int i = 0; i < ls_fact_ribs.Count(); i++)
                                {
                                    int number_fact = 0;
                                    double value = 0;
                                    foreach (var fact in ls_ribs)
                                    {
                                        if (fact.id_rib == ls_fact_ribs[i].Key)
                                        {
                                            value += fact.value;
                                            number_fact++;
                                        }
                                    }
                                    value = value / number_fact;
                                    results_ribs res = new results_ribs();
                                    res.id_expertise = id_expertise;
                                    res.id_rib = ls_fact_ribs[i].Key;
                                    res.value = value;
                                    db_Expertises.results_ribs.Add(res);
                                    db_Expertises.SaveChanges();
                                }
                            }
                            return id_expertise;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
       
        //добавление оценки экспертом экспертизе pospelov
        public int AddMarkPospelov(int id_expertise, int id_expert, Dictionary<int, float> dic_ribs_val, Dictionary<int, float> dic_facs_val)
        {
            try
            {
                foreach(var dic in dic_ribs_val)
                {
                    marks_ribs res = new marks_ribs();
                    res.id_expertise = id_expertise;
                    res.id_expert = id_expert;
                    res.id_rib = dic.Key;
                    res.value = dic.Value;
                    db_Expertises.marks_ribs.Add(res);
                    db_Expertises.SaveChanges();
                }
                foreach (var dic in dic_facs_val)
                {
                    marks_factors res = new marks_factors();
                    res.id_expertise = id_expertise;
                    res.id_expert = id_expert;
                    res.id_factor = dic.Key;
                    res.value = dic.Value;
                    db_Expertises.marks_factors.Add(res);
                    db_Expertises.SaveChanges();
                }
                return id_expertise;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //метод добавления статуса экспертизе поспелов
        public int AddResultMarkPospelov(int id_expertise)
        {
            try
            {
                List<experts_expertises> experts_Expertises = db_Expertises.experts_expertises.Where(o => o.id_expertise == id_expertise).ToList();
                if (experts_Expertises != null)
                {
                    bool triger = true;
                    foreach (experts_expertises ex_status in experts_Expertises)
                    {
                        if (ex_status.id_status == 1)
                        {
                            triger = false;
                        }
                    }
                    if(triger == true)
                    {
                       
                    }
                }
                return -1;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        #endregion

        #region Методы получения данных из БД


        //Метод получения Id эксперта
        public experts GetIdExpert(string login, string password)
        {
            try
            {
                experts _ex = db_Expertises.experts.Where(o=>o.login == login && o.password == password).FirstOrDefault();
                if(_ex != null)
                {
                    experts temp = new experts();
                    temp.id_expert = _ex.id_expert;
                    temp.login = _ex.login;
                    temp.password = _ex.password;
                    temp.first_name = _ex.first_name;
                    temp.second_name = _ex.second_name;
                    temp.patronymic = _ex.patronymic;
                    temp.access = _ex.access;
                    return temp;
                }
                else
                {
                    experts temp = new experts();
                    temp.id_expert = -1;
                    return temp;
                }
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                experts temp = new experts();
                temp.id_expert = -1;
                return temp;
            }
        }
        //Метод получения всех критериев
        public List<criterions> GetListCriterions()
        {
            try
            {
                List<criterions> result = new List<criterions>();
                List<criterions> tmpCriteries = db_Expertises.criterions.ToList();
                foreach (criterions criterions in tmpCriteries)
                {
                    criterions tmpC = new criterions();
                    tmpC.id_criterion = criterions.id_criterion;
                    tmpC.name = criterions.name;
                    result.Add(tmpC);
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                List<criterions> result = new List<criterions>();
                criterions tmpC = new criterions();
                tmpC.id_criterion = -1;
                result.Add(tmpC);
                return result;
            }
        }
        //Метод получения всех экспертов
        public List<experts> GetListExperts()
        {
            try
            {
                List<experts> result = new List<experts>();
                List<experts> tmplC = db_Expertises.experts.ToList();
                foreach (experts pC in tmplC)
                {
                    experts tmpC = new experts();
                    tmpC.id_expert = pC.id_expert;
                    tmpC.first_name = pC.first_name;
                    tmpC.login = pC.login;
                    tmpC.patronymic = pC.patronymic;
                    tmpC.second_name = pC.second_name;
                    tmpC.password = pC.password;
                    result.Add(tmpC);
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                List<experts> result = new List<experts>();
                experts tmpC = new experts();
                tmpC.id_expert = -1;
                result.Add(tmpC);

                return result;
            }
        }

        //Метод получения всех проектов
        public List<projects> GetListProjects()
        {
            try
            {
                List<projects> result = new List<projects>();
                List<projects> tmplC = db_Expertises.projects.ToList();
                foreach (projects pC in tmplC)
                {
                    projects tmpC = new projects();
                    tmpC.id_project = pC.id_project;
                    tmpC.name = pC.name;
                    result.Add(tmpC);
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                List<projects> result = new List<projects>();
                projects tmpC = new projects();
                tmpC.id_project = -1;
                result.Add(tmpC);

                return result;
            }
        }

        //Метод получения всех незавершенных экспертиз
        public List<expertises> GetListCurrentExpertises(int id_expert)
        {
            try
            {
                experts _ex = db_Expertises.experts.Where(o => o.id_expert == id_expert).FirstOrDefault();
                List<expertises> result = new List<expertises>();
                List<experts_expertises> tmplC2 = db_Expertises.experts_expertises.Where(o => o.expertises.id_status != 2 && o.id_expert == id_expert).ToList();

                List<expertises> tmplC = db_Expertises.expertises.Where(o => o.id_status != 2).ToList();
                foreach (experts_expertises pC in tmplC2)
                {
                    expertises tmpC = new expertises();
                    tmpC.id_expertise = pC.id_expertise;
                    tmpC.name = pC.expertises.name;
                    tmpC.type = pC.expertises.type;
                    result.Add(tmpC);
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                List<expertises> result = new List<expertises>();
                expertises tmpC = new expertises();
                tmpC.id_expertise = -1;
                result.Add(tmpC);

                return result;
            }
        }
        //Метод получения конкретной экспертизы
        public view_expertise_criterios GetListExpertisesCriterions(int id_expertise, int id_expert)
        {
            try
            {
                view_expertise_criterios result = new view_expertise_criterios();
                expertises _expertises = db_Expertises.expertises.Where(p => p.id_expertise == id_expertise).FirstOrDefault();
                if (_expertises != null)
                {
                    List<projects_expertises> list_pr_ex = db_Expertises.projects_expertises.Where(p => p.id_expertise == id_expertise).ToList();
                    List<expertises_criterions> list_ex_cr = db_Expertises.expertises_criterions.Where(p => p.id_expertise == id_expertise).ToList();
                    List<marks> list_marks = db_Expertises.marks.Where(p => p.id_expertise == id_expertise && p.id_expert == id_expert).ToList();
                    List<projects_expertises> temp = new List<projects_expertises>();
                    foreach (var pr in list_pr_ex)
                    {
                        projects_expertises t = new projects_expertises();
                        t.id_project = pr.id_project;
                        t.projects = new projects();
                       
                        t.projects.name = pr.projects.name;
                        temp.Add(t);
                    }
                    List<expertises_criterions> temp2 = new List<expertises_criterions>();
                    foreach (var pr in list_ex_cr)
                    {
                        expertises_criterions t = new expertises_criterions();
                        t.id_criterion = pr.id_criterion;
                        t.weight = pr.weight;
                        t.criterions = new criterions();
                        t.criterions.name = pr.criterions.name;
                        temp2.Add(t);
                    }

                    List<marks> temp3 = new List<marks>();
                    foreach (var pr in list_marks)
                    {
                        marks t = new marks();
                        t.id_criterion = pr.id_criterion;
                        t.id_expert = pr.id_expert;
                        t.id_project = pr.id_project;
                        t.id_expertise = pr.id_expertise;
                        t.value = pr.value;
                        temp3.Add(t);
                    }
                    result.id_expertise = id_expertise;
                    result.list_pr_ex = temp;
                    result.list_ex_cr = temp2;
                    result.list_marks = temp3;
                  
                    //return _expertises.id_expertise;
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                view_expertise_criterios tmpC = new view_expertise_criterios();
                tmpC.id_expertise = -1;
                

                return tmpC;
            }
        }

        public view_expertise_pospelov GetListExpertisePospelov(int id_expertise, int id_expert)
        {
            try
            {
                view_expertise_pospelov result = new view_expertise_pospelov();
                expertises _expertises = db_Expertises.expertises.Where(p => p.id_expertise == id_expertise).FirstOrDefault();
                if (_expertises != null)
                {
                    List<factors> list_pr_ex = db_Expertises.factors.Where(p => p.id_expertise == id_expertise).ToList();
                    List<marks_ribs> list_marks_ribs = db_Expertises.marks_ribs.Where(p => p.id_expertise == id_expertise && p.id_expert == id_expert).ToList();
                    List<marks_factors> list_marks_factors = db_Expertises.marks_factors.Where(p => p.id_expertise == id_expertise && p.id_expert == id_expert).ToList();
                    List<factors> temp = new List<factors>();
                    List<ribs> temp2 = new List<ribs>();
                    foreach (var pr in list_pr_ex)
                    {
                        factors t = new factors();
                        t.id_expertise = pr.id_expertise;
                        t.id_factor = pr.id_factor;
                        t.priority = pr.priority;
                        t.name = pr.name;
                        List<ribs> list_ex_cr = db_Expertises.ribs.Where(p => p.id_factor_from == pr.id_factor).ToList();
                        if(list_ex_cr != null)
                        {
                           
                            foreach (var pr2 in list_ex_cr)
                            {
                                ribs t2 = new ribs();
                                t2.id_rib = pr2.id_rib;
                                t2.id_factor_from = pr2.id_factor_from;
                                t2.id_factor_in = pr2.id_factor_in;
                                temp2.Add(t2);
                            }
                        }
                        temp.Add(t);
                    }
                    

                    List<marks_factors> temp3 = new List<marks_factors>();
                    foreach (var pr in list_marks_factors)
                    {
                        marks_factors t = new marks_factors();
                        t.id_mark = pr.id_mark;
                        t.id_expertise = pr.id_expertise;
                        t.id_factor = pr.id_factor;
                        t.id_expert = pr.id_expert;
                        t.value = pr.value;
                        temp3.Add(t);
                    }
                    List<marks_ribs> temp4 = new List<marks_ribs>();
                    foreach (var pr in list_marks_ribs)
                    {
                        marks_ribs t = new marks_ribs();
                        t.id_mark_rib = pr.id_mark_rib;
                        t.id_expertise = pr.id_expertise;
                        t.id_rib = pr.id_rib;
                        t.id_expert = pr.id_expert;
                        t.value = pr.value;
                        temp4.Add(t);
                    }
                    result.id_expertise = id_expertise;
                    result.list_factors = temp;
                    result.list_ribs = temp2;
                    result.list_marks_factors = temp3;
                    result.list_marks_ribs = temp4;

                    //return _expertises.id_expertise;
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                view_expertise_pospelov tmpC = new view_expertise_pospelov();
                tmpC.id_expertise = -1;


                return tmpC;
            }
        }

        //Метод получения всех завершенных экспертиз
        public List<expertises> GetListCompletedExpertises()
        {
            try
            {
                List<expertises> result = new List<expertises>();
                List<expertises> tmplC = db_Expertises.expertises.Where(o => o.id_status == 2).ToList();
                foreach (expertises pC in tmplC)
                {
                    expertises tmpC = new expertises();
                    tmpC.id_expertise = pC.id_expertise;
                    tmpC.name = pC.name;
                    tmpC.type = pC.type;
                    result.Add(tmpC);
                }
                return result;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                List<expertises> result = new List<expertises>();
                expertises tmpC = new expertises();
                tmpC.id_expertise = -1;
                result.Add(tmpC);

                return result;
            }
        }

        //получения данных по экпертизе Паттерн для вывода в ексель
        public view_completed_expertise GetListExpertiseReport(int id_expertise)
        {
            try
            {
                view_completed_expertise result = new view_completed_expertise();
                List<marks> ls_marks = db_Expertises.marks.Where(p => p.id_expertise == id_expertise).ToList();
                List<marks> temp_marks = new List<marks>();
                foreach (var pr in ls_marks)
                {
                    marks t = new marks();
                    t.id_project = pr.id_project;
                    t.id_expertise = pr.id_expertise;
                    t.id_expert = pr.id_expert;
                    t.id_criterion = pr.id_criterion;
                    t.projects = new projects();
                    t.projects.name = pr.projects.name;
                    t.experts = new experts();
                    t.experts.first_name = pr.experts.first_name;
                    t.experts.second_name = pr.experts.second_name;
                    t.experts.patronymic = pr.experts.patronymic;
                    
                    t.criterions = new criterions();
                    t.criterions.name = pr.criterions.name;
                    t.value = pr.value;
                    temp_marks.Add(t);
                }
                List<results_experts> list_res_ex = db_Expertises.results_experts.Where(p => p.id_expertise == id_expertise).ToList();
                List<results_experts> temp_result_experts = new List<results_experts>();
                foreach (var pr in list_res_ex)
                {
                    results_experts t = new results_experts();
                    t.id_project = pr.id_project;
                    t.id_expertise = pr.id_expertise;
                    t.id_expert = pr.id_expert;
                    t.projects = new projects();
                    t.projects.name = pr.projects.name;
                    t.experts = new experts();
                    t.experts.first_name = pr.experts.first_name;
                    t.value = pr.value;
                    temp_result_experts.Add(t);
                }
                List<results_expertises_criterions> ls_res_expertise_cr = db_Expertises.results_expertises_criterions.Where(p => p.id_expertise == id_expertise).ToList();
                List<results_expertises_criterions> temp_res_expertise_cr = new List<results_expertises_criterions>();
                foreach (var pr in ls_res_expertise_cr)
                {
                    results_expertises_criterions t = new results_expertises_criterions();
                    t.id_project = pr.id_project;
                    t.id_expertise = pr.id_expertise;
                    t.id_criterion = pr.id_criterion;
                    t.projects = new projects();
                    t.projects.name = pr.projects.name;
                    t.criterions = new criterions();
                    t.criterions.name = pr.criterions.name;
                    t.value = pr.value;
                    temp_res_expertise_cr.Add(t);
                }
                List<results_expertise> list_res_expertise = db_Expertises.results_expertise.Where(p => p.id_expertise == id_expertise).ToList();
                List<results_expertise> temp_res_expertise = new List<results_expertise>();
                foreach (var pr in list_res_expertise)
                {
                    results_expertise t = new results_expertise();
                    t.id_project = pr.id_project;
                    t.id_expertise = pr.id_expertise;
                    t.projects = new projects();
                    t.projects.name = pr.projects.name;
                    t.value = pr.value;
                    temp_res_expertise.Add(t);
                }
                result.id_expertise = id_expertise;
                result.list_marks = temp_marks;
                result.list_res_ex = temp_result_experts;
                result.list_res_exppertise = temp_res_expertise;
                result.list_res_ex_cr = temp_res_expertise_cr;

                return result;

            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                view_completed_expertise tmpC = new view_completed_expertise();
                tmpC.id_expertise = -1;
                return tmpC;
            }
        }

        // Получение данных по экспертизе поспелова для ексель
        public view_completed_expertise_pospelov GetListExpertiseReportPospelov(int id_expertise)
        {
            try
            {
                view_completed_expertise_pospelov result = new view_completed_expertise_pospelov();
                expertises _expertises = db_Expertises.expertises.Where(o => o.id_expertise == id_expertise).FirstOrDefault();
                List<marks_factors> ls_marks_factors = db_Expertises.marks_factors.Where(o => o.id_expertise == id_expertise).ToList();
                List<marks_ribs> ls_marks_ribs = db_Expertises.marks_ribs.Where(o => o.id_expertise == id_expertise).ToList();
                List<results_factors> ls_results_factors = db_Expertises.results_factors.Where(o => o.id_expertise == id_expertise).ToList();
                List<results_ribs> ls_results_ribs = db_Expertises.results_ribs.Where(o => o.id_expertise == id_expertise).ToList();

                expertises ex_new = new expertises();
                ex_new.id_expertise = _expertises.id_expertise;
                ex_new.name = _expertises.name;
                ex_new.id_status = _expertises.id_status;
                ex_new.type = _expertises.type;

                List<marks_factors> temp_marks_fact = new List<marks_factors>();
                foreach (var pr in ls_marks_factors)
                {
                    marks_factors t = new marks_factors();
                    t.id_mark = pr.id_mark;
                    t.id_factor = pr.id_factor;
                    t.id_expertise = pr.id_expertise;
                    t.id_expert = pr.id_expert;
                    t.value = pr.value;
                    t.factors = new factors();
                    t.factors.name = pr.factors.name;
                    t.factors.id_factor = pr.factors.id_factor;
                    t.factors.priority = pr.factors.priority;
                    t.experts = new experts();
                    t.experts.first_name = pr.experts.first_name;
                    t.experts.second_name = pr.experts.second_name;
                    t.experts.patronymic = pr.experts.patronymic;
                    temp_marks_fact.Add(t);
                }
                List<marks_ribs> temp_marks_ribs = new List<marks_ribs>();
                foreach (var pr in ls_marks_ribs)
                {
                    marks_ribs t = new marks_ribs();
                    t.id_rib = pr.id_rib;
                    t.id_mark_rib = pr.id_mark_rib;
                    t.id_expertise = pr.id_expertise;
                    t.id_expert = pr.id_expert;
                    t.value = pr.value;
                    t.ribs = new ribs();
                    t.ribs.id_rib = pr.ribs.id_rib;
                    t.ribs.id_factor_from = pr.ribs.id_factor_from;
                    t.ribs.factors = new factors();
                    t.ribs.factors.id_factor = pr.ribs.factors.id_factor;
                    t.ribs.factors.priority = pr.ribs.factors.priority;
                    t.ribs.id_factor_in = pr.ribs.id_factor_in;
                    t.ribs.factors1 = new factors();
                    t.ribs.factors1.id_factor = pr.ribs.factors1.id_factor;
                    t.ribs.factors1.priority = pr.ribs.factors1.priority;
                    t.experts = new experts();
                    t.experts.first_name = pr.experts.first_name;
                    t.experts.second_name = pr.experts.second_name;
                    t.experts.patronymic = pr.experts.patronymic;
                    temp_marks_ribs.Add(t);
                }
                List<results_ribs> temp_results_ribs = new List<results_ribs>();
                foreach (var pr in ls_results_ribs)
                {
                    results_ribs t = new results_ribs();
                    t.id_rib = pr.id_rib;
                    t.id_result_mark_rib = pr.id_result_mark_rib;
                    t.id_expertise = pr.id_expertise;
                    t.value = pr.value;
                    t.ribs = new ribs();
                    t.ribs.id_rib = pr.ribs.id_rib;
                    t.ribs.id_factor_from = pr.ribs.id_factor_from;
                    t.ribs.factors = new factors();
                    t.ribs.factors.id_factor = pr.ribs.factors.id_factor;
                    t.ribs.factors.priority = pr.ribs.factors.priority;
                    t.ribs.id_factor_in = pr.ribs.id_factor_in;
                    t.ribs.factors1 = new factors();
                    t.ribs.factors1.id_factor = pr.ribs.factors1.id_factor;
                    t.ribs.factors1.priority = pr.ribs.factors1.priority;
                    temp_results_ribs.Add(t);
                }
                List<results_factors> temp_results_fact = new List<results_factors>();
                foreach (var pr in ls_results_factors)
                {
                    results_factors t = new results_factors();
                    t.id_factor = pr.id_factor;
                    t.id_expertise = pr.id_expertise;
                    t.id_mark_result_factor = pr.id_mark_result_factor;
                    t.value = pr.value;
                    t.factors = new factors();
                    t.factors.name = pr.factors.name;
                    t.factors.priority = pr.factors.priority;
                    t.factors.id_factor = pr.factors.id_factor;
                    temp_results_fact.Add(t);
                }
                result.id_expertise = id_expertise;
                result.expertises = ex_new;
                result.list_marks_factors = temp_marks_fact;
                result.list_marks_ribs = temp_marks_ribs;
                result.list_results_factors = temp_results_fact;
                result.list_results_ribs = temp_results_ribs;

                return result;
            }
            catch(Exception ex)
            {
                view_completed_expertise_pospelov tmpC = new view_completed_expertise_pospelov();
                tmpC.id_expertise = -1;
                return tmpC;
            }
        }
        #endregion

        #region Редактирование БД
        //Редактирование критерия
        public int EditCriterion(int id_criterion, string name)
        {
            try
            {
                criterions criterions = db_Expertises.criterions.Where(p => p.id_criterion == id_criterion).FirstOrDefault();
                if (criterions != null)
                {
                    criterions.name = name;
                    db_Expertises.SaveChanges();
                    return criterions.id_criterion;
                }
                else return -1;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка
                
                return (-1);
            }
        }

        //Редактирование проекта
        public int EditProject(int id_project, string name)
        {
            try
            {
                projects _projects = db_Expertises.projects.Where(p => p.id_project == id_project).FirstOrDefault();
                if (_projects != null)
                {
                    _projects.name = name;
                    db_Expertises.SaveChanges();
                    return _projects.id_project;
                }
                else return -1;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка

                return (-1);
            }
        }
        //Редактирование эксперта
        public int EditExpert(int id_expert, string first_name, string second_name, string patronymic, string login, string password,int accses)
        {
            try
            {
                experts exp = db_Expertises.experts.Where(p => p.id_expert == id_expert).FirstOrDefault();
                if (exp != null)
                {
                    exp.first_name = first_name;
                    exp.second_name = second_name;
                    exp.patronymic = patronymic;
                    exp.login = login;
                    exp.access = accses;
                    exp.password = password;
                    db_Expertises.SaveChanges();
                    return exp.id_expert;
                }
                else return -1;
            }
            catch (Exception Ex)
            {
                // тут логируется ошибка

                return (-1);
            }
        }
        #endregion
    }
}
