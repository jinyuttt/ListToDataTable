using ListToDataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> lst = new List<Person>();
            lst.Add(new Person() { Age = 1, Name = "ji", Score = 23, KK = "100" });
            lst.Add(new Person() { Age = 3, Name = "ty", Score = 26, KK = "101" });
            lst.Add(new Person() { Age = 7, Name = "er", Score = 29, KK = "120" });
            IList<Person> tmp = lst;
            tmp.FormEntityToTableMap();
        }
    }
}
