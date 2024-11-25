using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// 構文解析()
    ///     字句を構文ごとにまとめる
    /// 
    /// VARIABLE 変数設定(計算処理) : variable = express
    ///                 array : VARIABLE[VARIABLE] = express
    /// FUNCTION 関数 : function ( arg, .. ) { steatement; steatement; ... } (function変数に結果が入る)
    /// STATEMENT 文  : while ( conditional ) { steatement; steatement; ... }
    ///                 if (conditional ) { steatement; steatement; ... } else { steatement; steatement; ... }
    ///                 for ( initial; conditional; iteration) { statement, statement... }
    ///                 return [express] ;
    ///                 break ;
    ///                 continue ;
    ///                 print( arg );
    ///                 println( arg );
    ///                 #include xxx;
    /// EXPRESS文     : 数式
    /// 
    /// 変数と関数の登録
    /// 
    /// 数値       : LITERAL    = 
    /// 文字列     : STRING     = 
    /// 変数       : VARIABL    = [a-z|A-Z][a-z|A-Z|0-9]+
    /// 演算子     : OPERATOR   = [+|-|*|/|^]
    /// 代入演算子 : ASSIGNMENT = [=|+=|-=|*=|/=|^=|++|--]
    /// 条件演算子 : CONDITINAL = [==|<|>|<=|>=]
    /// 論理演算子 : LOGICAL    = [!|&&|||]
    /// 配列       : ARRAY      = VARIABLE'['EXPRESS(,EXPRESS)']'
    /// 数式       : EXPRESS    = LITERAL|VARIABLE|OPERATOR
    /// 
    /// </summary>

    public class KParse
    {
        public Dictionary<string, Token> mGlobalVar = new Dictionary<string, Token>();  //  変数リスト(変数名,値)
        public Dictionary<string, Token> mVariables = new Dictionary<string, Token>();  //  変数リスト(変数名,値)
        public Dictionary<string, Token> mFunctions = new Dictionary<string, Token>();  //  関数リスト(関数名,(関数式))

        public string mErrorMessage = "";
        public KParse() { }

        private KLexer mLexer = new KLexer();
        private YLib ylib = new YLib();

        /// <summary>
        /// ステートメントごとにトークンリストを抽出
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>ステートメントリスト</returns>
        public List<List<Token>> getStatements(List<Token> tokens)
        {
            mErrorMessage = "";
            List<List<Token>> statemets = new List<List<Token>>();
            try {
                for (int i = 0; i < tokens.Count; i++) {
                    List<Token> statement = new List<Token>();
                    int sp = i, ep = i;
                    switch (tokens[i].mType) {
                        case TokenType.VARIABLE:
                        case TokenType.ARRAY:
                        case TokenType.CONSTANT:
                        case TokenType.ASSIGNMENT:
                        case TokenType.EXPRESS:
                            //  代入文 (変数 = 数式 ;) 条件文 (変数/式 条件演算子 変数/式)
                            while (i < tokens.Count && tokens[i].mValue != ";" && 0 > tokens[i].mValue.IndexOf("}")) i++;
                            if (i < tokens.Count)
                                statement.AddRange(tokens.GetRange(sp, i - sp + 1));
                            else
                                statement.AddRange(tokens.GetRange(sp, i - sp));
                            break;
                        case TokenType.FUNCTION:
                            //  関数 (関数構文を登録)
                            Token functionName = tokens[i];
                            statement.Add(tokens[i++]);             //  関数名
                            statement.Add(tokens[i++]);             //  (...) 引数
                            if (tokens[i].mValue == ";") {
                                statement.Add(tokens[i]);
                            } else {
                                List<Token> statements = getStatement(tokens, i);   //  {...} 処理文
                                statement.AddRange(statements);
                                addFunction(functionName, statement);
                                statement.Clear();
                            }
                            break;
                        case TokenType.STATEMENT:
                            //  制御文
                            statement.Add(tokens[i]);
                            if (tokens[i].mValue == "while") {
                                //  while文 (while (conditional) { 文... }
                                statement.Add(tokens[++i]);         //  (...) 制御文
                                List<Token> stateList = getStatement(tokens, ++i);
                                statement.AddRange(stateList);      //  {文...}/文 処理文
                                i += stateList.Count - 1;
                            } else if (tokens[i].mValue == "for") {
                                //  for 文
                                statement.Add(tokens[++i]);         //  (...) 制御文
                                List<Token> stateList = getStatement(tokens, ++i);
                                statement.AddRange(stateList);      //  {文...}/文 処理文
                                i += stateList.Count - 1;
                            } else if (tokens[i].mValue == "if") {
                                //  if文 (if (conditional) 文 / { 文 } else 文 / { 文 }
                                statement.Add(tokens[++i]);         //  (...) 制御文
                                List<Token> stateList = getStatement(tokens, ++i);
                                statement.AddRange(stateList);      //  {文...}/文 処理文
                                i += stateList.Count;
                                while (i + 1 < tokens.Count &&
                                    tokens[i].mValue == "else" && tokens[i + 1].mValue == "if") {
                                    statement.Add(tokens[i++]);
                                    statement.Add(tokens[i++]);
                                    statement.Add(tokens[i]);           //  (...) 制御文
                                    stateList = getStatement(tokens, ++i);
                                    statement.AddRange(stateList);      //  {文...}/文 処理文
                                    i += stateList.Count;
                                }
                                if (i < tokens.Count && tokens[i].mValue == "else") {
                                    statement.Add(tokens[i]);       //  else
                                    stateList = getStatement(tokens, ++i);
                                    statement.AddRange(stateList);  //  {文...}/文 処理分
                                    i += stateList.Count;
                                }
                                i--;
                            } else if (tokens[i].mValue == "return" ||
                                tokens[i].mValue == "break" ||
                                tokens[i].mValue == "continue" ||
                                tokens[i].mValue == "exit" ||
                                tokens[i].mValue == "pause" ||
                                tokens[i].mValue == "#include") {
                                //  return 文,break文,continue文,#include文
                                List<Token> stateList = getStatement(tokens, ++i);
                                statement.AddRange(stateList);      //  {文...}/文 処理分
                                i += stateList.Count - 1;
                            } else {
                                //  単なる文
                                i++;
                                if (i < tokens.Count && (tokens[i].mValue[0] == '(' || tokens[i].mValue[0] == '{'))
                                    statement.Add(tokens[i]);
                                else
                                    i--;
                            }
                            break;
                        case TokenType.COMMENT:
                            //System.Diagnostics.Debug.WriteLine($"Comment: {tokens[i]} ");
                            break;
                        default:
                            //System.Diagnostics.Debug.WriteLine($"default: {tokens[i]} ");
                            break;
                    }
                    if (0 < statement.Count)
                        statemets.Add(statement);
                }
            } catch (Exception e) {
                mErrorMessage = "Parser Error : " + e.Message + "\n";
            }

            return statemets;
        }

        /// <summary>
        /// 1ステートメントの構成文を抽出
        /// ({ }で囲まれた部分または ; までのステートメント)
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>ステートメント構成リスト</returns>
        public List<Token> getStatement(List<Token> tokens, int sp = 0)
        {
            List<Token> statement = new List<Token>();
            int ep = sp;
            while (ep < tokens.Count) {
                if (tokens[ep].mValue[0] == '{') {
                    statement.Add(tokens[ep]);
                    break;
                } else if (tokens[ep].mValue == ";") {
                    statement.Add(tokens[ep]);
                    break;
                } else {
                    statement.Add(tokens[ep]);
                }
                ep++;
            }
            return statement;
        }

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
        public void setVariable(string key, Token value = null)
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
        /// 変数格納データの取得
        /// </summary>
        /// <param name="variableName">変数名</param>
        /// <returns>変数リスト</returns>
        public Dictionary<string, Token> getVariables(Token variableName)
        {
            return getVariables(variableName.mValue);
        }

        /// <summary>
        /// 変数格納データの取得
        /// </summary>
        /// <param name="variableName">変数名</param>
        /// <returns>変数リスト</returns>
        public Dictionary<string, Token> getVariables(string variableName)
        {
            if (0 == variableName.IndexOf("g_"))
                return mGlobalVar;
            else
                return mVariables;
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
        /// 関数の登録
        /// </summary>
        /// <param name="key">関数名(トークン)</param>
        /// <param name="value">関数文(トークン)</param>
        public void addFunction(Token key, List<Token> func = null)
        {
            string tokens = "";
            if (func != null) {
                foreach (Token token in func)
                    tokens += token.mValue + " ";
            }
            if (!mFunctions.ContainsKey(key.mValue)) {
                mFunctions.Add(key.mValue, new Token(tokens, TokenType.FUNCTION));
            } else {
                mFunctions[key.mValue] = new Token(tokens, TokenType.FUNCTION);
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
            List<double> listData = new List<double>();
            string arrayName = getSearchName(arg);
            foreach (var variable in getVariables(arrayName)) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING)
                        listData.Add(ylib.doubleParse(variable.Value.mValue));
                }
            }
            return listData;
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
            foreach (var variable in getVariables(arrayName)) {
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
            foreach (var variable in getVariables(arrayName)) {
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
                    ret[i,j] = ylib.doubleParse(getVariable(name).mValue);
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
            foreach (var variable in getVariables(arrayName)) {
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
            foreach (var variable in getVariables(arrayName)) {
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
        /// 配列戻り値に設定(2DToken)
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
                    //Token key = src[i, j].copy();
                    //key.mValue = destName + key.mValue;
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    setVariable(key, src[i,j].copy());
                }
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
    }
}
