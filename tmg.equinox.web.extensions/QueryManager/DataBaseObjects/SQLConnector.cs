using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Threading;
using System.Drawing;

namespace tmg.equinox.web.QueryManager.DataBaseObjects
{
    public delegate void EndingLoad();
    public class SqlConnector
    {
        private bool _Loaded;
        public bool Loaded { get { return _Loaded; } }
        private bool _FullLoaded;
        public bool FullLoaded { get { return _FullLoaded; } }        
        private string _ConnectionString;
        public string ConnectionString 
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
        public bool _IsBusy;
        public bool IsBusy 
        { 
            get { return _IsBusy; }
            set { _IsBusy = value; } 
        }
        private SqlConnection _Connection;
        public SqlConnection Connection 
        {
            get { return _Connection; }
            set { _Connection = value; }
        }
        
        public string Server
        {
            get
            {
                if (Connection != null)
                    return Connection.DataSource;
                return "";
            }
        }
        public string DataBase
        {
            get
            {
                if (Connection != null)
                    return Connection.Database;
                return "";
            }
        }
        
        public SqlConnector(string ConnectionString)
        {
            Connection = new SqlConnection(ConnectionString);
            this.ConnectionString = ConnectionString;

        }
        private List<ISqlObject> _DbObjects = new List<ISqlObject>();
        public List<ISqlObject> DbObjects 
        {
            get { return _DbObjects; }
            set { _DbObjects = value; }
        }        

        public int LoadTables(bool FullLoad = false, int id = 0)
        {
            string sql;
            List<Table> Tables = new List<Table>();
            sql = @"
SELECT DISTINCT
	ISNULL(sch.name, 'dbo') as 'Schema',
	sobj.id as TableId,
	sobj.name as TableName,
	cols.colid as ColumnId,
	cols.name as ColumnName,
	type_name(cols.xusertype) as ColumnType,
	isnull(cols.prec, 0) as Precision,
	isnull(cols.Scale, 0) as Scale,
	isnull(cols.isnullable, 1) as Nullable,
	isnull(cols.iscomputed, 0) as Calculated,
	isnull(comm.text, '') as DefaultValue,
	case when pk.xtype is null then '0' else '1' end as PKey,
	case when fk.fkey is null then '0' else '1' end as FKey,
	isnull(fk.rkeyid, 0) as ReferenceID,
	isnull(fk2.name, '') as ReferenceTable,
	isnull(cols2.name, '') as ReferenceFieldName,
	isnull(cols2.colid, 0) as ReferenceFieldId,
	'' as IndexName,--isnull(indx.name, '') as IndexName,
	isnull(COLUMNPROPERTY(sobj.id,cols.name,'IsIdentity'), 0) IsIdentity,
	IDENT_SEED(sch.name + '.' + sobj.name) as Seed,
	IDENT_INCR(sch.name + '.' + sobj.name) as Increment
FROM   
	sysobjects sobj INNER JOIN syscolumns cols ON sobj.id = cols.id
	LEFT JOIN sysforeignkeys fk ON fk.fkeyid = cols.id AND fk.fkey = cols.colid
	LEFT JOIN syscolumns cols2 ON cols2.id = fk.rkeyid AND cols2.colid = fk.rkey
	LEFT JOIN sysobjects fk2 ON fk.rkeyid = fk2.id
	LEFT JOIN syscomments comm ON cols.cdefault = comm.id OR (cols.id = comm.id and cols.colid = comm.number)
	LEFT JOIN sysindexkeys ik ON ik.id = cols.id AND ik.colid = cols.colid
	LEFT JOIN sysindexes indx ON indx.id = ik.id AND indx.indid = ik.indid
	LEFT JOIN sysobjects pk ON indx.name = pk.name AND pk.parent_obj = indx.id AND pk.xtype = 'PK'
	LEFT JOIN Sys.Objects ObjAux ON sobj.id = ObjAux.object_id
	LEFT JOIN Sys.Schemas sch ON ObjAux.schema_id = sch.schema_id
WHERE   
	sobj.xtype = 'U'   
	and sobj.name <> 'sysdiagrams'";

            if(id > 0)            
                sql = sql + " and sobj.id = " + id.ToString() + " order by ISNULL(sch.name, 'dbo'), sobj.name, cols.colid";
            else
                sql = sql + " order by ISNULL(sch.name, 'dbo'), sobj.name, cols.colid";

            //DbObjects.RemoveAll(X => X.Kind == ObjectType.Table);

            //Get the schema for the tables
            DataTable Info = new DataTable();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.SelectCommand.Connection.Open();
                da.Fill(Info);
                da.SelectCommand.Connection.Close();
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                return 0;
            }
            cmd.Dispose();

