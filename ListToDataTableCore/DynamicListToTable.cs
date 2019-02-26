using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Reflection;
namespace ListToDataTableCore
{
    public delegate void LoadDataTable<T>(DataTable dr,T obj);
    public delegate void LoadDataRow<T>(DataRow row, T obj);

    /// <summary>
    /// List转DataTable扩展
    /// </summary>
    public static class DynamicListToTable
    {
        private static Dictionary<string, object> cache = new Dictionary<string, object>();
        private static Dictionary<string, DataTable> cacheDataTable = new Dictionary<string, DataTable>();

        /// <summary>
        /// 直接转换整个DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <returns></returns>
        public static DynamicMethod EntityToDataTableEmit<T>(Dictionary<string,string> map=null,Dictionary<string,Type>mapType=null)
        {
            DynamicMethod method = new DynamicMethod(typeof(T).Name + "ToDataTable", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, null,
                new Type[] {typeof(DataTable), typeof(T) }, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder reslut = generator.DeclareLocal(typeof(DataRow));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, typeof(DataTable).GetMethod("NewRow"));
            generator.Emit(OpCodes.Stloc, reslut);
            if (map == null)
            {
                foreach (var p in typeof(T).GetProperties())
                {
                    generator.Emit(OpCodes.Ldloc, reslut);
                    generator.Emit(OpCodes.Ldstr, p.Name);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, p.GetGetMethod());//直接给属性赋值
                    if (mapType == null||!mapType.ContainsKey(p.Name))
                    {
                        if (p.PropertyType.IsValueType)
                            generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                        else
                            generator.Emit(OpCodes.Castclass, p.PropertyType);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldtoken, mapType[p.Name]);
                        generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        generator.Emit(OpCodes.Call,typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                    }
                    generator.Emit(OpCodes.Call, typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
                }
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, typeof(DataTable).GetMethod("get_Rows"));
                generator.Emit(OpCodes.Ldloc, reslut);
                generator.Emit(OpCodes.Call, typeof(DataRowCollection).GetMethod("Add", new Type[] { typeof(DataRow) }));
                generator.Emit(OpCodes.Ret);
            }
            else
            {
                var properties= typeof(T).GetProperties();
                List<PropertyInfo> lst = new List<PropertyInfo>(properties);
                foreach (var kv in map)
                {
                   var p= lst.Find(x => x.Name == kv.Value);
                    generator.Emit(OpCodes.Ldloc, reslut);
                    generator.Emit(OpCodes.Ldstr, kv.Key);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, p.GetGetMethod());//获取属性值
                    //if (p.PropertyType.IsValueType)
                    //    generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                    //else
                    //    generator.Emit(OpCodes.Castclass, p.PropertyType);
                    if (mapType == null || !mapType.ContainsKey(kv.Key))
                    {
                        if (p.PropertyType.IsValueType)
                            generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                        else
                            generator.Emit(OpCodes.Castclass, p.PropertyType);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldtoken, mapType[kv.Key]);
                        generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        generator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                    }

