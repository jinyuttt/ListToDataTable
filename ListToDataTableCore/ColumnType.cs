using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListToDataTableCore
{
    /// <summary>
    /// 列类型映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
   public class ColumnType:Attribute
    {
        public Type Column{ get; set; }
        public ColumnType(Type column)
        {
            this.Column = column;
        }
    }
}
