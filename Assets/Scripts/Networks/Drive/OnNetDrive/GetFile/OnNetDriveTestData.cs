using System.Collections.Generic;

public class OnNetDriveTestData
{
    public readonly Dictionary<string, string> MetaTestData = new Dictionary<string, string>()
    {
        { "ForTestApp.000", "ForTestApp"},
        { "ForTestApp.001", "ForTestApp" },
        { "ForTestApp.002", "ForTestApp" },
        { "ForTestApp.003", "ForTestApp" },
        { "ForTestApp.004", "ForTestApp" },
        { "ForTestApp.005", "ForTestApp" },
        { "ForTestApp.006", "ForTestApp" },
        { "ForTestApp.007", "ForTestApp" },
        { "ForTestApp.008", "ForTestApp" },
    };

    public readonly DLData TestDLData = new DLData(1, "testData", 1);
    public readonly byte[] TestFileByte = new byte[1] { 255 };
}