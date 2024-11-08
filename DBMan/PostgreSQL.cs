using Npgsql;
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
    /// nuget 설치필요 Npgsql V4.0.17
    /// <para>Install-Package Npgsql -Version 4.0.17</para>
    /// </summary>
    public class PostgreSQL
    {
        public SqlConnectionInformation ConnectionInfo = new SqlConnectionInformation();
        NpgsqlConnection sqlConn;

        #region 생성자
        /// <summary>
        /// 이 생성자를 사용할 경우 반드시 연결 정보를 구성하고 이니셜라이징(InitSqlConn())을 거쳐야 합니다.
        /// </summary>
        public PostgreSQL()
        {
        }
        public PostgreSQL(string ip, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new NpgsqlConnection(ConnectionInfo.GetConnectionString());
        }

        public PostgreSQL(string ip, int port, string database, string id, string password)
        {
            ConnectionInfo.Ip = ip;
            ConnectionInfo.Port = port;
            ConnectionInfo.Database = database;
            ConnectionInfo.Id = id;
            ConnectionInfo.Password = password;
            sqlConn = new NpgsqlConnection(ConnectionInfo.GetConnectionString());
        }
        #endregion

        /// <summary>
        /// SQlConnection을 설정된 접속정보로 초기화 합니다.
        /// </summary>
        public void InitSqlConn()
        {
            sqlConn = new NpgsqlConnection(ConnectionInfo.GetConnectionString());
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
            NpgsqlCommand sqlcomm = new NpgsqlCommand(query, sqlConn);
            NpgsqlDataAdapter ada = new NpgsqlDataAdapter(sqlcomm);
            ada.Fill(result);
            return result;

        }

        public DataSet PostQuery(string query, NpgsqlParameter[] param)
        {
            DataSet result = new DataSet();
            NpgsqlCommand sqlcomm = new NpgsqlCommand(query, sqlConn);
            foreach (NpgsqlParameter p in param)
            { sqlcomm.Parameters.Add(p); }
            NpgsqlDataAdapter ada = new NpgsqlDataAdapter(sqlcomm);
            ada.Fill(result);
            return result;
        }

        public DataSet PostQuery(string query, Hashtable param)
        {
            List<NpgsqlParameter> sqlParam = new List<NpgsqlParameter>();
            foreach (DictionaryEntry i in param)
            {
                NpgsqlParameter sql = new NpgsqlParameter(i.Key.ToString(), i.Value);
                sqlParam.Add(sql);
            }
            return PostQuery(query, sqlParam.ToArray());
        }

        #region SqlConnInfo Class
        public class SqlConnectionInformation
        {
            string ip;
            string database;
            int? port = 5432;
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
                if (port == null) result = $"HOST ={ip}; Database ={database}; USERNAME ={id}; PASSWORD ={password}";
                else result = $"HOST ={ip};PORT={port}; Database ={database}; USERNAME ={id}; PASSWORD ={password}";


                return result;
            }
        }
        #endregion

    }
}
