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
        public Dictionary<string, Token> mVariables = new Dictionary<string, Token>();  //  変数リスト(変数名,値)
        public Dictionary<string, Token> mFunctions = new Dictionary<string, Token>();  //  関数リスト(関数名,(関数式))

        public KParse() { }

        /// <summary>
        /// ステートメントごとにトークンリストを抽出
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>ステートメントリスト</returns>
        public List<List<Token>> getStatements(List<Token> tokens)
        {
            List<List<Token>> statemets = new List<List<Token>>();
            for (int i = 0; i < tokens.Count; i++) {
                List<Token> statement = new List<Token>();
                int sp = i, ep = i;
                switch (tokens[i].mType) {
                    case TokenType.VARIABLE:
                    case TokenType.ARRAY:
                    case TokenType.CONSTANT:
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
        public void addVariable(Token key, Token value = null)
        {
            if (!mVariables.ContainsKey(key.mValue)) {
                mVariables.Add(key.mValue, value);
            } else {
                mVariables[key.mValue] = value;
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
    }
}
