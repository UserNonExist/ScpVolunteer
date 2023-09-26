using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace ScpVolunteer;

public class Config : IConfig
{
    [Description("Whether the plugin is enabled or not.")]
    public bool IsEnabled { get; set; } = true;
    [Description("Whether debug mode is enabled or not.")]
    public bool Debug { get; set; } = false;
    [Description("Time in seconds for the plugin to find a volunteer. (Cannot be lower than 0)")]
    public int FindVolunteerTime { get; set; } = 30;
    [Description("Whether the plugin should find volunteer for SCPs that left the game.")]
    public bool ReplaceLeftScps { get; set; } = true;
    [Description("Whether the SCPs can volunteer to be replaced.")]
    public bool ScpsCanVolunteer { get; set; } = true;
    [Description("Time in seconds for the SCPs before they can't request a volunteer. (-1 to disable)")]
    public int RequestVolunteerTime { get; set; } = -1;
    [Description("Whether the spectator can volunteer to swap with the requesting SCPs.")]
    public bool SpectatorCanVolunteerSwap { get; set; } = false;
    [Description("Time in seconds for a period where disconnected player will trigger a request for a volunteer. (-1 to always request)")]
    public int DisconnectReplacePeriod { get; set; } = -1;
    [Description("Whether the plugin can override the above config if there are no spectators.")]
    public bool NoSpectatorsOverride { get; set; } = true;
    [Description("List of SCPs that can volunteer to be replaced.")]
    public List<RoleTypeId> ScpsCanRequestVolunteerList { get; set; } = new List<RoleTypeId> 
    { 
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939,
        RoleTypeId.Scp079
    };
    [Description("List of SCPs that will NOT be replaced when left the game.")]
    public List<RoleTypeId> BlacklistedLeftScps { get; set; } = new List<RoleTypeId> 
    { 
        RoleTypeId.Scp0492,
    };
    [Description("Time in seconds for the remaining player to join volunteer list after the first player start volunteering. (-1 to instantly replace)")]
    public int FirstVolunteerTime { get; set; } = 8;
}