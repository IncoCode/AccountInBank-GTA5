#region Using

using System;
using System.Globalization;
using System.Windows.Forms;
using AccountInBank.Model;
using GTA;
using GTA.Math;
using GTA.Native;

#endregion

namespace AccountInBank
{
    internal enum PlayerModel
    {
        Michael = 1,
        Franklin = 2,
        Trevor = 3,
        None = 4
    }

    internal static class Helper
    {
        public static ATM[] GetAllATMs()
        {
            return new[]
            {
                new ATM( new Vector3( -1109.797f, -1690.808f, 4.375014f ), 122.9616f, false ),
                new ATM( new Vector3( -821.6062f, -1081.885f, 11.13243f ), 29.3056f, false ),
                new ATM( new Vector3( -537.8409f, -854.5145f, 29.28953f ), 182.9156f, false ),
                new ATM( new Vector3( -1315.744f, -834.6907f, 16.96173f ), 311.8347f, false ),
                new ATM( new Vector3( -1314.786f, -835.9669f, 16.96015f ), 310.3952f, false ),
                new ATM( new Vector3( -1570.069f, -546.6727f, 34.95547f ), 215.2224f, false ),
                new ATM( new Vector3( -1571.018f, -547.3666f, 34.95734f ), 218.438f, false ),
                new ATM( new Vector3( -866.6416f, -187.8008f, 37.84286f ), 123.5083f, false ),
                new ATM( new Vector3( -867.6165f, -186.1373f, 37.8433f ), 120.8281f, false ),
                new ATM( new Vector3( -721.1284f, -415.5296f, 34.98175f ), 268.9118f, false ),
                new ATM( new Vector3( -254.3758f, -692.4947f, 33.63751f ), 159.0533f, false ),
                new ATM( new Vector3( 24.37422f, -946.0142f, 29.35756f ), 339.1346f, false ),
                new ATM( new Vector3( 130.1186f, -1292.669f, 29.26953f ), 300.0509f, false ),
                new ATM( new Vector3( 129.7023f, -1291.954f, 29.26953f ), 303.6002f, false ),
                new ATM( new Vector3( 129.2096f, -1291.14f, 29.26953f ), 298.3779f, false ),
                new ATM( new Vector3( 288.8256f, -1282.364f, 29.64128f ), 271.0125f, false ),
                new ATM( new Vector3( 1077.768f, -776.4548f, 58.23997f ), 186.3605f, false ),
                new ATM( new Vector3( 527.2687f, -160.7156f, 57.08937f ), 272.5496f, false ),
                new ATM( new Vector3( -867.5897f, -186.1757f, 37.84291f ), 117.088f, false ),
                new ATM( new Vector3( -866.6556f, -187.7766f, 37.84278f ), 118.6207f, false ),
                new ATM( new Vector3( -1205.024f, -326.2916f, 37.83985f ), 117.6146f, false ),
                new ATM( new Vector3( -1205.703f, -324.7474f, 37.85942f ), 117.3035f, false ),
                new ATM( new Vector3( -1570.167f, -546.7214f, 34.95663f ), 216.3378f, false ),
                new ATM( new Vector3( -1571.056f, -547.3947f, 34.95724f ), 213.4567f, false ),
                new ATM( new Vector3( -57.64693f, -92.66162f, 57.77995f ), 295.8091f, false ),
                new ATM( new Vector3( 527.3583f, -160.6381f, 57.0933f ), 271.9661f, false ),
                new ATM( new Vector3( -165.1658f, 234.8314f, 94.92194f ), 90.9362f, false ),
                new ATM( new Vector3( -165.1503f, 232.7887f, 94.92194f ), 94.44688f, false ),
                new ATM( new Vector3( -2072.445f, -317.3048f, 13.31597f ), 269.9654f, false ),
                new ATM( new Vector3( -3241.082f, 997.5428f, 12.55044f ), 40.48888f, false ),
                new ATM( new Vector3( -1091.462f, 2708.637f, 18.95291f ), 44.16092f, false ),
                new ATM( new Vector3( 1172.492f, 2702.492f, 38.17477f ), 359.9989f, false ),
                new ATM( new Vector3( 1171.537f, 2702.492f, 38.17542f ), 359.9672f, false ),
                new ATM( new Vector3( 1822.637f, 3683.131f, 34.27678f ), 207.7707f, false ),
                new ATM( new Vector3( 1686.753f, 4815.806f, 42.00874f ), 272.3396f, false ),
                new ATM( new Vector3( 1701.209f, 6426.569f, 32.76408f ), 65.12852f, false ),
                new ATM( new Vector3( -95.54314f, 6457.19f, 31.46093f ), 46.20586f, false ),
                new ATM( new Vector3( -97.23336f, 6455.469f, 31.46682f ), 49.50279f, false ),
                new ATM( new Vector3( -386.7451f, 6046.102f, 31.50172f ), 315.2239f, false ),
                new ATM( new Vector3( -1091.42f, 2708.629f, 18.95568f ), 46.68598f, false ),
                new ATM( new Vector3( 5.132f, -919.7711f, 29.55953f ), 250.4304f, false ),
                new ATM( new Vector3( -660.703f, -853.971f, 24.484f ), 180.1663f, false ),
                new ATM( new Vector3( -2293.827f, 354.817f, 174.602f ), 115.5668f, false ),
                new ATM( new Vector3( -2294.637f, 356.553f, 174.602f ), 113.6837f, false ),
                new ATM( new Vector3( -2295.377f, 358.241f, 174.648f ), 110.682f, false )
            };
        }

