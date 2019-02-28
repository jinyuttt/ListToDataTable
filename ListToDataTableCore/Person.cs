using System;
using System.Collections.Generic;
using System.Text;

namespace ListToDataTableCore
{
    /// <summary>
    /// 例子
    /// </summary>
   public class Person
    {
        [DataField("PersonName")]
        public string Name { get; set; }
       
        public int? Age { get; set; }

        public int Score { get; set; }

        [ColumnType(typeof(int))]
        public string KK { get; set; }
    }
}
