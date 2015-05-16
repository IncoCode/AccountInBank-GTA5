#region Using

using System;
using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace AccountInBank
{
    internal class ATM : IDisposable
    {
        public float Heading { get; set; }
        public Vector3 Position { get; set; }
        public Blip Blip { get; set; }

        private bool _disposed;

        public ATM( Vector3 position, float heading, bool createBlip )
        {
            this.Position = position;
            this.Heading = heading;
            if ( createBlip )
            {
                this.CreateBlip();
            }
        }

        public void CreateBlip()
        {
            this.Blip = World.CreateBlip( this.Position );
            Function.Call( Hash.SET_BLIP_SPRITE, this.Blip.Handle, 108 ); // dollar sprite
            this.Blip.Color = BlipColor.Green;
            this.Blip.ShowRoute = true;
        }

        public bool IsInRange( Vector3 position, int range = 1 )
        {
            return range >= this.Position.DistanceTo( position );
        }

        #region Members

        protected virtual void Dispose( bool disposing )
        {
            if ( !this._disposed )
            {
                if ( disposing )
                {
                    this.Blip.Remove();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        #endregion
    }
}