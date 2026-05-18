using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace WildcatHub
{
    public class StudentEquipmentItem
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; } = "";
        public string Category { get; set; } = "";
        public string Brand { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string LabName { get; set; } = "";
        public string SubjectCode { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public int QuantityTotal { get; set; }
        public int QuantityMaintenance { get; set; }

        public int Available
        {
            get
            {
                int available = QuantityTotal - QuantityMaintenance;
                return available < 0 ? 0 : available;
            }
        }
    }




    public static class StudentEquipmentService
    {
        public static List<StudentEquipmentItem> GetEquipmentForStudent(
    int userId,
    string subjectFilter = "All",
    string categoryFilter = "All",
    string keyword = "")
        {
            List<StudentEquipmentItem> items = new List<StudentEquipmentItem>();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
SELECT DISTINCT
    E.EquipmentID,
    E.EquipmentName,
    E.Category,
    E.Brand,
    E.ImagePath,
    E.QuantityTotal,
    E.QuantityMaintenance,
    L.LabName,
    LS.SubjectCode,
    LS.SubjectName
FROM ((((StudentSubjectEnrollments AS SSE
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID)
INNER JOIN SubjectEquipments AS SE ON LS.SubjectID = SE.SubjectID)
INNER JOIN Equipment AS E ON SE.EquipmentID = E.EquipmentID)
INNER JOIN Laboratories AS L ON E.LabID = L.LabID)
WHERE SSE.UserID = ?
AND SSE.IsActive = True
AND LS.IsActive = True
AND E.IsArchived = False
AND E.Status = 'Active'";

            if (subjectFilter != "All")
                query += " AND LS.SubjectCode = ?";

            if (categoryFilter != "All")
                query += " AND E.Category = ?";

            if (!string.IsNullOrWhiteSpace(keyword))
                query += " AND E.EquipmentName LIKE ?";

            query += " ORDER BY LS.SubjectCode, E.EquipmentName";

            using OleDbCommand cmd = new OleDbCommand(query, conn);

            // CRITICAL: Add in the EXACT order the ? appears in the query
            cmd.Parameters.AddWithValue("@p1", userId);

            if (subjectFilter != "All")
                cmd.Parameters.AddWithValue("@p2", subjectFilter);

            if (categoryFilter != "All")
                cmd.Parameters.AddWithValue("@p3", categoryFilter);

            if (!string.IsNullOrWhiteSpace(keyword))
                cmd.Parameters.AddWithValue("@p4", "%" + keyword + "%");

            using OleDbDataReader reader = cmd.ExecuteReader();

            while (reader != null && reader.Read())
            {
                items.Add(new StudentEquipmentItem
                {
                    EquipmentID = Convert.ToInt32(reader["EquipmentID"]),
                    EquipmentName = reader["EquipmentName"]?.ToString() ?? "",
                    Category = reader["Category"]?.ToString() ?? "",
                    Brand = reader["Brand"]?.ToString() ?? "",
                    ImagePath = reader["ImagePath"]?.ToString() ?? "",
                    QuantityTotal = reader["QuantityTotal"] != DBNull.Value ? Convert.ToInt32(reader["QuantityTotal"]) : 0,
                    QuantityMaintenance = reader["QuantityMaintenance"] != DBNull.Value ? Convert.ToInt32(reader["QuantityMaintenance"]) : 0,
                    LabName = reader["LabName"]?.ToString() ?? "",
                    SubjectCode = reader["SubjectCode"]?.ToString() ?? "",
                    SubjectName = reader["SubjectName"]?.ToString() ?? ""
                });
            }

            return items;
        }

        public static List<string> GetStudentSubjects(int userId)
        {
            List<string> subjects = new List<string>();

            using OleDbConnection conn = DbHelper.GetConnection();
            conn.Open();

            string query = @"
SELECT DISTINCT LS.SubjectCode
FROM StudentSubjectEnrollments AS SSE
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID
WHERE SSE.UserID = ?
AND SSE.IsActive = True
AND LS.IsActive = True
ORDER BY LS.SubjectCode";

            using OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@p1", userId);

            using OleDbDataReader reader = cmd.ExecuteReader();

            while (reader != null && reader.Read())
            {
                subjects.Add(reader["SubjectCode"]?.ToString() ?? "");
            }

            return subjects;
        }
    }
}