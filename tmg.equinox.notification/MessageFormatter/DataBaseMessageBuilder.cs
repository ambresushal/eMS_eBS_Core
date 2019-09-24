using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.notification
{
    public class DataBaseMessageBuilder<T> : BaseMesssageBuilder
    {
        List<Paramters> result;
        public DataBaseMessageBuilder(string messagekey, T taskParamater, IUnitOfWorkAsync _unitOfWork) : base(messagekey, _unitOfWork)
        {
            var res = TryCast<List<Paramters>>(taskParamater, out result);
        }
        public override Message FormatMessage()
        {
            foreach (var val in result)
            {
                if (val.key == "user")
                    _message.MessageText = _message.MessageText.Replace("{" + val.key + "}", val.temp);
                else
                    _message.MessageText = _message.MessageText.Replace("{" + val.key + "}", val.Value);
            }
            return _message;
        }
    }
}
