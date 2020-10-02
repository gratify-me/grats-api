using Iso20022.Pain.V3;

namespace Iso20022.Pain
{
    public class DebitorInformation
    {
        public string Name { get; set; }

        public int OrgNr { get; set; }

        public long AccountNumber { get; set; }

        public string BankBusinessRegistryCode { get; set; }

        public int BankFileTransferId { get; set; } // Four-digit number identifying the bank during file transfer.

        public string CustomerFileTransferId { get; set; } // Customer specific identifier during file transfer.

        public PartyIdentification32 PartyIdentification()
        {
            var organisationId = new OrganisationIdentification4();
            organisationId.Othr.Add(new GenericOrganisationIdentification1
            {
                Id = OrgNr.ToString(),
            });

            var partyId = new PartyIdentification32();
            partyId.Nm = Name;
            partyId.Id = new Party6Choice
            {
                OrgId = organisationId,
            };

            return partyId;
        }
    }
}