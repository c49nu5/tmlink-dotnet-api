using System.Runtime.CompilerServices;

namespace Cygnus.Models;

public enum GridType : int
{
    RDRD = 0,
    LDLD,
    RURU,
    LULU,
    DRDR,
    DLDL,
    URUR,
    ULUL,
    RDLD,
    LDRD,
    RULU,
    LURU,
    URDR,
    ULDL,
    DRUR,
    DLUL,
    NUM
};

public static class GridTypeHelpers
{
    private const char Right = 'R';
    private const char Left = 'L';
    private const char Down = 'D';

    public static bool IsHorizontal(this GridType gridType)
    {
        return gridType.ToString().ElementAt(0) is Right or Left;
    }

    public static bool IsSecondAxisGoingForward(this GridType gridType)
    {
        return gridType.ToString().ElementAt(1) is Right or Down;
    }

    public static bool IsFirstAxisAlternating(this GridType gridType)
    {
        return gridType.ToString().ElementAt(0) != gridType.ToString().ElementAt(2);
    }

    public static bool IsFirstAxisGoingForward(this GridType gridType, bool isOddElement)
    {
        if (gridType.IsFirstAxisAlternating() && !isOddElement)
        {
            return gridType.ToString().ElementAt(2) is Right or Down;
        }

        return gridType.ToString().ElementAt(0) is Right or Down;
    }
}
