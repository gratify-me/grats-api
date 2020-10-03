using Iso20022.Pain.V3;

namespace Iso20022.Pain
{
    public static class CashAccount
    {
        public static CashAccount16 FromAccountNumber(string accountNumber)
        {
            var schemeName = new AccountSchemeName1Choice();
            schemeName.Cd = "BBAN"; // Basic Bank Account Number (As defined in ExternalCodeSets_2Q2020_August2020_v1)

            var identification = new GenericAccountIdentification1();
            identification.SchmeNm = schemeName;
            identification.Id = accountNumber;

            var choice = new AccountIdentification4Choice();
            choice.Othr = identification;

            var account = new CashAccount16();
            account.Id = choice;

            return account;
        }
    }
}