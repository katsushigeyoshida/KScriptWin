using CoreLib;
using System.IO;
using System.Windows.Input;

namespace KScriptWin
{
    public class ControlData
    {
        public string mOutputString = "";
        public Key mKeyCode = Key.None;
        public bool mAbort = false;
        public bool mPause = false;
        public bool mKey = false;
    }

    /// <summary>
    /// スクリプトの処理
    ///     KParse で処理した構文を実行処理する
    /// 
    /// 関数の構成
    ///   KScript            スクリプトの読み込み登録(字句・構文解析)
    ///     execute             スクリプト文の逐次処理
    ///       exeStatements         複数構文の実行
    ///         exeStatement            構文の実行
    ///         　　letStatement              代入文の処理
    ///         　　  funcStatement               関数文の処理
    ///         　　  express                     数式処理
    ///         　　  setArrayData                配列の一括値設定
    ///         　　  getArrayVariable            配列変数名の変換
    ///         　　ifStatement               if文の処理
    ///         　　  conditinalStatement         条件文
    ///         　　  exeStatements               構文処理
    ///         　　whileStatement            while文の処理
    ///         　　  conditinalStatement         条件文
    ///         　　  exeStatements               構文処理
    ///         　　forStatement              for文の処理
    ///         　　  letStatement                代入文処理(初期値)
    ///         　　  conditinalStatement         条件文
    ///         　　  exeStatements               構文処理
    ///         　　returnStatement           return文の処理
    ///         　　  express                     数式処理
    ///         　　printStatement            print文の処理
    ///         　　  express                     数式処理
    ///         　　funcStatement             関数文の実行
    ///               function                    関数処理
    ///                 innerFunc                   追加内部関数(ScriptLib)
    ///                 programFunc                 プログラム関数
    ///                 function                     数式処理関数
    /// 
    /// 関数一覧
    /// KScript(string script)                                   コンストラクタ(字句・構文解析)
    /// execute(string funcName, List<Token> arg = null)        スクリプトの実行
    /// exeStatements(List<Token> tokens, int sp)               複数の文の処理
    /// exeStatement(List<Token> tokens)                        文の処理
    /// letStatement(List<Token> tokens)                        代入文の処理
    /// ifStatement(List<Token> tokens)                         if文の処理
    /// whileStatement(List<Token> tokens)                      while文の処理
    /// forStatement(List<Token> tokens)                        for文の処理
    /// printStatement(List<Token> tokens)                      print文の処理
    /// returnStatement(List<Token> tokens)                     return文の処理(return 変数に登録)
    /// funcStatement(List<Token> tokens, int sp, Token ret = null) 関数文の実行
    /// function(Token funcName, Token arg, Token ret = null)    関数処理
    /// 
    /// programFunc(string funcName, Token arg, Token ret)      プログラム関数の実行(別 KScript で実行)
    /// 
    /// conditinalStatement(List<Token> tokens, int sp = 0)     条件文の処理(比較) (a < b, a == b...)
    /// 
    /// express(Token token)                                    数式処理
    /// express(List<Token> tokens, int sp = 0)                 数式処理(文字列を含むときは文字列に変換して結合)
    /// funcExpress(string funcName, Token arg)                 数式関数の処理
    /// 
    /// getFuncArgs(string func, int sp = 0)                    プログラム関数引数をリストに変換
    /// setFuncArg(List<Token> src, List<Token> dest, KScript script)    プログラム関数の引数を関数側にコピー
    /// cnvArgList(Token arg)                                   引数をリストに変換( '(a,b,c)' →  'a','b','c' )
    /// getVariable(Token token)                                変数名を数値に変換
    /// getValueToken(string value)                             変数または配列変数を数値に変換
    /// getArrayVariable(Token token)                           配列変数名の変換([m,n] → [2,3])
    /// setArrayData(List<Token> tokens)                        配列の一括値設定(a[] = { 1,2,1,3} )
    /// setArrayData(Token name, Token data)                    1次元配列の一括値設定(a[] = { 1,2,1,3} )
    /// setArrayData2(Token name, Token data)                   2次元配列の一括値設定(a[,] = { { 1,2,1,3}, {2,3,4,5} } )
    /// getFuncArray(Token src, Token dest, KScript script)      プログラム関数の戻り値受け渡し
    /// setFuncArray(Token src, Token dest, KScript script)     プログラム関数の引数受け渡し
    /// 
    /// outputString(string str = "\n")                         表示出力(callbackを呼び出す)
    /// printToken(string title, List<Token> tokens, bool type = false, bool debug = false, bool console = false)
    /// tokensString(List<Token> tokens)                        デバッグ用トークンリストの文字列か
    /// 
    /// </summary>
    public class KScript
    {
        public static string[] mStatmantHelp = {
            "while (条件文){ 処理; }; : 処理の繰返し",
            "if (条件文) { 処理A; } else { 処理B; } : 処理の分岐",
            "for (初期値; 条件; 更新処理) { 処理; } : 決まった回数の繰返し",
            "return 戻り値; : 戻り値の指定",
            "break; : 繰り返し処理の中断",
            "continue; : 繰返し処理の先頭に戻る",
            "print(出力); : 値の出力",
            "println(出力); : 値の出力(末尾に改行追加)",
            "#include \"ファイル\"; : ファイル読込による機能追加",
            "exit; : プログラムの終了",
            "pause(\"message\"); プログラムを中断",
        };

        public enum RETURNTYPE { NORMAL, CONTINUE, BREAK, RETURN, ERROR }

        public List<Token> mTokenList = new List<Token>();              //  字句解析リスト
        public List<List<Token>> mStatements = new List<List<Token>>(); //  構文リスト

        public GraphView mGraph;                                //  グラフィックWindow
        public Plot3DView mPlot3D;                              //  3DグラフィックWindow
        public KParse mParse = new KParse();                    //  構文解析
        public KLexer mLexer = new KLexer();                    //  字句解析
        public Variable mVar = new Variable();                  //  変数管理
        public ScriptLib mScriptLib;                            //  内部関数ライブラリ
        public FuncPlot mFuncPlot;                              //  グラフィック関数
        public FuncPlot3D mFuncPlot3D;                          //  3Dグラフィック関数
        public FuncArray mFuncArray;                            //  配列関数
        public string mScriptFolder = "";                       //  プログラムファイルフォルダ

