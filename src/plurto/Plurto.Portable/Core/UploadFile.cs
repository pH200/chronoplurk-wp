using System;
using System.IO;

namespace Plurto.Core
{
    /// <summary>
    /// Container for multipart post uploading. IDisposable disposes internal stream.
    /// </summary>
    public class UploadFile : IDisposable
    {
        public const string StreamContentType = "application/octet-stream";

        public Stream Stream { get; private set; }

        public string FileName { get; set; }

        /// <summary>
        /// Field name for multipart post. Changing this property would affect API request.
        /// </summary>
        public string FieldName { get; set; }

        public string ContentType { get; set; }

        public UploadFile(Stream stream, string fileName)
            : this(stream, fileName, StreamContentType)
        {
        }

        public UploadFile(Stream stream, string fileName, string contentType)
        {
            Stream = stream;
            FileName = fileName;
            ContentType = contentType;
        }

        public UploadFile(byte[] bytes, string fileName)
            : this(new MemoryStream(bytes), fileName)
        {
        }

        public UploadFile(byte[] bytes, string fileName, string contentType)
            : this(new MemoryStream(bytes), fileName, contentType)
        {
        }
        

        /// <summary>
        /// Dispose Stream member.
        /// </summary>
        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
            }
        }
    }
}
