namespace Server.Entities
{
    public class Document
    {
        public string CurrentText { get; set; } = "";
        public ShadowCopy ShadowCopy { get; } = new ShadowCopy();
        public BackupShadowCopy BackupShadowCopy { get; } = new BackupShadowCopy();
    }

    public class ShadowCopy
    {
        public string ShadowText { get; set; } = "";
        public int ClientVersion { get; set; } = 0;
        public int ServerVersion { get; set; } = 0;
    }

    public class BackupShadowCopy
    {
        public string BackupText { get; set; } = "";
        public int ServerVersion { get; set; } = 0;
    }
}