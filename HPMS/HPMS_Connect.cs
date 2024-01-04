using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HPMS
{
    public class HPMS_Connect
    {
        public async Task<Dictionary<string, object>> Post<T>(string sID, string sPassword, string sInterfaceID, T tModule)
        {
            var dict = new Dictionary<string, object>();

            try
            {
                // 서버에 요청하기 POST 방식
                // HttpClient 및 HttpRequestMessage 객체 생성, URL 생성
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(180);     //응답시간 조정 기본 100초
                string sQuery = Common.URL + "/select" + sInterfaceID;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, sQuery);

                // Header Setup
                request.Headers.Add("accept", "application/json");

                string sInput = sID + ":" + sPassword;
                byte[] byInput = System.Text.Encoding.UTF8.GetBytes(sInput);
                string sEncode64 = Convert.ToBase64String(byInput);

                request.Headers.Add("Authorization", "Basic " + sEncode64);

                // Serialize (Object -> JsonString)
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string sJsonString = jss.Serialize(Common.requestBody);

                // Body에 Json 메세지 셋팅
                request.Content = new StringContent(sJsonString);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 응답 가져오기
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize (Jsonstring -> Object)
                dict = jss.Deserialize<Dictionary<string, object>>(responseBody);

            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(System.Diagnostics.EventLogEntryType.Error, $"{sInterfaceID} {methodName} : {ex.Message}");

            }
            return dict;
        }
        public async Task<Dictionary<string, object>> Post(string sInterfaceID)
        {
            var dict = new Dictionary<string, object>();

            try
            {
                // 서버에 요청하기 POST 방식
                // HttpClient 및 HttpRequestMessage 객체 생성, URL 생성
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(180);     //응답시간 조정 기본 100초
                string sQuery = Common.URL + "/select" + sInterfaceID;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, sQuery);

                // Header Setup
                request.Headers.Add("accept", "application/json");

                string sInput = Common.ENTN_LKCD + ":" + Common.PASSWORD;
                byte[] byInput = System.Text.Encoding.UTF8.GetBytes(sInput);
                string sEncode64 = Convert.ToBase64String(byInput);

                request.Headers.Add("Authorization", "Basic " + sEncode64);

                // Serialize (Object -> JsonString)
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string sJsonString = jss.Serialize(Common.requestBody);

                // Body에 Json 메세지 셋팅
                request.Content = new StringContent(sJsonString);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 응답 가져오기
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize (Jsonstring -> Object)
                dict = jss.Deserialize<Dictionary<string, object>>(responseBody);

            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                GlobalLog.LogEvent(EventLogEntryType.Error, $"{sInterfaceID} {methodName} : {ex.Message}");

            }
            return dict;
        }

    }
}
