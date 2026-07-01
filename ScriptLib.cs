using CoreLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace KScriptWin
{
    /// <summary>
    /// 追加内部関数
    ///     input    : a = input();                         キー入力(文字列)
    ///     cmd      : cmd(command);                        Windowsコマンドの実行s)
    ///     inKey    : inkey();                             キー入力
    ///     sleep    : sleep(n);                            スリープ(n msec)",
    ///     dateTimeNow : a = dateTimeNow(type);            現在の時刻を文字列で取得(0:\"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd 
    ///                                                         3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日"),
    ///     startTime : startTime();                        時間計測の開始
    ///     lapTime   : a = lapTime();                      経過時間の取得(秒)
    ///     
    ///     y[] = solve.quadraticEquation(a,b,c);           2次方程式の解(y = a*x^2+b*x+c)(y[] = solv..) ",
    ///     y[] = solve.qubicEquation(a,b,c,d);             3次方程式の解(y = a*x^3+b*x^2+c*x+d)(y[] = solv..) ",
    ///     y[] = solve.quarticEquation(a,b,c,d,e);         4次方程式の解(y = a*x^4+b*x^3+c*x^2+d*x+e)(y[] = solv..) ",
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
    /// </summary>
    public class ScriptLib
    {
        public static string[] mFuncNames = new string[] {
            "inputBox(); 文字入力ダイヤログ",
            "inputBoxMulti(dataList[,]); 複数項目入力",
            "messageBox(outString[, title]); 文字列のダイヤログ表示",
            "menuSelect(menu[],title); メニューリストを表示して項目Noを返す",
            "inKey(); キー入力",
            "sleep(n); スリープ(n msec)",
            "cmd(command); Windowsコマンドの実行",
            "dateTimeNow(type); 現在の時刻を文字列で取得(0:\"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd 3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日",
            "startTime(); 時間計測の開始",
            "lapTime(); 経過時間の取得(秒)",
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
                case "inputBoxMulti"    : return inputBoxMulti(args, ret);
                case "menuSelect"       : return menuSelect(args);
                case "inKey"            : return inKey();
                case "messageBox"       : messageBox(args); break;
                case "sleep"            : sleep(args); break;
                case "cmd"              : cmd(args); break;
                case "dateTimeNow"      : return dateTimeNow(args);
                case "startTime"        : starTime(); break;
                case "lapTime"          : return lapTime();
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
        /// 現在の時刻の取得(dateTimeNow() / dateTimeNow(No) / dateTimeNow(Form))
        /// 0:"HH:mm:ss 1:yyyy/MM/dd HH:mm:ss 2:yyyy/MM/dd
        /// 3:HH時mm分ss秒 4:yyyy年MM月dd日 HH時mm分ss秒 5:yyyy年MM月dd日
        /// </summary>
        /// <param name="args">書式の種類</param>
        /// <returns></returns>
        public Token dateTimeNow(List<Token> args)
        {
            string datetime = "";
            if (args == null || args.Count == 0) {
                datetime = DateTime.Now.ToString("HH:mm:ss");
                return new Token(datetime, TokenType.STRING);
            }
            string form = args[0].getValue();
            int formNo = ylib.intParse(form);
            if (formNo == 1) {
                datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            } else if (formNo == 2) {
                datetime = DateTime.Now.ToString("yyyy/MM/dd");
            } else if (formNo == 3) {
                datetime = DateTime.Now.ToString("HH時mm分ss秒");
            } else if (formNo == 4) {
                datetime = DateTime.Now.ToString("yyyy年MM月dd日 HH時mm分ss秒");
            } else if (formNo == 5) {
                datetime = DateTime.Now.ToString("yyyy年MM月dd日");
            } else if (formNo == 0) {
                datetime = DateTime.Now.ToString(form);
            } else {
                datetime = DateTime.Now.ToString("HH:mm:ss");
            }
            return new Token(datetime, TokenType.STRING);
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
        /// 複数項目入力ダイヤログ(data[,] = inputBoxMulti(data[,], title);
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token inputBoxMulti(List<Token> args, Token ret)
        {
            if (0 < args.Count && mVar.getArrayOder(args[0]) == 2) {
                string[,] data = mVar.cnvArrayString2(args[0]);
                string title = "タイトル";
                List<string[]> dataList = new List<string[]>();
                for (int i = 0; i < data.GetLength(0); i++) {
                    if (1 < data.GetLength(1)) {
                        string[] ddate = new string[] { ylib.stripBracketString(data[i, 0], '"'), ylib.stripBracketString(data[i, 1], '"') };
                        dataList.Add(ddate);
                    }
                }
                if (1 < args.Count && mVar.getArrayOder(args[1]) == 0)
                    title = mVar.getStringFromArg(args[1]);
                InputBoxMulti dlg = new InputBoxMulti();
                dlg.Title = title;
                dlg.mDataList = dataList;
                if (dlg.ShowDialog() == true) {
                    for (int i = 0; i < dlg.mDataList.Count; i++) {
                        data[i, 1] = dlg.mDataList[i][1];
                    }
                    //  戻り値の設定
                    mVar.setReturnArray(data, ret);
                    mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
                    return mVar.getVariable("return");
                }
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
    }
}
