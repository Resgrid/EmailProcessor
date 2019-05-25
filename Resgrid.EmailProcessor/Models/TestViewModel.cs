using System;
using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Models
{
	public class TestViewModel
	{
		public bool CanLoadConfig { get; set; }
		public bool DirectoryAvailable { get; set; }
		public bool CanCreateFile { get; set; }
		public bool CanReadFile { get; set; }
		public bool CanDeleteFile { get; set; }
		public List<Tuple<int, bool>> PortsAvailable { get; set; }
	}
}
