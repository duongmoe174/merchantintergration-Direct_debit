package main

import (
	"crypto/ed25519"
	"crypto/x509"
	"encoding/base64"
	"encoding/pem"
)

var testKeyEd25519 = `
-----BEGIN PRIVATE KEY-----
MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF
-----END PRIVATE KEY-----
`

var testKeyEd25519Pub = `
-----BEGIN PUBLIC KEY-----
MCowBQYDK2VwAyEAJrQLj5P/89iXES9+vFgrIy29clF9CC/oPPsw3c5D0bs=
-----END PUBLIC KEY-----
`

func Signature(signingString string) (str string, err error) {
	blockPrivate, _ := pem.Decode([]byte(testKeyEd25519))
	pki, _ := x509.ParsePKCS8PrivateKey(blockPrivate.Bytes)
	pk := pki.(ed25519.PrivateKey)
	data := ed25519.Sign(pk, []byte(signingString))
	return base64.StdEncoding.EncodeToString(data), nil
}
