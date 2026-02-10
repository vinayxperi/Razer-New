using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cTransaction
{
    /// <summary>
    /// Class cTable used in cTransaction and to pass parms to cTransaction
    /// </summary>
    /// <remarks>Brian Dyer, TV Guide October 24, 2007</remarks>
    public class cTable : ICloneable
    {

        private List<cSpParm> ialSelect = null;
        private List<cSpParm> ialUpdate = null;
        private List<cSpParm> ialDelete = null;

        private List<cSpParm> ialInsert = null;
        private string isTable = null;
        private string isSelect = null;
        private string isUpdate = null;
        private string isDelete = null;
        private string isInsert = null;

        /// <summary>
        /// SelectSpParms returns a list of parameters for the select SP
        /// </summary>
        /// <value>List(Of cSpParm)</value>
        /// <returns>List(Of cSpParm)</returns>
        /// <remarks></remarks>
        public List<cSpParm> SelectSpParms
        {
            get { return ialSelect; }
        }
                
        /// <summary>
        /// UpdateSpParms returns a list of parameters for the update SP
        /// </summary>
        /// <value>List(Of cSpParm)</value>
        /// <returns>List(Of cSpParm)</returns>
        /// <remarks></remarks>
        public List<cSpParm> UpdateSpParms
        {
            get { return ialUpdate; }
        }

        /// <summary>
        /// DeleteSpParms returns a list of parameters for the delete SP
        /// </summary>
        /// <value>List(Of cSpParm)</value>
        /// <returns>List(Of cSpParm)</returns>
        /// <remarks></remarks>
        public List<cSpParm> DeleteSpParms
        {
            get { return ialDelete; }
        }

        /// <summary>
        /// InsertSpParms returns a list of parameters for the insert SP
        /// </summary>
        /// <value>List(Of cSpParm)</value>
        /// <returns>List(Of cSpParm)</returns>
        /// <remarks></remarks>
        public List<cSpParm> InsertSpParms
        {
            get { return ialInsert; }
        }

        /// <summary>
        /// TableName the table name
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string TableName
        {
            get { return isTable; }
        }

        /// <summary>
        /// TableSelect returns the select SP name
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string TableSelect
        {
            get { return isSelect; }
        }

        /// <summary>
        /// TableUpdate returns the update SP name
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string TableUpdate
        {
            get { return isUpdate; }
        }

        /// <summary>
        /// TableDelete returns the delete SP name
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string TableDelete
        {
            get { return isDelete; }
        }

        /// <summary>
        /// TableInsert returns the insert SP name
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string TableInsert
        {
            get { return isInsert; }
        }

        /// <summary>
        /// The Clone function returns a shallow copy of the object
        /// </summary>
        /// <returns>Object</returns>
        /// <remarks></remarks>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Constructor class for cTable
        /// </summary>
        /// <param name="table">String: Name of the database table</param>
        /// <remarks></remarks>
        public cTable(string table)
        {
            isTable = table;
        }

        /// <summary>
        /// The function NewSelect sets the select SP name and parameter list
        /// </summary>
        /// <param name="SpName">String: name of the SP</param>
        /// <param name="Parmlist">List(Of cSpParm)</param>
        /// <returns>Boolean</returns>
        /// <remarks></remarks>
        public bool NewSelect(string SpName, List<cSpParm> Parmlist)
        {
            bool bTF = false;

            try
            {
                isSelect = SpName;
                ialSelect = Parmlist;
                bTF = true;

            }
            catch
            {
            }

            return bTF;
        }

        /// <summary>
        /// The function NewUpdate sets the update SP name and parameter list
        /// </summary>
        /// <param name="SpName">String: name of the SP</param>
        /// <param name="Parmlist">List(Of cSpParm)</param>
        /// <returns>Boolean</returns>
        /// <remarks></remarks>
        public bool NewUpdate(string SpName, List<cSpParm> Parmlist)
        {
            bool bTF = false;

            try
            {
                isUpdate = SpName;
                ialUpdate = Parmlist;
                bTF = true;

            }
            catch
            {
            }

            return bTF;
        }

        /// <summary>
        /// The function NewDelete sets the delete SP name and parameter list
        /// </summary>
        /// <param name="SpName">String: name of the SP</param>
        /// <param name="Parmlist">List(Of cSpParm)</param>
        /// <returns>Boolean</returns>
        /// <remarks></remarks>
        public bool NewDelete(string SpName, List<cSpParm> Parmlist)
        {
            bool bTF = false;

            try
            {
                isDelete = SpName;
                ialDelete = Parmlist;
                bTF = true;

            }
            catch
            {
            }

            return bTF;
        }

        /// <summary>
        /// The function NewInsert sets the insert SP name and parameter list
        /// </summary>
        /// <param name="SpName">String: name of the SP</param>
        /// <param name="Parmlist">List(Of cSpParm)</param>
        /// <returns>Boolean</returns>
        /// <remarks></remarks>
        public bool NewInsert(string SpName, List<cSpParm> Parmlist)
        {
            bool bTF = false;

            try
            {
                isInsert = SpName;
                ialInsert = Parmlist;
                bTF = true;

            }
            catch
            {
            }

            return bTF;
        }

    }
}
