using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Iso20022.Pain.V3;

namespace Iso20022.Pain
{
    public class TransferInitiation
    {
        public Guid Id { get; }
        public DebitorInformation Debitor { get; }

        public Document Document { get; }

        public TransferInitiation(DebitorInformation debitor, CreditTransaction[] transactions)
        {
            Id = Guid.NewGuid();
            Debitor = debitor;
            Document = CreditTransferInitiationDocument(
                header: GroupHeader(
                    messageId: Id,
                    createdAt: DateTime.Now,
                    identification: debitor.PartyIdentification()),
                paymentInstructions: new PaymentInstructionInformation3[]
                {
                    PaymentInstruction(
                        paymentId: Guid.NewGuid(),
                        paymentMethod: PaymentMethod3Code.TRF,
                        requestedExecutedAt: DateTime.Now,
                        debitor: debitor.PartyIdentification(),
                        debitorAccount: CashAccount.FromAccountNumber(debitor.AccountNumber),
                        debitorAgent: BranchAndFinancialInstitutionIdentification(debitor.BankBusinessRegistryCode),
                        transactions: transactions.Select(t => t.Transaction).ToArray()),
                });
        }

        public decimal TotalAmount() =>
            Document.CstmrCdtTrfInitn.PmtInf
                .SelectMany(instruction => instruction.CdtTrfTxInf)
                .Sum(transaction => transaction.Amt.InstdAmt.Value);

        public string TempFileName(bool asTest = false) => FileName(asDatFile: false, asTest);

        public string FileName(bool asTest = false) => FileName(asDatFile: true, asTest);

        public string FileName(bool asDatFile, bool asTest = false) =>
            $"{(asTest ? "T." : "P.")}00{Debitor.OrgNr}.{Debitor.BankFileTransferId}.P001.{Id.ToString()}.{Debitor.CustomerFileTransferId}.{(asDatFile ? "DAT" : "xml")}";

        public void WriteToStream(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Document));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n"
            };

            using(var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, Document);
                stream.Position = 0;
            }
        }

        private Document CreditTransferInitiationDocument(
            GroupHeader32 header,
            PaymentInstructionInformation3[] paymentInstructions)
        {
            var transferInitiation = new CustomerCreditTransferInitiationV03();
            transferInitiation.GrpHdr = header;
            transferInitiation.GrpHdr.NbOfTxs = paymentInstructions.Length.ToString();

            foreach (var instruction in paymentInstructions)
            {
                transferInitiation.PmtInf.Add(instruction);
            }

            var document = new Document();
            document.CstmrCdtTrfInitn = transferInitiation;

            return document;
        }

        private GroupHeader32 GroupHeader(
            Guid messageId,
            DateTime createdAt,
            PartyIdentification32 identification)
        {
            var groupHeader = new GroupHeader32();
            groupHeader.MsgId = messageId.ToString("N");
            groupHeader.CreDtTm = createdAt;
            groupHeader.InitgPty = identification;

            return groupHeader;
        }

        private PaymentInstructionInformation3 PaymentInstruction(
            Guid paymentId,
            PaymentMethod3Code paymentMethod,
            DateTime requestedExecutedAt,
            PartyIdentification32 debitor,
            CashAccount16 debitorAccount,
            BranchAndFinancialInstitutionIdentification4 debitorAgent,
            CreditTransferTransactionInformation10[] transactions)
        {
            var paymentInstruction = new PaymentInstructionInformation3();
            paymentInstruction.PmtInfId = paymentId.ToString("N");
            paymentInstruction.PmtMtd = PaymentMethod3Code.TRF;
            paymentInstruction.ReqdExctnDt = requestedExecutedAt;

            paymentInstruction.Dbtr = debitor;
            paymentInstruction.DbtrAcct = debitorAccount;
            paymentInstruction.DbtrAgt = debitorAgent;

            foreach (var transaction in transactions)
            {
                paymentInstruction.CdtTrfTxInf.Add(transaction);
            }

            return paymentInstruction;
        }

        private BranchAndFinancialInstitutionIdentification4 BranchAndFinancialInstitutionIdentification(string businessRegistryCode)
        {
            var institutionId = new FinancialInstitutionIdentification7();
            institutionId.BIC = businessRegistryCode;

            var identification = new BranchAndFinancialInstitutionIdentification4();
            identification.FinInstnId = institutionId;

            return identification;
        }
    }
}