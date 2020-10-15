#region Using

using GTA;
using NativeUI;
using System;
using System.Linq;

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
        private readonly MenuPool _menuPool;
        private readonly MySettings _mySettings;
        private UIMenu _mainMenu;
        private bool _wasMenuOpen = false;

        public event EventHandler<EventArgs> MenuClosed;

        public MenuController( Bank bank, MySettings settings )
        {
            this._bank = bank;
            this._mySettings = settings;
            this._menuPool = new MenuPool();
        }

        public void Tick()
        {
            this._menuPool.ProcessMenus();
            if ( this._wasMenuOpen && !this._menuPool.IsAnyMenuOpen() )
            {
                this._wasMenuOpen = false;
                this.MenuClosed?.Invoke( this, new EventArgs() );
            }
        }

        #region Create Menus

        private void CreateMainMenu()
        {
            var showBalanceBnt = new UIMenuItem( "Show balance" );
            showBalanceBnt.Activated += ( sender, args ) =>
            {
                if ( this._mySettings.EnableAnimation )
                {
                    AccountInBankAnimation.PlayChooseAnimationWaitPlayIdle();
                }
                GTA.UI.Notification.Show( "Balance: $" + this._bank.Balance, true );
            };

            var depositBtn = new UIMenuItem( "Deposit" );
            depositBtn.Activated += ( sender, args ) =>
            {
                if ( this._mySettings.EnableAnimation )
                {
                    AccountInBankAnimation.PlayChooseAnimationWaitPlayIdle();
                }
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Deposit );
            };

            var withdrawalBtn = new UIMenuItem( "Withdrawal" );
            withdrawalBtn.Activated += ( sender, args ) =>
            {
                if ( this._mySettings.EnableAnimation )
                {
                    AccountInBankAnimation.PlayChooseAnimationWaitPlayIdle();
                }
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Withdrawal );
            };

            var moneyTransferBtn = new UIMenuItem( "Money transfer" );
            var moneyTransferMenu = this.GetMoneyTransferMenu();

            var menu = new UIMenu( "Bank menu", "" );
            menu.AddItem( showBalanceBnt );
            menu.AddItem( depositBtn );
            menu.AddItem( withdrawalBtn );
            menu.AddItem( moneyTransferBtn );
            menu.BindMenuToItem( moneyTransferMenu, moneyTransferBtn );
            menu.RefreshIndex();

            this._mainMenu = menu;
            this._menuPool.Add( this._mainMenu );
            this._menuPool.Add( moneyTransferMenu );
        }

        private UIMenu GetMoneyTransferMenu()
        {
            int playerIndex = Helper.GetPlayerIndex();
            PlayerModel[] availableCharacters =
                Enum.GetValues( typeof( PlayerModel ) )
                    .Cast<PlayerModel>()
                    .Where( p => (int)p != playerIndex && p != PlayerModel.None )
                    .ToArray();
            var menu = new UIMenu( "Money Transfer", "" );
            var availableCharactersList = new UIMenuListItem( "Character", availableCharacters.Cast<dynamic>().ToList(),
                0 );
            menu.AddItem( availableCharactersList );
            UIMenuItem nextBtn = new UIMenuItem( "Next" );
            menu.AddItem( nextBtn );
            nextBtn.Activated += ( sender, item ) =>
            {
                string valueS = Game.GetUserInput( new WindowTitle(), "", 9 );
                string status = "Operation \"Withdrawal\": ~g~Success!";
                try
                {
                    int target = ( (int)availableCharacters[ availableCharactersList.Index ] );
                    this._bank.TransferMoney( target, valueS );
                }
                catch ( Exception exception )
                {
                    status = $"Operation \"Withdrawal\": ~r~Failed!~n~Error: {exception.Message}";
                }
                GTA.UI.Notification.Show( status );
                menu.GoBack();
            };
            return menu;
        }

        #endregion

        public void ShowBankMenu()
        {
            this.CreateMainMenu();
            this._mainMenu.Visible = true;
            this._wasMenuOpen = true;
        }

        private void ATMBalanceActionMenuClick( ATMBalanceAction action )
        {
            string valueS = Game.GetUserInput( new WindowTitle(), "", 9 );
            string status = $"Operation \"{action}\": ~g~Success!";
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
                status = $"Operation \"{action}\": ~r~Failed!~n~Error: {exception.Message}";
            }
            GTA.UI.Notification.Show( status );
        }
    }
}