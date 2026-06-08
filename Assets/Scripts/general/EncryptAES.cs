using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 暗号化および復号を行うクラス
/// </summary>
public class EncryptAES
{
    //暗号化に利用する初期化ベクトル
    private readonly string AES_IV = "lNCQD2wgDBeToxas";

    //暗号化鍵
    private readonly string AES_KEY = "YMQDrQtEfwhoUUHhE883vGgr6XqU4JE0";

    /// <summary>
    /// 暗号化された文字列から文字列の復号を行う
    /// </summary>
    public string TxtToDecryptTxt(string cipherTxt)
    {
        //ファイルから読み込んだ文字列をバイト配列に変換
        byte[] cipherByteArray = strToByteArray(cipherTxt);

        string plainTxt;
        //復号処理
        plainTxt = DecryptStrFromByte(cipherByteArray);

        return plainTxt;
    }

    /// <summary>
    /// 暗号化したい文字列を暗号化して、文字列として返す
    /// </summary>
    public string TxtToEncryptTxt(string plainTxt)
    {
        byte[] encryptedByteArray = EncryptStrToByte(plainTxt);
        return byteToStr(encryptedByteArray);
    }

    /// <summary>
    /// 利用するAESの設定を行う
    /// </summary>
    private void CommonAesSetting(Aes aesAlg)
    {
        aesAlg.BlockSize = 128;
        aesAlg.KeySize = 256;
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.Padding = PaddingMode.PKCS7;

        aesAlg.IV = Encoding.UTF8.GetBytes(AES_IV);
        aesAlg.Key = Encoding.UTF8.GetBytes(AES_KEY);
    }

    /// <summary>
    /// 文字列からバイト配列へ暗号化を行う
    /// </summary>
    private byte[] EncryptStrToByte(string plainTxt)
    {
        byte[] encrypted;

        using (Aes aesAlg = Aes.Create())
        {
            CommonAesSetting(aesAlg);
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //暗号化
                        swEncrypt.Write(plainTxt);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    private string byteToStr(byte[] byteArray)
    {
        return System.Convert.ToBase64String(byteArray);
    }

    /// <summary>
    /// 暗号化されたバイト配列を文字列として復号するメソッド
    /// </summary>
    private string DecryptStrFromByte(byte[] cipherByteArray)
    {
        string plainTxt = null;
        using (Aes aesAlg = Aes.Create())
        {
            CommonAesSetting(aesAlg);
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherByteArray))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plainTxt = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plainTxt;
    }

    private byte[] strToByteArray(string cipherTxt)
    {
        return System.Convert.FromBase64String(cipherTxt);
    }
}
