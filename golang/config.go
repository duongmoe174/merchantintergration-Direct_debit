package main

import (
	"fmt"

	"github.com/spf13/viper"
)

type Config struct {
	MerchantId string `mapstructure:"merchant_id"`
	BaseUrl    string `mapstructure:"base_url"`
	UrlPrefix  string `mapstructure:"url_prefix"`
	ALG        string `mapstructure:"alg"`
}

type Param struct {
	DD_TOKENS                 string `mapstructure:"DD_TOKENS"`
	PAYMENTS_DDT              string `mapstructure:"PAYMENTS_DDT"`
	TOKENIZE_DIRECT_DEBIT     string `mapstructure:"TOKENIZE_DIRECT_DEBIT"`
	DIRECT_DEBIT_TOKEN        string `mapstructure:"DIRECT_DEBIT_TOKEN"`
	PURCHASE_DIRECT_DEBIT     string `mapstructure:"PURCHASE_DIRECT_DEBIT"`
	APPLICATION_JSON          string `mapstructure:"APPLICATION_JSON"`
	CONTENT_TYPE              string `mapstructure:"CONTENT_TYPE"`
	CONTENT_LENGTH            string `mapstructure:"CONTENT_LENGTH"`
	CONTENT_DIGEST            string `mapstructure:"CONTENT_DIGEST"`
	SIGNATURE_INPUT           string `mapstructure:"SIGNATURE_INPUT"`
	SIGNATURE                 string `mapstructure:"SIGNATURE"`
	MERCHANTS                 string `mapstructure:"MERCHANTS"`
	DELETE_TOKEN_DIRECT_DEBIT string `mapstructure:"DELETE_TOKEN_DIRECT_DEBIT"`
	DELETIONS                 string `mapstructure:"DELETIONS"`
}

var AppConfig Config
var AppParam Param

func LoadConfig() {
	viper.SetConfigName("config")
	viper.AddConfigPath(".")
	viper.SetConfigType("yaml")

	if err := viper.ReadInConfig(); err != nil {
		panic(err)
	}

	// Unmarshal the AppConfig

	if err := viper.UnmarshalKey("Config", &AppConfig); err != nil {
		fmt.Printf("Error unmarshaling config: %s\n", err)
		return
	}

	// Unmarshal the AppParam

	if err := viper.UnmarshalKey("Param", &AppParam); err != nil {
		fmt.Printf("Error unmarshaling param: %s\n", err)
		return
	}
}
