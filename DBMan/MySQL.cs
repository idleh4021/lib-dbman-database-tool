using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMan
{
    /// <summary>
    /// nuget 설치필요 MySqlConnector V2.1.13
    /// <para>Install-package MySqlConnector -Version 2.1.13</para>
    /// </summary>
    public class MySQL
    {
        #region 속성
        MySqlConnection sqlConn;
        public SqlConnectionInformation ConnectionInfo = new SqlConnectionInformation();


        #endregion

        #region 생성자
        /// <summary>
        /// 이 생성자를 사용할 경우 반드시 연결 정보를 구성하고 이니셜라이징(InitSqlConn())을 거쳐야 합니다.
        /// </summary>
        public MySQL()
        {
        }
        public MySQL(string ip, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new MySqlConnection(ConnectionInfo.GetConnectionString());
        }

        public MySQL(string ip, int port, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Port = port;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new MySqlConnection(ConnectionInfo.GetConnectionString());
        }
        #endregion


        /// <summary>
        /// SQlConnection을 설정된 접속정보로 초기화 합니다.
        /// </summary>
        public void InitSqlConn()
        {
            sqlConn = new MySqlConnection(ConnectionInfo.GetConnectionString());
        }

        public DataSet PostProcedure(string procedure_title, Hashtable hs_param)
        {
            List<MySqlParameter> sql_param_list = new List<MySqlParameter>();
            foreach (DictionaryEntry param in hs_param)
            {
                MySqlParameter sql_param = new MySqlParameter(param.Key.ToString(), param.Value.ToString());
                sql_param_list.Add(sql_param);
            }
            DataSet result = PostProcedure(procedure_title, sql_param_list.ToArray());
            return result;
        }

        public DataSet PostProcedure(string procedure_title, MySqlParameter[] sql_param)
        {
            DataSet ds = new DataSet();
            try
            {
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(procedure_title, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(sql_param);
                var da = new MySqlDataAdapter(cmd);

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
            MySqlBulkCopy bulk = new MySqlBulkCopy(sqlConn);
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
            MySqlCommand sqlComm = new MySqlCommand();
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataSet PostQuery(string query)
        {
            
            DataSet result = new DataSet();
            MySqlCommand sqlcomm = new MySqlCommand(query, sqlConn);
            MySqlDataAdapter ada = new MySqlDataAdapter(sqlcomm);
            sqlConn.Open();
            ada.Fill(result);
            sqlConn.Close();
            return result;

        }

        public DataSet PostQuery(string query, MySqlParameter[] param)
        {
            DataSet result = new DataSet();
            MySqlCommand sqlcomm = new MySqlCommand(query, sqlConn);
            foreach (MySqlParameter p in param)
            { sqlcomm.Parameters.Add(p); }
            MySqlDataAdapter ada = new MySqlDataAdapter(sqlcomm);
            sqlConn.Open();
            ada.Fill(result);
            sqlConn.Close();
            return result;
        }

        public DataSet PostQuery(string query, Hashtable param)
        {
            List<MySqlParameter> sqlParam = new List<MySqlParameter>();
            foreach (DictionaryEntry i in param)
            {
                MySqlParameter sql = new MySqlParameter(i.Key.ToString(), i.Value);
                sqlParam.Add(sql);
            }
            return PostQuery(query, sqlParam.ToArray());
        }

        public class SqlConnectionInformation
        {
            string ip;
            string database;
            int? port = 3306;
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
                if (port == null) result = $"Server={ip};Database={database};Uid={id};Pwd={password};";
                else result = $"Server={ip};Port={port};Database={database};Uid={id};Pwd={password};"; 

                return result;
            }
        }
    }
}
