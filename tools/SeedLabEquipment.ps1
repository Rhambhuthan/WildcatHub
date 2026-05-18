$ErrorActionPreference = "Stop"

$db = Resolve-Path ".\WildcatHub\WildcatHub\bin\Database\WildcatHub_LabSystem.accdb"
$backup = Join-Path (Split-Path $db) ("WildcatHub_LabSystem_before_equipment_seed_{0}.accdb" -f (Get-Date -Format "yyyyMMdd_HHmmss"))
Copy-Item -LiteralPath $db -Destination $backup

Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db;Persist Security Info=False;")
$conn.Open()

function Exec-NonQuery($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) { [void]$cmd.Parameters.AddWithValue("@p", $p) }
    try {
        [void]$cmd.ExecuteNonQuery()
    } catch {
        Write-Output "FAILED SQL:"
        Write-Output $sql
        Write-Output "PARAMS:"
        foreach ($p in $params) { Write-Output ("  {0} ({1})" -f $p, $p.GetType().FullName) }
        throw
    }
}

function Exec-Scalar($sql, $params = @()) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    foreach ($p in $params) { [void]$cmd.Parameters.AddWithValue("@p", $p) }
    return $cmd.ExecuteScalar()
}

function Get-LabId($code) {
    return [int](Exec-Scalar "SELECT LabID FROM Laboratories WHERE LabCode = ?" @($code))
}

function Get-SubjectId($code) {
    return [int](Exec-Scalar "SELECT SubjectID FROM LabSubjects WHERE SubjectCode = ?" @($code))
}

function Ensure-Category($labId, $category) {
    $existing = Exec-Scalar "SELECT CategoryID FROM EquipmentCategories WHERE LabID = ? AND CategoryName = ?" @($labId, $category)
    if ($existing -eq $null -or $existing -eq [DBNull]::Value) {
        Exec-NonQuery "INSERT INTO EquipmentCategories (LabID, CategoryName, IsActive) VALUES (?, ?, True)" @($labId, $category)
    } else {
        Exec-NonQuery "UPDATE EquipmentCategories SET IsActive = True WHERE CategoryID = ?" @([int]$existing)
    }
}

