using Exiled.API.Features;
using ScpVolunteer.API.Features.ExternalRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Features.PlayerReplace;

namespace ScpVolunteer.API;

public class API
{
    public static void DisableScpVolunteer()
    {
    }
    
    public static bool IsExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player) != ExternalRoleType.None)
            return true;
        
        return false;
    }
    
    public static ExternalRoleType GetExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player) != ExternalRoleType.None)
            return CiSpyRole.IsRole(player);
        
        return ExternalRoleType.None;
    }

    internal static readonly PlayerReplace PlayerReplace = new PlayerReplace();
    
    internal static readonly ExternalRoleChecker CiSpyRole = new CiSpyRole();
}