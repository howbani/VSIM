using System;

namespace VSIM.db
{

    /// <summary>
    /// ammar howbani 2011.
    /// sql operations
    /// </summary>
    [Serializable]
   public  class SqlOperations
    {

        [Serializable]
        public class Select
        {
            public static string AllWithNoCondition( string TableName)
            {
                return "Select *from" + " " +TableName;
            }
            public static string AllWithCondition(string TableName, string conditions)
            {
                string command = "Select *from  " +TableName;
                command = command + " where  ";
                command = command + conditions;
                return command;
            }

            public static string SpecialCols(string Tablename, string[] cols, string Conditions)
            {
                string command = " select "; //+ cols[0];
                for (int i = 0; i < cols.Length - 1; i++)
                {
                    command = command + cols[i] + " , ";
                }
                command = command + cols[cols.Length - 1];
                command = command + " from " + Tablename + " where " + Conditions;
                return command;
            }

        }



          [Serializable]
        public class InsertInTo
        {
            public static string Insert( string TableName,string[] clos, string[] values)
            {
                string command = "insert into" + " " + TableName + "(";

                for (int i = 0; i < clos.Length - 1; i++)
                {
                    // open the 
                    command = command + clos[i] + ",";
                }
                // close after read all the row from rows array
                command = command + clos[clos.Length - 1] + ")values(";
                // now the time to add the values.
                for (int j = 0; j < values.Length - 1; j++)
                {
                    command = command + "'" + values[j] + "'" + ",";
                }
                command = command + "'" + values[values.Length - 1] + "'" + ")";

                return command;
            }
        }


         [Serializable]
        public class Delete
        {
            public static string delet( string table)
            {
                string command = "DELETE *FROM " + table;
                return command;
            }
        }

          [Serializable]
        public class Update
        {
            public static string update( string table,string[] cols, string[] values, string Condition)
            {
                string command = "update " + table + " set "; // 
                for (int i = 0; i < cols.Length - 1; i++)
                {
                    command = command + cols[i] + "='" + values[i] + "',";
                }
                command = command + cols[cols.Length - 1] + "='" + values[cols.Length - 1] + "'";
                command = command + "  where " + Condition;
                return command;
            }

            public  static string update(string table, string[] cols, object[] values, string Condition)
            {
                string command = "update " + table + " set "; // 
                for (int i = 0; i < cols.Length - 1; i++)
                {
                    command = command + cols[i] + "='" + values[i].ToString() + "',";
                }
                command = command + cols[cols.Length - 1] + "='" + values[cols.Length - 1].ToString() + "'";
                command = command + "  where " + Condition;
                return command;
            }
        }


    }
}