        public bool mDebug = false;
        public bool mDebugConsole = false;

        public Action printCallback;                            //  print文のコールバック関数
        public ControlData mControlData;                        //  データを参照渡しするため

        private string mFuncName = "";                          //  実行中の関数名
        private Util mUtil = new Util();
        private YCalc mCalc = new YCalc();                      //  数式処理
        private YLib ylib = new YLib();                         //  汎用関数

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KScript()
        {
            mScriptLib  = new ScriptLib(this);
            mFuncArray  = new FuncArray(this);
            mFuncPlot   = new FuncPlot(this);
            mFuncPlot3D = new FuncPlot3D(this);
            mControlData = new ControlData();
        }

        /// <summary>
        /// コンストラクタ(スクリプト文の設定)
        /// </summary>
        /// <param name="script">スクリプト文</param>
        public KScript(string script, GraphView graph, Plot3DView plot3D)
        {
            //  初期化
            clear();
            mGraph  = graph;
            mPlot3D = plot3D;
            mScriptLib  = new ScriptLib(this);
            mFuncArray  = new FuncArray(this);
            mFuncPlot   = new FuncPlot(this);
            mFuncPlot3D = new FuncPlot3D(this);
            mControlData = new ControlData();

            //  字句解析・ スクリプト登録(mFunctionsに登録)
            mTokenList  = mLexer.tokenList(script);
            mStatements = mParse.getStatements(mTokenList);
            if (0 < mParse.mErrorMessage.Length) {
                outputString(mParse.mErrorMessage);
            }
        }

        /// <summary>
        /// データクリア
        /// </summary>
        public void clear()
        {
            mStatements.Clear();
            mTokenList.Clear();
            mVar.mGlobalVar.Clear();
            mVar.mVariables.Clear();
            mParse.mFunctions.Clear();
            if (mGraph != null) mGraph.Close();
            if (mPlot3D != null) mPlot3D.Close();
        }

        /// <summary>
        /// スクリプトの設定
        /// </summary>
        /// <param name="script">スクリプト文</param>
        public void setScript(string script)
        {
            clear();
            addScript(script);
        }

