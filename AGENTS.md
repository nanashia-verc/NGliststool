# AGENTS

## Repository overview
- This repository contains a .NET 9 WinForms application for NG case management.
- The solution file is NgProductManager.sln.

## Working conventions
- Prefer small, focused changes.
- Keep database access in the Data layer and business logic in the Services layer.
- Preserve Japanese UI strings and existing form behavior unless explicitly asked to change them.
- Run `dotnet test NgProductManager.sln` after code changes.

## Common commands
```powershell
dotnet build NgProductManager.sln
dotnet test NgProductManager.sln
```
