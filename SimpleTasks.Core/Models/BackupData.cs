using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [DataContract()]
    public class BackupData
    {
        public BackupData()
        {
            Info = new BackupDataInfo();
        }

        [DataMember(Name = "Info", Order = 1)]
        public BackupDataInfo Info { get; set; }

        [DataContract()]
        public class BackupDataInfo
        {
            [DataMember(Name = "Version")]
            public string Version { get; set; }

            [DataMember(Name = "UtcDateTime")]
            public DateTime UtcDateTime { get; set; }
        }

        [DataMember(Name = "Settings", Order = 2)]
        public Settings Settings { get; set; }

        [DataMember(Name = "Tasks", Order = 3)]
        public TaskCollection Tasks { get; set; }
    }
}
