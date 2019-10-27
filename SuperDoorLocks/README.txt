===Super Door Locks===

The aim of this solution is to provide a simple API to unlocks doors remotely using a simplified
authentication and authorization mechanism.

===Technical requirements:===
Visual Studio 2017 or newer
Support for .NET Core 2.0

===Technologies used:===
NSwag
Entity Framework Core
SQLite
ASP.NET Identity

===General usage:===
This PoC creates a new set of data upon every launch.
On startup, a default admin user is created with the following credentials:
username: admin
password: Password123

The following roles are available:
Admin
Employee

- All authenticated requests must have Authorization header with Bearer token obtained from Login request.
- An administrator can register new users and assign user roles to them.
- An administrator can create Doors and specify which roles grant permission to open them.
- An administrator can see the 
- Users can open doors so long as at least one of their roles is in the set of roles permitted
to open a particular door.
- Doors are automatically closed after 30 seconds.
