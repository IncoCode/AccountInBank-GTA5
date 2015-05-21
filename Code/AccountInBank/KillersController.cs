using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AccountInBank
{
    internal class KillersController
    {
        private readonly List<Ped> _killers;
        private Vehicle _killersVehicle;
        private readonly Action _waitAction;
        private bool _enabled;

        public Blip Blip { get; set; }

        public KillersController( Action waitAction )
        {
            this._waitAction = waitAction;
            _killers = new List<Ped>();
        }

        private void SpawnCarAndKillers()
        {
            //Vector3 position = Game.Player.Character.Position.Around( 100 );
            Vector3 position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5f;
            _killersVehicle = World.CreateVehicle( VehicleHash.Kuruma2, position );
            _killers.Add( _killersVehicle.CreatePedOnSeat( VehicleSeat.Driver, PedHash.Marine01SMY ) );
            var group1 = World.AddRelationShipGroup( "bank" );
            var group2 = World.AddRelationShipGroup( "pl" );
            World.SetRelationshipBetweenGroups( Relationship.Hate, group1, group2 );
            Game.Player.Character.RelationshipGroup = group2;
            for ( int i = 0; i < _killersVehicle.PassengerSeats - 1; i++ )
            {
                Ped killer = _killersVehicle.CreatePedOnSeat( VehicleSeat.Any, PedHash.Marine01SMM );
                //Ped killer = World.CreatePed( PedHash.Marine01SMM, _killersVehicle.Position );
                killer.BlockPermanentEvents = true;
                killer.Armor = 100;
                killer.AlwaysDiesOnLowHealth = false;
                killer.RelationshipGroup = group1;
                killer.Weapons.Give( WeaponHash.AssaultRifle, 100, true, true );
                killer.Weapons[ WeaponHash.AssaultRifle ].Ammo = killer.Weapons[ WeaponHash.AssaultRifle ].MaxAmmo;
                killer.Weapons.Give( WeaponHash.MicroSMG, 100, false, true );
                killer.Weapons[ WeaponHash.MicroSMG ].Ammo = killer.Weapons[ WeaponHash.MicroSMG ].MaxAmmo;
                _killers.Add( killer );
            }
            //Blip = _killersVehicle.AddBlip();
            //Blip.Color = BlipColor.Red;
        }

        private void PutAllInCar()
        {
            for ( int i = 0; i < this._killers.Count; i++ )
            {
                Ped killer = this._killers[ i ];
                if ( !killer.IsInVehicle( this._killersVehicle ) )
                {
                    killer.Task.EnterVehicle( this._killersVehicle, i == 0 ? VehicleSeat.Driver : VehicleSeat.Passenger, 4000 );
                }
            }
            while ( _killers.Any( killer => !killer.IsInVehicle( this._killersVehicle ) ) )
            {
                _waitAction();
            }
        }

        private void AllLeavesCarExceptDriver()
        {
            for ( int i = 1; i < _killers.Count; i++ )
            {
                Ped killer = this._killers[ i ];
                killer.Task.LeaveVehicle();
            }
            while ( IsAllInCarExceptDriver() )
            {
                _waitAction();
            }
        }

        private bool IsAllInCarExceptDriver()
        {
            return _killers.GetRange( 1, _killers.Count - 1 ).Any( killer => killer.IsInVehicle( this._killersVehicle ) );
        }

        private void ChaseShootPlayerInCar()
        {

        }

        public void Check()
        {
            if ( _killers != null && _killers.Count > 0 )
            {
                UI.ShowSubtitle( _killers[ 1 ].Weapons[ WeaponHash.AssaultRifle ].Ammo.ToString(), 2000 );
            }
            if ( !_enabled )
            {
                return;
            }
            Player player = Game.Player;
            Ped driver = _killers[ 0 ];
            if ( player.Character.IsInVehicle() )
            {
                this.PutAllInCar();
                Function.Call( Hash.TASK_VEHICLE_CHASE, driver.Handle, player.Character.Handle );
                for ( int i = 1; i < _killers.Count; i++ )
                {
                    Ped killer = this._killers[ i ];
                    Function.Call( Hash.TASK_VEHICLE_SHOOT_AT_PED, killer.Handle, player.Character.Handle, 20f ); // 20f - distance
                }
            }
            else
            {
                if ( driver.Position.DistanceTo( player.Character.Position ) > 5 )
                {
                    this.PutAllInCar();
                    Function.Call( Hash.TASK_VEHICLE_CHASE, driver.Handle, player.Character.Handle );
                    for ( int i = 1; i < _killers.Count; i++ )
                    {
                        Ped killer = this._killers[ i ];
                        Function.Call( Hash.TASK_VEHICLE_SHOOT_AT_PED, killer.Handle, player.Character.Handle, 20f ); // 20f - distance
                    }
                }
                else
                {
                    _enabled = false;
                    if ( IsAllInCarExceptDriver() )
                    {
                        this.AllLeavesCarExceptDriver();
                    }
                    unchecked
                    {
                        UI.ShowSubtitle( "SHOOT", 2000 );
                        for ( int i = 1; i < _killers.Count; i++ )
                        {
                            Ped killer = this._killers[ i ];
                            //killer.Weapons[ WeaponHash.AssaultRifle ].Ammo = 200;
                            //killer.Weapons[ WeaponHash.AssaultRifle ].AmmoInClip = 15;
                            killer.Weapons.Select( killer.Weapons[ WeaponHash.AssaultRifle ] );
                            //Function.Call( Hash.SET_PED_AMMO, killer.Handle, (int)WeaponHash.AssaultRifle, 200 );
                            //UI.ShowSubtitle( killer.Weapons.Current.Ammo.ToString(), 2000 );
                            killer.Task.ShootAt( player.Character );
                            Game.Player.Character.Weapons[ WeaponHash.AssaultRifle ].Ammo = 1000;
                        }
                    }
                }
            }
        }

        public void Start()
        {
            SpawnCarAndKillers();
            _enabled = true;
        }
    }
}
