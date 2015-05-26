#region Using

using System;
using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace AccountInBank.Model
{
    internal class ATM : IDisposable
    {
        public float Heading { get; private set; }
        public Vector3 Position { get; private set; }
        public Blip Blip { get; private set; }

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

        public Blip CreateBlip( bool showRoute = true, bool isBlipShortRange = false )
        {
            this.Blip = World.CreateBlip( this.Position );
            this.Blip.Sprite = 108; // dollar sprite
            this.Blip.Color = BlipColor.Green;
            this.Blip.ShowRoute = showRoute;
            if ( isBlipShortRange )
            {
                Function.Call( Hash.SET_BLIP_AS_SHORT_RANGE, this.Blip.Handle, true );
            }
            return this.Blip;
        }

        public bool IsInRange( Vector3 position, int range = 2 )
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