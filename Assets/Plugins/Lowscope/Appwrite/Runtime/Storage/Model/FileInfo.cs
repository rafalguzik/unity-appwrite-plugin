using System;

namespace Lowscope.AppwritePlugin.Storage.Model
{
	public class FileInfo
	{
		public string Id { get; set; }
		public string BucketId { get; set; }
		public string CreatedAt { get; set; }
		public string UpdatedAt { get; set; }
		public string[] Permissions { get; set; }
		public string Name { get; set; }
		public string Signature { get; set; }
		public string MimeType { get; set; }
		public ulong SizeTotal { get; set; }
		public ulong ChunksTotal { get; set; }
		public ulong ChunksUploaded { get; set; }
	}
}