        /// <summary>
        /// スクリプトの追加
        /// </summary>
        /// <param name="script">スクリプト文</param>
        public void addScript(string script)
        {
            mTokenList = mLexer.tokenList(script);
            mStatements.AddRange(mParse.getStatements(mTokenList));
            if (0 < mParse.mErrorMessage.Length) {
                outputString(mParse.mErrorMessage);
            }
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        public void execute(string funcName, List<Token> arg = null)
        {
            try {
                //  スクリプト関数処理部の抽出・登録
                mFuncName = funcName;
                List<List<Token>> funcStatement = null;
                if (mParse.mFunctions.ContainsKey(funcName)) {
                    //  関数指定
                    Token func = mParse.mFunctions[funcName];
                    List<Token> funcList = mParse.getStatement(mLexer.tokenList(func.mValue));
                    funcStatement = mParse.getStatements(mLexer.tokenList(mLexer.stripBracketString(funcList[2].mValue, funcList[2].mValue[0])));
                    if (0 < mParse.mErrorMessage.Length) {
                        outputString(mParse.mErrorMessage);
                    }

                    //  引数の登録
                    if (arg != null) {
                        string[] funcargs = mLexer.stripBracketString(funcList[1].mValue, '(').Split(',');
                        for (int i = 0; i < funcargs.Length; i++)
                            mVar.setVariable(new Token(funcargs[i].Trim(), TokenType.VARIABLE), arg[i]);
                    }
                }
                //  構文実行
                if (funcStatement == null)
                    funcStatement = mStatements;
                foreach (List<Token> statement in funcStatement) {
                    if (exeStatement(statement) != RETURNTYPE.NORMAL)
                        break;
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"{funcName} [{e.ToString()}]");
                if (funcName != "" && funcName != "main")
                    throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 複数の文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>実行結果</returns>
        private RETURNTYPE exeStatements(List<Token> tokens, int sp)
        {
            RETURNTYPE returnType = RETURNTYPE.NORMAL;
            List<Token> tokenList = new List<Token>();
            if (tokens[sp].mType == TokenType.STATEMENTS)
                tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue, tokens[sp].mValue[0]));
            else
                tokenList.AddRange(tokens.Skip(sp));
            List<List<Token>> statements = mParse.getStatements(tokenList);
            if (0 < mParse.mErrorMessage.Length) {
                outputString(mParse.mErrorMessage);
            }

            foreach (var statement in statements) {
                if (pause()) return RETURNTYPE.BREAK;
                returnType = exeStatement(statement);
                if (returnType != RETURNTYPE.NORMAL)
                    break;
            }
            return returnType;
        }

        /// <summary>
        /// 文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE exeStatement(List<Token> tokens)
        {
            printToken("", tokens, true, mDebug, mDebugConsole);
            if (tokens[0].mType == TokenType.VARIABLE ||
                tokens[0].mType == TokenType.ARRAY ||
                tokens[0].mType == TokenType.ASSIGNMENT) {
                return letStatement(tokens);
            } else if (tokens[0].mType == TokenType.STATEMENT) {
                if (tokens[0].mValue == "print") {
                    printStatement(tokens);
                } else if (tokens[0].mValue == "println") {
                    printStatement(tokens, true);
                } else if (tokens[0].mValue == "if") {
                    return ifStatement(tokens);
                } else if (tokens[0].mValue == "while") {
                    return whileStatement(tokens);
                } else if (tokens[0].mValue == "for") {
                    return forStatement(tokens);
                } else if (tokens[0].mValue == "return") {
                    returnStatement(tokens);
                    return RETURNTYPE.RETURN;
                } else if (tokens[0].mValue == "break") {
                    return RETURNTYPE.BREAK;
                } else if (tokens[0].mValue == "continue") {
                    return RETURNTYPE.CONTINUE;
                } else if (tokens[0].mValue == "exit") {
                    throw new Exception($"exit [{mFuncName}]");
                } else if (tokens[0].mValue == "pause") {
                    return pauseStatement(tokens);
                } else if (tokens[0].mValue == "#include") {
                    return includeStatemant(tokens);
                } else {
                    outputString($"Error: not found statement [{tokensString(tokens)}]\n");
                    return RETURNTYPE.ERROR;
                }
            } else if (tokens[0].mType == TokenType.FUNCTION) {
                return funcStatement(tokens, 0, null);
            } else if (tokens[0].mType == TokenType.COMMENT) {
                System.Diagnostics.Debug.WriteLine($"Comment: {tokens[0].mValue} ");
            } else {
                outputString($"Error: not found statement [{tokensString(tokens)}]\n");
                return RETURNTYPE.ERROR;
            }
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// include文の処理
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public RETURNTYPE includeStatemant(List<Token> tokens)
        {
            string scriptPath = tokens[1].getValue();
            scriptPath = Path.Combine(mScriptFolder, scriptPath);
            if (File.Exists(scriptPath)) {
                List<string> scriptData = ylib.loadListData(scriptPath);
                string code = string.Join("\n", scriptData);
                addScript(code);


                return RETURNTYPE.NORMAL;
            } else {
                outputString($"Error: not exists file [{scriptPath}]\n");
                return RETURNTYPE.ERROR;
            }
        }

        /// <summary>
        /// 代入文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public RETURNTYPE letStatement(List<Token> tokens)
        {
            Token variable = null;
            List<Token> expressList = new List<Token>();
            if (1 >= tokens.Count) return RETURNTYPE.NORMAL;
            if (0 <= tokens[0].mValue.IndexOf("[]") ||
                0 <= tokens[0].mValue.IndexOf(",]")) {
                //  配列一括設定
                if (tokens[2].mType == TokenType.STATEMENTS ||
                    tokens[2].mType == TokenType.ARRAY) {
                    if (setArrayData(tokens))
                        return RETURNTYPE.NORMAL;
                } else if (tokens[2].mType == TokenType.FUNCTION) {
                    return funcStatement(tokens, 2, tokens[0]);
                }
            } else if (tokens[0].mType == TokenType.VARIABLE ||
                tokens[0].mType == TokenType.ARRAY) {
                //  変数、配列に代入
                variable = getVariableName(tokens[0]);
                expressList.AddRange(tokens.Skip(2));
                if (tokens[1].mType == TokenType.ASSIGNMENT) {
                    if (tokens[1].mValue.Length == 2) {
                        //  複合演算子(++.--,+=,-=,*=,/=,^=)
                        //  [変数 = 変数 [+,-,*,/,^] 値] の形式に変換
                        Token token = new Token("1", TokenType.LITERAL);    //  '++','--'の時値
                                                                            //  '++','--' 以外の複合演算子
                        if (tokens[1].mValue[1] == '=') {
                            token = express(tokens, 2);
                            if (token == null) {
                                outputString($"Error: {tokensString(tokens)}\n");
                                return RETURNTYPE.ERROR;
                            }
                        }
                        expressList = new List<Token>(){
                            tokens[0],
                            new Token(tokens[1].mValue[0].ToString(), TokenType.OPERATOR),
                            token
                        };
                    }
                    Token value = express(expressList);
                    if (value != null) {
                        mVar.setVariable(variable, value);
                        return RETURNTYPE.NORMAL;
                    }
                }
            } else if (tokens[0].mType == TokenType.ASSIGNMENT) {
                //  '++','--'の時
                if (1 < tokens[0].mValue.Length && tokens[0].mValue[1] != '=') {
                    Token token = new Token("1", TokenType.LITERAL);
                    expressList = new List<Token>(){
                            tokens[1],
                            new Token(tokens[0].mValue[0].ToString(), TokenType.OPERATOR),
                            token
                        };
                    variable = getVariableName(tokens[1]);
                    Token v = express(expressList);
                    if (v == null) return RETURNTYPE.ERROR;
                    mVar.setVariable(variable, v);
                    return RETURNTYPE.NORMAL;
                }
            }
            outputString($"Error: {tokensString(tokens)}\n");
            return RETURNTYPE.ERROR;
        }

        /// <summary>
        /// if文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE ifStatement(List<Token> tokens)
        {
            List<Token> statment, tokenList;
            int n = tokens.FindIndex(p => p.mValue == "else");
            while (0 <= n) {
                statment = tokens.Take(n).ToList();    //  if ... (else)
                if (0 == statment.Count || statment[0].mValue != "if") break;
                if (conditinalStatement(statment, 1)) {
                    tokenList = mParse.getStatement(statment, 2);
                    return exeStatements(tokenList, 0);
                }
                tokens = tokens.Skip(n + 1).ToList();
                n = tokens.FindIndex(p => p.mValue == "else");
            }
            if (0 < tokens.Count && tokens[0].mValue == "if") {
                if (conditinalStatement(tokens, 1)) {
                    tokenList = mParse.getStatement(tokens, 2);
                    return exeStatements(tokenList, 0);
                } else
                    return RETURNTYPE.NORMAL;
            }
            tokenList = mParse.getStatement(tokens);
            return exeStatements(tokenList, 0);
        }

        /// <summary>
        /// while文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE whileStatement(List<Token> tokens)
        {
            while (conditinalStatement(tokens, 1)) {
                if (pause()) return RETURNTYPE.BREAK;
                RETURNTYPE returnType = exeStatements(tokens, 2);
                if (returnType == RETURNTYPE.BREAK)
                    break;
                else if (returnType == RETURNTYPE.RETURN)
                    return RETURNTYPE.RETURN;
            }
            //calcError("whileStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// for文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE forStatement(List<Token> tokens)
        {
            List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
            List<List<Token>> funcStatement = mParse.getStatements(tokenList);
            if (0 < mParse.mErrorMessage.Length) {
                outputString(mParse.mErrorMessage);
            }

            if (funcStatement.Count == 3) {
                letStatement(funcStatement[0]);                     //  初期値
                while (conditinalStatement(funcStatement[1])) {     //  条件
                    if (pause()) return RETURNTYPE.BREAK;
                    RETURNTYPE returnType = exeStatements(tokens, 2);
                    if (returnType == RETURNTYPE.BREAK)
                        break;
                    else if (returnType == RETURNTYPE.RETURN || returnType == RETURNTYPE.ERROR)
                        return returnType;
                    letStatement(funcStatement[2]);                 //  更新処理
                }
            } else {
                //  Error
                outputString($"Error: forStatement [{tokens[1]}]\n");
                return RETURNTYPE.ERROR;
            }
            //calcError("forStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// print文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public RETURNTYPE printStatement(List<Token> tokens, bool lf = false)
        {
            if (tokens.Count <= 1) {
                //  データなし改行のみ
                outputString();
            } else if (tokens[1].mType == TokenType.EXPRESS) {
                List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
                if (tokenList == null || tokenList.Count == 0) {
                    //  データなし改行のみ
                    outputString();
                } else {
                    //  コンマ区切りのデータ表示
                    List<Token> expList = new List<Token>();
                    string buf = "";
                    for (int i = 0; i < tokenList.Count; i++) {
                        if (tokenList[i].mType == TokenType.DELIMITER) {
                            Token v = express(expList);
                            buf += v.getValue();
                            expList = new List<Token>();
                        } else if (i == tokenList.Count - 1) {
                            expList.Add(tokenList[i]);
                            Token v = express(expList);
                             buf += v.getValue();
                        } else {
                            expList.Add(tokenList[i]);
                        }
                    }
                    if (0 < buf.Length)
                        outputString(lf ? buf + "\n" : buf);
                }
            } else if (tokens[1].mType == TokenType.STRING) {
                string buf = tokens[1].mValue.Replace("\\n", "\n");
                buf = ylib.stripBracketString(buf, '"');
                outputString(buf);
            } else {
                //  Error
                outputString($"Error: printStatement [{tokens[1]}]\n");
                return RETURNTYPE.ERROR;
            }
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// return文の処理(return 変数に登録)
        /// </summary>
        /// <param name="tokens">トークカリスト</param>
        public RETURNTYPE returnStatement(List<Token> tokens)
        {
            Token token;
            if (0 <= tokens[1].mValue.IndexOf("[]") || 0 <= tokens[1].mValue.IndexOf("[,]"))
                token = tokens[1].copy();
            else
                token = express(tokens, 1);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), token);

            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// pause文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns></returns>
        public RETURNTYPE pauseStatement(List<Token> tokens)
        {
            string msg = "";
            if (1 < tokens.Count)
                msg = ylib.getBracketString(tokens[1].mValue, 0, '"', false);
            mControlData.mPause = true;
            if (pause(msg)) return RETURNTYPE.BREAK;
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// 関数文の実行
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <param name="ret">配列の戻り値</param>
        /// <returns>戻り値</returns>
        public RETURNTYPE funcStatement(List<Token> tokens, int sp, Token ret = null)
        {
            Token funcName = tokens[sp];
            Token arg = tokens[sp + 1];
            Token result = function(funcName, arg, ret);
            if (result == null || result.mType == TokenType.ERROR)
                return RETURNTYPE.ERROR;
            mVar.setVariable(new Token("return", TokenType.VARIABLE), result);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// 関数処理
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">戻り値</param>
        /// <returns></returns>
        private Token function(Token funcName, Token arg, Token ret = null)
        {
            try {
                Token result;
                if (0 == funcName.mValue.IndexOf("plot.") || 0 == funcName.mValue.IndexOf("graph."))
                    result = mFuncPlot.plotFunc(funcName, arg, ret);    //  グラフィック関数
                else if (0 == funcName.mValue.IndexOf("plot3D."))
                    result = mFuncPlot3D.plotFunc(funcName, arg, ret);  //  3Dグラフィック関数
                else if (0 == funcName.mValue.IndexOf("array."))
                    result = mFuncArray.function(funcName, arg, ret);   //  配列関数
                else
                    result = mScriptLib.innerFunc(funcName, arg, ret);  //  内部関数処理
                if (result != null && result.mType != TokenType.ERROR)
                    return result;

                if (mParse.mFunctions.ContainsKey(funcName.mValue))
                    return programFunc(funcName.mValue, arg, ret);      //  プログラムの関数
                else
                    result = funcExpress(funcName.mValue, arg);         //  数式処理の関数

                if (result == null || result.mType == TokenType.ERROR) {
                    outputString($"Error: not found function [{funcName.mValue}]\n");
                    return new Token(funcName.mValue, TokenType.ERROR);
                } else
                    return result;
            } catch (Exception e) {
                if (0 <= e.Message.IndexOf("exit"))
                    throw new Exception(e.Message);
                outputString($"Error: function [{funcName.mValue}]\n {e.Message}\n");
                return new Token(funcName.mValue, TokenType.ERROR);
            }
        }

        /// <summary>
        /// プログラム関数の実行(別 KScript で実行)
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">配列の戻り値</param>
        /// <returns>戻り値(関数側の変数名)</returns>
        private Token programFunc(string funcName, Token arg, Token ret)
        {
            KScript script = new KScript(mParse.mFunctions[funcName].mValue, mGraph, mPlot3D);
            script.mControlData = mControlData;
            script.printCallback = printCallback;
            script.mVar.mGlobalVar = mVar.mGlobalVar;           //  グローバル変数
            script.mParse.mFunctions = mParse.mFunctions;       //  参照関数の設定
            List<Token> callArgs = getFuncArgs(arg.mValue);     //  呼出し側引数の取得(配列以外は数値に変換)
            List<Token> funcArgs = getFuncArgNames(mParse.mFunctions[funcName].mValue, 1);  //  関数側引数名の取得
            setFuncArg(callArgs, funcArgs, script);             //  呼出し側から関数側に値を渡す

            script.execute(funcName, null);                     //  関数の実行
            mGraph = script.mGraph;
            mPlot3D = script.mPlot3D;

            //  戻り値の設定
            if (script.mVar.mVariables.ContainsKey("return")) {
                getFuncArray(script.mVar.mVariables["return"], ret, script);
                return script.mVar.mVariables["return"];
            } else
                return new Token("", TokenType.LITERAL);
        }

        /// <summary>
        /// 条件文の処理(比較) (a < b, a == b...)
        /// </summary>
        /// <param name="tokens">条件文</param>
        /// <param name="sp">開始位置</param>
        /// <returns>処理結果</returns>
        public bool conditinalStatement(List<Token> tokens, int sp = 0)
        {
            try {
                int cindex = tokens.FindIndex(p => p.mType == TokenType.CONDITINAL);
                if (2 < tokens.Count && cindex < 1)
                    tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue));
                else if (tokens.Count == 1)
                    tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[0].mValue));

                if (tokens.Count == 1) return true;     //  条件式が定数の場合
                if (tokens.Count == 2) return false;    //  条件式意味不明

                List<Token> a = new List<Token>();
                List<Token> b = new List<Token>();
                int n = 0;
                while (n < tokens.Count && tokens[n].mType != TokenType.CONDITINAL)
                    a.Add(tokens[n++]);
                if (n == tokens.Count) return false;        //  Error
                Token cond = tokens[n++];
                while (n < tokens.Count && tokens[n].mType != TokenType.DELIMITER)
                    b.Add(tokens[n++]);
                switch (cond.mValue) {
                    case "||": return conditinalStatement(a) || conditinalStatement(b);
                    case "&&": return conditinalStatement(a) && conditinalStatement(b);
                    case "!": return !conditinalStatement(b);
                }
                Token aa = express(a, 0);
                Token bb = express(b, 0);
                if (aa.mType == TokenType.STRING || bb.mType == TokenType.STRING) {
                    string aaa = ylib.stripBracketString(aa.mValue, '"');
                    string bbb = ylib.stripBracketString(bb.mValue, '"');
                    switch (cond.mValue) {
                        case "==": return aaa.CompareTo(bbb) == 0;
                        case "!=": return aaa.CompareTo(bbb) != 0;
                        case "<": return aaa.CompareTo(bbb) < 0;
                        case ">": return aaa.CompareTo(bbb) > 0;
                        case "<=": return aaa.CompareTo(bbb) <= 0;
                        case ">=": return aaa.CompareTo(bbb) >= 0;
                        default:    //  Error
                            outputString($"ERROR: not conditional code [{cond.mValue}]\n");
                            break;
                    }
                } else {
                    double avalue = ylib.doubleParse(aa.mValue);
                    double bvalue = ylib.doubleParse(bb.mValue);
                    switch (cond.mValue) {
                        case "==": return avalue == bvalue;
                        case "!=": return avalue != bvalue;
                        case "<": return avalue < bvalue;
                        case ">": return avalue > bvalue;
                        case "<=": return avalue <= bvalue;
                        case ">=": return avalue >= bvalue;
                        default:    //  Error
                            outputString($"ERROR: not conditional code [{cond.mValue}]\n");
                            break;
                    }
                }
                return false;
            } catch (Exception e) {
                if (0 <= e.Message.IndexOf("exit"))
                    throw new Exception(e.Message);
                outputString($"ERROR: conditional Statement [{e.Message}]\n");
                return false;
            }
        }

        /// <summary>
        /// 数式処理
        /// </summary>
        /// <param name="token">トークン</param>
        /// <returns>計算結果</returns>
        public Token express(Token token)
        {
            return express(new List<Token>() { token });
        }

        /// <summary>
        /// 数式処理(文字列を含むときは文字列に変換して結合)
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>計算結果(文字列)</returns>
        private Token express(List<Token> tokens, int sp = 0)
        {
            Token buf = null;
            Token token = null;
            for (int i = sp; i < tokens.Count; i++) {
                if (tokens[i].mType == TokenType.DELIMITER) {
                    break;
                } else if (tokens[i].mType == TokenType.LITERAL) {
                    token = tokens[i];
                } else if (tokens[i].mType == TokenType.VARIABLE) {
                    token = getVariableValue(tokens[i]);
                } else if (tokens[i].mType == TokenType.ARRAY) {
                    token = getVariableValue(tokens[i]);
                } else if (tokens[i].mType == TokenType.STRING) {
                    token = tokens[i];
                } else if (tokens[i].mType == TokenType.CONSTANT) {
                    token = tokens[i];
                    token.mValue = mCalc.expression(token.mValue).ToString();
                    token.mType = TokenType.LITERAL;
                } else if (tokens[i].mType == TokenType.EXPRESS) {
                    List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[i].mValue));
                    token = express(tokenList);
                } else if (tokens[i].mType == TokenType.FUNCTION) {
                    token = function(tokens[i], tokens[i + 1]);
                    i++;
                } else if (tokens[i].mType == TokenType.OPERATOR) {
                    token = tokens[i];
                } else if (tokens[i].mType == TokenType.ASSIGNMENT) {
                    token = tokens[i].copy();
                    if (tokens[i].mValue[1] == '=') {
                        token.mValue = tokens[i].mValue[0].ToString() + getVariableValue(tokens[i - 1]).mValue;
                    } else {
                        token.mValue = tokens[i].mValue[0].ToString() + "1";
                    }
                } else {
                    //  ERROR
                    outputString($"ERROR: not express word [{tokens[i]}]\n");
                    return new Token("", TokenType.ERROR);
                }
                if (token == null || token.mType == TokenType.ERROR)
                    return new Token("", TokenType.ERROR);
                if (buf == null && token.mType != TokenType.ASSIGNMENT) {
                    buf = token.copy();
                } else if (token.mType == TokenType.ASSIGNMENT) {
                    Token tmpValue, tmpKey;
                    if (0 < i && tokens[i - 1].mType == TokenType.VARIABLE)
                        tmpKey = tokens[i - 1];
                    else
                        tmpKey = tokens[i + 1];
                    tmpValue = getVariableValue(tmpKey);
                    tmpValue.mValue += token.mValue;
                    tmpValue.mValue = mCalc.expression(tmpValue.mValue).ToString();
                    tmpValue.mType = TokenType.LITERAL;
                    if (i + 1 < tokens.Count && tokens[i + 1].mType == TokenType.VARIABLE) {
                        buf = tmpValue.copy();
                        i++;
                    } else if (tokens[i].mValue[1] == '=')
                        i++;
                    mVar.setVariable(tmpKey, tmpValue);
                } else if (buf.mType == TokenType.STRING || token.mType == TokenType.STRING) {
                    if (0 < i && tokens[i - 1].mType == TokenType.OPERATOR)
                        buf.mValue = buf.mValue.Remove(buf.mValue.Length - 1);
                    buf.mValue += token.mValue;
                    buf.mType = TokenType.STRING;
                } else if (token.mType == TokenType.OPERATOR) {
                    buf.mValue += token.mValue;
                    buf.mType = TokenType.EXPRESS;
                } else {
                    buf.mValue += token.mValue;
                }
            }
            if (buf != null && buf.mType != TokenType.STRING) {
                buf.mValue = mCalc.expression(buf.mValue).ToString();
                buf.mType = TokenType.LITERAL;
                return buf;
            } else if (buf != null && buf.mType == TokenType.STRING) {
                return buf;
            } else
                return new Token("Error: express", TokenType.ERROR);
        }

