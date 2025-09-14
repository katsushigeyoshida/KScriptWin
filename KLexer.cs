using CoreLib;

namespace KScriptWin
{
    //  字句の種類
    public enum TokenType
    {
        VARIABLE,               //  変数名
        ARRAY,                  //  配列変数名([],[,])
        OPERATOR,               //  演算子(+,-,*,/,%,^)
        CONDITINAL,             //  条件演算子(==,!=,<,>,<=,>=)
        ASSIGNMENT,             //  代入演算子(=,+=,-=,*=,/=,^=,++,--)
        DELIMITER,              //  区切り文字((,),{,},[,],;,',',)
        LITERAL,                //  固定値(数値)
        STRING,                 //  固定値(文字列)
        CONSTANT,               //  定数
        STATEMENT,              //  文
        STATEMENTS,             //  複数の文
        EXPRESS,                //  数式
        FUNCTION,               //  関数
        COMMENT,                //  コメント
        EMPTY,                  //  データなし
        ERROR,                  //  エラー
    }

    //  字句データ
    public class Token
    {
        public static string[] comment = { "//", "/*", "*/" };
        public static string[] condition = {
            "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!"
        };
        public static string[] assignment = {
            "=", "++", "--", "+=", "-=", "*=", "/=", "^=" };
        public static char[] operators = {
            '=', '+', '-', '*', '/', '%', '^', '!', '<', '>', '&', '|'
        };
        public static char[] delimiter = {
            '(', ')', '{', '}', '[', ']', ' ', ',', ';' };
        public static char[] skipChar = { ' ', '\t', '\r', '\n' };
        public static string[] statement = {
            "let", "while", "if", "else", "for", "return", "break", "continue",
            "print", "println", "exit", "pause", "#include",
        };
        public static string[] constatnt = { "PI", "E" };
        public char[] mBrackets = { '(', ')', '{', '}', '[', ']', '\"', '\"' };

        public string mValue;
        public TokenType mType;

        private KLexer mLexer = new KLexer();

