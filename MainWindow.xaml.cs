using CoreLib;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KScriptWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///
    /// KScript のWindows実行環境
    /// </summary>
    public partial class MainWindow : Window
    {
        private double mWindowWidth;            //  ウィンドウの高さ
        private double mWindowHeight;           //  ウィンドウ幅
        private double mPrevWindowWidth;        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        private string mDataFolder = "";        //  スクリプトファイルのフォルダ
        private string mFilePath = "";          //  スクリプトファイルパス
        private byte[] mSrcHash;                //  スクリプトの読み込み時ハッシュコード
        private double mFontSize = 12;          //  AvalonEditorのフォントサイズ
        private string mFontFamily = "Consolas";//  AvalonEditorのフォントファミリ
        private int mSearchWordIndex = 0;       //  検索開始位置
        private string mFolderListPath = "FolderList.csv";

        private RingBuffer<string> mOutBuffer = new RingBuffer<string>(2000);   //  出力表示のバッファ
        private GraphView mGraph;               //  グラフィックWindow
        private KScript mScript;                //  スクリプト本体
        private List<string> mKeyWordList = new List<string>(); //  入力候補リスト
        private string mHelpFile = "KScriptWinManual.pdf";      //  マニュアルファイル

        private YLib ylib = new YLib();

        public MainWindow()
        {
            InitializeComponent();

            //  FontFamilyの設定
            cbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            //  snippedの予約語
            mKeyWordList.AddRange(KScript.mStatmantHelp);
            mKeyWordList.AddRange(ScriptLib.mFuncNames);
            mKeyWordList.AddRange(YCalc.mFuncList);
            mKeyWordList.AddRange(YDraw.mLineTypeHelp);
            mKeyWordList.AddRange(YDraw.mPointTypeHelp);
            YLib.ColorTitle[] colorTitles = ylib.getColorList();
            foreach (var colorTitle in colorTitles)
                mKeyWordList.Add(colorTitle.colorTitle);

            //  KScriptの設定
            mScript = new KScript();
            mScript.printCallback = outputDisp;      //  出力表示先の設定
            mScript.mGraph = mGraph;

            WindowFormLoad();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //   int index = cbFontFamily.Items.IndexOf(mFontFamily);
            int index = -1;
            if (mFontFamily == "") mFontFamily = "Consolas";
            for (int i = 0; i < cbFontFamily.Items.Count; i++) {
                if (cbFontFamily.Items[i].ToString() == mFontFamily)
                    index = i;
            }
            if (0 <= index)
                cbFontFamily.SelectedIndex = index;
            if (mFontSize == 0)
                mFontSize = 12;
            avalonEditor.FontSize = mFontSize;
            //  参照フォルダの設定
            if (0 < mDataFolder.Length && Directory.Exists(mDataFolder)) {
                setRefFileList(mDataFolder);
                cbFolderList.Items.Add(mDataFolder);
                loadFolderList();
                cbFolderList.SelectedIndex = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mGraph != null)
                mGraph.Close();

            closeCheck();
            saveFolderLis();
            WindowFormSave();
        }

        /// <summary>
        /// 状態の復元
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.MainWindowWidth < 100 ||
                Properties.Settings.Default.MainWindowHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.MainWindowHeight) {
                Properties.Settings.Default.MainWindowWidth = mWindowWidth;
                Properties.Settings.Default.MainWindowHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.MainWindowTop;
                Left = Properties.Settings.Default.MainWindowLeft;
                Width = Properties.Settings.Default.MainWindowWidth;
                Height = Properties.Settings.Default.MainWindowHeight;
            }
            mDataFolder = Properties.Settings.Default.EditorDataFolder;
            double fontSize = Properties.Settings.Default.FontSize;
            if (0< fontSize) mFontSize = fontSize;
            string fontFamily = Properties.Settings.Default.FontFamily;
            if (0 < fontFamily.Length) mFontFamily = fontFamily;
        }

        /// <summary>
        /// 状態の保存
        /// </summary>
        private void WindowFormSave()
        {
            Properties.Settings.Default.EditorDataFolder = mDataFolder;
            Properties.Settings.Default.FontSize = mFontSize;
            Properties.Settings.Default.FontFamily = mFontFamily;
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.MainWindowTop = Top;
            Properties.Settings.Default.MainWindowLeft = Left;
            Properties.Settings.Default.MainWindowWidth = Width;
            Properties.Settings.Default.MainWindowHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 参照ファイルのフォルダ選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cbFolderList.SelectedIndex;
            if (0 < index) {
                string folder = cbFolderList.Items[index].ToString();
                setRefFileList(folder);
                cbFolderList.Items.Remove(folder);
                cbFolderList.Items.Insert(0, folder);
                cbFolderList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 参照ファイルのフォルダを登録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFolderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string folder = ylib.folderSelect("フォルダ選択", ".");
            if (0 < folder.Length) {
                if (cbFolderList.Items.Contains(folder))
                    cbFolderList.Items.Remove(folder);
                cbFolderList.Items.Insert(0, folder);
                setRefFileList(folder);
                cbFolderList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 参照ファイルの選択表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lbFileList.SelectedIndex;
            if (0 <= index) {
                string path = Path.Combine(cbFolderList.Text, lbFileList.Items[index].ToString());
                tbReference.Text = ylib.loadTextFile(path);
            }
        }

        /// <summary>
        /// フォントの選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFontFamily.SelectedItem != null) {
                avalonEditor.FontFamily = (FontFamily)cbFontFamily.SelectedItem;
                mFontFamily = cbFontFamily.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// メニューの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            if (menuItem.Name.CompareTo("fileNewMenu") == 0) {
                newScript();
            } else if (menuItem.Name.CompareTo("fileOpenMenu") == 0) {
                selectLoad();
            } else if (menuItem.Name.CompareTo("fileSaveMenu") == 0) {
                save();
            } else if (menuItem.Name.CompareTo("fileSaveAsMenu") == 0) {
                saveAs();
            } else if (menuItem.Name.CompareTo("fileCloseMenu") == 0) {
                Close();
            } else if (menuItem.Name.CompareTo("editSearchMenu") == 0) {
                search();
            } else if (menuItem.Name.CompareTo("editReplaceMenu") == 0) {

            } else if (menuItem.Name.CompareTo("editToCommentMenu") == 0) {
                toComment();
            } else if (menuItem.Name.CompareTo("GraphViewMenu") == 0) {
                if (mGraph != null) {
                    mGraph.Close();
                }
                mGraph = new GraphView();
                mGraph.Show();
            }
        }

        /// <summary>
        /// ボタンの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name.CompareTo("btNew") == 0) {
                newScript();
            } else if (button.Name.CompareTo("btOpen") == 0) {
                selectLoad();
            } else if (button.Name.CompareTo("btSave") == 0) {
                save();
            } else if (button.Name.CompareTo("btSaveAs") == 0) {
                saveAs();
            } else if (button.Name.CompareTo("btFontUp") == 0) {
                mFontSize += 1;
                avalonEditor.FontSize = mFontSize;
            } else if (button.Name.CompareTo("btFontDown") == 0) {
                mFontSize -= 1;
                avalonEditor.FontSize = mFontSize;
            } else if (button.Name.CompareTo("btExecute") == 0) {
                exeute();
            } else if (button.Name.CompareTo("btAbort") == 0) {
                mScript.mControlData.mAbort = true;
                outMessage("Abort\n");
            } else if (button.Name.CompareTo("btPause") == 0) {
                mScript.mControlData.mPause = !mScript.mControlData.mPause;
                outMessage("Pause\n");
            } else if (button.Name.CompareTo("btSearch") == 0) {
                search();
            } else if (button.Name.CompareTo("btHelp") == 0) {
                ylib.openUrl(mHelpFile);
            }
        }

        /// <summary>
        /// Editor部のキー入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void avalonEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"Prev Editor KEY: {e.Key} {e.KeyboardDevice.Modifiers == ModifierKeys.Control} {e.KeyboardDevice.Modifiers == ModifierKeys.Shift}");
        }

        /// <summary>
        /// 全体のキー入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (keyCommand(e.Key, e.KeyboardDevice.Modifiers == ModifierKeys.Control, e.KeyboardDevice.Modifiers == ModifierKeys.Shift))
                e.Handled = true;   //  イベントキャンセル
            else
                e.Handled = false;
            //if (avalonEditor.IsKeyboardFocusWithin) {
            //    System.Diagnostics.Debug.WriteLine($"Prev Win KEY: {e.Key} {e.KeyboardDevice.Modifiers == ModifierKeys.Control} {e.KeyboardDevice.Modifiers == ModifierKeys.Shift}");
            //}
        }

        /// <summary>
        /// 新規
        /// </summary>
        private void newScript()
        {
            closeCheck();
            avalonEditor.Text = "";
            mFilePath = "";
            setTitle();
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        private void exeute()
        {
            outMessage($"Start : [{Path.GetFileNameWithoutExtension(mFilePath)}]");
            mScript.mControlData.mAbort = false;
            mScript.mControlData.mPause = false;
            mScript.mScriptFolder = mDataFolder;
            mScript.setScript(avalonEditor.Text);
            mScript.execute("main");
            mGraph = mScript.mGraph;
            outMessage($"End : [{Path.GetFileNameWithoutExtension(mFilePath)}]");
        }

        /// <summary>
        /// コメントの追加/解除
        /// </summary>
        private void toComment()
        {
            bool remove = false;
            string text = avalonEditor.Text;
            int selSp = avalonEditor.SelectionStart;
            int selEp = selSp + avalonEditor.SelectionLength;
            //  改行位置と選択行位置を求める
            int stLine = -1;
            int endLine = -1;
            List<int> crList = new List<int>() { 0 };
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n')
                    crList.Add(i + 1);
            }
            crList.Add(text.Length);
            for (int i = 0; i < crList.Count - 1; i++) {
                if (crList[i] <= selSp && selSp < crList[i + 1])
                    stLine = i;
                if (crList[i] <= selEp && selEp < crList[i + 1])
                    endLine = i;
            }
            if (stLine < 0) stLine = crList.Count - 2;
            if (endLine < 0) endLine = crList.Count - 2;
            //  選択開始行のコメントの有無を確認
            string strLine = text.Substring(crList[stLine],
                stLine < crList.Count - 1 ? crList[stLine + 1] - crList[stLine] : text.Length - crList[stLine]);
            if (0 <= strLine.IndexOf("//"))
                remove = true;
            //  コメント化/コメント解除
            for (int i = crList.Count - 2; 0 <= i; i--) {
                if ((selSp < crList[i + 1] || selSp == avalonEditor.Document.TextLength)
                    && crList[i] <= selEp) {
                    if (remove)
                        removeComment(crList[i], selEp);
                    else
                        avalonEditor.Document.Insert(crList[i], "//");
                }
            }
            selSp = Math.Max(0, Math.Min(selSp, avalonEditor.Document.TextLength));
            avalonEditor.Select(selSp, 0);
        }

        /// <summary>
        /// 指定位置のコメント解除
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="sp">位置</param>
        /// <returns>解除後のテキスト</returns>
        private void removeComment(int sp, int ep)
        {
            for (int i = sp; i <= ep && i < avalonEditor.Document.Text.Length - 1; i++) {
                if (avalonEditor.Document.Text[i] == '/' && avalonEditor.Document.Text[i + 1] == '/') {
                    avalonEditor.Document.Remove(i, 2);
                    break;
                }
            }
        }


        /// <summary>
        /// 検索
        /// </summary>
        private void search()
        {
            string searchWord = cbSearchWord.Text;
            int index = avalonEditor.Text.IndexOf(searchWord, mSearchWordIndex);
            if (index != -1) {
                avalonEditor.Select(index, searchWord.Length);
                mSearchWordIndex = index + searchWord.Length;
                int lineCount = 0;
                for (int i = 0; i < index; i++) {
                    if (avalonEditor.Text[i] == '\n')
                        lineCount++;
                }
                avalonEditor.ScrollToLine(lineCount);
            } else {
                mSearchWordIndex = 0;
            }
        }

        /// <summary>
        /// ファイルに保存
        /// </summary>
        private void save()
        {
            if (0 < mFilePath.Length) {
                ylib.saveTextFile(mFilePath, avalonEditor.Text);
                setHash(avalonEditor.Text);
            } else
                saveAs();
        }

        /// <summary>
        /// 名前をつけて保存
        /// </summary>
        private void saveAs()
        {
            List<string[]> filters = new List<string[]>() { new string[] { "scファイル", "*.sc" } };
            mFilePath = ylib.fileSaveSelectDlg("ファイル保存", mDataFolder, filters);
            if (0 < mFilePath.Length) {
                string ext = Path.GetExtension(mFilePath);
                if (ext == "")
                    mFilePath += ".sc";
                ylib.saveTextFile(mFilePath, avalonEditor.Text);
                mDataFolder = Path.GetDirectoryName(mFilePath);
                setHash(avalonEditor.Text);
                setTitle();
            }
        }

        /// <summary>
        /// ファイルを選択して読み込む
        /// </summary>
        private void selectLoad()
        {
            closeCheck();
            List<string[]> filters = new List<string[]>() { new string[] { "scファイル", "*.sc" } };
            string path = ylib.fileOpenSelectDlg("ファイル選択", mDataFolder, filters);
            if (path != null && 0 < path.Length) {
                mDataFolder = Path.GetDirectoryName(path);
                mFilePath = path;
                string text = ylib.loadTextFile(path);
                avalonEditor.Text = text;
                setHash(text);
                setTitle();
            }
        }

        /// <summary>
        /// 文字列のハッシュ値を保存
        /// </summary>
        /// <param name="text"></param>
        private void setHash(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            MD5 md5 = MD5.Create();
            mSrcHash = md5.ComputeHash(data);
            md5.Clear();
        }

        /// <summary>
        /// 保存されているハッシュ値の比較
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool compareHash(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            MD5 md5 = MD5.Create();
            byte[] tempHash = md5.ComputeHash(data);
            return mSrcHash.SequenceEqual(tempHash);
        }

        /// <summary>
        /// 変更を確認して保存
        /// </summary>
        private void closeCheck()
        {
            if ((mSrcHash != null && !compareHash(avalonEditor.Text)) ||
                (mSrcHash == null && 0 < avalonEditor.Text.Length)) {
                if (MessageBox.Show("内容が変更されていますが保存しますか?", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    save();
                }
            }
        }

        /// <summary>
        /// 時刻付きでメッセージを出力
        /// </summary>
        /// <param name="msg">メッセージ</param>
        private void outMessage(string msg)
        {
            string buf = $"[{DateTime.Now.ToString("HH:mm:ss")}] {msg}\n";
            outputDisp(buf);
        }

        /// <summary>
        /// 出力ウィンドウに表示(callback用)
        /// </summary>
        public void outputDisp()
        {
            outputDisp(mScript.mControlData.mOutputString);
        }

        /// <summary>
        /// 出力ウィンドウに表示
        /// </summary>
        /// <param name="str">文字列</param>
        public void outputDisp(string output)
        {
            mOutBuffer.Add(output);
            string buf = "";
            foreach (var text in mOutBuffer)
                buf += text;
            tbOutput.Text = buf;
            tbOutput.Select(tbOutput.Text.Length, 0);
            tbOutput.ScrollToEnd();
            ylib.DoEvents();
        }

        /// <summary>
        /// タイトル設定
        /// </summary>
        private void setTitle()
        {
            Title = $"KScriptWin [ {Path.GetFileNameWithoutExtension(mFilePath)} ]";
        }

        /// <summary>
        /// キー入力処理
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <param name="control">Cntrolの有無</param>
        /// <param name="shift">Shiftの有無</param>
        private bool keyCommand(Key key, bool control, bool shift)
        {
            if (control && shift) {
                switch (key) {
                    case Key.S: saveAs(); break;
                    default: return false; ;
                }
            } else if (control) {
                switch (key) {
                    case Key.D:
                        mScript.mControlData.mPause = !mScript.mControlData.mPause;
                        outMessage("Pause\n");
                        break;
                    case Key.F: search(); break;
                    case Key.N: newScript(); break;
                    case Key.O: selectLoad(); break;
                    case Key.S: save(); break;
                    case Key.Divide: toComment(); break;
                    case Key.OemQuestion: toComment(); break;
                    default: return false; ;
                }
            } else {
                switch (key) {
                    case Key.F5: exeute(); break;
                    case Key.F8:
                    case Key.Apps: snippet(); break;    //  入力候補
                    default: return false;;
                }
            }
            return true;
        }

        /// <summary>
        /// スニペット(カーソル位置の手前の単語から予約語などをメニュー表示する)
        /// </summary>
        private void snippet()
        {
            //  カーソル前の単語の取得
            int cursorPos = avalonEditor.SelectionStart - 1;
            string word = "";
            int start = cursorPos;
            for (; 0 <= start; start--) {
                if (!Char.IsLetterOrDigit(avalonEditor.Text[start]) || start == 0) {
                    if (start == 0) {
                        word = avalonEditor.Text.Substring(start, cursorPos - start + 1);
                        start--;
                    } else
                        word = avalonEditor.Text.Substring(start + 1, cursorPos - start);
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
            dlg.mMainWindow = this;
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
                avalonEditor.Document.Text = avalonEditor.Document.Text.Remove(start + 1, cursorPos - start).Insert(start + 1, funcName);
                avalonEditor.Select(start + 1 + funcName.Length, 0);
            }
        }

        /// <summary>
        /// 参照ファイルリストの登録
        /// </summary>
        /// <param name="dataFolder">参照ファイルのフォルダ</param>
        private void setRefFileList(string dataFolder)
        {
            dataFolder = Path.Combine(dataFolder, "*.*");
            string[] fileList = ylib.getFiles(dataFolder);
            lbFileList.Items.Clear();
            foreach (var path in fileList)
                lbFileList.Items.Add(Path.GetFileName(path));
        }

        /// <summary>
        /// 参照ファイルのフォルダリストの読込
        /// </summary>
        private void loadFolderList()
        {
            if (File.Exists(mFolderListPath)) {
                List<string> folderList = ylib.loadListData(mFolderListPath);
                foreach (var folder in folderList) {
                    if (Directory.Exists(folder)) {
                        if (!cbFolderList.Items.Contains(folder))
                            cbFolderList.Items.Add(folder);
                    }
                }
            }
        }

        /// <summary>
        /// 参照ファイルのフォルダリストの保存
        /// </summary>
        private void saveFolderLis()
        {
            List<string> folderList = new List<string>();
            foreach (var folder in cbFolderList.Items)
                folderList.Add(folder.ToString());
            ylib.saveListData(mFolderListPath, folderList);
        }
    }
}