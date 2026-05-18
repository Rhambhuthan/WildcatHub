$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$db = Resolve-Path (Join-Path $root "WildcatHub\WildcatHub\bin\Database\WildcatHub_LabSystem.accdb")
$exportDir = Join-Path $root "Exports"
New-Item -ItemType Directory -Force -Path $exportDir | Out-Null

$csvPath = Join-Path $exportDir "students_by_subject_section_schedule.csv"
$dualCsvPath = Join-Path $exportDir "dual_laboratory_students.csv"
$htmlPath = Join-Path $exportDir "students_by_subject_section_schedule_report.html"

Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db;Persist Security Info=False;")
$rows = New-Object System.Collections.Generic.List[object]

try {
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
SELECT
    L.LabName,
    L.LabCode,
    LS.SubjectCode,
    LS.SubjectName,
    SS.Section,
    SS.DayOfWeek,
    SS.StartTime,
    SS.EndTime,
    U.UserID,
    U.SchoolID,
    U.FullName,
    U.SchoolEmail
FROM ((((StudentSubjectEnrollments AS SSE
INNER JOIN Users AS U ON SSE.UserID = U.UserID)
INNER JOIN LabSubjects AS LS ON SSE.SubjectID = LS.SubjectID)
INNER JOIN Laboratories AS L ON LS.LabID = L.LabID)
INNER JOIN SubjectSchedules AS SS ON SSE.ScheduleID = SS.ScheduleID)
WHERE SSE.IsActive = True
ORDER BY L.LabCode, LS.SubjectCode, SS.Section, U.FullName
"@

    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        $rows.Add([pscustomobject]@{
            Laboratory = $reader["LabName"].ToString()
            LabCode = $reader["LabCode"].ToString()
            SubjectCode = $reader["SubjectCode"].ToString()
            SubjectName = $reader["SubjectName"].ToString()
            Section = $reader["Section"].ToString()
            Day = $reader["DayOfWeek"].ToString()
            StartTime = ([datetime]$reader["StartTime"]).ToString("h:mm tt")
            EndTime = ([datetime]$reader["EndTime"]).ToString("h:mm tt")
            UserID = [int]$reader["UserID"]
            SchoolID = $reader["SchoolID"].ToString()
            FullName = $reader["FullName"].ToString()
            SchoolEmail = $reader["SchoolEmail"].ToString()
        }) | Out-Null
    }
    $reader.Close()
}
finally {
    $conn.Close()
}

$byUser = $rows | Group-Object UserID
$userClasses = @{}

foreach ($group in $byUser) {
    $labs = $group.Group | Select-Object -ExpandProperty LabCode -Unique
    $classes = $group.Group | ForEach-Object {
        "$($_.LabCode) - $($_.SubjectCode) / $($_.Section) / $($_.Day) $($_.StartTime)-$($_.EndTime)"
    }

    $userClasses[[int]$group.Name] = [pscustomobject]@{
        LabCount = ($labs | Measure-Object).Count
        Classes = ($classes -join "; ")
    }
}

$exportRows = $rows | ForEach-Object {
    $info = $userClasses[$_.UserID]
    [pscustomobject]@{
        Laboratory = $_.Laboratory
        LabCode = $_.LabCode
        SubjectCode = $_.SubjectCode
        SubjectName = $_.SubjectName
        Section = $_.Section
        Day = $_.Day
        StartTime = $_.StartTime
        EndTime = $_.EndTime
        SchoolID = $_.SchoolID
        FullName = $_.FullName
        SchoolEmail = $_.SchoolEmail
        DualLaboratoryStudent = $(if ($info.LabCount -gt 1) { "Yes" } else { "No" })
        StudentClasses = $info.Classes
    }
}

