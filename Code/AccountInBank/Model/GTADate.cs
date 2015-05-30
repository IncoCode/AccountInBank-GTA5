#region Using

using System;
using System.IO;

#endregion

namespace AccountInBank.Model
{
    public class GTADate : IComparable
    {
        private int _month;
        private int _day;

        #region Fields

        public int Year { get; set; }

        public int Month
        {
            get { return this._month; }
            set
            {
                if ( value < 0 || value > 11 )
                {
                    throw new InvalidDataException();
                }
                this._month = value;
            }
        }

        public int Day
        {
            get { return this._day; }
            set
            {
                if ( value < 1 || value > 31 )
                {
                    throw new InvalidDataException();
                }
                this._day = value;
            }
        }

        #endregion

        public GTADate( int year, int month, int day )
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        public static GTADate Parse( string dateStr )
        {
            string[] dateArr = dateStr.Split( '.' );
            return new GTADate( int.Parse( dateArr[ 0 ] ), int.Parse( dateArr[ 1 ] ), int.Parse( dateArr[ 2 ] ) );
        }

        #region Members

        public int CompareTo( object obj )
        {
            GTADate date = obj as GTADate;
            if ( date == null )
            {
                return 1;
            }
            if ( this.Year == date.Year )
            {
                if ( this.Month == date.Month )
                {
                    return this.Day.CompareTo( date.Day );
                }
                return this.Month.CompareTo( date.Month );
            }
            return this.Year.CompareTo( date.Year );
        }

        public static bool operator >( GTADate left, GTADate right )
        {
            return left.CompareTo( right ) > 0;
        }

        public static bool operator <( GTADate left, GTADate right )
        {
            return left.CompareTo( right ) < 0;
        }

        public static bool operator ==( GTADate left, GTADate right )
        {
            if ( ReferenceEquals( left, null ) )
            {
                return ReferenceEquals( right, null );
            }
            return left.Equals( right );
        }

        public static bool operator !=( GTADate left, GTADate right )
        {
            return !( left == right );
        }

        public static bool operator >=( GTADate left, GTADate right )
        {
            return left == right || left > right;
        }

        public static bool operator <=( GTADate left, GTADate right )
        {
            return left == right || left < right;
        }

        public override int GetHashCode()
        {
            return this.Year.GetHashCode() ^ this.Month.GetHashCode() ^ this.Day.GetHashCode();
        }

        public override string ToString()
        {
            return this.Year + "." + this.Month + "." + this.Day;
        }

        public override bool Equals( object obj )
        {
            var date = obj as GTADate;
            if ( date == null )
            {
                return false;
            }
            return this.Year == date.Year && this.Month == date.Month && this.Day == date.Day;
        }

        #endregion
    }
}