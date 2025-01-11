using CoreLib;
using System.Windows;

namespace KScriptWin
{
    /// <summary>
    /// 追加内部関数
    ///     input    : a = input();                         キー入力(文字列)
    ///     cmd      : cmd(command);                        Windowsコマンドの実行
    ///     array.xxxx                                      配列関数 (FuncArray.cs)
    ///     matrix.xxxx                                     マトリックス関数
    ///         a[,] = matrix.unit(size);                   単位行列の作成
    ///         b[,] = matrix.transpose(a[,]);              転置行列  行列Aの転置A^T
    ///         c[,] = matrix.multi(a[,], b[,]);            行列の積 AxB
    ///         c[,] = matrix.add(a[,], b[,]);              行列の和 A+B
    ///         b[,] = matrix.inverse(a[,]);                逆行列 A^-1
    ///         b[,] = matrix.copy(a[,]);                   行列のコピー
    ///     
    /// Windows用関数
    ///     inputBox    : a = inputBox();                   文字入力ダイヤログ
    ///     messageBox  : messageBox(outString[, title]);   文字列のダイヤログ表示
    ///     menuSelect  : menuSelect(menu[],title);         メニューリストを表示して項目Noを返す
    ///     plot.xxxx                                       グラフィック関数(FuncPlot.cs)
    ///     plot3D.xxxx                                     3Dグラフィック関数(FuncPlot3D.cs)
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
    /// スクリプト関数の引数や戻り値を処理する関数(KParse)
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
            "inKey(); キー入力",
            "sleep(n); スリープ(n msec)",
            "cmd(command); Windowsコマンドの実行",
            "matrix.unit(size); 単位行列(2次元)の作成(a[,]=...)",
            "matrix.transpose(a[,]); 転置行列(2次元行列Aの転置(A^T) b[,]=...)",
            "matrix.multi(a[,],b[,]); 行列の積 AxB (c[,]=...)",
            "matrix.add(a[,],b[,]); 行列の和 A+B c[,]=...)",
            "matrix.inverse(a[,]); 逆行列 A^-1 (b[,]=...)",
            "matrix.copy(a[,]); 行列のコピー(b[,]=...)",
            "dateTimeNow(type); 現在の時刻を文字列で取得(0:\"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd 3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日",
            "startTime(); 時間計測の開始",
            "lapTime(); 経過時間の取得(秒)",
            "solve.quadraticEquation(a,b,c); 2次方程式の解(y = a*x^2+b*x+c)(y[] = solv..) ",
            "solve.qubicEquation(a,b,c,d); 3次方程式の解(y = a*x^3+b*x^2+c*x+d)(y[] = solv..) ",
            "solve.quarticEquation(a,b,c,d,e); 4次方程式の解(y = a*x^4+b*x^3+c*x^2+d*x+e)(y[] = solv..) ",
        };

        //  共有クラス
        public KScript mScript;

        private DateTime mStartTime;
        private KParse mParse;
        private Variable mVar;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script">Scriptクラス</param>
        public ScriptLib(KScript script)
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
        public Token innerFunc(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                //case "input"            : return new Token(Console.ReadLine(), TokenType.STRING);
                case "inputBox"         : return inputBox(args);
                case "inKey"            : return inKey();
                case "messageBox"       : messageBox(args); break;
                case "sleep"            : sleep(args); break;
                case "cmd"              : cmd(args); break;
                case "matrix.unit"      : return unitMatrix(args, ret);
                case "matrix.transpose" : return matrixTranspose(args, ret);
                case "matrix.multi"     : return matrixMulti(args, ret);
                case "matrix.add"       : return matrixAdd(args, ret);
                case "matrix.inverse"   : return matrixInverse(args, ret);
                case "matrix.copy"      : return matrixCopy(args, ret);
                case "menuSelect"       : return menuSelect(args);
                case "dateTimeNow"      : return dateTimeNow(args);
                case "startTime"        : starTime(); break;
                case "lapTime"          : return lapTime();
                case "solve.quadraticEquation": return solveQuadraticEquation(args, ret);
                case "solve.qubicEquation"    : return solveCubicEquation(args, ret);
                case "solve.quarticEquation"  : return solveQuarticEquation(args, ret);
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
                List<string> menu = mVar.cnvListString(new Token(args[0].mValue, TokenType.ARRAY));
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
        /// キー入力
        /// </summary>
        /// <returns>キーコード</returns>
        public Token inKey()
        {
            mScript.mControlData.mKey = true;
            while (mScript.mControlData.mKey) {
                Thread.Sleep(100);
                ylib.DoEvents();
            }
            return new Token(mScript.mControlData.mKeyCode.ToString(), TokenType.STRING);
        }

        /// <summary>
        /// スリーブ(1/10s単位)
        /// </summary>
        /// <param name="args"></param>
        public void sleep(List<Token> args)
        {
            if (0 < args.Count) {
                int count = ylib.intParse(args[0].mValue);
                while (0 < count) {
                    Thread.Sleep(60);
                    ylib.DoEvents();
                    count--;
                }
            }
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
        /// Windowsコマンド実行
        /// </summary>
        /// <param name="args"></param>
        private void cmd(List<Token> args)
        {
            ylib.openUrl(ylib.stripBracketString(args[0].mValue,'"'));
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
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            mVar.setReturnArray(matrix, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixTranspose(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = mVar.cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixMulti(a, b);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = mVar.cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixAdd(a, b);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixInverse(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
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
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.copyMatrix(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }
    }
}
