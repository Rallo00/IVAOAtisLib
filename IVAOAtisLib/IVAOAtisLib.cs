using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class IVAOAtisLib
{
    private const string IVAO_ATIS_ENDPOINT = "https://api.ivao.aero/v2/tracker/whazzup/atis";
    /// <summary>
    /// Get all ATIS available from online towers on IVAO.
    /// </summary>
    /// <returns>Returns a raw Json string.</returns>
    public static async Task<string> GetAtisRaw()
    {
        string response = await Http_GetRequest(IVAO_ATIS_ENDPOINT);
        return response;
    }
    /// <summary>
    /// Get a list of all available ATIS divided by ICAO code
    /// </summary>
    /// <returns>Returns an ATIS object containing all information regarding the ATIS for each ICAO station</returns>
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
            //Fix Revision
            a.Revision = ConvertAtisRevision(a.Revision);
        }
        return resultList;
    }
    /// <summary>
    /// Get a specified ATIS from all available ATIS on IVAO
    /// </summary>
    /// <param name="ICAO">ICAO code of the desidered station</param>
    /// <returns>Returns an ATIS object containing all information regarding the requested ICAO station ATIS.</returns>
    public static async Task<Atis> GetAtis(string ICAO)
    {
        string response = await GetAtisRaw();
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
            //Fix Revision
            a.Revision = ConvertAtisRevision(a.Revision);
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
    private static string ConvertAtisRevision(string revision)
    {
        if (revision != null)
        {
            switch (revision)
            {
                case "A": return "Alpha";
                case "B": return "Bravo";
                case "C": return "Charlie";
                case "D": return "Delta";
                case "E": return "Echo";
                case "F": return "Foxtrot";
                case "G": return "Golf";
                case "H": return "Hotel";
                case "I": return "India";
                case "J": return "Juliett";
                case "K": return "Kilo";
                case "L": return "Lima";
                case "M": return "Mike";
                case "N": return "November";
                case "O": return "Oscar";
                case "P": return "Papa";
                case "Q": return "Quebec";
                case "R": return "Romeo";
                case "S": return "Sierra";
                case "T": return "Tango";
                case "U": return "Uniform";
                case "V": return "Victr";
                case "W": return "Whiskey";
                case "X": return "X-Ray";
                case "Y": return "Yankee";
                case "Z": return "Zulu";
                default: return "Not available";
            }
        }
        else return "Not available";
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