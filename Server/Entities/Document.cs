namespace Server.Entities
{
    public class Document
    {
        public string Current { get; set; }
        public string Shadow { get; set; }

        public string BackupShadow { get; set; }
    }
}