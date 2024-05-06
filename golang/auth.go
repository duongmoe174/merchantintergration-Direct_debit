package main

import (
	"crypto/ed25519"
	"encoding/base64"
	"fmt"
)

var finalKey = `yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg==`

func Signature(signingString string) (str string, err error) {
	keyBytes, err := base64.StdEncoding.DecodeString(finalKey)
	if err != nil {
		fmt.Println("Error decoding base64:", err)
		return
	}
	privateKey := ed25519.PrivateKey(keyBytes)
	signature := ed25519.Sign(privateKey, []byte(signingString))
	return base64.StdEncoding.EncodeToString(signature), nil
}
