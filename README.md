List<T> 转换DataTable（VS2019）
------------------------------
如果model类带有特性转换 
 
FormEntityToTableMap    （IList<T>扩展方法）  

没有特性  
FormEntityToTable   （IList<T>扩展方法）  

设计了三类特性ColumnType（列类型映射）,DataField(列名称映射），NoColumn（没有对应的列，忽略该属性）  

没有使用高级语法糖，应该.net 4.0及以上都可以，如果使用了低版本的.net,直接复制源码文件即可

