﻿#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace AccountInBank
{
    internal enum ActionType
    {
        None = 0,
        ChaseInVehicle = 1,
        ShootingOutsideVehicle = 2,
        ShootingOutsideVehicleTooFar = 3
    }

    internal class KillersController
    {
        private readonly List<Ped> _killers;
        private Vehicle _killersVehicle;
        private readonly AccountInBank.SleepScript _sleepScript;
        private bool _enabled;
        private ActionType _type = ActionType.None;

        private readonly VehicleSeat[] _vehicleSeat =
        {
            VehicleSeat.Driver, VehicleSeat.RightFront,
            VehicleSeat.RightRear, VehicleSeat.LeftRear
        };

        public Blip Blip { get; set; }

        public KillersController( AccountInBank.SleepScript waitAction )
        {
            this._sleepScript = waitAction;
            this._killers = new List<Ped>();
        }

        private void SetupKiller( Ped killer )
        {
            killer.BlockPermanentEvents = true;
            killer.Health = 5000;
            killer.Armor = 5000;
            killer.Accuracy = 100;
            killer.AlwaysDiesOnLowHealth = false;
            // give weapons
            killer.Weapons.Give( WeaponHash.AssaultRifle, 100, false, true );
            killer.Weapons[ WeaponHash.AssaultRifle ].Ammo = killer.Weapons[ WeaponHash.AssaultRifle ].MaxAmmo;
            Function.Call( Hash.SET_PED_INFINITE_AMMO, killer.Handle, true,
                (int)killer.Weapons[ WeaponHash.AssaultRifle ].Hash );
            killer.Weapons.Give( WeaponHash.MicroSMG, 100, false, true );
            killer.Weapons[ WeaponHash.MicroSMG ].Ammo = killer.Weapons[ WeaponHash.MicroSMG ].MaxAmmo;
            Function.Call( Hash.SET_PED_INFINITE_AMMO, killer.Handle, true,
                (int)killer.Weapons[ WeaponHash.MicroSMG ].Hash );
        }

        private void UpgradeKillersVehicle( Vehicle vehicle )
        {
            Function.Call( Hash.SET_VEHICLE_MOD_KIT, vehicle.Handle, 0 );
            vehicle.SetMod( VehicleMod.Engine, 3, true );
            vehicle.ToggleMod( VehicleToggleMod.Turbo, true );
            vehicle.SetMod( VehicleMod.Transmission, 2, true );
            vehicle.SecondaryColor = VehicleColor.MetallicRed;
        }

        private void SpawnCarAndKillers()
        {
            Vector3 position = Game.Player.Character.Position.Around( 75 );
            //Vector3 position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5f;
            this._killersVehicle = World.CreateVehicle( VehicleHash.Kuruma2, position );
            this._killersVehicle.PlaceOnNextStreet();
            this._killersVehicle.PlaceOnGround();
            this.UpgradeKillersVehicle( this._killersVehicle );

            Ped driver = this._killersVehicle.CreatePedOnSeat( VehicleSeat.Driver, PedHash.Marine01SMY );
            driver.CanBeDraggedOutOfVehicle = false;
            this.SetupKiller( driver );
            this._killers.Add( driver );
            //var group1 = World.AddRelationShipGroup( "bank" );
            //var group2 = World.AddRelationShipGroup( "pl" );
            //World.SetRelationshipBetweenGroups( Relationship.Hate, group1, group2 );
            //Game.Player.Character.RelationshipGroup = group2;
            for ( int i = 0; i < this._killersVehicle.PassengerSeats; i++ )
            {
                Ped killer = this._killersVehicle.CreatePedOnSeat( this._vehicleSeat[ i + 1 ], PedHash.Marine01SMM );
                this.SetupKiller( killer );
                this._killers.Add( killer );
            }
            this.Blip = this._killersVehicle.AddBlip();
            this.Blip.Color = BlipColor.Red;
        }

        private void PutAllInCar()
        {
            Ped driver = _killers[ 0 ];
            if ( !driver.IsInVehicle( _killersVehicle ) )
            {
                driver.Task.EnterVehicle( _killersVehicle, VehicleSeat.Driver, 100 );
            }
            for ( int i = 1; i < this._killers.Count; i++ )
            {
                Ped killer = this._killers[ i ];
                if ( !killer.IsInVehicle( this._killersVehicle ) )
                {
                    killer.Task.EnterVehicle( this._killersVehicle, this._vehicleSeat[ i ], 1000 );
                }
            }
            while ( this._killers.Any( killer => !killer.IsInVehicle( this._killersVehicle ) ) )
            {
                this._sleepScript( 200 );
            }
        }

        private void AllLeavesCarExceptDriver()
        {
            while ( (int)Math.Round( this._killersVehicle.Velocity.X ) != 0 ||
                    (int)Math.Round( this._killersVehicle.Velocity.Y ) != 0 )
            {
                this._sleepScript( 200 );
            }
            for ( int i = 1; i < this._killers.Count; i++ )
            {
                Ped killer = this._killers[ i ];
                killer.Task.LeaveVehicle();
            }
            while ( this.IsAllInCarExceptDriver() )
            {
                this._sleepScript( 200 );
            }
        }

        private bool IsAllInCarExceptDriver()
        {
            return
                this._killers.GetRange( 1, this._killers.Count - 1 )
                    .Any( killer => killer.IsInVehicle( this._killersVehicle ) || !killer.IsAlive );
        }

        private void ChaseShootPlayerInCar( bool chase = true )
        {
            Player player = Game.Player;
            Ped driver = this._killers[ 0 ];
            if ( chase )
            {
                Function.Call( Hash.TASK_VEHICLE_CHASE, driver.Handle, player.Character.Handle );
            }
            else
            {
                driver.Task.DriveTo( this._killersVehicle, player.Character.Position, 3f, 90, 0 );
            }
            for ( int i = 1; i < this._killers.Count; i++ )
            {
                Ped killer = this._killers[ i ];
                Function.Call( Hash.TASK_VEHICLE_SHOOT_AT_PED, killer.Handle, player.Character.Handle, 20f );
            }
        }

        public void Check()
        {
            if ( !this._enabled )
            {
                return;
            }
            Player player = Game.Player;
            if ( this._killers.Count( killer => killer.IsAlive ) < 2 )
            {
                this._enabled = false;
                var aliveKiller = this._killers.FirstOrDefault( killer => killer.IsAlive );
                if ( aliveKiller != null )
                {
                    aliveKiller.Task.ReactAndFlee( player.Character );
                    Function.Call( Hash.SET_PED_DROPS_WEAPON, aliveKiller.Handle );
                }
                this._killersVehicle.MarkAsNoLongerNeeded();
                foreach ( Ped killer in this._killers )
                {
                    killer.MarkAsNoLongerNeeded();
                }
                if ( this.Blip != null )
                {
                    this.Blip.Remove();
                }
                return;
            }

            Ped driver = this._killers[ 0 ];
            if ( !driver.IsAlive )
            {
                _killers.Remove( driver );
                driver = _killers[ 0 ];
            }
            if ( player.Character.IsInVehicle() )
            {
                if ( this._type == ActionType.ChaseInVehicle )
                {
                    return;
                }
                this._type = ActionType.ChaseInVehicle;
                this.PutAllInCar();
                Function.Call( Hash.SET_VEHICLE_HANDBRAKE, this._killersVehicle.Handle, false );
                this.ChaseShootPlayerInCar();
            }
            else
            {
                if ( driver.Position.DistanceTo( player.Character.Position ) > 15 )
                {
                    /*if ( this._type == ActionType.ShootingOutsideVehicleTooFar )
                    {
                        return;
                    }*/
                    this._type = ActionType.ShootingOutsideVehicleTooFar;
                    this.PutAllInCar();
                    Function.Call( Hash.SET_VEHICLE_HANDBRAKE, this._killersVehicle.Handle, false );
                    this.ChaseShootPlayerInCar( false );
                    this._sleepScript( 5000 );
                }
                else
                {
                    if ( this._type == ActionType.ShootingOutsideVehicle )
                    {
                        return;
                    }
                    //driver.Task.ClearAllImmediately();
                    //Function.Call( Hash.SET_PED_INTO_VEHICLE, _killersVehicle.Handle, driver.Handle, (int)VehicleSeat.Driver );
                    this._type = ActionType.ShootingOutsideVehicle;
                    Function.Call( Hash.SET_VEHICLE_HANDBRAKE, this._killersVehicle.Handle, true );
                    driver.Task.Wait( 2500 );
                    if ( this.IsAllInCarExceptDriver() )
                    {
                        this.AllLeavesCarExceptDriver();
                    }
                    for ( int i = 1; i < this._killers.Count; i++ )
                    {
                        Ped killer = this._killers[ i ];
                        killer.Task.ClearAllImmediately();
                        killer.Weapons.Select( killer.Weapons[ WeaponHash.AssaultRifle ] );
                        killer.Task.ShootAt( player.Character );
                    }
                }
            }
        }

        public void Start()
        {
            this.SpawnCarAndKillers();
            this._enabled = true;
        }
    }
}