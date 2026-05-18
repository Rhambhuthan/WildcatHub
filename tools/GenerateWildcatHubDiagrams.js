const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "..");
const outDir = path.join(root, "Exports", "Diagrams");
fs.mkdirSync(outDir, { recursive: true });

function esc(value) {
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function wrapText(text, max = 24) {
  const words = String(text).split(/\s+/);
  const lines = [];
  let current = "";
  for (const word of words) {
    if ((current + " " + word).trim().length > max && current) {
      lines.push(current);
      current = word;
    } else {
      current = (current + " " + word).trim();
    }
  }
  if (current) lines.push(current);
  return lines;
}

function featureSvg() {
  const classes = [
    {
      id: "users",
      x: 610,
      y: 70,
      name: "Users",
      attributes: ["UserID", "FullName", "SchoolID / AdminID", "Email", "Password", "Role"],
      methods: ["Login()", "Logout()", "ViewDashboard()"],
    },
    {
      id: "admin",
      x: 120,
      y: 300,
      name: "Admin",
      attributes: ["AdminID", "AdminFullName", "OfficeEmail", "LabID", "Role"],
      methods: ["ManageStudents()", "ManageEquipment()", "ApproveSlip()", "ReturnEquipment()", "CreateReport()"],
    },
    {
      id: "student",
      x: 1100,
      y: 300,
      name: "Students",
      attributes: ["UserID", "SchoolID", "FullName", "SchoolEmail", "IsActive", "RestrictionStatus"],
      methods: ["ViewEquipment()", "AddToCart()", "SubmitSlip()", "ViewNotifications()", "ViewHistory()"],
    },
    {
      id: "equipment",
      x: 120,
      y: 610,
      name: "Equipment",
      attributes: ["EquipmentID", "Name", "Category", "Brand", "AvailableQuantity", "LowStockThreshold", "EquipmentType"],
      methods: ["AddEquipment()", "EditEquipment()", "ArchiveEquipment()", "AssignSubject()", "UpdateStock()"],
    },
    {
      id: "slip",
      x: 610,
      y: 610,
      name: "Borrower's Slip",
      attributes: ["SlipID", "LeaderName", "Members", "Subject", "Schedule", "GroupNumber", "SlipStatus"],
      methods: ["SubmitSlip()", "EditPendingSlip()", "ApproveSlip()", "DeclineSlip()", "MarkClaimed()"],
    },
    {
      id: "report",
      x: 1100,
      y: 610,
      name: "Equipment Report",
      attributes: ["ReportID", "EquipmentID", "DamageType", "DamageQuantity", "ReplacementCost", "ReportStatus"],
      methods: ["CreateReport()", "SetCost()", "NotifyMembers()", "RestrictLab()", "MarkPaid()"],
    },
    {
      id: "notification",
      x: 365,
      y: 930,
      name: "Notifications",
      attributes: ["NotificationType", "Recipient", "Message", "Status", "DateCreated"],
      methods: ["SendSlipAlert()", "SendLowStockAlert()", "SendOverdueAlert()", "SendReportAlert()"],
    },
    {
      id: "history",
      x: 855,
      y: 930,
      name: "History",
      attributes: ["SlipID", "BorrowDate", "ReturnDate", "Status", "EquipmentList"],
      methods: ["ViewTransaction()", "ViewSlipDetails()", "FilterHistory()"],
    },
  ];

  const arrows = [
    ["users", "admin", "has role"],
    ["users", "student", "has role"],
    ["admin", "equipment", "manages"],
    ["student", "slip", "submits"],
    ["slip", "admin", "reviewed by"],
    ["slip", "equipment", "borrows"],
    ["admin", "report", "creates"],
    ["report", "student", "restricts / notifies"],
    ["equipment", "notification", "low stock"],
    ["report", "notification", "report alert"],
    ["slip", "history", "returned slips"],
  ];

  return renderClassDiagram(1540, 1220, classes, arrows, "Diagram");
}

const erdEntities = [
  { id: "Laboratories", x: 60, y: 95, fields: ["PK LabID", "LabName", "LabCode", "IsActive"] },
  { id: "AdminCredentials", x: 60, y: 305, fields: ["PK AdminID", "FK LabID", "AdminFullName", "OfficeEmail", "Role", "IsActive"] },
  { id: "Users", x: 60, y: 555, fields: ["PK UserID", "SchoolID", "FullName", "SchoolEmail", "Password", "IsActive"] },

  { id: "LabSubjects", x: 390, y: 95, fields: ["PK SubjectID", "FK LabID", "SubjectCode", "SubjectName", "IsActive"] },
  { id: "SubjectSchedules", x: 390, y: 335, fields: ["PK ScheduleID", "FK SubjectID", "DayOfWeek", "StartTime", "EndTime", "Section", "Room"] },
  { id: "StudentSubjectEnrollments", x: 390, y: 605, fields: ["PK EnrollmentID", "FK UserID", "FK SubjectID", "FK ScheduleID", "IsActive"] },

  { id: "EquipmentCategories", x: 720, y: 95, fields: ["PK CategoryID", "FK LabID", "CategoryName", "IsActive"] },
  { id: "Equipment", x: 720, y: 300, fields: ["PK EquipmentID", "FK LabID", "EquipmentName", "Category", "Brand", "QuantityTotal", "LowStockThreshold", "EquipmentType", "HasSerial", "IsArchived"] },
  { id: "EquipmentUnits", x: 720, y: 625, fields: ["PK UnitID", "FK EquipmentID", "SerialNumber", "UnitStatus", "DateAdded"] },
  { id: "SubjectEquipments", x: 720, y: 850, fields: ["PK SubjectEquipmentID", "FK SubjectID", "FK EquipmentID"] },

  { id: "BorrowSlips", x: 1050, y: 95, fields: ["PK SlipID", "FK UserID", "FK SubjectID", "FK ScheduleID", "FK ExperimentID", "GroupNumber", "LeaderName", "SlipStatus", "ApprovedByAdminID"] },
  { id: "BorrowSlipItems", x: 1050, y: 420, fields: ["PK SlipItemID", "FK SlipID", "FK EquipmentID", "QuantityRequested", "QuantityReturned", "ItemReturnStatus"] },
  { id: "BorrowSlipMembers", x: 1050, y: 685, fields: ["PK MemberID", "FK SlipID", "FK UserID", "MemberName"] },
  { id: "BorrowSlipUnits", x: 1050, y: 900, fields: ["PK BorrowSlipUnitID", "FK SlipItemID", "FK UnitID", "DateAssigned"] },

  { id: "DamageReports", x: 1380, y: 95, fields: ["PK ReportID", "FK SlipID", "FK SlipItemID", "FK EquipmentID", "FK SubjectID", "FK ScheduleID", "FK ReportedByAdminID", "DamageType", "DamageQuantity", "ReportStatus", "ReplacementCost"] },
  { id: "DamageReportMembers", x: 1380, y: 455, fields: ["PK ReportMemberID", "FK ReportID", "FK UserID", "AmountShare", "IsRestricted", "HasPaid"] },
  { id: "DamageReportUnits", x: 1380, y: 700, fields: ["PK DamageReportUnitID", "FK ReportID", "FK UnitID"] },

  { id: "Experiments", x: 390, y: 1050, fields: ["PK ExperimentID", "FK SubjectID", "ExperimentNumber", "ExperimentTitle", "Description", "IsActive"] },
  { id: "ExperimentItems", x: 720, y: 1080, fields: ["PK ExperimentItemID", "FK ExperimentID", "FK EquipmentID", "RequiredQuantity"] },
  { id: "ExperimentManuals", x: 1050, y: 1120, fields: ["PK ExperimentID", "FK SubjectID", "ExperimentName", "IsActive", "DateCreated"] },
  { id: "ExperimentManualItems", x: 1380, y: 1120, fields: ["PK ManualItemID", "FK ExperimentID", "FK EquipmentID", "QuantityNeeded"] },
];

function erdSvg() {
  const nodes = {};
  for (const entity of erdEntities) {
    nodes[entity.id] = {
      x: entity.x,
      y: entity.y,
      w: 240,
      h: 42 + entity.fields.length * 22,
      title: entity.id,
      lines: entity.fields,
    };
  }

  const edges = [
    ["Laboratories", "AdminCredentials"],
    ["Laboratories", "LabSubjects"],
    ["Laboratories", "EquipmentCategories"],
    ["LabSubjects", "SubjectSchedules"],
    ["SubjectSchedules", "StudentSubjectEnrollments"],
    ["Users", "StudentSubjectEnrollments"],
    ["LabSubjects", "SubjectEquipments"],
    ["EquipmentCategories", "Equipment"],
    ["Equipment", "EquipmentUnits"],
    ["Equipment", "SubjectEquipments"],
    ["Users", "BorrowSlips"],
    ["BorrowSlips", "BorrowSlipItems"],
    ["BorrowSlips", "BorrowSlipMembers"],
    ["BorrowSlipItems", "BorrowSlipUnits"],
    ["Equipment", "BorrowSlipItems"],
    ["BorrowSlips", "DamageReports"],
    ["BorrowSlipItems", "DamageReports"],
    ["DamageReports", "DamageReportMembers"],
    ["DamageReports", "DamageReportUnits"],
    ["LabSubjects", "Experiments"],
    ["Experiments", "ExperimentItems"],
    ["LabSubjects", "ExperimentManuals"],
    ["ExperimentManuals", "ExperimentManualItems"],
  ];

  const bands = [
    { x: 35, y: 55, w: 280, h: 800, title: "Accounts / Labs" },
    { x: 365, y: 55, w: 280, h: 980, title: "Subjects / Schedules" },
    { x: 695, y: 55, w: 280, h: 980, title: "Equipment" },
    { x: 1025, y: 55, w: 280, h: 980, title: "Borrowing" },
    { x: 1355, y: 55, w: 280, h: 980, title: "Reports / Restrictions" },
    { x: 365, y: 1035, w: 1270, h: 250, title: "Experiment Manuals" },
  ];

  const notes = [
    { x: 60, y: 900, w: 255, h: 210, title: "Other foreign keys", lines: ["BorrowSlips also link to SubjectID, ScheduleID, ExperimentID, ApprovedByAdminID", "DamageReports also link to EquipmentID, SubjectID, ScheduleID, ReportedByAdminID", "DamageReportMembers links reports to users for restriction/payment"] },
    { x: 720, y: 1045, w: 255, h: 120, title: "Manual links", lines: ["ExperimentItems and ManualItems also reference EquipmentID"] },
  ];

  return renderBoxDiagram(1680, 1325, nodes, edges, "WildcatHub Entity Relationship Diagram", true, bands, notes);
}

function renderBoxDiagram(width, height, nodes, edges, title, compact = false, bands = [], notes = []) {
  const parts = [];
  parts.push(`<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">`);
  parts.push(`<defs><marker id="arrow" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto" markerUnits="strokeWidth"><path d="M0,0 L0,6 L9,3 z" fill="#3c2a45"/></marker></defs>`);
  parts.push(`<rect width="100%" height="100%" fill="#fffaf3"/>`);
  parts.push(`<text x="${width / 2}" y="34" text-anchor="middle" font-family="Segoe UI, Arial" font-size="24" font-weight="700" fill="#4b2130">${esc(title)}</text>`);

  for (const band of bands) {
    parts.push(`<rect x="${band.x}" y="${band.y}" width="${band.w}" height="${band.h}" rx="18" fill="#f8f1f7" stroke="#d9c7dc" stroke-width="1.2"/>`);
    parts.push(`<text x="${band.x + 18}" y="${band.y + 30}" font-family="Segoe UI, Arial" font-size="16" font-weight="700" fill="#8b001a">${esc(band.title)}</text>`);
  }

  for (const [from, to] of edges) {
    const a = nodes[from];
    const b = nodes[to];
    if (!a || !b) continue;
    const p1 = edgePoint(a, b);
    const p2 = edgePoint(b, a);
    parts.push(`<path d="${connectorPath(p1, p2)}" fill="none" stroke="#3c2a45" stroke-width="${compact ? 1 : 1.35}" marker-end="url(#arrow)" opacity="0.58"/>`);
  }

  for (const node of Object.values(nodes)) {
    parts.push(`<rect x="${node.x}" y="${node.y}" width="${node.w}" height="${node.h}" rx="7" fill="#ffffff" stroke="#1f1624" stroke-width="1.5"/>`);
    parts.push(`<rect x="${node.x}" y="${node.y}" width="${node.w}" height="34" rx="7" fill="#8b001a"/>`);
    parts.push(`<rect x="${node.x}" y="${node.y + 26}" width="${node.w}" height="8" fill="#8b001a"/>`);
    parts.push(`<text x="${node.x + node.w / 2}" y="${node.y + 23}" text-anchor="middle" font-family="Segoe UI, Arial" font-size="${compact ? 12 : 14}" font-weight="700" fill="#fff7d6">${esc(node.title)}</text>`);
    let y = node.y + 55;
    for (const line of node.lines || []) {
      const wrapped = wrapText(line, compact ? 26 : 30);
      for (const part of wrapped) {
        parts.push(`<text x="${node.x + 14}" y="${y}" font-family="Segoe UI, Arial" font-size="${compact ? 11 : 13}" fill="#26142f">${esc(part)}</text>`);
        y += compact ? 18 : 20;
      }
    }
  }

  for (const note of notes) {
    parts.push(`<rect x="${note.x}" y="${note.y}" width="${note.w}" height="${note.h}" rx="10" fill="#fff8df" stroke="#d4a82d" stroke-width="1.2"/>`);
    parts.push(`<text x="${note.x + 14}" y="${note.y + 25}" font-family="Segoe UI, Arial" font-size="13" font-weight="700" fill="#8b001a">${esc(note.title)}</text>`);
    let y = note.y + 50;
    for (const line of note.lines || []) {
      for (const part of wrapText(line, compact ? 28 : 34)) {
        parts.push(`<text x="${note.x + 14}" y="${y}" font-family="Segoe UI, Arial" font-size="${compact ? 10.5 : 12}" fill="#26142f">${esc(part)}</text>`);
        y += compact ? 16 : 18;
      }
    }
  }

  parts.push(`</svg>`);
  return parts.join("\n");
}

function renderClassDiagram(width, height, classes, arrows, title) {
  const parts = [];
  const classMap = new Map(classes.map((item) => [item.id, item]));

  parts.push(`<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">`);
  parts.push(`<defs><marker id="arrowClass" markerWidth="12" markerHeight="12" refX="10" refY="4" orient="auto" markerUnits="strokeWidth"><path d="M0,0 L0,8 L11,4 z" fill="#4b2130"/></marker></defs>`);
  parts.push(`<rect width="100%" height="100%" fill="#fffaf3"/>`);
  parts.push(`<text x="${width / 2}" y="38" text-anchor="middle" font-family="Segoe UI, Arial" font-size="26" font-weight="700" fill="#4b2130">${esc(title)}</text>`);

  for (const [from, to, label] of arrows) {
    const start = classMap.get(from);
    const end = classMap.get(to);
    if (!start || !end) continue;

    const p1 = classTitlePoint(start, end);
    const p2 = classTitlePoint(end, start);
    const d = labeledConnectorPath(p1, p2);
    const mid = connectorMidpoint(p1, p2);

    parts.push(`<path d="${d}" fill="none" stroke="#4b2130" stroke-width="1.45" marker-end="url(#arrowClass)" opacity="0.9"/>`);
    parts.push(`<rect x="${mid.x - 54}" y="${mid.y - 14}" width="108" height="22" rx="8" fill="#fffaf3" stroke="#d4a82d" stroke-width="0.8"/>`);
    parts.push(`<text x="${mid.x}" y="${mid.y + 2}" text-anchor="middle" font-family="Segoe UI, Arial" font-size="11" fill="#4b2130">${esc(label)}</text>`);
  }

  for (const item of classes) {
    drawClassBlock(parts, item);
  }

  parts.push(`</svg>`);
  return parts.join("\n");
}

function drawClassBlock(parts, item) {
  const nameW = 200;
  const nameH = 42;
  const boxW = 200;
  const attrH = Math.max(120, 36 + item.attributes.length * 19);
  const methodH = Math.max(120, 36 + item.methods.length * 19);
  const boxX = item.x;
  const attrY = item.y + 70;
  const methodY = attrY + attrH + 24;

  parts.push(`<rect x="${item.x}" y="${item.y}" width="${nameW}" height="${nameH}" rx="7" fill="#ffffff" stroke="#1f1624" stroke-width="1.4"/>`);
  parts.push(`<text x="${item.x + nameW / 2}" y="${item.y + 27}" text-anchor="middle" font-family="Segoe UI, Arial" font-size="14" font-weight="700" fill="#26142f">${esc(item.name)}</text>`);

  const centerX = item.x + nameW / 2;
  parts.push(`<line x1="${centerX}" y1="${item.y + nameH}" x2="${centerX}" y2="${attrY}" stroke="#1f1624" stroke-width="1.1"/>`);
  parts.push(`<line x1="${centerX}" y1="${attrY + attrH}" x2="${centerX}" y2="${methodY}" stroke="#1f1624" stroke-width="1.1"/>`);

  drawListBox(parts, boxX, attrY, boxW, attrH, "Attributes", item.attributes);
  drawListBox(parts, boxX, methodY, boxW, methodH, "Methods", item.methods);
}

function drawListBox(parts, x, y, w, h, title, lines) {
  parts.push(`<rect x="${x}" y="${y}" width="${w}" height="${h}" rx="6" fill="#ffffff" stroke="#1f1624" stroke-width="1.2"/>`);
  parts.push(`<rect x="${x}" y="${y}" width="${w}" height="30" rx="6" fill="#8b001a"/>`);
  parts.push(`<rect x="${x}" y="${y + 22}" width="${w}" height="8" fill="#8b001a"/>`);
  parts.push(`<text x="${x + w / 2}" y="${y + 21}" text-anchor="middle" font-family="Segoe UI, Arial" font-size="12" font-weight="700" fill="#fff7d6">${esc(title)}</text>`);
  let lineY = y + 52;
  for (const line of lines) {
    for (const part of wrapText(line, 24)) {
      parts.push(`<text x="${x + w / 2}" y="${lineY}" text-anchor="middle" font-family="Segoe UI, Arial" font-size="11.5" fill="#26142f">${esc(part)}</text>`);
      lineY += 17;
    }
  }
}

function classTitlePoint(from, to) {
  const fromBox = { x: from.x, y: from.y, w: 200, h: 42 };
  const toBox = { x: to.x, y: to.y, w: 200, h: 42 };
  return edgePoint(fromBox, toBox);
}

function connectorPath(p1, p2) {
  const dx = Math.abs(p2.x - p1.x);
  const dy = Math.abs(p2.y - p1.y);
  if (dx < 8 || dy < 8) {
    return `M ${p1.x} ${p1.y} L ${p2.x} ${p2.y}`;
  }

  const midX = (p1.x + p2.x) / 2;
  return `M ${p1.x} ${p1.y} L ${midX} ${p1.y} L ${midX} ${p2.y} L ${p2.x} ${p2.y}`;
}

function labeledConnectorPath(p1, p2) {
  const dx = Math.abs(p2.x - p1.x);
  const dy = Math.abs(p2.y - p1.y);
  if (dx < 8 || dy < 8) {
    return `M ${p1.x} ${p1.y} L ${p2.x} ${p2.y}`;
  }

  const midX = (p1.x + p2.x) / 2;
  return `M ${p1.x} ${p1.y} L ${midX} ${p1.y} L ${midX} ${p2.y} L ${p2.x} ${p2.y}`;
}

function connectorMidpoint(p1, p2) {
  return {
    x: (p1.x + p2.x) / 2,
    y: (p1.y + p2.y) / 2,
  };
}

function edgePoint(from, to) {
  const fx = from.x + from.w / 2;
  const fy = from.y + from.h / 2;
  const tx = to.x + to.w / 2;
  const ty = to.y + to.h / 2;
  const dx = tx - fx;
  const dy = ty - fy;

  if (Math.abs(dx) > Math.abs(dy)) {
    return { x: dx > 0 ? from.x + from.w : from.x, y: fy };
  }

  return { x: fx, y: dy > 0 ? from.y + from.h : from.y };
}

const featureMermaid = `flowchart TB
    Users[Users] --> Admin[Admin]
    Users --> Students[Students]

    Admin --> AdminDashboard[Dashboard\\nToday schedules, top returned equipment, workload, notifications]
    Admin --> StudentManagement[Students Tab\\nView details, activate/deactivate users]
    Admin --> EquipmentManagement[Equipment Tab\\nAdd/edit/archive equipment, manage categories, assign subjects]
    Admin --> SlipManagement[Slips Tab\\nApprove/decline pending slips, mark claimed]
    Admin --> BorrowedManagement[Borrowed Tab\\nView active borrowed slips, return items, create reports]
    Admin --> ReportManagement[Reports Drawer\\nSet cost, notify users, restrict/unrestrict after payment]
    Admin --> History[History Tab\\nView returned transactions]

    Students --> StudentDashboard[Dashboard\\nStatus, borrowed, borrowing, overdue, notifications]
    Students --> EquipmentBrowse[Equipment Tab\\nSchedule-based equipment list, search, category filter]
    Students --> Cart[Cart / Borrower's Slip\\nAdd equipment, quantity, members, submit slip]
    Students --> UserBorrowed[Borrowed Tab\\nActive group borrowings]
    Students --> UserHistory[History Tab\\nReturned group transactions]
    Students --> UserReports[Report Notifications\\nView report, receipt, cost to pay]

    EquipmentBrowse --> ScheduleCheck[Check current lab schedule]
    ScheduleCheck --> SubjectEquipment[SubjectEquipments]
    Cart --> BorrowSlips
    SlipManagement --> BorrowSlips
    BorrowedManagement --> BorrowSlipItems
    BorrowedManagement --> DamageReports
    ReportManagement --> DamageReportMembers
    DamageReportMembers --> StudentDashboard
    EquipmentManagement --> Equipment
    EquipmentManagement --> EquipmentCategories
    EquipmentManagement --> EquipmentUnits
    History --> BorrowSlips
`;

const erdMermaid = `erDiagram
    Laboratories ||--o{ AdminCredentials : manages
    Laboratories ||--o{ LabSubjects : offers
    Laboratories ||--o{ Equipment : owns
    Laboratories ||--o{ EquipmentCategories : defines

    LabSubjects ||--o{ SubjectSchedules : schedules
    LabSubjects ||--o{ SubjectEquipments : requires
    LabSubjects ||--o{ StudentSubjectEnrollments : enrolls
    LabSubjects ||--o{ BorrowSlips : used_in
    LabSubjects ||--o{ DamageReports : reported_under
    LabSubjects ||--o{ Experiments : has
    LabSubjects ||--o{ ExperimentManuals : has

    Users ||--o{ StudentSubjectEnrollments : enrolled
    Users ||--o{ BorrowSlips : leads
    Users ||--o{ BorrowSlipMembers : member
    Users ||--o{ DamageReportMembers : pays

    SubjectSchedules ||--o{ StudentSubjectEnrollments : assigned
    SubjectSchedules ||--o{ BorrowSlips : deadline_source
    SubjectSchedules ||--o{ DamageReports : restricts_lab_time

    Equipment ||--o{ SubjectEquipments : assigned_to
    Equipment ||--o{ EquipmentUnits : has
    Equipment ||--o{ BorrowSlipItems : requested
    Equipment ||--o{ DamageReports : damaged_or_lost
    Equipment ||--o{ ExperimentItems : required
    Equipment ||--o{ ExperimentManualItems : required

    BorrowSlips ||--o{ BorrowSlipItems : contains
    BorrowSlips ||--o{ BorrowSlipMembers : includes
    BorrowSlips ||--o{ DamageReports : creates
    BorrowSlipItems ||--o{ BorrowSlipUnits : assigns_units
    BorrowSlipItems ||--o{ DamageReports : reports_item
    EquipmentUnits ||--o{ BorrowSlipUnits : borrowed_unit

    DamageReports ||--o{ DamageReportMembers : restricts
    DamageReports ||--o{ DamageReportUnits : includes
    EquipmentUnits ||--o{ DamageReportUnits : reported_unit

    Experiments ||--o{ ExperimentItems : contains
    ExperimentManuals ||--o{ ExperimentManualItems : contains

    Laboratories {
        int LabID PK
        string LabName
        string LabCode
        bool IsActive
    }
    Users {
        int UserID PK
        string SchoolID
        string FullName
        string SchoolEmail
        string Password
        bool IsActive
    }
    AdminCredentials {
        int AdminID PK
        int LabID FK
        string AdminFullName
        string OfficeEmail
        string Role
        bool IsActive
    }
    LabSubjects {
        int SubjectID PK
        int LabID FK
        string SubjectCode
        string SubjectName
        bool IsActive
    }
    SubjectSchedules {
        int ScheduleID PK
        int SubjectID FK
        string DayOfWeek
        datetime StartTime
        datetime EndTime
        string Section
        string Room
    }
    StudentSubjectEnrollments {
        int EnrollmentID PK
        int UserID FK
        int SubjectID FK
        int ScheduleID FK
        bool IsActive
    }
    Equipment {
        int EquipmentID PK
        int LabID FK
        string EquipmentName
        string Category
        int QuantityTotal
        int LowStockThreshold
        string EquipmentType
        bool HasSerial
        bool IsArchived
    }
    BorrowSlips {
        int SlipID PK
        int UserID FK
        int SubjectID FK
        int ScheduleID FK
        string GroupNumber
        string LeaderName
        string SlipStatus
    }
    DamageReports {
        int ReportID PK
        int SlipID FK
        int EquipmentID FK
        string DamageType
        int DamageQuantity
        currency CurrentReplacementCost
        string ReportStatus
    }
`;

fs.writeFileSync(path.join(outDir, "feature_function_connections.svg"), featureSvg(), "utf8");
fs.writeFileSync(path.join(outDir, "wildcathub_erd.svg"), erdSvg(), "utf8");
fs.writeFileSync(path.join(outDir, "feature_function_connections.mmd"), featureMermaid, "utf8");
fs.writeFileSync(path.join(outDir, "wildcathub_erd.mmd"), erdMermaid, "utf8");

const html = `<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>WildcatHub Diagrams</title>
  <style>
    body { font-family: Segoe UI, Arial, sans-serif; margin: 24px; color: #2f203a; background: #fffaf3; }
    h1 { color: #4b2130; }
    h2 { color: #8b001a; margin-top: 28px; }
    img { max-width: 100%; border: 1px solid #e2d5de; background: white; }
    a { color: #8b001a; }
  </style>
</head>
<body>
  <h1>WildcatHub Diagrams</h1>
  <p>Feature/function connections and ERD generated from the current WildcatHub application structure.</p>
  <h2>Diagram</h2>
  <p><a href="feature_function_connections.svg">Open SVG</a> | <a href="feature_function_connections.mmd">Open Mermaid source</a></p>
  <img src="feature_function_connections.svg" alt="Feature and Function Connections">
  <h2>Entity Relationship Diagram</h2>
  <p><a href="wildcathub_erd.svg">Open SVG</a> | <a href="wildcathub_erd.mmd">Open Mermaid source</a></p>
  <img src="wildcathub_erd.svg" alt="WildcatHub ERD">
</body>
</html>`;

fs.writeFileSync(path.join(outDir, "wildcathub_diagrams.html"), html, "utf8");

console.log(path.join(outDir, "feature_function_connections.svg"));
console.log(path.join(outDir, "wildcathub_erd.svg"));
console.log(path.join(outDir, "feature_function_connections.mmd"));
console.log(path.join(outDir, "wildcathub_erd.mmd"));
console.log(path.join(outDir, "wildcathub_diagrams.html"));
