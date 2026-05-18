$ErrorActionPreference = "Stop"

$db = "C:\Users\HF\Documents\CPE_PROJECT\CCMA\WildCat Hub\WildcatHub\WildcatHub\bin\Database\WildcatHub_LabSystem.accdb"
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backup = [System.IO.Path]::Combine(
    [System.IO.Path]::GetDirectoryName($db),
    "WildcatHub_LabSystem_before_demo_seed_$stamp.accdb")

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

function Get-OrCreateSubject($labId, $code, $name) {
    $id = Invoke-Scalar "SELECT SubjectID FROM LabSubjects WHERE SubjectCode = ?" @($code)
    if ($id -ne $null -and $id -ne [DBNull]::Value) {
        Invoke-NonQuery "UPDATE [LabSubjects] SET [LabID] = ?, [SubjectName] = ?, [IsActive] = True WHERE [SubjectID] = ?" @($labId, $name, [int]$id)
        return [int]$id
    }

    Invoke-NonQuery "INSERT INTO [LabSubjects] ([LabID], [SubjectCode], [SubjectName], [IsActive]) VALUES (?, ?, ?, True)" @($labId, $code, $name)
    return [int](Invoke-Scalar "SELECT @@IDENTITY")
}

function Get-OrCreateSchedule($subjectId, $day, $section, $room) {
    $id = Invoke-Scalar "SELECT [ScheduleID] FROM [SubjectSchedules] WHERE [SubjectID] = ? AND [DayOfWeek] = ? AND [Section] = ?" @($subjectId, $day, $section)
    if ($id -ne $null -and $id -ne [DBNull]::Value) {
        Invoke-NonQuery "UPDATE [SubjectSchedules] SET [StartTime] = ?, [EndTime] = ?, [Room] = ? WHERE [ScheduleID] = ?" @([datetime]"1899-12-30 00:00:00", [datetime]"1899-12-30 23:59:00", $room, [int]$id)
        return [int]$id
    }

    Invoke-NonQuery "INSERT INTO [SubjectSchedules] ([SubjectID], [DayOfWeek], [StartTime], [EndTime], [Section], [Room]) VALUES (?, ?, ?, ?, ?, ?)" @($subjectId, $day, [datetime]"1899-12-30 00:00:00", [datetime]"1899-12-30 23:59:00", $section, $room)
    return [int](Invoke-Scalar "SELECT @@IDENTITY")
}

function Get-OrCreateUser($index, $firstName, $lastName) {
    $schoolId = "DEMO-{0:D3}" -f $index
    $email = ("{0}.{1}{2:D2}@cit.edu" -f $firstName.ToLower(), $lastName.ToLower(), $index)
    $fullName = "$firstName $lastName"
    $password = "demo{0:D3}" -f $index

    $id = Invoke-Scalar "SELECT [UserID] FROM [Users] WHERE [SchoolID] = ?" @($schoolId)
    if ($id -ne $null -and $id -ne [DBNull]::Value) {
        Invoke-NonQuery "UPDATE [Users] SET [FullName] = ?, [SchoolEmail] = ?, [Password] = ?, [IsActive] = True WHERE [UserID] = ?" @($fullName, $email, $password, [int]$id)
        return [pscustomobject]@{
            UserID = [int]$id
            SchoolID = $schoolId
            FullName = $fullName
            Email = $email
            Password = $password
        }
    }

    Invoke-NonQuery "INSERT INTO [Users] ([SchoolID], [FullName], [SchoolEmail], [Password], [IsActive]) VALUES (?, ?, ?, ?, True)" @($schoolId, $fullName, $email, $password)
    return [pscustomobject]@{
        UserID = [int](Invoke-Scalar "SELECT @@IDENTITY")
        SchoolID = $schoolId
        FullName = $fullName
        Email = $email
        Password = $password
    }
}

function EnsureEnrollment($userId, $subjectId, $scheduleId) {
    $id = Invoke-Scalar "SELECT [EnrollmentID] FROM [StudentSubjectEnrollments] WHERE [UserID] = ? AND [SubjectID] = ? AND [ScheduleID] = ?" @($userId, $subjectId, $scheduleId)
    if ($id -ne $null -and $id -ne [DBNull]::Value) {
        Invoke-NonQuery "UPDATE [StudentSubjectEnrollments] SET [IsActive] = True WHERE [EnrollmentID] = ?" @([int]$id)
        return
    }

    Invoke-NonQuery "INSERT INTO [StudentSubjectEnrollments] ([UserID], [SubjectID], [ScheduleID], [IsActive]) VALUES (?, ?, ?, True)" @($userId, $subjectId, $scheduleId)
}

