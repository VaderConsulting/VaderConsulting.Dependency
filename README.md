# VaderConsulting.Dependency

C# .NET 4.0 class library modelling business-application and server dependencies, HA, recovery streams, and SRM runbook order. `Collection` stores `Relationship` edges and feeds a Gregory Adam topological sort so `SortRunbook` / `CalculateServiceRunbook` can assign VMware SRM recovery order, while `SetServiceStream` rolls up HA versus SRM versus backup streams from `PhysicalSite` types. `ComponentBase` tracks system and user `HealthState` (OK, Degraded, Error) with optional emulation; `Server` can WMI-query `Win32_Share` names and persist attributes through `VaderConsulting.Database.SQLServer`. An older leftover `DC.csproj` (.NET 3.5, assembly `DC`, namespace `DC`) still sits beside the main project and compiles only `DependencyCollection`, `Server`, and `Service`.

**Source last updated:** 2015-09-29 · **Language:** C# · **Target:** .NET Framework 4.0 · **Output:** class library (`Library`)

## Solution structure

| Project | Language | Type | Purpose |
|---------|----------|------|---------|
| `VaderConsulting.Dependency` | C# | class library (`net40`) | Dependency graph of `BusinessApplication` / `Server`, HA, recovery streams, SRM runbook order. |
| `DC` | C# | class library (`net35`) | Leftover earlier `DependencyCollection` of `Service` and `Server` (same folder, not the main build). |

## How to open

Open `VaderConsulting.Dependency.csproj` in Visual Studio 2013 or later (ToolsVersion 12.0). There is no `.sln` in this folder. The project references sibling folders `..\VaderConsulting.Database\VaderConsulting.Database.csproj` and `..\VaderConsulting.Helper\VaderConsulting.Helper.csproj`. `BusinessService.cs` and `AddOrRemoveServiceEventArgs.cs` are present but not listed in the `.csproj`. `DC.csproj` is a leftover .NET 3.5 project that will not build against the current `Server.cs` / `Service.cs` (those now live in namespace `VaderConsulting.Dependency`).

## Attribution and provenance

Working copy from Dave Robinson's OneDrive Historical Dev folder `VaderConsulting.Dependency`. Assembly title/product `DependencyCollection`; copyright `Copyright ©  2015`; company empty. Namespace `VaderConsulting.Dependency` (leftover `DC` and demo `TopoSortDemo` types also present). `packages.config` lists AsyncBridge 0.1.1 (referenced by the `.csproj`). `App.config` has leftover Entity Framework 6 LocalDB section, not referenced by the project. `TopologicalSort.cs` is adapted from Gregory Adam (2009) with a textbook citation to Tremblay and Sorenson.

## License

MIT © 2026 VaderConsulting. See `LICENSE`.
