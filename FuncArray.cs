using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// 配列関数
    /// </summary>
    public class FuncArray
    {
        public static string[] mFuncNames = new string[] {
            "array.contains(c[2]); 配列の有無(0:なし 1:あり)",
            "array.count(a[]); 1次元配列のサイズ",
            "array.count(a[,]); 2次元配列のサイズ",
            "array.count(a[1,]); 2次元配列1列目のサイズ",
            "array.clear(a[]); 配列クリア",
            "array.remove(a[],start[,end]); 配列要素を範囲指定で削除",
            "array.squeeze(a[]); 配列の未使用データを削除圧縮",
            "array.sort(a[]); 配列のソート",
            "array.sort(a[,n]); 配列をn列でソート",
            "array.reverse(a[]); 配列の逆順",
            "array.reverse(a[,]); 配列の行を逆順",
            "array.reverse(a[,],1); 配列の列を逆順",
            "array.max(a[]); 配列の最大値",
            "array.min(a[,]); 配列の最小値",
            "array.sum(a[]);  配列の合計",
            "array.average(a[,]); 配列の平均",
            "array.variance(a[]); 配列の分散",
            "array.stdDeviation(a[]); 配列の標準偏差",
            "array.covariance(a[], b[]); 共分散",
            "array.corrCoeff(x[],y[]); 配列の相関係数",
        };

        //  共有クラス
        public KScript mScript;
        private KParse mParse;

        private YLib ylib = new YLib();

        public FuncArray(KScript script)
        {
            mScript = script;
            mParse = script.mParse;
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
                case "array.contains": return contains(arg);
                case "array.count": return getCount(args);
                case "array.clear": clear(args); break;
                case "array.remove": remove(args); break;
                case "array.squeeze": squeeze(args); break;
                case "array.sort": sort(args); break;
                case "array.reverse": reverse(args); break;
                case "array.max": return max(args);
                case "array.min": return min(args);
                case "array.sum": return sum(args);
                case "array.average": return average(args);
                case "array.variance": return variance(args);
                case "array.stdDeviation": return standardDeviation(args);
                case "array.covariance": return covariance(args);
                case "array.corrCoeff": return correlationCoefficient(args);
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }


        /// <summary>
        /// 変数の存在を確認(配列はインデックスも必要)(内部関数)
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>0:存在しない/1:存在する</returns>
        public Token contains(Token arg)
        {
            string str = ylib.stripBracketString(arg.mValue);
            if (mParse.containsVariable(str))
                return new Token("1", TokenType.LITERAL);
            return new Token("0", TokenType.LITERAL);
        }

        /// <summary>
        /// 配列のサイズの取得(内部関数)
        /// 引数 [],[,]:全データ数 [m,]:行ごとのデータ数 [,n]:列ごとのデータ数(行数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>サイズ</returns>
        public Token getCount(List<Token> args)
        {
            int count = 0;
            string arrayName = "";
            int n = args[0].mValue.IndexOf("[");
            if (n < 0)
                return new Token(mParse.countVariable(args[0].mValue).ToString(), TokenType.LITERAL);
            //  全配列のデータ数
            if (0 < args[0].mValue.IndexOf("[]") || 0 < args[0].mValue.IndexOf("[,]")) {
                arrayName = args[0].mValue.Substring(0, n + 1);
                count = mParse.countVariable(arrayName);
                return new Token(count.ToString(), TokenType.LITERAL);
            }
            //  2次元配列の行ごとのデータ数(列数)
            int n2 = args[0].mValue.IndexOf(",]");
            if (0 < n2) {
                arrayName = args[0].mValue.Substring(0, n2 + 1);
                count = mParse.countVariable(arrayName);
                return new Token(count.ToString(), TokenType.LITERAL);
            }
            //  2次元配列の列ごとのデータ数(行数)
            int n3 = args[0].mValue.IndexOf(",");
            if (0 < n3) {
                arrayName = args[0].mValue.Substring(0, n + 1);
                string last = args[0].mValue.Substring(n3);
                count = mParse.countVariable(arrayName, last);
                return new Token(count.ToString(), TokenType.LITERAL);
            }
            //  その他
            count = mParse.countVariable(args[0].mValue);
            return new Token(count.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        public void clear(List<Token> args)
        {
            string arrayName = args[0].mValue.Substring(0, args[0].mValue.IndexOf("[") + 1);
            mParse.clearArray(arrayName);
        }

        /// <summary>
        /// 配列から範囲指定で要素を削除する(remove(a[],st[,ed]);)
        /// </summary>
        /// <param name="args">配列名と要素番号</param>
        public void remove(List<Token> args)
        {
            if (args.Count < 2) return;
            (string arrayName, int no) = mParse.getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            int st = ylib.intParse(args[1].mValue);
            int ed = args.Count > 2 ? ylib.intParse(args[2].mValue) : st;
            for (int i = st; i <= ed; i++) {
                string key = $"{arrayName}[{i}]";
                mParse.removeVariable(key);
            }
            squeeze(args);
        }

        /// <summary>
        /// 配列の圧縮(未使用インデックス削除)(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        public void squeeze(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            if (no != 1)
                return;
            List<Token> listToken = new();
            int maxcol = mParse.getMaxArray(arrayName);
            for (int i = 0; i < maxcol + 1; i++) {
                string key = $"{arrayName}[{i}]";
                if (mParse.containsVariable(key)) {
                    listToken.Add(mParse.getVariable(key));
                    mParse.removeVariable(key);
                }
            }
            for (int i = 0; i < listToken.Count; i++) {
                string key = $"{arrayName}[{i}]";
                mParse.setVariable(key, listToken[i]);
            }
        }

        /// <summary>
        /// ソート(inner function)
        /// 2D配列の時 array[m,n] n : ソート列位置
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名</param>
        public void sort(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            if (no == 1) {
                if (mParse.isStringArray(args[0])) {
                    //  文字列のソート
                    string[]? strArray = mParse.cnvListString(args[0]).ToArray();
                    Array.Sort(strArray);
                    clear(args);
                    mParse.setReturnArray(strArray, args[0]);
                } else {
                    //  実数のソート
                    double[]? doubleArray = mParse.cnvListDouble(args[0]).ToArray();
                    Array.Sort(doubleArray);
                    clear(args);
                    mParse.setReturnArray(doubleArray, args[0]);
                }
            } else if (no == 2) {
                (string name, string row, string col) = mParse.getArgArray2(args[0].mValue);
                string outname = name + "[,]";
                int n = col == "" ? 0 : ylib.intParse(col);
                if (mParse.isStringArray(args[0])) {
                    string[,] stringArray = mParse.cnvArrayString2(args[0]);
                    stringArray = ylib.stringArray2Sort(stringArray, n);
                    clear(args);
                    mParse.setReturnArray(stringArray, new Token(outname));
                } else {
                    double[,] doubleArray = mParse.cnvArrayDouble2(args[0]);
                    doubleArray = ylib.doubleArray2Sort(doubleArray, n);
                    clear(args);
                    mParse.setReturnArray(doubleArray, new Token(outname));
                }
            }
        }

        /// <summary>
        /// 配列を逆順にする(inner function)
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名</param>
        public void reverse(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            bool colRevers = false;
            if (1 < args.Count)
                colRevers = args[1].mValue == "1";
            if (no == 1) {
                int maxcol = mParse.getMaxArray(arrayName);
                if (maxcol <= 0) return;
                Token[] tokens = new Token[maxcol + 1];
                arrayName += "[";
                foreach (var variable in mParse.getVariables(arrayName)) {
                    if (0 <= variable.Key.IndexOf(arrayName)) {
                        (string name, int col) = mParse.getArrayNo(variable.Key);
                        tokens[maxcol - col] = variable.Value;
                    }
                }
                clear(args);
                mParse.setReturnArray(tokens, args[0]);
            } else if (no == 2) {
                Token[,] arrayData = mParse.cnvArrayToken2(args[0]);
                Token[,] tokens = new Token[arrayData.GetLength(0), arrayData.GetLength(1)];
                if (colRevers) {
                    for (int i = 0; i < arrayData.GetLength(0); i++) {
                        for (int j = 0; j < arrayData.GetLength(1); j++) {
                            tokens[i, tokens.GetLength(1) - j - 1] = arrayData[i, j].copy();
                        }
                    }
                } else {
                    for (int i = 0; i < arrayData.GetLength(0); i++) {
                        for (int j = 0; j < arrayData.GetLength(1); j++) {
                            tokens[tokens.GetLength(0) - i - 1, j] = arrayData[i, j].copy();
                        }
                    }
                }
                clear(args);
                mParse.setReturnArray(tokens, args[0]);
            }
        }

        /// <summary>
        /// 最大値を求める(a[] / a[,] / a[x,])(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最大値</returns>
        public Token max(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            double max = double.MinValue;
            if (no == 1 || no == 2) {
                arrayName = mParse.getSearchName(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mParse.getVariables(arrayName)) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING) {
                        double x = ylib.doubleParse(variable.Value.mValue);
                        if (max < x)
                            max = x;
                    }
                }
            }
            return new Token(max.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 最小値を求める(a[] / a[,] / a[x,])(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最小値</returns>
        public Token min(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            double min = double.MaxValue;
            if (no == 1 || no == 2) {
                arrayName = mParse.getSearchName(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mParse.getVariables(arrayName)) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING) {
                        double x = ylib.doubleParse(variable.Value.mValue);
                        if (min > x)
                            min = x;
                    }
                }
            }
            return new Token(min.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列の合計(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>合計</returns>
        public Token sum(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = mParse.cnvListDouble(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double sum = listData.Sum();
            return new Token(sum.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 平均値を求める(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>平均値</returns>
        public Token average(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = mParse.cnvListDouble(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double ave = listData.Sum() / listData.Count;
            return new Token(ave.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 分散(inner function)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>分散値</returns>
        public Token variance(List<Token> args)
        {
            (string arrayName, int no) = mParse.getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = mParse.cnvListDouble(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double ave = listData.Sum() / listData.Count;
            double vari = listData.Sum(p => (p - ave) * (p - ave)) / listData.Count;
            return new Token(vari.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 標準偏差(inner function)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>標準偏差</returns>
        public Token standardDeviation(List<Token> args)
        {
            Token token = variance(args);
            if (token.mType != TokenType.ERROR)
                return new Token(Math.Sqrt(ylib.doubleParse(token.mValue)).ToString(), TokenType.LITERAL);
            else
                return token;
        }

        /// <summary>
        /// 共分散(a[],b[])(inner function)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>共分散</returns>
        public Token covariance(List<Token> args)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> listData0 = mParse.cnvListDouble(args[0]);
            List<double> listData1 = mParse.cnvListDouble(args[1]);
            if (listData0.Count != listData1.Count)
                return new Token("", TokenType.ERROR);
            double ave0 = listData0.Average();
            double ave1 = listData1.Average();
            double total = 0;
            for (int i = 0; i < listData0.Count; i++) {
                total += (listData0[i] - ave0) * (listData1[i] - ave1);
            }
            return new Token((total / listData0.Count).ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 相関係数(x[],y[])(inner function)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>相関係数</returns>
        public Token correlationCoefficient(List<Token> args)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> x = mParse.cnvListDouble(args[0]);
            List<double> y = mParse.cnvListDouble(args[1]);
            if (x.Count != y.Count)
                return new Token("", TokenType.ERROR);
            double avex = x.Average();
            double avey = y.Average();
            double cov = 0;
            for (int i = 0; i < x.Count; i++) {
                cov += (x[i] - avex) * (y[i] - avey);
            }
            cov /= x.Count;
            double stdx = Math.Sqrt(x.Sum(p => (p - avex) * (p - avex)) / x.Count);
            double stdy = Math.Sqrt(y.Sum(p => (p - avey) * (p - avey)) / y.Count);
            return new Token((cov / (stdx * stdy)).ToString(), TokenType.LITERAL);
        }
    }
}
