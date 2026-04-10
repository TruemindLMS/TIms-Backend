namespace TeamIndia.TalentFlow.Application.Helpers;

public static class CertificateCodeHelper
{
    public static string GenerateFriendlyCertificateCode(Guid id)
    {
        var bytes = id.ToByteArray();
        var part = BitConverter.ToUInt64(bytes.Concat(new byte[2]).ToArray(), 0) & 0x0000FFFFFFFFFFFFUL;
        var hex = part.ToString("X");
        if (hex.Length < 8) hex = hex.PadLeft(8, '0');
        var a = hex.Substring(0, 4);
        var b = hex.Substring(4, 4);
        return $"TF-{a}-{b}";
    }
}
