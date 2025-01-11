using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// 変数管理
    /// 
    /// ===  変数の設定・取得  ===
    /// void setVariable(Token key, Token value = null)     変数の登録(変数名と数値)
    /// void setVariable(string key, Token value = null)    変数の登録(変数名と数値)
    /// Token getVariable(Token key)                        変数の値の取得
    /// Token getVariable(string key)                       変数の値の取得
    /// Dictionary<string, Token> getVariables(Token key)   配列変数の抽出(a[],a[,],a[n,])
    /// void clearVariables(Token key)                      配列変数の削除 (a[],a[,],a[n,])
    /// Dictionary<string, Token> getVariableList(Token variableName)  変数格納データの取得
    /// Dictionary<string, Token> getVariableList(string variableName) 変数格納データの取得
    /// bool containsVariable(string key)                   変数の存在確認
    /// void removeVariable(string key)                     指定した変数を削除
    /// ===  配列  ====
    /// int countVariable(string key, string last = "")     指定の文字で始まる変数の数を求める(配列の大きさ)
    /// void clearArray(string key)                         指定の文字で始まる配列を削除
    /// bool isStringArray(Token args)                      配列に文字列名があるかの確認
    /// int getMaxArray(string arrayName)                   列の最大インデックスを求める
    /// List<double> cnvListDouble(Token arg)               配列データを実数のリストに変換
    /// List<string> cnvListString(Token arg)               配列データを文字列のリストに変換
    /// double[,]? cnvArrayDouble2(Token args)              配列変数を実数配列double[,]に変換
    /// string[,]? cnvArrayString2(Token args)              配列変数を実数配列double[,]に変換
    /// Token[,] cnvArrayToken2(Token args)                 配列変数を配列 Token[,] に変換
    /// string getSearchName(Token arg)                     配列検索用の配列名を求める
    /// (string name, int no) getArrayName(Token args)      変数名または配列名と配列の次元の取得
    /// (string name, int index) getArrayNo(string arrayName)   配列から配列名と配列のインデックスを取得
    /// (string name, int? row, int? col) getArrayNo2(string arrayName) 2次元配列から配列名と行と列を取り出す
    /// (string name, string row, string col) getArgArray2(string arrayName)    2次元配列名から配列名、行名、列名を抽出
    /// ===  配列の戻り値  ===
    /// void setReturnArray(Token[] src, Token dest)        配列戻り値に設定
    /// void setReturnArray(Token[,] src, Token dest)       配列戻り値に設定(2D Token)
    /// void setReturnArray(double[] src, Token dest)       配列の戻り値に設定
    /// void setReturnArray(string[] src, Token dest)       文字列配列を戻り値に設定
    /// void setReturnArray(double[,] src, Token dest)      2D配列の戻り値に設定
    /// void setReturnArray(string[,] src, Token dest)      2D配列の戻り値に設定
    /// 
    /// </summary>
    public class Variable
    {
        public Dictionary<string, Token> mGlobalVar = new Dictionary<string, Token>();  //  変数リスト(変数名,値)
        public Dictionary<string, Token> mVariables = new Dictionary<string, Token>();  //  変数リスト(変数名,値)

        private KParse mParse = new KParse();                    //  構文解析
        private KLexer mLexer = new KLexer();                    //  字句解析
        private Util mUtil = new Util();
        private YLib ylib = new YLib();

        public Variable() { }


        /// <summary>
        /// 変数の登録(変数名と数値)
        /// </summary>
        /// <param name="key">変数名(トークン)</param>
        /// <param name="value">数値/数式(トークン)</param>
        public void setVariable(Token key, Token value = null)
        {
            setVariable(key.mValue, value);
        }

        /// <summary>
        /// 変数の登録(変数名と数値)
        /// </summary>
        /// <param name="key">変数名</param>
        /// <param name="value">数値/数式(トークン)</param>
        private void setVariable(string key, Token value = null)
        {
            if (0 == key.IndexOf("g_")) {
                //  グローバル変数
                if (!mGlobalVar.ContainsKey(key)) {
                    mGlobalVar.Add(key, value);
                } else {
                    mGlobalVar[key] = value;
                }
            } else {
                //  ローカル変数
                if (!mVariables.ContainsKey(key)) {
                    mVariables.Add(key, value);
                } else {
                    mVariables[key] = value;
                }
            }
        }

        /// <summary>
        /// 配列変数の抽出(a[],a[,],a[n,])
        /// </summary>
        /// <param name="key">配列変数名</param>
        /// <returns>抽出配列変数リスト</returns>
        public Dictionary<string, Token> getVariables(Token key)
        {
            if (0 == key.mValue.IndexOf("g_")) {
                //  グローバル変数
                return getVariables(mGlobalVar, key);
            } else {
                //  ローカル変数
                return getVariables(mVariables, key);
            }
        }

        /// <summary>
        /// 配列変数の抽出(a[],a[,],a[n,])
        /// </summary>
        /// <param name="variables">変数登録リスト</param>
        /// <param name="key">配列変数名</param>
        /// <returns>抽出配列変数リスト</returns>
        private Dictionary<string, Token> getVariables(Dictionary<string, Token> variables, Token key)
        {
            Dictionary<string, Token> varsList = new Dictionary<string, Token>();
            string keyWord = getKeyWord(key.mValue);
            if (keyWord.Length == 0) return varsList;
            foreach (var variable in variables) {
                if (variable.Key.IndexOf(keyWord) == 0)
                    varsList.Add(variable.Key, variable.Value);
            }
            return varsList;
        }

        /// <summary>
        /// 配列変数の削除 (a[],a[,],a[n,])
        /// </summary>
        /// <param name="key">配列変数</param>
        public void clearVariables(Token key)
        {
            if (0 == key.mValue.IndexOf("g_")) {
                //  グローバル変数
                clearVariables(mGlobalVar, key);
            } else {
                //  ローカル変数
                clearVariables(mVariables, key);
            }
        }

        /// <summary>
        /// 配列変数の削除
        /// </summary>
        /// <param name="variables">変数登録リスト</param>
        /// <param name="key">配列変数</param>
        private void clearVariables(Dictionary<string, Token> variables, Token key)
        {
            string keyWord = getKeyWord(key.mValue);
            if (keyWord.Length == 0) return;
            foreach (var variable in variables) {
                if (variable.Key.IndexOf(keyWord) == 0)
                    variables.Remove(variable.Key);
            }
        }

        /// <summary>
        /// 配列変数名から検索ワードを作成
        /// "a[]", "a[,]" → "a[" ,  "a[n,]" →　"a[n"
        /// </summary>
        /// <param name="key">配列変数名</param>
        /// <returns>検索ワード</returns>
        private string getKeyWord(string key)
        {
            int n = key.IndexOf("[]");
            if (n < 0) n = key.IndexOf("[,]");
            if (n < 0) {
                n = key.IndexOf(']');
                if (n <= 0) return "";
                return key.Substring(0, n);
            } else
                return key.Substring(0, n + 1);
        }

        /// <summary>
        /// 変数の値の取得
        /// </summary>
        /// <param name="key">変数名</param>
        /// <returns>値</returns>
        public Token getVariable(Token key)
        {
            return getVariable(key.mValue);
        }

        /// <summary>
        /// 変数の値の取得
        /// </summary>
        /// <param name="key">変数名</param>
        /// <returns>値</returns>
        public Token getVariable(string key)
        {
            if (0 == key.IndexOf("g_")) {
                if (mGlobalVar.ContainsKey(key))
                    return mGlobalVar[key];
            } else {
                if (mVariables.ContainsKey(key))
                    return mVariables[key];
            }
            return new Token(key, (0 < key.Length && key[0] == '"') ? TokenType.STRING : TokenType.LITERAL);
        }

        /// <summary>
        /// 変数格納データの取得
        /// </summary>
        /// <param name="variableName">変数名</param>
        /// <returns>変数リスト</returns>
        public Dictionary<string, Token> getVariableList(Token variableName)
        {
            return getVariableList(variableName.mValue);
        }

        /// <summary>
        /// 変数格納データの取得
        /// </summary>
        /// <param name="variableName">変数名</param>
        /// <returns>変数リスト</returns>
        public Dictionary<string, Token> getVariableList(string variableName)
        {
            if (0 == variableName.IndexOf("g_"))
                return mGlobalVar;
            else
                return mVariables;
        }

        /// <summary>
        /// 変数の存在確認(配列を確認するときはインデックスも必要)
        /// </summary>
        /// <param name="key">変数名</param>
        /// <returns>存在の有無</returns>
        public bool containsVariable(string key)
        {
            if (0 == key.IndexOf("g_")) {
                if (mGlobalVar.ContainsKey(key))
                    return true;
            } else {
                if (mVariables.ContainsKey(key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 指定した変数を削除
        /// </summary>
        /// <param name="key">変数名</param>
        public void removeVariable(string key)
        {
            if (0 == key.IndexOf("g_")) {
                if (mGlobalVar.ContainsKey(key))
                    mGlobalVar.Remove(key);
            } else {
                if (mVariables.ContainsKey(key))
                    mVariables.Remove(key);
            }
        }

        //  ===  配列  ====

        /// <summary>
        /// 指定の文字で始まる変数の数を求める(配列の大きさ)
        /// </summary>
        /// <param name="key">変数名</param>
        /// <returns>変数の数</returns>
        public int countVariable(string key, string last = "")
        {
            int count = 0;
            if (0 == key.IndexOf("g_")) {
                foreach (var variable in mGlobalVar)
                    if (0 == variable.Key.IndexOf(key) && 0 <= variable.Key.IndexOf(last))
                        count++;
            } else {
                foreach (var variable in mVariables)
                    if (0 == variable.Key.IndexOf(key) && 0 <= variable.Key.IndexOf(last))
                        count++;
            }
            return count;
        }

        /// <summary>
        /// 指定の文字で始まる配列を削除
        /// </summary>
        /// <param name="key">配列名</param>
        public void clearArray(string key)
        {
            int count = 0;
            if (0 == key.IndexOf("g_")) {
                foreach (var variable in mGlobalVar)
                    if (0 == variable.Key.IndexOf(key))
                        mGlobalVar.Remove(variable.Key);
            } else {
                foreach (var variable in mVariables)
                    if (0 == variable.Key.IndexOf(key))
                        mVariables.Remove(variable.Key);
            }
        }

        /// <summary>
        /// 配列に文字列名があるかの確認
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>文字列あるなし</returns>
        public bool isStringArray(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (0 < no) {
                if (0 == arrayName.IndexOf("g_")) {
                    foreach (var variable in mGlobalVar) {
                        if (variable.Key.IndexOf($"{arrayName}[") == 0) {
                            Token token = mGlobalVar[variable.Key];
                            if (token.mType == TokenType.STRING)
                                return true;
                        }
                    }
                } else {
                    foreach (var variable in mVariables) {
                        if (variable.Key.IndexOf($"{arrayName}[") == 0) {
                            Token token = mVariables[variable.Key];
                            if (token.mType == TokenType.STRING)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 配列の最大インデックスを求める
        /// </summary>
        /// <param name="arrayName">配列名</param>
        /// <returns>最大インデックス値</returns>
        public int getMaxArray(string arrayName)
        {
            int maxCol = 0;
            if (0 == arrayName.IndexOf("g_")) {
                foreach (var variable in mGlobalVar) {
                    (string name, int? col) = getArrayNo(variable.Key);
                    if (name == arrayName && col != null)
                        maxCol = Math.Max(maxCol, (int)col);
                }
            } else {
                foreach (var variable in mVariables) {
                    (string name, int? col) = getArrayNo(variable.Key);
                    if (name == arrayName && col != null)
                        maxCol = Math.Max(maxCol, (int)col);
                }
            }
            return maxCol;
        }

        /// <summary>
        /// 配列データを実数のリストに変換
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>実数リスト</returns>
        public List<double> cnvListDouble(Token arg)
        {
            Dictionary<string, Token> arrayList = getVariables(arg);
            int maxArray = mUtil.maxIndexOfArray(arrayList);
            double[] arrayData = new double[maxArray + 1];
            foreach (var variable in arrayList) {
                int index = mUtil.indexOfArray(variable.Key);
                if (0 <= index) {
                    if (variable.Value.mType != TokenType.STRING)
                        arrayData[index] = ylib.doubleParse(variable.Value.mValue);
                }
            }
            return arrayData.ToList();
        }

        /// <summary>
        /// 配列データを文字列のリストに変換
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>文字列リスト</returns>
        public List<string> cnvListString(Token arg)
        {
            List<string> listData = new List<string>();
            string arrayName = getSearchName(arg);
            foreach (var variable in getVariableList(arrayName)) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType == TokenType.STRING)
                        listData.Add(variable.Value.getValue());
                }
            }
            return listData;
        }

        /// <summary>
        /// 配列変数を実数配列double[,]に変換
        /// </summary>
        /// <param name="args">配列変数</param>
        /// <returns>実数配列</returns>
        public double[,]? cnvArrayDouble2(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 2)
                return null;
            if (0 < arrayName.IndexOf("["))
                arrayName = arrayName.Substring(0, arrayName.IndexOf("["));
            int maxRow = 0, maxCol = 0;
            foreach (var variable in getVariableList(arrayName)) {
                (string name, int? row, int? col) = getArrayNo2(variable.Key);
                if (name == arrayName && row != null && col != null) {
                    maxRow = Math.Max(maxRow, (int)row);
                    maxCol = Math.Max(maxCol, (int)col);
                }
            }
            double[,] ret = new double[maxRow + 1, maxCol + 1];
            for (int i = 0; i <= maxRow; i++) {
                for (int j = 0; j <= maxCol; j++) {
                    string name = $"{arrayName}[{i},{j}]";
                    ret[i, j] = ylib.doubleParse(getVariable(name).mValue);
                }
            }
            return ret;
        }

        /// <summary>
        /// 配列変数を実数配列double[,]に変換
        /// </summary>
        /// <param name="args">配列変数</param>
        /// <returns>実数配列</returns>
        public string[,]? cnvArrayString2(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 2)
                return null;
            int maxRow = 0, maxCol = 0;
            foreach (var variable in getVariableList(arrayName)) {
                (string name, int? row, int? col) = getArrayNo2(variable.Key);
                if (name == arrayName && row != null && col != null) {
                    maxRow = Math.Max(maxRow, (int)row);
                    maxCol = Math.Max(maxCol, (int)col);
                }
            }
            string[,] ret = new string[maxRow + 1, maxCol + 1];
            for (int i = 0; i <= maxRow; i++) {
                for (int j = 0; j <= maxCol; j++) {
                    string name = $"{arrayName}[{i},{j}]";
                    ret[i, j] = getVariable(name).mValue;
                }
            }
            return ret;
        }

        /// <summary>
        /// 配列変数を配列 Token[,] に変換
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列変数</param>
        /// <returns>Token配列</returns>
        public Token[,] cnvArrayToken2(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 2)
                return null;
            int maxRow = 0, maxCol = 0;
            foreach (var variable in getVariableList(arrayName)) {
                (string name, int? row, int? col) = getArrayNo2(variable.Key);
                if (name == arrayName && row != null && col != null) {
                    maxRow = Math.Max(maxRow, (int)row);
                    maxCol = Math.Max(maxCol, (int)col);
                }
            }
            Token[,] ret = new Token[maxRow + 1, maxCol + 1];
            for (int i = 0; i <= maxRow; i++) {
                for (int j = 0; j <= maxCol; j++) {
                    string name = $"{arrayName}[{i},{j}]";
                    ret[i, j] = getVariable(name);
                }
            }
            return ret;
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
            Console.WriteLine($"{splitName.Count}");
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

        //  ===  配列の戻り値  ===

        /// <summary>
        /// 配列戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(Token[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                setVariable(key, src[i].copy());
            }
        }

        /// <summary>
        /// 配列戻り値に設定(2D Token)
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(Token[,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            string srcName = src[0, 0].mValue;
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    setVariable(key, src[i, j].copy());
                }
            }
        }

        /// <summary>
        /// 配列の戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(double[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                setVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 文字列配列を戻り値に設定
        /// </summary>
        /// <param name="src">文字列配列</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(string[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                setVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 2D配列の戻り値に設定
        /// </summary>
        /// <param name="src">2D配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(double[,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    setVariable(key, new Token(src[i, j].ToString(), TokenType.LITERAL));
                }
            }
        }

        /// <summary>
        /// 2D配列の戻り値に設定
        /// </summary>
        /// <param name="src">2D配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(string[,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    setVariable(key, new Token(src[i, j], TokenType.STRING));
                }
            }
        }

    }
}
