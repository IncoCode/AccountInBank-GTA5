#region Using

using System;
using System.Drawing;
using System.Threading;
using GTA;

#endregion

namespace AccountInBank
{
    internal class MenuController
    {
        private readonly Bank _bank;
        private readonly Player _player;
        private readonly Script _script;

        public MenuController( Bank bank, Player player, Script script )
        {
            this._bank = bank;
            this._player = player;
            this._script = script;
        }

        #region Static

        public static Point MenuPositioning( int menuWidth, int menuHeight )
        {
            int width = 1280, height = 720;
            return new Point( width / 2 - menuWidth / 2, height / 2 - menuHeight / 2 );
        }

        private static void SetColors( Menu menu )
        {
            menu.HeaderColor = Color.FromArgb( 69, 146, 153 );
            menu.UnselectedItemColor = Color.FromArgb( 79, 188, 198 );
            menu.SelectedItemColor = Color.FromArgb( 92, 220, 232 );
            menu.HasFooter = false;
        }

        #endregion

        private void ShowMenu( Menu menu )
        {
            this._script.View.AddMenu( menu );
            this._script.View.MenuPosition = MenuPositioning( menu.Width, menu.ItemHeight );
        }

        /// <summary>
        /// Closes all menu and displays main bank menu
        /// </summary>
        /// <param name="delay"></param>
        private void CloseAndShowMainMenu( int delay )
        {
            var thread = new Thread( () =>
            {
                Thread.Sleep( delay );
                this._script.View.CloseAllMenus();
                this.ShowBankMenu();
            } ) { Priority = ThreadPriority.Lowest };
            thread.Start();
        }

        private void ShowOperationStatusMenu( string status, Color color )
        {
            var label = new MenuLabel( status );
            var menu = new Menu( "Status", new MenuItem[]
                {
                    label
                } )
            {
                HeaderColor = color,
                UnselectedItemColor = color,
                SelectedItemColor = color,
                HasFooter = false
            };
            menu.Width += 10;
            this.ShowMenu( menu );
        }

        public void ShowBankMenu()
        {
            var menu = new Menu( "ATM Menu", new MenuItem[]
            {
                new MenuButton( "Deposit", this.DepositMenuClick ),
                new MenuButton( "Withdrawal", () => { } ),
            } );
            SetColors( menu );
            this.ShowMenu( menu );
        }

        private void DepositMenuClick()
        {
            string value = Game.GetUserInput( "Enter value...", 9 );
            int deposit;
            if ( int.TryParse( value, out deposit ) )
            {
                string status = "Success!";
                var color = Color.LimeGreen;
                try
                {
                    this._bank.DepositMoney( this._player, deposit );
                }
                catch ( Exception exception )
                {
                    color = Color.Red;
                    status = exception.Message;
                }
                this.ShowOperationStatusMenu( status, color );
                this.CloseAndShowMainMenu( 2000 );
            }
        }
    }
}