        /// <summary>
        /// 数式関数の処理
        /// </summary>
        /// <param name="funcName">数式関数</param>
        /// <param name="arg">引数</param>
        /// <returns>計算結果</returns>
        private Token funcExpress(string funcName, Token arg)
        {
            List<Token> argValue = cnvArgList(arg);
            if (argValue == null || argValue.Count == 0 || argValue[0].mType == TokenType.ERROR) {
                outputString($"Error: funcExpress [{funcName}]\n");
                return new Token("", TokenType.ERROR);
            }
            string buf = "";
            foreach (Token token in argValue)
                buf += express(token).mValue + ",";
            buf = buf.TrimEnd(',');
            string result = mCalc.expression($"{funcName}({buf})").ToString();
            if (mCalc.mError) {
                outputString($"Error: funcExpress [{mCalc.mErrorMsg}]\n");
                return new Token("", TokenType.ERROR); ;
            }
            return new Token(result, TokenType.LITERAL);
        }

        /// <summary>
        /// 引数をリストに変換( '(a,b,c)' →  'a','b','c' )
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>引数リスト</returns>
        private List<Token> cnvArgList(Token arg)
        {
            List<Token> argValue = new List<Token>();
            if (arg.mValue == null || arg.mValue == "") {
                argValue.Add(new Token("Error: cnvArgList", TokenType.ERROR));
            } else if (arg.mType == TokenType.EXPRESS && 0 > arg.mValue.IndexOf(',')) {
                argValue.Add(express(new Token(arg.mValue.Trim(), TokenType.EXPRESS)));
            } else {
                List<string> args = mLexer.commaSplit(mLexer.stripBracketString(arg.mValue, '('));
                for (int i = 0; i < args.Count; i++) {
                    if (0 <= args[i].IndexOf("[@]") || 0 <= args[i].IndexOf("[%]"))
                        argValue.Add(new Token(args[i].Trim(), TokenType.STRING));
                    else if (0 <= args[i].IndexOf("("))
                        argValue.Add(new Token(args[i].Trim(), TokenType.EXPRESS));
                    else if (0 <= args[i].IndexOf("["))
                        argValue.Add(express(new Token(args[i].Trim(), TokenType.ARRAY)));
                    else
                        argValue.Add(express(new Token(args[i].Trim(), TokenType.EXPRESS)));
                }
            }
            return argValue;
        }

