using System;

public static class UUIDGenerator
{
    public static string GenerateUUID()
    {
        //uuidの生成(デフォルトでVersion4による生成)
        Guid uuid = Guid.NewGuid();
        return uuid.ToString("N");
    }
}