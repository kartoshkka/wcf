//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfService1
{
    using System;
    using System.Collections.Generic;
    
    public partial class results_expertises_criterions
    {
        public int id_result { get; set; }
        public int id_expertise { get; set; }
        public int id_project { get; set; }
        public int id_criterion { get; set; }
        public double value { get; set; }
    
        public virtual criterions criterions { get; set; }
        public virtual expertises expertises { get; set; }
        public virtual projects projects { get; set; }
    }
}