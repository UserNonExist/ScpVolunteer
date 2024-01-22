using System;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using ScpVolunteer.API.Interfaces;
using ScpVolunteer.Handlers;

namespace ScpVolunteer.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
public class RequestVolunteerCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);
        
        if (Entrypoint.EventHandler.IsDisbled)
        {
            response = "ScpVolunteer is disabled.";
            return false;
        }
        
        if (player == null)
        {
            response = "Can't get player instance.";
            return false;
        }

        if (!player.IsScp)
        {
            response = "You are not an SCP.";
            return false;
        }

        if (!Entrypoint.Instance.Config.ScpsCanVolunteer)
        {
            response = "SCP's can't volunteer.";
            return false;
        }

        if (!Entrypoint.Instance.Config.ScpsCanRequestVolunteerList.Contains(player.Role.Type))
        {
            response = "Your role can't request for a volunteer.";
            return false;
        }

        if (!Entrypoint.EventHandler.CanRequestVolunteer)
        {
            response = "Volunteer period is over.";
            return false;
        }
        
        if (Entrypoint.EventHandler.PlayersNeedReplacing.ContainsKey(player))
        {
            response = "You already requested a volunteer.";
            return false;
        }

        IData data;
        
        if (player.Role.Type == RoleTypeId.Scp079)
        {
            data = new Scp079Data();
        }
        else
        {
            data = new ScpData();
        }
        
        data.Initialize(player);
        
        Entrypoint.EventHandler.PlayersNeedReplacing.Add(player, data);
        response = "You requested a volunteer.";
        return true;
    }

    public string Command { get; } = "requestvolunteer";
    public string[] Aliases { get; } = { "rv", "needvolunteer" };
    public string Description { get; } = "Request a volunteer to replace you.";
}