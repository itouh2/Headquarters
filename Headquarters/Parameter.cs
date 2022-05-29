using System.Diagnostics;

namespace Headquarters
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value
        {
            get { return IsDependIP ? "On IP List" : ParameterManager.Instance.Get(Name)?.ToString(); }
            set
            {
                if (!IsDependIP)
                    ParameterManager.Instance.Set(Name, value);
            }
        }

        /// <summary>
        /// Bool値の出し入れ 文字列Valueを利用する
        /// </summary>
        public bool BoolValue
        {
            get
            {
                var b = false;
                try
                {
                    // "True"・"False"以外だと、例外になる その場合はFalseを入れる
                    b = System.Convert.ToBoolean(Value);
                } catch { }
                return b;
            }
            set
            {
                Value = System.Convert.ToString(value);
            }
        }

        /// <summary>
        /// 画面に出すパラメータ名
        /// </summary>
        public string DisplayName { 
            get {
                return Name.Replace("[bool]", "");
            } 
        }

        public bool IsIndependentIP => !IsDependIP;

        public bool IsDependIP => IPListViewModel.Instance.Contains(Name);
        /// <summary>
        /// PowerShellスクリプト、引数に明示的にBool型が指定してあるかチェック
        /// </summary>
        public bool IsBoolValue => Name.Contains("[bool]");
        public bool IsNotBoolValue => !IsBoolValue;



        public Parameter(string name)
        {
            Name = name;
        }

        public string Get(IPParams ipParam)
        {
            return ipParam.Get(Name) ?? Value;
        }
    }
}
