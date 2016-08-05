using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace AutomateGenerator
{
    class GenerateEntity
    {

        #region Structs

        struct TableColumn
        {
            private string columnName;
            private string columnType;

            public string ColumnName
            {
                get { return columnName; }
            }

            public string ColumnType
            {
                get { return columnType; }
            }

            public TableColumn(string columnName, string columnType)
            {
                this.columnName = columnName;
                this.columnType = columnType;
            }
        }

        #endregion

        #region Const Fields

        private const string OutPutFilePath = @"C:\Brandon\OutPutFiles\{0}.txt";

        private const string SqlScript = @"
select c.name as {0}, typ.name as {1} from dbo.syscolumns c inner join dbo.sysobjects t
on c.id = t.id 
inner join dbo.systypes typ on typ.xtype = c.xtype
where OBJECTPROPERTY(t.id, N'IsUserTable') = 1 and typ.name <> 'sysname'
and t.name='{2}' order by c.colorder";

        private const string ConnStr = "server=COSAPX2;uid=sa;pwd=Advent.sa;database=CARS";

        private const string ColumnNameStr = "ColumnName";

        private const string DataTypeNameStr = "DataTypeName";

        private const string OPClassStructure = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CARS.Backend.Common;

namespace CARS.Backend.Entity
{{
    public class {0} : BaseEntity
    {{
        {1}
    }}
}}";

        private const string OPPrivateFieldsAll = @"
#region Private Fields

{0}
#endregion
";

        private const string OPPrivateField = "private {0} {1};";

        private const string OPPublicPropertiesAll = @"#region Public Properties

{0}
#endregion
";

        private const string OPPublicProperty = @"public {0} {1}
{{
    get {{ return {2}; }}
    set {{ {2} = value; }}
}}";

        private const string OPPublicGetProperty = @"public {0} {1}
{{
    get {{ return {2}; }}
}}";

        private const string OPOverrideFieldsAll = @"#region Override Fields

        public override Guid GetPKID()
        {{
            return {0};
        }}

        public override string GetPKIDName()
        {{
            return ""{1}"";
        }}

        public override void SetPKID(Guid pkID)
        {{
            {2} = pkID;
        }}

        public override void FillEntity(DataRow row)
        {{
            {3}}}

#endregion";

        private const string OPOverrideFillEntity = @"{0} = row[""{1}""] != DBNull.Value ? ({2})row[""{1}""] : {3};";

        private const string OPOverrideFillEntityEnum = @"{0} = row[""{1}""] != DBNull.Value ? ({2})Enum.Parse(typeof({2}), row[""{1}""].ToString()) : ({2})Enum.Parse(typeof({2}), {3});";

        #endregion

        #region Public Methods

        /// <summary>
        /// Load table from database.
        /// 1. Generate private fields.
        /// 2. Generaet public properties.
        /// 3. Generate constructures.
        /// 4. Generate public methods.
        /// 5. Generate private methods.
        /// 6. Generate override methods.
        /// 7. Output to a text file.
        /// </summary>
        /// <param name="tableName"></param>
        public static void Generate(string fileName, string tableName)
        {
            List<TableColumn> columns = LoadDB(tableName);
            StringBuilder content = new StringBuilder();
            content.AppendLine(GeneratePrivateFields(columns));
            content.AppendLine(GeneratePublicProperties(columns));
            content.AppendLine(GenerateOverrideFields(columns));
            GenerateOutputFile(fileName, string.Format(OPClassStructure, tableName, content));
        }

        #endregion

        #region Generation Methods

        private static List<TableColumn> LoadDB(string tableName)
        {
            List<TableColumn> columns = new List<TableColumn>();
            DataTable table = new DataTable();

            if (!string.IsNullOrEmpty(tableName))
            {
                string query = string.Format(SqlScript, ColumnNameStr, DataTypeNameStr, tableName);

                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(table);
                    }
                    conn.Close();
                }

                foreach (DataRow row in table.Rows)
                {
                    columns.Add(new TableColumn((string)row[ColumnNameStr], (string)row[DataTypeNameStr]));
                }
            }

            return columns;
        }

        private static string GeneratePrivateFields(List<TableColumn> columns)
        {
            StringBuilder result = new StringBuilder();

            if (null != columns && columns.Count > 0)
            {
                foreach (TableColumn column in columns)
                {
                    result.AppendLine(string.Format(OPPrivateField, 
                                                    GetDataType(column.ColumnName, column.ColumnType),
                                                    ConvertTableColumnNameToFieldName(column.ColumnName)));
                }
            }

            return string.Format(OPPrivateFieldsAll, result.ToString());
        }

        private static string GeneratePublicProperties(List<TableColumn> columns)
        {
            StringBuilder result = new StringBuilder();

            if (null != columns && columns.Count > 0)
            {
                foreach (TableColumn column in columns)
                {
                    if (column.ColumnName.Contains("PK"))
                    {
                        result.AppendLine(string.Format(OPPublicGetProperty,
                                                        GetDataType(column.ColumnName, column.ColumnType),
                                                        column.ColumnName,
                                                        ConvertTableColumnNameToFieldName(column.ColumnName)));
                    }
                    else
                    {
                        result.AppendLine(string.Format(OPPublicProperty,
                                                        GetDataType(column.ColumnName, column.ColumnType),
                                                        column.ColumnName,
                                                        ConvertTableColumnNameToFieldName(column.ColumnName)));
                    }
                }
            }

            return string.Format(OPPublicPropertiesAll, result.ToString());
        }

        private static string GenerateOverrideFields(List<TableColumn> columns)
        {
            StringBuilder result = new StringBuilder();

            if (null != columns && columns.Count > 0)
            {
                foreach (TableColumn column in columns)
                {
                    string emptyValue = null;
                    string templateStr = OPOverrideFillEntity;

                    switch(GetDataType(column.ColumnName, column.ColumnType))
                    {
                        case "Guid":
                            emptyValue = "Guid.Empty";
                            break;
                        case "Sex":
                        case "LeaveStatus":
                        case "RoleRank":
                            emptyValue = string.Format(@"""{0}""", "None");
                            templateStr = OPOverrideFillEntityEnum;
                            break;
                        case "int":
                            emptyValue = "0";
                            break;
                        case "String":
                            emptyValue = "null";
                            break;
                        case "DateTime":
                            emptyValue = "DateTime.MinValue";
                            break;
                        case "bool":
                            emptyValue = "false";
                            break;
                        default:break;
                    }

                    result.AppendLine(string.Format(templateStr,
                                                    ConvertTableColumnNameToFieldName(column.ColumnName),
                                                    column.ColumnName,
                                                    GetDataType(column.ColumnName, column.ColumnType),
                                                    emptyValue));
                }
            }

            return string.Format(OPOverrideFieldsAll, GetPKField(columns),
                                                      GetPKProperty(columns),
                                                      GetPKField(columns),
                                                      result.ToString());
        }

        private static void GenerateOutputFile(string fileName, string content)
        {
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(content))
            {
                File.WriteAllText(string.Format(OutPutFilePath, fileName), content);
            }
        }

        #endregion  

        #region Private Methods

        private static string GetDataType(string columnName, string typeName)
        {
            string result = null;

            if (!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(columnName))
            {
                switch (typeName)
                {
                    case "uniqueidentifier":
                        result = "Guid";
                        break;
                    case "nvarchar":
                        result = "String";
                        break;
                    case "smallint":
                        switch (columnName)
                        {
                            case "Gender":
                                result = "Sex";
                                break;
                            case "Status":
                                result = "LeaveStatus";
                                break;
                            case "Rank":
                                result = "RoleRank";
                                break;
                            default: break;
                        }
                        break;
                    case "datetime":
                        result = "DateTime";
                        break;
                    case "bit":
                        result = "bool";
                        break;
                    case "int":
                        result = "int";
                        break;
                    default: break;
                }
            }

            return result;
        }

        private static string ConvertTableColumnNameToFieldName(string tableColumnName)
        {
            string result = null;

            if (!string.IsNullOrEmpty(tableColumnName))
            {
                if (tableColumnName.Contains("PK"))
                {
                    result = "pk" + tableColumnName.Substring(2);
                }
                else if (tableColumnName.Contains("FK"))
                {
                    result = "fk" + tableColumnName.Substring(2);
                }
                else
                {
                    result = tableColumnName[0].ToString().ToLower() + tableColumnName.Substring(1);
                }
            }

            return result;
        }

        private static string GetPKProperty(List<TableColumn> columns)
        {
            string result = null;

            if (null != columns && columns.Count > 0)
            {
                foreach (TableColumn column in columns)
                {
                    if (column.ColumnName.Contains("PK"))
                    {
                        result = column.ColumnName;
                        break;
                    }
                }
            }

            return result;
        }

        private static string GetPKField(List<TableColumn> columns)
        {
            return ConvertTableColumnNameToFieldName(GetPKProperty(columns));
        }

        #endregion

    }
}
