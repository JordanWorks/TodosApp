using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ToDoApp.Services
{
    public class TodoServices
    {
        internal static string parseJson(JObject jsonData, string v)
        {
            if((string)jsonData[v] != null)
            {
                return (string)jsonData[v];
            }
            return null;
        }
    }
}