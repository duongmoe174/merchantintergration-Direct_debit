const axios = require("axios");
const crypto = require("crypto");
const IConstants = require("./IContants");
const Config = require("./Config");

async function makeGetRequest(url, header) {
  console.log(url);
  const config = {
    method: "get",
    url: url,
    headers: header,
  };
  let res = await axios(config);
  console.log(res.status);
  console.log(res.data);
}

async function makePutRequest(url, data, header) {
  const config = {
    method: "put",
    url: url,
    data: data,
    headers: header,
  };
  let res = await axios(config);
  console.log(res.status);
  console.log(res.data);
}

async function makePostRequest(url, data, header) {
  const config = {
    method: "post",
    url: url,
    data: data,
    headers: header,
  };
  let res = await axios(config);
  console.log(res.status);
  console.log(res.data);
}

function sha256Hash(data) {
  const hash = crypto.createHash("sha256");
  hash.update(data, "utf8");
  return hash.digest();
}

function createSignatureInput(method, createTime, expireTime) {
  let result = "";
  switch (method) {
    case IConstants.GET:
      result =
        'sig=("@method" "@path")' +
        ";" +
        "created=" +
        createTime +
        ";" +
        "expires=" +
        expireTime +
        ";" +
        "keyid=" +
        '"' +
        Config.MERCHANT_ID +
        '"' +
        ";" +
        "alg=" +
        '"' +
        Config.ALG +
        '"';
      return result;
    case IConstants.PUT:
    case IConstants.POST:
      result =
        'sig=("@method" "@path" "content-digest" "content-type" "content-length")' +
        ";" +
        "created=" +
        createTime +
        ";" +
        "expires=" +
        expireTime +
        ";" +
        "keyid=" +
        '"' +
        Config.MERCHANT_ID +
        '"' +
        ";" +
        "alg=" +
        '"' +
        Config.ALG +
        '"';
      return result;
  }
}

function generateStringToSign(
  method,
  path,
  contentType,
  contentLength,
  contentDigest,
  createTime,
  expiresTime
) {
  let resutl = "";
  if (contentType != "" && contentLength != "" && contentDigest != "") {
    let signMethod = '"@method": ' + method;
    let signPath = '"@path": ' + path;
    let signContentDigest = '"content-digest": ' + contentDigest;
    let signContentType = '"content-type": ' + contentType;
    let signContentLength = '"content-length": ' + contentLength;
    let signParam =
      '"@signature-params": ("@method" "@path" "content-digest" "content-type" "content-length")' +
      ";" +
      "created=" +
      createTime +
      ";" +
      "expires=" +
      expiresTime +
      ";" +
      "keyid=" +
      '"' +
      Config.MERCHANT_ID +
      '"' +
      ";" +
      "alg=" +
      '"' +
      Config.ALG +
      '"';
    resutl =
      signMethod +
      "\n" +
      signPath +
      "\n" +
      signContentDigest +
      "\n" +
      signContentType +
      "\n" +
      signContentLength +
      "\n" +
      signParam;
  } else {
    let signMethod = '"@method": ' + method;
    let signPath = '"@path": ' + path;
    let signParam =
      '"@signature-params": ("@method" "@path")' +
      ";" +
      "created=" +
      createTime +
      ";" +
      "expires=" +
      expiresTime +
      ";" +
      "keyid=" +
      '"' +
      Config.MERCHANT_ID +
      '"' +
      ";" +
      "alg=" +
      '"' +
      Config.ALG +
      '"';
    resutl = signMethod + "\n" + signPath + "\n" + signParam;
  }
  return resutl;
}

function createHeaderRequest(
  signatureInput,
  signature,
  contentType,
  contentLength,
  contentDigest
) {
  let header;
  if (contentType != "" && contentLength != "" && contentDigest != "") {
    header = {
      "Signature-Input": signatureInput,
      Signature: "sig=:" + signature + ":",
      "Content-Type": contentType,
      "Content-Length": contentLength,
      "Content-Digest": contentDigest,
    };
  } else {
    header = {
      "Signature-Input": signatureInput,
      Signature: "sig=:" + signature + ":",
    };
  }
  return header;
}

module.exports = {
  makeGetRequest,
  makePutRequest,
  makePostRequest,
  sha256Hash,
  createSignatureInput,
  generateStringToSign,
  createHeaderRequest,
};
