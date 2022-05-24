using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class IVAOAtisLib
{
    private const string IVAO_ATIS_ENDPOINT = "https://api.ivao.aero/v2/tracker/whazzup/atis";
    public static async Task<string> GetAtisRaw()
    {
        string response = await Http_GetRequest(IVAO_ATIS_ENDPOINT);
        return response;
    }
    public static List<Atis> GetAtisList()
    {
        string response = GetAtisRaw().Result;
        List<Atis> resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Atis>>(response);
        //Fixing Data
        foreach (Atis a in resultList)
        {
            //Inserting ICAO Code for filter
            a.ICAOCode = a.Callsign.Split('_')[0];
            //Single string ATIS with newlines (to not use array)
            foreach (string s in a.ArrayValue)
                a.Value += $"{s}\n";
            //Fixing Timestamp
            a.Timestamp = a.TimestampZulu.ToLocalTime();
        }
        return resultList;
    }
    public static Atis GetAtis(string ICAO)
    {
        string response = GetAtisRaw().Result;
        List<Atis> resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Atis>>(response);
        //Fixing Data
        foreach(Atis a in resultList)
        {
            //Inserting ICAO Code for filter
            a.ICAOCode = a.Callsign.Split('_')[0];
            //Single string ATIS with newlines (to not use array)
            foreach (string s in a.ArrayValue)
                a.Value += $"{s}\n";
            //Fixing Timestamp
            a.Timestamp = a.TimestampZulu.ToLocalTime();
        }
        //Searching for data
        Atis result = null;
        foreach (Atis a in resultList)
            if (a.ICAOCode == ICAO.ToUpper())
                result = a;
        //Fixing tower not online (no ATIS)
        if (result == null)
            result = new Atis("Tower is not online on IVAO!", ICAO, "Not available");
        return result;   
    }
    private static async Task<string> Http_GetRequest(string URI)
    {
        string result;
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URI);
        using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)await request.GetResponseAsync())
        using (System.IO.Stream stream = response.GetResponseStream())
        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            result = await reader.ReadToEndAsync();
        return result;
    }
    public class Atis
    {
        public string ICAOCode;
        [Newtonsoft.Json.JsonProperty("lines")]
        internal string[] ArrayValue;
        [Newtonsoft.Json.JsonProperty("callsign")]
        public string Callsign;
        [Newtonsoft.Json.JsonProperty("revision")]
        public string Revision;
        public DateTime Timestamp;
        [Newtonsoft.Json.JsonProperty("timestamp")]
        public DateTime TimestampZulu;
        public string Value;

        public Atis(string v, string icao, string r)
        {
            Value = v; ICAOCode = icao; Revision = r;
        }
    }
}