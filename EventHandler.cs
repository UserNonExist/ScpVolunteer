using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using ScpVolunteer.API.Interfaces;
using ScpVolunteer.Handlers;
using UnityEngine;

namespace ScpVolunteer;

public class EventHandler
{
    public Dictionary <Player, IData> PlayersNeedReplacing = new Dictionary<Player, IData>();
    public List<CoroutineHandle> CoroutineHandles = new List<CoroutineHandle>();
    public List<Player> Volunteers = new List<Player>();
    
    
    public bool FindingVoluteer = false;
    public bool CanRequestVolunteer;
    public bool NoSpectatorsSwap = false;
    public bool DisconnectReplace = false;
    
    public bool IsDisbled = false;

    public void OnRestartingRound()
    {
        PlayersNeedReplacing.Clear();
        Volunteers.Clear();
        FindingVoluteer = false;
        CanRequestVolunteer = false;
        NoSpectatorsSwap = false;
        DisconnectReplace = false;
        
        foreach (var cHandle in CoroutineHandles)
        {
            Timing.KillCoroutines(cHandle);
        }
        CoroutineHandles.Clear();
    }

    public void OnRoundStarted()
    {
        CanRequestVolunteer = true;
        DisconnectReplace = true;
        PlayersNeedReplacing.Clear();
        Volunteers.Clear();
        if (Entrypoint.Instance.Config.RequestVolunteerTime != -1)
            CoroutineHandles.Add(Timing.RunCoroutine(CanRequestVolunteerCountdown(Entrypoint.Instance.Config.RequestVolunteerTime)));
        if (Entrypoint.Instance.Config.DisconnectReplacePeriod != -1)
            CoroutineHandles.Add(Timing.RunCoroutine(DisconnectReplaceCountdown(Entrypoint.Instance.Config.DisconnectReplacePeriod)));
        CoroutineHandles.Add(Timing.RunCoroutine(CheckForRequest()));
    }

