# WildcatHub

WildcatHub is a C# Windows Forms laboratory equipment borrowing and management system.

## Main Features

- Admin and student login
- Student account creation with email verification
- Forgot password with reset code
- Student equipment browsing by enrolled subject and schedule
- Borrow cart and editable pending borrower's slip
- Admin approval and decline of borrow slips
- Serial number assignment for serialized equipment
- Equipment management with reusable, one-time-use, and limited-use item types
- Return processing based on equipment type
- Damage and lost report workflow
- Report cost assignment and payment slip viewing
- User and admin history
- Dashboard charts, low-stock alerts, and workload summaries

## Technologies Used

- C#
- .NET Windows Forms
- Microsoft Access database
- OleDb database connection
- MailKit and MimeKit for email verification and password reset codes
- System.Windows.Forms.DataVisualization for dashboard charts

## Project Files

- Source code: `WildcatHub/WildcatHub`
- Solution file: `WildcatHub/WildcatHub.sln`
- Final database: `WildcatHub/WildcatHub/Database/WildcatHub_LabSystem.accdb`
- Diagram: `Diagrams_and_PPT/Diagrams/ERD.svg`
- Database ERD: `Diagrams_and_PPT/Diagrams/wildcathub_erd.svg`
- Presentation: `Diagrams_and_PPT/Presentation/WildcatHub_Presentation_easy_words.pptx`

## How To Run

1. Open `WildcatHub/WildcatHub.sln` in Visual Studio.
2. Restore NuGet packages if Visual Studio asks.
3. Build the solution.
4. Run the `WildcatHub` project.

The app uses `WildcatHub_LabSystem.accdb` from the project database folder or the output database folder.
