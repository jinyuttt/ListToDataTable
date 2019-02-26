using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListToDataTableCore
{
    /// <summary>
    /// 字段与列的映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataField:Attribute
    {
        public string ColumnName { get; set; }
        public DataField(string name)
        {
            this.ColumnName = name;
        }
    }
}
