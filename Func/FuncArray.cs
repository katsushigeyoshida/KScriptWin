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
            "array.copy(a[],start,end); 配列からデータを抽出する(b[]=array.copy(a[],start,end))",
            "array.concat(a[],b[]); 配列同士の結合c[]=array.add(a[],b[])",
            "array.concat(a[,],b[,]); 配列同士の結合c[,]=array.add(a[,],b[,])",
            "array.append(a[],v); 配列に値を追加",
            "array.add(a[],val); 配列に値を足す",
            "array.sub(a[],val); 配列に値を引く",
            "array.multi(a[],val); 配列に値を掛ける",
            "array.divide(a[],val); 配列を値で割る",
            "array.arrange(start,end,step); 等間隔のの数値配列作成",
            "array.linspace(start,end,div); 等分割した数値配列の作成",
            "array.create(size,value/express); sizeで指定した配列作成(a[]=array.creat(size, value/express))",
            "array.create(size1,size2,value/express); sizeで指定した配列作成(a[,]=array.creat(size1, size2, value/express))",
            "array.create(size1,size2,size3,value/express); sizeで指定した配列作成(a[,,]=array.creat(size1,size2,size3, value/express))",
            "array.calc(a[],express); 配列のデータを数式で処理する express = \"[x]*[x]+2\"",
        };

        //  共有クラス
        public KScript mScript;
        private KParse mParse;
        private Util mUtil = new Util();
        private Variable mVar;

        private YLib ylib = new YLib();

        public FuncArray(KScript script)
        {
            mScript = script;
            mParse = script.mParse;
            mVar = script.mVar;
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
                case "array.copy": return copy(args, ret);
                case "array.concat": return concat(args, ret);
                case "array.append": append(args); break;
                case "array.add": add(args); break;
                case "array.sub": sub(args); break;
                case "array.multi": multi(args); break;
                case "array.divide": divide(args); break;
                case "array.arrange": return arrange(args, ret);
                case "array.linspace": return linspace(args, ret);
                case "array.create": return create(args, ret);
                case "array.calc": calcArray(args, ret); break;
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
            if (mVar.containsVariable(str))
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
            if (args.Count == 0)
                return new Token("", TokenType.EMPTY);
            List<string> arrayNameList = mVar.getArrayNameList(args[0]);
            return new Token(arrayNameList.Count.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        public void clear(List<Token> args)
        {
            string arrayName = args[0].mValue.Substring(0, args[0].mValue.IndexOf("[") + 1);
            mVar.clearArray(arrayName);
        }

        /// <summary>
        /// 配列から範囲指定で要素を削除する
        /// remove(a[] / a[,] / a[0,] / a[,2] / a[1,2]...)
        /// </summary>
        /// <param name="args">配列名と要素番号</param>
        public void remove(List<Token> args)
        {
            if (args.Count == 0)
                return ;
            List<string> arrayNameList = mVar.getArrayNameList(args[0]);
            foreach (var arrayName in arrayNameList)
                mVar.removeVariable(arrayName);
            //squeeze(args);
        }

        /// <summary>
        /// 配列の圧縮(未使用インデックス削除)
        /// </summary>
        /// <param name="args">配列名</param>
        public void squeeze(List<Token> args)
        {
            (string arrayName, int no) = mUtil.getArrayName(args[0]);
            if (no != 1)
                return;
            List<Token> listToken = new();
            int maxcol = mVar.getMaxArray(arrayName);
            for (int i = 0; i < maxcol + 1; i++) {
                string key = $"{arrayName}[{i}]";
                if (mVar.containsVariable(key)) {
                    listToken.Add(mVar.getVariable(key));
                    mVar.removeVariable(key);
                }
            }
            for (int i = 0; i < listToken.Count; i++) {
                string key = $"{arrayName}[{i}]";
                mVar.setVariable(new Token(key), listToken[i]);
            }
        }

        /// <summary>
        /// ソート
        /// 2D配列の時 array[m,n] n : ソート列位置
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名</param>
        public void sort(List<Token> args)
        {
            (string arrayName, int no) = mUtil.getArrayName(args[0]);
            if (no == 1) {
                //  1次元配列
                if (mVar.isStringArray(args[0])) {
                    //  文字列のソート
                    string[]? strArray = mVar.cnvListString(args[0]).ToArray();
                    Array.Sort(strArray);
                    clear(args);
                    mVar.setReturnArray(strArray, args[0]);
                } else {
                    //  実数のソート
                    double[]? doubleArray = mVar.cnvListDouble(args[0]).ToArray();
                    Array.Sort(doubleArray);
                    clear(args);
                    mVar.setReturnArray(doubleArray, args[0]);
                }
            } else if (no == 2) {
                //  2次元配列
                (string name, string row, string col) = mUtil.getArgArray2(args[0].mValue);
                string outname = name + "[,]";
                int n = col == "" ? 0 : ylib.intParse(col);
                if (mVar.isStringArray(args[0])) {
                    string[,] stringArray = mVar.cnvArrayString2(args[0]);
                    stringArray = ylib.stringArray2Sort(stringArray, n);
                    clear(args);
                    mVar.setReturnArray(stringArray, new Token(outname));
                } else {
                    double[,] doubleArray = mVar.cnvArrayDouble2(args[0]);
                    doubleArray = ylib.doubleArray2Sort(doubleArray, n);
                    clear(args);
                    mVar.setReturnArray(doubleArray, new Token(outname));
                }
            }
        }

        /// <summary>
        /// 配列を逆順にする
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名[,colReverse]</param>
        public void reverse(List<Token> args)
        {
            (string arrayName, int no) = mUtil.getArrayName(args[0]);
            bool colRevers = false;
            if (1 < args.Count)
                colRevers = args[1].mValue == "1";
            if (no == 1) {
                //  1次元配列
                int maxcol = mVar.getMaxArray(arrayName);
                if (maxcol <= 0) return;
                Token[] tokens = new Token[maxcol + 1];
                arrayName += "[";
                foreach (var variable in mVar.getVariableList(arrayName)) {
                    if (0 <= variable.Key.IndexOf(arrayName)) {
                        (string name, int col) = mUtil.getArrayNo(variable.Key);
                        tokens[maxcol - col] = variable.Value;
                    }
                }
                clear(args);
                mVar.setReturnArray(tokens, args[0]);
            } else if (no == 2) {
                //  2次元配列
                Token[,] arrayData = mVar.cnvArrayToken2(args[0]);
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
                mVar.setReturnArray(tokens, args[0]);
            }
        }


        /// <summary>
        /// 配列のコピーを作成  a[] = array.copy(b[], start, end);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token copy(List<Token> args, Token ret)
        {
            List<double> src = new List<double>();
            List<double> dest = new List<double>();
            int start = 0, end = 0;
            if (0 < args.Count && mVar.getArrayOder(args[0]) == 1) {
                src = mVar.getDoubleArrayList(args[0]);
                end = src.Count;
                if (1 < args.Count && mVar.getArrayOder(args[1]) == 0)
                    start = (int)mVar.getDoubleFromArg(args[1]);
                if (2 < args.Count && mVar.getArrayOder(args[2]) == 0)
                    end = Math.Min((int)mVar.getDoubleFromArg(args[2]), end);
                dest = src.Skip(start).Take(end - start).ToList();
                mVar.setReturnArray(dest.ToArray(), ret);
            } else
                return new Token("", TokenType.EMPTY);

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 配列と配列を結合する
        /// c[] = array.concat(a[],b[]);
        /// c[,] = array.concat(a[,],b[,]);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token concat(List<Token> args, Token ret)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            (string arrayA, int noA) = mUtil.getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            (string arrayB, int noB) = mUtil.getArrayName(new Token(args[1].mValue, TokenType.VARIABLE));
            if (noA == 1 && noB == 1) {
                //  1次元配列同士
                List<double> a = mVar.cnvListDouble(args[0]);
                List<double> b = mVar.cnvListDouble(args[1]);
                a.AddRange(b);
                mVar.setReturnArray(a.ToArray(), ret);
            } else if (noA ==2 && noB == 2) {
                //  2次元配列同士
                double[,] a = mVar.cnvArrayDouble2(args[0]);
                double[,] b = mVar.cnvArrayDouble2(args[1]);
                double[,] c = new double[a.GetLength(0) + b.GetLength(0), a.GetLength(1)];
                for (int i = 0; i < a.GetLength(0); i++) {
                    for (int j = 0; j < a.GetLength(1); j++)
                        c[i, j] = a[i, j];
                }
                for (int i = 0; i < b.GetLength(0); i++) {
                    for (int j = 0; j < Math.Min(a.GetLength(1),b.GetLength(1)); j++)
                        c[i + a.GetLength(0), j] = b[i, j];
                }
                mVar.setReturnArray(c, ret);
            } else
                return new Token("", TokenType.EMPTY);

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 配列に値を追加 (array.append(a[], 5) , array.append(a[1,], 5))
        /// </summary>
        /// <param name="args"></param>
        private void append(List<Token> args)
        {
            double v = 0;
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0) {
                //  数値
                v = mVar.getDoubleFromArg(args[1]);
            } else
                return ;
            if (0 < args.Count && mVar.getArrayOder(args[0]) != 0) {
                //  1次元配列
                mVar.appendArray(args[0], v);
            }
        }

        /// <summary>
        /// 配列に配列を追加 array.extend(a[], b[]);
        /// </summary>
        /// <param name="args"></param>
        private void extend(List<Token> args)
        {

        }

        /// <summary>
        /// 配列のn番目に値を挿入 array.insert(a[], n, v);
        /// </summary>
        /// <param name="args"></param>
        private void insert(List<Token> args)
        {

        }

        /// <summary>
        /// 等間隔のの数値配列作成
        /// a[] = array.arrange(start, end, step);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private Token arrange(List<Token> args, Token ret)
        {
            List<double> arrayList = new();
            double start =0, end = 1, step = 1;
            if (args.Count < 2) {
                return new Token("", TokenType.EMPTY);
            }
            //  引数の取得
            if (mVar.getArrayOder(args[0]) == 0)
                start = mVar.getDoubleFromArg(args[0]);
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0)
                end = mVar.getDoubleFromArg(args[1]);
            if (1 < args.Count && mVar.getArrayOder(args[2]) == 0)
                step = mVar.getDoubleFromArg(args[2]);
            //  配列の方向を確認
            double v = start;
            if (start < end && 0 < step) {
                //  配列の作成(増加方向)
                while (v <= end) {
                    arrayList.Add(v);
                    v += step;
                }
            } else if (start > end && step < 0) {
                //  配列の作成(減少方向)
                while (v >= end) {
                    arrayList.Add(v);
                    v += step;
                }
            } else {
                return new Token("", TokenType.EMPTY);
            }
            mVar.setReturnArray(arrayList.ToArray(), ret);

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 等分割した数値配列の作成
        /// a[]  = array.linspace(start, end, div);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private Token linspace(List<Token> args, Token ret)
        {
            List<double> arrayList = new();
            double start = 0, end = 1, step = 1;
            if (args.Count < 2) {
                return new Token("", TokenType.EMPTY);
            }
            //  引数の取得
            if (mVar.getArrayOder(args[0]) == 0)
                start = mVar.getDoubleFromArg(args[0]);
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0)
                end = mVar.getDoubleFromArg(args[1]);
            if (1 < args.Count && mVar.getArrayOder(args[2]) == 0) {
                double div = mVar.getDoubleFromArg(args[2]);
                step = Math.Abs(start - end) / div;
            }
            //  配列の方向を確認
            if (start < end && 0 < step) {
            } else if (start > end && 0 < step) {
                double t = start;
                start = end;
                end = t;
            } else {
                return new Token("", TokenType.EMPTY);
            }
            //  配列の作成
            double v = start;
            while (v <= end) {
                arrayList.Add(v);
                v += step;
            }
            mVar.setReturnArray(arrayList.ToArray(), ret);

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// a[]  = array.creat(size, value);            sizeで指定した1次元配列
        /// a[,] = array.creat(size0, size1, value);    sizeで指定した1次元配列
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private Token create(List<Token> args, Token ret)
        {
            YCalc calc = new YCalc();
            List<string> exceptVar = new List<string>() { "x" };
            int order = mVar.getArrayOder(ret);
            string express = "";
            double initValue = 0;
            bool cb = false;
            if (0 < order && args.Count == order + 1) {
                if (args[order].mType == TokenType.STRING) {
                    express = mScript.cnvExpress(args[order].getValue());
                    cb = true;
                } else
                    initValue = mVar.getDoubleFromArg(args[order]);
                if (order == 1) {
                    //  1次元配列の作成
                    int size = (int)mVar.getDoubleFromArg(args[0]);
                    double[] a = new double[size];
                    for (int i = 0; i < size; i++) {
                        if (cb) {
                            string exp = express.Replace("[x]", i.ToString());
                            a[i] = calc.expression(exp);
                        } else
                            a[i] = initValue;
                    }
                    mVar.setReturnArray(a, ret);
                } else if (order == 2) {
                    //  2次元配列の作成
                    int size0 = (int)mVar.getDoubleFromArg(args[0]);
                    int size1 = (int)mVar.getDoubleFromArg(args[1]);
                    double[,] a = new double[size0, size1];
                    for (int i = 0; i < size0; i++)
                        for (int j = 0; j < size1; j++) {
                            if (cb) {
                                string exp = express.Replace("[x]", i.ToString());
                                exp = exp.Replace("[y]", j.ToString());
                                a[i,j] = calc.expression(exp);
                            } else
                                a[i, j] = initValue;
                        }
                    mVar.setReturnArray(a, ret);
                } else if (order == 3) {
                    //  3次元配列の作成
                    int size0 = (int)mVar.getDoubleFromArg(args[0]);
                    int size1 = (int)mVar.getDoubleFromArg(args[1]);
                    int size2 = (int)mVar.getDoubleFromArg(args[2]);
                    double[,,] a = new double[size0, size1, size2];
                    for (int i = 0; i < size0; i++)
                        for (int j = 0; j < size1; j++)
                            for (int k = 0; k < size2; k++) {
                                if (cb) {
                                    string exp = express.Replace("[x]", i.ToString());
                                    exp = exp.Replace("[y]", j.ToString());
                                    exp = exp.Replace("[z]", k.ToString());
                                    a[i, j, k] = calc.expression(exp);
                                } else
                                    a[i, j, k] = initValue;
                            }
                    mVar.setReturnArray(a, ret);
                } else {
                    return new Token("", TokenType.EMPTY);
                }
            }
            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// x[]配列から計算式を使ってy[]配列を作成 (calc(x[],"express")
        /// express = "[x]*2+n";など
        /// </summary>
        /// <param name="args">引数</param>
        /// <param name="ret">返数名</param>
        private void calcArray(List<Token> args, Token ret)
        {
            YCalc calc = new YCalc();
            string express = mScript.cnvExpress(args[1].getValue());
            List<string> arrayNameList = mVar.getArrayNameList(args[0]);    //  数式処理する配列リスト
            foreach(var x in arrayNameList) {
                //  数式に配列の値を代入して計算
                string exp = express.Replace("[x]", mVar.getVariable(x).mValue);
                double result = calc.expression(exp);
                mVar.setVariable(new Token(x), new Token(result.ToString()));
            }
        }


        /// <summary>
        /// 配列に値を足すarray.add(a[], val) / array.add(a[1,], val)
        /// </summary>
        /// <param name="args"></param>
        private void add(List<Token> args)
        {
            double v = 0;
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0) {
                //  数値
                v = mVar.getDoubleFromArg(args[1]);
            } else
                return;
            if (0 < args.Count && 0 < mVar.getArrayOder(args[0])) {
                //  配列
                List<string> arrayNameList = mVar.getArrayNameList(args[0]);
                mVar.addArrayValue(arrayNameList, v);
            }
        }

        /// <summary>
        /// 配列に値を引く array.sub(a[], val);
        /// </summary>
        /// <param name="args"></param>
        private void sub(List<Token> args)
        {
            double v = 0;
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0) {
                //  数値
                v = mVar.getDoubleFromArg(args[1]);
            } else
                return;
            if (0 < args.Count && 0 < mVar.getArrayOder(args[0])) {
                //  配列
                List<string> arrayNameList = mVar.getArrayNameList(args[0]);
                mVar.addArrayValue(arrayNameList, -v);
            }
        }

        /// <summary>
        /// 配列に値を掛ける array.multi(a[], val);
        /// </summary>
        /// <param name="args"></param>
        private void multi(List<Token> args)
        {
            double v = 0;
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0) {
                //  数値
                v = mVar.getDoubleFromArg(args[1]);
            } else
                return;
            if (0 < args.Count && 0 < mVar.getArrayOder(args[0])) {
                //  配列
                List<string> arrayNameList = mVar.getArrayNameList(args[0]);
                mVar.multiArrayValue(arrayNameList, v);
            }
        }

        /// <summary>
        /// 配列に値で割る array.divide(a[], val);
        /// </summary>
        /// <param name="args"></param>
        private void divide(List<Token> args)
        {
            double v = 0;
            if (1 < args.Count && mVar.getArrayOder(args[1]) == 0) {
                //  数値
                v = mVar.getDoubleFromArg(args[1]);
            } else
                return;
            if (0 < args.Count && 0 < mVar.getArrayOder(args[0])) {
                //  配列
                List<string> arrayNameList = mVar.getArrayNameList(args[0]);
                mVar.multiArrayValue(arrayNameList, 1 / v);
            }
        }

        /// <summary>
        /// c[] = array.addArray(a[], b[]);                  配列同士の演算
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void addArray(List<Token> args, Token ret)
        {

        }

        /// <summary>
        /// c[] = array.subArray(a[], b[]);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void subArray(List<Token> args, Token ret)
        {

        }

        /// <summary>
        /// c[] = array.multiArray(a[], b[]);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void multiArray(List<Token> args, Token ret)
        {

        }

        /// <summary>
        /// c[] = array.divideArray(a[], b[]);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void divideArray(List<Token> args, Token ret)
        {

        }

        /// <summary>
        /// n = array.indexOf(a[], val, start);         valを検索してindexを返す
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void indexOf(List<Token> args, Token ret)
        {

        }

        /// <summary>
        /// n = array.lastIndexOf(a[], val, lastStart); valを最後から検索してindexを返す
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        private void lastIndexOf(List<Token> args, Token ret)
        {

        }
    }
}
