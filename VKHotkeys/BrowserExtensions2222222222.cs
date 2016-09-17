using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using System.Diagnostics;
using System.Threading;

namespace VKHotkeys
{
    public static class BrowserExtensions
    {
        //Browser browser = new IE();

        public static void RunScriptIgnoreEx(this IE bro, string script)
        {
            try
            {
                bro.RunScript(script);
            }
            catch
            {
                Thread.Sleep(500);
            }
        }

        public static void WaitForAjaxRequest(this WatiN.Core.Browser browser)
        {
            int timeWaitedInMilliseconds = 0;
            var maxWaitTimeInMilliseconds = Settings.WaitForCompleteTimeOut * 1000;

            while (browser.IsAjaxRequestInProgress()
                    && timeWaitedInMilliseconds < maxWaitTimeInMilliseconds)
            {
                Thread.Sleep(Settings.SleepTime);
                timeWaitedInMilliseconds += Settings.SleepTime;
            }
        }

        public static bool IsAjaxRequestInProgress(this WatiN.Core.Browser browser)
        {
            var evalResult = browser.Eval("watinAjaxMonitor.isRequestInProgress()");
            return evalResult == "true";
        }

        public static void InjectAjaxMonitor(this WatiN.Core.Browser browser)
        {
            const string jquerry = @"var element1 = document.createElement('script');element1.src = 'http://ajax.googleapis.com/ajax/libs/jquery/1.8.0/jquery.min.js';element1.type='text/javascript';document.getElementsByTagName('head')[0].appendChild(element1);";
            browser.Eval(jquerry);

            const string monitorScript =
                @"function AjaxMonitor(){"
                + "var ajaxRequestCount = 0;"

                + "$(document).ajaxSend(function(){"
                + "    ajaxRequestCount++;"
                + "});"

                + "$(document).ajaxComplete(function(){"
                + "    ajaxRequestCount--;"
                + "});"

                + "this.isRequestInProgress = function(){"
                + "    return (ajaxRequestCount > 0);"
                + "};"
                + "}"

                + "var watinAjaxMonitor = new AjaxMonitor();";

            browser.Eval(monitorScript);
        }
    }
}
