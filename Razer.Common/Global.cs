using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing.Printing;

namespace Razer.Common
{
    public static class Global
    {
        /// <summary>
        /// Return the Unity container for the application
        /// </summary>
        public static IUnityContainer Container { get; set; }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="source">The original object.</param>
        /// <param name="copy">The object to compare to the originial.</param>
        /// <returns></returns>
        public static bool Compare(object source, object copy)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            MemoryStream sourceStream = new MemoryStream();
            formatter.Serialize(sourceStream, source);

            MemoryStream copyStream = new MemoryStream();
            formatter.Serialize(copyStream, copy);

            byte[] sourceByteStream = sourceStream.ToArray();
            byte[] copyByteStream = copyStream.ToArray();


            if (CompareBytes(sourceByteStream, copyByteStream))
                return true;
            
            return false;
        }


        private static bool CompareBytes(byte[] source, byte[] copy)
        {
            if (source.Length != copy.Length)
                return false;

            if (!source.Take(source.Length).SequenceEqual(copy.Take(copy.Length)))
            {
                return false;
            }

            return true;
        }


        public static string GetDefaultPrinter()
        {           
            PrintDocument document = new PrintDocument();
            
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {                
                if (document.PrinterSettings.IsDefaultPrinter)
                {
                    return document.PrinterSettings.PrinterName;
                }
            }

            return string.Empty;
        }
    }
}
