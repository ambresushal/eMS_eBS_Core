using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Account
{
    public class UserActivityProfile
    {
        public DateTime TimeUtc { get; set; }

        public string UserName { get; set; }

        public Event Event { get; set; }

        public string Host { get; set; }

        public int TenantID { get; set; }

        public Category Category { get; set; }

        public string RequestUrl { get; set; }

        public string AppDomain { get; set; }

        public string UserAgent { get; set; }

        public Severity Severity { get; set; }

        public string Message { get; set; }

        public  Priority Priority { get; set; }

    }

    public enum Priority:int
    {
        High = 2,
        Medium = 1,
        Low = 0
    }

    public enum Event
    {
        LogOn,
        LogOff,
        CustomEvent,
		Caching,
        AppStart
    }

    public enum Category
    {
        Error = 1,
        Audit = 2
    }

    public enum Severity
    {
        Emergency, 
        Alert, 
        Critical, 
        Error, 
        Warning, 
        Notice, 
        Information, 
        Debug   
    }

}
