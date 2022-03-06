using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Headquarters
{
    class Script
    {
        public static class ReservedParameterName
        {
            public const string Session = "session";
        }

        public string name => Path.GetFileNameWithoutExtension(filepath);
        public List<string> paramNames { get; protected set; }
        public string explainString { get; protected set; }


        readonly string filepath;
        PowerShellScript psScript;

        public Script(string filepath)
        {
            this.filepath = filepath;
            Load();
        }

        public void Load()
        {
            if (File.Exists(filepath))
            {
                var script = File.ReadAllText(filepath);

                psScript = new PowerShellScript(name, script);

                paramNames = SearchParameters(script);

                // 説明テキストの抽出
                var expMatch = Regex.Match(script, @"(?<=#EXPLAIN )(.*)");
                explainString = "";
                while (expMatch.Success)
                {
                    explainString += expMatch.Value;
                    expMatch=expMatch.NextMatch();
                }

            }
        }

        List<string> SearchParameters(string script)
        {

            var match = Regex.Match(script, @"(?<=param\().*?(?=\))");
            return match.Value
                .Replace("$", "")
                .Replace(" ", "")
                .Split(',')
                .Where(str => string.Compare(str, ReservedParameterName.Session, true) != 0 && string.Compare(str, "IPList", true) != 0)
                .ToList();
        }

        public PowerShellScript.Result Run(string ipAddress, PowerShellScript.InvokeParameter param, PSObject ipList = null)
        {
            PowerShellScript.Result result;


            param.parameters.Add("IPList", ipList);

            // スクリプト引数に $noSessionがあれば、セッション接続をせず実行できる
            if (paramNames.Any(paramNames => paramNames.ToLower() == "nosession"))
            {
                result = psScript.Invoke(param);
                return result;
            }

            param.parameters.TryGetValue(ParameterManager.SpecialParamName.UserName, out var userName);
            param.parameters.TryGetValue(ParameterManager.SpecialParamName.UserPassword, out var userPassword);

            var sessionResult = SessionManager.Instance.CreateSession(ipAddress, (string)userName, (string)userPassword, param);
            var session = sessionResult.objs.FirstOrDefault()?.BaseObject;
            if (session == null)
            {
                result = sessionResult;
            }
            else
            {
                param.parameters.Add(ReservedParameterName.Session, session);
                result = psScript.Invoke(param);
            }

            return result;
        }
    }
}
