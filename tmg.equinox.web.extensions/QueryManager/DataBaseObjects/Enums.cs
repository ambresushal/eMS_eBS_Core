using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.web.QueryManager.DataBaseObjects
{
    public enum ObjectType { Schema, Table, View, Procedure, ScalarFunction, TableFunction, Alias };
    public enum ChildType { Field, Parameter };
}
