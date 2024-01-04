using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace HPMS
{
    public class Common
    {
        public static ConnectionMSSQLDB AllbaroDB_conn { get; set; }
        public static RequestBody requestBody { get; set; }
        public static SqlConnectionStringBuilder connStringBuilder { get; set; }
        public static string connectionString { get; set; }
        public static string API_CERT_KEY { get; set; }
        public static string ENTN_LKCD { get; set; }
        public static string PASSWORD { get; set; }
        public static string URL { get; set; }

        /// <summary>
        /// DB Server 연결
        /// </summary>
        public static async Task DbExecute(string sqlScript)
        {
            try
            {
                // 데이터베이스 연결
                await AllbaroDB_conn.ConnectSQLDBAsync();
                if (AllbaroDB_conn.IsConnected())
                {
                    await AllbaroDB_conn.CommonLookUpRsAsync(sqlScript);        // SQL 쿼리 실행
                }

                AllbaroDB_conn.Dispose();
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{methodName} : {ex.Message}");
            }

        }
        public static async Task<DataSet> DbExecute(string sqlScript, string flag)
        {
            DataSet _ds = new DataSet();

            try
            {
                await AllbaroDB_conn.ConnectSQLDBAsync();
                if (AllbaroDB_conn.IsConnected())
                {
                    _ds = await AllbaroDB_conn.CommonLookUpRsAsync(sqlScript, flag);
                }

                AllbaroDB_conn.Dispose();
            }
            catch (Exception ex)
            {
                // 에러 발생 시 로그 기록하고 null 반환
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{methodName} : {ex.Message}");
                _ds = null;
            }
            return _ds;

        }

        /// <summary>
        /// AES 암호화
        /// </summary>
        /// <param name="data">암호화 하려는 문자 Json 데이터</param>
        /// <param name="inKey">키/param>
        /// <param name="inIV">vector (IV).</param>
        /// <returns>암호화된 문자열</returns>
        public static async Task<string> AES_Encrypt(string data, string inKey, string inIV)
        {
            byte[] Key = Convert.FromBase64String(inKey);
            byte[] IV = Convert.FromBase64String(inIV);

            // 인자 검사
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));

            byte[] encrypted;

            // AesCryptoServiceProvider 객체 생성
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // 암호화 변환기 생성
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // 암호화를 위한 메모리 스트림 생성
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // 데이터를 스트림에 쓰기
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // 암호화된 데이터를 base64 문자열로 변환하여 반환
            return Convert.ToBase64String(encrypted);
        }



    }

    /// <summary>
    /// 공통 RequestBody
    /// </summary>
    public class RequestBody
    {
        public string api_cert_key { get; set; }        // 인증키
        public string entn_lkcd { get; set; }           // 연계업체코드
        public string manf_nums { get; set; }           // 인계번호
        public string req_type { get; set; }            // 요청구분
        public string page_no { get; set; }             // 페이지번호
        public string period_from_date { get; set; }    // 기간시작일
        public string period_to_date { get; set; }      // 기간종료일
        public string subcd_include_yn { get; set; }    // 하위업체포함여부
    }

    /// <summary>
    /// 공통 RESPONSE_BODY
    /// </summary>
    public class ResponseMessage : Common
    {
        public string ifid { get; set; }                // 연계인터페이스ID
        public string txid { get; set; }                // 트랜젝션ID
        public string resultCode { get; set; }          // 연계결과코드
        public string resultMessage { get; set; }       // 연계결과메시지
        public object errorMessage { get; set; }        // 오류메시지
        public object totalPageNo { get; set; }         // 총페이지번호

        // RESPONSE Message에 등록
        public async Task<string> SetResponseMessage(Dictionary<string, object> dict)
        {
            // Dictionary Key가 있는지 파악
            if (!dict.ContainsKey("totalPageNo")) dict.Add("totalPageNo", null);

            // Class 에 등록
            ResponseMessage responseMessage = new ResponseMessage()
            {
                ifid = dict["ifid"]?.ToString() ?? string.Empty,    // null 값 처리 
                txid = dict["txid"]?.ToString() ?? string.Empty,
                resultCode = dict["resultCode"]?.ToString() ?? string.Empty,
                resultMessage = dict["resultMessage"]?.ToString() ?? string.Empty,
                errorMessage = dict["errorMessage"]?.ToString() ?? string.Empty,
                totalPageNo = dict["totalPageNo"]?.ToString() ?? string.Empty,
            };

            string sqlScript = string.Empty;

            sqlScript = string.Format(@"
                INSERT INTO [DBO].[RESPONSE_MESSAGE]
                           (
                            [LFID]
                           ,[TXID]
                           ,[RESULTCODE]
                           ,[RESULTMESSAGE]
                           ,[ERRORMESSAGE]
                           ,[TOTALPAGENO]
                            )
                     VALUES
                           (
                             '{0}'
                           , '{1}'
                           , '{2}'
                           , '{3}'
                           , '{4}'
                           , '{5}'
                           );
                    "
                , responseMessage.ifid
                , responseMessage.txid
                , responseMessage.resultCode
                , responseMessage.resultMessage
                , responseMessage.errorMessage
                , responseMessage.totalPageNo
                );

            await DbExecute(sqlScript);                      // DB 실행

            return responseMessage.txid;

        }

    }


