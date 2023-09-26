using System;
using System.Reflection;
using Exiled.API.Features;

namespace ScpVolunteer.API.Features.AFKReplace;

public class AFKReplace
{
    Assembly Assembly { get; set; }
    bool PluginEnabled { get; set; } = false;
    
    public void Init(Assembly assembly)
    {
        PluginEnabled = true;
        Assembly = assembly;
    }
    
    public void ToggleAFKReplace(bool enabled)
    {
        if (!PluginEnabled)
            return;
        
        var apiType = Assembly.GetType("AFKReplace.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        var toggleMethod = apiType.GetMethod("ToggleAFKReplace");
        
        if (toggleMethod == null)
        {
            Log.Error("AFKReplace API method TogglePlayerReplace not found.");
            return;
        }
        
        toggleMethod.Invoke(instance, new object[] {enabled, nameof(ScpVolunteer)});
    }
}