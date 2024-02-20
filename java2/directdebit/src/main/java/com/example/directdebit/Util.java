package com.example.directdebit;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.security.MessageDigest;
import java.util.HashMap;
import java.util.Map;
import java.util.logging.Logger;

public class Util {
  private static final Logger logger = Logger.getLogger(Util.class.getName());

  public static void executeGet(String targetURL, Map<String, Object> headerMap) throws IOException {
    try {
      // Create URL object
      URL url = new URL(targetURL);
      // Open a connection
      HttpURLConnection connection = (HttpURLConnection) url.openConnection();

      // Set the request method to GET
      connection.setRequestMethod("GET");

      // Set headers
      for (Map.Entry<String, Object> entry : headerMap.entrySet()) {
        String key = entry.getKey();
        String value = entry.getValue().toString();
        connection.setRequestProperty(key, value);
      }

      // Add any other headers as needed

      // Get the response code
      int responseCode = connection.getResponseCode();
      logger.info("Response code: " + responseCode);

      // Read the response
      try (BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()))) {
        String line;
        StringBuilder response = new StringBuilder();
        while ((line = reader.readLine()) != null) {
          response.append(line);
        }
        logger.info("Response body: " + response.toString());
      } catch (IOException e) {
        // Handle exceptions
        e.printStackTrace();
      } finally {
        // Close the connection
        connection.disconnect();
      }
    } catch (Exception e) {
      logger.info(e.getMessage());
    }
  }

  public static void executePut(String targetURL, Map<String, Object> headerMap,
      String content) {
    HttpURLConnection connection = null;
    try {
      // Create connection
      URL url = new URL(targetURL);
      connection = (HttpURLConnection) url.openConnection();
      connection.setRequestMethod("PUT");

      // Set Header
      for (Map.Entry<String, Object> entry : headerMap.entrySet()) {
        String key = entry.getKey();
        String value = entry.getValue().toString();
        connection.setRequestProperty(key, value);
      }

      connection.setDoOutput(true);
      // Send request
      DataOutputStream wr = new DataOutputStream(
          connection.getOutputStream());
      wr.writeBytes(content);
      wr.close();

      // Get Response
      BufferedReader in = new BufferedReader(
          new InputStreamReader(connection.getInputStream()));
      String inputLine;
      StringBuilder contentStr = new StringBuilder();
      while ((inputLine = in.readLine()) != null) {
        contentStr.append(inputLine);
      }
      in.close();
      logger.info("Response code: " + connection.getResponseCode());
      logger.info("Response body: " + contentStr.toString());

    } catch (Exception e) {
      logger.info(e.getMessage());
    } finally {
      if (connection != null) {
        connection.disconnect();
      }
    }
  }

  public static void executePost(String targetURL, Map<String, Object> headerMap,
      String content) {
    HttpURLConnection connection = null;
    try {
      // Create connection
      URL url = new URL(targetURL);
      connection = (HttpURLConnection) url.openConnection();
      connection.setRequestMethod("POST");

      // Set Header
      for (Map.Entry<String, Object> entry : headerMap.entrySet()) {
        String key = entry.getKey();
        String value = entry.getValue().toString();
        connection.setRequestProperty(key, value);
      }

      connection.setDoOutput(true);
      // Send request
      DataOutputStream wr = new DataOutputStream(
          connection.getOutputStream());
      wr.writeBytes(content);
      wr.close();

      // Get Response
      BufferedReader in = new BufferedReader(
          new InputStreamReader(connection.getInputStream()));
      String inputLine;
      StringBuilder contentStr = new StringBuilder();
      while ((inputLine = in.readLine()) != null) {
        contentStr.append(inputLine);
      }
      in.close();
      logger.info("Response code: " + connection.getResponseCode());
      logger.info("Response body: " + contentStr.toString());

    } catch (Exception e) {
      e.printStackTrace();
    } finally {
      if (connection != null) {
        connection.disconnect();
      }
    }
  }

  public static String hex(byte[] data) {
    StringBuilder sb = new StringBuilder();
    byte[] var2 = data;
    int var3 = data.length;

    for (int var4 = 0; var4 < var3; ++var4) {
      byte b = var2[var4];
      sb.append(String.format("%02x", b & 255));
    }

    return sb.toString();
  }

  public static byte[] sha256Hash(String data) throws Exception {
    return sha256Hash(data.getBytes("UTF-8"));
  }

  public static byte[] sha256Hash(byte[] data) throws Exception {
    MessageDigest md = MessageDigest.getInstance("SHA-256");
    md.update(data);
    return md.digest();
  }

  public static String createSignatureInput(String method, String createTime, String expireTime) {
    String result = "";
    switch (method) {
      case IConstants.GET:
        result = "sig=(\"@method\" \"@path\")" + ";" + "created=" + createTime + ";" + "expires="
            + expireTime + ";" + "keyid=" + "\"" + Config.MERCHANT_ID + "\"" + ";" + "alg=" + "\"" + Config.ALG
            + "\"";
        break;
      case IConstants.PUT:
        result = "sig=(\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
            + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + "\""
            + Config.MERCHANT_ID + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
        break;
      case IConstants.POST:
        result = "sig=(\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
            + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + "\""
            + Config.MERCHANT_ID + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
        break;
      default:
        break;
    }
    return result;
  }

  /* Use for method PUT|POST */
  public static String generateStringToSign(String method, String path, String contentType, String contentLength,
      String contentDigest, String createTime, String expiresTime) {
    String signMethod = "\"@method\": " + method;
    String signPath = "\"@path\": " + path;
    String signContentDigest = "\"content-digest\": " + contentDigest;
    String signContentType = "\"content-type\": " + contentType;
    String signContentLength = "\"content-length\": " + contentLength;
    String signParam = "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
        + ";" + "created=" + createTime + ";" + "expires=" + expiresTime + ";" + "keyid=" + "\""
        + Config.MERCHANT_ID
        + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
    return signMethod + "\n" + signPath + "\n" + signContentDigest + "\n" + signContentType + "\n"
        + signContentLength + "\n" + signParam;
  }

  /* Use for method GET */
  public static String generateStringToSign(String method, String path, String createTime, String expiresTime) {

    String signMethod = "\"@method\": " + method;
    String signPath = "\"@path\": " + path;
    String signParam = "\"@signature-params\": (\"@method\" \"@path\")"
        + ";" + "created=" + createTime + ";" + "expires=" + expiresTime + ";" + "keyid=" + "\""
        + Config.MERCHANT_ID
        + "\"" + ";" + "alg=" + "\"" + Config.ALG + "\"";
    return signMethod + "\n" + signPath + "\n" + signParam;
  }

  /* Use for method GET */
  public static Map<String, Object> createHeaderRequest(String signatureInput, String signature) {
    Map<String, Object> header = new HashMap<>();
    header.put(IConstants.SIGNATURE_INPUT, signatureInput);
    header.put(IConstants.SIGNATURE, "sig=:" + signature + ":");
    header.put(IConstants.ACCEPT, "text/plain, */*");
    return header;
  }

  /* Use for method PUT|POST */
  public static Map<String, Object> createHeaderRequest(String signatureInput, String signature, String contentType,
      String contentLength, String contentDigest) {
    Map<String, Object> header = new HashMap<>();
    header.put(IConstants.CONTENT_TYPE, contentType);
    header.put(IConstants.CONTENT_LENGTH, contentLength);
    header.put(IConstants.CONTENT_DIGEST, contentDigest);
    header.put(IConstants.SIGNATURE_INPUT, signatureInput);
    header.put(IConstants.SIGNATURE, "sig=:" + signature + ":");
    header.put(IConstants.ACCEPT, "application/json, text/plain, */*");
    return header;
  }
}