                    generator.Emit(OpCodes.Call, typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
                }
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, typeof(DataTable).GetMethod("get_Rows"));
                generator.Emit(OpCodes.Ldloc, reslut);
                generator.Emit(OpCodes.Call, typeof(DataRowCollection).GetMethod("Add", new Type[] { typeof(DataRow) }));
                generator.Emit(OpCodes.Ret);
            }
            return method;

        }

        /// <summary>
        /// 转换DataRow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DynamicMethod EntityToDataRowEmit<T>(Dictionary<string, string> map = null, Dictionary<string, Type> mapType = null)
        {
            DynamicMethod method = new DynamicMethod(typeof(T).Name + "ToDataRow", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, null,
                new Type[] { typeof(DataRow), typeof(T)}, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();
            if (map == null)
            {
                foreach (var p in typeof(T).GetProperties())
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldstr, p.Name);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, p.GetGetMethod());//直接给属性赋值
                    //if (p.PropertyType.IsValueType)
                    //    generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                    //else
                    //    generator.Emit(OpCodes.Castclass, p.PropertyType);
                    if (mapType == null || !mapType.ContainsKey(p.Name))
                    {
                        if (p.PropertyType.IsValueType)
                            generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                        else
                            generator.Emit(OpCodes.Castclass, p.PropertyType);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldtoken, mapType[p.Name]);
                        generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        generator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                    }

                    generator.Emit(OpCodes.Call, typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
                }
                //generator.Emit(OpCodes.Ldarg_0);
                //generator.Emit(OpCodes.Call, typeof(DataTable).GetMethod("get_Rows"));

                //generator.Emit(OpCodes.Call, typeof(DataRowCollection).GetMethod("Add", new Type[] { typeof(DataRow) }));
                generator.Emit(OpCodes.Ret);
            }
            else
            {
                
                var properties = typeof(T).GetProperties();
                List<PropertyInfo> lst = new List<PropertyInfo>(properties);
                foreach (var kv in map)
                {
                    var p = lst.Find(x => x.Name == kv.Value);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldstr, kv.Key);
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Call, p.GetGetMethod());//获取属性值
                    //if (p.PropertyType.IsValueType)
                    //    generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                    //else
                    //    generator.Emit(OpCodes.Castclass, p.PropertyType);
                    if (mapType == null || !mapType.ContainsKey(kv.Key))
                    {
                        if (p.PropertyType.IsValueType)
                            generator.Emit(OpCodes.Box, p.PropertyType);//一直在折腾这个地方，哎
                        else
                            generator.Emit(OpCodes.Castclass, p.PropertyType);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldtoken,mapType[kv.Key]);
                        generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        generator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                    }

                    generator.Emit(OpCodes.Call, typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
                }
                //generator.Emit(OpCodes.Ldarg_0);
                //generator.Emit(OpCodes.Call, typeof(DataTable).GetMethod("get_Rows"));

                //generator.Emit(OpCodes.Call, typeof(DataRowCollection).GetMethod("Add", new Type[] { typeof(DataRow) }));
                generator.Emit(OpCodes.Ret);
            }

            return method;

        }


        /// <summary>
        /// 直接属性转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static DataTable FormEntityToTable<T>(this IList<T> lst)
        {
            DataTable dt = new DataTable();
            if (!cacheDataTable.ContainsKey(typeof(T).FullName))
            {
                var properties = typeof(T).GetProperties();
               
                foreach (var p in properties)
                {
                    dt.Columns.Add(p.Name, p.PropertyType);
                }
            }
            else
            {
                dt = cacheDataTable[typeof(T).FullName].Clone();
            }
            //1.如果调用table转换
            //LoadDataTable<T> load = (LoadDataTable<T>)PersonToDataTable<T>().CreateDelegate(typeof(LoadDataTable<T>));
            LoadDataTable<T> load = Find<T>();
            foreach (var item in lst)
            {
                load(dt, item);
            }
            ////2.如果调用行转换(控制度大些)
            //LoadDataRow<T> loadrow = (LoadDataRow<T>)PersonToDataRow<T>().CreateDelegate(typeof(LoadDataRow<T>));
            //foreach (var item in lst)
            //{
            //    var row = dt.NewRow();
            //    loadrow(row, item);
            //    dt.Rows.Add(row);
            //}
            return dt;
        }

        /// <summary>
        /// 带有特性的转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static DataTable FormEntityToTableMap<T>(this IList<T> lst)
        {
            LoadDataRow<T> loadrow = FindMap<T>();
            DataTable dt = new DataTable();
            if (loadrow == null)
            {
                var properties = typeof(T).GetProperties();
               
                Dictionary<string, string> map = new Dictionary<string, string>();
                Dictionary<string, Type> mapType = new Dictionary<string, Type>();
                foreach (var p in properties)
                {

                    if (p.GetCustomAttribute(typeof(NoColumn)) != null)
                    {
                        //没有该列映射
                        continue;
                    }
                    else if (p.GetCustomAttribute(typeof(DataField)) != null)
                    {
                        DataField ttr = p.GetCustomAttribute<DataField>();
                        var type = p.GetCustomAttribute<ColumnType>();
                        map.Add(ttr.ColumnName, p.Name);

                        if (type != null && !type.Column.Equals(p.PropertyType))
                        {
                            dt.Columns.Add(ttr.ColumnName, type.Column);
                            mapType[ttr.ColumnName] = type.Column;
                        }
                        else
                        {
                            dt.Columns.Add(ttr.ColumnName, p.PropertyType);
                        }
                    }
                    else if (p.GetCustomAttribute(typeof(ColumnType)) != null)
                    {
                        var type = p.GetCustomAttribute<ColumnType>();
                        dt.Columns.Add(p.Name, type.Column);
                        map.Add(p.Name, p.Name);
                        if (!type.Column.Equals(p.PropertyType))
                        {
                            mapType[p.Name] = type.Column;
                        }
                    }
                    else
                    {
                        dt.Columns.Add(p.Name, p.PropertyType);
                        map.Add(p.Name, p.Name);
                    }
                }
                if (map.Count == 0)
                {
                    map = null;
                }
                if (mapType.Count == 0)
                {
                    mapType = null;
                }
                 loadrow = CreateMap<T>(map, mapType);
                cacheDataTable[typeof(T).FullName + "_map"]= dt;
            }
            else
            {
                dt = cacheDataTable[typeof(T).FullName + "_map"].Clone();
            }
            
            ////1.如果调用table转换
            //LoadDataTable<T> load = (LoadDataTable<T>)PersonToDataTable<T>(map,mapType).CreateDelegate(typeof(LoadDataTable<T>));
            //foreach (var item in lst)
            //{
            //    load(dt, item);
            //}
            //2.如果调用行转换(控制度大些)

           
            foreach (var item in lst)
            {
                var row = dt.NewRow();
                loadrow(row, item);
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 忽略特性的查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LoadDataTable<T> Find<T>()
        {
            LoadDataTable<T> load = null;
            object v = null;
            string name = typeof(T).FullName;
            if (cache.TryGetValue(name, out v))
            {
                load = v as LoadDataTable<T>;
            }
            else
            {
                load =(LoadDataTable < T >) EntityToDataTableEmit<T>().CreateDelegate(typeof(LoadDataTable<T>));
                cache[name] = load;
            }
            return load;
        }

        /// <summary>
        /// 带有特性的查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LoadDataRow<T> FindMap<T>()
        {
            LoadDataRow<T> loadrow = null;
            object v = null;
            if(cache.TryGetValue(typeof(T).FullName+"_map", out v))
            {
                loadrow = v as LoadDataRow<T>;
            }
            return loadrow;
        }

        /// <summary>
        /// 带有特性的创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        private static LoadDataRow<T> CreateMap<T>(Dictionary<string,string>map,Dictionary<string,Type>mapType)
        { 
            var loadRow= (LoadDataRow<T>)EntityToDataRowEmit<T>(map, mapType).CreateDelegate(typeof(LoadDataRow<T>));
            cache[typeof(T).FullName+"_map"] = loadRow;
            return loadRow;
        }
    }
}
