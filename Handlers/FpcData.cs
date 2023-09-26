using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using PlayerRoles;
using ScpVolunteer.API.Features.ExternalRoles.Enums;
using ScpVolunteer.API.Interfaces;
using UnityEngine;

namespace ScpVolunteer.Handlers;

public class FpcData : IData
{
    public RoleTypeId Role { get; set; }
    public ExternalRoleType ExternalRoleType { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Quaternion CameraRotation { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Ahp { get; set; }
    public float MaxAhp { get; set; }
    public float Stamina { get; set; }
    public IEnumerable<Item> Inventory { get; set; } = new List<Item>();
    public ushort AmmoNato9 { get; set; }
    public ushort AmmoNato556 { get; set; }
    public ushort AmmoNato762 { get; set; }
    public ushort Ammo12Gauge { get; set; }
    public ushort Ammo44Cal { get; set; }
    public bool Disconnected { get; set; } = false;
    public Item CurrentItem { get; set; }

    public void Initialize(Player player, bool disconnected = false)
    {
        Role = player.Role.Type;
        Position = player.Position;
        Rotation = player.Rotation;
        CameraRotation = player.CameraTransform.rotation;
        Health = player.Health;
        MaxHealth = player.MaxHealth;
        Ahp = player.ArtificialHealth;
        MaxAhp = player.MaxArtificialHealth;
        Stamina = player.Stamina;
        Inventory = player.Items.ToArray();
        CurrentItem = player.CurrentItem;
        
        AmmoNato9 = player.GetAmmo(AmmoType.Nato9);
        AmmoNato556 = player.GetAmmo(AmmoType.Nato556);
        AmmoNato762 = player.GetAmmo(AmmoType.Nato762);
        Ammo12Gauge = player.GetAmmo(AmmoType.Ammo12Gauge);
        Ammo44Cal = player.GetAmmo(AmmoType.Ammo44Cal);
        
        
        ExternalRoleType = API.API.GetExternalRole(player);
    }
    
    public void Apply(Player player)
    {
        switch (ExternalRoleType)
        {
            case ExternalRoleType.ChaosSpy:
                API.API.CiSpyRole.SpawnRole(player, ExternalRoleType.ChaosSpy);
                break;
            case ExternalRoleType.NtfSpy:
                API.API.CiSpyRole.SpawnRole(player, ExternalRoleType.NtfSpy);
                break;
            default:
                player.Role.Set(Role, RoleSpawnFlags.None);
                break;
        }
        
        player.Position = Position;
        player.Health = Health;
        player.MaxHealth = MaxHealth;
        player.ArtificialHealth = Ahp;
        player.MaxArtificialHealth = MaxAhp;
        player.Stamina = Stamina;
        
        Timing.CallDelayed(0.5f, () =>
        {
            int i = 0;
            foreach (var item in Inventory)
            {

                Item newItem = item.Clone();
                
                player.AddItem(item.Type);

                Item addedItem = player.Items.ToList()[i];

                switch (item.Type)
                {
                    case ItemType.MicroHID:
                        MicroHid microHid = player.Items.FirstOrDefault(i => i.Type == ItemType.MicroHID) as MicroHid;
                        if (microHid != null)
                        {
                            microHid.Energy = (item as MicroHid).Energy;
                        }
                        i++;
                        return;
                    case ItemType.Radio:
                        Radio radio = player.Items.FirstOrDefault(i => i.Type == ItemType.Radio) as Radio;
                        if (radio != null)
                        {
                            radio.BatteryLevel = (item as Radio).BatteryLevel;
                            radio.IsEnabled = (item as Radio).IsEnabled;
                            radio.SetRangeSettings( (item as Radio).Range, (item as Radio).RangeSettings);
                        }
                        i++;
                        return;
                    default:
                        break;
                }

                if (item.Type is Firearm == true)
                {
                    Firearm firearm = addedItem as Firearm;
                    if (firearm != null)
                    {
                        firearm.Ammo = (item as Firearm).Ammo;
                        firearm.AddAttachment( (item as Firearm).AttachmentIdentifiers);
                    }
                }
                
                i++;
            }   
            
            player.Rotation = Rotation;
            player.CameraTransform.rotation = CameraRotation;
            
            player.SetAmmo(AmmoType.Nato9, AmmoNato9);
            player.SetAmmo(AmmoType.Nato556, AmmoNato556);
            player.SetAmmo(AmmoType.Nato762, AmmoNato762);
            player.SetAmmo(AmmoType.Ammo12Gauge, Ammo12Gauge);
            player.SetAmmo(AmmoType.Ammo44Cal, Ammo44Cal);
            player.CurrentItem = CurrentItem;
        });
    }
}