            if (Tables != null)
                Tables.Clear();
            else
                Tables = new List<Table>();

            Table CurObj = null;
            string curtable = "";
            int i, rowcount = Info.Rows.Count;
            for (i = 0; i < rowcount; i++)
            {
                if (!curtable.Equals(Info.Rows[i]["TableName"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    CurObj = new Table()
                    {
                        Name = Info.Rows[i]["TableName"].ToString(),
                        Comment = "",
                        Id = Convert.ToInt32(Info.Rows[i]["TableId"]),
                        Schema = Info.Rows[i]["Schema"].ToString()
                    };
                    curtable = CurObj.Name;
                    Tables.Add(CurObj);
                }

                ISqlChild Exists;
                Exists = CurObj.Childs.Where(X => X.Id == Convert.ToInt32(Info.Rows[i]["ColumnId"])).FirstOrDefault();
                if (Exists != null && Exists.Id > 0)
                {
                    if (!Exists.IsPrimaryKey)
                        Exists.IsPrimaryKey = Convert.ToInt32(Info.Rows[i]["PKey"]) == 1;
                }
                else
                {
                    Field Field = new Field()
                    {
                        Comment = "",
                        Computed = Convert.ToInt32(Info.Rows[i]["Calculated"]) == 1,
                        Id = Convert.ToInt32(Info.Rows[i]["ColumnId"]),
                        ForeignKey = Convert.ToInt32(Info.Rows[i]["ReferenceFieldId"]),
                        IsIdentity = Convert.ToInt32(Info.Rows[i]["IsIdentity"]) == 1,
                        IsPrimaryKey = Convert.ToInt32(Info.Rows[i]["PKey"]) == 1,
                        Name = Info.Rows[i]["ColumnName"].ToString(),
                        Nullable = Convert.ToInt32(Info.Rows[i]["Nullable"]) == 1,
                        Parent = CurObj,
                        Precision = Convert.ToInt32(Info.Rows[i]["Precision"]),
                        Type = Info.Rows[i]["ColumnType"].ToString()
                    };
                    if (Field.IsIdentity)
                    {
                        Field.Increment = Convert.ToInt32(Info.Rows[i]["Increment"]);
                        Field.Seed = Convert.ToInt32(Info.Rows[i]["Seed"]);
                    }
                    else
                    {
                        Field.Increment = 0;
                        Field.Seed = 0;
                    }
                    CurObj.Childs.Add(Field);
                }
            }
            if (FullLoad)
            {
                foreach (ISqlObject Table in Tables)
                {
                    Table.LoadScript(null);
                }
            }
            DbObjects.AddRange(Tables);
            return Tables.Count;
        }
        public int LoadViews(bool FullLoad = false, int id = 0)
        {
            List<View> Views = new List<View>();
            string sql;
            sql = @"
SELECT  
	ISNULL(sch.name, 'dbo') as 'Schema',
	sobj.id as ViewId, 
	sobj.name as ViewName, 
	cols.name as FieldName,    
	type_name(cols.xusertype) as Type,    
	isnull(cols.prec, 0) as Length,    
	isnull(cols.Scale, 0) as Scale,    
	isnull(cols.isnullable, 1) as Nullable,    
	isnull(cols.iscomputed, 0) as Calculated	 
FROM  
	sysobjects sobj INNER JOIN syscolumns cols ON sobj.id=cols.id 
	LEFT JOIN Sys.Objects ObjAux ON sobj.id = ObjAux.object_id
	LEFT JOIN Sys.Schemas sch ON ObjAux.schema_id = sch.schema_id
WHERE  
	sobj.xtype = 'V'";

            if (id > 0)
                sql = sql + " and sobj.id = " + id.ToString() + " ORDER BY sobj.id, cols.name";
            else
                sql = sql + " ORDER BY sobj.id, cols.name";

            DataTable Info = new DataTable();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.SelectCommand.Connection.Open();
                da.Fill(Info);
                da.SelectCommand.Connection.Close();
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                return 0;
            }
            cmd.Dispose();

            //DbObjects.RemoveAll(X => X.Kind == ObjectType.View);

            View CurObj = null;
            string curview = "";
            int i, rowcount = Info.Rows.Count;
            for (i = 0; i < rowcount; i++)
            {
                if (!curview.Equals(Info.Rows[i]["ViewName"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                {//agregar solo el campo
                    CurObj = new View()
                        {
                            Name = Info.Rows[i]["ViewName"].ToString(),
                            Id = Convert.ToInt32(Info.Rows[i]["ViewId"]),
                            Schema = Info.Rows[i]["Schema"].ToString(),
                            Comment = ""
                        };
                    curview = CurObj.Name;
                    Views.Add(CurObj);
                }

                Field Field = new Field()
                {
                    Comment = "",
                    Computed = false,
                    DefaultValue = "",
                    ForeignKey = 0,
                    Id = 0,
                    IdentityScript = "",
                    Increment = 0,
                    IsForeignKey = false,
                    IsIdentity = false,
                    IsPrimaryKey = false,
                    Name = Info.Rows[i]["FieldName"].ToString(),
                    Nullable = true,
                    Parent = CurObj,
                    Precision = Convert.ToInt32(Info.Rows[i]["Length"]),
                    ReferenceChild = null,
                    ReferenceChildName = "",
                    ReferenceParent = null,
                    ReferenceParentName = "",
                    Seed = 0,
                    Type = Info.Rows[i]["Type"].ToString()
                };

                CurObj.Childs.Add(Field);
            }
            if (FullLoad)
            {
                cmd.Connection.Open();
                try
                {
                    foreach (ISqlObject View in Views)
                    {
                        View.LoadScript(cmd);
                    }
                }
                catch (Exception)
                {
                    ;
                }
                finally
                {
                    cmd.Connection.Close();
                }

            }
            DbObjects.AddRange(Views);
            return Views.Count;
        }
        public int LoadProcedures(bool FullLoad = false, int id = 0)
        {
            List<Procedure> Procedures = new List<Procedure>();
            string sql = @"
SELECT  
	ISNULL(sch.name, 'dbo') as 'Schema',
	sobj.id as ProcedureId, 
	sobj.name as ProcedureName, 
	cols.name as ParamName,    
	type_name(cols.xusertype) as Type,    
	isnull(cols.prec, 0) as Length,    
	isnull(cols.Scale, 0) as Scale,    
	isnull(cols.isnullable, 1) as Nullable,    
	isnull(cols.iscomputed, 0) as Calculated	 
FROM  
	sysobjects sobj LEFT OUTER JOIN syscolumns cols ON sobj.id=cols.id 
	LEFT JOIN Sys.Objects ObjAux ON sobj.id = ObjAux.object_id
	LEFT JOIN Sys.Schemas sch ON ObjAux.schema_id = sch.schema_id
WHERE  
	sobj.xtype = 'P' 
	AND sobj.category = 0";

            if (id > 0)
                sql = sql + " and sobj.id = " + id.ToString() + " ORDER BY sobj.id, cols.name";
            else
                sql = sql + " ORDER BY sobj.id, cols.name";

            DataTable Info = new DataTable();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.SelectCommand.Connection.Open();
                da.Fill(Info);
                da.SelectCommand.Connection.Close();
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                return 0;
            }
            cmd.Dispose();

            //DbObjects.RemoveAll(X => X.Kind == ObjectType.Procedure);

            Procedure CurObj = null;
            string curproc = "";
            int i, rowcount = Info.Rows.Count;
            for (i = 0; i < rowcount; i++)
            {
                if (!curproc.Equals(Info.Rows[i]["ProcedureName"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                {//agregar solo el campo
                    CurObj = new Procedure()
                    {
                        Name = Info.Rows[i]["ProcedureName"].ToString(),
                        Id = Convert.ToInt32(Info.Rows[i]["ProcedureId"]),
                        Schema = Info.Rows[i]["Schema"].ToString(),
                        Comment = ""
                    };
                    curproc = CurObj.Name;
                    Procedures.Add(CurObj);
                }

                if (Info.Rows[i]["ParamName"] == DBNull.Value || String.IsNullOrEmpty(Info.Rows[i]["ParamName"].ToString()))
                    continue;

                Parameter Param = new Parameter()
                {
                    Comment = "",
                    Computed = false,
                    DefaultValue = "",
                    ForeignKey = 0,
                    Id = 0,
                    IdentityScript = "",
                    Increment = 0,
                    IsForeignKey = false,
                    IsIdentity = false,
                    IsPrimaryKey = false,
                    Name = Info.Rows[i]["ParamName"].ToString(),
                    Nullable = true,
                    Parent = CurObj,
                    Precision = Convert.ToInt32(Info.Rows[i]["Length"]),
                    ReferenceChild = null,
                    ReferenceChildName = "",
                    ReferenceParent = null,
                    ReferenceParentName = "",
                    Seed = 0,
                    Type = Info.Rows[i]["Type"].ToString()
                };

                CurObj.Childs.Add(Param);
            }
            if (FullLoad)
            {
                cmd.Connection.Open();
                try
                {
                    foreach (ISqlObject Proc in Procedures)
                    {
                        Proc.LoadScript(cmd);
                    }
                }
                catch (Exception)
                {
                    ;
                }
                finally
                {
                    cmd.Connection.Close();
                }

            }
            DbObjects.AddRange(Procedures);
            return Procedures.Count;
        }
        public int LoadScalarFunctions(bool FullLoad = false, int id = 0)
        {
            List<ScalarFunction> Functions = new List<ScalarFunction>();
            string sql = @"
SELECT  
	ISNULL(sch.name, 'dbo') as 'Schema',
	sobj.id as FunctionId, 
	sobj.name as FunctionName, 
	cols.name as ParamName,    
	type_name(cols.xusertype) as Type,    
	isnull(cols.prec, 0) as Length,    
	isnull(cols.Scale, 0) as Scale,    
	isnull(cols.isnullable, 1) as Nullable,    
	isnull(cols.iscomputed, 0) as Calculated	 
FROM  
	sysobjects sobj LEFT OUTER JOIN syscolumns cols ON sobj.id=cols.id 
	LEFT JOIN Sys.Objects ObjAux ON sobj.id = ObjAux.object_id
	LEFT JOIN Sys.Schemas sch ON ObjAux.schema_id = sch.schema_id
WHERE  
	sobj.xtype = 'FN'";

            if (id > 0)
                sql = sql + " and sobj.id = " + id.ToString() + " ORDER BY sobj.id, cols.colid";
            else
                sql = sql + " ORDER BY sobj.id, cols.colid";

            DataTable Info = new DataTable();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.SelectCommand.Connection.Open();
                da.Fill(Info);
                da.SelectCommand.Connection.Close();
            }
            catch (Exception)
            {
                cmd.Connection.Close();
                return 0;
            }
            cmd.Dispose();

            //DbObjects.RemoveAll(X => X.Kind == ObjectType.ScalarFunction);

            ScalarFunction CurObj = null;
            string curfunc = "";
            int i, rowcount = Info.Rows.Count;
            for (i = 0; i < rowcount; i++)
            {
                if (!curfunc.Equals(Info.Rows[i]["FunctionName"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                {//agregar solo el campo
                    CurObj = new ScalarFunction()
                    {
                        Name = Info.Rows[i]["FunctionName"].ToString(),
                        Id = Convert.ToInt32(Info.Rows[i]["FunctionId"]),
                        Schema = Info.Rows[i]["Schema"].ToString(),
                        Comment = ""
                    };
                    curfunc = CurObj.Name;
                    Functions.Add(CurObj);
                }

                if (Info.Rows[i]["ParamName"] == DBNull.Value || String.IsNullOrEmpty(Info.Rows[i]["ParamName"].ToString()))
                    continue;

                Parameter Param = new Parameter()
                {
                    Comment = "",
                    Computed = false,
                    DefaultValue = "",
                    ForeignKey = 0,
                    Id = 0,
                    IdentityScript = "",
                    Increment = 0,
                    IsForeignKey = false,
                    IsIdentity = false,
                    IsPrimaryKey = false,
                    Name = Info.Rows[i]["ParamName"].ToString(),
                    Nullable = true,
                    Parent = CurObj,
                    Precision = Convert.ToInt32(Info.Rows[i]["Length"]),
                    ReferenceChild = null,
                    ReferenceChildName = "",
                    ReferenceParent = null,
                    ReferenceParentName = "",
                    Seed = 0,
                    Type = Info.Rows[i]["Type"].ToString()
                };

                CurObj.Childs.Add(Param);
            }
            if (FullLoad)
            {
                cmd.Connection.Open();
                try
                {
                    foreach (ISqlObject Funct in Functions)
                    {
                        Funct.LoadScript(cmd);
                    }
                }
                catch (Exception)
                {
                    ;
                }
                finally
                {
                    cmd.Connection.Close();
                }

            }
            DbObjects.AddRange(Functions);
            return Functions.Count;
        }

    }
}