        /// <summary>
        /// Returns nearest ATM to the player
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static ATM GetNearestATM( Vector3 position )
        {
            ATM[] atmList = GetAllATMs();
            float minDist = float.MaxValue;
            ATM nearestAtm = null;
            foreach ( ATM atm in atmList )
            {
                float currDist = atm.Position.DistanceTo( position );
                if ( currDist < minDist )
                {
                    minDist = currDist;
                    nearestAtm = atm;
                }
            }
            return nearestAtm;
        }

        /// <summary>
        /// Returns current game date
        /// </summary>
        /// <returns></returns>
        public static GTADate GetCurrentDate()
        {
            int day = Function.Call<int>( Hash.GET_CLOCK_DAY_OF_MONTH );
            int month = Function.Call<int>( Hash.GET_CLOCK_MONTH );
            int year = Function.Call<int>( Hash.GET_CLOCK_YEAR );
            return new GTADate( year, month, day );
        }

        public static int GetPlayerIndex()
        {
            Player player = Game.Player;
            switch ( (PedHash)player.Character.Model.Hash )
            {
                case PedHash.Michael:
                    return 1;

                case PedHash.Franklin:
                    return 2;

                case PedHash.Trevor:
                    return 3;

                default:
                    return 4;
            }
        }

        public static Keys StringToKey( string key, Keys defaultValue )
        {
            if ( string.IsNullOrEmpty( key ) )
            {
                return defaultValue;
            }
            Keys resKey = defaultValue;
            try
            {
                resKey = (Keys)Enum.Parse( typeof( Keys ), key.Trim(), true );
            }
            catch
            {
                return resKey;
            }
            return resKey;
        }

        public static unsafe int GetDeathsValueStat( int playerId )
        {
            int hashDeaths = Function.Call<int>( Hash.GET_HASH_KEY, "SP" + playerId + "_DEATHS" );
            int deaths = 0;
            Function.Call<bool>( Hash.STAT_GET_INT, hashDeaths, &deaths, -1 );
            return deaths;
        }

        public static unsafe int GetArrestsValueStat( int playerId )
        {
            int hashArrests = Function.Call<int>( Hash.GET_HASH_KEY, "SP" + playerId + "_BUSTED" );
            int arrests = 0;
            Function.Call<bool>( Hash.STAT_GET_INT, hashArrests, &arrests, -1 );
            return arrests;
        }

        /// <summary>
        /// Can parse signle numbers or return a percent from baseVal (then valueS should be like "50%").
        /// Also support ALL (equivalent to 100%).
        /// </summary>
        /// <param name="valueS"></param>
        /// <param name="baseVal"></param>
        /// <param name="value"></param>
        /// <param name="ignoreAll"></param>
        /// <returns></returns>
        public static bool GetNumWithPercent( string valueS, int baseVal, out int value, bool ignoreAll = false )
        {
            valueS = valueS.Trim();
            if ( int.TryParse( valueS, out value ) )
            {
                return true;
            }
            if ( valueS.ToUpper() == "ALL" )
            {
                if ( ignoreAll )
                {
                    return false;
                }
                value = baseVal;
                return true;
            }
            if ( !valueS.EndsWith( "%" ) )
            {
                return false;
            }
            float percent;
            if (
                !float.TryParse( valueS.Substring( 0, valueS.Length - 1 ), NumberStyles.Number,
                    CultureInfo.CreateSpecificCulture( "en-US" ), out percent ) )
            {
                return false;
            }
            if ( percent < 0 || percent > 100 )
            {
                return false;
            }
            if ( percent == 100 && ignoreAll )
            {
                return false;
            }
            value = (int)( baseVal * ( percent / 100 ) );
            return true;
        }

        public static string ToReadableNumber( double x, int significantDigits = 0 )
        {
            string[] prefixes = { "f", "a", "p", "n", "μ", "m", string.Empty, "k", "M", "G", "T", "P", "E" };
            // check for special numbers and non-numbers
            if ( double.IsInfinity( x ) || double.IsNaN( x ) || x == 0 || significantDigits <= 0 )
            {
                return x.ToString();
            }
            // extract sign so we deal with positive numbers only
            int sign = Math.Sign( x );
            x = Math.Abs( x );
            // get scientific exponent, 10^3, 10^6, ...
            int sci = x == 0 ? 0 : (int)Math.Floor( Math.Log( x, 10 ) / 3 ) * 3;
            // scale number to exponent found
            x = x * Math.Pow( 10, -sci );
            // find number of digits to the left of the decimal
            int dg = x == 0 ? 0 : (int)Math.Floor( Math.Log( x, 10 ) ) + 1;
            // adjust decimals to display
            int decimals = Math.Min( significantDigits - dg, 15 );
            // format for the decimals
            string fmt = new string( '0', decimals );
            if ( sci == 0 )
            {
                //no exponent
                return string.Format( "{0}{1:0." + fmt + "}",
                    sign < 0 ? "-" : string.Empty,
                    Math.Round( x, decimals ) );
            }
            // find index for prefix. every 3 of sci is a new index
            int index = sci / 3 + 6;
            if ( index >= 0 && index < prefixes.Length )
            {
                // with prefix
                return string.Format( "{0}{1:0." + fmt + "}{2}",
                    sign < 0 ? "-" : string.Empty,
                    Math.Round( x, decimals ),
                    prefixes[ index ] );
            }
            // with 10^exp format
            return string.Format( "{0}{1:0." + fmt + "}·10^{2}",
                sign < 0 ? "-" : string.Empty,
                Math.Round( x, decimals ),
                sci );
        }
    }
}