        /// <summary>
        /// プログラム関数引数をリストに変換
        /// funcName(args) { staetment .. } → List(args)
        /// </summary>
        /// <param name="func">引数文字列</param>
        /// <param name="sp">開始位置</param>
        /// <returns>引数リスト</returns>
        public List<Token> getFuncArgs(string func, int sp = 0)
        {
            List<Token> args = new List<Token>();
            List<Token> funcList = mParse.getStatement(mLexer.tokenList(func));
            List<string> funcargs = mLexer.commaSplit(mLexer.stripBracketString(funcList[sp].mValue, '('));
            for (int i = 0; i < funcargs.Count; i++)
                args.Add(getVariableValue(new Token(funcargs[i].Trim())));
            return args;
        }

        /// <summary>
        /// プログラム関数の引数名の取得
        /// </summary>
        /// <param name="func">関数スクリプト</param>
        /// <param name="sp">引数の位置</param>
        /// <returns>引数名リスト</returns>
        public List<Token> getFuncArgNames(string func, int sp = 0)
        {
            List<Token> args = new List<Token>();
            List<Token> funcList = mParse.getStatement(mLexer.tokenList(func));
            List<string> funcargs = mLexer.commaSplit(mLexer.stripBracketString(funcList[sp].mValue, '('));
            for (int i = 0; i < funcargs.Count; i++)
                args.Add(new Token(funcargs[i].Trim()));
            return args;
        }

