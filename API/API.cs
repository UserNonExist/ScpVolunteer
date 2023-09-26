using Exiled.API.Features;
using ScpVolunteer.API.Features.AFKReplace;
using ScpVolunteer.API.Features.ExternalRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Features.PlayerReplace;

namespace ScpVolunteer.API;

public class API
{
    public static void DisableScpVolunteer(bool enable, string pluginName = null)
    {
        if (pluginName != null)
            Log.Debug($"{pluginName} is trying to toggle ScpVolunteer to {enable}.");
        else
            Log.Debug("A plugin is trying to toggle ScpVolunteer to {enable}.");
        
        Entrypoint.EventHandler.IsDisbled = !enable;
        
        if (pluginName != null)
            Log.Debug($"{pluginName} has toggled ScpVolunteer to {enable}.");
        else
            Log.Debug("A plugin has toggled ScpVolunteer to {enable}.");
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
    
    internal static readonly AFKReplace AFKReplace = new AFKReplace();
    
    internal static readonly ExternalRoleChecker CiSpyRole = new CiSpyRole();
}