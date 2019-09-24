using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace tmg.equinox.infrastructure.util.Util
{
    public static class ConvertToDataTable
    {
        public static DataTable ToDataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            //Get all the properties
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                //Setting column names as Property names
                //dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                //    prop.PropertyType);
                dataTable.Columns.Add(prop.Name);
            }
            if (data != null )
            {
                foreach (TSource item in data)
                {
                    if (item != null)
                    {
                        var values = new object[props.Length];
                        for (int i = 0; i < props.Length; i++)
                        {
                            //inserting property values to datatable rows
                            values[i] = props[i].GetValue(item, null);
                        }
                        dataTable.Rows.Add(values);
                    }
                }
            }
            return dataTable;
        }  
    }
}
