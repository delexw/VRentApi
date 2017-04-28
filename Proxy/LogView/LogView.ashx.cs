using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CF.VRent.Common;
using System.IO;
using System.Configuration;

namespace Proxy.LogView
{
    /// <summary>
    /// Summary description for LogView1
    /// </summary>
    public class LogView1 : IHttpHandler
    {
        private static List<LogViewEntity> logs = new List<LogViewEntity>();
        private static string filePath = "";

        public void ProcessRequest(HttpContext context)
        {
            if (ConfigurationManager.AppSettings["EnableLogView"].ToBool())
            {
                if (context.Request.Params["action"] == "getTyppe")
                {
                    List<object> types = new List<object>();

                    var defaultlogConfig = LogInfor.DefaultLogWriter.Configuration;
                    var id = 1;
                    foreach (var lisener in defaultlogConfig.Listeners)
                    {
                        types.Add(new { id = id, text = lisener.OtherAttributes["folder"] + lisener.OtherAttributes["fileName"] });
                        id++;
                    }

                    var emailConfig = LogInfor.EmailLogWriter.Configuration;
                    foreach (var lisener in emailConfig.Listeners)
                    {
                        types.Add(new { id = id, text = lisener.OtherAttributes["folder"] + lisener.OtherAttributes["fileName"] });
                        id++;
                    }
                    context.Response.ContentType = "application/json";
                    context.Response.Write(types.ObjectToJson());
                    context.Response.End();
                }

                if (context.Request.Params["action"] == "getLog")
                {
                    var returnRows = new object();
                    var pageRows = context.Request.Params["rows"].ToInt();
                    var pageRowsStart = (context.Request.Params["page"].ToInt() - 1) * context.Request.Params["rows"].ToInt();

                    if (logs.Count > 0 && filePath == context.Request.Params["file"])
                    {
                        if (!String.IsNullOrWhiteSpace(context.Request.Params["type"]))
                        {
                            var filterRows = logs.Where(r => r.type.ToStr() == context.Request.Params["type"]).ToList();
                            returnRows = new
                            {
                                total = logs.Count,
                                rows = filterRows
                                    .Skip(pageRowsStart).Take(pageRows)
                            };
                        }
                        else
                        {
                            returnRows = new
                            {
                                total = logs.Count,
                                rows = logs
                                    .Skip(pageRowsStart).Take(pageRows)
                            };
                        }
                    }
                    else if (File.Exists(context.Request.Params["file"]))
                    {
                        var reader = File.OpenRead(context.Request.Params["file"]);
                        StreamReader sr = new StreamReader(reader);
                        logs = new List<LogViewEntity>();
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var lineSplit = line.Split(';');
                            DateTime t = new DateTime();
                            if (DateTime.TryParse(lineSplit[0], out t))
                            {
                                logs.Add(new LogViewEntity() { time = lineSplit[0].Replace("\t", ""), type = lineSplit[1].Replace("\t", ""), title = lineSplit[2].Replace("\t", ""), content = lineSplit[4].Replace("\t", "") });
                            }
                            else
                            {
                                logs.Add(new LogViewEntity() { time = "", type = "", title = "", content = lineSplit[0].Replace("\t", "") });
                            }
                        }

                        returnRows = new
                        {
                            total = logs.Count,
                            rows = logs
                                .Skip(pageRowsStart).Take(pageRows)
                        };
                        filePath = context.Request.Params["file"];
                    }

                    context.Response.ContentType = "application/json";
                    context.Response.Write(returnRows.ObjectToJson());
                    context.Response.End();
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class LogViewEntity
    {
        public string time { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }
}