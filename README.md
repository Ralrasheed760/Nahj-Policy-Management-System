# نَهْج (Nahj) — Health Policy Management System

Nahj is a web-based health policy management system developed to centralize, organize, manage, standardize, and provide access to organizational health policies through a unified platform.

The system was developed as an individual software development project during my cooperative training, covering requirements analysis, database design, system implementation, user interfaces, role-based access control, policy document processing, and testing.

## Overview

Nahj provides a structured environment for managing policies across multiple branches and departments while separating policy visibility from policy management permissions.

Published policies are available for employees to view across the organization regardless of their assigned department. For example, an employee in the IT department can view policies belonging to Human Resources or other departments.

Policy management permissions are controlled according to the user's role and organizational scope. Admins have system-wide management privileges, while SubAdmins can create, edit, and update policies only within their assigned branch and department. Employees have read-only access to published policies.

Nahj also introduces a structured policy-document workflow. Instead of permanently storing uploaded Word documents as the policy itself, the system extracts their content, converts it into structured JSON data, and combines it with standardized templates to generate consistent PDF policy documents.

This approach supports consistent formatting, organizational branding, policy versioning, and standardized document structure.

## Main Features

- Role-based access control for Admin, SubAdmin, and Employee users
- Organization-wide access to published policies
- User account management
- Branch and department management
- Policy creation, editing, and updating
- Policy version management and history
- Search, filtering, and sorting
- Notifications
- Download tracking
- Activity logging
- Soft-delete functionality
- Structured policy content using JSON
- Word document content extraction
- Standardized PDF policy generation
- Template-based document formatting

## User Roles

### Admin

Has system-wide management privileges and can manage users, branches, departments, policies, templates, and other administrative operations across the system.

### SubAdmin

Can view published policies across the system while managing policy-related operations only within the assigned organizational scope.

A SubAdmin can create, edit, and update policies for the assigned branch and department, while policy management outside that scope is restricted.

### Employee

Has read-only access to published policies across the organization, regardless of the employee's assigned department.

Employees can browse, search, filter, view, and download policies but cannot create, edit, or manage them.

## Policy Processing Workflow

Nahj supports a structured document-processing workflow:

`Word Document → Content Extraction → JSON → Template Engine → Standardized PDF`

Policy content is extracted from uploaded `.docx` documents and transformed into structured JSON data.

The uploaded Word document is used as a source for extracting policy content rather than being permanently stored as the final policy document.

The Template Engine combines the structured policy content with predefined formatting and branding settings to generate a standardized PDF.

Templates can define elements such as logos, headers, footers, fonts, colors, layout settings, and document structure, helping policies maintain a consistent organizational identity.

## Technologies

- C#
- ASP.NET Core
- Razor Pages
- Entity Framework Core
- SQL Server
- JSON
- HTML
- CSS
- JavaScript
- Bootstrap

## Database

Nahj uses SQL Server as its relational database and Entity Framework Core for database integration and data access.

The database supports the system's main components, including users, branches, departments, policies, policy versions, notifications, downloads, and activity logs.

The repository includes:

- Entity Framework Core migrations
- Database models and relationships
- SQL Server database creation script
- Application database context

The SQL database script is available at:

`HealthPolicyManagementSystem/DataBase/HealthPolicyDB.sql`

## Project Structure

```text
HealthPolicyManagementSystem/
├── Data/
├── DataBase/
├── Helpers/
├── Migrations/
├── Models/
├── Pages/
├── Services/
├── ViewModels/
├── wwwroot/
├── Program.cs
└── appsettings.json
```

## Development Scope

The project covered the main stages of software development, including:

- Requirements analysis
- System and database design
- Relational database implementation
- Backend development
- User interface development
- Role-based authorization
- Policy document processing
- JSON-based content handling
- PDF generation
- Testing and system refinement

## Project Status

Nahj was developed as a cooperative training project and represents a functional prototype of a centralized health policy management system.

---

**Developed by Razan Al-Rasheed**
