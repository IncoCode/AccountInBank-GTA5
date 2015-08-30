#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using NativeUI;

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
        private readonly MenuPool _menuPool;
        private UIMenu _mainMenu;

        public event EventHandler<EventArgs> MenuClosed;

        public MenuController( Bank bank, Script script )
        {
            this._bank = bank;
            this._script = script;
            this._menuPool = new MenuPool();
            this.CreateMenus();
        }

        public void Tick()
        {
            _menuPool.ProcessMenus();
        }

        #region Create Menus

        private void CreateMainMenu()
        {
            var showBalanceBnt = new UIMenuItem( "Show balance" );
            showBalanceBnt.Activated += ( sender, args ) =>
            {
                UI.Notify( "Balance: $" + this._bank.Balance, true );
            };

            var depositBtn = new UIMenuItem( "Deposit" );
            depositBtn.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Deposit );
            };

            var withdrawalBtn = new UIMenuItem( "Withdrawal" );
            withdrawalBtn.Activated += ( sender, args ) =>
            {
                MyAnimation_Bank.PlayChooseAnimationWaitPlayIdle();
                this.ATMBalanceActionMenuClick( ATMBalanceAction.Withdrawal );
            };

            var moneyTransferBtn = new UIMenuItem( "Money transfer" );
            var moneyTransferMenu = this.GetMoneyTransferMenu();

            var menu = new UIMenu( "Bank menu", "" );
            menu.AddItem( showBalanceBnt );
            menu.AddItem( depositBtn );
            menu.AddItem( withdrawalBtn );
            menu.AddItem( moneyTransferBtn );
            menu.OnMenuClose += sender =>
            {
                // fire OnClose event
                var onMenuClosed = this.MenuClosed;
                if ( onMenuClosed != null )
                {
                    onMenuClosed( this, new EventArgs() );
                }
            };
            menu.BindMenuToItem( moneyTransferMenu, moneyTransferBtn );
            menu.RefreshIndex();
            menu.Visible = false;

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
                string valueS = Game.GetUserInput( 9 );
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
                UI.Notify( status );
                menu.GoBack();
            };
            return menu;
        }

        private void CreateMenus()
        {
            this.CreateMainMenu();
        }

        #endregion

        public void ShowBankMenu()
        {
            this._mainMenu.Visible = true;
        }

        private void ATMBalanceActionMenuClick( ATMBalanceAction action )
        {
            this._script.View.CloseAllMenus();
            string valueS = Game.GetUserInput( 9 );
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
            UI.Notify( status );
        }
    }
}