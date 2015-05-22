#region Using

using System;
using System.Collections.Generic;
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

        public Bank( IniFile settings )
        {
            this._settings = settings;
            this._balances = new Dictionary<int, BankAccount>();
            this.LoadSettings();
        }

        public void DepositMoney( Player player, int deposit )
        {
            if ( deposit < 1 || player.Money < deposit )
            {
                throw new Exception( "Not enough money!" );
            }
            int playerIndex = Helper.GetPlayerIndex();
            if ( playerIndex > 3 )
            {
                throw new Exception( "Wrong player index!" );
            }
            player.Money -= deposit;
            this._balances[ playerIndex ].Balance += deposit;
            this.SaveSettings();
        }

        public void WithdrawalMoney( Player player, int value )
        {
            int playerIndex = Helper.GetPlayerIndex();
            if ( playerIndex > 3 )
            {
                throw new Exception( "Wrong player index!" );
            }
            if ( value < 1 || value > this._balances[ playerIndex ].Balance )
            {
                throw new Exception( "Wrong value!" );
            }
            player.Money += value;
            this._balances[ playerIndex ].Balance -= value;
            this.SaveSettings();
        }

        public void AccrueInterest()
        {
            GTADate currDate = Helper.GetCurrentDate();
            for ( int i = 1; i <= 3; i++ )
            {
                BankAccount account = this._balances[ i ];
                if ( account.InterestDate >= currDate )
                {
                    return;
                }
                int interest = (int)Math.Round( account.Balance * this._percentsPerDay );
                account.Balance = account.Balance + interest;
                account.InterestDate = currDate;
                if ( Helper.GetPlayerIndex() == i || interest > 0 )
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
            this._percentsPerDay = this._settings.Read( "PercentsPerDay", "Bank", 0.1 );
        }
    }
}