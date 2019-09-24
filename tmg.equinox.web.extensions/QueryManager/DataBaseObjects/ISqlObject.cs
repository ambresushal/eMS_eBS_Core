using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
//using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace tmg.equinox.web.QueryManager.DataBaseObjects
{
    public interface ISqlObject
    {
        int Id { get; set; }
        string Name { get; set; }
        ObjectType Kind { get; }
        string Script { get; }
        string Schema { get; set; }
        string Comment { get; set; }
        List<ISqlChild> Childs { get; set; }
        bool IsScriptLoaded { get; }
        void LoadScript(SqlCommand cmd);
    }
}
