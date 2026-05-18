using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace WildcatHub
{
    public static class DbHelper
    {
        private static readonly string DbFileName = "WildcatHub_LabSystem.accdb";

        public static string GetDatabasePath()
        {
            string[] candidatePaths =
            {
                Path.Combine(Application.StartupPath, "Database", DbFileName),
                Path.Combine(Application.StartupPath, @"..\..\..\Database", DbFileName),
                Path.Combine(Application.StartupPath, @"..\..\..\bin\Database", DbFileName),
                Path.Combine(AppContext.BaseDirectory, "Database", DbFileName),
            };

            foreach (string candidate in candidatePaths)
            {
                string fullPath = Path.GetFullPath(candidate);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            string expectedPath = Path.GetFullPath(candidatePaths[0]);

            if (!File.Exists(expectedPath))
            {
                MessageBox.Show(
                    "Database file not found.\n\nExpected path:\n" + expectedPath,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return expectedPath;
        }

        public static string GetConnectionString()
        {
            return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={GetDatabasePath()};Persist Security Info=False;";
        }

        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(GetConnectionString());
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                using (OleDbConnection conn = GetConnection())
                {
                    conn.Open();
                    message = "Database connection successful.\n\n" + GetDatabasePath();
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Database connection failed.\n\n" + ex.Message;
                return false;
            }
        }
    }
}