function LinkLabEquipment($labId, $subjectId) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT TOP 15 [EquipmentID] FROM [Equipment] WHERE [LabID] = ? AND [IsArchived] = False AND [Status] = 'Active' ORDER BY [EquipmentID]"
    $null = $cmd.Parameters.AddWithValue("@p", $labId)
    $reader = $cmd.ExecuteReader()
    $equipmentIds = New-Object System.Collections.Generic.List[int]
    while ($reader.Read()) {
        $equipmentIds.Add([int]$reader["EquipmentID"])
    }
    $reader.Close()

    foreach ($equipmentId in $equipmentIds) {
        $existing = Invoke-Scalar "SELECT [SubjectEquipmentID] FROM [SubjectEquipments] WHERE [SubjectID] = ? AND [EquipmentID] = ?" @($subjectId, $equipmentId)
        if ($existing -eq $null -or $existing -eq [DBNull]::Value) {
            Invoke-NonQuery "INSERT INTO [SubjectEquipments] ([SubjectID], [EquipmentID]) VALUES (?, ?)" @($subjectId, $equipmentId)
        }
    }
}

$cheSubjectId = Get-OrCreateSubject 1 "DEMO-CHE101" "Demo Chemical Engineering Laboratory"
$meSubjectId = Get-OrCreateSubject 2 "DEMO-ME101" "Demo Mechanical Engineering Laboratory"

$days = @("Monday", "Tuesday", "Wednesday", "Thursday", "Friday")
$scheduleIds = New-Object System.Collections.Generic.List[object]
foreach ($day in $days) {
    $scheduleIds.Add([pscustomobject]@{
        SubjectID = $cheSubjectId
        ScheduleID = Get-OrCreateSchedule $cheSubjectId $day "DEMO-CHE" "Demo CHE Lab"
    })
    $scheduleIds.Add([pscustomobject]@{
        SubjectID = $meSubjectId
        ScheduleID = Get-OrCreateSchedule $meSubjectId $day "DEMO-ME" "Demo ME Lab"
    })
}

$names = @(
    @("Alyssa", "Demo"),
    @("Brian", "Demo"),
    @("Camille", "Demo"),
    @("Darren", "Demo"),
    @("Elise", "Demo"),
    @("Francis", "Demo"),
    @("Gianne", "Demo"),
    @("Harvey", "Demo"),
    @("Iris", "Demo"),
    @("Jared", "Demo"),
    @("Kyla", "Demo"),
    @("Lance", "Demo"),
    @("Mika", "Demo"),
    @("Nico", "Demo"),
    @("Olivia", "Demo"),
    @("Paolo", "Demo"),
    @("Quinn", "Demo"),
    @("Rina", "Demo"),
    @("Sean", "Demo"),
    @("Tala", "Demo")
)

$users = New-Object System.Collections.Generic.List[object]
for ($i = 1; $i -le 20; $i++) {
    $user = Get-OrCreateUser $i $names[$i - 1][0] $names[$i - 1][1]
    $users.Add($user)

    foreach ($schedule in $scheduleIds) {
        EnsureEnrollment $user.UserID $schedule.SubjectID $schedule.ScheduleID
    }
}

LinkLabEquipment 1 $cheSubjectId
LinkLabEquipment 2 $meSubjectId

Write-Output ""
Write-Output "Demo users:"
$users | ForEach-Object {
    Write-Output ("{0} | {1} | {2} | {3}" -f $_.SchoolID, $_.FullName, $_.Email, $_.Password)
}

Write-Output ""
Write-Output "Demo subjects:"
Write-Output "DEMO-CHE101 | CHE | Monday-Friday | 00:00-23:59 | Section DEMO-CHE"
Write-Output "DEMO-ME101  | ME  | Monday-Friday | 00:00-23:59 | Section DEMO-ME"

$conn.Close()
