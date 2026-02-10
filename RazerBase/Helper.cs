using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace RazerBase
{
    public static class Helper
    {
        public static DataTable CustomObjectToDataTable<T>(List<T> items, string dataTableName = null)
        {
            DataTable dt = new DataTable();
            Type tType = typeof(T);
            PropertyInfo[] props = tType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                DataColumn col = new DataColumn(prop.Name, prop.PropertyType);
                dt.Columns.Add(col);
            }

            foreach (T item in items)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo prop in props)
                {
                    dr[prop.Name] = prop.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }

            if (dataTableName != null) { dt.TableName = dataTableName; }

            return dt;
        }

        public static bool IsDateTime(object value)
        {
            return (value is DateTime);
        }

        public static bool IsNumeric(object value)
        {
            var TF = false;
            if (!(value == null || value is DateTime))
            {
                TF = (value is byte || value is Int16 || value is Int32 || value is Int64 || value is Single || value is Double || value is Decimal);
                if (!TF) { TF = (IsType<Int64>(value) || IsType<Single>(value) || IsType<Double>(value) || IsType<Decimal>(value)); }
            }
            return TF;
        }

        public static bool IsType<T>(object value)
        {
            var TF = false;

            try { TF = (Convert.ChangeType(value, typeof(T)) is T); }
            catch { /* do nothing */ }

            return TF;
        }
    }
}