function Ensure-Equipment($labCode, $name, $category, $brand, $qty, $threshold, $type, $description, $subjects) {
    $labId = Get-LabId $labCode
    Ensure-Category $labId $category

    $equipmentId = Exec-Scalar "SELECT EquipmentID FROM Equipment WHERE LabID = ? AND EquipmentName = ?" @($labId, $name)
    $serialPrefix = ($labCode + "-" + (($name -replace "[^A-Za-z0-9]", "").ToUpper()))
    $serialPreview = ("{0}-001 to {0}-{1:000}" -f $serialPrefix, $qty)

    if ($equipmentId -eq $null -or $equipmentId -eq [DBNull]::Value) {
        Exec-NonQuery @"
INSERT INTO Equipment
(LabID, EquipmentName, Category, Brand, SerialNumber, QuantityTotal, QuantityMaintenance, Status, ImagePath, IsArchived, LowStockThreshold, EquipmentType, HasSerial, Description)
VALUES (?, ?, ?, ?, ?, ?, 0, 'Active', '', False, ?, ?, True, ?)
"@ @($labId, $name, $category, $brand, $serialPreview, $qty, $threshold, $type, $description)
        $equipmentId = [int](Exec-Scalar "SELECT @@IDENTITY")
    } else {
        $equipmentId = [int]$equipmentId
        Exec-NonQuery @"
UPDATE Equipment
SET Category = ?, Brand = ?, SerialNumber = ?, QuantityTotal = ?, Status = 'Active',
    IsArchived = False, LowStockThreshold = ?, EquipmentType = ?, HasSerial = True, Description = ?
WHERE EquipmentID = ?
"@ @($category, $brand, $serialPreview, $qty, $threshold, $type, $description, $equipmentId)
    }

    foreach ($subjectCode in $subjects) {
        $subjectId = Get-SubjectId $subjectCode
        $link = Exec-Scalar "SELECT SubjectEquipmentID FROM SubjectEquipments WHERE SubjectID = ? AND EquipmentID = ?" @($subjectId, $equipmentId)
        if ($link -eq $null -or $link -eq [DBNull]::Value) {
            Exec-NonQuery "INSERT INTO SubjectEquipments (SubjectID, EquipmentID) VALUES (?, ?)" @($subjectId, $equipmentId)
        }
    }

    $unitCount = [int](Exec-Scalar "SELECT COUNT(*) FROM EquipmentUnits WHERE EquipmentID = ?" @($equipmentId))
    if ($unitCount -lt $qty) {
        for ($i = $unitCount + 1; $i -le $qty; $i++) {
            $serial = "{0}-{1:000}" -f $serialPrefix, $i
            $unitCmd = $conn.CreateCommand()
            $unitCmd.CommandText = "INSERT INTO EquipmentUnits (EquipmentID, SerialNumber, UnitStatus, DateAdded) VALUES (?, ?, ?, ?)"
            [void]$unitCmd.Parameters.Add("@p1", [System.Data.OleDb.OleDbType]::Integer)
            $unitCmd.Parameters["@p1"].Value = $equipmentId
            [void]$unitCmd.Parameters.Add("@p2", [System.Data.OleDb.OleDbType]::VarWChar)
            $unitCmd.Parameters["@p2"].Value = $serial
            [void]$unitCmd.Parameters.Add("@p3", [System.Data.OleDb.OleDbType]::VarWChar)
            $unitCmd.Parameters["@p3"].Value = "Available"
            [void]$unitCmd.Parameters.Add("@p4", [System.Data.OleDb.OleDbType]::Date)
            $unitCmd.Parameters["@p4"].Value = [DateTime]::Now
            [void]$unitCmd.ExecuteNonQuery()
        }
    }
}

