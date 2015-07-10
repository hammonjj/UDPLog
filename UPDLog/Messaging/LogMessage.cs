using System;
using System.Linq;
using System.Text.RegularExpressions;
using UPDLog.DataStructures;

namespace UPDLog.Messaging
{
    public class LogMessage
    {
        //Log Message Fields
        public string Received { get; set; } //DateTime
        public string Severity { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; } //IPAddress:Port
        public string Pid { get; set; }
        public string Process { get; set; }
        public string File { get; set; } //File(Line)
        public string Thread { get; set; }
        public string Project { get; set; }
        public string TiError { get; set; }
        public string SocketId { get; set; }
        public string Message { get; set; }

        //Regex Definitions

        public LogMessage()
        {
            //This empty log message is used for the insertion of a marker in the log
        }

        public LogMessage(RawMessage rawMessage)
        {
            IpAddress = rawMessage.IpAddress + ":" + rawMessage.Port;
            var headerRegex = new Regex(@"^(.*?) - \[");
            var headerMatch = headerRegex.Match(rawMessage.Message);
            if(headerMatch.Success)
            {
                var header = headerMatch.Value;
                ParseHeaderData(header.Remove(header.Length - 4));
            }

            var metaDataRegex = new Regex(@"\[(.*?)\]");
            var metaDataMatch = metaDataRegex.Match(rawMessage.Message);
            if(metaDataMatch.Success)
            {
                var metaData = metaDataMatch.Value.Remove(0, 1);
                metaData = metaData.Remove(metaData.Count() - 1);
                ParseMetaData(metaData);
            }

            var messageRegex = new Regex(@"] *([^\n\r]*)");
            var messageMatch = messageRegex.Match(rawMessage.Message);
            if(messageMatch.Success)
            {
                Message = messageMatch.Value.Remove(0, 2);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                Received,
                Severity,
                HostName,
                IpAddress,
                Pid,
                Process,
                File,
                Thread,
                Project,
                SocketId,
                Message);
        }

        private void ParseHeaderData(string header)
        {
            var splitHeader = header.Split(' ');
            try
            {
                Severity = ParseSeverity(splitHeader[0]);
                Received = Convert.ToDateTime(splitHeader[1]).ToString();
                HostName = splitHeader[2];
                Process = splitHeader[3];
                Pid = splitHeader[4];
                TiError = splitHeader[5];
            }
            catch (IndexOutOfRangeException)
            {
                //Swallow the exception.  We take the lazy way out and just check for all of the normal log
                //message fields instead of finding out how big the array is an dealing with it
            }
            
        }

        private static string ParseSeverity(string rawSeverity)
        {
            var severityRegex = new Regex("<(.*?)>");
            var severityMatch = severityRegex.Match(rawSeverity);

            string severity;
            switch (severityMatch.Value)
            {
                case "<11>":
                    severity = "Error";
                    break;
                case "<12>":
                    severity = "Warning";
                    break;
                case "<13>":
                    severity = "Notice";
                    break;
                case "<14>":
                    severity = "Info";
                    break;
                case "<15>":
                    severity = "Debug";
                    break;
                default:
                    severity = "Unknown: " + rawSeverity;
                    break;
            }

            return severity;
        }

        private void ParseMetaData(string metaData)
        {
            var fileRegex = new Regex("file=\"(.*?)\"");
            var fileMatch = fileRegex.Match(metaData);
            var lineRegex = new Regex("line=\"(.*?)\"");
            var lineMatch = lineRegex.Match(metaData);
            if (fileMatch.Success && lineMatch.Success)
            {
                File = fileMatch.Groups[1].Value + "(" + lineMatch.Groups[1].Value + ")";
            }

            var threadRegex = new Regex("thread=\"(.*?)\"");
            var threadMatch = threadRegex.Match(metaData);
            if (threadMatch.Success)
            {
                Thread = threadMatch.Groups[1].Value;
            }

            var projectRegex = new Regex("project=\"(.*?)\"");
            var projectMatch = projectRegex.Match(metaData);
            if (projectMatch.Success)
            {
                Project = projectMatch.Groups[1].Value;
            }

            var socketIdRegex = new Regex("SocketId=\"(.*?)\"");
            var socketIdMatch = socketIdRegex.Match(metaData);
            if (socketIdMatch.Success)
            {
                SocketId = socketIdMatch.Groups[1].Value;
            }
        }
    }
}
