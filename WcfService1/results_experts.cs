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
    
    public partial class results_experts
    {
        public int id_result_expert { get; set; }
        public int id_expertise { get; set; }
        public int id_project { get; set; }
        public int id_expert { get; set; }
        public double value { get; set; }
    
        public virtual expertises expertises { get; set; }
        public virtual experts experts { get; set; }
        public virtual projects projects { get; set; }
    }
}
