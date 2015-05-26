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
        }
    }
}