#region Using

using System;
using System.Collections.Generic;
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
        private ATM _nearestATM;
        private readonly ATM[] _atmList;
        private readonly Bank _bank;
        private readonly MenuController _menuController;
        private readonly IniFile _settings;
        private readonly MySettings _mySettings;
        private readonly CharacterStat[] _charactersStats;
        private readonly List<Blip> _atmBlips;

        public AccountInBank()
        {
            this.KeyDown += this.OnKeyDown;
            this.Interval = 1000;
            this.Tick += this.AccountInBank_Tick;

            this._settings = new IniFile( "scripts\\AccountInBank.ini" );
            this._mySettings = new MySettings();

            this._atmList = Helper.GetAllATMs();
            this._bank = new Bank( this._settings, this._mySettings );

            this._menuController = new MenuController( this._bank, this._mySettings );
            this._menuController.MenuClosed += this._menuController_MenuClosed;

            this._charactersStats = new CharacterStat[ 3 ];
            this.LoadCharactersStats();
            this._atmBlips = new List<Blip>();
            if ( this._mySettings.ShowAllATMLocations )
            {
                this.PrintAllBlips();
            }
        }

        protected override void Dispose( bool disposing )
        {
            if ( !disposing )
            {
                return;
            }
            Game.Player.CanControlCharacter = true;
            if ( this._nearestATM != null )
            {
                this._nearestATM.Dispose();
                this._nearestATM = null;
            }
            foreach ( Blip blip in this._atmBlips.Where( blip => blip != null && blip.Exists() ) )
            {
                blip.Remove();
            }
        }

        private void PrintAllBlips()
        {
            foreach ( ATM atm in this._atmList )
            {
                this._atmBlips.Add( atm.CreateBlip( false, true ) );
            }
        }

        private void _menuController_MenuClosed( object sender, EventArgs e )
        {
            if ( this._mySettings.EnableAnimation )
            {
                AccountInBankAnimation.PlayExitAnimation();
                Wait( 6500 );
            }
            this.Interval = 1000;
            Game.Player.CanControlCharacter = true;
        }

        private void AccountInBank_Tick( object sender, EventArgs e )
        {
            this._bank.AccrueInterest();
            this._menuController.Tick();

            int playerId = Helper.GetPlayerIndex() - 1;
            CharacterStat charStats = this._charactersStats[ playerId ];
            if ( this._mySettings.LoseCashOnArrest )
            {
                int arrests = Helper.GetArrestsValueStat( playerId );
                if ( arrests > charStats.Arrests )
                {
                    charStats.Arrests = arrests;
                    Game.Player.Money = 0;
                    UI.Notify( "Oh, no! You lost all your cash!" );
                }
            }
            if ( this._mySettings.LoseCashOnDeath )
            {
                int deaths = Helper.GetDeathsValueStat( playerId );
                if ( deaths > charStats.Deaths )
                {
                    charStats.Deaths = deaths;
                    Game.Player.Money = 0;
                    UI.Notify( "Oh, no! You lost all your cash!" );
                }
            }
        }

        private void LoadCharactersStats()
        {
            for ( int i = 0; i < 3; i++ )
            {
                int deaths = Helper.GetDeathsValueStat( i );
                int arrests = Helper.GetArrestsValueStat( i );
                this._charactersStats[ i ] = new CharacterStat( i, deaths, arrests );
            }
        }

        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == this._mySettings.MarkATMKey )
            {
                if ( this._nearestATM == null )
                {
                    this._nearestATM = Helper.GetNearestATM( Game.Player.Character.Position );
                    this._nearestATM.CreateBlip();
                }
                else
                {
                    if ( this._nearestATM != null )
                    {
                        this._nearestATM.Dispose();
                        this._nearestATM = null;
                    }
                }
            }
            else if ( e.KeyCode == this._mySettings.OpenATMMenuKey )
            {
                ATM nearATM = this._atmList.FirstOrDefault( atm => atm.IsInRange( Game.Player.Character.Position ) );
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
                Game.Player.CanControlCharacter = false;
                Game.Player.Character.Task.SlideTo( nearATM.Position, nearATM.Heading );
                DateTime endTime = DateTime.Now + new TimeSpan( 0, 0, 0, 0, 4500 );
                do
                {
                    Wait( 200 );
                    if ( DateTime.Now >= endTime )
                    {
                        break;
                    }
                }
                while ( Math.Abs( Game.Player.Character.Velocity.X ) + Math.Abs( Game.Player.Character.Velocity.Y ) >
                        0 || (int)Game.Player.Character.Heading != (int)nearATM.Heading );
                if ( this._mySettings.EnableAnimation )
                {
                    AccountInBankAnimation.PlayEnterAnimation();
                    Wait( 6500 );
                    AccountInBankAnimation.PlayIdleAnimation();
                }
                this.Interval = 0;
                this._menuController.ShowBankMenu();
            }
        }
    }
}