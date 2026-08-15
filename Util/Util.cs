using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// ===  配列===
    /// List<String> arrayNameSort(List<string> arrayNameList)      配列名をソート
    /// List<string> splitArrayName(string arg)                     配列変数を分解する(abc[m,n] → abc [ m , n ]  , abc[,n] → abc [ "" , n ])
    /// bool arrayNameMatch(List<string> a, List<string> b)         配列名のマッチングを行う
    /// int indexOfArray(string array)                              1次元配列名のインデックスを求める
    /// int maxIndexOfArray(Dictionary<string, Token> array)        1次元配列リストからインデックスの最大値を求める
    /// string getArraySearchName(Token arg)                        配列検索用の配列名を求める
    /// (string name, int no) getArrayName(Token args)              変数名または配列名と配列の次元の取得
    /// (string name, int index) getArrayNo(string arrayName)       配列から配列名と配列のインデックスを取得
    /// (string name, int? row, int? col) getArrayNo2(string arrayName)         2次元配列から配列名と行と列を取り出す
    /// (string name, string row, string col) getArgArray2(string arrayName)    2次元配列名から配列名、行名、列名を抽出
    /// </summary>
    public class Util
    {
        private KLexer mLexer = new KLexer();                    //  字句解析
        private YLib ylib = new YLib();

 

        /// <summary>
        /// 配列変数を分解する(abc[m,n] → abc [ m , n ]  , abc[,n] → abc [ "" , n ])
        /// </summary>
        /// <param name="arg">配列名([]を含む)</param>
        /// <returns>配列名を分解したリスト</returns>
        public List<string> splitArrayName(string arg)
        {
            List<string> argList = new List<string>();
            string buf = "";
            for (int i = 0; i < arg.Length; i++) {
                if (arg[i] == '[' || arg[i] == ']' || arg[i] == ',') {
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
        /// 配列名のマッチングを行う (a[m,n] == a[m,n] => true, a[m,n] == a[m,] => true, a[m,n] == a[,] => true, a[m,n] == a[n,m] => false)
        /// </summary>
        /// <param name="a">分解配列名a</param>
        /// <param name="b">分解配列名b</param>
        /// <returns>マッチング結果</returns>
        public bool arrayNameMatch(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) 
                return false;
            for (int i = 0; i < a.Count; i++) {
                if (a[i] == "" || b[i] == "" || a[i] == b[i])
                    continue;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 1次元配列名のインデックスを求める
        /// </summary>
        /// <param name="array">配列名([]を含む)</param>
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
        /// <param name="arg">配列名([]を含む)</param>
        /// <returns>検索用配列名</returns>
        public string getArraySearchName(Token arg)
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
        /// <param name="args">変数/配列名([]を含む)</param>
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
        /// 配列から配列名と配列のインデック
        /// }スを取得
        /// </summary>
        /// <param name="arrayName">配列名([]を含む)</param>
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
        /// <param name="arrayName">2D配列([]を含む)</param>
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
        /// <param name="arrayName">2D配列名([]を含む)</param>
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

        /// <summary>
        /// 配列変数を分解する (a[b[n,0],0] →  a[ b[n,0] , 0 ]
        /// </summary>
        /// <param name="text">配列変数文字列</param>
        /// <returns>分解リスト</returns>
        public List<string> splitArrayVariable(string text)
        {
            List<string> extractList = new List<string>();
            int pos = 0;
            int count = 0;
            string buf = "";
            while (pos < text.Length) {
                if (text[pos] == '[') {
                    count++;
                    buf += text[pos++];
                    extractList.Add(buf);
                    buf = "";
                    while (pos < text.Length) {
                        if (text[pos] == ']') {
                            count--;
                            if (count == 0) {
                                if (0 < buf.Length)
                                    extractList.Add(buf);
                                extractList.Add(text[pos++].ToString());
                                buf = "";
                                break;
                            } else {
                                buf += text[pos++];
                            }
                        } else if (1 == count && text[pos] == ',') {
                            if (0 < buf.Length)
                                extractList.Add(buf);
                            extractList.Add(text[pos++].ToString());
                            buf = "";
                        } else if (text[pos] == '[') {
                            count++;
                            buf += text[pos++];
                        } else if (text[pos] == ' ' || text[pos] == '\n' || text[pos] == '\r') {
                            pos++;
                        } else {
                            buf += text[pos++];
                        }
                    }
                } else if (text[pos] == ',' || text[pos] == ']'
                     || text[pos] == '{' || text[pos] == '}') {
                    if (0 < buf.Length)
                        extractList.Add(buf);
                    extractList.Add(text[pos++].ToString());
                    buf = "";
                } else if (text[pos] == '"') {
                    buf += text[pos++];
                    while (pos < text.Length && text[pos] != '"') {
                        buf += text[pos++];
                    }
                } else if (text[pos] == ' ' || text[pos] == '\t'
                    || text[pos] == '\n' || text[pos] == '\r') {
                    pos++;
                } else {
                    buf += text[pos++];
                }
            }
            if (0 < buf.Length)
                extractList.Add(buf);
            return extractList;
        }

        /// <summary>
        /// 文字列が配列変数かの確認 ([]の対応があっていないものは配列とはみなさない)
        /// </summary>
        /// <param name="vari">変数文字列</param>
        /// <returns>配列変数</returns>
        public bool isArrayVariable(string vari)
        {
            int sc = 0, ec = 0;
            for (int i = 0; i < vari.Length; i++) {
                if (vari[i] == '[') sc++;
                if (vari[i] == ']') ec++;
            }
            if (0 < sc && sc == ec)
                return true;
            return false;
        }

        /// <summary>
        /// デバッグ用トークンリストの文字列化
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>文字列</returns>
        public string tokensString(List<Token> tokens)
        {
            string buf = "";
            foreach (var token in tokens)
                buf += token.mValue + " ";
            buf.Trim();
            return buf;
        }

    }
}