$dualRows = $byUser |
    Where-Object { ($_.Group | Select-Object -ExpandProperty LabCode -Unique | Measure-Object).Count -gt 1 } |
    ForEach-Object {
        $first = $_.Group[0]
        $classes = $_.Group | ForEach-Object {
            "$($_.LabCode) - $($_.SubjectCode) / $($_.Section) / $($_.Day) $($_.StartTime)-$($_.EndTime)"
        }

        [pscustomobject]@{
            SchoolID = $first.SchoolID
            FullName = $first.FullName
            SchoolEmail = $first.SchoolEmail
            LaboratoryCount = (($_.Group | Select-Object -ExpandProperty LabCode -Unique | Measure-Object).Count)
            Classes = ($classes -join "; ")
        }
    }

$exportRows | Export-Csv -Path $csvPath -NoTypeInformation -Encoding UTF8
$dualRows | Export-Csv -Path $dualCsvPath -NoTypeInformation -Encoding UTF8

$html = New-Object System.Text.StringBuilder
[void]$html.AppendLine('<!doctype html>')
[void]$html.AppendLine('<html><head><meta charset="utf-8"><title>WildcatHub Student Laboratory Schedules</title>')
[void]$html.AppendLine('<style>')
[void]$html.AppendLine('body{font-family:Segoe UI,Arial,sans-serif;margin:28px;color:#2f203a}h1{color:#4b2130}h2{margin-top:28px;color:#7a0019}.meta{color:#6d5a72}table{border-collapse:collapse;width:100%;margin:10px 0 22px}th{background:#8b001a;color:#fff;text-align:left;padding:8px;font-size:13px}td{border:1px solid #ddd;padding:7px;font-size:12px}.dual{background:#fff2cc;font-weight:600}.section{background:#f7f1f8;padding:10px 12px;border-left:5px solid #d4a82d;margin-top:16px}')
[void]$html.AppendLine('</style></head><body>')
[void]$html.AppendLine('<h1>WildcatHub Student Laboratory Schedules</h1>')
[void]$html.AppendLine('<p class="meta">Generated on ' + (Get-Date).ToString('MMMM d, yyyy h:mm tt') + '</p>')
[void]$html.AppendLine('<h2>Dual Laboratory Students</h2>')
[void]$html.AppendLine('<table><tr><th>School ID</th><th>Name</th><th>Email</th><th>Classes</th></tr>')

foreach ($dual in $dualRows) {
    [void]$html.AppendLine('<tr class="dual"><td>' + $dual.SchoolID + '</td><td>' + $dual.FullName + '</td><td>' + $dual.SchoolEmail + '</td><td>' + $dual.Classes + '</td></tr>')
}

[void]$html.AppendLine('</table>')

$groups = $exportRows | Group-Object Laboratory, SubjectCode, Section, Day, StartTime, EndTime | Sort-Object Name
foreach ($group in $groups) {
    $sample = $group.Group[0]
    [void]$html.AppendLine('<div class="section"><strong>' + $sample.Laboratory + ' - ' + $sample.SubjectCode + ' / Section ' + $sample.Section + '</strong><br>' + $sample.Day + ', ' + $sample.StartTime + ' - ' + $sample.EndTime + ' | Students: ' + $group.Count + '</div>')
    [void]$html.AppendLine('<table><tr><th>#</th><th>School ID</th><th>Name</th><th>Email</th><th>Dual Lab?</th></tr>')

    $index = 1
    foreach ($row in ($group.Group | Sort-Object FullName)) {
        $classAttr = if ($row.DualLaboratoryStudent -eq "Yes") { ' class="dual"' } else { "" }
        [void]$html.AppendLine('<tr' + $classAttr + '><td>' + $index + '</td><td>' + $row.SchoolID + '</td><td>' + $row.FullName + '</td><td>' + $row.SchoolEmail + '</td><td>' + $row.DualLaboratoryStudent + '</td></tr>')
        $index++
    }

    [void]$html.AppendLine('</table>')
}

[void]$html.AppendLine('</body></html>')
Set-Content -Path $htmlPath -Value $html.ToString() -Encoding UTF8

Write-Output $csvPath
Write-Output $dualCsvPath
Write-Output $htmlPath
