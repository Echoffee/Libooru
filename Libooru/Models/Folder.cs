﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Models
{
    public class Folder
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public int FileCount { get; set; }

		public bool ScanAtStart { get; set; }

		public long Size { get; set; }
    }
}