        /// <summary>
        /// コンストラクタ(データの種別は自動判定)
        /// </summary>
        /// <param name="value">データ</param>
        public Token(string value)
        {
            mValue = value;
            mType = getTokenType(value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">データ</param>
        /// <param name="type">種別</param>
        public Token(string value, TokenType type)
        {
            mValue = value;     //  データの値
            mType = type;       //  データの種類
        }

        /// <summary>
        /// STRING データを取得するとき'"'の囲みを外す
        /// </summary>
        /// <returns>値</returns>
        public string getValue()
        {
            if (mType == TokenType.STRING) {
                if (0 <= mValue.IndexOf('"'))
                    return mLexer.stripBracketString(mValue, '"');
            }
            return mValue;
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns></returns>
        public Token copy()
        {
            return new Token(mValue, mType);
        }

        public override string ToString()
        {
            return $" {mValue} [{mType.ToString()}]";
        }

        /// <summary>
        /// 文字列からデータの種類を求める
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>種別</returns>
        public TokenType getTokenType(string str)
        {
            List<Token> tokens = mLexer.tokenList(str);
            if (1 < tokens.Count)
                return TokenType.EXPRESS;
            else if (1 == tokens.Count)
                return tokens[0].mType;
            else
                return TokenType.ERROR;
        }
    }


    /// <summary>
    /// 字句解析
    /// SKIP CHAR               :  ' ', '\t', '\r', '\n'
    /// コメント  (COMMENT)     : // ... \n, /* ...*/
    /// 計算式    (EXPRESS)     : ( 計算式 )
    /// 文リスト  (STAETMENTS)  : { 文; 文; ... }
    /// 文        (STATEMENT)   : let|while|if|else|return|print
    /// 変数名    (VARIABLE)    : [a..z|A..Z][a..z|A..Z|0..9]*
    /// 関数名    (FUNCTION)    : [a..z|A..Z][a..z|A..Z|0..9]* (...)
    /// 配列      (ARRAY)       : (VARIABLE)'['*']'
    /// 数値      (LITERAL)     : [0..9|.|+|-][0..9|.]
    /// 文字列    (STRING)      : "..."
    /// 定数      (CONSTAT)     : PI,E
    /// 演算子    (OPERATOR)    : +|-|*|/|%|^|!
    /// 代入演算子(ASSIGNMENT)  : =|+=|-=|*=|/=|^=|++|--
    /// 条件演算子(CONDITINAL)  : ==|!=|<|>|<=|>=
    /// 区切り文字(DELIMITER)   : (|)|{|}| |,|;
    /// 
    /// List<Token> tokenList(string str)               字句解析 文字列をトークンリストに変換
    /// 
    /// List<List<Token>> tokensList(List<Token> tokens, char sep = ',')    トークンをセパレータで区切ってまとめる
    /// string stripBracketString(string str, char bracket = '(')   括弧付き文字列で前後の括弧を除いた文字列
    /// getBracketString(string str, int n, char bracket = '(')     括弧で囲まれた文字列の抽出(括弧含む)
    /// List<string> getBracketStringList(string str, int n, char bracket = '(')    複数の括弧で囲まれた文字列の抽出(括弧含む)
    /// List<string> commaSplit(string str)             カンマで文字列を分割する("",(),{},[]内は無視)
    /// List<Token> splitArgList(string args)           引数文字列の分解(args[a,b] → args,[,a,,,b,])
    /// 
    /// </summary>
    public class KLexer
    {
        //  括弧リスト
        private char[] mBrackets = { '(', ')', '{', '}', '[', ']', '\"', '\"' };

        public KLexer() { }

        /// <summary>
        /// 字句解析 文字列をトークンリストに変換
        /// コメント(COMMENT),括弧(){}(EXPRESS/STATEMENT),変数名(STATEMENT/FUNCTION/VAROABLE)
        /// 数値(LITERAL),文字列(STRING),識別子(演算子(OPERATOR),条件演算子(CONDITINAL))
        /// 区切文字(DELIMITER)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>トークンリスト</returns>
        public List<Token> tokenList(string str)
        {
            List<Token> tokens = new List<Token>();
            for (int i = 0; i < str.Length; i++) {
                string buf = "";
                string twoChar = "";
                if (i + 1 < str.Length)
                    twoChar = str.Substring(i, 2);
                if (0 <= Array.IndexOf(Token.skipChar, str[i])) {
                    //  読み飛ばし
                } else if (twoChar != "" && 0 <= Array.IndexOf(Token.comment, twoChar)) {
                    //  コメント
                    if (twoChar == "//") {
                        while (i < str.Length && (str[i] != '\n' && str[i] != '\r')) buf += str[i++];
                    } else if (twoChar == "/*") {
                        while (i < str.Length - 1 && !(str[i] == '*' && str[i + 1] == '/')) buf += str[i++];
                        buf += str[i++];
                        buf += str[i++];
                    }
                    if (0 < buf.Length) {
                        tokens.Add(new Token(buf, TokenType.COMMENT));
                        i--;
                    }
                } else if (str[i] == '(' || str[i] == '{') {
                    //  括弧で囲まれた計算式
                    buf = getBracketString(str, i, str[i]);
                    tokens.Add(new Token(buf, str[i] == '{' ? TokenType.STATEMENTS : TokenType.EXPRESS));
                    i += buf.Length - 1;
                } else if (Char.IsLetter(str[i]) || str[i] == '#') { //  アルファベットの確認(a-z,A-Z,全角も)
                    //  変数名、予約語
                    while (i < str.Length && Array.IndexOf(Token.operators, str[i]) < 0
                             && Array.IndexOf(Token.delimiter, str[i]) < 0
                             && Array.IndexOf(Token.skipChar, str[i]) < 0) {
                        buf += str[i++];
                    }
                    while (i < str.Length && str[i] == ' ') i++;    //  空白削除
                    if (Token.statement.Contains(buf)) {
                        //  STATEMENT
                        tokens.Add(new Token(buf, TokenType.STATEMENT));
                    } else if (i < str.Length && str[i] == '[') {
                        //  ARRAY
                        string backet = getBracketString(str, i, str[i]);
                        buf += backet;
                        tokens.Add(new Token(buf, TokenType.ARRAY));
                        i += backet.Length;
                    } else if (i < str.Length && str[i] == '(') {
                        //  FUNCTION
                        tokens.Add(new Token(buf, TokenType.FUNCTION));
                    } else if (Token.constatnt.Contains( buf)) {
                        //  CONSTATNT
                        tokens.Add(new Token(buf, TokenType.CONSTANT));
                    } else {
                        //  VARIABLE
                        tokens.Add(new Token(buf, TokenType.VARIABLE));
                    }
                    i--;
                } else if (twoChar != "" && 0 <= Array.IndexOf(Token.assignment, twoChar)) {
                    //  複合演算子
                    tokens.Add(new Token(twoChar, TokenType.ASSIGNMENT));
                    i++;
                } else if (Char.IsNumber(str[i]) || str[i] == '.' ||
                    (i == 0 && (str[i] == '-' || str[i] == '+'))) {
                    //  数値
                    while (i < str.Length && (Char.IsNumber(str[i])
                        || str[i] == '.' || str[i] == '-' || str[i] == 'E' || str[i] == 'e')) {
                        if (Array.IndexOf(Token.skipChar, str[i]) < 0)
                            buf += str[i];
                        i++;
                    }
                    if (0 < buf.Length) {
                        tokens.Add(new Token(buf, TokenType.LITERAL));
                        i--;
                    }
                } else if (str[i] == '\"') {
                    //  文字列
                    buf = getBracketString(str, i, str[i]);
                    i += buf.Length - 1;
                    buf = buf.Replace("\\n", "\n");
                    tokens.Add(new Token(buf, TokenType.STRING));
                } else if (0 <= Array.IndexOf(Token.operators, str[i])) {
                    //  識別子(演算子/条件演算子)
                    if (twoChar != "" && 0 <= Array.IndexOf(Token.condition, twoChar)) {
                        tokens.Add(new Token(twoChar, TokenType.CONDITINAL));
                        i++;
                    } else if (0 <= Array.IndexOf(Token.condition, str[i].ToString())) {
                        tokens.Add(new Token(str[i].ToString(), TokenType.CONDITINAL));
                    } else if (0 <= Array.IndexOf(Token.assignment, str[i].ToString())) {
                        tokens.Add(new Token(str[i].ToString(), TokenType.ASSIGNMENT));
                    } else {
                        tokens.Add(new Token(str[i].ToString(), TokenType.OPERATOR));
                    }
                } else if (0 <= Array.IndexOf(Token.delimiter, str[i])) {
                    //  区切文字
                    tokens.Add(new Token(str[i].ToString(), TokenType.DELIMITER));
                } else {
                    tokens.Add(new Token(str[i].ToString(), TokenType.ERROR));
                }
            }
            return tokens;
        }

        /// <summary>
        /// トークンをセパレータで区切ってまとめる
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sep">セパレータ</param>
        /// <returns>トークンリストのリスト</returns>
        public List<List<Token>> tokensList(List<Token> tokens, char sep = ',')
        {
            List<List<Token>> tokensList = new List<List<Token>>();
            List<Token> buf = new List<Token>();
            foreach (Token token in tokens) {
                if (token.mType == TokenType.DELIMITER && token.mValue == sep.ToString()) {
                    tokensList.Add(buf);
                    buf = new List<Token>();
                } else {
                    buf.Add(token);
                }
            }
            if (0 < buf.Count)
                tokensList.Add(buf);
            return tokensList;
        }

        /// <summary>
        /// 括弧付き文字列で前後の括弧を除いた文字列
        /// </summary>
        /// <param name="str">括弧付き文字列</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>文字列</returns>
        public string stripBracketString(string str, char bracket = '(')
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            str = str.Trim();
            int sp = str.IndexOf(mBrackets[offset]);
            int ep = str.LastIndexOf(mBrackets[offset + 1]);
            if (0 == sp && ep < 0)
                return str.Substring(sp + 1);
            else if (0 == sp && ep == str.Length - 1)
                return str.Substring(sp + 1, ep - sp - 1);
            else if (sp < 0 && 0 <= ep)
                return str.Substring(0, ep - 1);
            return str;
        }

        /// <summary>
        /// 括弧で囲まれた文字列の抽出(括弧含む)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="n">開始位置</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>抽出文字列</returns>
        public string getBracketString(string str, int n, char bracket = '(')
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            int sp = str.IndexOf(mBrackets[offset], n);
            int pos = sp;
            if (pos < 0) return str;
            int count = 1;
            pos++;
            while (pos < str.Length && 0 < count) {
                if (bracket != '"' && str[pos] == '"') {
                    while (pos + 1 < str.Length && str[++pos] != '"') ;
                }
                if (str[pos] == '\\') pos++;
                else if (str[pos] == mBrackets[offset + 1]) count--;
                else if (str[pos] == mBrackets[offset]) count++;
                pos++;
            }
            return str.Substring(sp, pos - sp);
        }

        /// <summary>
        /// 複数の括弧で囲まれた文字列の抽出(括弧含む)
        /// "{..},{..},{...} ... "  →  "{...}", "{...}", "{...}"
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="n">開始位置</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>抽出文字列リスト</returns>
        public List<string> getBracketStringList(string str, int n, char bracket = '(')
        {
            List<string> strings = new List<string>();
            //  Bracketの選択
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return strings;
            //  Breacketの開始位置
            int sp = str.IndexOf(mBrackets[offset], n);
            if (sp < 0) return strings;
            int pos = sp;
            int count = 0;
            while (0 <= pos && pos < str.Length) {
                if (str[pos] == mBrackets[offset + 1]) count--;
                else if (str[pos] == mBrackets[offset]) count++;
                pos++;
                if (count == 0) {
                    strings.Add(str.Substring(sp, pos - sp));
                    sp = str.IndexOf(mBrackets[offset], pos);
                    pos = sp;
                }
            }
            return strings;
        }

        /// <summary>
        /// カンマで文字列を分割する("",(),{},[]内は無視)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>分割文字列リスト</returns>
        public List<string> commaSplit(string str)
        {
            List<string> strings = new List<string>();
            int n = 0;
            int sp = 0;
            while (n < str.Length) {
                int offset = Array.IndexOf(mBrackets, str[n]);
                if (0 <= offset) {
                    if (str[n++] == '"') {
                        while (n < str.Length && str[n] != '"') n++;
                    } else {
                        while (n < str.Length && str[n] != mBrackets[offset + 1]) {
                            if (str[n++] == '"') {
                                while (n < str.Length && str[n++] != '"') ;
                            }
                        }
                    }
                } else if (str[n] == ',') {
                    strings.Add(str.Substring(sp, n - sp));
                    sp = n + 1;
                }
                n++;
            }
            if (sp < n && n <= str.Length)
                strings.Add(str.Substring(sp, n - sp));
            if (0 < str.Length && str[str.Length - 1] == ',')
                strings.Add("");
            return strings;
        }

        /// <summary>
        /// 引数文字列の分解(args[a,b] → args,[,a,,,b,])
        /// </summary>
        /// <param name="args">引数文字列</param>
        /// <returns>文字列リスト</returns>
        public List<Token> splitArgList(string args)
        {
            List<Token> argList = new();
            int n = 0;
            string buf = "";
            while (n < args.Length) {
                if (args[n] == '[') {
                    if (0 < buf.Length) {
                        argList.Add(new Token(buf, TokenType.VARIABLE));
                        buf = "";
                    }
                    argList.Add(new Token(args[n].ToString(), TokenType.DELIMITER));
                } else if (args[n] == ']') {
                    if (0 < buf.Length) {
                        argList.Add(new Token(buf, TokenType.EXPRESS));
                        buf = "";
                    }
                    argList.Add(new Token(args[n].ToString(), TokenType.DELIMITER));
                } else if (args[n] == ',') {
                    if (0 < buf.Length) {
                        argList.Add(new Token(buf, TokenType.EXPRESS));
                        buf = "";
                    }
                    argList.Add(new Token(args[n].ToString(), TokenType.DELIMITER));
                } else if (args[n] == ' ' || args[n] == '\t') {
                    //  読み飛ばし
                } else {
                    buf += args[n];
                }
                n++;
            }
            if (0 < buf.Length)
                argList.Add(new Token(buf, TokenType.VARIABLE));
            return argList;
        }
    }
}
