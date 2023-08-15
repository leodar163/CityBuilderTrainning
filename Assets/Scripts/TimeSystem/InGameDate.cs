namespace TimeSystem
{
    public struct InGameDate
    {
        public int totalMonths;

        public int years => totalMonths / 12;
        public int months => totalMonths % 12;

        public InGameDate(int months)
        {
            totalMonths = months;
        }

        public InGameDate(int years, int relativeMonths)
        {
            totalMonths = years * 12 + relativeMonths;
        }
        
        public static InGameDate operator +(InGameDate a, InGameDate b)
        {
            return new InGameDate(a.totalMonths + b.totalMonths);
        }

        public static InGameDate operator +(InGameDate a, int b)
        {
            return new InGameDate(a.totalMonths + b);
        }

        public static InGameDate operator ++(InGameDate date)
        {
            return new InGameDate(date.totalMonths + 1);
        }
        
        public static InGameDate operator -(InGameDate a, InGameDate b)
        {
            return new InGameDate(a.totalMonths - b.totalMonths);
        }

        public static InGameDate operator -(InGameDate a, int b)
        {
            return new InGameDate(a.totalMonths - b);
        }

        public static InGameDate operator --(InGameDate date)
        {
            return new InGameDate(date.totalMonths - 1);
        }

        public static bool operator ==(InGameDate a, InGameDate b)
        {
            return a.totalMonths == b.totalMonths;
        }

        public static bool operator !=(InGameDate a, InGameDate b)
        {
            return a.totalMonths != b.totalMonths;
        }

        public override bool Equals(object obj)
        {
            return obj != null && this == (InGameDate)obj;
        }

        public override int GetHashCode()
        {
            return totalMonths;
        }

        public override string ToString()
        {
            return $"year: {years}, month: {months}";
        }
    }
}