using System;

namespace Resgrid.EmailProcessor.Models
{
	public class TestViewModel
	{
		public bool CanLoadConfig { get; set; }
		public bool DirectoryAvailable { get; set; }
		public bool CanCreateFile { get; set; }
		public bool CanRenameFile { get; set; }
		public bool CanDeleteFile { get; set; }
		public Tuple<int, bool> PortsAvailable { get; set; }
	}
}
