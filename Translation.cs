using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace ScpVolunteer;

public class Translation : ITranslation
{
    [Description("Message to send to all players when SCPs disconnected")]
    public string ScpDisconnectedMessage { get; set; } = "%player% have disconnected as %role%\nyou can volunteer to replace by pressing ALT or Noclip key (%time% seconds left)\nVolunteer Count: <color=green>%volunteerCount%</color>";
    
    [Description("Message to send to alive players when SCPs requested to be replaced")]
    public string ScpRequestedReplaceMessage { get; set; } = "%player% want a volunteer to play as %role%\nyou can volunteer to replace by pressing ALT or Noclip key (%time% seconds left)\nVolunteer Count: <color=green>%volunteerCount%</color>";
    
    [Description("Message to send to all player when an SCP is replaced")]
    public string ScpReplacedMessage { get; set; } = "%player% have replaced as %role%";
    
    [Description("Message to send to all player when couldn't find a volunteer in time")]
    public string NoVolunteerMessage { get; set; } = "Couldn't find a volunteer in time";
    
    [Description("Message to send to spectator when they try to volunteer to swap with the requesting SCPs but the option is disabled")]
    public string SpectatorCantVolunteerSwapMessage { get; set; } = "You can't volunteer to swap with the requesting SCPs";
    
    [Description("Message to SCP that attempt to volunteer")]
    public string ScpCantVolunteerMessage { get; set; } = "You can't volunteer to replace while you are an SCP";
    
    [Description("Message to send when a request is cancelled due to various reason (ex. cancelled due to role change)")]
    public string CancelMessage { get; set; } = "The request has been cancelled automatically by the system";
}