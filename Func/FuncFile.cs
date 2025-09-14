using CoreLib;
using System.Globalization;
using System.IO;

namespace KScriptWin
{
    public class FuncFile
    {
        public static string[] mFuncNames = new string[] {
            "file.fileExists(path); ファイルの存在確認",
            "file.dirExists(path); ディレクトリの存在確認",
            "file.makeDir(dir); ディレクトリの作成",
            "file.copy(src,dest); ファイルのコピー",
            "file.move(path,dir); ファイルの移動",
            "file.rename(path,new); ファイル名変更",
            "file.delete(path); ファイル削除",
            "file.delDir(dir); ファイル削除",
            "file.getFileName(path); ファイル名の取得",
            "file.getDirectory(path); ディレクトリ名の取得",
            "file.getExtention(path); 拡張子の取得",
            "file.getFileNameWithoutExtension(path); 拡張子なしのファイル名",
            "file.loadText(path); テキストファイルの読込",
            "file.saveText(path,text); テキストのファイル保存",
            "file.loadCsv(path); CSVファイルの読込",
            "file.saveCsv(path,text[,]); テキストのCSV保存",
            "file.size(path); ファイルサイズの取得",
            "file.select(dir,ext); ファイル選択",
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
        public FuncFile(KScript script)
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
                case "file.fileExists": return fileExists(args);
                case "file.dirExists": return dirExists(args);
                case "file.makeDir": return makeDir(args);
                case "file.copy": return copy(args);
                case "file.move": return move(args);
                case "file.rename": return rename(args);
                case "file.delete": return delete(args);
                case "file.delDir": return delDir(args);
                case "file.getFileName": return getFileName(args);
                case "file.getDirectory": return getDirectory(args);
                case "file.getExtention": return getExtention(args);
                case "file.getFileNameWithoutExtension": return getFileNameWithoutExtension(args);
                case "file.loadText": return loadText(args);
                case "file.saveText": return saveText(args);
                case "file.loadCsv": return loadCsvData(args, ret);
                case "file.saveCsv": return saveCsvData(args);
                case "file.size": return filesize(args);
                case "file.lastWrite": return lastWrite(args);
                case "file.select": return fileSelect(args);
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイルの存在確認(fileExists(path))
        /// </summary>
        /// <param name="args"></param>
        /// <returns>(0/1)</returns>
        private Token fileExists(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                if (File.Exists(str))
                    return new Token("1", TokenType.LITERAL);
                else
                    return new Token("0", TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ディレクトリの存在確認(dirExists(filename))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token dirExists(List<Token> args)
        {
            if (0 < args.Count) {
                string str = args[0].getValue();
                if (Directory.Exists(str))
                    return new Token("1", TokenType.LITERAL);
                else
                    return new Token("0", TokenType.LITERAL);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ディレクトリの作成(makeDir(filename))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token makeDir(List<Token> args)
        {
            if (0 < args.Count) {
                string dir = args[0].getValue();
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイルのコピー(copy(srcPath,destPath))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token copy(List<Token> args)
        {
            if (1 < args.Count) {
                string srcpath = args[0].getValue();
                string dstpath = args[1].getValue();
                if (File.Exists(srcpath)) {
                    string dir = Path.GetDirectoryName(dstpath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.Copy(srcpath, dstpath);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイルの移動/リネーム(move(oldpath,newpath))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token move(List<Token> args)
        {
            if (1 < args.Count) {
                string srcpath = args[0].getValue();
                string dstpath = args[1].getValue();
                if (File.Exists(srcpath)) {
                    string dir = Path.GetDirectoryName(dstpath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    File.Move(srcpath, dstpath);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイル名だけを変更(rename("c:\filename\name.txt", "nawName"))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token rename(List<Token> args)
        {
            if (1 < args.Count) {
                string path = args[0].getValue();
                string dir = Path.GetDirectoryName(path);
                string newName = args[1].getValue();
                if (File.Exists(path)) {
                    string newPath = Path.Combine(dir, newName);
                    if (!File.Exists(newPath))
                        File.Move(path, newPath);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイル削除(delete(path))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token delete(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ディレクトリ削除(delDir(filename))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token delDir(List<Token> args)
        {
            if (0 < args.Count) {
                string dir = args[0].getValue();
                if (Directory.Exists(dir)) {
                    Directory.Delete(dir);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// パスからファイル名を抽出
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token getFileName(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                string fileName = Path.GetFileName(path);
                return new Token(fileName, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// パスからディレクトリ名を抽出
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token getDirectory(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                string dir = Path.GetDirectoryName(path);
                return new Token(dir, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイル名から拡張子を抽出
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token getExtention(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                string ext = Path.GetExtension(path);
                return new Token(ext, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// パス名から拡張子抜きのファイル名を抽出
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token getFileNameWithoutExtension(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                string filename = Path.GetFileNameWithoutExtension(path);
                return new Token(filename, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// テキストファイルの読込
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token loadText(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                string text = ylib.loadTextFile(path);
                return new Token(text, TokenType.STRING);
            }

            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// テキストのファイル保存
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token saveText(List<Token> args)
        {
            if (1 < args.Count) {
                string path = args[0].getValue();
                string text = args[1].getValue();
                ylib.saveTextFile(path, text);
            }

            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// CSVファイルデータを2次元配列に格納(a[,] = file.loadCsv(path))
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token loadCsvData(List<Token> args, Token ret)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                List<string[]> listData = ylib.loadCsvData(path);
                int maxlength = listData.Max(p => p.Length);
                string[,] texts = new string[listData.Count, maxlength];
                for (int i = 0; i < listData.Count; i++) {
                    for (int j = 0; j < listData[i].Length; j++)
                        texts[i, j] = listData[i][j];
                }
                mVar.setReturnArray(texts, ret);
                //  戻り値の設定
                mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
                return mVar.getVariable("return");
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 2次元配列をCSVファイルに保存(file.saveCsv(path,text[,]))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token saveCsvData(List<Token> args)
        {
            if (1 < args.Count) {
                string path = args[0].getValue();
                string[,] texts = mVar.cnvArrayString2(args[1]);
                List<string[]> listData = new List<string[]>();
                for (int i = 0; i < texts.GetLength(0); i++) {
                    string[] strings = new string[texts.GetLength(1)];
                    for(int j = 0; j < texts.GetLength(1); j++)
                        strings[j] = texts[i, j];
                    listData.Add(strings);
                }
                ylib.saveCsvData(path, listData);
            }

            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイルサイズ(file.size(path))
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token filesize(List<Token> args)
        {
            if (0 < args.Count) {
                string path = args[0].getValue();
                if (File.Exists(path)) {
                    FileInfo fi = new FileInfo(path);
                    return new Token(fi.Length.ToString(), TokenType.LITERAL);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイルの日時の取得(lastWrite(path[,form[,"jp"]])
        /// form = ""           yyyy/mm/dd hh/mm/ss
        ///        "d"          6/19/2025
        ///        "D"          Wednesday, June 19, 2025
        ///        "F"          Wednesday, June 19, 2025 2:45:30
        ///        "s"          2025-06-19T14:45:30
        ///        "yyyy-MM-dd"         2025-06-19"
        ///        "MM/dd/yyyy HH:mm"   06/19/2025 14:45
        ///        "dddd, MMMM dd"      Wednesday, June 19
        ///        "hh:mm tt"           02:45 PM
        ///        "D", "jp"            2025年6月19日水曜日
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token lastWrite(List<Token> args)
        {
            string form = "";
            CultureInfo culture = new CultureInfo("en-US");
            if (0 < args.Count) {
                string path = args[0].getValue();
                if (1 < args.Count)
                    form = args[1].getValue();
                if (2 < args.Count)
                    if (args[2].getValue().ToLower() == "jp")
                        culture = new CultureInfo("ja-JP");
                if (File.Exists(path)) {
                    FileInfo fi = new FileInfo(path);
                    DateTime time = fi.LastWriteTime;
                    return new Token(time.ToString(form, culture), TokenType.STRING);
                }
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// ファイル選択 (path = file.select(".","テキストファイル,*.txt");)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Token fileSelect(List<Token> args)
        {
            List<string[]> filters = new List<string[]>() ;
            if (0 < args.Count) {
                string dir = args[0].getValue();
                if (1 < args.Count) {
                    for (int i = 1; i < args.Count; i++) {
                        string[] filter = args[i].getValue().Split(',');
                        filters.Add(filter);
                    }
                }
                filters.Add(new string[] { "すべてのファイル", "*.*" });
                string path = ylib.fileOpenSelectDlg("ファイル選択", dir, filters);
                if (0 < path.Length)
                    return new Token(path, TokenType.STRING);
            }
            return new Token("", TokenType.EMPTY);
        }
    }


}

