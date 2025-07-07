using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class VnPayLibrary
{
    private SortedList<string, string> requireField = new SortedList<string, string>();
    public void AddRequestData(string key, string value) => requireField[key] = value;
    public SortedList<string, string> RequireField => requireField;

    public string CreateRequestUrl(string baseUrl, string secretKey)
    {
        var query = new StringBuilder();
        var hashData = new StringBuilder();

        foreach (var kv in requireField)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                query.Append($"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}&");
                hashData.Append($"{kv.Key}={kv.Value}&");
            }
        }

        hashData.Length--;
        string sign = ComputeSha256(secretKey + hashData);
        query.Append($"vnp_SecureHashType=SHA256&vnp_SecureHash={sign}");
        return baseUrl + "?" + query;
    }

    public static string ComputeSha256(string raw)
    {
        using (SHA256 sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

    }
}
