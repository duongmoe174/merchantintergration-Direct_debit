package main

import (
	"encoding/base64"
	"encoding/json"
	"fmt"
	"strconv"
	"time"
)

type TOKEN_REG_BODY struct {
	ApiOperation   string `json:"apiOperation"`
	BrowserPayment struct {
		ReturnURL   string `json:"returnUrl"`
		CallBackURL string `json:"callbackUrl"`
	} `json:"browserPayment"`
	Customer struct {
		Account struct {
			ID string `json:"id"`
		} `json:"account"`
		Email string `json:"email"`
		Name  string `json:"name"`
		Phone string `json:"phone"`
	} `json:"customer"`
	SourceOfFunds struct {
		Types []string `json:"types"`
	} `json:"sourceOfFunds"`
	Device struct {
		ID               string `json:"id"`
		IpAddress        string `json:"ipAddress"`
		Browser          string `json:"browser"`
		MobilePhoneModel string `json:"mobilePhoneModel"`
		Fingerprint      string `json:"fingerprint"`
	} `json:"device"`
	Locale string `json:"locale"`
}

type PAYMENT_TOKEN_BODY struct {
	ApiOperation string `json:"apiOperation"`
	Invoice      struct {
		Amount      int    `json:"amount"`
		Currency    string `json:"currency"`
		Description string `json:"description"`
	} `json:"invoice"`
	Customer struct {
		Account struct {
			ID string `json:"id"`
		} `json:"account"`
		Email string `json:"email"`
		Name  string `json:"name"`
		Phone string `json:"phone"`
	} `json:"customer"`
	Transaction struct {
		SourceOfFunds struct {
			Type  string `json:"type"`
			Token string `json:"token"`
		} `json:"sourceOfFunds"`
		Source string `json:"source"`
	} `json:"transaction"`
	Device struct {
		ID               string `json:"id"`
		IpAddress        string `json:"ipAddress"`
		Browser          string `json:"browser"`
		MobilePhoneModel string `json:"mobilePhoneModel"`
		Fingerprint      string `json:"fingerprint"`
	} `json:"device"`
}

type DELETE_TOKEN_BODY struct {
	ApiOperation   string `json:"apiOperation"`
	BrowserPayment struct {
		ReturnURL   string `json:"returnUrl"`
		CallBackURL string `json:"callbackUrl"`
	} `json:"browserPayment"`
	Customer struct {
		Account struct {
			ID string `json:"id"`
		} `json:"account"`
		Email string `json:"email"`
		Name  string `json:"name"`
		Phone string `json:"phone"`
	} `json:"customer"`
	Device struct {
		IpAddress        string `json:"ipAddress"`
		Browser          string `json:"browser"`
		MobilePhoneModel string `json:"mobilePhoneModel"`
	} `json:"device"`
	Locale string `json:"locale"`
}

func TokenRegistration(merchantId string) {
	method := "PUT"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	currentTimeMilisecond := time.Now().UnixNano()
	reference := "DUONGTTTOKEN_" + fmt.Sprintf("%d", currentTimeMilisecond)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.DD_TOKENS + "/" + reference

	bodyRequest := TOKEN_REG_BODY{
		ApiOperation: AppParam.TOKENIZE_DIRECT_DEBIT,
		BrowserPayment: struct {
			ReturnURL   string "json:\"returnUrl\""
			CallBackURL string "json:\"callbackUrl\""
		}{ReturnURL: "https://mtf.onepay.vn/ldp/direct-debit/result", CallBackURL: "https://mtf.onepay.vn/paygate/api/rest/v1/ipn"},
		Customer: struct {
			Account struct {
				ID string "json:\"id\""
			} "json:\"account\""
			Email string "json:\"email\""
			Name  string "json:\"name\""
			Phone string "json:\"phone\""
		}{Account: struct {
			ID string "json:\"id\""
		}{ID: "000000001"}, Email: "duongtt@onepay.vn", Name: "TRAN THAI DUONG", Phone: "0367573933"},
		SourceOfFunds: struct {
			Types []string "json:\"types\""
		}{Types: []string{"DD_SGTTVNVX", "DD_BIDVVNVX"}},
		Device: struct {
			ID               string "json:\"id\""
			IpAddress        string "json:\"ipAddress\""
			Browser          string "json:\"browser\""
			MobilePhoneModel string "json:\"mobilePhoneModel\""
			Fingerprint      string "json:\"fingerprint\""
		}{ID: "DV213124124", IpAddress: "192.168.1.999", Browser: "Chrome", MobilePhoneModel: "nokia 1280", Fingerprint: ""},
		Locale: "vi",
	}

	jsonData, err := json.Marshal(bodyRequest)
	if err != nil {
		fmt.Println("Lỗi khi chuyển đổi thành JSON:", err)
		return
	}

	hashPayload := sha256Hash([]byte(string(jsonData)))
	contentDigest := "sha-256=:" + base64.StdEncoding.EncodeToString(hashPayload) + ":"

	contentType := AppParam.APPLICATION_JSON

	jsonLength := len(jsonData)
	contentLength := strconv.Itoa(jsonLength)

	stringToSign := GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime)

	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	signatureInput := GenerateSignatureInput(method, createTime, expireTime)

	headerRequest := map[string]string{
		AppParam.CONTENT_TYPE:    contentType,
		AppParam.CONTENT_LENGTH:  contentLength,
		AppParam.CONTENT_DIGEST:  contentDigest,
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	urlRequest := AppConfig.BaseUrl + path

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("jsonDataRequest: ", string(jsonData))
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("Signature-Input: ", signatureInput)
	fmt.Println("signature: ", signature)

	CallPutRequest(urlRequest, string(jsonData), headerRequest)
}

