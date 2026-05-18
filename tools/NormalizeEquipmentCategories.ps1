$ErrorActionPreference = "Stop"

$db = "C:\Users\HF\Documents\CPE_PROJECT\CCMA\WildCat Hub\WildcatHub\WildcatHub\bin\Database\WildcatHub_LabSystem.accdb"
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backup = [System.IO.Path]::Combine(
    [System.IO.Path]::GetDirectoryName($db),
    "WildcatHub_LabSystem_before_category_normalize_$stamp.accdb")

Copy-Item -LiteralPath $db -Destination $backup
Write-Output "Backup created: $backup"

Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.OleDb.OleDbConnection(
    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db;Persist Security Info=False;")
$conn.Open()

function Invoke-NonQuery($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) {
        $null = $cmd.Parameters.AddWithValue("@p", $p)
    }
    [void]$cmd.ExecuteNonQuery()
}

function Invoke-Scalar($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) {
        $null = $cmd.Parameters.AddWithValue("@p", $p)
    }
    return $cmd.ExecuteScalar()
}

function Ensure-CategoryTable {
    $tables = $conn.GetSchema("Tables")
    $exists = $false
    foreach ($row in $tables.Rows) {
        if ([string]::Equals($row["TABLE_NAME"], "EquipmentCategories", [System.StringComparison]::OrdinalIgnoreCase)) {
            $exists = $true
            break
        }
    }

    if (-not $exists) {
        Invoke-NonQuery @"
CREATE TABLE EquipmentCategories
(
    CategoryID AUTOINCREMENT PRIMARY KEY,
    LabID INTEGER,
    CategoryName TEXT(100),
    IsActive YESNO
)
"@
    }
}

function Set-LabCategories($labId, [string[]]$categories) {
    Invoke-NonQuery "UPDATE [EquipmentCategories] SET [IsActive] = False WHERE [LabID] = ?" @($labId)

    foreach ($category in $categories) {
        $existing = Invoke-Scalar "SELECT [CategoryID] FROM [EquipmentCategories] WHERE [LabID] = ? AND [CategoryName] = ?" @($labId, $category)
        if ($existing -ne $null -and $existing -ne [DBNull]::Value) {
            Invoke-NonQuery "UPDATE [EquipmentCategories] SET [IsActive] = True WHERE [CategoryID] = ?" @([int]$existing)
        }
        else {
            Invoke-NonQuery "INSERT INTO [EquipmentCategories] ([LabID], [CategoryName], [IsActive]) VALUES (?, ?, True)" @($labId, $category)
        }
    }
}

function Set-Category($labId, $fromCategory, $toCategory) {
    Invoke-NonQuery "UPDATE [Equipment] SET [Category] = ? WHERE [LabID] = ? AND [Category] = ?" @($toCategory, $labId, $fromCategory)
}

Ensure-CategoryTable

$cheCategories = @(
    "Apparatus",
    "Glassware",
    "Measuring Instruments",
    "Process Equipment",
    "Safety Materials"
)

$meCategories = @(
    "Thermal Equipment",
    "Fluid Equipment",
    "Machine Tools",
    "Measuring Instruments",
    "Safety Materials"
)

$ceCategories = @(
    "Surveying Equipment",
    "Soil Testing",
    "Materials Testing",
    "Hydraulics Equipment",
    "Safety Materials"
)

Set-LabCategories 1 $cheCategories
Set-LabCategories 2 $meCategories
Set-LabCategories 3 $ceCategories

$cheMap = @{
    "Apparatus" = "Apparatus"
    "Consumables" = "Safety Materials"
    "Flow Measurement" = "Measuring Instruments"
    "Fluid Mechanics Equipment" = "Process Equipment"
    "Glassware" = "Glassware"
    "Heat Transfer Equipment" = "Process Equipment"
    "Heating Equipment" = "Process Equipment"
    "Materials" = "Safety Materials"
    "Measuring Instruments" = "Measuring Instruments"
    "Measuring Tools" = "Measuring Instruments"
    "Safety Equipment" = "Safety Materials"
    "Safety Materials" = "Safety Materials"
    "Separation Equipment" = "Process Equipment"
    "Tools" = "Apparatus"
}

$meMap = @{
    "Consumables" = "Safety Materials"
    "Fluid and Pressure Equipment" = "Fluid Equipment"
    "Fluid Mechanics Equipment" = "Fluid Equipment"
    "Heat Transfer Equipment" = "Thermal Equipment"
    "Machine Design Equipment" = "Machine Tools"
    "Machine Design Tools" = "Machine Tools"
    "Manufacturing Tools" = "Machine Tools"
    "Measuring Instruments" = "Measuring Instruments"
    "Safety Equipment" = "Safety Materials"
    "Thermal Instruments" = "Thermal Equipment"
}

$ceMap = @{
    "Concrete Testing Equipment" = "Materials Testing"
    "Hydraulics Equipment" = "Hydraulics Equipment"
    "Materials Testing Equipment" = "Materials Testing"
    "Safety Equipment" = "Safety Materials"
    "Soil Testing Equipment" = "Soil Testing"
    "Soil Testing Materials" = "Soil Testing"
    "Structural Testing Equipment" = "Materials Testing"
    "Surveying Equipment" = "Surveying Equipment"
}

foreach ($entry in $cheMap.GetEnumerator()) { Set-Category 1 $entry.Key $entry.Value }
foreach ($entry in $meMap.GetEnumerator()) { Set-Category 2 $entry.Key $entry.Value }
foreach ($entry in $ceMap.GetEnumerator()) { Set-Category 3 $entry.Key $entry.Value }

Write-Output ""
Write-Output "Normalized categories:"
foreach ($labId in @(1, 2, 3)) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
SELECT L.[LabCode], E.[Category], COUNT(*) AS Cnt
FROM [Equipment] AS E
INNER JOIN [Laboratories] AS L ON E.[LabID] = L.[LabID]
WHERE E.[LabID] = ?
GROUP BY L.[LabCode], E.[Category]
ORDER BY E.[Category]
"@
    $null = $cmd.Parameters.AddWithValue("@p", $labId)
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Output ("{0} | {1} | {2}" -f $reader["LabCode"], $reader["Category"], $reader["Cnt"])
    }
    $reader.Close()
}

$conn.Close()