        /// <summary>
        /// プログラム関数の引数を関数側にコピー
        /// </summary>
        /// <param name="src">呼出側引数</param>
        /// <param name="dest">関数側引数</param>
        /// <param name="script">関数 KScript</param>
        private void setFuncArg(List<Token> src, List<Token> dest, KScript script)
        {
            for (int i = 0; i < src.Count; i++) {
                if (0 <= src[i].mValue.IndexOf("[")) {
                    //  配列のコピー
                    setFuncArray(src[i], dest[i], script);
                } else {
                    string buf = getVariableValue(src[i]).mValue;
                    script.mVar.setVariable(dest[i], express(new Token(buf)));
                }
            }
        }

        /// <summary>
        /// 変数の値の取得
        /// </summary>
        /// <param name="token">変数名</param>
        /// <returns>値</returns>
        public Token getVariableValue(Token token)
        {
            var v = getVariableName(token);
            if (v.mType == TokenType.EXPRESS)
                v = express(v);
            return mVar.getVariable(v);
        }

        /// <summary>
        /// 変数名の変換(配列のインデックスを変換)
        /// </summary>
        /// <param name="token">変数名</param>
        /// <returns>変数名</returns>
        public Token getVariableName(Token token)
        {
            if (token.mType == TokenType.ARRAY) {
                string arrayname = token.mValue.Substring(0, token.mValue.IndexOf('['));
                string index = ylib.stripBracketString(ylib.getBracketString(token.mValue, 0, '['), '[');
                List<string> varables = mLexer.commaSplit(index);
                List<Token> tokens = new List<Token>();
                for (int i = 0; i < varables.Count; i++)
                    tokens.Add(getVariableValue(new Token(varables[i].Trim())));
                string variable = "[";
                for (int i = 0; i < tokens.Count; i++)
                    variable += tokens[i].mValue + ",";
                if (variable[variable.Length - 1] == ',')
                    variable = variable.Remove(variable.Length - 1);
                variable = arrayname + variable + "]";
                return new Token(variable);
            } else {
                return token;
            }
        }

