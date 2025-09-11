using System.Collections.Concurrent;

namespace LilCloudServerConsole.Classes
{
    public class ClientRegistry
    {
        public BlockingCollection<FileData> FileQueue = new BlockingCollection<FileData>();
    }
}
