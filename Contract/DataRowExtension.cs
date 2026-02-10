using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public static class DataRowExtension
    {
        public static List<DataRow> ToList(this DataRowCollection drc)
        {
            var list = new List<DataRow>();

            foreach (DataRow row in drc)
            {
                list.Add(row);
            }

            return list;
        }
    }
}
