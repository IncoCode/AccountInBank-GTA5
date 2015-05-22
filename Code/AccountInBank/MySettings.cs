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

        #endregion

        public MySettings( IniFile settings )
        {
            this._settings = settings;
            this.Load();
        }

        public void Load()
        {
            this.MarkATMKey = Helper.StringToKey( this._settings.Read( "MarkATMKey", "Settings", "B" ), Keys.B );
            this.OpenATMMenuKey = Helper.StringToKey( this._settings.Read( "OpenATMMenuKey", "Settings", "O" ), Keys.O );
        }
    }
}