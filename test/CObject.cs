using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class CObject
    {
        private int Id_criterion;
        private string Name;
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
        public string name
        {
            get { return Name; }
            set
            {
                Name = value;
                OnPropertyChanged("name");
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
