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
            "array.count(a[]); 配列のサイズ(arg=a[]/b[,]/b[n,])/b[,m]...)",
            "array.maxIndex(a[]); 配列の最大インデックス値を求める",
            "array.minIndex(a[]); 配列の最小インデックス値を求める",
            "array.remove(a[]); 配列要素を範囲指定で削除(arg=a[n]/a[]/b[,]/b[n,]/b[,m]...)",
            "array.insert(a[],n,b[]); 配列要素の挿入(挿入元,挿入位置,挿入データ)(arg=a[]/a[,],n,b[])",
            "array.squeeze(a[]); 配列の未使用データを削除圧縮",
            "array.sort(a[]); 配列のソート(arg=a[]/b[,]/b[n,])/b[,m]...)",
            "array.reverse(a[]); 配列の逆順(arg=a[]/b[,]/b[n,])/b[,m]...)",
            "array.copy(a[],start,end); 配列からデータを抽出する(b[]=array.copy(a[],start,end))",
            "array.concat(a[],b[]); 配列同士の結合c[]=array.add(a[],b[])",
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
                case "array.count"   : return getCount(args);
                case "array.maxIndex": return maxIndex(args);
                case "array.minIndex": return minIndex(args);
                case "array.clear"   : clear(args); break;
                case "array.remove"  : remove(arg); break;
                case "array.insert"  : insert(args); break;
                case "array.squeeze" : squeeze(args); break;
                case "array.sort"    : sort(args); break;
                case "array.reverse" : reverse(args); break;
                case "array.copy"    : return copy(args, ret);
                case "array.concat"  : return concat(args, ret);
                case "array.append"  : append(args); break;
                case "array.add"     : add(args); break;
                case "array.sub"     : sub(args); break;
                case "array.multi"   : multi(args); break;
                case "array.divide"  : divide(args); break;
                case "array.arrange" : return arrange(args, ret);
                case "array.linspace": return linspace(args, ret);
                case "array.create"  : return create(args, ret);
                case "array.calc"    : calcArray(args, ret); break;
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
        /// 配列のインデックスの最大値を求める
        /// 2次元以上の場合は上位インデックスから最初に見つかった空白位置のインデックス値の最大値
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最大インデックス</returns>
        public Token maxIndex(List<Token> args)
        {
            if (args.Count == 0)
                return new Token("", TokenType.EMPTY);
            int maxInd = mVar.maxIndex(args[0]);
            return new Token(maxInd.ToString(), TokenType.LITERAL);
        }


        /// 配列のインデックスの最小値を求める
        /// 2次元以上の場合は上位インデックスから最初に見つかった空白位置のインデックス値の最小値
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最小インデックス</returns>
        public Token minIndex(List<Token> args)
        {
            if (args.Count == 0)
                return new Token("", TokenType.EMPTY);
            int maxInd = mVar.minIndex(args[0]);
            return new Token(maxInd.ToString(), TokenType.LITERAL);
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
        public void remove(Token arg)
        {
            List<Token> args = mScript.getFuncArgNames(arg.mValue);
            for (int i = 0; i < args.Count; i++)
                if (args[i].mType == TokenType.ARRAY) {
                    var v = mScript.getVariableName(args[i]);
                    mVar.remove(v);
                }
            //mVar.squeeze(args[0]);
        }

        /// <summary>
        /// 配列の圧縮(空き配列を詰める)
        /// </summary>
        /// <param name="args">配列名</param>
        public void squeeze(List<Token> args)
        {
            if (args.Count == 0)
                return;
            mVar.squeeze(args[0]);
        }

        /// <summary>
        /// ソート
        /// 2D配列の時 array[m,n] n : ソート列位置
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名</param>
        public void sort(List<Token> args)
        {
            mVar.sort(args[0]);
        }

        /// <summary>
        /// 配列を逆順にする
        /// 配列のインデックスが0以上の数値のみに対応
        /// </summary>
        /// <param name="args">配列名[,colReverse]</param>
        public void reverse(List<Token> args)
        {
            mVar.reverse(args[0]);
        }


        /// <summary>
        /// 配列のコピーを作成  a[] = array.copy(b[], start, end);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token copy(List<Token> args, Token ret)
        {
            List<Token> src = new List<Token>();
            List<Token> dest = new List<Token>();
            int start = 0, end = 0;
            if (0 < args.Count && mVar.getArrayOder(args[0]) == 1) {
                src = mVar.getTokenArrayList(args[0]);
                end = src.Count- 1;
                if (1 < args.Count && mVar.getArrayOder(args[1]) == 0)
                    start = (int)mVar.getDoubleFromArg(args[1]);
                if (2 < args.Count && mVar.getArrayOder(args[2]) == 0)
                    end = Math.Min((int)mVar.getDoubleFromArg(args[2]), end);
                dest = src.Skip(start).Take(end - start + 1).ToList();
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
            if (args.Count < 2 || ret == null)
                return new Token("", TokenType.ERROR);

            List<List<ArrayName>> arrayList = new List<List<ArrayName>>();
            int maxIndexSize = 0;
            for (int i = 0; i < args.Count; i++) {
                arrayList.Add(mVar.getArrayList(args[i]));
                int indexSize = arrayList[i].Max(x => x.mIndexs.Count);
                if (maxIndexSize < indexSize)
                    maxIndexSize = indexSize;
            }
            List<ArrayName> cList = new List<ArrayName>();
            int indexCount = -1;
            string preIndexCount = "";
            string dest = ret.getValue();
            string destName = dest.Substring(0, dest.IndexOf("["));
            foreach (List<ArrayName> aList in arrayList) {
                foreach (ArrayName arrayName in aList) {
                    List<string> indexs = arrayName.getIndex(maxIndexSize).ToList();
                    if (preIndexCount != indexs[0]) {
                        indexCount++;
                        preIndexCount = indexs[0];
                    }
                    indexs[0] = indexCount.ToString();
                    cList.Add(new ArrayName(destName, indexs, arrayName.mValue));
                }
            }
            remove(ret);
            mVar.setArrayList(cList);

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
        /// 配列のn番目に値を挿入(挿入元配列,挿入位置,挿入データ) array.insert(a[]/a[,], n, v/v[]);
        /// </summary>
        /// <param name="args"></param>
        private void insert(List<Token> args)
        {
            if (args.Count < 3)
                return;
            List<ArrayName> arrayList = mVar.getArrayList(args[0]);
            string destName = arrayList[0].mName;                   //  挿入先データ
            int indexSize = arrayList.Max(x => x.mIndexs.Count);    //  インデックスの数
            int n = (int)mVar.getDoubleFromArg(args[1]);            //  挿入位置
            List<ArrayName> blist = new List<ArrayName>();          //  挿入データ
            if (mVar.getArrayOder(args[2]) == 0) {
                //  挿入データが配列以外の変数または定数
                List<string> indexs = new List<string>();
                for (int i = 0; i < indexSize; i++)
                    indexs.Add("0");
                ArrayName arrayName = new ArrayName(destName, indexs, args[2].mValue);
                blist.Add(arrayName);
            } else {
                //  挿入データが配列
                blist = mVar.getArrayList(args[2]);
            }

            List<ArrayName> cList = new List<ArrayName>();
            int indexCount = -1;
            string preIndexCount = "*#*";  //    初期データ
            foreach (ArrayName arrayName in arrayList) {
                List<string> indexs = arrayName.getIndex(indexSize).ToList();
                if (preIndexCount != indexs[0]) {
                    if (indexCount == n - 1) {
                        string preInsIndexCount = "*#*";
                        foreach (var array in blist) {
                            List<string> insIndex = array.getIndex(indexSize).ToList();
                            if (preInsIndexCount != insIndex[0]) {
                                indexCount++;
                                preInsIndexCount = insIndex[0];
                            }
                            insIndex[0] = indexCount.ToString();
                            cList.Add(new ArrayName(destName, insIndex, array.mValue));
                        }
                    }
                    indexCount++;
                    preIndexCount = indexs[0];
                }
                indexs[0] = indexCount.ToString();
                cList.Add(new ArrayName(destName, indexs, arrayName.mValue));
            }
            remove(args[0]);
            mVar.setArrayList(cList);
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
