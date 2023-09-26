using Exiled.API.Features;
using PlayerRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Interfaces;
using UnityEngine;

namespace ScpVolunteer.Handlers;

public class ScpData : IData
{
    public RoleTypeId Role { get; set; }
    public ExternalRoleType ExternalRoleType { get; set; } = ExternalRoleType.None;
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Quaternion CameraRotation { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float HumeShield { get; set; }
    public bool Disconnected { get; set; }
    
    
    public void Initialize(Player player, bool disconnected = false)
    {
        Disconnected = disconnected;
        Role = player.Role.Type;
        Position = player.Position;
        Rotation = player.Rotation;
        CameraRotation = player.CameraTransform.rotation;
        Health = player.Health;
        MaxHealth = player.MaxHealth;
        HumeShield = player.HumeShield;
    }

    public void Apply(Player player)
    {
        player.ClearInventory();
        player.Role.Set(Role, RoleSpawnFlags.None);
        player.Position = Position;
        player.Health = Health;
        player.MaxHealth = MaxHealth;
        player.HumeShield = HumeShield;
        player.Rotation = Rotation;
        player.CameraTransform.rotation = CameraRotation;
    }
}