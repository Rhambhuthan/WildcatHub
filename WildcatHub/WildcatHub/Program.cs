using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WildcatHub
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool adminReady = false;

            try
            {
                using (OleDbConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Admin is "ready" only if a record exists AND password is set
                    string query = @"
SELECT COUNT(*) FROM AdminCredentials
WHERE [Password] IS NOT NULL AND [Password] <> ''";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        adminReady = (result != null && result != DBNull.Value && Convert.ToInt32(result) > 0);
                    }
                }
            }
            catch
            {
                // If DB fails on startup, still show login — it will fail gracefully there
                adminReady = true;
            }

            if (!adminReady)
            {
                Application.Run(new frmAdminSetup());
            }
            else
            {
                Application.Run(new frmLogin());
            }
        }
    }
}