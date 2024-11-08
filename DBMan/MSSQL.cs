using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMan
{
    
    public class MSSQL
    {

        #region 속성
        SqlConnection sqlConn;
        public SqlConnectionInformation ConnectionInfo = new SqlConnectionInformation();


        #endregion

        #region 생성자
        /// <summary>
        /// 이 생성자를 사용할 경우 반드시 연결 정보를 구성하고 이니셜라이징(InitSqlConn())을 거쳐야 합니다.
        /// </summary>
        public MSSQL()
        {
        }
        public MSSQL(string ip, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new SqlConnection(ConnectionInfo.GetConnectionString());
        }

        public MSSQL(string ip, int port, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Port = port;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new SqlConnection(ConnectionInfo.GetConnectionString());
        }
        #endregion


        /// <summary>
        /// SQlConnection을 설정된 접속정보로 초기화 합니다.
        /// </summary>
        public void InitSqlConn()
        {
            sqlConn = new SqlConnection(ConnectionInfo.GetConnectionString());
        }

        public DataSet PostProcedure(string procedure_title, Hashtable hs_param)
        {
            List<SqlParameter> sql_param_list = new List<SqlParameter>();
            foreach(DictionaryEntry param in hs_param)
            {
                SqlParameter sql_param = new SqlParameter(param.Key.ToString(),param.Value.ToString());
                sql_param_list.Add(sql_param);
            }
            DataSet result = PostProcedure(procedure_title, sql_param_list.ToArray());
            return result;
        }

        public DataSet PostProcedure(string procedure_title, SqlParameter[] sql_param)
        {
            DataSet ds = new DataSet();
            try
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(procedure_title, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(sql_param);
                var da = new SqlDataAdapter(cmd);

                da.Fill(ds);
                sqlConn.Close();
            }
            catch (Exception e)
            {
                DataTable dt = new DataTable();
                Console.WriteLine(e.ToString());
                dt.Columns.Add("RESULT_CODE");
                dt.Columns.Add("RESULT_MSG");
                dt.Rows.Add("-1", "프로시저 실행 실패\n" + e.ToString());
                sqlConn.Close();
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataTable BulkCopy(DataTable dt, string table)
        {
            DataTable result = new DataTable();
            result.Columns.Add("RESULT_CODE");
            result.Columns.Add("RESULT_MSG");

            sqlConn.Open();
            SqlBulkCopy bulk = new SqlBulkCopy(sqlConn);
            bulk.DestinationTableName = table;
            try
            {
                bulk.WriteToServer(dt);
                result.Rows.Add("1", "Bulk Insert 성공");
            }
            catch (Exception ex)
            {
                result.Rows.Add("-1", $"Bulk Insert 실패 \n {ex.ToString()} ");
            }
            sqlConn.Open();

            return result;
        }

        public void PostNonQuery(string query)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.Connection = sqlConn;
            sqlConn.Open();
            sqlComm.CommandText = query;
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
        }

        public bool ConnectionCheck()
        {
            try
            {
                sqlConn.Open();
                sqlConn.Close();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public DataSet PostQuery(string query)
        {
            DataSet result = new DataSet();
            SqlCommand sqlcomm = new SqlCommand(query, sqlConn);
            SqlDataAdapter ada = new SqlDataAdapter(sqlcomm);
            ada.Fill(result);
            return result;

        }

        public DataSet PostQuery(string query,SqlParameter[] param)
        {
                DataSet result = new DataSet();
                SqlCommand sqlcomm = new SqlCommand(query, sqlConn);
                foreach (SqlParameter p in param)
                { sqlcomm.Parameters.Add(p); }
                SqlDataAdapter ada = new SqlDataAdapter(sqlcomm);
                ada.Fill(result);
                return result;
        }

        public DataSet PostQuery(string query,Hashtable param)
        {
            List<SqlParameter> sqlParam = new List<SqlParameter>();
            foreach(DictionaryEntry i in param)
            {
                SqlParameter sql = new SqlParameter(i.Key.ToString(),i.Value);
                sqlParam.Add(sql);
            }
            return PostQuery(query, sqlParam.ToArray());
        }

        public class SqlConnectionInformation
        {
            string ip;
            string database;
            int? port = 1433;
            string id;
            string password;
            public string Ip
            {
                get
                {
                    return ip;

                }

                set
                {
                    ip = value;
                }
            }

            public string Database
            {
                get
                {
                    return database;
                }

                set
                {
                    database = value;
                }
            }

            public int? Port
            {
                get
                {
                    return port;
                }

                set
                {
                    port = value;
                }
            }

            public string Id
            {
                get
                {
                    return id;
                }

                set
                {
                    id = value;
                }
            }

            public string Password
            {
                //get
                //{
                //    return password;
                //}

                set
                {
                    password = value;
                }
            }

            internal string GetConnectionString()
            {
                string result;
                if (port == null) result = $"Server ={ip}; Database ={database}; uid ={id}; pwd ={password}";
                else result = $"Server ={ip},{port}; Database ={database}; uid ={id}; pwd ={password}";


                return result;
            }
        }
    }
}