func RetrieveRegisteredTokenInformation(merchantId string, merchantTokenRef string) {
	method := "GET"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.DD_TOKENS + "/" + merchantTokenRef
	stringToSign := GenerateStringToSign(method, path, "", "", "", createTime, expireTime)
	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}
	signatureInput := GenerateSignatureInput(method, createTime, expireTime)
	urlRequest := AppConfig.BaseUrl + path
	headerRequest := map[string]string{
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("signature: ", signature)

	CallGetRequest(urlRequest, headerRequest)
}

func PaymentWithRegisteredToken(merchantId string, merchantToken string) {
	method := "POST"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	currentTimeMilisecond := time.Now().UnixNano()
	reference := "PAYMENTDDT_" + fmt.Sprintf("%d", currentTimeMilisecond)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.PAYMENTS_DDT + "/" + reference

	bodyRequest := PAYMENT_TOKEN_BODY{
		ApiOperation: AppParam.PURCHASE_DIRECT_DEBIT,
		Invoice: struct {
			Amount      int    "json:\"amount\""
			Currency    string "json:\"currency\""
			Description string "json:\"description\""
		}{Amount: 10000000, Currency: "VND", Description: "DUONGTT THANH TOAN"},
		Customer: struct {
			Account struct {
				ID string "json:\"id\""
			} "json:\"account\""
			Email string "json:\"email\""
			Name  string "json:\"name\""
			Phone string "json:\"phone\""
		}{Account: struct {
			ID string "json:\"id\""
		}{ID: "000000001"}, Email: "duongtt@onepay.vn", Name: "TRAN THAI DUONG", Phone: "0367573933"},
		Transaction: struct {
			SourceOfFunds struct {
				Type  string "json:\"type\""
				Token string "json:\"token\""
			} "json:\"sourceOfFunds\""
			Source string "json:\"source\""
		}{Source: "MIT", SourceOfFunds: struct {
			Type  string "json:\"type\""
			Token string "json:\"token\""
		}{Type: AppParam.DIRECT_DEBIT_TOKEN, Token: merchantToken}},
		Device: struct {
			ID               string "json:\"id\""
			IpAddress        string "json:\"ipAddress\""
			Browser          string "json:\"browser\""
			MobilePhoneModel string "json:\"mobilePhoneModel\""
			Fingerprint      string "json:\"fingerprint\""
		}{ID: "DV213124124", IpAddress: "192.168.1.999", Browser: "Chrome", MobilePhoneModel: "nokia 1280", Fingerprint: ""},
	}
	jsonData, err := json.Marshal(bodyRequest)
	if err != nil {
		fmt.Println("Lỗi khi chuyển đổi thành JSON:", err)
		return
	}

	hashPayload := sha256Hash([]byte(string(jsonData)))
	contentDigest := "sha-256=:" + base64.StdEncoding.EncodeToString(hashPayload) + ":"

	contentType := AppParam.APPLICATION_JSON

	jsonLength := len(jsonData)
	contentLength := strconv.Itoa(jsonLength)

	stringToSign := GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime)

	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	signatureInput := GenerateSignatureInput(method, createTime, expireTime)

	headerRequest := map[string]string{
		AppParam.CONTENT_TYPE:    contentType,
		AppParam.CONTENT_LENGTH:  contentLength,
		AppParam.CONTENT_DIGEST:  contentDigest,
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	urlRequest := AppConfig.BaseUrl + path

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("jsonDataRequest: ", string(jsonData))
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("Signature-Input: ", signatureInput)
	fmt.Println("signature: ", signature)

	CallPostRequest(urlRequest, string(jsonData), headerRequest)
}

