using System;
using System.Text;
using Crestron.SimplSharp;      // For Basic SIMPL# Classes
using Crestron.SimplSharp.Net.Http;

namespace hts
{
    public class httpToSignal
    {
        public delegate void digitalToSimplPlus(ushort joinNumber, SimplSharpString action);
        public delegate void stringToSimplPlus(ushort joinNumber, SimplSharpString text);

        HttpServer server;
        public digitalToSimplPlus digitalToPlus
        {
            get;
            set;
        }
        public stringToSimplPlus stringToPlus
        {
            get;
            set;
        }

        public httpToSignal()
        {
            server = new HttpServer();
            server.OnHttpRequest += new OnHttpRequestHandler(server_OnHttpRequest);
        }

        public void startListening(int portNumber)
        {
            try
            {
                server.Port = portNumber;
                server.Open();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Unable to open http server for httpToSignal service (" + e.ToString() + ")");
            }
        }

        protected void printDebugLine(string debugText)
        {
            CrestronConsole.PrintLine("httpToSignal service: " + debugText);
        }

        void server_OnHttpRequest(object sender, OnHttpRequestArgs e)
        {
            if (!e.Request.HasContentLength)
            {
                printDebugLine("http request received from " + e.Request.DataConnection.RemoteEndPointAddress + " without content. (so I have no idea what to do with this!)");
                return;
            }

            if (e.Request.Path.ToLower() != "/signalprocessor.html")
            {
                printDebugLine("http request received from " + e.Request.DataConnection.RemoteEndPointAddress + " for page " + e.Request.Path.ToLower() + " but I only have signalprocessor.html implemented");
                return;
            }

            CrestronConsole.PrintLine("Incoming HTTP request from {0} requesting {1} with content: {2}", e.Request.DataConnection.RemoteEndPointAddress, e.Request.Path.ToUpper(), e.Request.ContentString);

            e.Response.KeepAlive = false;

            // Get all the variables from the request. (var=value&var2=value2)
            ContentVariables vars = new ContentVariables(e.Request.ContentString);

            if (vars.variableExists("digitaljoin") && vars.variableExists("digitalaction"))
            {
                if (digitalToPlus != null)
                    digitalToPlus(Convert.ToUInt16(vars.getVariable("digitaljoin")), vars.getVariable("digitalaction"));
                else
                    printDebugLine("Delegate digitaljoin not set!");
            }

            if (vars.variableExists("stringjoin") && vars.variableExists("text"))
            {
                if(stringToPlus != null)
                    stringToPlus(Convert.ToUInt16(vars.getVariable("stringjoin")), vars.getVariable("text"));
                else
                    printDebugLine("Delegate digitaljoin not set!");
            }
        }

    }
}
