using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyAHP
{
    public class Table
    {
        public string name;
        public List<List<Cell>> matrix = new List<List<Cell>>();
        public List<Table> subTables = new List<Table>();
        public List<double> weigth = new List<double>();
        public List<Cell> normalize = new List<Cell>();
     
           
    }

}
