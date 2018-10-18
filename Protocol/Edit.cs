using System.Collections.Generic;
using OT.Entities;

namespace Protocol
{
    public class Edit
    {
        public List<Diff> Diffs { get; }
        public int ClientVersion { get; }
        public int ServerVersion { get; }

        public Edit(List<Diff> diffs, int clientVersion, int serverVersion)
        {
            Diffs = diffs;
            ClientVersion = clientVersion;
            ServerVersion = serverVersion;
        }
    }
}