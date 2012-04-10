using System;

namespace ChronoPlurk.Services
{
    public sealed class SharePickerService
    {
        public string FileId { get; private set; }

        public bool IsFromSharePicker { get { return FileId != null; } }

        public bool IsActionProcessed { get; private set; }

        public bool ProcessAction
        {
            get { return IsFromSharePicker && !IsActionProcessed; }
        }

        public void SetFileId(string fileId)
        {
            FileId = fileId;
        }

        public void SetActionProcessed(bool processed)
        {
            IsActionProcessed = processed;
        }
    }
}
