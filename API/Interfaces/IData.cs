using Exiled.API.Features;
using PlayerRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;

namespace ScpVolunteer.API.Interfaces;

public interface IData
{
    void Initialize(Player player, bool disconnected = false);
    void Apply(Player player);
    bool Disconnected { get; set; }
    RoleTypeId Role { get; set; }
    ExternalRoleType ExternalRoleType { get; set; }
}