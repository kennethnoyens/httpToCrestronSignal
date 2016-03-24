using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net;

namespace hts
{
    public class ContentVariables
    {
        Dictionary<string, string> variables;

        public ContentVariables(String httpContent)
        {
            variables = new Dictionary<string, string>();
            httpContentChanged(httpContent);
        }

        public void httpContent(string httpContent)
        {
            httpContentChanged(httpContent);
        }

        private void httpContentChanged(string httpContent)
        {
            if (httpContent.Length == 0)
                return;

            String[] contentVariables;
            contentVariables = httpContent.Split('&');

            foreach (string var in contentVariables)
            {
                if (var.Length > 0)
                {
                    if (var.Contains("="))
                    {
                        String[] oneVariable = var.Split('=');
                        if (oneVariable[1].Length > 0)
                            variables.Add(oneVariable[0], HttpUtility.UrlDecode(oneVariable[1]));
                    }
                }

            }
        }

        public string getVariable(string name)
        {
            string value;
            if (variables.TryGetValue(name, out value))
                return value;
            else
                return "";
        }

        public bool variableExists(string name)
        {
            string value;
            return variables.TryGetValue(name, out value);
        }
    }
}