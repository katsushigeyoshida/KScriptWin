using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// ===  配列===
    /// int indexOfArray(string array)                          1次元配列名のインデックスを求める
    /// int maxIndexOfArray(Dictionary<string, Token> array)    1次元配列リストからインデックスの最大値を求める
    /// string getSearchName(Token arg)                         配列検索用の配列名を求める
    /// (string name, int no) getArrayName(Token args)          変数名または配列名と配列の次元の取得
    /// (string name, int index) getArrayNo(string arrayName)   配列から配列名と配列のインデックスを取得
    /// (string name, int? row, int? col) getArrayNo2(string arrayName) 2次元配列から配列名と行と列を取り出す
    /// (string name, string row, string col) getArgArray2(string arrayName)    2次元配列名から配列名、行名、列名を抽出
    /// </summary>
    public class Util
    {
        private KLexer mLexer = new KLexer();                    //  字句解析
        private YLib ylib = new YLib();

        /// <summary>
        /// 配列変数を分解する(abc[m,n] → abc [ m , n ])
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<string> splitArgVariable(string arg)
        {
            List<string> argList = new List<string>();
            string buf = "";
            for (int i = 0; i < arg.Length; i++) {
                if (arg[i] == '[' || arg[i] == ']' || arg[i] == ',') {
                    if (0 < buf.Length)
                        argList.Add(buf);
                    argList.Add(arg[i].ToString());
                    buf = "";
                } else {
                    buf += arg[i];
                }
            }
            return argList;
        }

        /// <summary>
        /// 1次元配列名のインデックスを求める
        /// </summary>
        /// <param name="array">配列名</param>
        /// <returns>インデックス</returns>
        public int indexOfArray(string array)
        {
            int sp = array.LastIndexOf(',');
            if (sp < 0)
                sp = array.IndexOf('[');
            int ep = array.IndexOf("]");
            if (0 < sp && sp < ep)
                return ylib.intParse(array.Substring(sp + 1, ep - sp - 1), -1);
            return -1;
        }

        /// <summary>
        /// 1次元配列リストからインデックスの最大値を求める
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public int maxIndexOfArray(Dictionary<string, Token> array)
        {
            int maxIndex = 0;
            foreach (var keyValue in array) {
                int index = indexOfArray(keyValue.Key);
                if (maxIndex < index)
                    maxIndex = index;
            }
            return maxIndex;
        }

        /// <summary>
        /// 配列検索用の配列名を求める(arrayName[, arraName[, , arrayName[aa, )
        /// a[] => a[ , a[1] => a[1]
        /// a[,] => a[ , a[1,] => a[1, , a[,1] => a[,1] , a[1,1] => a[1,1]
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>検索用配列名</returns>
        public string getSearchName(Token arg)
        {
            string arrayName = "";
            if (0 <= arg.mValue.IndexOf("[]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf('[') + 1);
            else if (0 <= arg.mValue.IndexOf("[,]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf('[') + 1);
            else if (0 <= arg.mValue.IndexOf(",]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf(',') + 1);
            else
                arrayName = arg.mValue;
            return arrayName;
        }

        /// <summary>
        /// 変数名または配列名と配列の次元の取得
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>(配列名, 次元)</returns>
        public (string name, int no) getArrayName(Token args)
        {
            int dimNo = 0;
            int cp = args.mValue.LastIndexOf(',');
            int sp = args.mValue.IndexOf("[");
            if (0 < sp && cp < 0) dimNo = 1;
            if (0 < sp && 0 < cp) dimNo = 2;
            if (0 < args.mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = args.mValue.Substring(0, cp + 1);
            else if (0 < sp)
                arrayName = args.mValue.Substring(0, sp);
            return (arrayName, dimNo);
        }

        /// <summary>
        /// 配列から配列名と配列のインデックスを取得
        /// </summary>
        /// <param name="arrayName">配列</param>
        /// <returns>(配列名,インデックス)</returns>
        public (string name, int index) getArrayNo(string arrayName)
        {
            List<Token> splitName = mLexer.splitArgList(arrayName);
            if (splitName.Count < 2)
                return ("", -1);
            string name = splitName[0].mValue;
            int index = ylib.intParse(splitName[2].mValue);
            return (name, index);
        }

        /// <summary>
        /// 2次元配列から配列名と行と列を取り出す
        /// </summary>
        /// <param name="arrayName">2D配列</param>
        /// <returns>(配列名、行、列)</returns>
        public (string name, int? row, int? col) getArrayNo2(string arrayName)
        {
            List<Token> splitName = mLexer.splitArgList(arrayName);
            if (splitName.Count < 5)
                return ("", null, null);
            string name = splitName[0].mValue;
            int row = ylib.intParse(splitName[2].mValue);
            int col = ylib.intParse(splitName[4].mValue);
            return (name, row, col);
        }

        /// <summary>
        /// 2次元配列名から配列名、行名、列名を抽出
        /// a[,] => a,, , a[m,] => a,m, , a[,n] => a,,n , a[m,n] => a,m,n
        /// </summary>
        /// <param name="arrayName">2D配列名</param>
        /// <returns>(配列名,行名,列名)</returns>
        public (string name, string row, string col) getArgArray2(string arrayName)
        {
            List<Token> splitName = mLexer.splitArgList(arrayName);
            if (splitName.Count < 4)
                return ("", "", "");
            string name = splitName[0].mValue;
            string row = "", col = "";
            if (splitName[3].mValue == ",") {
                row = splitName[2].mValue;
                if (5 < splitName.Count && splitName[5].mValue == "]")
                    col = splitName[4].mValue;
            } else if (splitName[2].mValue == "," && 4 < splitName.Count && splitName[4].mValue == "]")
                col = splitName[3].mValue;
            return (name, row, col);
        }
    }
}