        /// <summary>
        /// 変数または配列変数を数値に変換
        /// m + 2 →  3 + 2 → 5
        /// a[m, n+1] → a[2,3+1] → a[2,4] → 5
        /// </summary>
        /// <param name="value">変数または配列変数</param>
        /// <returns>数値</returns>
        public Token getValueToken(string value)
        {
            string buf = "";
            int sp = value.IndexOf("[");
            if (0 <= sp) {
                //  配列引数
                List<Token> tokens = mLexer.splitArgList(value);
                buf = tokens[0].mValue;
                for (int i = 1; i < tokens.Count; i++) {
                    if (tokens[i].mType == TokenType.VARIABLE ||
                        tokens[i].mType == TokenType.EXPRESS) {
                        buf += express(tokens[i]).mValue;
                    } else {
                        buf += tokens[i].mValue;
                    }
                }
            } else {
                //  通常の引数
                buf = express(new Token(value)).mValue.Trim();
            }
            return getVariableValue(new Token(buf));
        }

        /// <summary>
        /// 配列の一括値設定(代入処理)(a[] = { 1,2,1,3} )
        /// tokens[0] = tokens[2]   (tokens[1] = [=])
        /// </summary>
        /// <param name="tokens">設定文(トークンリスト)</param>
        /// <returns>可否</returns>
        private bool setArrayData(List<Token> tokens)
        {
            Token name = convVariable(tokens[0]);
            Token data = convVariable(tokens[2]);
            if (0 <= name.mValue.IndexOf("[]"))
                return setArrayData(name, data);    //  1次元配列
            else if (0 <= name.mValue.IndexOf(",]"))
                return setArrayData2(name, data);   //  2次元配列
            return false;
        }

