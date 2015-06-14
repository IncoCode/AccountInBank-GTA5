#region Using

using System;
using System.Drawing;
using System.Linq;
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
        private readonly Script _script;
        private int _selectedChar;

        public event EventHandler<EventArgs> MenuClosed;

        public MenuController( Bank bank, Script script )
        {
            this._bank = bank;
            this._script = script;
        }

        #region Static

        private static Point MenuPositioning( int menuWidth, int menuHeight )
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
            var menu = new Menu( "Status", new IMenuItem[]
            {
                label
            } )
            {
                HasFooter = false
            };
            SetDefaultColors( menu );
            if ( headerColor.HasValue )
            {
                menu.HeaderColor = headerColor.Value;
            }
            if ( unselectedColor.HasValue )
            {
                menu.UnselectedItemColor = unselectedColor.Value;
            }
            if ( selectedColor.HasValue )
            {
                menu.SelectedItemColor = selectedColor.Value;
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
            var showBalanceBnt = new MenuButton( "Show balance" );
            showBalanceBnt.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.ShowOperationStatusMenu( "Balance: $" + this._bank.Balance, true, 3000, null );
            };

            var depositBtn = new MenuButton( "Deposit" );
            depositBtn.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Deposit );
            };

            var withdrawalBtn = new MenuButton( "Withdrawal" );
            withdrawalBtn.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Withdrawal );
            };

            var moneyTransferBtn = new MenuButton( "Money transfer" );
            moneyTransferBtn.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.MoneyTransferActionMenuClick();
            };

            var closeBtn = new MenuButton( "Close" );
            closeBtn.Activated += ( sender, args ) =>
            {
                this._script.View.CloseAllMenus();
                // fire OnClose event
                var onMenuClosed = this.MenuClosed;
                if ( onMenuClosed != null )
                {
                    onMenuClosed( this, new EventArgs() );
                }
            };

            var menu = new Menu( "ATM Menu",
                new IMenuItem[] { showBalanceBnt, depositBtn, withdrawalBtn, moneyTransferBtn, closeBtn } );
            SetDefaultColors( menu );
            this.ShowMenu( menu );
        }

        private void ATMBalanceActionMenuClick( ATMBalanceAction action )
        {
            string valueS = Game.GetUserInput( "Enter value...", 9 );
            string status = "Success!";
            Color color = Color.LimeGreen;
            try
            {
                switch ( action )
                {
                    case ATMBalanceAction.Deposit:
                        this._bank.DepositMoney( Game.Player, valueS );
                        break;

                    case ATMBalanceAction.Withdrawal:
                        this._bank.WithdrawalMoney( Game.Player, valueS );
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
            this.ShowOperationStatusMenu( status, true, 2000, color );
        }

        private void MoneyTransferActionMenuClick()
        {
            this._script.View.CloseAllMenus();
            int playerIndex = Helper.GetPlayerIndex();
            PlayerModel[] availableCharacters =
                Enum.GetValues( typeof( PlayerModel ) )
                    .Cast<PlayerModel>()
                    .Where( p => (int)p != playerIndex && p != PlayerModel.None )
                    .ToArray();
            var charactedEnumScroller = new MenuEnumScroller( "Character", "",
                Array.ConvertAll( availableCharacters, p => p.ToString() ) );
            charactedEnumScroller.Changed += ( sender, args ) => this._selectedChar = charactedEnumScroller.Index;

            var nextBtn = new MenuButton( "Next" );
            nextBtn.Activated += ( sender, args ) =>
            {
                string valueS = Game.GetUserInput( "Enter value...", 9 );
                var status = "Success!";
                var color = Color.LimeGreen;
                try
                {
                    int target = ( (int)availableCharacters[ this._selectedChar ] );
                    this._bank.TransferMoney( target, valueS );
                }
                catch ( Exception exception )
                {
                    color = Color.Red;
                    status = exception.Message;
                }
                this.ShowOperationStatusMenu( status, true, 2000, color );
            };

            var menu = new Menu( "Money transfer", new IMenuItem[] { charactedEnumScroller, nextBtn } );
            menu.Width += 25;
            SetDefaultColors( menu );
            this.ShowMenu( menu );
        }
    }
}