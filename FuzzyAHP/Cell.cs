using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyAHP
{ 
    public class Cell
    {

        public double low = 0;
        public double middle = 0;
        public double up = 0;

        public Cell()
        {
        }

        public Cell(double low, double middle, double up)
        {
            this.low = low;
            this.middle = middle;
            this.up = up;
        }
    }
}