        /// <summary>
        /// 1次元配列の代入処理
        /// a[] = { 1,2,1,3};
        /// a[] = b[];
        /// a[] = b[n,];
        /// </summary>
        /// <param name="name">代入先配列名</param>
        /// <param name="data">代入元設定値</param>
        /// <returns>可否</returns>
        private bool setArrayData(Token name, Token data)
        {
            string arrayName = name.mValue.Substring(0, name.mValue.IndexOf('['));
            mVar.clearVariables(name);
            if (0 <= data.mValue.IndexOf("{")) {
                // a[] = { 1,2,3..};
                List<Token> dataList= convLiteralList(data);
                for (int i = 0; i < dataList.Count; i++) {
                    string buf = $"{arrayName}[{i}]";
                    mVar.setVariable(new Token(buf, TokenType.VARIABLE), dataList[i]);
                }
                return true;
            } else if (0 <= data.mValue.IndexOf("[]")) {
                // a[] = b[];
                Dictionary<string, Token> dataList = mVar.getVariables(data);
                foreach (var vari in dataList) {
                    string buf = $"{arrayName}{vari.Key.Substring(vari.Key.IndexOf('['))}";
                    mVar.setVariable(new Token(buf, TokenType.ARRAY), vari.Value);
                }
                return true;
            } else if (0 <= data.mValue.IndexOf(",]")) {
                // a[] = b[n,];
                Dictionary<string, Token> dataList = mVar.getVariables(data);
                foreach (var vari in dataList) {
                    string buf = $"{arrayName}[{vari.Key.Substring(vari.Key.LastIndexOf(",") + 1)}";
                    mVar.setVariable(new Token(buf, TokenType.ARRAY), vari.Value);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 2次元配列の一括値設定
        /// a[,] = { { 1,2,1,3}, {2,3,4,5} };
        /// a[n,] = { 1,2,3 }; 
        /// a[,] = b[,];
        /// a[n,] = b[n,];
        /// a[n,] = b[];
        /// </summary>
        /// <param name="name">配列名</param>
        /// <param name="data">設定値</param>
        /// <returns></returns>
        private bool setArrayData2(Token name, Token data)
        {
            string arrayName = name.mValue.Substring(0, name.mValue.IndexOf('['));
            mVar.clearVariables(name);
            if (0 <= data.mValue.IndexOf("{")) {
                //  一括設定(a[,] = {{1,2,3},{3,4,5}..};)
                if (0 <= name.mValue.IndexOf("[,]")) {
                    //  a[,] = {{1,2,3},{2,3,4}...};
                    List<List<Token>> dataList = convLiteral2List(data);
                    for (int i = 0; i < dataList.Count; i++) {
                        for (int j = 0; j < dataList[i].Count; j++) {
                            string buf = $"{arrayName}[{i},{j}]";
                            mVar.setVariable(new Token(buf, TokenType.VARIABLE), express(dataList[i][j]));
                        }
                    }
                } else {
                    //  a[n,] = { 1,2,3 };
                    arrayName = name.mValue.Substring(0, name.mValue.LastIndexOf(','));
                    arrayName = arrayName.Replace(" ", "");
                    List<Token> dataList = convLiteralList(data);
                    for (int i = 0; i < dataList.Count; i++) {
                        string buf = $"{arrayName},{i}]";
                        mVar.setVariable(new Token(buf, TokenType.VARIABLE), express(dataList[i]));
                    }
                }
                return true;
            } else if (0 <= data.mValue.IndexOf("[,]")) {
                //  a[,] = b[,];
                if (0 <= name.mValue.IndexOf("[,]")) {
                    Dictionary<string, Token> dataList = mVar.getVariables(data);
                    foreach (var vari in dataList) {
                        string buf = $"{arrayName}{vari.Key.Substring(vari.Key.IndexOf('['))}";
                        mVar.setVariable(new Token(buf, TokenType.ARRAY), vari.Value);
                    }
                    return true;
                }
            } else if (0 <= data.mValue.IndexOf(",]")) {
                //  a[n,] = b[n,];
                arrayName = name.mValue.Substring(0, name.mValue.LastIndexOf(','));
                Dictionary<string, Token> dataList = mVar.getVariables(data);
                foreach (var vari in dataList) {
                    string buf = $"{arrayName},{vari.Key.Substring(vari.Key.LastIndexOf(",") + 1)}";
                    mVar.setVariable(new Token(buf, TokenType.ARRAY), vari.Value);
                }
                return true;
            } else if (0 <= data.mValue.IndexOf("[]")) {
                //  a[n,] = b[];
                arrayName = name.mValue.Substring(0, name.mValue.LastIndexOf(','));
                Dictionary<string, Token> dataList = mVar.getVariables(data);
                foreach (var vari in dataList) {
                    string buf = $"{arrayName},{vari.Key.Substring(vari.Key.LastIndexOf("[") + 1)}";
                    mVar.setVariable(new Token(buf, TokenType.ARRAY), vari.Value);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 変数、配列変数、数式をリテラルに変換する(変換でないものはそのまま)
        /// </summary>
        /// <param name="array">変数、配列変数、数式</param>
        /// <returns>変換変数</returns>
        private Token convVariable(Token array)
        {
            return new Token(convVariable(array.mValue));
        }

        /// <summary>
        /// 変数、配列変数、数式をリテラルに変換する(変換でないものはそのまま)
        /// </summary>
        /// <param name="array">変数、配列変数、数式</param>
        /// <returns>変換変数</returns>
        private string convVariable(string array)
        {
            List<string> arrayList = splitArrayVariable(array);
            string buf = "";
            foreach (var vari in arrayList) {
                if (0 <= vari.IndexOf('[') && 0 <= vari.IndexOf(']')) {
                    buf += convVariable(vari);
                } else if (0 <= vari.IndexOf('[') || 0 <= vari.IndexOf(']')
                 || 0 <= vari.IndexOf(',')) {
                    buf += vari;
                    if (isArrayVariable(buf)) {
                        buf = mVar.getVariable(buf).mValue;
                    }
                } else if (0 <= vari.IndexOf('{') || 0 <= vari.IndexOf('}')) {
                    buf += vari;
                } else {
                    buf += express(new Token(vari)).mValue;
                }
            }
            return buf;
        }

        /// <summary>
        /// 配列変数を分解する (a[b[n,0],0] →  a[ b[n,0] , 0 ]
        /// </summary>
        /// <param name="text">配列変数文字列</param>
        /// <returns>分解リスト</returns>
        private List<string> splitArrayVariable(string text)
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
        private bool isArrayVariable(string vari)
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
        /// 配列のリテラル値({ 1,2,3})を Tokenリストに変換
        /// </summary>
        /// <param name="data">1次元リテラル配列</param>
        /// <returns>データリスト</returns>
        private List<Token> convLiteralList(Token data)
        {
            string str = mLexer.stripBracketString(data.mValue, '{');
            List<Token> datas = mLexer.tokenList(str);
            List<List<Token>> dataList = mLexer.tokensList(datas);
            List<Token> literalData = new List<Token>();
            for (int i = 0; i < dataList.Count; i++) {
                literalData.Add(express(dataList[i]));
            }
            return literalData;
        }

        /// <summary>
        /// 二次元配列のリテラル値({{1,2,3},{4,5,6},..})を Tokenリストに変換
        /// </summary>
        /// <param name="data">2次元リテラル配列</param>
        /// <returns>データリスト</returns>
        private List<List<Token>> convLiteral2List(Token data)
        {
            string str = mLexer.stripBracketString(data.mValue, '{');
            List<string> strings = mLexer.getBracketStringList(str, 0, '{');
            List<List<Token>> literalDatas = new List<List<Token>>();
            for (int i = 0; i < strings.Count; i++) {
                List<Token> datas = mLexer.tokenList(mLexer.stripBracketString(strings[i], '{'));
                List<List<Token>> dataList = mLexer.tokensList(datas);
                List<Token> buf = new List<Token>();
                for (int j = 0; j < dataList.Count; j++) {
                    buf.Add(express(dataList[j]));
                }
                literalDatas.Add(buf);
            }
            return literalDatas;
        }

        /// <summary>
        /// プログラム関数の戻り値受け渡し
        /// 関数の戻り値(配列)を呼出し側の変数にコピー(script.src → dest)
        /// </summary>
        /// <param name="src">関数側の戻り値</param>
        /// <param name="dest">呼出し側の変数</param>
        /// <param name="script">関数 KScript</param>
        private void getFuncArray(Token src, Token dest, KScript script)
        {
            if (src == null || dest == null || script == null) return;
            int sp = src.mValue.IndexOf("[");
            int dp = dest.mValue.IndexOf("[");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in script.mVar.getVariableList(src)) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    mVar.setVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// プログラム関数の配列引数受け渡し(src → script.dest)
        /// </summary>
        /// <param name="src">呼出し元引数</param>
        /// <param name="dest">関数側引数</param>
        /// <param name="script">関数 KScript</param>
        private void setFuncArray(Token src, Token dest, KScript script)
        {
            int sp = src.mValue.IndexOf("[");
            int dp = dest.mValue.IndexOf("[");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in mVar.getVariableList(srcName)) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    script.mVar.setVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// 表示出力(callbackを呼び出す)
        /// </summary>
        /// <param name="str"></param>
        public void outputString(string str = "\n")
        {
            mControlData.mOutputString = str;
            printCallback();
        }

        /// <summary>
        /// デバッグ用トークンリスト表示
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="type">VALUEとTYPE両表示</param>
        /// <param name="debug">非表示</param>
        /// <param name="console">コンソール出力</param>
        private void printToken(string title, List<Token> tokens, bool type = false, bool debug = false, bool console = false)
        {
            if (!debug) return;
            if (!console) {
                System.Diagnostics.Debug.Write($"{title} ");
                if (type)
                    tokens.ForEach(p => System.Diagnostics.Debug.Write($"{p} "));
                else
                    tokens.ForEach(p => System.Diagnostics.Debug.Write($"{p.mValue} "));
                System.Diagnostics.Debug.WriteLine("");
            } else {
                outputString($"{title} ");
                tokens.ForEach(p => outputString($"{p.mValue} "));
                outputString("\n");
            }
        }

        /// <summary>
        /// デバッグ用トークンリストの文字列化
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>文字列</returns>
        private string tokensString(List<Token> tokens)
        {
            string buf = "";
            foreach (var token in tokens)
                buf += token.mValue + " ";
            buf.Trim();
            return buf;
        }

        /// <summary>
        /// Pause機能 
        /// </summary>
        /// <returns>true(Abort)</returns>
        private bool pause(string msg = "")
        {
            if (mControlData.mAbort) return true;
            if (0 < msg.Length && mControlData.mPause)
                outputString($"[{DateTime.Now.ToString("HH:mm:ss")}] puse: [{msg}]\n");
            while (mControlData.mPause) {
                if (mControlData.mAbort) return true;
                Thread.Sleep(100);
                ylib.DoEvents();
                continue;
            }
            return false;
        }
    }
}
