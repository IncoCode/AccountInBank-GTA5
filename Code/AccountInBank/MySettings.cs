#region Using

using System.Windows.Forms;
using Ini;

#endregion

namespace AccountInBank
{
    internal class MySettings
    {
        private readonly IniFile _settings;

        #region Fields

        public Keys MarkATMKey { get; private set; }
        public Keys OpenATMMenuKey { get; private set; }
        public bool LoseCashOnDeath { get; private set; }
        public bool LoseCashOnArrest { get; private set; }
        public bool ShowAllATMLocations { get; private set; }

        public Keys ActivateKey { get; private set; }
        public Keys BackKey { get; private set; }
        public Keys LeftKey { get; private set; }
        public Keys RightKey { get; private set; }
        public Keys UpKey { get; private set; }
        public Keys DownKey { get; private set; }

        public bool EnableDepositTax { get; set; }
        public string DepositTax { get; set; }
        public bool EnableWithdrawalTax { get; set; }
        public string WithdrawalTax { get; set; }
        public bool EnableServiceTax { get; set; } // every day
        public string ServiceTax { get; set; }
        public bool EnableMoneyTransferTax { get; set; }
        public string MoneyTransferTax { get; set; }

        #endregion

        public MySettings()
        {
            this._settings = new IniFile( "scripts\\AccountInBankS.ini" );
            this.Load();
        }

        private void Load()
        {
            this.MarkATMKey = Helper.StringToKey( this._settings.Read( "MarkATMKey", "Settings", "B" ), Keys.B );
            this.OpenATMMenuKey = Helper.StringToKey( this._settings.Read( "OpenATMMenuKey", "Settings", "O" ), Keys.O );
            this.LoseCashOnDeath = this._settings.Read( "LoseCachOnDeath", "Settings", false );
            this.LoseCashOnArrest = this._settings.Read( "LoseCachOnArrest", "Settings", false );
            this.ShowAllATMLocations = this._settings.Read( "ShowAllATMLocations", "Settings", false );

            this.ActivateKey = Helper.StringToKey( this._settings.Read( "ActivateKey", "MenuNavigation", "NumPad5" ),
                Keys.NumPad5 );
            this.BackKey = Helper.StringToKey( this._settings.Read( "BackKey", "MenuNavigation", "NumPad0" ),
                Keys.NumPad0 );
            this.LeftKey = Helper.StringToKey( this._settings.Read( "LeftKey", "MenuNavigation", "NumPad4" ),
                Keys.NumPad4 );
            this.RightKey = Helper.StringToKey( this._settings.Read( "RightKey", "MenuNavigation", "NumPad6" ),
                Keys.NumPad6 );
            this.UpKey = Helper.StringToKey( this._settings.Read( "UpKey", "MenuNavigation", "NumPad8" ), Keys.NumPad8 );
            this.DownKey = Helper.StringToKey( this._settings.Read( "DownKey", "MenuNavigation", "NumPad2" ),
                Keys.NumPad2 );

            this.EnableDepositTax = this._settings.Read( "EnableDepositTax", "Taxes", false );
            this.EnableWithdrawalTax = this._settings.Read( "EnableWithdrawalTax", "Taxes", false );
            this.EnableServiceTax = this._settings.Read( "EnableServiceTax", "Taxes", false );
            this.EnableMoneyTransferTax = this._settings.Read( "EnableMoneyTransferTax", "Taxes", false );
            this.DepositTax = this._settings.Read( "DepositTax", "Taxes", "3%" );
            this.WithdrawalTax = this._settings.Read( "WithdrawalTax", "Taxes", "5%" );
            this.ServiceTax = this._settings.Read( "ServiceTax", "Taxes", "0.5%" );
            this.MoneyTransferTax = this._settings.Read( "MoneyTransferTax", "Taxes", "2%" );
        }
    }
}