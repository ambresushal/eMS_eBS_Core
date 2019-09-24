using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace tmg.equinox.services.webapi.Framework
{
    internal class ErrorResult : IHttpActionResult
    {
        #region Private Members
        private HttpRequestMessage _request;
        private HttpResponseMessage _httpResponseMessage;
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ErrorResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
        {
            _request = request;
            _httpResponseMessage = httpResponseMessage;
        }
        #endregion Constructor

        #region Public Methods
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_httpResponseMessage);
        }
        #endregion Public Methods
    }
}