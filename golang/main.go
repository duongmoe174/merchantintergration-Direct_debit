package main

func main() {
	LoadConfig()

	tokenRegistration2()
}

func tokenRegistration2() {
	merchantId := AppConfig.MerchantId
	TokenRegistration(merchantId)
}

func tokenRegistration() {
	merchantId := AppConfig.MerchantId
	TokenRegistration(merchantId)
}

func retrieveRegisteredTokenInformation() {
	merchantId := AppConfig.MerchantId
	merchantTokenRef := "DUONGTTTOKEN_1708059862124580000"
	RetrieveRegisteredTokenInformation(merchantId, merchantTokenRef)
}

func paymentWithRegisteredToken() {
	merchanId := AppConfig.MerchantId
	merchantToken := "TKN-3EIixH6SSNOXOhWRq_3mgA"
	PaymentWithRegisteredToken(merchanId, merchantToken)
}

func retrievePaymentInformation() {
	merchantId := AppConfig.MerchantId
	merchantTxnRef := "PAYMENTDDT_1708068183697556600"
	RetrievePaymentInformation(merchantId, merchantTxnRef)
}

func deleteToken() {
	merchantId := AppConfig.MerchantId
	merchantToken := "TKN-3EIixH6SSNOXOhWRq_3mgA"
	DeleteToken(merchantId, merchantToken)
}

func retrieveTokenDeletionInfo() {
	merchantId := AppConfig.MerchantId
	merchantToken := "TKN-3EIixH6SSNOXOhWRq_3mgA"
	merchantDefRef := "DELETETOKEN_1708072553988150700"
	RetrieveTokenDeletionInfo(merchantId, merchantToken, merchantDefRef)
}