/// <summary>
/// 로그를 관리하기 위한 클래스
/// </summary>
    public static class GlobalLog
    {
        public static Form1 obj;

        // 로그 레벨과 메시지를 인자로 받아서 로그 이벤트 발생
        public static void LogEvent(EventLogEntryType eLevel, string msg)
        {
            LogData(eLevel, msg);
        }

        #region Log OverLoading
        // 로그 레벨과 로그 메시지를 인자로 받아서 현재 시간 정보와 함께 로그 작성
        private static void Log(EventLogEntryType eLevel, string LogDesc)
        {
            string LogInfo = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{eLevel.ToString()}] {LogDesc}";

            obj.lboxLog.Items.Insert(0, LogInfo);           // 새로운 LogInfo를 맨 위에 추가
            // lboxLog의 아이템 갯수가 1000을 초과하면 가장 오래된 아이템(맨 아래의 아이템) 삭제
            while (obj.lboxLog.Items.Count > 1000)
            {
                obj.lboxLog.Items.RemoveAt(obj.lboxLog.Items.Count - 1); // 맨 아래의 아이템 삭제
            }

            WriteEventLogEntry(eLevel, LogInfo);
        }
        // 로그 레벨, 로그 메시지, 로그 작성 시간을 인자로 받아서 로그 작성
        private static void Log(DateTime dTime, EventLogEntryType eLevel, string LogDesc)
        {
            string LogInfo = $"{dTime.ToString("yyyy-MM-dd HH:mm:ss")} [{eLevel.ToString()}] {LogDesc}";

            obj.lboxLog.Items.Insert(0, LogInfo);           // 새로운 LogInfo를 맨 위에 추가
            // lboxLog의 아이템 갯수가 1000을 초과하면 가장 오래된 아이템(맨 아래의 아이템) 삭제
            while (obj.lboxLog.Items.Count > 1000)
            {
                obj.lboxLog.Items.RemoveAt(obj.lboxLog.Items.Count - 1); // 맨 아래의 아이템 삭제
            }

            WriteEventLogEntry(eLevel, LogInfo);
        }
        // 로그 레벨과 로그 데이터를 인자로 받아 콜백 함수를 이용해 스레드 세이프하게 로그 작성
        public static void LogData(EventLogEntryType enLog, string strData)
        {
            obj.Invoke(new Action(delegate ()
            {
                Log(enLog, strData);
            }));
        }
        // 윈도우 OS의 이벤트뷰어에 로그 저장
        private static void WriteEventLogEntry(EventLogEntryType enLog, string message)
        {
            EventLog eventLog = new EventLog();         // EventLog 인스턴스 생성

            if (!EventLog.SourceExists("Allbaro"))      // 이벤트 소스가 존재하는지 확인하고 없으면 생성
            {
                EventLog.CreateEventSource("Allbaro", "Allbaro");
            }

            eventLog.Source = "Allbaro";                // 로그 항목을 작성하기 위해 소스 이름 설정.

            int eventID = 1024;                         // 이벤트 로그에 추가할 이벤트 ID 생성.

            eventLog.WriteEntry(message,                // 이벤트 로그에 항목 작성.
                                enLog,
                                eventID);

            eventLog.Close();                           // Event Log을 닫습니다.
        }
        #endregion Log OverLoading

    }

}
