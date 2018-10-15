namespace Client.Entities
{
    public class Document
    {
        public string CurrentText { get; set; }
        public ShadowCopy ShadowCopy { get; } = new ShadowCopy();
    }

    public class ShadowCopy
    {
        public string ShadowText { get; set; }
        public int ClientVersion { get; set; }
        public int ServerVersion { get; set; }
    }
}