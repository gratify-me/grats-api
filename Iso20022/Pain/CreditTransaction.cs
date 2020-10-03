using System;
using Iso20022.Pain.V3;

namespace Iso20022.Pain
{
    public class CreditTransaction
    {
        public CreditTransferTransactionInformation10 Transaction { get; }

        public CreditTransaction(Guid correlationId, string creditorName, string creditorAccount, int amountNok)
        {
            Transaction = TransferTransaction(
                identification: PaymentIdentification(correlationId),
                amount: CurrencyAndAmount(amountNok, "NOK"),
                creditor: CreditorPartyIdentification(creditorName),
                creditorAccount: CashAccount.FromAccountNumber(creditorAccount));
        }

        private CreditTransferTransactionInformation10 TransferTransaction(
            PaymentIdentification1 identification,
            ActiveOrHistoricCurrencyAndAmount amount,
            PartyIdentification32 creditor,
            CashAccount16 creditorAccount)
        {
            var amountChoice = new AmountType3Choice();
            amountChoice.InstdAmt = amount;

            var transaction = new CreditTransferTransactionInformation10();
            transaction.PmtId = identification;
            transaction.Amt = amountChoice;
            transaction.Cdtr = creditor;
            transaction.CdtrAcct = creditorAccount;
            /*
             * Creditor agent is only required for cross border transactions.
             * transaction.CdtrAgt = creditorAgent;
             */
            return transaction;
        }

        private PaymentIdentification1 PaymentIdentification(Guid endToEndId)
        {
            var identification = new PaymentIdentification1();
            identification.EndToEndId = endToEndId.ToString("N");

            return identification;
        }

        private ActiveOrHistoricCurrencyAndAmount CurrencyAndAmount(decimal value, string currency)
        {
            return new ActiveOrHistoricCurrencyAndAmount
            {
                Value = value,
                Ccy = currency,
            };
        }

        private PartyIdentification32 CreditorPartyIdentification(string name)
        {
            var partyId = new PartyIdentification32();
            partyId.Nm = name;

            return partyId;
        }
    }
}