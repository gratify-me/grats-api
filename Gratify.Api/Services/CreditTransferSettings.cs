using Renci.SshNet;

namespace Gratify.Api.Services
{
    public class CreditTransferSettings
    {
        public string Username { get; set; }

        public string ToUserDirectory => $"/{Username}/touser";

        public string ToEvryDirectory => $"/{Username}/toevry";

        public bool UseTestFiles { get; set; } = false;

        public string KeyFilePath { get; set; }

        public string KeyFilePassword { get; set; }

        public PrivateKeyFile GetPrivateKeyFile() =>
            new PrivateKeyFile(
                fileName: KeyFilePath,
                passPhrase: KeyFilePassword);

        public string SftpHost { get; set; }

        public int SftpPort { get; set; } = 22;

        public ConnectionInfo GetConnectionInfo() =>
            new ConnectionInfo(
                host: SftpHost,
                port: SftpPort,
                username: Username,
                new PrivateKeyAuthenticationMethod(Username, GetPrivateKeyFile()));
    }
}