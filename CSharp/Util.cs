using System.Net;
using System.Text;
using System.Security.Cryptography;
using AppConfig;
using AppParam;
using System.Text.Json.Nodes;
using System;
using System.Text.RegularExpressions;

namespace AppUtil
{
  public class Util
  {
    public static async Task HttpGetRequest(string url, Dictionary<string, string> headerRequest)
    {
      Console.WriteLine("========== start call http get =========");
      Console.WriteLine(url);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "GET";
      foreach (var entry in headerRequest)
      {
        string key = entry.Key;
        string value = entry.Value;
        request.Headers.Add(key, value);
      }

      try
      {
        // Get the response
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          string result = reader.ReadToEnd();
          Console.WriteLine(result);
        }
      }
      catch (WebException ex)
      {
        // Handle exceptions
        if (ex.Response is HttpWebResponse errorResponse)
        {
          Console.WriteLine($"Error: {errorResponse.StatusCode} - {errorResponse.StatusDescription}");
          // Handle error response
        }
        else
        {
          Console.WriteLine($"Exception: {ex.Message}");
        }
      }
    }

    public static async Task HttpPutRequest(string url, string content, Dictionary<string, string> headerRequest)
    {
      Console.WriteLine("========== start call http put =========");
      Console.WriteLine(url);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "PUT";
      //request.ContentType = "application/json";
      foreach (var entry in headerRequest)
      {
        string key = entry.Key;
        string value = entry.Value;
        request.Headers.Add(key, value);
      }

      // Write the JSON data to the request stream
      using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
      {
        writer.Write(content);
        writer.Flush();
      }

      try
      {
        // Get the response
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          string result = reader.ReadToEnd();
          Console.WriteLine(result);
        }
      }
      catch (WebException ex)
      {
        // Handle exceptions
        if (ex.Response is HttpWebResponse errorResponse)
        {
          Console.WriteLine($"Error: {errorResponse.StatusCode} - {errorResponse.StatusDescription}");
          // Handle error response
        }
        else
        {
          Console.WriteLine($"Exception: {ex.Message}");
        }
      }
    }

    public static async Task HttpPostRequest(string url, string content, Dictionary<string, string> headerRequest)
    {
      Console.WriteLine("========== start call http post =========");
      Console.WriteLine(url);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "POST";
      //request.ContentType = "application/json";

      foreach (var entry in headerRequest)
      {
        string key = entry.Key;
        string value = entry.Value;
        request.Headers.Add(key, value);
      }

      // Write the JSON data to the request stream
      using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
      {
        writer.Write(content);
        writer.Flush();
      }

      try
      {
        // Get the response
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          string result = reader.ReadToEnd();
          Console.WriteLine(result);
        }
      }
      catch (WebException ex)
      {
        // Handle exceptions
        if (ex.Response is HttpWebResponse errorResponse)
        {
          Console.WriteLine($"Error: {errorResponse.StatusCode} - {errorResponse.StatusDescription}");
          // Handle error response
        }
        else
        {
          Console.WriteLine($"Exception: {ex.Message}");
        }
      }
    }
    public static byte[] Sha256Hash(string data) { return Sha256Hash(Encoding.UTF8.GetBytes(data)); }

    public static HashAlgorithm SHA256Algorithm = HashAlgorithm.Create("SHA-256");
    public static byte[] Sha256Hash(byte[] data) { return SHA256Algorithm.ComputeHash(data); }

    public static string Hex(byte[] data)
    {
      StringBuilder sb = new StringBuilder();
      foreach (byte b in data) sb.Append(b.ToString("x2"));
      return sb.ToString();
    }

    public static string CreateSignatureInput(string mehtod, string createTime, string expireTime)
    {
      string result = "";
      switch (mehtod)
      {
        case "GET":
          result = "sig=(\"@method\" \"@path\")" + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + "\"" + Config.MERCHANT_ID + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
          break;
        case "PUT":
        case "POST":
          result = "sig=(\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")" + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + "\"" + Config.MERCHANT_ID + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
          break;
        default:
          break;
      }
      return result;
    }

    public static string GenerateStringToSign(string method, string path, string contentType, string contentLength, string contentDigest, string createTime, string expiresTime)
    {
      string signMethod = "\"@method\": " + method;
      string signPath = "\"@path\": " + path;
      string signContentDigest = "\"content-digest\": " + contentDigest;
      string signContentType = "\"content-type\": " + contentType;
      string signContentLength = "\"content-length\": " + contentLength;
      string signParam = "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
          + ";" + "created=" + createTime + ";" + "expires=" + expiresTime + ";" + "keyid=" + "\""
          + Config.MERCHANT_ID
          + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
      return signMethod + "\n" + signPath + "\n" + signContentDigest + "\n" + signContentType + "\n"
          + signContentLength + "\n" + signParam;
    }

    public static string GenerateStringToSign(string method, string path, string createTime, string expiresTime)
    {
      string signMethod = "\"@method\": " + method;
      string signPath = "\"@path\": " + path;
      string signParam = "\"@signature-params\": (\"@method\" \"@path\")"
          + ";" + "created=" + createTime + ";" + "expires=" + expiresTime + ";" + "keyid=" + "\""
          + Config.MERCHANT_ID
          + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
      return signMethod + "\n" + signPath + "\n" + signParam;
    }

    public static Dictionary<string, string> CreateHeaderRequest(string signatureInput, string signature)
    {
      Dictionary<string, string> header = new Dictionary<string, string>();
      header.Add(IConstants.SIGNATURE_INPUT, signatureInput);
      header.Add(IConstants.SIGNATURE, "sig=:" + signature + ":");
      return header;
    }

    public static Dictionary<string, string> CreateHeaderRequest(string signatureInput, string signature, string contentType, string contentLength, string contentDigest)
    {
      Dictionary<string, string> header = new Dictionary<string, string>();
      header.Add(IConstants.CONTENT_TYPE, contentType);
      header.Add(IConstants.CONTENT_LENGTH, contentLength);
      header.Add(IConstants.CONTENT_DIGEST, contentDigest);
      header.Add(IConstants.SIGNATURE_INPUT, signatureInput);
      header.Add(IConstants.SIGNATURE, "sig=:" + signature + ":");
      return header;
    }

    public static string GetTimeCreatedAndExpiresValue(string input, string key)
    {
      string pattern = $@"{key}=(\d+)";
      Match match = Regex.Match(input, pattern);
      if (match.Success)
      {
        return match.Groups[1].Value;
      }
      return string.Empty;
    }
  }
}