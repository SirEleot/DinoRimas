using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace DinoRimas.FileWatcher
{
    public class DinoWatcherData
    {
        public DinoWatcherData(FileSystemEventArgs e, int serverid)
        {
            ServerId = serverid;
            Steamid = e.Name[0..^5];
            Path = e.FullPath;
            Type = e.ChangeType;
        }
        public string Steamid { get; set; }
        public string Path { get; set; }
        public int ServerId { get; set; }
        public WatcherChangeTypes Type { get; set; }
    }
}
