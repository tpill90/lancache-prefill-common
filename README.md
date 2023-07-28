# lancache-prefill-common

Common code shared across lancache-prefill projects.  Not intended for use outside of these projects.

## Repo Structure


|  |  |
| ---------|----------|
| `/dotnet` | Contains C# code that is shared by all of the prefills.  Code will only be added here when it is used by ALL of the prefills, otherwise it should live with the prefill it applies to. |
| `/lib` | Precompiled assemblies that contain customizations that aren't merged into their respective upstream repos.  Keeping assemblies here allows for customization without having to wait for pull requests to be approved and merged. |