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
        private ATM _nearestATM;
        private readonly ATM[] _atmList;
        private readonly Bank _bank;
        private readonly MenuController _menuController;
        private readonly IniFile _settings;
        private readonly MySettings _mySettings;
        private readonly CharacterStat[] _charactersStats;

        public AccountInBank()
        {
            this.KeyDown += this.OnKeyDown;
            this.Interval = 1000;
            this.Tick += this.AccountInBank_Tick;

            this._settings = new IniFile( "scripts\\AccountInBank.ini" );
            this._atmList = Helper.GetAllATMs();
            this._bank = new Bank( this._settings );

            this._menuController = new MenuController( this._bank, Game.Player, this );
            this._menuController.MenuClosed += this._menuController_MenuClosed;

            this._mySettings = new MySettings();
            this._charactersStats = new CharacterStat[ 3 ];
            this.LoadCharactersStats();
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

            int playerId = Helper.GetPlayerIndex() - 1;
            if ( this._mySettings.LoseCashOnArrest )
            {
                int arrests = Helper.GetArrestsValueStat( playerId );
                if ( arrests > this._charactersStats[ playerId ].Arrests )
                {
                    this._charactersStats[ playerId ].Arrests = arrests;
                    Game.Player.Money = 0;
                    UI.Notify( "Oh, no! You lost all your cash!" );
                }
            }
            if ( this._mySettings.LoseCashOnDeath )
            {
                int deaths = Helper.GetDeathsValueStat( playerId );
                if ( deaths > this._charactersStats[ playerId ].Deaths )
                {
                    this._charactersStats[ playerId ].Deaths = deaths;
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
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
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
                DateTime endTime = DateTime.Now + new TimeSpan( 0, 0, 0, 0, 2500 );
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
                this._menuController.ShowBankMenu();
                this.Interval = 0;
            }
        }
    }
}