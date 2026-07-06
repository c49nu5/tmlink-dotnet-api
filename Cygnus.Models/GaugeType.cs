namespace Cygnus.Models;
public enum GaugeType
{
    None = 0,
    M1DIVE = 1,
    M5ROVMM = 2,
    M1OEM= 3,
    M5ROVFMD = 4,
    M5EX = 5,
    M5SG = 6,           // Was M5C6 now M5SG, supports M5C4 gauges too.
    M4UW = 7,           // Connect using M4Subsea
    M4ROV = 8,          // Connect using M4Subsea
    M2DIVE = 9,
    M5C4 = 10,          // Connects using M5SG
    M2ROPE = 11,
    M5ROVASCAN = 12,
    M1FMD = 13,
    M5UW = 14,
    M5C6 = 21,
    M5ROV063 = 22
}
