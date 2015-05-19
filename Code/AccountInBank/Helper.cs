#region Using

using System.Collections.Generic;
using GTA.Math;
using GTA.Native;

#endregion

namespace AccountInBank
{
    internal static class Helper
    {
        public static ATM[] GetAllATMs()
        {
            var list = new List<ATM>
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
                new ATM( new Vector3( -57.6918f, -92.56804f, 57.77821f ), 291.214f, false ),
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
                new ATM( new Vector3( -1091.42f, 2708.629f, 18.95568f ), 46.68598f, false )
            };

            return list.ToArray();
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
    }
}