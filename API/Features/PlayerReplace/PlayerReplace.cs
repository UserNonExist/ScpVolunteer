using System;
using System.Reflection;
using Exiled.API.Features;

namespace ScpVolunteer.API.Features.PlayerReplace;

public class PlayerReplace
{
    Assembly Assembly { get; set; }
    bool PluginEnabled { get; set; } = false;
    
    public void Init(Assembly assembly)
    {
        PluginEnabled = true;
        Assembly = assembly;
    }
    
    public void TogglePlayerReplace(bool enabled)
    {
        if (!PluginEnabled)
            return;
        
        var apiType = Assembly.GetType("PlayerReplace.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        var toggleMethod = apiType.GetMethod("TogglePlayerReplace");
        
        if (toggleMethod == null)
        {
            Log.Error("PlayerReplace API method TogglePlayerReplace not found.");
            return;
        }
        
        toggleMethod.Invoke(instance, new object[] {enabled, nameof(ScpVolunteer)});
    }
}