#region Using

using System.Collections.Generic;
using GTA.Math;

#endregion

namespace AccountInBank
{
    internal static class Helper
    {
        public static ATM[] GetAllATMs()
        {
            var list = new List<ATM>
            {
                new ATM( new Vector3( (float)-1109.797, (float)-1690.808, (float)4.375014 ), (float)122.9616, false ),
                new ATM( new Vector3( (float)-821.6062, (float)-1081.885, (float)11.13243 ), (float)29.3056, false ),
                new ATM( new Vector3( (float)-537.8409, (float)-854.5145, (float)29.28953 ), (float)182.9156, false ),
                new ATM( new Vector3( (float)-1315.744, (float)-834.6907, (float)16.96173 ), (float)311.8347, false ),
                new ATM( new Vector3( (float)-1314.786, (float)-835.9669, (float)16.96015 ), (float)310.3952, false ),
                new ATM( new Vector3( (float)-1570.069, (float)-546.6727, (float)34.95547 ), (float)215.2224, false ),
                new ATM( new Vector3( (float)-1571.018, (float)-547.3666, (float)34.95734 ), (float)218.438, false ),
                new ATM( new Vector3( (float)-866.6416, (float)-187.8008, (float)37.84286 ), (float)123.5083, false ),
                new ATM( new Vector3( (float)-867.6165, (float)-186.1373, (float)37.8433 ), (float)120.8281, false ),
                new ATM( new Vector3( (float)-721.1284, (float)-415.5296, (float)34.98175 ), (float)268.9118, false ),
                new ATM( new Vector3( (float)-254.3758, (float)-692.4947, (float)33.63751 ), (float)159.0533, false ),
                new ATM( new Vector3( (float)24.37422, (float)-946.0142, (float)29.35756 ), (float)339.1346, false ),
                new ATM( new Vector3( (float)130.1186, (float)-1292.669, (float)29.26953 ), (float)300.0509, false ),
                new ATM( new Vector3( (float)129.7023, (float)-1291.954, (float)29.26953 ), (float)303.6002, false ),
                new ATM( new Vector3( (float)129.2096, (float)-1291.14, (float)29.26953 ), (float)298.3779, false ),
                new ATM( new Vector3( (float)288.8256, (float)-1282.364, (float)29.64128 ), (float)271.0125, false ),
                new ATM( new Vector3( (float)1077.768, (float)-776.4548, (float)58.23997 ), (float)186.3605, false ),
                new ATM( new Vector3( (float)527.2687, (float)-160.7156, (float)57.08937 ), (float)272.5496, false ),
                new ATM( new Vector3( (float)-57.6918, (float)-92.56804, (float)57.77821 ), (float)291.214, false ),
                new ATM( new Vector3( (float)-867.5897, (float)-186.1757, (float)37.84291 ), (float)117.088, false ),
                new ATM( new Vector3( (float)-866.6556, (float)-187.7766, (float)37.84278 ), (float)118.6207, false ),
                new ATM( new Vector3( (float)-1205.024, (float)-326.2916, (float)37.83985 ), (float)117.6146, false ),
                new ATM( new Vector3( (float)-1205.703, (float)-324.7474, (float)37.85942 ), (float)117.3035, false ),
                new ATM( new Vector3( (float)-1570.167, (float)-546.7214, (float)34.95663 ), (float)216.3378, false ),
                new ATM( new Vector3( (float)-1571.056, (float)-547.3947, (float)34.95724 ), (float)213.4567, false ),
                new ATM( new Vector3( (float)-57.64693, (float)-92.66162, (float)57.77995 ), (float)295.8091, false ),
                new ATM( new Vector3( (float)527.3583, (float)-160.6381, (float)57.0933 ), (float)271.9661, false ),
                new ATM( new Vector3( (float)-165.1658, (float)234.8314, (float)94.92194 ), (float)90.9362, false ),
                new ATM( new Vector3( (float)-165.1503, (float)232.7887, (float)94.92194 ), (float)94.44688, false ),
                new ATM( new Vector3( (float)-2072.445, (float)-317.3048, (float)13.31597 ), (float)269.9654, false ),
                new ATM( new Vector3( (float)-3241.082, (float)997.5428, (float)12.55044 ), (float)40.48888, false ),
                new ATM( new Vector3( (float)-1091.462, (float)2708.637, (float)18.95291 ), (float)44.16092, false ),
                new ATM( new Vector3( (float)1172.492, (float)2702.492, (float)38.17477 ), (float)359.9989, false ),
                new ATM( new Vector3( (float)1171.537, (float)2702.492, (float)38.17542 ), (float)359.9672, false ),
                new ATM( new Vector3( (float)1822.637, (float)3683.131, (float)34.27678 ), (float)207.7707, false ),
                new ATM( new Vector3( (float)1686.753, (float)4815.806, (float)42.00874 ), (float)272.3396, false ),
                new ATM( new Vector3( (float)1701.209, (float)6426.569, (float)32.76408 ), (float)65.12852, false ),
                new ATM( new Vector3( (float)-95.54314, (float)6457.19, (float)31.46093 ), (float)46.20586, false ),
                new ATM( new Vector3( (float)-97.23336, (float)6455.469, (float)31.46682 ), (float)49.50279, false ),
                new ATM( new Vector3( (float)-386.7451, (float)6046.102, (float)31.50172 ), (float)315.2239, false ),
                new ATM( new Vector3( (float)-1091.42, (float)2708.629, (float)18.95568 ), (float)46.68598, false )
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
    }
}