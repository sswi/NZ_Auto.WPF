using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XE.Commands;

namespace NZ_Auto.WPF.Models
{

    /// <summary>
    /// 鼠标坐标
    /// </summary>
 public class CursorPos:BindableBase
    {
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; OnPropertyChanged(); }
        }

        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; OnPropertyChanged(); }
        }


   
    }
}
