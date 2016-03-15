using System;

namespace CodersCloud.Data
{
    public class BuildResponse
    {
        public Uri BlobUri { get; set; }

        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }

        public Guid JobId { get; set; }
    }
}
