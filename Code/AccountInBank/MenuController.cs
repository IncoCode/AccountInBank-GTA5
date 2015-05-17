#region Using

using System.Drawing;
using GTA;

#endregion

namespace AccountInBank
{
    internal static class MenuController
    {
        public static Point MenuPositioning( int menuWidth, int menuHeight )
        {
            int width = 1280, height = 720;
            return new Point( width / 2 - menuWidth / 2, height / 2 - menuHeight / 2 );
        }

        private static void SetColors( Menu menu )
        {
            menu.HeaderColor = Color.FromArgb( 69, 146, 153 );
            menu.UnselectedItemColor = Color.FromArgb( 79, 188, 198 );
            menu.SelectedItemColor = Color.FromArgb( 92, 220, 232 );
            menu.HasFooter = false;
        }

        public static Menu GetBankMenu()
        {
            var menu = new Menu( "ATM Menu", new MenuItem[]
            {
                new MenuButton( "Deposit", () => { } ),
                new MenuButton( "Withdrawal", () => { } ),
            } );
            SetColors( menu );
            return menu;
        }
    }
}