﻿using System;
using System.Runtime.Serialization;

namespace SimpleTasks.Core.Models
{
    [DataContract()]
    public class BackupData
    {
        [DataMember(Name = "Version", Order = 0)]
        public string Version { get; set; }

        [DataMember(Name = "Created", Order = 1)]
        public DateTime Created { get; set; }

        [DataMember(Name = "Settings", Order = 2)]
        public Settings Settings { get; set; }

        [DataMember(Name = "Tasks", Order = 3)]
        public TaskCollection Tasks { get; set; }
    }
}
