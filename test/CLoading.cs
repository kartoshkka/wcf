using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace test
{
    class CLoading
    {
        Viewbox _v;
        
        public CLoading(Viewbox _viewbox)
        {
            
            _v = _viewbox;
        }
        public void start()
        {
            _v.Visibility = System.Windows.Visibility.Visible;
        }
        public void stop()
        {
            _v.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
