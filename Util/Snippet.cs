using CoreLib;
using System.IO;
using System.Windows;

namespace KScriptWin
{
    public class Snippet
    {
        private List<string> mKeyWordList = new List<string>(); //  入力候補リスト

        private YLib ylib = new YLib();

        public Snippet() { }

        public void clear()
        {
            mKeyWordList.Clear();
        }

        /// <summary>
        /// 予約語の追加登録
        /// </summary>
        /// <param name="keyWordList">予約語リスト</param>
        public void add(List<string> keyWordList)
        {
            mKeyWordList.AddRange(keyWordList);
        }

        /// <summary>
        /// 予約語の追加
        /// </summary>
        /// <param name="keyWordList">予約語リスト</param>
        public void add(string[] keyWordList)
        {
            mKeyWordList.AddRange(keyWordList);
        }

        /// <summary>
        /// 色リストの登録
        /// </summary>
        public void addColorList()
        {
            YLib.ColorTitle[] colorTitles = ylib.getColorList();
            foreach (var colorTitle in colorTitles)
                mKeyWordList.Add($"{colorTitle.colorTitle} 色");
        }

        /// <summary>
        /// スクリプトファイルから関数名の抽出
        /// </summary>
        /// <param name="filepath">スクリプトファイルパス</param>
        /// <returns>関数名リスト</returns>
        public List<string> getFuncList(string filepath)
        {
            List<string> funcList = new List<string>();
            if (File.Exists(filepath)) {
                string str = ylib.loadTextFile(filepath);
                List<string> list = exructFuncList(str);
                string buf = "";
                for (int i = 0; i < list.Count; i++) {
                    if (list[i].IndexOf("//") == 0)
                        buf = list[i].Substring(2).Trim();
                    else {
                        funcList.Add($"{list[i]} {buf}");
                        buf = "";
                    }
                }
            }
            return funcList;
        }

        /// <summary>
        /// スクリプトコードから関数名とコメントを抽出
        /// </summary>
        /// <param name="str">スクリプトコード</param>
        /// <returns>関数名リスト</returns>
        private List<string> exructFuncList(string str)
        {
            List<string> tokenList = new();
            string buf = "";
            for (int i = 0; i < str.Length; i++) {
                if (str[i] == ' ') {
                    continue;
                } else if (i < str.Length - 1 && str[i] == '/' && str[i + 1] == '/') {
                    buf = "";
                    while (i < str.Length && str[i] != '\n') {
                        buf += str[i++];
                    }
                    i--;
                    tokenList.Add(buf);
                } else if (Char.IsLetter(str[i])) {
                    buf = "";
                    while (i < str.Length && str[i] != ')') {
                        if (str[i] == ';') {
                            buf = "";
                            break;
                        }
                        if (str[i] != ' ' && str[i] != '\t' && str[i] != '\n')
                            buf += str[i++];
                        else
                            i++;
                    }
                    if (i < str.Length && str[i] == ')')
                        buf += str[i];
                    if (0 < buf.Length && 0 < buf.IndexOf('(') && 0 < buf.IndexOf(')'))
                        tokenList.Add(buf);
                } else if (str[i] == '{') {
                    i++;
                    int count = 0;
                    while (i < str.Length && !(count == 0 && str[i] == '}')) {
                        if (str[i] == '{') count++;
                        if (str[i] == '}') count--;
                        i++;
                    }
                }
            }
            return tokenList;
        }

        /// <summary>
        /// スニペット(カーソル位置の手前の単語から予約語などをメニュー表示する)
        /// </summary>
        /// <param name="window">親ウィンドウ</param>
        /// <param name="text">テキスト全体</param>
        /// <param name="selectionStart">カーソル位置</param>
        /// <returns>(text, selectStart)</returns>
        public (string text, int selectStart) showDialog(Window window, string text, int selectionStart)
        {
            //  カーソル前の単語の取得
            int cursorPos = selectionStart - 1;
            string word = "";
            int start = cursorPos;
            for (; 0 <= start; start--) {
                if ((!Char.IsLetterOrDigit(text[start]) && text[start] != '.') || start == 0) {
                    if (start == 0) {
                        word = text.Substring(start, cursorPos - start + 1);
                        start--;
                    } else
                        word = text.Substring(start + 1, cursorPos - start);
                    break;
                }
            }
            //  単語を含む予約語の抽出
            List<string> keyWords = new List<string>();
            foreach (var key in mKeyWordList)
                if (0 <= key.IndexOf(word, StringComparison.InvariantCultureIgnoreCase))
                    keyWords.Add(key);

            //  予約語のメニュー表示と選択
            MenuDialog dlg = new MenuDialog();
            dlg.Title = "関数などの候補";
            dlg.mMainWindow = window;
            dlg.mHorizontalAliment = 1;
            dlg.mVerticalAliment = 1;
            dlg.mOneClick = true;
            dlg.mMenuList = keyWords;
            dlg.ShowDialog();
            if (dlg.mResultMenu != "") {
                //  予約語の挿入
                string funcName = dlg.mResultMenu;
                if (0 <= funcName.IndexOf(" : "))
                    funcName = funcName.Substring(0, funcName.IndexOf(" : "));
                else if (0 <= funcName.IndexOf(' '))
                    funcName = funcName.Substring(0, funcName.IndexOf(' '));
                funcName = funcName.Trim();
                text = text.Remove(start + 1, cursorPos - start).Insert(start + 1, funcName);
                return (text, start + 1 + funcName.Length);
            } else
                return (text, start);
        }

        /// <summary>
        /// スクリプト文から#includeのファイル名を抽出
        /// </summary>
        /// <param name="text">スクリプト文</param>
        /// <returns>ファイル名リスト</returns>
        public List<string> getIncludeFilePath(string text, string scriptFolder)
        {
            List<string> flePath = new List<string>();
            string[] list = text.Split('\n');
            foreach (var line in list)
                if (0 <= line.IndexOf("#include")) {
                    List<string> path = ylib.extractBrackets(line, '"', '"');
                    if (0 < path.Count) {
                        flePath.Add(Path.Combine(scriptFolder, path[0]));
                    }
                }
            return flePath;
        }

    }
}
