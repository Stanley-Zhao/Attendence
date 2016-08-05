using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.Entity;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using CARS.Backend.Common;

namespace CARS.Backend.DAL
{
	/// <summary>
	/// 1. T is the exact type of entity.
	/// 2. Get method, should invoke the constructure of each entity to fill in the data.
	/// 3. Insert method, go through the properties of the entity and combine the query. 
	/// 4. Update method, use the GUID and update all properties.
	/// 5. Delete method, use the GID to delete the record.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CommonDAL<T> where T : BaseEntity
	{
		#region Public Methods

		public static T GetSingleObject(List<SearchCondition> conditions)
		{
			T t = null;

			string query = GetSelectQuery(conditions);
			DataTable table = GetDataTable(query);
			if (null != table.Rows && table.Rows.Count > 0)
			{
				t = (T)Activator.CreateInstance(typeof(T), 523);
				t.SetIsNewFlag(false);
				t.Init(table.Rows[0]);
			}

			return t;
		}

		public static List<T> GetObjects(List<SearchCondition> conditions)
		{
			return GetObjects(conditions, string.Empty, OrderType.ASC);
		}

		public static List<T> GetObjects(List<SearchCondition> conditions, string columnName, OrderType orderType)
		{
			List<T> objects = new List<T>();
			string query;
			if (columnName != string.Empty)
				query = GetSelectQueryOrderBy(conditions, columnName, orderType);
			else
				query = GetSelectQuery(conditions);
			DataTable table = GetDataTable(query);

			if (null != table.Rows)
			{
				foreach (DataRow row in table.Rows)
				{
					T t = (T)Activator.CreateInstance(typeof(T), 523);
					t.SetIsNewFlag(false);
					t.Init(row);
					objects.Add(t);
				}
			}

			return objects;
		}

		public static void Update(T t, List<SearchCondition> conditions)
		{
			t.SetKnowledgeDate(DateTime.Now);
			string query = GetUpdateQuery(t, conditions);

			using (SqlConnection conn = CommonConnection.Conn)
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.Add(new SqlParameter(GlobalParams.TimeTokenParameter, t.TimeToken));
					t.SetTimeToker((byte[])cmd.ExecuteScalar());
				}
				conn.Close();
			}
		}

		public static void Insert(T t)
		{
			t.SetPKID(Guid.NewGuid());
			t.SetKnowledgeDate(DateTime.Now);
			t.SetCreatedDate(DateTime.Now);
			string query = GetInsertQuery(t);

			using (SqlConnection conn = CommonConnection.Conn)
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					t.SetTimeToker((byte[])cmd.ExecuteScalar());
				}
				conn.Close();
			}

			t.SetIsNewFlag(false);
		}

		public static void Delete(T t)
		{
			string query = string.Format(GlobalParams.DeleteQuery, typeof(T).Name, t.GetPKIDName(), t.GetPKID().ToString());

			using (SqlConnection conn = CommonConnection.Conn)
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.ExecuteNonQuery();
				}
				conn.Close();
			}
		}

		#endregion

		#region Private Methods

		private static string GetSelectQuery(List<SearchCondition> conditions)
		{
			StringBuilder query = new StringBuilder(string.Format(GlobalParams.SelectQuery, typeof(T).Name));

			string whereClause = GetWhereClause(conditions);
			if (!string.IsNullOrEmpty(whereClause)) query.Append(whereClause);

			return query.ToString();
		}

		private static string GetSelectQueryOrderBy(List<SearchCondition> conditions, string columnName, OrderType orderType)
		{
			StringBuilder query = new StringBuilder(string.Format(GlobalParams.SelectQuery, typeof(T).Name));
			
			string whereClause = GetWhereClause(conditions);
			if (!string.IsNullOrEmpty(whereClause)) query.Append(whereClause);

			query.Append(" ORDER BY ");
			query.Append(columnName);
			query.Append(" ");
			query.Append(orderType.ToString());			

			return query.ToString();
		}

		private static string GetUpdateQuery(T t, List<SearchCondition> conditions)
		{
			StringBuilder setClause = new StringBuilder();
			PropertyInfo[] propertyInfos = t.GetType().GetProperties();

			foreach (PropertyInfo info in propertyInfos)
			{
				string setItem = GetSetClause(t, info);
				if (!string.IsNullOrEmpty(setItem))
				{
					if (setClause.Length != 0) setClause.Append(string.Format(", {0}", setItem));
					else setClause.Append(setItem);
				}
			}

			string whereQuery = GetWhereClause(conditions);
			StringBuilder whereClause = new StringBuilder(whereQuery);
			if (whereClause.Length > 0) whereClause.Append(" and " + GlobalParams.UpdateTimeTokenClause);
			else whereClause.Append(" where " + GlobalParams.UpdateTimeTokenClause);

			// Select the time token back after update.
			return string.Format(GlobalParams.UpdateQuery, typeof(T).Name, setClause.ToString(), whereClause.ToString())
				   + string.Format(GlobalParams.SelectTimeTokenQuery, typeof(T).Name, whereQuery);
		}

		private static string GetInsertQuery(T t)
		{
			StringBuilder query = new StringBuilder();
			StringBuilder propNameClause = new StringBuilder();
			StringBuilder propValueClause = new StringBuilder();

			PropertyInfo[] propertyInfos = t.GetType().GetProperties();
			foreach (PropertyInfo info in propertyInfos)
			{
				if (IsNormalProperty(info))
				{
					if (propNameClause.Length != 0) propNameClause.Append(string.Format(", {0}", info.Name));
					else propNameClause.Append(info.Name);

					Object obj = info.GetValue(t, null);
					string itemValue = null;
					string propertyType = info.PropertyType.Name.ToUpper();

					if (obj != null)
					{
						switch (propertyType)
						{
							case "INT32":
							case "DOUBLE":
							case "SINGLE":
								itemValue = obj.ToString();
								break;
							case "STRING":
								itemValue = string.Format("N'{0}'", null != obj.ToString() ? obj.ToString().Replace("'", "''") : null);
								break;
                            case "DATETIME":
                            case "NULLABLE`1": //This type is DateTime?
							case "BOOLEAN":
								itemValue = string.Format("'{0}'", obj.ToString());
								break;
							case "GUID":
								itemValue = (Guid)obj != Guid.Empty ? string.Format("'{0}'", obj.ToString()) : "NULL";
								break;
							default:
								if (info.PropertyType.IsEnum)
								{
									itemValue = Enum.Parse(info.PropertyType, obj.ToString()).GetHashCode().ToString();
								}
								break;
						}
					}
					else
					{
						itemValue = "NULL";
					}

					if (propValueClause.Length != 0) propValueClause.Append(string.Format(", {0}", itemValue));
					else propValueClause.Append(itemValue);
				}
			}

			// Gets the time token back after insert.
			return string.Format(GlobalParams.InsertQuery, typeof(T).Name, propNameClause.ToString(), propValueClause.ToString())
				   + string.Format(GlobalParams.SelectTimeTokenQuery, typeof(T).Name, string.Format(" where {0} = '{1}' ", t.GetPKIDName(), t.GetPKID().ToString()));
		}

		private static string GetSetClause(T t, PropertyInfo info)
		{
			string result = null;

			if (IsNormalProperty(info))
			{
				Object obj = info.GetValue(t, null);

                switch (info.PropertyType.Name.ToUpper())
                {
                    case "STRING":
                        result = null != obj ? string.Format(" {0} = N'{1}' ", info.Name, null != obj.ToString() ? obj.ToString().Replace("'", "''") : null) : string.Format(" {0} = {1} ", info.Name, "null");
                        break;
                    case "DATETIME":
                    case "NULLABLE`1": //This type is DateTime?
                    case "BOOLEAN":
                        result = null != obj ? string.Format(" {0} = '{1}' ", info.Name, obj.ToString()) : string.Format(" {0} = {1} ", info.Name, "null");
                        break;
                    case "GUID":
                        result = Guid.Empty != (Guid)obj ? string.Format(" {0} = '{1}' ", info.Name, obj.ToString()) : string.Format(" {0} = {1} ", info.Name, "null");
                        break;
                    case "INT32":
                    case "DOUBLE":
                    case "SINGLE":
                        result = string.Format(" {0} = {1} ", info.Name, null != obj ? obj.ToString() : "null");
                        break;
                    case "SEX":
                    case "LEAVESTATUS":
                    case "ROLERANK":
                        result = string.Format(" {0} = {1} ", info.Name, null != obj ? ((int)obj).ToString() : "null");
                        break;
                    default: break;
                }
			}

			return result;
		}

		private static string GetItemValueClause(T t, PropertyInfo info)
		{
			string result = null;

			switch (info.PropertyType.Name.ToUpper())
			{
				case "STRING":
				case "GUID":
					Object obj = info.GetValue(t, null);
					if (null != obj) result = string.Format(" '{0}' ", obj.ToString());
					else result = string.Empty;
					break;
				case "DATE":
					break;
				default: break;
			}

			return result;
		}

		private static string GetWhereClause(List<SearchCondition> conditions)
		{
			StringBuilder whereClause = new StringBuilder();
			string comparator = null;

			if (null != conditions && conditions.Count > 0)
			{
				foreach (SearchCondition condition in conditions)
				{
					if (whereClause.Length == 0)
					{
						whereClause.Append("Where");
					}
					else
					{
						whereClause.Append("and");
					}

					switch (condition.Comparator)
					{
						case Common.SearchComparator.Equal:
							comparator = "=";
							break;
						case Common.SearchComparator.NotEqual:
							comparator = "!=";
							break;
						case Common.SearchComparator.Greater:
							comparator = ">=";
							break;
						case Common.SearchComparator.Less:
							comparator = "<=";
							break;
						default: break;
					}

					switch (condition.ComparaType)
					{
						case SearchType.SearchNotString:
							whereClause.Append(string.Format(" {0} {1} {2} ", condition.SearchKey, comparator, condition.SearchValue));
							break;
						case SearchType.SearchString:
							whereClause.Append(string.Format(" {0} {1} '{2}' ", condition.SearchKey, comparator, condition.SearchValue));
							break;
						default: break;
					}
				}
			}

			return whereClause.ToString();
		}

		private static DataTable GetDataTable(string query)
		{
			DataTable table = new DataTable();

			using (SqlConnection conn = CommonConnection.Conn)
			{
				conn.Open();
				using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
				{
					adapter.Fill(table);
				}
				conn.Close();
			}

			return table;
		}

		private static bool IsNormalProperty(PropertyInfo property)
		{
			bool result = null != property;

			if (result)
			{
				object[] attributes = property.GetCustomAttributes(true);
				foreach (Attribute attribute in attributes)
				{
					if (attribute is CustomAttribute)
					{
						result = false;
						break;
					}
				}
			}

			return result;
		}

		#endregion
	}
}
