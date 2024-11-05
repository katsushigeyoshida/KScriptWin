using CoreLib;
using System;
using System.Windows;

namespace KScriptWin
{
    /// <summary>
    /// 追加内部関数
    ///     input    : a = input();                         キー入力(文字列)
    ///     contains : a = contains(c[2]);                  配列の有無(0:なし 1:あり)
    ///     count    : size = count(a[]);                   1次元配列のサイズ
    ///                size = count(b[,]);                  2次元配列のサイズ
    ///                size = count(b[2,]);                 2次元配列1列目のサイズ
    ///     max      : max = max(a[]);                      配列の最大値
    ///     min      : min = min(a[,]);                     配列の最小値
    ///     sum      : sum = sum(a[]);                      配列の合計
    ///     average  : ave = average(a[,]);                 配列の平均
    ///     variance    : vari = variance(a[]);             分散
    ///     stdDeviation: std = stdDeviation(a[]);          標準偏差
    ///     covariance  : cov = covariance(x[],y[]);        共分散
    ///     corrCoeff   : corr = corrCoeff(x[],y[]);        相関係数
    ///     clear    : clear(a[]);                          配列クリア
    ///     remove   : remove(a[],st[,ed]);                 配列要素の削除
    ///     sort     : sort(a[]);                           ソート
    ///     reverse  : reverse(a[]);                        逆順
    ///     cmd      : cmd(command);                        Windowsコマンドの実行
    ///     
    ///     unitMatrix      : a[,] = unitMatrix(size);          単位行列の作成
    ///     matrixTranspose : b[,] = matrixTranspose(a[,]);     転置行列  行列Aの転置A^T
    ///     matrixMulti     : c[,] = matrixMulti(a[,], b[,]);   行列の積 AxB
    ///     matrixAdd       : c[,] = matrixAdd(a[,], b[,]);     行列の和 A+B
    ///     matrixInverse   : b[,] = matrixInverse(a[,]);       逆行列 A^-1
    ///     matrixCopy      : b[,] = matrixCopy(a[,]);          行列のコピー
    ///     
    /// Windows用関数
    ///     inputBox    : a = inputBox();                       文字入力ダイヤログ
    ///     messageBox  : messageBox(outString[, title]);       文字列のダイヤログ表示
    ///     menuSelect  : menuSelect(menu[],title);             メニューリストを表示して項目Noを返す
    ///     plotWindow  : plotWindow(left,bottom,right,top);    表示領域の設定
    ///     plotAspect  : plotAspect(1);                        アスペクト比固定の設定(0(非固定)/1(固定))
    ///     plotColor   : plotColor("Blue");                    要素の色設定
    ///     plotPointType   : plotPointType("cross");           点種の設定("dot", "cross", "plus", "box", "circle", "triangle")
    ///     plotLineType    : plotLineType("dash");             線種の設定("solid", "dash", "center", "phantom")
    ///     plotPointSize   : plotPointSize(3);                 点サイズの設定
    ///     plotLineThickness   : plotLineThickness(2);         線の太さの設定
    ///     plotPoint   : plotPoint(x, y);                      点の表示
    ///     plotLine    : plotLine(sx, sy, ex, ey);             線分の表示(始点x,y、終点x,y)
    ///     plotArc     : plotArc(cx, cy, r[, sa][, ea]);       円弧の表示(中心x,中心y,半径[、始角][、終角])
    ///     plotText    : plotText(text, x, y[, size[, rot[, ha[,va]]]]);  文字列の表示(文字列,座標x,座標y,サイズ,回転角,水平アライメント,垂直アライメント)
    ///     graphSet    ; graphSet(x[], y[]);                   グラフデータの設定(X[], Y[][, Title])
    ///     graphFontSize   : graphFontSize(5);                 グラフのフォントサイズの設定
    ///     
    /// 追加したい関数
    ///     key  : a = key();                               1文字入力
    ///     load : text = load(path);                       テキストファイルの読込み
    ///     save : save(path, text);                        テキストファイルの書き込み
    ///     graph                                           グラフ表示(Win版)
    ///     chart                                           チャート表示(Win版)
    ///     table                                           表編集(Win版)
    ///     imageView                                       イメージ表示(Win版)
    ///     
    /// 
    /// スクリプト関数の引数や戻り値を処理する関数
    ///     bool isStringArray(Token args)                      配列に文字列名があるかの確認
    ///     string[]? cnvArrayString(Token args)                配列をstring[]に変換
    /// 　　double[]? cnvArrayDouble(Token args)                配列をdouble[]に変換
    /// 　　double[,]? cnvArrayDouble2(Token args)              配列変数を実数配列double[,]に変換
    /// 　　List<double> cnvListDouble(Token arg)               配列データを実数のリストに変換
    /// 　　int getMaxArray(string arrayName)                   配列の最大インデックスを求める
    /// 　　(string name, int index) getArrayNo(string arrayName)   配列から配列名と配列のインデックスを取得
    /// 　　(string name, int? row, int? col) getArrayNo2(string arrayName) 2次元配列から配列名と行と列を取り出す
    /// 　　(string name, int no) getArrayName(Token args)      変数名または配列名と配列の次元の取得
    ///                                                     
    /// 　　void setReturnArray(Token[] src, Token dest)        配列戻り値に設定
    /// 　　void setReturnArray(double[,] src, Token dest)      2D配列の戻り値に設定
    /// 　　void setReturnArray(double[] src, Token dest)        配列の戻り値に設定
    /// 　　void setReturnArray(string[] src, Token dest)       文字列配列を戻り値に設定
    /// 
    /// </summary>
    public class ScriptLib
    {
        public static string[] mFuncNames = new string[] {
            "inputBox(); 文字入力ダイヤログ",
            "messageBox(outString[, title]); 文字列のダイヤログ表示",
            "menuSelect(menu[],title); メニューリストを表示して項目Noを返す",
            "contains(c[2]); 配列の有無(0:なし 1:あり)",
            "count(a[]); 1次元配列のサイズ",
            "count(a[,]); 2次元配列のサイズ",
            "count(a[1,]); 2次元配列1列目のサイズ",
            "clear(a[]); 配列クリア",
            "remove(a[],start[,end]); 配列要素の削除",
            "squeeze(a[]); 配列の未使用データを削除",
            "sort(a[]); 配列のソート",
            "reverse(a[]); 配列の逆順",
            "max(a[]); 配列の最大値",
            "min(a[,]); 配列の最小値",
            "sum(a[]);  配列の合計",
            "average(a[,]); 配列の平均",
            "variance(a[]); 配列の分散",
            "stdDeviation(a[]); 配列の標準偏差",
            "covariance(a[], b[]); 共分散",
            "corrCoeff(x[],y[]); 配列の相関係数",
            "cmd(command); Windowsコマンドの実行",
            "unitMatrix(size); 単位行列(2次元)の作成(a[,]=...)",
            "matrixTranspose(a[,]); 転置行列(2次元行列Aの転置(A^T) b[,]=...)",
            "matrixMulti(a[,], b[,]); 行列の積 AxB (c[,]=...)",
            "matrixAdd(a[,], b[,]); 行列の和 A+B c[,]=...)",
            "matrixInverse(a[,]); 逆行列 A^-1 (b[,]=...)",
            "matrixCopy(a[,]); 行列のコピー(b[,]=...)",
            "plotWindow(left,bottom,right,top); 表示領域の設定",
            "plotDisp(); グラフィックデータの再表示",
            "plotAspect(1); アスペクト比固定の設定(0(非固定)/1(固定))",
            "plotColor(\"Blue\"); 要素の色設定",
            "plotPointType(\"cross\"); 点種の設定(\"dot\", \"cross\", \"plus\", \"box\", \"circle\", \"triangle\")",
            "plotLineType(\"dash\"); 線種の設定(\"solid\", \"dash\", \"center\", \"phantom\")",
            "plotPointSize(3); 点サイズの設定",
            "plotLineThickness(2); 線の太さの設定",
            "plotPoint(x,y); 点の表示",
            "plotLine(sx,sy,ex,ey); 線分の表示(始点x,y、終点x,y)",
            "plotArc(cx,cy,r[,sa][,ea]); 円弧の表示(中心x,中心y,半径[、始角][、終角])",
            "plotText(text,x,y[,size[,rot[,ha[,va]]]]); 文字列の表示(文字列,X座標,Y座標,サイズ,回転角,水平アライメント,垂直アライメント)",
            "graphSet(x[],y[][,Title]); グラフデータの設定(X[],Y[][,Title])",
            "graphFontSize(5); グラフのフォントサイズの設定",
            "dateTimeNow(type); 現在の時刻を文字列で取得(0:\"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd 3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日",
            "startTime(); 時間計測の開始",
            "lapTime(); 経過時間の取得(秒)",
            "solveQuadraticEquation(a,b,c); 2次方程式の解(y = a*x^2+b*x+c)(y[] = solv..) ",
            "solveCubicEquation(a,b,c,d); 3次方程式の解(y = a*x^3+b*x^2+c*x+d)(y[] = solv..) ",
            "solveQuarticEquation(a,b,c,d,e); 4次方程式の解(y = a*x^4+b*x^3+c*x^2+d*x+e)(y[] = solv..) ",
        };

