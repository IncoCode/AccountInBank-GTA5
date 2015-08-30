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
        private List<Blip> _atmBlips;

        public AccountInBank()
        {
            this.KeyDown += this.OnKeyDown;
            this.Interval = 1000;
            this.Tick += this.AccountInBank_Tick;

            this._settings = new IniFile( "scripts\\AccountInBank.ini" );
            this._mySettings = new MySettings();

            this._atmList = Helper.GetAllATMs();
            this._bank = new Bank( this._settings, this._mySettings );

            this._menuController = new MenuController( this._bank, this );
            this._menuController.MenuClosed += this._menuController_MenuClosed;

            this._charactersStats = new CharacterStat[ 3 ];
            this.LoadCharactersStats();
            if ( this._mySettings.ShowAllATMLocations )
            {
                this.PrintAllBlips();
            }
            this.ApplyMenuNavigationKeys();
            this._atmBlips = new List<Blip>();
            MyAnimation_Bank.DisableAnimation = this._mySettings.DisableAnimation;
        }

        protected override void Dispose( bool A_0 )
        {
            if ( A_0 )
            {
                if ( this._nearestATM != null )
                {
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
                }
                foreach ( Blip blip in this._atmBlips )
                {
                    if ( blip != null && blip.Exists() )
                    {
                        blip.Remove();
                    }
                }
            }
        }

        private void ApplyMenuNavigationKeys()
        {
            this.ActivateKey = this._mySettings.ActivateKey;
            this.BackKey = this._mySettings.BackKey;
            this.LeftKey = this._mySettings.LeftKey;
            this.RightKey = this._mySettings.RightKey;
            this.UpKey = this._mySettings.UpKey;
            this.DownKey = this._mySettings.DownKey;
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
            this.Interval = 1000;
            MyAnimation_Bank.PlayExitAnimation();
            Wait( 6500 );
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
                Game.Player.Character.Task.SlideToCoord( nearATM.Position, nearATM.Heading );
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
                MyAnimation_Bank.PlayEnterAnimation();
                Wait( 6500 );
                MyAnimation_Bank.PlayIdleAnimation();
                this.Interval = 0;
                this._menuController.ShowBankMenu();
            }
        }
    }
}