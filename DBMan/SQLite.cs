using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DBMan
{
    /// <summary>
    /// nuget 설치필요(System.Data.SQLite) 
    /// <para>Install-Package System.Data.SQLite</para>
    /// </summary>
    public class SQLite
    {
        public SqlConnectionInformation ConnectionInfo = new SqlConnectionInformation();
        SQLiteConnection sqlConn;

        #region 생성자
        public SQLite()
        {

        }

        public SQLite(string DBPath)
        {
            ConnectionInfo.DBPath = DBPath;
            sqlConn = new SQLiteConnection(ConnectionInfo.DBSource);
        }
        #endregion
        /// <summary>
        /// 이 생성자를 사용할 경우 반드시 연결 정보를 구성하고 이니셜라이징(InitSqlConn())을 거쳐야 합니다.
        /// </summary>

        /// <summary>
        /// SQlConnection을 설정된 접속정보로 초기화 합니다.
        /// </summary>
        public void InitSqlConn()
        {
            sqlConn = new SQLiteConnection(ConnectionInfo.DBSource);
        }

        public DataSet PostQuery(string query)
        {
            DataSet result= new DataSet();
            SQLiteDataAdapter ada = new SQLiteDataAdapter(query, ConnectionInfo.DBSource);
                        
            ada.Fill(result);
            return result;
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
        public void PostNonQuery(string query)
        {
            SQLiteCommand sqlComm = new SQLiteCommand();
            sqlComm.Connection = sqlConn;
            sqlConn.Open();
            sqlComm.CommandText = query;
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
        }


        public DataSet PostQuery(string query, SQLiteParameter[] param)
        {
            DataSet result = new DataSet();
            SQLiteCommand sqlcomm = new SQLiteCommand(query, sqlConn);
            foreach (SQLiteParameter p in param)
            { sqlcomm.Parameters.Add(p); }
            SQLiteDataAdapter ada = new SQLiteDataAdapter(sqlcomm);
            ada.Fill(result);
            return result;
        }

        public DataSet PostQuery(string query, Hashtable param)
        {
            List<SQLiteParameter> sqlParam = new List<SQLiteParameter>();
            foreach (DictionaryEntry i in param)
            {
                SQLiteParameter sql = new SQLiteParameter(i.Key.ToString(), i.Value);
                sqlParam.Add(sql);
            }
            return PostQuery(query, sqlParam.ToArray());
        }


        public class SqlConnectionInformation
        {
            string dbPath;

            public string DBPath
            {
                get { return dbPath; }
                set { dbPath = value; }
            }

            internal string DBSource
            {
                get { return $"Data Source ={dbPath}"; }
            }

        }
    }
}
