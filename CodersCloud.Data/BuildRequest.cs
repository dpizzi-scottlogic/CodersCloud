using System;
using System.IO;

namespace CodersCloud.Data
{
    public class BuildRequest
    {
        public Uri BlobUri { get; set; }

        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }

        public string BlobNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BlobName);
            }
        }

        public Guid JobId { get; set; }
        public Uri Url { get; set; }
    }
}
