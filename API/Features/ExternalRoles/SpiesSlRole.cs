using System;
using System.Reflection;
using Exiled.API.Features;
using MEC;
using ScpVolunteer.API.Features.ExternalRoles.Enums;

namespace ScpVolunteer.API.Features.ExternalRoles;

public class SpiesSlRole : ExternalRoleChecker
{
    public override void Init(Assembly assembly)
    {
        PluginEnabled = true;
        Assembly = assembly;

        Log.Debug("SpiesSl assembly attached.");
    }

    public override ExternalRoleType IsRole(Player player)
    {
        if (!PluginEnabled)
            return ExternalRoleType.None;

        var apiType = Assembly.GetType("SpiesSl.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        var isSpyMethod = apiType.GetMethod("IsSpy");
        
        if (isSpyMethod == null)
        {
            Log.Error("SpiesSl API method IsSpy not found.");
            return ExternalRoleType.None;
        }
        
        var isSpy = (bool) isSpyMethod.Invoke(instance, new object[] {player, nameof(ScpVolunteer)});

        if (!isSpy)
            return ExternalRoleType.None;
        
        var isNtfSpyMethod = apiType.GetMethod("IsNtfSpy");
        var isChaosSpyMethod = apiType.GetMethod("IsChaosSpy");
        
        if (isNtfSpyMethod == null)
        {
            Log.Error("SpiesSl API method GetNtfSpyList not found.");
            return ExternalRoleType.None;
        }
        
        if (isChaosSpyMethod == null)
        {
            Log.Error("SpiesSl API method GetChaosSpyList not found.");
            return ExternalRoleType.None;
        }
        
        var isNtfSpy = (bool) isNtfSpyMethod.Invoke(instance, new object[] {player, nameof(ScpVolunteer)});
        var isChaosSpy = (bool) isChaosSpyMethod.Invoke(instance, new object[] {player, nameof(ScpVolunteer)});
        
        if (isNtfSpy)
            return ExternalRoleType.NtfSpy;
        
        if (isChaosSpy)
            return ExternalRoleType.ChaosSpy;

        return ExternalRoleType.None;
    }

    public override void SpawnRole(Player newPlayer, ExternalRoleType extenalRole)
    {
        if (!PluginEnabled)
            return;
        
        var apiType = Assembly.GetType("SpiesSl.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        if (extenalRole == ExternalRoleType.NtfSpy)
        {
            var spawnNtfSpyMethod = apiType.GetMethod("SpawnNtfSpy");
            
            if (spawnNtfSpyMethod == null)
            {
                Log.Error("SpiesSl API method GetSpawnNtfSpy not found.");
                return;
            }
            
            spawnNtfSpyMethod.Invoke(instance, new object[] {newPlayer, nameof(ScpVolunteer)});
            Log.Debug($"Sucessfully spawned {newPlayer.Nickname} as NTF Spy.");
        }
        else if (extenalRole == ExternalRoleType.ChaosSpy)
        {
            var spawnChaosSpyMethod = apiType.GetMethod("SpawnChaosSpy");
            
            if (spawnChaosSpyMethod == null)
            {
                Log.Error("SpiesSl API method GetSpawnChaosSpy not found.");
                return;
            }
            
            spawnChaosSpyMethod.Invoke(instance, new object[] {newPlayer, nameof(ScpVolunteer)});
            Log.Debug($"Sucessfully spawned {newPlayer.Nickname} as Chaos Spy.");
        }
    }
}