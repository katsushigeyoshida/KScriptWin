using CoreLib;

namespace KScriptWin
{
    public class FuncString
    {
        public static string[] mFuncNames = new string[] {
            "string.concat(a, b); 文字列の接続",
            "string.length(a); 文字列の長さ",
            "string.substring(str,start,count); 文字列から部分取得",
            "string.join(sep,str[]); 指定文字列を挟んで連結",
            "string.split(str,sep); 文字列を分割",
            "string.contains(str,val); 文字列内に指定文字列を含むか判定",
            "string.indexOf(str,val,star,count); 文字列内から指定文字列を検索",
            "string.toString(form,v); 数値の書式設定",
            "string.format(form,v0,v1...); 数値の書式設定",
            "string.compare(strA,strB); 文字列を比較",
            "string.replace(str,old,new); 文字列の置換",
            "string.insert(str,start,val); 文字列の挿入",
            "string.remove(str,start,count); 文字列の削除",
            "string.padLeft(str,totalWidth[,paddingChar]); 左寄せ",
            "string.padRight(str,totalWidth[,paddingChar]); 右寄せ",
            "string.toUpper(str); 大文字化",
            "string.toLower(str); 小文字化",
            "string.trim(str[,trimChars]); 前後の空白削除",
        }; 
        //  共有クラス
        public KScript mScript;
        private KParse mParse;
        private Util mUtil = new Util();
        private Variable mVar;

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script"></param>
        public FuncString(KScript script)
        {
            mScript = script;
            mParse  = script.mParse;
            mVar    = script.mVar;
        }

