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
    
    public partial class experts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public experts()
        {
            this.experts_expertises = new HashSet<experts_expertises>();
            this.marks = new HashSet<marks>();
            this.results_experts = new HashSet<results_experts>();
            this.marks_factors = new HashSet<marks_factors>();
            this.marks_ribs = new HashSet<marks_ribs>();
        }
    
        public int id_expert { get; set; }
        public string second_name { get; set; }
        public string first_name { get; set; }
        public string patronymic { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int access { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<experts_expertises> experts_expertises { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<marks> marks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<results_experts> results_experts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<marks_factors> marks_factors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<marks_ribs> marks_ribs { get; set; }
    }
}
