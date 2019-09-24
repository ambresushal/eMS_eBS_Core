using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class ResponseResult
    {
        public List<string> Errors { get; set; }
        public object Result { get; set; }

        public ResponseResult(object result, List<string> errors)
        {
            this.Errors = errors;
            this.Result = result;
        }
    }
}