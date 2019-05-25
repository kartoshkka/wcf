using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Expertise
    {
        private int Id_criterion;
        private string Name_criterion;
        private float Weight;
        
        public int id_criterion
        {
            get { return Id_criterion; }
            set
            {
                Id_criterion = value;
                OnPropertyChanged("id_criterion");
            }
        }
        public string name_criterion
        {
            get { return Name_criterion; }
            set
            {
                Name_criterion = value;
                OnPropertyChanged("name_criterion");
            }
        }
        public float weight
        {
            get { return Weight; }
            set
            {
                Weight = value;
                OnPropertyChanged("weight");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
