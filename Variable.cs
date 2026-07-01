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
    /// int getArrayOder(Token var)                         配列変数の次数を求める
    /// int countVariable(string key, string last = "")     指定の文字で始まる変数の数を求める(配列の大きさ)
    /// void clearArray(string key)                         指定の文字で始まる配列を削除
    /// bool isStringArray(Token arg)                       配列に文字列名があるかの確認
    /// int getMaxArray(string arrayName)                   列の最大インデックスを求める
    /// List<double> cnvListDouble(Token arg)               配列データを実数のリストに変換
    /// List<string> cnvListString(Token arg)               配列データを文字列のリストに変換
    /// double[,]? cnvArrayDouble2(Token arg)               配列変数を実数配列double[,]に変換
    /// string[,]? cnvArrayString2(Token arg)               配列変数を実数配列double[,]に変換
    /// Token[,] cnvArrayToken2(Token arg)                  配列変数を配列 Token[,] に変換
    /// ===  配列の戻り値  ===
    /// void setReturnArray(Token[] src, Token dest)        配列戻り値に設定
    /// void setReturnArray(Token[,] src, Token dest)       配列戻り値に設定(2D Token)
    /// void setReturnArray(double[] src, Token dest)       配列の戻り値に設定
    /// void setReturnArray(string[] src, Token dest)       文字列配列を戻り値に設定
    /// void setReturnArray(double[,] src, Token dest)      2D配列の戻り値に設定
    /// void setReturnArray(string[,] src, Token dest)      2D配列の戻り値に設定
    /// ===  関数の引数を変数に変換
    /// List<PointD> args2PointList(List<Token> args)       引数からPointDリストを作成(plist[,]/p0[],p1[].../x0,y0,x1,y1...  → List<PointD>)</PointD>
    /// List<double> getDoubleListFromArgs(List<Token> args)    引数からdoubleリストを取得(配列を除く)
    /// List<string> getStringListFromArgs(List<Token> args)    引数からstringリストを取得
    /// List<PointD> getPointListFromArgs(List<Token> args)     引数からPointDリストを取得(配列のみ)
    /// string getStringFromArg(Token arg)                  一引数から文字列を取得
    /// double getDoubleFromArg(Token arg)                  一引数から数値を取得
    /// 
    /// ====  配列の演算
    /// void addArrayValue(Token arg, double v)                     配列の値に値を加える
    /// void addArrayValue(List<string> arrayNameList, double v)    配列の値に値を加える
    /// void multiArrayValue(Token arg, double v)                   配列の値に値を掛ける
    /// void multiArrayValue(List<string> arrayNameList, double v)  配列の値に値を掛ける
    /// 
    /// 
    /// </summary>
    public class Variable
    {
        public Dictionary<string, Token> mGlobalVar = new Dictionary<string, Token>();  //  変数リスト(変数名,値)　グローバル変数
        public Dictionary<string, Token> mVariables = new Dictionary<string, Token>();  //  変数リスト(変数名,値)　ローカル変数

        private KParse mParse = new KParse();                    //  構文解析
        private KLexer mLexer = new KLexer();                    //  字句解析
        private Util mUtil = new Util();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Variable() { }

        /// ===  変数の設定・取得  ===

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
            List<string> arrayName = mUtil.splitArrayName(key.mValue);
            foreach (var variable in variables) {
                if (mUtil.arrayNameMatch(arrayName, mUtil.splitArrayName(variable.Key)))
                    varsList.Add(variable.Key, variable.Value);
            }
            return varsList;
        }

        /// <summary>
        /// 変数名リストの取得
        /// </summary>
        /// <param name="key">検索キー</param>
        /// <returns>変数名リスト</returns>
        public List<string> getVariableNameList(Token key)
        {
            if (0 == key.mValue.IndexOf("g_")) {
                //  グローバル変数
                return getVariableNameList(mGlobalVar, key);
            } else {
                //  ローカル変数
                return getVariableNameList(mVariables, key);
            }
        }

        /// <summary>
        /// 変数登録リストから変数名リストを取得
        /// </summary>
        /// <param name="variables">変数登録リスト</param>
        /// <param name="key">検索キー</param>
        /// <returns>へか数名リスト</returns>
        public List<string>getVariableNameList(Dictionary<string, Token> variables, Token key)
        {
            List<string> varsList = new ();
            List<string> arrayName = mUtil.splitArrayName(key.mValue);
            foreach (var variable in variables) {
                if (mUtil.arrayNameMatch(arrayName, mUtil.splitArrayName(variable.Key)))
                    varsList.Add(variable.Key);
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
            if (0 < key.Length && key[0] == '"')
                return new Token(key, TokenType.STRING);
            else if (0 < key.IndexOf('['))
                return new Token(key, TokenType.ARRAY);
            else
                return new Token(key, TokenType.LITERAL);
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
        /// 配列変数の次数を求める(次数=0 通常の変数,1:1次元配列,2=2次元配列 0> :エラー)
        /// </summary>
        /// <param name="var">変数</param>
        /// <returns>次数</returns>
        public int getArrayOder(Token var)
        {
            if (var.mType == TokenType.ARRAY) {
                //  配列の次数
                string varStr = var.mValue;
                int sp = varStr.IndexOf('[');
                int ep = varStr.IndexOf("]");
                if (sp < 0 && ep < 0)
                    return 0;
                if (0 < sp && sp < ep) {
                    string arg = varStr.Substring(sp, ep - sp);
                    return arg.Count(c => c == ',') + 1;
                }
            } else if (var.mType == TokenType.STRING || var.mType == TokenType.VARIABLE || var.mType == TokenType.LITERAL) {
                //  配列以外の変数や定数
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// 指定の文字で始まる変数の数を求める(配列の大きさ)
        /// (2次元配列の行数を求めるとき last に列名を指定)
        /// </summary>
        /// <param name="key">変数名</param>
        /// <param name="last">2次元配列の列名[,n]</param>
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
        /// 配列に値を追加する a[] = {1,2,3} => {1,2,3,4,v}
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <param name="v">追加の値</param>
        public void appendArray(Token arg, double v)
        {
            List<string> splitTargetName = mUtil.splitArrayName(arg.mValue);
            int col = splitTargetName.IndexOf("");
            if (col < 0 || 0 < splitTargetName.IndexOf("", col + 1)) return;
            List<string> arrayList = getArrayNameList(arg);
            int maxIndex = -1;
            foreach (string arrayName in arrayList) {
                List<string> splitName = mUtil.splitArrayName(arrayName);
                int index = ylib.intParse(splitName[col]);
                if (maxIndex < index)
                    maxIndex = index;
            }
            if (0 <= maxIndex) {
                splitTargetName[col] = (maxIndex + 1).ToString();
                string arrayName = string.Join("", splitTargetName);
                setVariable(arrayName, new Token(v.ToString()));
            }
        }

        /// <summary>
        /// 配列名から配列名リストを作成 (a[] => a[0],a[1]... , a[1,] => a[1,0],a[1,1]... , a[,1] => a[0,1],a[1,1]... )
        /// </summary>
        /// <param name="array">抽出配列名</param>
        /// <returns>配列名リスト</returns>
        public List<string> getArrayNameList(Token array)
        {
            (string arrayName, int no) = mUtil.getArrayName(array);
            List<string> splitName = mUtil.splitArrayName(array.mValue);
            List<string> arrayList = new List<string>();
            if (0 < no) {
                if (0 == arrayName.IndexOf("g_")) {
                    //  グローバル変数
                    foreach (var variable in mGlobalVar) {
                        if (mUtil.arrayNameMatch(splitName, mUtil.splitArrayName(variable.Key))) {
                            arrayList.Add(variable.Key);
                        }
                    }
                } else {
                    //  ローカル変数
                    foreach (var variable in mVariables) {
                        if (mUtil.arrayNameMatch(splitName, mUtil.splitArrayName(variable.Key))) {
                            arrayList.Add(variable.Key);
                        }
                    }
                }
            }
            return mUtil.arrayNameSort(arrayList);
        }

        /// <summary>
        /// 配列に文字列名があるかの確認
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>文字列あるなし</returns>
        public bool isStringArray(Token args)
        {
            (string arrayName, int no) = mUtil.getArrayName(args);
            if (0 < no) {
                if (0 == arrayName.IndexOf("g_")) {
                    //  グローバル変数
                    foreach (var variable in mGlobalVar) {
                        if (variable.Key.IndexOf($"{arrayName}[") == 0) {
                            Token token = mGlobalVar[variable.Key];
                            if (token.mType == TokenType.STRING)
                                return true;
                        }
                    }
                } else {
                    //  ローカル変数
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
        /// <param name="arrayName">配列名([]を含まない)</param>
        /// <returns>最大インデックス値</returns>
        public int getMaxArray(string arrayName)
        {
            int maxCol = 0;
            if (0 == arrayName.IndexOf("g_")) {
                //  グローバル変数
                foreach (var variable in mGlobalVar) {
                    (string name, int? col) = mUtil.getArrayNo(variable.Key);
                    if (name == arrayName && col != null)
                        maxCol = Math.Max(maxCol, (int)col);
                }
            } else {
                //  ローカル変数
                foreach (var variable in mVariables) {
                    (string name, int? col) = mUtil.getArrayNo(variable.Key);
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
            string arrayName = mUtil.getArraySearchName(arg);
            foreach (var variable in getVariableList(arrayName)) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType == TokenType.STRING)
                        listData.Add(variable.Value.getValue());
                }
            }
            return listData;
        }

        /// <summary>
        /// 配列変数を実数配列double[,]に変換(1次元配列も2次元に変換)
        /// </summary>
        /// <param name="arg">配列変数</param>
        /// <returns>実数配列</returns>
        public double[,]? cnvArrayDouble2(Token arg)
        {
            (string arrayName, int no) = mUtil.getArrayName(arg);
            if (no == 0)
                return null;
            if (0 < arrayName.IndexOf("["))
                arrayName = arrayName.Substring(0, arrayName.IndexOf("["));
            //  配列サイズを求める
            int maxRow = 0, maxCol = 0;
            if (no == 1) {
                //  1次元配列
                maxRow = 0;
                foreach (var variable in getVariableList(arrayName)) {
                    (string name, int? col) = mUtil.getArrayNo(variable.Key);
                    if (name == arrayName && col != null) {
                        maxCol = Math.Max(maxCol, (int)col);
                    }
                }
            } else if (no == 2) {
                //  2次元配列
                foreach (var variable in getVariableList(arrayName)) {
                    (string name, int? row, int? col) = mUtil.getArrayNo2(variable.Key);
                    if (name == arrayName && row != null && col != null) {
                        maxRow = Math.Max(maxRow, (int)row);
                        maxCol = Math.Max(maxCol, (int)col);
                    }
                }
            }
            //  2次元配列の格納
            double[,] ret = new double[maxRow + 1, maxCol + 1];
            if (no == 1) {
                //  1次元配列
                for (int j = 0; j <= maxCol; j++) {
                    string name = $"{arrayName}[{j}]";
                    ret[0, j] = ylib.doubleParse(getVariable(name).mValue);
                }
            } else if (no == 2) {
                //  2次元配列
                for (int i = 0; i <= maxRow; i++) {
                    for (int j = 0; j <= maxCol; j++) {
                        string name = $"{arrayName}[{i},{j}]";
                        ret[i, j] = ylib.doubleParse(getVariable(name).mValue);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 配列変数を文字配列string[,]に変換
        /// </summary>
        /// <param name="arg">配列変数</param>
        /// <returns>実数配列</returns>
        public string[,]? cnvArrayString2(Token arg)
        {
            (string arrayName, int no) = mUtil.getArrayName(arg);
            if (no != 2)
                return null;
            int maxRow = 0, maxCol = 0;
            foreach (var variable in getVariableList(arrayName)) {
                (string name, int? row, int? col) = mUtil.getArrayNo2(variable.Key);
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
        /// <param name="arg">配列変数</param>
        /// <returns>Token配列</returns>
        public Token[,] cnvArrayToken2(Token arg)
        {
            (string arrayName, int no) = mUtil.getArrayName(arg);
            if (no != 2)
                return null;
            int maxRow = 0, maxCol = 0;
            foreach (var variable in getVariableList(arrayName)) {
                (string name, int? row, int? col) = mUtil.getArrayNo2(variable.Key);
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
        public void setReturnArray(string[] src, Token dest, TokenType tokenType = TokenType.LITERAL)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                setVariable(key, new Token(src[i].ToString(), tokenType));
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

        /// <summary>
        /// 3D配列の戻り値に設定
        /// </summary>
        /// <param name="src">3D配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        public void setReturnArray(double[,,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    for (int k = 0; k < src.GetLength(2); k++) {
                        Token key = new Token($"{destName}[{i},{j},{k}]", TokenType.VARIABLE);
                        setVariable(key, new Token(src[i, j, k].ToString(), TokenType.LITERAL));
                    }
                }
            }
        }

        //  ===  関数の引数を変数に変換

        /// <summary>
        /// 実数配列値の取得 (a[] 
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>実数配列リスト</returns>
        public List<double> getDoubleArrayList(Token arg)
        {
            List<double> doubleList = new List<double>();
            List<string> arrayNameList = getArrayNameList(arg);
            foreach (var name in arrayNameList)
                doubleList.Add(ylib.doubleParse(getVariable(name).mValue));
            return doubleList;
        }

        /// <summary>
        /// 文字配列値の取得
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>文字列配列</returns>
        public List<string> getStringArrayList(Token arg)
        {
            List<string> stringList = new List<string>();
            List<string> arrayNameList = getArrayNameList(arg);
            foreach (var name in arrayNameList)
                stringList.Add(getVariable(name).mValue);
            return stringList;
        }


        /// <summary>
        /// 引数からPointDリストを作成(plist[,]/x[],y[]/p0[],p1[].../x0,y0,x1,y1...  → List<PointD>)</PointD>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<PointD> args2PointList(List<Token> args)
        {
            List<PointD> pointList = new List<PointD>();
            if (0 < args.Count && getArrayOder(args[0]) == 2) {
                //  plist[,] → List<PointD>
                double[,] plist = cnvArrayDouble2(args[0]);
                if (1 < plist.GetLength(1)) {
                    for (int i = 0; i < plist.GetLength(0); i++) {
                        PointD p = new PointD(plist[i, 0], plist[i, 1]);
                        pointList.Add(p);
                    }
                }
            } else if (args.Count == 2 && getArrayOder(args[0]) == 1 && getArrayOder(args[1]) == 1) {
                //  x[],y[] → List<PointD>
                List<double> xlist = getDoubleArrayList(args[0]);
                List<double> ylist = getDoubleArrayList(args[1]);
                for (int i = 0; i < xlist.Count && i < ylist.Count; i++)
                    pointList.Add(new PointD(xlist[i], ylist[i]));
            } else if (0 < args.Count && getArrayOder(args[0]) == 1) {
                //  p0[],p1[]... → List<PointD>
                for (int i = 0; i < args.Count; i++) {
                    List<double> spList = cnvListDouble(args[i]);
                    if (1 < spList.Count) {
                        PointD p = new PointD(spList[0], spList[1]);
                        pointList.Add(p);
                    }
                }
            } else if (1 < args.Count && getArrayOder(args[0]) == 0) {
                //  x0,y0,x1,y1... → List<PointD>
                for (int i = 0; i < args.Count - 1; i += 2) {
                    if (getArrayOder(args[i]) == 0 && getArrayOder(args[i + 1]) == 0) {
                        if (getVariable(args[i]).mType == TokenType.LITERAL && getVariable(args[i + 1]).mType == TokenType.LITERAL) {
                            PointD p = new PointD(ylib.doubleParse(args[i].mValue), ylib.doubleParse(args[i + 1].mValue));
                            pointList.Add(p);
                        }
                    }
                }
            }
            return pointList;
        }

        /// <summary>
        /// 引数からdoubleリストを取得(配列を除く)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<double> getDoubleListFromArgs(List<Token> args)
        {
            List<double> doubleList = new List<double>();
            //  x0,x1,x2....  →  List<double>
            for (int i = 0; i < args.Count; i++) {
                if (getArrayOder(args[i]) == 0) {
                    if (getVariable(args[i]).mType == TokenType.LITERAL)
                        doubleList.Add(ylib.doubleParse(args[i].mValue));
                }
            }
            return doubleList;
        }

        /// <summary>
        /// 引数からstringリストを取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<string> getStringListFromArgs(List<Token> args)
        {
            List<string> stringList = new List<string>();
            //  x0,x1,x2....  →  List<double>
            for (int i = 0; i < args.Count; i++) {
                if (getArrayOder(args[i]) == 0) {
                    if (getVariable(args[i]).mType == TokenType.STRING)
                        stringList.Add(ylib.stripBracketString(getVariable(args[i]).mValue, '"'));
                }
            }
            return stringList;
        }


        /// <summary>
        /// 引数からPointDリストを取得(配列のみ)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<PointD> getPointListFromArgs(List<Token> args)
        {
            List<PointD> pointList = new List<PointD>();
            for (int i = 0; i < args.Count; i++) {
                if (0 < args.Count && getArrayOder(args[i]) == 2) {
                    //  plist[,] → List<PointD>
                    double[,] plist = cnvArrayDouble2(args[i]);
                    if (1 < plist.GetLength(1)) {
                        for (int j = 0; j < plist.GetLength(0); j++) {
                            PointD p = new PointD(plist[j, 0], plist[j, 1]);
                            pointList.Add(p);
                        }
                    }
                } else if (getArrayOder(args[i]) == 1) {
                    //  p0[],p1[]... → List<PointD>
                    List<double> spList = cnvListDouble(args[i]);
                    if (1 < spList.Count) {
                        PointD p = new PointD(spList[0], spList[1]);
                        pointList.Add(p);
                    }
                } else if (i < args.Count - 1 && getArrayOder(args[i]) == 0 && getArrayOder(args[i + 1]) == 0) {
                    if (getVariable(args[i]).mType == TokenType.LITERAL && getVariable(args[i + 1]).mType == TokenType.LITERAL) {
                        //  x0,y0,x1,y1... → List<PointD>
                        PointD p = new PointD(ylib.doubleParse(args[i].mValue), ylib.doubleParse(args[i + 1].mValue));
                        pointList.Add(p);
                        i++;
                    }
                }
            }
            return pointList;
        }


        /// <summary>
        /// 一引数から文字列を取得
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public string getStringFromArg(Token arg)
        {
            int oder = getArrayOder(arg);
            if (oder <= 0) {
                if (getVariable(arg).mType == TokenType.STRING)
                    return ylib.stripBracketString(getVariable(arg).mValue, '"');
            }
            return "";
        }

        /// <summary>
        /// 一引数から数値を取得
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public double getDoubleFromArg(Token arg)
        {
            if (getArrayOder(arg) == 0) {
                if (getVariable(arg).mType == TokenType.LITERAL)
                    return ylib.doubleParse(arg.mValue);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<double> getDoubleListfromArg(Token arg)
        {
            List<double> doubleList = new List<double>();
            int order = getArrayOder(arg);
            if (order == 0 && getVariable(arg).mType == TokenType.LITERAL) {
                doubleList.Add(ylib.doubleParse(arg.mValue));
            } else if (0 < order) {
                List<string> arrayNameList = getArrayNameList(arg);
                foreach (string arrayName in arrayNameList) {
                    Token value = getVariable(arrayName);
                    if (value.mType == TokenType.LITERAL)
                        doubleList.Add(ylib.doubleParse(value.mValue));
                }
            }
            return doubleList;
        }


        //  ====  配列の演算

        /// <summary>
        /// 配列の値に値を加える
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <param name="v">加算値</param>
        public void addArrayValue(Token arg, double v)
        {
            Dictionary<string, Token> arrayList = getVariables(arg);
            foreach (var variable in arrayList) {
                var value = ylib.doubleParse(variable.Value.mValue);
                setVariable(variable.Key, new Token((value + v).ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 配列の値に値を加える
        /// </summary>
        /// <param name="arrayNameList">配列名リスト</param>
        /// <param name="v">加算値</param>
        public void addArrayValue(List<string> arrayNameList, double v)
        {
            foreach (var arrayName in arrayNameList) {
                var value = ylib.doubleParse(getVariable(arrayName).mValue);
                setVariable(arrayName, new Token((value + v).ToString(), TokenType.LITERAL));
            }
        }


        /// <summary>
        /// 配列の値に値を掛ける
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <param name="v">乗算値</param>
        public void multiArrayValue(Token arg, double v)
        {
            Dictionary<string, Token> arrayList = getVariables(arg);
            foreach (var variable in arrayList) {
                var value = ylib.doubleParse(variable.Value.mValue);
                setVariable(variable.Key, new Token((value * v).ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 配列の値に値を掛ける
        /// </summary>
        /// <param name="arrayNameList">配列名リスト</param>
        /// <param name="v">乗算値</param>
        public void multiArrayValue(List<string> arrayNameList, double v)
        {
            foreach (var arrayName in arrayNameList) {
                var value = ylib.doubleParse(getVariable(arrayName).mValue);
                setVariable(arrayName, new Token((value * v).ToString(), TokenType.LITERAL));
            }
        }
    }
}
