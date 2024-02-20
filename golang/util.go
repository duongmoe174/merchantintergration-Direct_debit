package main

import (
	"bytes"
	"crypto/sha256"
	"fmt"
	"io/ioutil"
	"net/http"
)

func GenerateStringToSign(method string, path string, contentType string, contentLength string, contentDigest string, createTime string, expireTime string) string {

	sign := ""

	if contentLength != "" && contentDigest != "" && contentType != "" {
		signMethod := `"@method": ` + method
		signPath := `"@path": ` + path
		signContentDigest := `"content-digest": ` + contentDigest
		signContentType := `"content-type": ` + contentType
		signContentLength := `"content-length": ` + contentLength
		signParam := `"@signature-params": ("@method" "@path" "content-digest" "content-type" "content-length")` + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + `"` + AppConfig.MerchantId + `"` + ";" + "alg=" + `"` + AppConfig.ALG + `"`

		sign = signMethod + "\n" + signPath + "\n" + signContentDigest + "\n" + signContentType + "\n" + signContentLength + "\n" + signParam
	} else {
		signMethod := `"@method": ` + method
		signPath := `"@path": ` + path
		signParam := `"@signature-params": ("@method" "@path")` + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + `"` + AppConfig.MerchantId + `"` + ";" + "alg=" + `"` + AppConfig.ALG + `"`

		sign = signMethod + "\n" + signPath + "\n" + signParam
	}

	return sign
}

func GenerateSignatureInput(method string, createTime string, expireTime string) string {

	switch method {
	case "GET":
		return `sig=("@method" "@path")` + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + `"` + AppConfig.MerchantId + `"` + ";" + "alg=" + `"` + AppConfig.ALG + `"`
	case "PUT":
		return `sig=("@method" "@path" "content-digest" "content-type" "content-length")` + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + `"` + AppConfig.MerchantId + `"` + ";" + "alg=" + `"` + AppConfig.ALG + `"`
	case "POST":
		return `sig=("@method" "@path" "content-digest" "content-type" "content-length")` + ";" + "created=" + createTime + ";" + "expires=" + expireTime + ";" + "keyid=" + `"` + AppConfig.MerchantId + `"` + ";" + "alg=" + `"` + AppConfig.ALG + `"`
	default:
		return ""
	}
}

func CallGetRequest(url string, headers map[string]string) {
	fmt.Println("url: ", url)
	request, err := http.NewRequest("GET", url, nil)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	for key, value := range headers {
		request.Header.Set(key, value)
	}

	client := &http.Client{}

	response, err := client.Do(request)
	if err != nil {
		fmt.Println("Error sending request:", err)
		return
	}

	defer response.Body.Close()

	fmt.Println("Response Status:", response.Status)
	body, err := ioutil.ReadAll(response.Body)
	if err != nil {
		fmt.Println("error read response", err)
		return
	}
	fmt.Println(string(body))
}

func CallPutRequest(url string, content string, headers map[string]string) {
	fmt.Println("url: ", url)
	payload := []byte(content)
	request, err := http.NewRequest("PUT", url, bytes.NewBuffer(payload))
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	for key, value := range headers {
		request.Header.Set(key, value)
	}

	client := &http.Client{}

	response, err := client.Do(request)
	if err != nil {
		fmt.Println("Error sending request:", err)
		return
	}

	defer response.Body.Close()

	fmt.Println("Response Status:", response.Status)
	body, err := ioutil.ReadAll(response.Body)
	if err != nil {
		fmt.Println("error read response:", err)
		return
	}
	fmt.Println(string(body))
}

func CallPostRequest(url string, content string, headers map[string]string) {
	fmt.Println("url: ", url)
	payload := []byte(content)
	request, err := http.NewRequest("POST", url, bytes.NewBuffer(payload))
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	for key, value := range headers {
		request.Header.Set(key, value)
	}

	client := &http.Client{}

	response, err := client.Do(request)
	if err != nil {
		fmt.Println("Error sending request:", err)
		return
	}

	defer response.Body.Close()

	fmt.Println("Response Status:", response.Status)
	body, err := ioutil.ReadAll(response.Body)
	if err != nil {
		fmt.Println("error read response:", err)
		return
	}
	fmt.Println(string(body))
}

func sha256Hash(data []byte) []byte {
	hash := sha256.New()
	hash.Write(data)
	return hash.Sum(nil)
}
