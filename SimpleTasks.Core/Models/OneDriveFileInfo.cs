using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    public class OneDriveFileInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FolderId { get; set; }

        public DateTime Created { get; set; }
    }
}
