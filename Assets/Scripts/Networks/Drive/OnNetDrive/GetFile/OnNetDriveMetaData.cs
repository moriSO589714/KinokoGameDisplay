
using System.Collections.Generic;

public interface OnNetDriveMetaData
{
    Dictionary<string, string> GetFileList(string driveFolderPath);
}