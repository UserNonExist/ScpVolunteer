using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Interfaces;

namespace ScpVolunteer.Handlers;

public class Scp079Data : IData
{
    public RoleTypeId Role { get; set; } = RoleTypeId.Scp079;
    public ExternalRoleType ExternalRoleType { get; set; } = ExternalRoleType.None;
    public Camera Camera { get; set; }
    public float Energy { get; set; }
    public int Experience { get; set; }
    public bool Disconnected { get; set; }

    public void Initialize(Player player, bool disconnected = false)
    {
        var role = player.Role.As<Scp079Role>();
        
        Disconnected = disconnected;
        Camera = role.Camera;
        Energy = role.Energy;
        Experience = role.Experience;
    }

    public void Apply(Player player)
    {
        player.Role.Set(Role);
        player.ClearInventory();

        Timing.CallDelayed(0.5f, () =>
        {
            var role = player.Role.As<Scp079Role>();
            role.Camera = Camera;
            role.Energy = Energy;
            role.Experience = Experience;
        });
        
    }
}