$items = @(
    # Chemical Engineering Laboratory
    @{L="CHE"; N="Analytical Balance"; C="Measuring Instruments"; B="Ohaus Pioneer"; Q=8; T=2; Ty="Reusable"; S=@("CHEM 101","CHEM102","CHEN 203"); D="Precision balance for mass measurements in chemical preparation and separation experiments."}
    @{L="CHE"; N="Digital pH Meter"; C="Measuring Instruments"; B="Hanna Instruments"; Q=10; T=3; Ty="Reusable"; S=@("CHEM 101","CHEM102","CHEN 203"); D="Benchtop pH meter for acid-base, solution analysis, and process samples."}
    @{L="CHE"; N="Hot Plate Magnetic Stirrer"; C="Heating Equipment"; B="Thermo Scientific"; Q=12; T=3; Ty="Reusable"; S=@("CHEM 101","CHEM 202","CHEN 203"); D="Combined hot plate and magnetic stirrer for heating and mixing solutions."}
    @{L="CHE"; N="Erlenmeyer Flask Set"; C="Glassware"; B="Pyrex"; Q=40; T=8; Ty="Reusable"; S=@("CHEM 101","CHEM102","CHEN 203"); D="Borosilicate flasks used for mixing, titration, and sample holding."}
    @{L="CHE"; N="Burette Clamp and Stand"; C="Apparatus"; B="Eisco"; Q=18; T=4; Ty="Reusable"; S=@("CHEM 101","CHEM102"); D="Support stand and clamp assembly for titration setups."}
    @{L="CHE"; N="Volumetric Pipette Set"; C="Glassware"; B="Kimble Chase"; Q=30; T=6; Ty="Limited Use"; S=@("CHEM 101","CHEM102","CHEN 203"); D="Calibrated pipettes for precise liquid transfer; inspected after each use."}
    @{L="CHE"; N="Fluid Flow Bench"; C="Fluid Mechanics Equipment"; B="Armfield"; Q=6; T=2; Ty="Reusable"; S=@("CHEM 201","CHEN 203"); D="Bench for pump, pipe flow, pressure drop, and flow measurement experiments."}
    @{L="CHE"; N="Rotameter"; C="Flow Measurement"; B="Dwyer"; Q=12; T=3; Ty="Reusable"; S=@("CHEM 201","CHEN 203"); D="Variable-area flowmeter for laboratory liquid and gas flow readings."}
    @{L="CHE"; N="Shell and Tube Heat Exchanger Trainer"; C="Heat Transfer Equipment"; B="Armfield"; Q=5; T=1; Ty="Reusable"; S=@("CHEM 202","CHEN 203"); D="Trainer for heat exchanger performance, LMTD, and heat balance experiments."}
    @{L="CHE"; N="Thermocouple Data Logger"; C="Measuring Instruments"; B="Omega Engineering"; Q=12; T=3; Ty="Reusable"; S=@("CHEM 202","CHEM 201"); D="Multi-channel temperature logger for heat transfer and process monitoring."}
    @{L="CHE"; N="Distillation Column Trainer"; C="Separation Equipment"; B="Gunt Hamburg"; Q=4; T=1; Ty="Reusable"; S=@("CHEN 203"); D="Packed column trainer for distillation and vapor-liquid separation experiments."}
    @{L="CHE"; N="Separatory Funnel"; C="Glassware"; B="Pyrex"; Q=20; T=4; Ty="Limited Use"; S=@("CHEM 101","CHEN 203"); D="Glass funnel for liquid-liquid extraction and phase separation."}
    @{L="CHE"; N="Nitrile Gloves Pack"; C="Safety Materials"; B="Kimberly-Clark"; Q=120; T=25; Ty="One Time Use"; S=@("CHEM 101","CHEM102","CHEM 201","CHEM 202","CHEN 203"); D="Disposable nitrile gloves for chemical handling and general laboratory safety."}
    @{L="CHE"; N="Safety Goggles"; C="Safety Equipment"; B="3M"; Q=50; T=10; Ty="Reusable"; S=@("CHEM 101","CHEM102","CHEM 201","CHEM 202","CHEN 203"); D="Impact-resistant eye protection required for wet laboratory activities."}
    @{L="CHE"; N="Filter Paper Pack"; C="Consumables"; B="Whatman"; Q=150; T=30; Ty="One Time Use"; S=@("CHEM 101","CHEM102","CHEN 203"); D="Round qualitative filter paper for filtration and sample preparation."}

    # Mechanical Engineering Laboratory
    @{L="ME"; N="Digital Vernier Caliper"; C="Measuring Instruments"; B="Mitutoyo"; Q=20; T=4; Ty="Reusable"; S=@("ME 411","ME 431"); D="Precision caliper for dimensional inspection and machine design measurements."}
    @{L="ME"; N="Micrometer Screw Gauge"; C="Measuring Instruments"; B="Starrett"; Q=18; T=4; Ty="Reusable"; S=@("ME 411","ME 431"); D="Outside micrometer for shaft, plate, and specimen thickness measurements."}
    @{L="ME"; N="Thermocouple Kit"; C="Thermal Instruments"; B="Omega Engineering"; Q=16; T=4; Ty="Reusable"; S=@("ME 317","ME 421"); D="Thermocouple probes and connectors for temperature measurement experiments."}
    @{L="ME"; N="Pressure Gauge Set"; C="Fluid and Pressure Equipment"; B="Wika"; Q=18; T=4; Ty="Reusable"; S=@("ME 317","ME 318"); D="Pressure gauges for fluids, thermodynamics, and pump test benches."}
    @{L="ME"; N="Centrifugal Pump Test Rig"; C="Fluid Mechanics Equipment"; B="TecQuipment"; Q=5; T=1; Ty="Reusable"; S=@("ME 318"); D="Pump test rig for head, flow rate, and efficiency experiments."}
    @{L="ME"; N="Venturi Meter Assembly"; C="Fluid Mechanics Equipment"; B="Armfield"; Q=8; T=2; Ty="Reusable"; S=@("ME 318"); D="Venturi assembly for flow rate and pressure differential studies."}
    @{L="ME"; N="Heat Exchanger Trainer"; C="Heat Transfer Equipment"; B="Gunt Hamburg"; Q=5; T=1; Ty="Reusable"; S=@("ME 421","ME 317"); D="Trainer for conduction, convection, and exchanger heat balance activities."}
    @{L="ME"; N="Lathe Tool Bit Set"; C="Manufacturing Tools"; B="Kennametal"; Q=30; T=6; Ty="Limited Use"; S=@("ME 431","ME 411"); D="Tool bit set for turning, facing, and machining process demonstrations."}
    @{L="ME"; N="Welding Shield"; C="Safety Equipment"; B="Lincoln Electric"; Q=18; T=4; Ty="Reusable"; S=@("ME 431"); D="Face shield for welding and hot work laboratory exercises."}
    @{L="ME"; N="Torque Wrench"; C="Machine Design Tools"; B="Snap-on"; Q=12; T=3; Ty="Reusable"; S=@("ME 411","ME 431"); D="Torque wrench for bolted joint and machine assembly experiments."}
    @{L="ME"; N="Gear Train Demonstrator"; C="Machine Design Equipment"; B="TecQuipment"; Q=6; T=2; Ty="Reusable"; S=@("ME 411"); D="Gear train trainer for speed ratio, torque, and mechanism analysis."}
    @{L="ME"; N="Tachometer"; C="Measuring Instruments"; B="Extech"; Q=14; T=3; Ty="Reusable"; S=@("ME 317","ME 318","ME 411"); D="Handheld tachometer for speed measurement of rotating machines."}
    @{L="ME"; N="Cutting Fluid Bottle"; C="Consumables"; B="Mobilcut"; Q=80; T=15; Ty="One Time Use"; S=@("ME 431"); D="Consumable cutting fluid for machining and tooling demonstrations."}
    @{L="ME"; N="Safety Gloves"; C="Safety Equipment"; B="Ansell"; Q=70; T=14; Ty="Limited Use"; S=@("ME 317","ME 318","ME 411","ME 421","ME 431"); D="Reusable industrial gloves for mechanical laboratory handling."}
    @{L="ME"; N="Infrared Thermometer"; C="Thermal Instruments"; B="Fluke"; Q=12; T=3; Ty="Reusable"; S=@("ME 317","ME 421"); D="Non-contact thermometer for temperature checks in heat transfer activities."}

    # Civil Engineering Laboratory
    @{L="CE"; N="Automatic Level"; C="Surveying Equipment"; B="Sokkia"; Q=10; T=2; Ty="Reusable"; S=@("CE 311"); D="Optical level for elevation measurement and surveying field exercises."}
    @{L="CE"; N="Tripod Stand"; C="Surveying Equipment"; B="Topcon"; Q=14; T=3; Ty="Reusable"; S=@("CE 311"); D="Tripod support for levels, total stations, and field instruments."}
    @{L="CE"; N="Surveying Staff"; C="Surveying Equipment"; B="Leica"; Q=18; T=4; Ty="Reusable"; S=@("CE 311"); D="Leveling rod used for height and distance observations."}
    @{L="CE"; N="Total Station"; C="Surveying Equipment"; B="Leica Geosystems"; Q=5; T=1; Ty="Reusable"; S=@("CE 311"); D="Electronic surveying instrument for angle, distance, and coordinate measurements."}
    @{L="CE"; N="Soil Moisture Tin Set"; C="Soil Testing Materials"; B="Humboldt"; Q=60; T=12; Ty="Limited Use"; S=@("CE 312"); D="Moisture tins for water content determination of soil samples."}
    @{L="CE"; N="Atterberg Limits Kit"; C="Soil Testing Equipment"; B="ELE International"; Q=8; T=2; Ty="Reusable"; S=@("CE 312"); D="Kit for liquid limit and plastic limit soil classification tests."}
    @{L="CE"; N="Sieve Analysis Set"; C="Materials Testing Equipment"; B="Gilson"; Q=10; T=2; Ty="Reusable"; S=@("CE 312","CE 431"); D="Stacked sieves for particle size distribution of soil and aggregates."}
    @{L="CE"; N="Hydraulic Bench"; C="Hydraulics Equipment"; B="Armfield"; Q=5; T=1; Ty="Reusable"; S=@("CE 321"); D="Bench for flow measurement, pump, and open-channel hydraulic experiments."}
    @{L="CE"; N="Flow Meter"; C="Hydraulics Equipment"; B="Dwyer"; Q=12; T=3; Ty="Reusable"; S=@("CE 321"); D="Flow measurement device for hydraulics and pipe flow activities."}
    @{L="CE"; N="Compression Testing Machine"; C="Structural Testing Equipment"; B="Controls Group"; Q=4; T=1; Ty="Reusable"; S=@("CE 421","CE 431"); D="Machine for concrete cylinder, cube, and structural specimen compression tests."}
    @{L="CE"; N="Rebar Locator"; C="Structural Testing Equipment"; B="Proceq"; Q=6; T=2; Ty="Reusable"; S=@("CE 421","CE 431"); D="Scanner for locating reinforcement in concrete members."}
    @{L="CE"; N="Concrete Slump Cone Set"; C="Concrete Testing Equipment"; B="Humboldt"; Q=12; T=3; Ty="Reusable"; S=@("CE 431"); D="Cone, rod, and base plate set for concrete slump test."}
    @{L="CE"; N="Concrete Cylinder Mold"; C="Concrete Testing Equipment"; B="Forney"; Q=60; T=12; Ty="Limited Use"; S=@("CE 431","CE 421"); D="Cylinder molds for concrete specimen casting and curing."}
    @{L="CE"; N="Safety Helmet"; C="Safety Equipment"; B="MSA"; Q=60; T=12; Ty="Reusable"; S=@("CE 311","CE 312","CE 321","CE 421","CE 431"); D="Hard hat for field surveying, structures, materials, and hydraulics activities."}
    @{L="CE"; N="Nitrile Coated Work Gloves"; C="Safety Equipment"; B="Ansell"; Q=80; T=16; Ty="Limited Use"; S=@("CE 311","CE 312","CE 321","CE 421","CE 431"); D="Protective gloves for soil, concrete, and field laboratory handling."}
)