        //  共有クラス
        public Dictionary<string, Token> mVariables;    //  変数リスト(変数名,値)
        public KScript mScript;
        public GraphView mGraph;

        private DateTime mStartTime;
        private double mGraphFontSize = 12;
        private bool mAspectFix = true;

        private KLexer mLexer = new KLexer();
        private YCalc mCalc = new YCalc();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script">Scriptクラス</param>
        public ScriptLib(KScript script)
        {
            mScript = script;
            mVariables = script.mParse.mVariables;
            mGraph = script.mGraph;
        }

        /// <summary>
        /// 追加内部関数
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>戻り値</returns>
        public Token innerFunc(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "input": return new Token(Console.ReadLine(), TokenType.STRING);
                case "inputBox": return inputBox(args);
                case "messageBox": messageBox(args); break;
                case "contains": return contains(args);
                case "count": return getCount(args);
                case "clear": clear(args); break;
                case "remove": remove(args); break;
                case "squeeze": squeeze(args); break;
                case "sort": sort(args); break;
                case "reverse": reverse(args); break;
                case "max": return max(args);
                case "min": return min(args);
                case "sum": return sum(args);
                case "average": return average(args);
                case "variance": return variance(args);
                case "stdDeviation": return standardDeviation(args);
                case "covariance": return covariance(args);
                case "corrCoeff": return correlationCoefficient(args);
                case "cmd": cmd(args); break;
                case "unitMatrix": return unitMatrix(args, ret);
                case "matrixTranspose": return matrixTranspose(args, ret);
                case "matrixMulti": return matrixMulti(args, ret);
                case "matrixAdd": return matrixAdd(args, ret);
                case "matrixInverse": return matrixInverse(args, ret);
                case "matrixCopy": return matrixCopy(args, ret);
                case "plotWindow": plotWindow(args); break;
                case "plotDisp": plotDisp(); break;
                case "plotAspect": plotAspect(args); break;
                case "plotColor": plotColor(args); break;
                case "plotPointType": plotPointType(args); break;
                case "plotLineType": plotLineType(args); break;
                case "plotPointSize": plotPointSize(args); break;
                case "plotLineThickness": plotLineThickness(args); break;
                case "plotPoint": plotPoint(args); break;
                case "plotLine": plotLine(args); break;
                case "plotArc": plotArc(args); break;
                case "plotText": plotText(args); break;
                case "graphSet": graphSet(args); break;
                case "graphFontSize": graphFontSize(args); break;
                case "menuSelect": return menuSelect(args);
                case "dateTimeNow": return dateTimeNow(args);
                case "startTime": starTime(); break;
                case "lapTime": return lapTime();
                case "solveQuadraticEquation": return solveQuadraticEquation(args, ret);
                case "solveCubicEquation": return solveCubicEquation(args, ret);
                case "solveQuarticEquation": return solveQuarticEquation(args, ret);
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 時間計測の開始
        /// </summary>
        public void starTime()
        {
            mStartTime = DateTime.Now;
        }

        /// <summary>
        /// 経過時間の取得(秒)
        /// </summary>
        /// <returns>経過時間(s)</returns>
        public Token lapTime()
        {
            var endTime = DateTime.Now;
            return new Token((endTime - mStartTime).TotalSeconds.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 現在の時刻の取得
        /// 0:"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd
        /// 3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日
        /// </summary>
        /// <param name="args">書式の種類</param>
        /// <returns></returns>
        public Token dateTimeNow(List<Token> args)
        {
            if (args == null || args.Count == 0) 
                return new Token("", TokenType.ERROR);
            int format = ylib.intParse(mScript.getValueToken(args[0].mValue).mValue);
            if (format == 1) {
                string datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                return new Token(datetime, TokenType.STRING);
            } else if (format == 2) {
                string datetime = DateTime.Now.ToString("yyyy/MM/dd");
                return new Token(datetime, TokenType.STRING);
            } else if (format == 3) {
                string datetime = DateTime.Now.ToString("HH時mm分ss秒");
                return new Token(datetime, TokenType.STRING);
            } else if (format == 4) {
                string datetime = DateTime.Now.ToString("yyyy年MM月dd日 HH時mm分ss秒");
                return new Token(datetime, TokenType.STRING);
            } else if (format == 5) {
                string datetime = DateTime.Now.ToString("yyyy年MM月dd日");
                return new Token(datetime, TokenType.STRING);
            } else {
                string datetime = DateTime.Now.ToString("HH:mm:ss");
                return new Token(datetime, TokenType.STRING);
            }
        }


        /// <summary>
        /// メニューを出して項目を選択(inner function)
        /// </summary>
        /// <param name="args">引数(menu[],title)</param>
        /// <returns>項目No</returns>
        public Token menuSelect(List<Token> args)
        {
            int no = -1;
            if (args != null && 1 < args.Count) {
                List<string> menu = cnvListString(new Token(args[0].mValue, TokenType.ARRAY));
                MenuDialog dlg = new MenuDialog();
                dlg.Title = args[1].mValue.Trim('"');
                dlg.mOneClick = true;
                dlg.mMenuList = menu;
                dlg.ShowDialog();
                if (dlg.mResultMenu != "") {
                    no = menu.FindIndex(p => p == dlg.mResultMenu);
                }
            }
            return new Token(no.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// グラフのフォントサイズの設定(inner function)
        /// </summary>
        /// <param name="arg">サイズ</param>
        public void graphFontSize(List<Token> args)
        {
            if (args == null || args.Count == 0) return ;
            double size = ylib.doubleParse(mScript.getValueToken(args[0].mValue).mValue);
            mGraphFontSize = size;
        }

        /// <summary>
        /// グラフデータの設定(X[], Y[][, Title])(inner function)
        /// </summary>
        /// <param name="args">引数(x[],y[][,title]</param>
        public void graphSet(List<Token> args)
        {
            if (args.Count < 2) return ;
            List<double> x = cnvListDouble(args[0]);
            List<double> y = cnvListDouble(args[1]);
            if (x.Count != y.Count)
                return ;
            string title = "";
            if (2 < args.Count)
                title = mScript.getValueToken(args[2].mValue).mValue.Trim('"');

            if (mGraph != null)
                mGraph.Close();
            mGraph = new GraphView();
            mScript.mGraph = mGraph;
            mGraph.Show();
            mGraph.setAspectFix(0);
            mGraph.setFontSize(mGraphFontSize);
            mGraph.setGraph(x.ToArray(), y.ToArray(), title);
        }

        /// <summary>
        /// 表示領域の設定(left,bottom,right,top)(inner function)
        /// </summary>
        /// <param name="args">left,bottom,right,top</param>
        public void plotWindow(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i< args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (mGraph != null)
                mGraph.Close();
            mGraph = new GraphView();
            mGraph.mAspectFix = mAspectFix;
            mScript.mGraph = mGraph;
            mGraph.Show();
            if (3 < datas.Count)
                mGraph.setPlotWindow(datas[0], datas[1], datas[2], datas[3]);
        }

        /// <summary>
        /// 登録したデータを表示する
        /// </summary>
        public void plotDisp()
        {
            mGraph.plotDraw();
        }

        /// <summary>
        /// アスペクト比固定の設定(inner function)
        /// </summary>
        /// <param name="args">0(非固定)/1(固定)</param>
        public void plotAspect(List<Token> args)
        {
            int aspect = ylib.intParse(args[0].mValue);
            mAspectFix = aspect == 1 ? true : false;
            //mGraph.setAspectFix(aspect);
        }

        /// <summary>
        /// 要素の色設定(inner function)
        /// </summary>
        /// <param name="args">色名</param>
        public void plotColor(List<Token> args)
        {
            string colorName = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setColor(colorName);
        }

        /// <summary>
        /// 点種の設定("dot", "cross", "plus", "box", "circle", "triangle")(inner function)
        /// </summary>
        /// <param name="args">点種</param>
        public void plotPointType(List<Token> args)
        {
            string pointType = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setPointType(pointType);
        }

        /// <summary>
        /// 線種の設定("solid", "dash", "center", "phantom")(inner function)
        /// </summary>
        /// <param name="args">線種</param>
        public void plotLineType(List<Token> args)
        {
            string lineType = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setLineType(lineType);
        }

        /// <summary>
        /// 点サイズの設定(inner function)
        /// </summary>
        /// <param name="args">サイズ</param>
        public void plotPointSize(List<Token> args)
        {
            double pointSize = ylib.doubleParse(args[0].mValue);
            mGraph.setPointSize(pointSize);
        }

        /// <summary>
        /// 線の太さの設定(inner function)
        /// </summary>
        /// <param name="arg">線の太さ</param>
        public void plotLineThickness(List<Token> args)
        {
            double lineThickness = ylib.doubleParse(args[0].mValue);
            mGraph.setLineThickness(lineThickness);
        }

        /// <summary>
        /// 点の表示(inner function)
        /// </summary>
        /// <param name="args">点座標x,y</param>
        public void plotPoint(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (1 < datas.Count) {
                PointD point = new PointD(datas[0], datas[1]);
                mGraph.plotPoint(point);
            }
        }

        /// <summary>
        /// 線分の表示(始点x,y、終点x,y)(inner function)
        /// </summary>
        /// <param name="args">始終点座標</param>
        public void plotLine(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (3 < datas.Count) {
                LineD line = new LineD(datas[0], datas[1], datas[2], datas[3]);
                mGraph.plotLine(line);
            }
        }

        /// <summary>
        /// 円弧の表示(中心x,y,半径、始角、終角)(inner function)
        /// </summary>
        /// <param name="args">円弧データ</param>
        public void plotArc(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            ArcD arc;
            if (datas.Count == 3) {
                arc = new ArcD(datas[0], datas[1], datas[2]);
            } else if (datas.Count == 4) {
                arc = new ArcD(datas[0], datas[1], datas[2], datas[3]);
            } else if (datas.Count == 5) {
                arc = new ArcD(datas[0], datas[1], datas[2], datas[3], datas[4]);
            } else
                return;
            mGraph.plotArc(arc);
        }

        /// <summary>
        /// 文字列の表示(文字列,座標x,座標y,サイズ,回転角,水平アライメント,垂直アライメント)(inner function)
        /// </summary>
        /// <param name="args"></param>
        public void plotText(List<Token> args)
        {
            List<double> datas = new List<double>();
            string str = mLexer.stripBracketString(args[0].mValue, '"');
            for (int i = 1; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            TextD text;
            if (datas.Count == 2) {
                //  文字列と座標
                text = new TextD(str, new PointD(datas[0], datas[1]));
            } else if (datas.Count == 3) {
                //  + サイズ
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2]);
            } else if (datas.Count == 4) {
                //  + 回転角
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3]);
            } else if (datas.Count == 5) {
                //  + 水平アライメント
                HorizontalAlignment ha = datas[4] == 1 ? HorizontalAlignment.Center : datas[4] == 2 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3], ha);
            } else if (datas.Count == 6) {
                //  + 水平アライメント + 垂直アライメント
                HorizontalAlignment ha = datas[4] == 1 ? HorizontalAlignment.Center : datas[4] == 2 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                VerticalAlignment va = datas[5] == 1 ? VerticalAlignment.Center : datas[5] == 2 ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3], ha, va);
            } else 
                return ;
            mGraph.plotText(text);
        }


        /// <summary>
        /// 文字列入力ダイヤログボックス(ダイヤログタイトル)(inner function)
        /// </summary>
        /// <param name="args">タイトル</param>
        /// <returns>入力文字</returns>
        public Token inputBox(List<Token> args)
        {
            InputBox dlg = new InputBox();
            dlg.Title = args.Count < 1 ? "入力" : args[0].mValue.Trim('"');
            if (dlg.ShowDialog() == true) {
                if (ylib.IsNumberString(dlg.mEditText.ToString(), true))
                    return new Token(dlg.mEditText.ToString(), TokenType.LITERAL);
                else
                    return new Token(dlg.mEditText.ToString(), TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 文字列表示ダイヤログ(出力文字列[,タイトル])(inner function)
        /// </summary>
        /// <param name="args">出力文字列[,タイトル]</param>
        public void messageBox(List<Token> args)
        {
            if (args.Count ==1) {
                MessageBox.Show(args[0].mValue);
            } else if (args.Count == 2) {
                MessageBox.Show(args[0].mValue, args[1].mValue.Trim('"'));
            }
        }

        /// <summary>
        /// 配列変数の存在を確認(内部関数)
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>0:存在しない/1:存在する</returns>
        public Token contains(List<Token> args)
        {
            if (mVariables.ContainsKey(args[0].mValue))
                return new Token("1", TokenType.LITERAL);
            return new Token("0", TokenType.LITERAL);
        }

        /// <summary>
        /// 配列のサイズの取得(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>サイズ</returns>
        public Token getCount(List<Token> args)
        {
            int cp = args[0].mValue.LastIndexOf(',');
            int sp = args[0].mValue.IndexOf("[");
            if (0 < args[0].mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = args[0].mValue.Substring(0, cp + 1);   //  行単位の配列名(abc[a,
            else if (0 < sp)
                arrayName = args[0].mValue.Substring(0, sp);       //  配列名(abc[)
            int count = 0;
            foreach (var variable in mVariables) {
                if (0 < cp) {
                    //  2D配列の行単位でカウント
                    int ecp = variable.Key.LastIndexOf(",");
                    if (0 < ecp)
                        if (variable.Key.Substring(0, ecp + 1) == arrayName) count++;
                } else {
                    //  全カウント
                    int esp = variable.Key.IndexOf("[");
                    if (0 <= esp)
                        if (variable.Key.Substring(0, esp) == arrayName) count++;
                }
            }
            return new Token(count.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        public void clear(List<Token> args)
        {
            string arrayName = args[0].mValue.Substring(0, args[0].mValue.IndexOf("["));
            int count = 0;
            foreach (var variable in mVariables) {
                int sp = variable.Key.IndexOf("[");
                if (0 <= sp) {
                    if (variable.Key.Substring(0, sp) == arrayName)
                        mVariables.Remove(variable.Key);
                }
            }
        }

        /// <summary>
        /// 配列から要素を削除する(remove(a[],st[,ed]);)
        /// </summary>
        /// <param name="args">配列名と要素番号</param>
        public void remove(List<Token> args)
        {
            if (args.Count < 2) return;
            (string arrayName, int no) = getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            int st = ylib.intParse(args[1].mValue);
            int ed = args.Count > 2 ? ylib.intParse(args[2].mValue) : st;
            for (int i = st; i <= ed; i++) {
                string key = $"{arrayName}[{i}]";
                if (mVariables.ContainsKey(key))
                    mVariables.Remove(key);
            }
            squeeze(args);
        }

        /// <summary>
        /// 配列の圧縮(未使用インデックス削除)(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        public void squeeze(List<Token> args)
        {
            (string arrayName, int no) = getArrayName(args[0]);
            if (no != 1)
                return;
            List<Token> listToken = new();
            int maxcol = getMaxArray(arrayName);
            for (int i = 0; i < maxcol + 1; i++) {
                string key = $"{arrayName}[{i}]";
                if (mVariables.ContainsKey(key)) {
                    if (mVariables[key] != null)
                        listToken.Add(mVariables[key]);
                    mVariables.Remove(key);
                }
            }
            for (int i = 0; i < listToken.Count; i++) {
                string key = $"{arrayName}[{i}]";
                mVariables.Add(key, listToken[i]);
            }
        }

        /// <summary>
        /// ソート(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        public void sort(List<Token> args)
        {
            (string arrayName, int no) = getArrayName(args[0]);
            if (no != 1)
                return;
            if (isStringArray(args[0])) {
                //  文字列のソート
                string[]? strArray = cnvArrayString(args[0]);
                Array.Sort(strArray);
                clear(args);
                setReturnArray(strArray, args[0]);
            } else {
                //  実数のソート
                double[]? doubleArray = cnvArrayDouble(args[0]);
                Array.Sort(doubleArray);
                clear(args);
                setReturnArray(doubleArray, args[0]);
            }
        }

        /// <summary>
        /// 配列を逆順にする(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        public void reverse(List<Token> args)
        {
            (string arrayName, int no) = getArrayName(args[0]);
            if (no != 1) return;
            int maxcol = getMaxArray(arrayName);
            if (maxcol <= 0) return;
            Token[] tokens = new Token[maxcol + 1];
            arrayName += "[";
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
                    (string name, int col) = getArrayNo(variable.Key);
                    tokens[maxcol - col] = variable.Value;
                }
            }
            clear(args);
            setReturnArray(tokens, args[0]);
        }

        /// <summary>
        /// 最大値を求める(a[], a[,], a[x,])(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最大値</returns>
        public Token max(List<Token> args)
        {
            (string arrayName, int no) = getArrayName(args[0]);
            double max = double.MinValue;
            if (no == 1 || no == 2) {
                arrayName = getSearchName(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
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
        /// 最小値を求める(a[], a[,], a[x,])(inner function)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最小値</returns>
        public Token min(List<Token> args)
        {
            (string arrayName, int no) = getArrayName(args[0]);
            double min = double.MaxValue;
            if (no == 1 || no == 2) {
                arrayName = getSearchName(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
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
            (string arrayName, int no) = getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(args[0]);
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
            (string arrayName, int no) = getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(args[0]);
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
            (string arrayName, int no) = getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(args[0]);
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
            List<double> listData0 = cnvListDouble(args[0]);
            List<double> listData1 = cnvListDouble(args[1]);
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
            List<double> x = cnvListDouble(args[0]);
            List<double> y = cnvListDouble(args[1]);
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

        /// <summary>
        /// Windowsコマンド実行
        /// </summary>
        /// <param name="args"></param>
        private void cmd(List<Token> args)
        {
            ylib.openUrl(args[0].mValue);
        }

        /// <summary>
        /// 2次方程式の解を求める
        /// result[] = a * x^2 + b * x + c
        /// </summary>
        /// <param name="args">a,b,c</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveQuadraticEquation(List<Token> args, Token ret)
        {
            if (args.Count < 3) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double[] result = ylib.solveQuadraticEquation(a, b, c).ToArray();
            //  戻り値の設定
            setReturnArray(result, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 3次方程式の解を求める
        /// result[] = a * x^3 + b * x^2 + c * x + d
        /// </summary>
        /// <param name="args">a,b,c,d</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveCubicEquation(List<Token> args, Token ret)
        {
            if (args.Count < 4) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double d = ylib.doubleParse(args[3].mValue);
            double[] result = ylib.solveCubicEquation(a, b, c, d).ToArray();
            //  戻り値の設定
            setReturnArray(result, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 4次方程式の解を求める
        /// result[] = a * x^4 + b * x^3 + c * x^2 + d * x + e
        /// </summary>
        /// <param name="args">a,b,c,d,e</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveQuarticEquation(List<Token> args, Token ret)
        {
            if (args.Count < 5) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double d = ylib.doubleParse(args[3].mValue);
            double e = ylib.doubleParse(args[4].mValue);
            double[] result = ylib.solveQuarticEquation(a, b, c, d, e).ToArray();
            //  戻り値の設定
            setReturnArray(result, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 単位行列の作成(n x n)
        /// </summary>
        /// <param name="size">行列の大きさ</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token unitMatrix(List<Token> args, Token ret)
        {
            double[,] matrix = ylib.unitMatrix(ylib.intParse(args[0].mValue));

            //  戻り値の設定
            setReturnArray(matrix, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 転置行列  行列Aの転置A^T
        /// </summary>
        /// <param name="args">引数(行列 A</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixTranspose(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixTranspose(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列の積  AxB
        /// 行列の積では 結合の法則  (AxB)xC = Ax(BxC) , 分配の法則 (A+B)xC = AxC+BxC , Cx(A+B) = CxA + CxB　が可
        /// 交換の法則は成立しない  AxB ≠ BxA
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixMulti(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixMulti(a, b);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列の和 A+B
        /// 異なるサイズの行列はゼロ行列にする
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixAdd(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixAdd(a, b);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列)
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixInverse(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixInverse(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列のコピー(inner function)
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixCopy(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.copyMatrix(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 配列データを実数のリストに変換
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>実数リスト</returns>
        private List<double> cnvListDouble(Token arg)
        {
            List<double> listData = new List<double>();
            string arrayName = getSearchName(arg);
            foreach (var variable in mVariables) {
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
        private List<string> cnvListString(Token arg)
        {
            List<string> listData = new List<string>();
            string arrayName = getSearchName(arg);
            foreach (var variable in mVariables) {
                if (0 == variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType == TokenType.STRING)
                        listData.Add(variable.Value.getValue());
                }
            }
            return listData;
        }

        /// <summary>
        /// 配列検索用の配列名を求める(arrayName[, arraName[, , arrayName[aa, )
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>検索用配列名</returns>
        private string getSearchName(Token arg)
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
        /// 配列に文字列名があるかの確認
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>文字列あるなし</returns>
        public bool isStringArray(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (0 < no) {
                foreach (var variable in mVariables) {
                    if (variable.Key.IndexOf($"{arrayName}[") == 0) {
                        Token token = mVariables[variable.Key];
                        if (token.mType == TokenType.STRING)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 配列をstring[]に変換
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>string[]</returns>
        public string[]? cnvArrayString(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 1)
                return null;
            int maxCol = getMaxArray(arrayName);
            string[] ret = new string[maxCol + 1];
            for (int j = 0; j <= maxCol; j++) {
                string name = $"{arrayName}[{j}]";
                if (mVariables.ContainsKey(name))
                    ret[j] = mVariables[name].mValue;
            }
            return ret;
        }

        /// <summary>
        /// 配列をdouble[]に変換
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>double[]</returns>
        public double[]? cnvArrayDouble(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 1)
                return null;
            int maxCol = getMaxArray(arrayName);
            double[] ret = new double[maxCol + 1];
            for (int j = 0; j <= maxCol; j++) {
                string name = $"{arrayName}[{j}]";
                if (mVariables.ContainsKey(name))
                    ret[j] = ylib.doubleParse(mVariables[name].mValue);
            }
            return ret;
        }

        /// <summary>
        /// 配列の最大インデックスを求める
        /// </summary>
        /// <param name="arrayName">配列名</param>
        /// <returns>最大インデックス値</returns>
        public int getMaxArray(string arrayName)
        {
            int maxCol = 0;
            foreach (var variable in mVariables) {
                (string name, int? col) = getArrayNo(variable.Key);
                if (name == arrayName && col != null)
                    maxCol = Math.Max(maxCol, (int)col);
            }
            return maxCol;
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
            int maxRow = 0, maxCol = 0;
            foreach (var variable in mVariables) {
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
                    if (mVariables.ContainsKey(name))
                        ret[i, j] = ylib.doubleParse(mVariables[name].mValue);
                }
            }
            return ret;
        }

        /// <summary>
        /// 配列戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(Token[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.setVariable(key, src[i].copy());
            }
        }

        /// <summary>
        /// 2D配列の戻り値に設定
        /// </summary>
        /// <param name="src">2D配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(double[,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    mScript.mParse.setVariable(key, new Token(src[i, j].ToString(), TokenType.LITERAL));
                }
            }
        }

        /// <summary>
        /// 配列の戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(double[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.setVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 文字列配列を戻り値に設定
        /// </summary>
        /// <param name="src">文字列配列</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(string[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.setVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
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
            if (splitName.Count < 3)
                return ("", null, null);
            string name = splitName[0].mValue;
            int row = ylib.intParse(splitName[2].mValue);
            int col = ylib.intParse(splitName[4].mValue);
            return (name, row, col);
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
    }
}
