#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using AccountInBank.Model;
using GTA;
using Ini;

#endregion

namespace AccountInBank
{
    internal class Bank
    {
        private double _percentsPerDay = 0.1;
        private readonly IniFile _settings;
        private readonly MySettings _mySettings;
        private readonly Dictionary<int, BankAccount> _balances;

        #region Fields

        public int Balance
        {
            get
            {
                int playerIndex = Helper.GetPlayerIndex();
                return playerIndex > 3 ? 0 : this._balances[ playerIndex ].Balance;
            }
        }

        public double PercentsPerDay
        {
            get { return this._percentsPerDay; }
        }

        #endregion

        public Bank( IniFile settings, MySettings mySettings )
        {
            this._settings = settings;
            this._mySettings = mySettings;
            this._balances = new Dictionary<int, BankAccount>();
            this.LoadSettings();
        }

        public void DepositMoney( Player player, string depositS )
        {
            int deposit;
            if ( !Helper.GetNumWithPercent( depositS, player.Money, out deposit ) )
            {
                throw new Exception( "Wrong data!" );
            }
            if ( deposit < 1 || player.Money < deposit )
            {
                throw new Exception( "Not enough money!" );
            }
            int playerIndex = Helper.GetPlayerIndex();
            if ( playerIndex > 3 )
            {
                throw new Exception( "Wrong player index!" );
            }
            BankAccount account = this._balances[ playerIndex ];
            if ( (double)account.Balance + deposit > int.MaxValue )
            {
                throw new Exception( "Reached the maximum value!" );
            }
            int tax = 0;
            if ( this._mySettings.EnableDepositTax )
            {
                if ( !Helper.GetNumWithPercent( this._mySettings.DepositTax, deposit, out tax, true ) )
                {
                    throw new Exception( "Wrong DepositTax value!" );
                }
                if ( deposit + tax > player.Money )
                {
                    deposit -= deposit + tax - player.Money;
                }
            }
            player.Money -= deposit + tax;
            account.Balance += deposit;
            this.SaveSettings();
        }

        public void WithdrawalMoney( Player player, string valueS )
        {
            int value;
            int playerIndex = Helper.GetPlayerIndex();
            if ( playerIndex > 3 )
            {
                throw new Exception( "Wrong player index!" );
            }
            if ( !Helper.GetNumWithPercent( valueS, this._balances[ playerIndex ].Balance, out value ) )
            {
                throw new Exception( "Wrong data!" );
            }
            if ( value < 1 || value > this._balances[ playerIndex ].Balance )
            {
                throw new Exception( "Wrong value!" );
            }
            if ( (double)player.Money + value > int.MaxValue )
            {
                throw new Exception( "Reached the maximum value!" );
            }
            int tax = 0;
            if ( this._mySettings.EnableWithdrawalTax )
            {
                if ( !Helper.GetNumWithPercent( this._mySettings.WithdrawalTax, value, out tax, true ) )
                {
                    throw new Exception( "Wrong WithdrawalTax value!" );
                }
                if ( value + tax > this._balances[ playerIndex ].Balance )
                {
                    value -= value + tax - this._balances[ playerIndex ].Balance;
                }
            }
            player.Money += value;
            this._balances[ playerIndex ].Balance -= value + tax;
            this.SaveSettings();
        }

        public void TransferMoney( int targetPlayer, string valueS )
        {
            int playerIndex = Helper.GetPlayerIndex();
            int value;
            if ( targetPlayer == playerIndex )
            {
                throw new Exception( "Wrong target index!" );
            }
            if ( !Helper.GetNumWithPercent( valueS, this._balances[ playerIndex ].Balance, out value ) )
            {
                throw new Exception( "Wrong data!" );
            }
            if ( value < 1 || value > this._balances[ playerIndex ].Balance )
            {
                throw new Exception( "Wrong value!" );
            }
            if ( (double)this._balances[ targetPlayer ].Balance + value > int.MaxValue )
            {
                throw new Exception( "Reached the maximum value!" );
            }
            this._balances[ playerIndex ].Balance -= value;
            this._balances[ targetPlayer ].Balance += value;
            this.SaveSettings();
        }

        public void AccrueInterest()
        {
            GTADate currDate = Helper.GetCurrentDate();
            if ( currDate.Year == 2010 && this._balances.Any( v => v.Value.InterestDate.Year > 2010 ) )
            {
                for ( int i = 1; i <= 3; i++ )
                {
                    BankAccount account = this._balances[ i ];
                    account.InterestDate = new GTADate( 2010, 0, 1 );
                }
                this.SaveSettings();
                return;
            }
            for ( int i = 1; i <= 3; i++ )
            {
                BankAccount account = this._balances[ i ];
                if ( account.InterestDate >= currDate )
                {
                    continue;
                }
                int interest = (int)Math.Round( account.Balance * this._percentsPerDay );
                account.InterestDate = currDate;
                if ( (double)account.Balance + interest > int.MaxValue || interest < 0 )
                {
                    continue;
                }
                account.Balance = account.Balance + interest;
                if ( Helper.GetPlayerIndex() == i && interest > 0 )
                {
                    UI.Notify( "Interest accrued: $" + interest );
                }
            }
            this.SaveSettings();
        }

        private void SaveSettings()
        {
            for ( int i = 1; i <= 3; i++ )
            {
                BankAccount account = this._balances[ i ];
                this._settings.Write( "Balance" + i, account.Balance, "Bank" );
                this._settings.Write( "InterestDate" + i, account.InterestDate.ToString(), "Bank" );
            }
            this._settings.Write( "PercentsPerDay", this._percentsPerDay, "Bank" );
        }

        private void LoadSettings()
        {
            this._balances.Clear();
            for ( int i = 1; i <= 3; i++ )
            {
                int balance = this._settings.Read( "Balance" + i, "Bank", 0 );
                GTADate interestDate =
                    GTADate.Parse( this._settings.Read( "InterestDate" + i, "Bank", Helper.GetCurrentDate().ToString() ) );
                this._balances[ i ] = new BankAccount( balance, interestDate );
            }
            this._percentsPerDay = this._settings.Read( "PercentsPerDay", "Bank", 0.001 );
        }
    }
}