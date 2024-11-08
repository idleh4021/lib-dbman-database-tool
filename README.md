# DBMan
<!--![배지 또는 로고 이미지 (선택사항)](링크)-->
<!--프로젝트에 대한 간단한 설명을 여기에 작성합니다.-->
DB 연결/제어를 손쉽게 처리하기 위해 제작된 **DB 통합연결 라이브러리** 입니다.

## 목차
- [소개](#소개)<!--- [설치](#설치)-->
- [설치](#설치)
- [사용법](#사용법)
- [예제](#예제)
<!--- [기여](#기여)
- [라이선스](#라이선스)
- [문의](#문의)
-->
## 소개
<!--프로젝트에 대한 자세한 설명을 여기에 작성합니다.  -->
- **개발환경** : C#, .Net FrameWork 4.0
- **DB 리스트** : 
  
    ![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
    ![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)
    ![SQLite](https://img.shields.io/badge/sqlite-%2307405e.svg?style=for-the-badge&logo=sqlite&logoColor=white)
    ![MySQL](https://img.shields.io/badge/mysql-4479A1.svg?style=for-the-badge&logo=mysql&logoColor=white)
    ![MariaDB](https://img.shields.io/badge/MariaDB-003545?style=for-the-badge&logo=mariadb&logoColor=white)

 
- **주요기능** : DB 연결 체크, 쿼리 실행, 프로시저 실행 등
```
    ConnectionCheck();
    PostQuery(query);
    PostQuery(string query, Hashtable param);
    PostProcedure(string procedure_title, Hashtable hs_param);
    BulkCopy(DataTable dt, string table);
```

## 설치

특정 DB들은 아래 Nuget패키지를 필요로 합니다.
1. ![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white) - Npgsql (4.0.17)

         Install-Package Npgsql -Version 4.0.17
   
2. ![SQLite](https://img.shields.io/badge/sqlite-%2307405e.svg?style=for-the-badge&logo=sqlite&logoColor=white) - System.Data.SQLite
   
       Install-Package System.Data.SQLite

3. ![MySQL](https://img.shields.io/badge/mysql-4479A1.svg?style=for-the-badge&logo=mysql&logoColor=white)
    ![MariaDB](https://img.shields.io/badge/MariaDB-003545?style=for-the-badge&logo=mariadb&logoColor=white)- MySqlConnector(2.1.13)

       Install-package MySqlConnector -Version 2.1.13

## 사용법
1. DBMan 참조추가
   <!--* PostGreSql을 사용하는경우 Nuget에서 NpgSql V4.0.17 버전을 설치해야합니다.-->
3. DB에 맞는 인스턴스 생성
   ```
   //포트없음
   DBMan.MSSQL sql=new DBMan.MSSQL("192.168.0.20", "DB_Test","sa", "pwd1234");
   
   //포트가있는경우
   DBMan.MSSQL sql=new DBMan.MSSQL("192.168.0.20",1433, "DB_Test","sa", "pwd1234");
   
   //연결정보 없이 인스턴스 생성 -- *사용전 반드시 연결정보를 설정해야함*
   DBMan.MSSQL sql=new DBMan.MSSQL();
   .
   .
   .
   sql.ConnectionInfo.Ip = "192.168.0.20";
   sql.ConnectionInfo.Port = 1433;
   .
   .
   .
   sql.InitSqlConn();
   ```
4. 연결 체크
   ```
   bool connectStatus = sql.ConnectionInfo();
   ```
5. 쿼리 or 프로시저 실행
   ```
   //쿼리에 파라미터가 필요한 경우
   DataSet ds = sql.PostQuery(sb.ToString(), hs_param);

   //파라미터 없음
   DataSet ds = sql.PostQuery(sb.ToString());

   //프로시저
   DataSet ds = PostProcedure(procedure_title,hs_param)
   ```

 ## 예제
 
<details>
  <summary>MS-SQL , MySql | MariaDB </summary>
        
    ```
    //전역변수
    //MSSQL
     DBMan.MSSQL sql = new DBMan.MSSQL("192.168.0.123", "DB_TEST", "sa", "pwd1234");
    //MySQL 
     DBMan.MySQL sql = new DBMan.MySQL("192.168.0.123", "DB_TEST", "sa", "pwd1234");
    .
    .
    .
    
      private DataSet SearchFromQueryParam()
        {
           //쿼리에 파라미터가 필요한 경우
            if (!sql.ConnectionCheck())
            {
                MessageBox.Show("DB 접속 실패");
                return null;
            }
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM TB_TMP");
            query.AppendLine(" WHERE ID = @ID");

            Hashtable param = new Hashtable();
            param.Add("@ID", "IDValue");
            DataSet ds = sql.PostQuery(query.ToString(), param);
            return ds;
        }

        private DataSet SearchFromQuery()
        {
           //쿼리실행
            if (!sql.ConnectionCheck())
            {
                MessageBox.Show("DB 접속 실패");
                return null;
            }
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM TB_TMP");
            query.AppendLine(" WHERE ID = @ID");

            DataSet ds = sql.PostQuery(query.ToString());
            return ds;
        }

        private DataSet SearchFromProcedure()
        {
            //프로시저 실행
            if (!sql.ConnectionCheck())
            {
                MessageBox.Show("DB 접속 실패");
                return null;
            }
            Hashtable param = new Hashtable();
            param.Add("@ID", "12344");

            DataSet ds = sql.PostProcedure("SP_Pro_01",param);
            return ds;
        }
    ```
</details>

<details>
    <summary>PostgreSQL</summary>
    
```
    //전역변수
   DBMan.PostgreSQL postgre = new DBMan.PostgreSQL("192.168.0.226","DB_TEST","postgres","1234");
    .
    .
    .

    private DataSet SearchFromQuery()
        {
            if (!postgre.ConnectionCheck()){ MessageBox.Show("접속오류"); return null; }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select * from tb_test where id = @id");
            Hashtable hs = new Hashtable();
            hs.Add("@id", 1);
            DataSet result = postgre.PostQuery(sb.ToString(), hs);
            return result;
        }
```
    
</details>

<details>
    <summary>SQLite</summary>

```
    private DataSet SearchFromQuery()
        {
           string query =  "select * from tb_test where id = @id";
            DBMan.SQLite sql = new DBMan.SQLite("C:\Users\UserName\Desktop\DB_TEST.db");
            
            Hashtable hs = new Hashtable();
            hs.Add("@id", 2);
            //DataSet ds = sql.PostQuery(query);
            DataSet ds = sql.PostQuery(query, hs);
            
            return ds;
        }
```
    
</details>
<!--프로젝트가 해결하려는 문제점이나 프로젝트의 목표를 적어 주는 것도 좋습니다.-->
<!--
## 설치
프로젝트를 로컬 환경에 설치하는 방법을 안내합니다. 예시:


# 1. 저장소 클론
git clone https://github.com/사용자명/프로젝트명.git

# 2. 프로젝트 디렉토리로 이동
cd 프로젝트명

# 3. 필요한 라이브러리 설치 (예: Python, Node.js 등 프로젝트에 따라 다름)
pip install -r requirements.txt   # Python 예시
npm install                       # Node.js 예시

# 기여-->
