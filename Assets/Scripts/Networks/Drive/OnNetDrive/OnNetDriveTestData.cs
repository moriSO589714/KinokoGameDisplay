using System.Collections.Generic;

public class OnNetDriveTestData
{
    public readonly Dictionary<string, string> MetaTestData = new Dictionary<string, string>()
    {
        { "testData.000", "demoID"},
        { "testData.001", "demoID" },
    };

    public readonly DLData TestDLData = new DLData(1, "testData", 1);
    public readonly byte[] TestFileByte = new byte[1] { 255 };
}