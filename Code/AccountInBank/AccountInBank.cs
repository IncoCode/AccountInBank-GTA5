#region Using

using System;
using System.Linq;
using System.Windows.Forms;
using AccountInBank.Model;
using GTA;
using Ini;

#endregion

namespace AccountInBank
{
    internal class AccountInBank : Script
    {
        private ATM _nearestATM = null;
        private readonly Player _player;
        private readonly Ped _playerPed;
        private readonly ATM[] _atmList;
        private readonly Bank _bank;
        private readonly MenuController _menuController;
        private readonly IniFile _settings;

        public AccountInBank()
        {
            this.KeyDown += this.OnKeyDown;
            this.Interval = 1000;
            this.Tick += this.AccountInBank_Tick;

            this._player = Game.Player;
            this._playerPed = this._player.Character;
            this._settings = new IniFile( "scripts\\AccountInBank.ini" );
            this._atmList = Helper.GetAllATMs();
            this._bank = new Bank( this._settings );
            this._menuController = new MenuController( this._bank, this._player, this );
            this._menuController.MenuClosed += this._menuController_MenuClosed;
        }

        private void _menuController_MenuClosed( object sender, EventArgs e )
        {
            this.Interval = 1000;
            this._player.CanControlCharacter = true;
        }

        private void AccountInBank_Tick( object sender, EventArgs e )
        {
            this._bank.AccrueInterest();
        }

        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.B )
            {
                if ( !this._player.CanControlCharacter )
                {
                    this._player.CanControlCharacter = true;
                }
                if ( this._nearestATM == null )
                {
                    this._nearestATM = Helper.GetNearestATM( this._playerPed.Position );
                    this._nearestATM.CreateBlip();
                }
                else
                {
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
                }
            }
            else if ( e.KeyCode == Keys.O )
            {
                ATM nearATM = this._atmList.FirstOrDefault( atm => atm.IsInRange( this._playerPed.Position ) );
                // if player isn't near some atm
                if ( nearATM == null )
                {
                    return;
                }
                if ( this._nearestATM != null )
                {
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
                }
                this._player.CanControlCharacter = false;
                this._player.Character.Task.SlideToCoord( nearATM.Position, nearATM.Heading );
                do
                {
                    Wait( 200 );
                }
                while ( Math.Abs( this._player.Character.Velocity.X ) + Math.Abs( this._player.Character.Velocity.Y ) >
                        0 || (int)this._player.Character.Heading != (int)nearATM.Heading );
                this._menuController.ShowBankMenu();
                this.Interval = 0;
            }
        }
    }
}