foreach ($item in $items) {
    Ensure-Equipment $item.L $item.N $item.C $item.B $item.Q $item.T $item.Ty $item.D $item.S
}

$supplementalLinks = @{
    "CHEM 201" = @("Analytical Balance","Digital pH Meter","Hot Plate Magnetic Stirrer","Erlenmeyer Flask Set","Volumetric Pipette Set","Separatory Funnel","Filter Paper Pack")
    "CHEM 202" = @("Analytical Balance","Digital pH Meter","Erlenmeyer Flask Set","Volumetric Pipette Set","Fluid Flow Bench","Rotameter","Separatory Funnel","Filter Paper Pack")
    "ME 317" = @("Digital Vernier Caliper","Micrometer Screw Gauge","Centrifugal Pump Test Rig","Venturi Meter Assembly","Heat Exchanger Trainer","Torque Wrench","Cutting Fluid Bottle","Infrared Thermometer")
    "ME 318" = @("Digital Vernier Caliper","Micrometer Screw Gauge","Thermocouple Kit","Heat Exchanger Trainer","Torque Wrench","Gear Train Demonstrator","Cutting Fluid Bottle","Infrared Thermometer")
    "ME 411" = @("Thermocouple Kit","Pressure Gauge Set","Centrifugal Pump Test Rig","Venturi Meter Assembly","Heat Exchanger Trainer","Welding Shield","Cutting Fluid Bottle","Infrared Thermometer")
    "ME 421" = @("Digital Vernier Caliper","Micrometer Screw Gauge","Pressure Gauge Set","Centrifugal Pump Test Rig","Venturi Meter Assembly","Lathe Tool Bit Set","Welding Shield","Torque Wrench","Gear Train Demonstrator","Tachometer","Cutting Fluid Bottle","Safety Gloves")
    "ME 431" = @("Thermocouple Kit","Pressure Gauge Set","Centrifugal Pump Test Rig","Venturi Meter Assembly","Heat Exchanger Trainer","Gear Train Demonstrator","Tachometer","Infrared Thermometer")
    "CE 311" = @("Sieve Analysis Set","Hydraulic Bench","Flow Meter","Compression Testing Machine","Rebar Locator","Concrete Slump Cone Set","Concrete Cylinder Mold")
    "CE 312" = @("Automatic Level","Tripod Stand","Surveying Staff","Total Station","Hydraulic Bench","Flow Meter","Compression Testing Machine","Concrete Slump Cone Set","Concrete Cylinder Mold")
    "CE 321" = @("Automatic Level","Tripod Stand","Surveying Staff","Total Station","Soil Moisture Tin Set","Atterberg Limits Kit","Sieve Analysis Set","Compression Testing Machine","Rebar Locator","Concrete Slump Cone Set","Concrete Cylinder Mold")
    "CE 421" = @("Automatic Level","Tripod Stand","Surveying Staff","Total Station","Soil Moisture Tin Set","Atterberg Limits Kit","Sieve Analysis Set","Hydraulic Bench","Flow Meter")
    "CE 431" = @("Automatic Level","Tripod Stand","Surveying Staff","Total Station","Soil Moisture Tin Set","Atterberg Limits Kit","Hydraulic Bench","Flow Meter")
}

