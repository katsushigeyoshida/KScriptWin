using CoreLib;
using System.Windows;

namespace KScriptWin
{
    /// <summary>
    /// GraphView.xaml の相互作用ロジック
    /// </summary>
    public partial class GraphView : Window
    {
        private double mWindowWidth;            //  ウィンドウの高さ
        private double mWindowHeight;           //  ウィンドウ幅
        private double mPrevWindowWidth;        //  変更前のウィンドウ幅
        private double mPrevWindowHeight;
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)
        private string[] mGraphMenu = { "散布図", "折線", "棒グラフ" };

        public double[] mX;
        public double[] mY;

        public enum MODE { PLOT, GRAPH }
        public MODE mGraphMode = MODE.GRAPH;
        public enum GRAPHTYPE { non, scatter, line, bar }
        public GRAPHTYPE mGraphType = GRAPHTYPE.scatter;

        private GraphDraw mDraw;
        private YLib ylib = new YLib();

        public GraphView()
        {
            InitializeComponent();

            WindowFormLoad();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbGraphType.ItemsSource = mGraphMenu;
            mDraw = new GraphDraw(cnCanvas);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            //  最大化時の処理
            if (WindowState != mWindowState &&
                WindowState == WindowState.Maximized) {
                mWindowWidth = SystemParameters.WorkArea.Width;
                mWindowHeight = SystemParameters.WorkArea.Height;
            } else if (WindowState != mWindowState ||
                mWindowWidth != Width ||
                mWindowHeight != Height) {
                mWindowWidth = Width;
                mWindowHeight = Height;
            } else {
                //  ウィンドウサイズが変わらない時は何もしない
                mWindowState = this.WindowState;
                return;
            }
            //  ウィンドウの大きさに合わせてコントロールの高さを変更する
            if (WindowState == mWindowState && WindowState == WindowState.Maximized)
                return;

            mWindowState = WindowState;
            mPrevWindowWidth = mWindowWidth;
            mPrevWindowHeight = mWindowHeight;
            //  再描画処理
            if (mDraw != null) {
                if (mGraphMode == MODE.GRAPH)
                    graphDraw();
                else
                    plotDraw();
            }
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.GraphViewWidth < 100 ||
                Properties.Settings.Default.GraphViewHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.GraphViewHeight) {
                Properties.Settings.Default.GraphViewWidth = mWindowWidth;
                Properties.Settings.Default.GraphViewHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.GraphViewTop;
                Left = Properties.Settings.Default.GraphViewLeft;
                Width = Properties.Settings.Default.GraphViewWidth;
                Height = Properties.Settings.Default.GraphViewHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.GraphViewTop = Top;
            Properties.Settings.Default.GraphViewLeft = Left;
            Properties.Settings.Default.GraphViewWidth = Width;
            Properties.Settings.Default.GraphViewHeight = Height;
            Properties.Settings.Default.Save();
        }

        private void cbGraphType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int index = cbGraphType.SelectedIndex;
            if (0 <= index) {
                switch (index) {
                    case 0: mGraphType = GRAPHTYPE.scatter; break;
                    case 1: mGraphType = GRAPHTYPE.line; break;
                    case 2: mGraphType = GRAPHTYPE.bar; break;
                    default: mGraphType = GRAPHTYPE.non; break;
                }
                graphDraw();
            }
        }

        /// <summary>
        /// プロットデータの表示
        /// </summary>
        public void plotDraw()
        {
            mDraw.drawInit();
            mDraw.plotDraw();
        }

        /// <summary>
        /// グラフデータの表示
        /// </summary>
        public void graphDraw()
        {
            mDraw.drawInit();
            mDraw.setGraphWindow();
            mDraw.drawGraph();
        }

        /// <summary>
        /// Graphモードでデータ設定
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setGraph(double[] x, double[] y, string title = "Graph")
        {
            Title = $"GraphView [{title}]"; 
            mDraw.mX = x;
            mDraw.mY = y;
            mGraphMode = MODE.GRAPH;

            mDraw.drawInit();
            mDraw.setGraphWindow();
            mDraw.drawGraph();
        }

        /// <summary>
        /// PLOTモードでワールドウィンドウの範囲を設定
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        public void setPlotWindow(double left, double bottom, double right, double top, string title = "Plot")
        {
            Title = $"PlotView [{title}]";
            mGraphMode = MODE.PLOT;
            mDraw.setWindow(left, bottom, right, top);
            mDraw.drawInit();
            mDraw.plotClear();
        }

        /// <summary>
        /// アスペクト比固定の設定
        /// </summary>
        /// <param name="aspect">0/1</param>
        public void setAspectFix(int aspect)
        {
            mDraw.setAspectFix(aspect == 1 ? true : false);
        }

        /// <summary>
        /// カラー設定
        /// </summary>
        /// <param name="color">カラー名</param>
        public void setColor(string color)
        {
            mDraw.plotColor(color);
        }

        /// <summary>
        /// 点サイズの設定
        /// </summary>
        /// <param name="size">点サイズ</param>
        public void setPointSize(double size)
        {
            mDraw.setPlotPointSize(size);
        }

        /// <summary>
        /// 線幅の設定
        /// </summary>
        /// <param name="thickness">線幅</param>
        public void setLineThickness(double thickness)
        {
            mDraw.setPlotLineThickness(thickness);
        }

        /// <summary>
        /// 文字の大きさの設定(スクリーンサイズ)
        /// </summary>
        /// <param name="size"></param>
        public void setFontSize(double size)
        {
            mDraw.mFontSize = size;
        }


        /// <summary>
        /// 点種の設定
        /// </summary>
        /// <param name="pointType">点種名</param>
        public void setPointType(string pointType)
        {
            mDraw.plotPointType(pointType);
        }

        /// <summary>
        /// 線種の設定
        /// </summary>
        /// <param name="lineType">線種名</param>
        public void setLineType(string lineType)
        {
            mDraw.plotLineType(lineType);
        }

        /// <summary>
        /// 点の表示
        /// </summary>
        /// <param name="point">点座標</param>
        public void plotPoint(PointD point)
        {
            mDraw.plotPoint(point);
        }

        /// <summary>
        /// 線分の表示
        /// </summary>
        /// <param name="line">線分</param>
        public void plotLine(LineD line)
        {
            mDraw.plotLine(line);
        }

        /// <summary>
        /// 円弧の表示
        /// </summary>
        /// <param name="arc">円弧データ</param>
        public void plotArc(ArcD arc)
        {
            mDraw.plotArc(arc);
        }

        /// <summary>
        /// 文字列の表示
        /// </summary>
        /// <param name="text">文字列データ</param>
        public void plotText(TextD text)
        {
            mDraw.plotText(text);
        }
    }
}
