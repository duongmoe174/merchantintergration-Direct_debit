import Api
from Config import Config


def main():
    token_registration()


def token_registration():
    merchant_id = Config.MERCHANT_ID
    Api.token_registration(merchant_id)


def retrieve_registered_token_information():
    merchant_id = Config.MERCHANT_ID
    merchant_token_ref = "DUONGTTTOKEN_1709257964"
    Api.retrieve_registered_token_information(merchant_id, merchant_token_ref)


def payment_with_registered_token():
    merchant_id = Config.MERCHANT_ID
    merchant_token = "TKN-Uij4KcR3Sm-2EARy6loIJQ"
    Api.payment_with_registered_token(merchant_id, merchant_token)


def retrieve_payment_information():
    merchant_id = Config.MERCHANT_ID
    merchant_txn_ref = "PAYMENTDTT_1709259446"
    Api.retrieve_payment_information(merchant_id, merchant_txn_ref)


def delete_token():
    merchant_id = Config.MERCHANT_ID
    merchant_token = "TKN-Uij4KcR3Sm-2EARy6loIJQ"
    Api.delete_token(merchant_id, merchant_token)


def retrieve_token_deletion_info():
    merchant_id = Config.MERCHANT_ID
    merchant_token = "TKN-Uij4KcR3Sm-2EARy6loIJQ"
    merchant_del_ref = "DELETETOKEN_1709260852"
    Api.retrieve_token_deletion_info(merchant_id, merchant_token, merchant_del_ref)


main()
