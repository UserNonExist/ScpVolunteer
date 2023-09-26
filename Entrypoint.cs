using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using HarmonyLib;

namespace ScpVolunteer;

public class Entrypoint : Plugin<Config, Translation>
{
    public override string Name { get; } = "ScpVolunteer";
    public override string Author { get; } = "User_NotExist";
    public override Version Version { get; } = new Version(1, 0, 2);
    public override Version RequiredExiledVersion { get; } = new Version(8, 2, 1);
    public override PluginPriority Priority { get; } = PluginPriority.Highest;

    public static Entrypoint Instance { get; private set; }
    public static EventHandler EventHandler;
    
    public Harmony Harmony;

    public override void OnEnabled()
    {
        Instance = this;
        EventHandler = new EventHandler();
        
        Harmony = new Harmony($"com.user.scpvolunteer");
        Harmony.PatchAll();

        foreach (IPlugin<IConfig> plugin in Loader.Plugins)
        {
            switch (plugin.Name)
            {
                case "PlayerReplace":
                    Log.Debug("PlayerReplace found, enabling compatibility patch...");
                    API.API.PlayerReplace.Init(plugin.Assembly);
                    break;
                case "CiSpy":
                    Log.Debug("CiSpy found, enabling compatibility patch...");
                    API.API.CiSpyRole.Init(plugin.Assembly);
                    break;
                case "AFKReplace":
                    Log.Debug("AFKReplace found, enabling compatibility patch...");
                    API.API.AFKReplace.Init(plugin.Assembly);
                    break;
            }
        }
        
        Exiled.Events.Handlers.Server.RestartingRound += EventHandler.OnRestartingRound;
        Exiled.Events.Handlers.Server.RoundStarted += EventHandler.OnRoundStarted;
        Exiled.Events.Handlers.Player.Left += EventHandler.OnLeft;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination += EventHandler.OnAnnouncingScpTermination;
        Exiled.Events.Handlers.Player.SpawningRagdoll += EventHandler.OnSpawningRagdoll;
    }
    
    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.RestartingRound -= EventHandler.OnRestartingRound;
        Exiled.Events.Handlers.Server.RoundStarted -= EventHandler.OnRoundStarted;
        Exiled.Events.Handlers.Player.Left -= EventHandler.OnLeft;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination -= EventHandler.OnAnnouncingScpTermination;
        Exiled.Events.Handlers.Player.SpawningRagdoll -= EventHandler.OnSpawningRagdoll;
        
        Harmony.UnpatchAll();
        EventHandler = null;
        Harmony = null;
        Instance = null;
    }
}