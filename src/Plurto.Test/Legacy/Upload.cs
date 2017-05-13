using System;
using System.IO;
using System.Threading;
using Plurto.Commands;
using Plurto.Core;
using Xunit;

namespace Plurto.Test.Legacy
{
    public class Upload
    {
        [Fact]
        public void OAuthUpload()
        {
            const string filename = "testimage.png";
            const string differentUnicodeFilename = "あいうえお- _.png";

            using (var file = File.OpenRead(filename))
            using (var uploadFile = new UploadFile(file, differentUnicodeFilename))
            {
                var command = TimelineCommand.UploadPicture(uploadFile).Client(TestConfig.OAuthClient);
                var manualResetEvent = new ManualResetEvent(false);
                command.ToObservable().Subscribe(pic => pic.WriteDump(),
                                                 () => manualResetEvent.Set());

                var signal = manualResetEvent.WaitOne(TimeSpan.FromSeconds(20));
                Assert.True(signal);
            }
        }
    }
}
