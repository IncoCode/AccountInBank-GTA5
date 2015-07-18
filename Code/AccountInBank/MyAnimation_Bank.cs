#region Using

using GTA;

#endregion

namespace AccountInBank
{
    internal class MyAnimation_Bank : Script
    {
        public static bool DisableAnimation = false;

        public static void PlayIdleAnimation()
        {
            if ( DisableAnimation )
            {
                return;
            }
            Game.Player.Character.Task.PlayAnimation( "amb@prop_human_atm@male@base", "base", 8f, -1, true, 4f );
        }

        public static void PlayEnterAnimation()
        {
            if ( DisableAnimation )
            {
                return;
            }
            Game.Player.Character.Task.PlayAnimation( "amb@prop_human_atm@male@idle_a", "idle_c", 12f, 6500, true, 4f );
        }

        public static void PlayChooseAnimationWaitPlayIdle()
        {
            if ( DisableAnimation )
            {
                return;
            }
            Game.Player.Character.Task.PlayAnimation( "amb@prop_human_atm@male@idle_a", "idle_b", 8f, 3500, true, 4f );
            Wait( 2500 );
            PlayIdleAnimation();
        }

        public static void PlayExitAnimation()
        {
            if ( DisableAnimation )
            {
                return;
            }
            Game.Player.Character.Task.PlayAnimation( "amb@prop_human_atm@male@exit", "exit", 8f, 6500, true, 1 );
        }
    }
}