#region Using

using System;
using GTA;
using Ini;

#endregion

namespace AccountInBank
{
    internal class Bank
    {
        private int _balance;
        private double _percentsPerDay = 0.1;
        private readonly IniFile _settings;
        private GTADate _interestDate;

        #region Fields

        public int Balance
        {
            get { return this._balance; }
        }

        public double PercentsPerDay
        {
            get { return this._percentsPerDay; }
        }

        #endregion

        public Bank( IniFile settings )
        {
            this._settings = settings;
            this.LoadSettings();
        }

        public void DepositMoney( Player player, int deposit )
        {
            if ( deposit < 1 || player.Money < deposit )
            {
                throw new Exception( "Not enough money!" );
            }
            player.Money -= deposit;
            this._balance += deposit;
            this.SaveSettings();
        }

        public void WithdrawalMoney( Player player, int value )
        {
            if ( value < 1 || value > this._balance )
            {
                throw new Exception( "Wrong value!" );
            }
            player.Money += value;
            this._balance -= value;
            this.SaveSettings();
        }

        public void AccrueInterest()
        {
            GTADate currDate = Helper.GetCurrentDate();
            if ( this._interestDate >= currDate )
            {
                return;
            }
            int interest = (int)Math.Round( this._balance * this._percentsPerDay );
            this._balance = this._balance + interest;
            this._interestDate = currDate;
            UI.Notify( "Interest accrued: $" + interest );
            this.SaveSettings();
        }

        private void SaveSettings()
        {
            this._settings.Write( "Balance", this._balance, "Bank" );
            this._settings.Write( "PercentsPerDay", this._percentsPerDay, "Bank" );
            this._settings.Write( "InterestDate", this._interestDate.ToString(), "Bank" );
        }

        private void LoadSettings()
        {
            this._balance = this._settings.Read( "Balance", "Bank", 0 );
            this._percentsPerDay = this._settings.Read( "PercentsPerDay", "Bank", 0.1 );
            this._interestDate =
                GTADate.Parse( this._settings.Read( "InterestDate", "Bank", Helper.GetCurrentDate().ToString() ) );
        }
    }
}