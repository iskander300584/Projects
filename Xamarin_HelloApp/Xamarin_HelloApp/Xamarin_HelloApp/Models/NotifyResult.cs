using System.Collections.Generic;


namespace PilotMobile.Models
{
    public class NotifyResult
    {
        private List<string> result = new List<string>();
        public List<string> Result
        {
            get => result;
            set
            {
                result = value;
            }
        }
    }
}