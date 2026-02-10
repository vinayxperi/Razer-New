using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazerBase
{
    public static class ExceptionHandler
    {
        public static void HandleException(Exception ex)
        {
            System.Windows.MessageBox.Show(string.Format("{0}", ex.Message));
        }
    }
}