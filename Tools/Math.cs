using System.Numerics;

class MathTools
{
    public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T>
    {
        while (a != default && b != default)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a == default ? b : a;
    }

    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T> =>
        (a * b) / GreatestCommonDivisor(a, b);
}