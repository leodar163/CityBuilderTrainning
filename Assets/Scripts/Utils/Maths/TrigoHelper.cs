using UnityEngine;

namespace Utils.Maths
{
    public static class TrigoHelper
    {
        public static bool PointInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
        {
            return IsSameSide(p, a, b, c) && IsSameSide(p, b, a, c) && IsSameSide(p, c, a, b);
        }

        public static bool IsSameSide(Vector3 point1, Vector3 point2, Vector3 a, Vector3 b)
        {
            Vector3 crossProd1 = Vector3.Cross(b - a, point1 - a);
            Vector3 crossProd2 = Vector3.Cross(b - a, point2 - a);

            return Vector3.Dot(crossProd1, crossProd2) >= 0;
        }
    }
}