        /// <summary>
        /// 追加内部関数
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>戻り値</returns>
        public Token function(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "string.concat": return concat(args);
                case "string.length": return length(args);
                case "string.substring": return substring(args);
                case "string.join": return join(args);
                case "string.split": return split(args, ret);
                case "string.contains": return contains(args);
                case "string.indexOf": return indexOf(args);
                case "string.toString": return toString(args);
                case "string.format": return format(args);
                case "string.compare": return compare(args);
                case "string.replace": return replace(args);
                case "string.insert": return insert(args);
                case "string.remove": return remove(args);
                case "string.padLeft": return padLeft(args);
                case "string.padRight": return padRight(args);
                case "string.toUpper": return toUpper(args);
                case "string.toLower": return toLower(args);
                case "string.trim": return trim(args);

                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = concat(str0,str1,str2...);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token concat(List<Token> args)
        {
            string str = "";
            for (int i = 0; i < args.Count; i++)
                str += args[i].getValue();
            return new Token(str, TokenType.STRING);
        }

        /// <summary>
        /// length = length(string);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token length(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                return new Token(str.Length.ToString(), TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = substring(string,strat[,cont]);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token substring(List<Token> args)
        {
            string str = "";
            int start = 0;
            int count = 0;
            if (1 < args.Count) {
                str = args[0].getValue();
                start = ylib.intParse(args[1].getValue());
                start = Math.Min(start, str.Length);
                if (2 < args.Count) {
                    count = ylib.intParse(args[2].getValue());
                    if (start + count >  str.Length)
                        count = str.Length - start;
                    str = str.Substring(start, count);
                } else {
                    str = str.Substring(start);
                }
                return new Token(str, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = join(separator,strings[]);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token join(List<Token> args)
        {
            if (1 < args.Count) {
                string sep = args[0].getValue();
                string str = "";
                for (int i = 1; i < args.Count; i++) {
                    List<string> texts =mVar.cnvListString(args[i]);
                    for (int j = 0; j < texts.Count(); j++)
                        str += texts[j] + sep;
                }
                str = str.Substring(0, str.Length - sep.Length);
                return new Token(str, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string[] = split(str,sep);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token split(List<Token> args, Token ret)
        {
            if (1 < args.Count) {
                string str = args[0].getValue();
                string sep = args[1].getValue();
                string[] splitArray = str.Split(sep);
                //  戻り値の設定
                mVar.setReturnArray(splitArray, ret, TokenType.STRING);
                mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
                return mVar.getVariable("return");
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// result = contains(str, val);
        /// </summary>
        /// <param name="args"></param>
        /// <returns>0:存在しない/1:存在する</returns>
        public Token contains(List<Token> args)
        {
            if (1 < args.Count) {
                string str = args[0].getValue();
                string val = args[1].getValue();
                if (str.Contains(val))
                    return new Token("1", TokenType.LITERAL);
                else
                    return new Token("0", TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// pos = string.indexOf(str,val[,start][,count]);
        /// </summary>
        /// <param name="args"></param>
        /// <returns>位置</returns>
        public Token indexOf(List<Token> args)
        {
            if (1 < args.Count) {
                string str = args[0].getValue();
                string val = args[1].getValue();
                int start = 0;
                int count = str.Length;
                if (2 < args.Count) {
                    start = ylib.intParse(args[2].getValue());
                    start = Math.Min(start, str.Length);
                } else if (args.Count == 4) {
                    count= ylib.intParse(args[1].getValue());
                }
                count -= start;
                return new Token(str.IndexOf(val, start, count).ToString(), TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 書式付き文字列変換(toString(val,form))
        /// form : C : 通過表記
        ///        D : 10進表記
        ///        E : 指数表記
        ///        F : 固定小数表記
        ///        G : 全般表記
        ///        N : 区切付き
        ///        P : パーセント
        ///        X : 16進表記
        ///     15:D : 15桁右詰め
        ///        X8: 16進8桁右詰
        ///        F4: 固定小数点小数点以下4桁
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token toString(List<Token> args)
        {
            if (1 < args.Count) {
                string form = args[0].getValue();
                string val = args[1].getValue();
                string sep = 0 <= form.IndexOf(':') ? "," : ":";
                if (0 <= form.IndexOfAny(new char[] { 'C', 'c', 'D', 'd', 'X', 'x' })) {
                    int i = ylib.intParse(val);
                    form = "{0" + sep + form + "}";
                    return new Token(string.Format(form, i), TokenType.STRING);
                } else {
                    double d = ylib.doubleParse(val);
                    form = "{0" + sep + form + "}";
                    return new Token(string.Format(form, d), TokenType.STRING);
                }
            } else if (0 < args.Count) {
                string val = args[0].getValue();
                return new Token(string.Format("{0}", val), TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 数値の書式設定(string = format(form,arg0,arg1...);)
        /// format("埋め込み {0},{1},{2}",s0,s1,s2);
        /// format("埋め込み {0},{1},{0}",s0,s1);
        /// format("数値 {0}",i0);
        /// format("10進 {0:D}",i0);
        /// format("全般 {0:G}",i0);
        /// format("桁区切り {0:N}",i0);
        /// format("パーセント {0:P}",i0);
        /// format("16進8桁右詰 {0:X8}",i0);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token format(List<Token> args)
        {
            if (0 < args.Count) {
                string form = args[0].getValue();
                string str = "";
                double a1, a2, a3, a4;
                if (args.Count == 2) {
                    a1 = ylib.doubleParse(args[1].getValue());
                    str = string.Format(form, a1);
                } else if (args.Count == 3) {
                    a1 = ylib.doubleParse(args[1].getValue());
                    a2 = ylib.doubleParse(args[2].getValue());
                    str = string.Format(form, a1, a2);
                } else if (args.Count == 4) {
                    a1 = ylib.doubleParse(args[1].getValue());
                    a2 = ylib.doubleParse(args[2].getValue());
                    a3 = ylib.doubleParse(args[3].getValue());
                    str = string.Format(form, a1, a2, a3);
                } else if (args.Count == 5) {
                    a1 = ylib.doubleParse(args[1].getValue());
                    a2 = ylib.doubleParse(args[2].getValue());
                    a3 = ylib.doubleParse(args[3].getValue());
                    a4 = ylib.doubleParse(args[4].getValue());
                    str = string.Format(form, a1, a2, a3, a4);
                }
                return new Token(str, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// val = compare(strA, strB);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token compare(List<Token> args)
        {
            if (1 < args.Count) {
                string strA = args[0].getValue();
                string strB = args[1].getValue();
                return new Token(string.Compare(strA, strB).ToString(), TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// str = replace(styr, src, dest);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token replace(List<Token> args)
        {
            if (2 < args.Count) {
                string str = args[0].getValue();
                string src = args[1].getValue();
                string dest = args[2].getValue();
                str = str.Replace(src, dest);
                return new Token(str, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = insert(str, start, val);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token insert(List<Token> args)
        {
            if (2 < args.Count) {
                string str = args[0].getValue();
                int start = ylib.intParse(args[1].getValue());
                string val = args[2].getValue();
                return new Token(str.Insert(start, val).ToString(), TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = remove(str, start, count);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token remove(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                int start = ylib.intParse(args[1].getValue());
                int count = ylib.intParse(args[2].getValue());
                str = str.Remove(start, count);
                return new Token(str, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = padLeft(str, totalWidth, paddingChar);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token padLeft(List<Token> args)
        {
            if (1 < args.Count) {
                string str = args[0].getValue();
                int totalWidth = ylib.intParse(args[1].getValue());
                if (args.Count == 2)
                    return new Token(str.PadLeft(totalWidth).ToString(), TokenType.STRING);
                if (2 < args.Count) {
                    char padingChar = args[2].getValue()[0];
                    return new Token(str.PadLeft(totalWidth, padingChar).ToString(), TokenType.STRING);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = padRight(str, totalWidth, paddingChar);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token padRight(List<Token> args)
        {
            if (1 < args.Count) {
                string str = args[0].getValue();
                int totalWidth = ylib.intParse(args[1].getValue());
                if (args.Count == 2)
                    return new Token(str.PadRight(totalWidth).ToString(), TokenType.STRING);
                if (2 < args.Count) {
                    char padingChar = args[2].getValue()[0];
                    return new Token(str.PadRight(totalWidth, padingChar).ToString(), TokenType.STRING);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = toUpper(str);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token toUpper(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                return new Token(str.ToUpper(), TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = toLower(str);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token toLower(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                return new Token(str.ToLower(), TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// string = trim(str[,trimChar]);
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Token trim(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                if (args.Count == 1)
                    return new Token(str.Trim(), TokenType.STRING);
                if (1 < args.Count) {
                    char[] trimChars = args[2].getValue().ToCharArray();
                    return new Token(str.Trim(trimChars), TokenType.STRING);
                }
            }
            return new Token("", TokenType.EMPTY);
        }
    }
}
