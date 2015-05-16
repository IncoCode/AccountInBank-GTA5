#region Using

using System.Linq;
using System.Windows.Forms;
using GTA;

#endregion

namespace AccountInBank
{
    internal class AccountInBank : Script
    {
        private ATM _nearestATM = null;
        private readonly Player _player;
        private readonly Ped _playerPed;
        private ATM[] _atmList;

        public AccountInBank()
        {
            this.KeyDown += this.OnKeyDown;
            this._player = Game.Player;
            this._playerPed = this._player.Character;
            this._atmList = Helper.GetAllATMs();
        }

        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.B )
            {
                if ( this._nearestATM == null )
                {
                    this._nearestATM = Helper.GetNearestATM( this._playerPed.Position );
                    this._nearestATM.CreateBlip();
                }
                else
                {
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
                }
            }
            else if ( e.KeyCode == Keys.O )
            {
                ATM nearATM = this._atmList.FirstOrDefault( atm => atm.IsInRange( this._playerPed.Position ) );
                // if player isn't near some atm
                if ( nearATM == null )
                {
                    return;
                }
                if ( this._nearestATM != null )
                {
                    this._nearestATM.Dispose();
                    this._nearestATM = null;
                }
                UI.ShowSubtitle( "InRange", 2000 );
            }
        }
    }
}