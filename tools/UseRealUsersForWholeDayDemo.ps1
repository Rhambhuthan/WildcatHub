$ErrorActionPreference = "Stop"

$db = "C:\Users\HF\Documents\CPE_PROJECT\CCMA\WildCat Hub\WildcatHub\WildcatHub\bin\Database\WildcatHub_LabSystem.accdb"
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backup = [System.IO.Path]::Combine(
    [System.IO.Path]::GetDirectoryName($db),
    "WildcatHub_LabSystem_before_real_whole_day_demo_$stamp.accdb")

Copy-Item -LiteralPath $db -Destination $backup
Write-Output "Backup created: $backup"

Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.OleDb.OleDbConnection(
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db;Persist Security Info=False;")
$conn.Open()

function Invoke-Scalar($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) {
        $null = $cmd.Parameters.AddWithValue("@p", $p)
    }
    return $cmd.ExecuteScalar()
}

function Invoke-NonQuery($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) {
        $null = $cmd.Parameters.AddWithValue("@p", $p)
    }
    [void]$cmd.ExecuteNonQuery()
}

function Get-SubjectId($subjectCode) {
    $subjectId = Invoke-Scalar "SELECT [SubjectID] FROM [LabSubjects] WHERE [SubjectCode] = ?" @($subjectCode)
    if ($subjectId -eq $null -or $subjectId -eq [DBNull]::Value) {
        throw "Subject not found: $subjectCode"
    }
    return [int]$subjectId
}

function Get-OrCreateWholeDaySchedule($subjectId, $day, $section, $room) {
    $scheduleId = Invoke-Scalar @"
SELECT [ScheduleID]
FROM [SubjectSchedules]
WHERE [SubjectID] = ?
AND [DayOfWeek] = ?
AND [Section] = ?
"@ @($subjectId, $day, $section)

    if ($scheduleId -ne $null -and $scheduleId -ne [DBNull]::Value) {
        Invoke-NonQuery @"
UPDATE [SubjectSchedules]
SET [StartTime] = ?, [EndTime] = ?, [Room] = ?
WHERE [ScheduleID] = ?
"@ @([datetime]"1899-12-30 00:00:00", [datetime]"1899-12-30 23:59:00", $room, [int]$scheduleId)
        return [int]$scheduleId
    }

    Invoke-NonQuery @"
INSERT INTO [SubjectSchedules]
([SubjectID], [DayOfWeek], [StartTime], [EndTime], [Section], [Room])
VALUES (?, ?, ?, ?, ?, ?)
"@ @($subjectId, $day, [datetime]"1899-12-30 00:00:00", [datetime]"1899-12-30 23:59:00", $section, $room)

    return [int](Invoke-Scalar "SELECT @@IDENTITY")
}

function Ensure-Enrollment($userId, $subjectId, $scheduleId) {
    $enrollmentId = Invoke-Scalar @"
SELECT [EnrollmentID]
FROM [StudentSubjectEnrollments]
WHERE [UserID] = ?
AND [SubjectID] = ?
AND [ScheduleID] = ?
"@ @($userId, $subjectId, $scheduleId)

    if ($enrollmentId -ne $null -and $enrollmentId -ne [DBNull]::Value) {
        Invoke-NonQuery "UPDATE [StudentSubjectEnrollments] SET [IsActive] = True WHERE [EnrollmentID] = ?" @([int]$enrollmentId)
        return
    }

    Invoke-NonQuery @"
INSERT INTO [StudentSubjectEnrollments]
([UserID], [SubjectID], [ScheduleID], [IsActive])
VALUES (?, ?, ?, True)
"@ @($userId, $subjectId, $scheduleId)
}

# Keep the earlier generated demo accounts out of the way.
Invoke-NonQuery "UPDATE [Users] SET [IsActive] = False WHERE [SchoolID] LIKE 'DEMO-%'"

$cheSubjectId = Get-SubjectId "CHEM 202"
$meSubjectId = Get-SubjectId "ME 421"

$days = @("Monday", "Tuesday", "Wednesday", "Thursday", "Friday")
$schedulePairs = New-Object System.Collections.Generic.List[object]
foreach ($day in $days) {
    $schedulePairs.Add([pscustomobject]@{
        SubjectID = $cheSubjectId
        ScheduleID = Get-OrCreateWholeDaySchedule $cheSubjectId $day "WHOLEDAY-CHE" "CHE Demo Lab"
    })
    $schedulePairs.Add([pscustomobject]@{
        SubjectID = $meSubjectId
        ScheduleID = Get-OrCreateWholeDaySchedule $meSubjectId $day "WHOLEDAY-ME" "ME Demo Lab"
    })
}

$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT TOP 20 [UserID], [SchoolID], [FullName], [SchoolEmail], [Password]
FROM [Users]
WHERE [SchoolID] NOT LIKE 'DEMO-%'
AND [IsActive] = True
ORDER BY [UserID]
"@
$reader = $cmd.ExecuteReader()
$users = New-Object System.Collections.Generic.List[object]
while ($reader.Read()) {
    $users.Add([pscustomobject]@{
        UserID = [int]$reader["UserID"]
        SchoolID = $reader["SchoolID"].ToString()
        FullName = $reader["FullName"].ToString()
        Email = $reader["SchoolEmail"].ToString()
        Password = $reader["Password"].ToString()
    })
}
$reader.Close()

foreach ($user in $users) {
    foreach ($schedule in $schedulePairs) {
        Ensure-Enrollment $user.UserID $schedule.SubjectID $schedule.ScheduleID
    }
}

Write-Output ""
Write-Output "Whole-day demo subjects using real subjects:"
Write-Output "CHEM 202 | CHE | Monday-Friday | 00:00-23:59 | Section WHOLEDAY-CHE"
Write-Output "ME 421   | ME  | Monday-Friday | 00:00-23:59 | Section WHOLEDAY-ME"

Write-Output ""
Write-Output "Existing users enrolled in both whole-day subjects:"
$users | ForEach-Object {
    Write-Output ("{0} | {1} | {2} | {3}" -f $_.SchoolID, $_.FullName, $_.Email, $_.Password)
}

$conn.Close()