func RetrievePaymentInformation(merchantId string, merchantTxnRef string) {
	method := "GET"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.PAYMENTS_DDT + "/" + merchantTxnRef
	stringToSign := GenerateStringToSign(method, path, "", "", "", createTime, expireTime)
	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}
	signatureInput := GenerateSignatureInput(method, createTime, expireTime)
	urlRequest := AppConfig.BaseUrl + path
	headerRequest := map[string]string{
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("signature: ", signature)

	CallGetRequest(urlRequest, headerRequest)
}

func DeleteToken(merchantId string, merchantToken string) {
	method := "PUT"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	currentTimeMilisecond := time.Now().UnixNano()
	reference := "DELETETOKEN_" + fmt.Sprintf("%d", currentTimeMilisecond)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.DD_TOKENS + "/" + merchantToken + "/" + AppParam.DELETIONS + "/" + reference

	bodyRequest := DELETE_TOKEN_BODY{
		ApiOperation: AppParam.DELETE_TOKEN_DIRECT_DEBIT,
		BrowserPayment: struct {
			ReturnURL   string "json:\"returnUrl\""
			CallBackURL string "json:\"callbackUrl\""
		}{ReturnURL: "https://mtf.onepay.vn/ldp/direct-debit/result", CallBackURL: "https://mtf.onepay.vn/paygate/api/rest/v1/ipn"},
		Customer: struct {
			Account struct {
				ID string "json:\"id\""
			} "json:\"account\""
			Email string "json:\"email\""
			Name  string "json:\"name\""
			Phone string "json:\"phone\""
		}{Account: struct {
			ID string "json:\"id\""
		}{ID: "000000001"}, Email: "duongtt@onepay.vn", Name: "TRAN THAI DUONG", Phone: "0367573933"},
		Device: struct {
			IpAddress        string "json:\"ipAddress\""
			Browser          string "json:\"browser\""
			MobilePhoneModel string "json:\"mobilePhoneModel\""
		}{IpAddress: "192.168.1.999", Browser: "Chrome", MobilePhoneModel: "nokia 1280"},
		Locale: "vi",
	}
	jsonData, err := json.Marshal(bodyRequest)
	if err != nil {
		fmt.Println("Lỗi khi chuyển đổi thành JSON:", err)
		return
	}

	hashPayload := sha256Hash([]byte(string(jsonData)))
	contentDigest := "sha-256=:" + base64.StdEncoding.EncodeToString(hashPayload) + ":"

	contentType := AppParam.APPLICATION_JSON

	jsonLength := len(jsonData)
	contentLength := strconv.Itoa(jsonLength)

	stringToSign := GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime)

	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	signatureInput := GenerateSignatureInput(method, createTime, expireTime)

	headerRequest := map[string]string{
		AppParam.CONTENT_TYPE:    contentType,
		AppParam.CONTENT_LENGTH:  contentLength,
		AppParam.CONTENT_DIGEST:  contentDigest,
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	urlRequest := AppConfig.BaseUrl + path

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("jsonDataRequest: ", string(jsonData))
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("Signature-Input: ", signatureInput)
	fmt.Println("signature: ", signature)

	CallPutRequest(urlRequest, string(jsonData), headerRequest)
}

func RetrieveTokenDeletionInfo(merchantId string, merchantToken string, merchantDelRef string) {
	method := "GET"
	// Get the current time
	currentTime := time.Now()
	fiveMinutesLater := currentTime.Add(5 * time.Minute)
	// Convert the current time to Unix timestamp (in seconds)
	createTime := strconv.FormatInt(currentTime.Unix(), 10)
	expireTime := strconv.FormatInt(fiveMinutesLater.Unix(), 10)

	path := AppConfig.UrlPrefix + "merchants" + "/" + merchantId + "/" + AppParam.DD_TOKENS + "/" + merchantToken + "/" + AppParam.DELETIONS + "/" + merchantDelRef
	stringToSign := GenerateStringToSign(method, path, "", "", "", createTime, expireTime)
	signature, err := Signature(stringToSign)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}
	signatureInput := GenerateSignatureInput(method, createTime, expireTime)
	urlRequest := AppConfig.BaseUrl + path
	headerRequest := map[string]string{
		AppParam.SIGNATURE_INPUT: signatureInput,
		AppParam.SIGNATURE:       "sig=:" + signature + ":",
	}

	fmt.Println("urlRequest: ", urlRequest)
	fmt.Println("HeaderRequest: ", headerRequest)
	fmt.Println("StringToSign: ", stringToSign)
	fmt.Println("signature: ", signature)

	CallGetRequest(urlRequest, headerRequest)
}
