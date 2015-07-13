namespace UPDLog.DataStructures
{
    public class LogFilterRule
    {
        public bool Acceptance { get; set; }
        public string Column { get; set; }
        public string Action { get; set; }
        public string Content { get; set; }
    }
}
