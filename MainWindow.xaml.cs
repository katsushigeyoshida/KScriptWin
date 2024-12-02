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
        private Plot3DView mPlot3D;             //  3Dグラフィック
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

            //  snippet の読込
            setSnippetKeyword();

            //  KScriptの設定
            mScript = new KScript();
            mScript.printCallback = outputDisp;      //  出力表示先の設定
            mScript.mGraph = mGraph;
            mScript.mPlot3D = mPlot3D;
            setTitle();

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
            if (mPlot3D != null)
                mPlot3D.Close();

            closeCheck();
            saveFolderList();
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
                tbReference.Text = ylib.tab2space(ylib.loadTextFile(path));
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
            } else if (menuItem.Name.CompareTo("editTab2SpaceMenu") == 0) {
                tab2space();
            } else if (menuItem.Name.CompareTo("GraphViewMenu") == 0) {
                graphView();
            } else if (menuItem.Name.CompareTo("Plot3DViewMenu") == 0) {
                plot3DView();
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
                if (mScript.mControlData.mPause)
                    mScript.mControlData.mPause = false;
                else
                    exeute();
            } else if (button.Name.CompareTo("btAbort") == 0) {
                mScript.mControlData.mAbort = true;
                outMessage("Abort\n");
            } else if (button.Name.CompareTo("btPause") == 0) {
                mScript.mControlData.mPause = !mScript.mControlData.mPause;
                if (mScript.mControlData.mPause)
                    outMessage("Pause\n");
            } else if (button.Name.CompareTo("btSearch") == 0) {
                search();
            } else if (button.Name.CompareTo("btHelp") == 0) {
                ylib.openUrl(mHelpFile);
            }
        }

        /// <summary>
        /// 参照ファイルリストのコンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbFileListMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            int index = lbFileList.SelectedIndex;
            string path = "";
            if (0 <= index)
                path = Path.Combine(cbFolderList.Text, lbFileList.Items[index].ToString());

            if (menuItem.Name.CompareTo("lbFileListCopyMenu") == 0) {
                copyFile(path);
            } else if (menuItem.Name.CompareTo("lbFileListmoveMenu") == 0) {
                copyFile(path, true);
            } else if (menuItem.Name.CompareTo("lbFileListRemoveMenu") == 0) {
                removeFile(path);
            } else if (menuItem.Name.CompareTo("lbFileListRenameMenu") == 0) {
                renameFile(path);
            }
        }

        /// <summary>
        /// 参照ファイルのコピー
        /// </summary>
        /// <param name="path">コピー元ファイルパス</param>
        /// <param name="move">コピー/移動</param>
        private void copyFile(string path, bool move = false)
        {
            string folder = ylib.folderSelect("フォルダの選択", Path.GetDirectoryName(path));
            if (0 < folder.Length) {
                string newPath = Path.Combine(folder, Path.GetFileName(path));
                if (File.Exists(newPath)) {
                    if (ylib.messageBox(this, newPath + " が存在します。上書きしますか", "",
                        "ファイルコピー/削除", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        File.Delete(newPath);
                    else
                        return;
                }
                if (move) {
                    File.Move(path, newPath);
                    folder = Path.GetDirectoryName(path);
                    setRefFileList(folder);
                } else {
                    File.Copy(path, newPath);
                }
            }
        }

        /// <summary>
        /// ファイルの削除
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private void removeFile(string path)
        {
            string folder = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            if (ylib.messageBox(this, fileName + " を削除します", "", "ファイル削除", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                File.Delete(path);
                setRefFileList(folder);
            }
        }

        /// <summary>
        /// ファイル名の変更
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private void renameFile(string path)
        {
            string folder = Path.GetDirectoryName(path);
            InputBox dlg = new InputBox();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Title = "ファイル名変更";
            dlg.mEditText = Path.GetFileName(path);
            if (dlg.ShowDialog() == true) {
                string newFilePath = Path.Combine(folder, dlg.mEditText.ToString());
                if (File.Exists(newFilePath)) {
                    ylib.messageBox(this, "すでにファイルが存在しています。");
                } else {
                    File.Move(path, newFilePath);
                    setRefFileList(folder);
                }
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
            mPlot3D = mScript.mPlot3D;
            outMessage($"End : [{Path.GetFileNameWithoutExtension(mFilePath)}]");
        }

        /// <summary>
        /// 選択行のタブをスペースに変換
        /// </summary>
        private void tab2space()
        {
            int selSp = avalonEditor.SelectionStart;
            int selEp = selSp + avalonEditor.SelectionLength;
            List<int> crList = getCrList(avalonEditor.Document.Text);   //  改行位置
            for (int i = crList.Count - 2; 0 <= i; i--) {
                if (selSp < crList[i + 1] && crList[i] < selEp) {
                    string text = avalonEditor.Document.Text.Substring(crList[i], crList[i + 1] - crList[i] - 1);
                    avalonEditor.Document.Remove(crList[i], text.Length);
                    text = ylib.tab2space(text);
                    avalonEditor.Document.Insert(crList[i], text);
                }
            }
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
            List<int> crList = getCrList(text);   //  改行位置
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
            int index = strLine.ToList().FindIndex(c => (c != ' ' && c != '\t'));
            if (index < strLine.Length - 1 && strLine[index] == '/' && strLine[index + 1] == '/')
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
        /// テキストの改行位置をリスト化する
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>改行位置リスト</returns>
        private List<int> getCrList(string text)
        {
            List<int> crList = new List<int>() { 0 };   //  改行位置
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n')
                    crList.Add(i + 1);
            }
            crList.Add(text.Length);
            return crList;
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
            List<string[]> filters = new List<string[]>() {
                new string[] { "scファイル", "*.sc" }, new string[] { "全ファイル", "*.*" }
            };
            string path = ylib.fileOpenSelectDlg("ファイル選択", mDataFolder, filters);
            if (path != null && 0 < path.Length) {
                mDataFolder = Path.GetDirectoryName(path);
                mFilePath = path;
                string text = ylib.loadTextFile(path);
                avalonEditor.Text = text;
                setHash(text);
                setTitle();
                updateSnippetKeyWird(text);
            }
        }

        /// <summary>
        /// 入力候補のデータを更新する
        /// </summary>
        /// <param name="text">スクリプトコード</param>
        private void updateSnippetKeyWird(string text)
        {
            List<string> paths = getIncludeFilePath(text);
            if (0 < paths.Count)
                setSnippetKeyword(paths);
            else
                setSnippetKeyword();
        }

        /// <summary>
        /// スクリプト文から#includeのファイル名を抽出
        /// </summary>
        /// <param name="text">スクリプト文</param>
        /// <returns>ファイル名リスト</returns>
        private List<string> getIncludeFilePath(string text)
        {
            List<string> flePath = new List<string>();
            string[] list = text.Split('\n');
            foreach (var line in list)
                if (0 <= line.IndexOf("#include")) {
                    List<string> path = ylib.extractBrackets(line, '"', '"');
                    if (0 < path.Count) {
                        flePath.Add(Path.Combine(mDataFolder, path[0]));
                    }
                }
            return flePath;
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
                    case Key.F12: updateSnippetKeyWird(avalonEditor.Text); break;
                    default: return false;;
                }
            }
            return true;
        }

        /// <summary>
        /// snippets 予約語の登録
        /// </summary>
        /// <param name="filepath">追加ファイルパス</param>
        public void setSnippetKeyword(List<string> files = null)
        {
            mKeyWordList.Clear();
            //  snippetの予約語
            mKeyWordList.AddRange(KScript.mStatmantHelp);
            mKeyWordList.AddRange(ScriptLib.mFuncNames);
            mKeyWordList.AddRange(FuncArray.mFuncNames);
            mKeyWordList.AddRange(FuncPlot.mFuncNames);
            mKeyWordList.AddRange(FuncPlot3D.mFuncNames);
            mKeyWordList.AddRange(YCalc.mFuncList);
            mKeyWordList.AddRange(YDraw.mLineTypeHelp);
            mKeyWordList.AddRange(YDraw.mPointTypeHelp);
            YLib.ColorTitle[] colorTitles = ylib.getColorList();
            foreach (var colorTitle in colorTitles)
                mKeyWordList.Add($"{colorTitle.colorTitle} 色");
            if (files != null && 0 < files.Count) {
                foreach (var filepath in files) {
                    if (0 < filepath.Length) {
                        mKeyWordList.AddRange(getFuncList(filepath));
                    }
                }
            }
        }

        /// <summary>
        /// スクリプトファイルから関数名の抽出
        /// </summary>
        /// <param name="filepath">スクリプトファイルパス</param>
        /// <returns>関数名リスト</returns>
        private List<string> getFuncList(string filepath)
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
        List<string> exructFuncList(string str)
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
        private void snippet()
        {
            //  カーソル前の単語の取得
            int cursorPos = avalonEditor.SelectionStart - 1;
            string word = "";
            int start = cursorPos;
            for (; 0 <= start; start--) {
                if ((!Char.IsLetterOrDigit(avalonEditor.Text[start]) && avalonEditor.Text[start] != '.') || start == 0) {
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
        private void saveFolderList()
        {
            List<string> folderList = new List<string>();
            foreach (var folder in cbFolderList.Items)
                folderList.Add(folder.ToString());
            ylib.saveListData(mFolderListPath, folderList);
        }

        private void graphView()
        {
            if (mGraph != null) {
                mGraph.Close();
            }
            mGraph = new GraphView();
            mGraph.Show();
        }


        private void plot3DView()
        {
            if (mPlot3D != null)
                mPlot3D.Close();
            mPlot3D = new Plot3DView();
            mPlot3D.Show();
        }
    }
}