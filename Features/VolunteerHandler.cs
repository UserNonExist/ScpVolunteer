using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Interfaces;
using ScpVolunteer.Handlers;

namespace ScpVolunteer.Features;

public class VolunteerHandler
{
    public static bool OnPlayerTogglingNoclip(ReferenceHub player)
    {
        if (FpcNoclip.IsPermitted(player))
            return true;

        if (!Entrypoint.EventHandler.FindingVoluteer)
            return true;

        Player ply = Player.Get(player);

        if (ply.Role.Side == Side.Scp)
        {
            ply.ShowHint(Entrypoint.Instance.Translation.ScpCantVolunteerMessage, 5);
            return true;
        }

        if (ply.IsDead && Entrypoint.EventHandler.NoSpectatorsSwap && Entrypoint.Instance.Config.SpectatorCanVolunteerSwap)
        {
            ply.ClearBroadcasts();
            ply.Broadcast(5, Entrypoint.Instance.Translation.SpectatorCantVolunteerSwapMessage);
            return true;
        }

        if (Entrypoint.EventHandler.Volunteers.Contains(ply))
        {
            return true;
        }
        
        Entrypoint.EventHandler.Volunteers.Add(ply);
        return false;
    }

    public static void OnVolunteerFound(Player newPlayer, IData oldPlayerData, Player oldPlayer)
    {
        if (oldPlayerData.Disconnected)
        {
            DisconnectReplaceRole(newPlayer, oldPlayerData);
        }
        else
        {
            SwapRole(newPlayer, oldPlayer, oldPlayerData);
        }
    }

    public static void DisconnectReplaceRole(Player newPlayer, IData oldPlayerData)
    {
        if (oldPlayerData.Role == RoleTypeId.Scp079)
        {
            oldPlayerData.Apply(newPlayer);
            Entrypoint.EventHandler.FindingVoluteer = false;
            Map.ClearBroadcasts();
            Map.Broadcast(5, Entrypoint.Instance.Translation.ScpReplacedMessage.Replace("%player%", newPlayer.Nickname).Replace("%role%", oldPlayerData.Role.ToString()));
        }
        
        oldPlayerData.Apply(newPlayer);
        Entrypoint.EventHandler.FindingVoluteer = false;
        Map.ClearBroadcasts();
        Map.Broadcast(5, Entrypoint.Instance.Translation.ScpReplacedMessage.Replace("%player%", newPlayer.Nickname).Replace("%role%", oldPlayerData.Role.ToString()));
        Round.IsLocked = false;
    }
    
    public static void SwapRole(Player newPlayer, Player oldPlayer, IData oldPlayerData)
    {
        if (oldPlayer == null)
        {
            Log.Debug("Old player is null, aborting swap");
            Map.ClearBroadcasts();
            Entrypoint.EventHandler.FindingVoluteer = false;
            return;
        }
        
        if (newPlayer == null)
        {
            Log.Debug("New player is null, aborting swap");
            Map.ClearBroadcasts();
            Entrypoint.EventHandler.FindingVoluteer = false;
            return;
        }

        IData newPlayerData = new FpcData();
        newPlayerData.Initialize(newPlayer);
        oldPlayerData.Initialize(oldPlayer);
        
        oldPlayerData.Apply(newPlayer);
        newPlayerData.Apply(oldPlayer);
        
        
        Entrypoint.EventHandler.FindingVoluteer = false;
        Map.ClearBroadcasts();
        Map.Broadcast(5, Entrypoint.Instance.Translation.ScpReplacedMessage.Replace("%player%", newPlayer.Nickname).Replace("%role%", oldPlayerData.Role.ToString()));
    }
}