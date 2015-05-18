#region Using

using System;
using System.Drawing;
using System.Threading;
using GTA;

#endregion

namespace AccountInBank
{
    internal enum ATMBalanceAction
    {
        Deposit,
        Withdrawal
    }

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

        private static void SetDefaultColors( Menu menu )
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

        private void ShowOperationStatusMenu( string status, bool autoClose = false, int autoCloseDelay = 2000,
            Color? headerColor = null, Color? unselectedColor = null, Color? selectedColor = null )
        {
            var label = new MenuLabel( status );
            var menu = new Menu( "Status", new MenuItem[]
            {
                label
            } )
            {
                HasFooter = false
            };
            SetDefaultColors( menu );
            if ( headerColor != null )
            {
                menu.HeaderColor = (Color)headerColor;
            }
            if ( unselectedColor != null )
            {
                menu.UnselectedItemColor = (Color)unselectedColor;
            }
            if ( selectedColor != null )
            {
                menu.SelectedItemColor = (Color)selectedColor;
            }
            menu.Width += 10;
            this.ShowMenu( menu );
            if ( autoClose )
            {
                this.CloseAndShowMainMenu( autoCloseDelay );
            }
        }

        private void ShowOperationStatusMenu( string status, bool autoClose = false, int autoCloseDelay = 2000,
            Color? allColor = null )
        {
            this.ShowOperationStatusMenu( status, autoClose, autoCloseDelay, allColor, allColor, allColor );
        }

        public void ShowBankMenu()
        {
            var menu = new Menu( "ATM Menu", new MenuItem[]
            {
                new MenuButton( "Show balance",
                    () => { this.ShowOperationStatusMenu( "Balance: " + this._bank.Balance, true, 3000, null ); } ),
                new MenuButton( "Deposit", () => { this.ATMBalanceActionMenuClick( ATMBalanceAction.Deposit ); } ),
                new MenuButton( "Withdrawal", () => { this.ATMBalanceActionMenuClick( ATMBalanceAction.Withdrawal ); } ),
                new MenuButton( "Close", () => { this._script.View.CloseAllMenus(); } ),
            } );
            SetDefaultColors( menu );
            this.ShowMenu( menu );
        }

        private void ATMBalanceActionMenuClick( ATMBalanceAction action )
        {
            string valueS = Game.GetUserInput( "Enter value...", 9 );
            int deposit;
            string status = "Incorrect value!";
            Color color = Color.Red;
            if ( int.TryParse( valueS, out deposit ) )
            {
                status = "Success!";
                color = Color.LimeGreen;
                try
                {
                    switch ( action )
                    {
                        case ATMBalanceAction.Deposit:
                            this._bank.DepositMoney( this._player, deposit );
                            break;

                        case ATMBalanceAction.Withdrawal:
                            this._bank.WithdrawalMoney( this._player, deposit );
                            break;

                        default:
                            return;
                    }
                }
                catch ( Exception exception )
                {
                    color = Color.Red;
                    status = exception.Message;
                }
            }
            this.ShowOperationStatusMenu( status, true, 2000, color );
        }
    }
}