using System.Reflection;
using Exiled.API.Features;
using ScpVolunteer.API.Features.ExternalRoles.Enums;

namespace ScpVolunteer.API.Features.ExternalRoles;

public abstract class ExternalRoleChecker
{
    public abstract void Init(Assembly assembly);
    public abstract ExternalRoleType IsRole(Player player);
    public abstract void SpawnRole(Player oldPlayer, ExternalRoleType extenalRole);
    protected bool PluginEnabled { get; set; }
    protected Assembly Assembly { get; set; }
}