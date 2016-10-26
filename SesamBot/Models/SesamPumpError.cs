using System;

namespace SesamBot.Models
{
    public interface ISesamError
    {
        string Hash { get; set; }
        string Url { get; set; }
        string Entity { get; set; }
        string EventType { get; set; }
        DateTime End_time { get; set; }
        string Title { get; }
        string SubTitle { get; }
        string Text { get; }
        int Updated { get; set; }

    }

    [Serializable]
    public class SesamPumpError : ISesamError
    {
        public string Hash { get; set; }
        public string Url { get; set; }
        public int Updated { get; set; }
        public string Entity { get; set; }
        public DateTime End_time { get; set; }
        public string Reason_why_stopped { get; set; }
        public string Traceback { get; set; }
        public string Original_traceback { get; set; }
        public string Original_error_message { get; set; }
        public string EventType { get; set; }
        public string Text
        {
            get
            {
                return $"See full json: {Url}";
            }
        }

        public string Title
        {
            get
            {
                return $"{EventType}:{Entity}";
            }
        }

        public string SubTitle
        {
            get
            {
                string subTitle = Reason_why_stopped.Substring(0, Math.Min(Reason_why_stopped.Length, 300));
                return $"{End_time}:{subTitle}";
            }
        }

        
    }
}