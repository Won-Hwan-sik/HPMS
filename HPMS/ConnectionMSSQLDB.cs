using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HPMS
{
    public class ConnectionMSSQLDB
    {
        private static SqlConnection _conn { get; set; }
        private static SqlCommand _cmd { get; set; }
        private static SqlDependency _dependency { get; set; }
        public static SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
        private static List<SqlDependency> _sqlDependencies = new List<SqlDependency>();

        //public string DataSource { get; set; }          // 데이터베이스 서버 주소
        //public string InitialCatalog { get; set; }      // 데이터베이스 이름
        //public string UserID { get; set; }              // 데이터베이스 로그인 ID
        //public string Password { get; set; }            // 데이터베이스 로그인 암호

        /// <summary>
        /// SQL 문장에서 테이블 이름 추출하는 메서드 
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>

        private string GetTableNameFromSqlStatement(string sqlStatement)
        {
            sqlStatement = sqlStatement.ToUpper(); // 대문자로 변환
            sqlStatement = sqlStatement.Replace("[DBO].", string.Empty); // '[DBO].' 제거

            string pattern = string.Format(@"\[([\w\d_]+)\]");
            Match match = Regex.Match(sqlStatement, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string tableName = match.Groups[1].Value;
                tableName = tableName.TrimStart('[').TrimEnd(']');  // 테이블 이름에서 대괄호 []를 제거
                return tableName;
            }

            return null; // 테이블 이름을 찾지 못한 경우
        }

        public ConnectionMSSQLDB()
        {
            connStringBuilder = new SqlConnectionStringBuilder();
        }

        // 데이터베이스 연결
        public async Task ConnectSQLDBAsync()
        {
            connDBExec();                                  // 연결 문자열 생성 메소드 호출

            try
            {
                await _conn.OpenAsync();                    // 연결
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{ this.GetType().Name} {methodName} : {ex.Message}");

            }
        }

        // 데이터베이스 연결 상태 반환
        public bool IsConnected()
        {
            return _conn != null && _conn.State == ConnectionState.Open;
        }

        // 데이터베이스 연결을 실행
        private void connDBExec()
        {
            connStringBuilder = Common.connStringBuilder;

            try
            {
                _conn = new SqlConnection(connStringBuilder.ConnectionString);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{this.GetType().Name} {methodName} : {ex.Message}");
            }
        }

        // 데이터 조회를 비동기식으로 실행하고 결과를 DataSet으로 반환
        public async Task<DataSet> CommonLookUpRsAsync(string sqlScript, string flag)
        {
            DataSet pqtdsResult = null;

            if (!IsConnected())
            {
                return pqtdsResult;
            }

            try
            {
                using (_cmd = new SqlCommand(sqlScript, _conn))
                {
                    _cmd.Connection = _conn;
                    _cmd.CommandTimeout = 60;

                    using (SqlDataAdapter iDataAd = new SqlDataAdapter(_cmd))
                    {
                        pqtdsResult = new DataSet();
                        await Task.Run(() => iDataAd.Fill(pqtdsResult));        // 조회 쿼리 실행
                    }
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"[{this.GetType().Name} {methodName} : {ex.Message}");
            }

            return pqtdsResult;
        }

        // 데이터 삽입, 갱신, 삭제를 비동기식으로 실행하고 결과를 bool 값으로 반환
        public async Task<bool> CommonLookUpRsAsync(string sqlScript)
        {
            // 중복된 Primary Key 값이 발생한 경우 Update 작업 수행 (TXID가 PK 설정이 아닌경우 작성해야 함)
            Func<SqlException, string, Task> UpdateQueryAsync = async (ex, sqlStatement) =>
            {
                string txid = string.Empty;
                string updateScript = string.Empty;
                string tableName = GetTableNameFromSqlStatement(sqlStatement);
                string[] values = sqlStatement.Split(new[] { "],[", ",'", "([", "]) VALUES (", "')" }, StringSplitOptions.RemoveEmptyEntries);

                switch (tableName)
                {
                    case "T100_3001_02":
                        txid = values[6].Trim('\'');
                        string codeType = values[7].Trim('\'');
                        string code = values[9].Trim('\'');
                        string typeName = values[8].Trim('\'');
                        string codeName = values[10].Trim('\'');

                        updateScript = string.Format($@"
                            UPDATE [DBO].[{tableName}]
                            SET [TYPE_NAME] = '{typeName}'
                                , [CODE_NAME] = '{codeName}'
                                , [TXID] = '{txid}'
                            WHERE [CODE_TYPE] = '{codeType}'
                                AND [CODE] = '{code}';
                            "
                            );
                        break;
                    case "T400_4001_05":
                        string entnLkcd = values[17].Trim('\'');
                        string entn = values[18].Trim('\'');
                        string firmName = values[19].Trim('\'');
                        string entnType = values[20].Trim('\'');
                        string tranType = values[21].Trim('\'');
                        string trtmType = values[22].Trim('\'');
                        string busnRegn = values[23].Trim('\'');
                        string ceo = values[24].Trim('\'');
                        string offcTel = values[25].Trim('\'');
                        string e_mail = values[26].Trim('\'');
                        string cmptAuth = values[27].Trim('\'');
                        string offcZip = values[28].Trim('\'');
                        string offcAddr = values[29].Trim('\'');
                        string entnStat = values[values.Length - 1].Trim('\'');
                        txid = values[16].Trim('\'');

                        updateScript = string.Format($@"
                            UPDATE [DBO].[T400_4001_05]
                               SET [FIRM_NAME] = '{firmName}'
                                  ,[ENTN_TYPE] = '{entnType}'
                                  ,[TRAN_TYPE] = '{tranType}'
                                  ,[TRTM_TYPE] = '{trtmType}'
                                  ,[BUSN_REGN] = '{busnRegn}'
                                  ,[CEO] = '{ceo}'
                                  ,[OFFC_TEL] = '{offcTel}'
                                  ,[E_MAIL] = '{e_mail}'
                                  ,[CMPT_AUTH] = '{cmptAuth}'
                                  ,[OFFC_ZIP] = '{offcZip}'
                                  ,[OFFC_ADDR] = '{offcAddr}'
                                  ,[ENTN_STAT] = '{entnStat}'
                                  ,[TXID] = '{txid}'
                             WHERE [ENTN_LKCD] = '{entnLkcd}'
	                            AND [ENTN] = '{entn}'
                            "
                            );

                        break;
                    default:
                        break;
                }

                using (SqlCommand updateCmd = new SqlCommand(updateScript, _conn))
                {
                    await updateCmd.ExecuteNonQueryAsync();
                }
            };

            if (!IsConnected())
            {
                return false;
            }
            //            SqlTransaction tran = _conn.BeginTransaction();                 // 트랜잭션 시작

            try
            {
                /*
                // 묶여있는 스크립트
                using (_cmd = new SqlCommand(sqlScript, _conn))
                {
                    _cmd.Connection = _conn;
                    // _cmd.Transaction = tran;                                 // 현재사용할트랜잭션객체지정
                    _cmd.CommandTimeout = 60;
                    await _cmd.ExecuteNonQueryAsync();
                    // tran.Commit();                                          // 트랜잭션 commit
                }
                */

                // Action UpdateQuery 와 함께 사용 (묶여있던 스크립트를 분리하여 적용)
                string[] sqlStatements = sqlScript.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string sqlStatement in sqlStatements)
                {
                    using (SqlCommand _cmd = new SqlCommand(sqlStatement, _conn))
                    {
                        _cmd.Connection = _conn;
                        _cmd.CommandTimeout = 60;

                        try
                        {
                            await _cmd.ExecuteNonQueryAsync();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 2627 || ex.Number == 2601) // Unique Key Violation
                                await UpdateQueryAsync(ex, sqlStatement);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //                tran.Rollback();                                            // 에러발생시 rollback

                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{this.GetType().Name} {methodName} : {ex.Message}");

                return false;
            }

            return true;
        }

        // 클래스의 리소스를 해제하는 Dispose 메소드
        public void Dispose()
        {
            try
            {
                if (IsConnected())
                {
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"[{this.GetType().Name} {methodName} : {ex.Message}");
            }
        }

        #region ■ dependency 테스트
        public async Task<bool> ProcSqlDenpendency(string sqlScript)
        {
            if (!IsConnected())
            {
                return false;
            }

            SqlDependency.Stop(connStringBuilder.ConnectionString);        // SQL Dependency 종료
            SqlDependency.Start(connStringBuilder.ConnectionString);       // SQL Dependency 시작

            try
            {
                using (_cmd = new SqlCommand(sqlScript, _conn))
                {
                    _cmd.Notification = null;
                    _dependency = new SqlDependency(_cmd);
                    _dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
                    _sqlDependencies.Add(_dependency);

                    await _cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{this.GetType().Name} {methodName} : {ex.Message}");
                return false;
            }

            return true;
        }

        private static void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            //            MessageBox.Show($"DB 연결");
        }

        public async Task SqlSelects()
        {
            await ProcSqlDenpendency("SELECT * FROM [dbo].[COMPANY_INFO];");
            await ProcSqlDenpendency("SELECT * FROM [dbo].[T200_5001_01];");
            await ProcSqlDenpendency("SELECT * FROM [dbo].[T200_5001_02];");
            await ProcSqlDenpendency("SELECT * FROM [dbo].[T400_5001_01];");
            await ProcSqlDenpendency("SELECT * FROM [dbo].[T400_5001_02];");
        }

        public void StopSqlDenpendency()
        {
            SqlDependency.Stop(connStringBuilder.ConnectionString);

            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }

            if (_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }

            foreach (var dependency in _sqlDependencies)
            {
                if (_dependency != null)
                {
                    _dependency.OnChange -= OnDependencyChange;
                    _dependency = null;
                }
            }

            SqlDependency.Stop(connStringBuilder.ConnectionString);
        }

        #endregion ■ dependency 테스트

    }
}