foreach ($subjectCode in $supplementalLinks.Keys) {
    $subjectId = Get-SubjectId $subjectCode
    foreach ($equipmentName in $supplementalLinks[$subjectCode]) {
        $equipmentId = Exec-Scalar @"
SELECT E.EquipmentID
FROM Equipment AS E
INNER JOIN LabSubjects AS LS ON E.LabID = LS.LabID
WHERE LS.SubjectID = ? AND E.EquipmentName = ?
"@ @($subjectId, $equipmentName)
        if ($equipmentId -ne $null -and $equipmentId -ne [DBNull]::Value) {
            $link = Exec-Scalar "SELECT SubjectEquipmentID FROM SubjectEquipments WHERE SubjectID = ? AND EquipmentID = ?" @($subjectId, [int]$equipmentId)
            if ($link -eq $null -or $link -eq [DBNull]::Value) {
                Exec-NonQuery "INSERT INTO SubjectEquipments (SubjectID, EquipmentID) VALUES (?, ?)" @($subjectId, [int]$equipmentId)
            }
        }
    }
}

$summary = @()
foreach ($labCode in @("CHE","ME","CE")) {
    $labId = Get-LabId $labCode
    $equipmentCount = Exec-Scalar "SELECT COUNT(*) FROM Equipment WHERE LabID = ?" @($labId)
    $linkCount = Exec-Scalar @"
SELECT COUNT(*)
FROM SubjectEquipments AS SE
INNER JOIN LabSubjects AS LS ON SE.SubjectID = LS.SubjectID
WHERE LS.LabID = ?
"@ @($labId)
    $summary += "{0}: equipment={1}, subject links={2}" -f $labCode, $equipmentCount, $linkCount
}

$conn.Close()
Write-Output "Backup: $backup"
Write-Output ($summary -join "`n")