    public void OnLeft(LeftEventArgs ev)
    {
        if (IsDisbled)
            return;
        
        if (Volunteers.Contains(ev.Player))
            Volunteers.Remove(ev.Player);
        
        if (!Round.InProgress)
            return;
        
        if (ev.Player.Role.Side != Side.Scp)
            return;
        
        if (!Entrypoint.Instance.Config.ReplaceLeftScps)
            return;
        
        if (Entrypoint.Instance.Config.BlacklistedLeftScps.Contains(ev.Player.Role.Type))
            return;
        
        if (PlayersNeedReplacing.ContainsKey(ev.Player))
            return;
        
        if (Player.List.Count <= 3)
            return;
        
        List<Player> spectators = Player.List.Where(x => x.Role == RoleTypeId.Spectator).ToList();

        if (DisconnectReplace)
        {
            API.API.PlayerReplace.TogglePlayerReplace(false);
            API.API.AFKReplace.ToggleAFKReplace(false);
        
            IData data;
        
            if (ev.Player.Role.Type == RoleTypeId.Scp079)
            {
                data = new Scp079Data();
            }
            else
            {
                data = new ScpData();
            }
        
            data.Initialize(ev.Player, true);
            
            Log.Debug("SCP disconnected and passed the disconnect replace check, requesting...");
        
            PlayersNeedReplacing.Add(ev.Player, data);
            Timing.CallDelayed(1.1f, () =>
            {
                API.API.PlayerReplace.TogglePlayerReplace(true);
                API.API.AFKReplace.ToggleAFKReplace(true);
            });
        }
        else if (spectators.Count < 1 && Entrypoint.Instance.Config.NoSpectatorsOverride)
        {
            API.API.PlayerReplace.TogglePlayerReplace(false);
            API.API.AFKReplace.ToggleAFKReplace(false);
        
            IData data;
        
            if (ev.Player.Role.Type == RoleTypeId.Scp079)
            {
                data = new Scp079Data();
            }
            else
            {
                data = new ScpData();
            }
        
            data.Initialize(ev.Player, true);
            
            Log.Debug("SCP disconnected and passed the disconnect replace check, requesting...");
        
            PlayersNeedReplacing.Add(ev.Player, data);
            Timing.CallDelayed(1.1f, () =>
            {
                API.API.PlayerReplace.TogglePlayerReplace(true);
                API.API.AFKReplace.ToggleAFKReplace(true);
            });
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (Volunteers.Contains(ev.Player))
            Volunteers.Remove(ev.Player);
    }
    
    public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
    {
        if (FindingVoluteer)
            ev.IsAllowed = false;
    }
    
    public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
    {
        if (!PlayersNeedReplacing.Keys.Contains(ev.Player))
            return;
        
        if (FindingVoluteer)
            ev.IsAllowed = false;
    }

    #region Coroutines

    public IEnumerator<float> CheckForRequest()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(1f);

            if (PlayersNeedReplacing.Count > 0 && !FindingVoluteer)
            {
                Log.Debug("Found a request for volunteer, starting countdown...");
                
                Player player = PlayersNeedReplacing.Keys.First();
                IData data = PlayersNeedReplacing.Values.First();
                
                if (data.Disconnected)
                    Round.IsLocked = true;
                
                PlayersNeedReplacing.Remove(player);
                Timing.RunCoroutine(FindingVolunteerCountdown(Entrypoint.Instance.Config.FindVolunteerTime, player, data));
            }
            
            if (Round.IsEnded)
                yield break;
        }
    }

    public IEnumerator<float> CanRequestVolunteerCountdown(float time)
    {
        yield return Timing.WaitForSeconds(time);
        CanRequestVolunteer = false;
    }
    
    public IEnumerator<float> DisconnectReplaceCountdown(float time)
    {
        yield return Timing.WaitForSeconds(time);
        DisconnectReplace = false;
    }
    
    public IEnumerator<float> FindingVolunteerCountdown(float time, Player player, IData data)
    {
        FindingVoluteer = true;
        bool dataDisconnected = data.Disconnected;
        
        int gracePeriod = Entrypoint.Instance.Config.FirstVolunteerTime;
        
        Log.Debug($"Player requesting a replace disconnected: {dataDisconnected.ToString()}");
        
        while (time > 0)
        {
            string formattedMessage;
            string timestr = time.ToString();

            if (dataDisconnected)
            {
                formattedMessage = Entrypoint.Instance.Translation.ScpDisconnectedMessage;
                formattedMessage = formattedMessage.Replace("%player%", player.Nickname);
                formattedMessage = formattedMessage.Replace("%role%", data.Role.ToString());
                formattedMessage = formattedMessage.Replace("%time%", timestr);
                formattedMessage = formattedMessage.Replace("%volunteerCount%", Volunteers.Count.ToString());
            }
            else
            {
                formattedMessage = Entrypoint.Instance.Translation.ScpRequestedReplaceMessage;
                formattedMessage = formattedMessage.Replace("%player%", player.Nickname);
                formattedMessage = formattedMessage.Replace("%role%", data.Role.ToString());
                formattedMessage = formattedMessage.Replace("%time%", timestr);
                formattedMessage = formattedMessage.Replace("%volunteerCount%", Volunteers.Count.ToString());
                

                if (data.Role != player.Role.Type)
                {
                    Map.ClearBroadcasts();
                    Map.Broadcast(5, Entrypoint.Instance.Translation.CancelMessage);
                    yield return Timing.WaitForSeconds(3f);
                    PlayersNeedReplacing.Remove(player);
                    Volunteers.Clear();
                    FindingVoluteer = false;
                    NoSpectatorsSwap = false;
                    yield break;
                }
            }
            
            Map.ClearBroadcasts();
            Map.Broadcast(1, formattedMessage);
            
            yield return Timing.WaitForSeconds(1f);
            

            if (Volunteers.Count > 0)
            {
                if (gracePeriod == -1)
                {
                    Player volunteer = Volunteers[0];
                    Features.VolunteerHandler.OnVolunteerFound(volunteer, data, player);
                    Volunteers.Clear();
                    NoSpectatorsSwap = false;
                    yield break;
                }
                if (gracePeriod > 0)
                {
                    gracePeriod--;
                }
                else
                {
                    Player volunteer = Volunteers[Random.Range(0, Volunteers.Count-1)];
                    Features.VolunteerHandler.OnVolunteerFound(volunteer, data, player);
                    Volunteers.Clear();
                    NoSpectatorsSwap = false;
                    yield break;
                }
            }
            
            time--;
        }
        
        if (Volunteers.Count > 0)
        {
            Player volunteer = Volunteers[Random.Range(0, Volunteers.Count-1)];
            Features.VolunteerHandler.OnVolunteerFound(volunteer, data, player);
            Volunteers.Clear();
            NoSpectatorsSwap = false;
                
            yield break;
        }

        if (dataDisconnected)
        {
            List<Player> spectators = Player.List.Where(x => x.Role == RoleTypeId.Spectator).ToList();

            if (spectators.Count < 1)
            {
                Log.Debug("No spectators found, resetting...");
                Map.ClearBroadcasts();
                Map.Broadcast(5, Entrypoint.Instance.Translation.NoVolunteerMessage);
                yield break;
            }
            
            Player targetPlayer = spectators[Random.Range(0, spectators.Count-1)];

            if (player == null)
            {
                Log.Debug("Player random for a replace is null, resetting...");
                Map.ClearBroadcasts();
                Map.Broadcast(5, Entrypoint.Instance.Translation.NoVolunteerMessage);
                yield break;
            }
            
            Features.VolunteerHandler.OnVolunteerFound(targetPlayer, data, player);
            Volunteers.Clear();
            NoSpectatorsSwap = false;
            Round.IsLocked = false;
        }
        else
        {
            Log.Debug("No volunteer found, resetting...");
            Map.ClearBroadcasts();
            Map.Broadcast(5, Entrypoint.Instance.Translation.NoVolunteerMessage);
            PlayersNeedReplacing.Remove(player);
            FindingVoluteer = false;
        }
    }

    #endregion
    
}