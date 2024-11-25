using CoreLib;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace KScriptWin
{
    /// <summary>
    /// Plot3DView.xaml の相互作用ロジック
    /// 
    /// OpenTKのインストール(Ver 3.3.3) (Ver 4では動作しないようだ)
    ///     NuGet で OpenTK.GLControl をインストールする
    /// OpenGL で描画する領域
    ///     ツールボックスから[WindowsFormsHos] をメインウィンドウに設定する
    /// GLを使うには using OpenTK.Graphics.OpenGL;が必要
    /// </summary>
    public partial class Plot3DView : Window
    {
        private double mWindowWidth;            //  ウィンドウの高さ
        private double mWindowHeight;           //  ウィンドウ幅
        private double mPrevWindowWidth;        //  変更前のウィンドウ幅
        private System.Windows.WindowState mWindowState = System.Windows.WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        private Vector3 mMin = new Vector3(-1, -1, -1);     //  表示領域の最小値
        private Vector3 mMax = new Vector3(1, 1, 1);        //  表示領域の最大値
        private Color4 mBackColor = Color4.AliceBlue;       //  背景色
        private double m3DScale = 1;                        //  3D表示の初期スケール
        private bool mAxisDisp = true;
        private bool mFrameDisp = true;

        private Plot3DDraw m3DDraw;
        private GL3DLib m3DLib;                             //  三次元表示ライブラリ
        private GLControl glControl;                        //  OpenTK.GLcontrol
        private YLib ylib = new YLib();

        public Plot3DView()
        {
            InitializeComponent();

            //  OpenGLの設定
            glControl = new GLControl();
            m3DLib = new GL3DLib(glControl);
            m3DLib.initPosition(1.3f, 0f, 0f, 0f);
            m3DLib.setArea(mMin, mMax);
            //  OpenGLイベント処理
            glControl.Load       += GlControl_Load;
            glControl.Paint      += GlControl_Paint;
            glControl.Resize     += GlControl_Resize;
            glControl.MouseDown  += GlControl_MouseDown;
            glControl.MouseUp    += GlControl_MouseUp;
            glControl.MouseMove  += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;
            glHost.Child = glControl;                //  OpenGLをWindowsに接続

            m3DDraw = new Plot3DDraw();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowFormLoad();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (WindowState != mWindowState && WindowState == System.Windows.WindowState.Maximized) {
                //  ウィンドウの最大化時
                mWindowWidth = SystemParameters.WorkArea.Width;
                mWindowHeight = SystemParameters.WorkArea.Height;
            } else if (WindowState != mWindowState ||
                mWindowWidth != Width || mWindowHeight != Height) {
                //  ウィンドウサイズが変わった時
                mWindowWidth = Width;
                mWindowHeight = Height;
            } else {
                //  ウィンドウサイズが変わらない時は何もしない
                mWindowState = WindowState;
                return;
            }
            mWindowState = WindowState;
        }
        /// <summary>
        /// 状態の復元
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.Plot3DViewWidth < 100 ||
                Properties.Settings.Default.Plot3DViewHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.Plot3DViewHeight) {
                Properties.Settings.Default.Plot3DViewWidth = mWindowWidth;
                Properties.Settings.Default.Plot3DViewHeight = mWindowHeight;
            } else {
                Top    = Properties.Settings.Default.Plot3DViewTop;
                Left   = Properties.Settings.Default.Plot3DViewLeft;
                Width  = Properties.Settings.Default.Plot3DViewWidth;
                Height = Properties.Settings.Default.Plot3DViewHeight;
            }
        }

        /// <summary>
        /// 状態の保存
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Plot3DViewTop    = Top;
            Properties.Settings.Default.Plot3DViewLeft   = Left;
            Properties.Settings.Default.Plot3DViewWidth  = Width;
            Properties.Settings.Default.Plot3DViewHeight = Height;
            Properties.Settings.Default.Save();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            keyCommand(e.Key, e.KeyboardDevice.Modifiers == ModifierKeys.Control, e.KeyboardDevice.Modifiers == ModifierKeys.Shift);

        }

        private void GlControl_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            //throw new NotImplementedException();
            float delta = (float)e.Delta / 1000f;// - wheelPrevious;
            m3DLib.setZoom(delta);
            renderFrame();
        }

        private void GlControl_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (m3DLib.moveObject(e.X, e.Y))
                renderFrame();
        }

        private void GlControl_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            //throw new NotImplementedException();
            m3DLib.setMoveEnd();
        }

        private void GlControl_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Button == MouseButtons.Left) {
                m3DLib.setMoveStart(true, e.X, e.Y);
            } else if (e.Button == MouseButtons.Right) {
                m3DLib.setMoveStart(false, e.X, e.Y);
            }
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
            GL.Viewport(glControl.ClientRectangle);
        }

        private void GlControl_Paint(object? sender, System.Windows.Forms.PaintEventArgs e)
        {
            //throw new NotImplementedException();
            renderFrame();
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
            m3DLib.initLight();
        }

        /// <summary>
        /// 三次元データ表示
        /// </summary>
        public void renderFrame()
        {
            m3DLib.mWorldWidth = (int)glHost.ActualWidth;
            m3DLib.mWorldHeight = (int)glHost.ActualHeight;
            if (m3DLib.mWorldWidth == 0 || m3DLib.mWorldHeight == 0)
                return;
            m3DLib.setBackColor(mBackColor);
            m3DLib.renderFrameStart();
            //  Surfaceデータの取得
            List<SurfaceData> slist = m3DDraw.mSurfaceDataList;
            //  表示領域にはいるようにスケールと位置移動ベクトルを求める
            double scale = m3DScale / m3DDraw.mArea.getSize();
            Point3D v = m3DDraw.mArea.getCenter();
            v.inverse();
            //  データの登録
            for (int i = 0; i < slist.Count; i++) {
                m3DLib.drawSurface(slist[i].mVertexList, slist[i].mDrawType,
                    slist[i].mFaceColor, scale, v);
            }

            m3DLib.setAreaFrameDisp(mAxisDisp, mFrameDisp);
            //m3DLib.drawAxis(plotScale, new Point3D(0, 0, 0));
            m3DLib.drawAxis(scale, v);
            m3DLib.rendeFrameEnd();
        }

        /// <summary>
        /// キー操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="control"></param>
        /// <param name="shift"></param>
        private void keyCommand(Key key, bool control, bool shift)
        {
                // 3D表示
                m3DLib.keyMove(key, control, shift);
                renderFrame();
        }

        /// <summary>
        /// 初期表示エリアの設定
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void setArea(Point3D min, Point3D max)
        {
            m3DDraw.mArea = new Box3D(min, max);
            m3DDraw.mArea.normalize();
        }

        /// <summary>
        /// 軸とフレーム表示の有無
        /// </summary>
        /// <param name="axisDisp"></param>
        /// <param name="frameDisp"></param>
        public void setAxisFrameDisp(bool axisDisp, bool frameDisp)
        {
            mAxisDisp = axisDisp;
            mFrameDisp = frameDisp;
        }

        /// <summary>
        /// 要素の色の設定
        /// </summary>
        /// <param name="color"></param>
        public void setColor(string color)
        {
            m3DDraw.setColor(color);
        }

        /// <summary>
        /// 移動 (vec[x/y/z])
        /// </summary>
        /// <param name="vec">移動量</param>
        public void translate(Point3D vec)
        {
            m3DDraw.translate(vec);
        }

        /// <summary>
        /// 回転 (X/Y/Z軸を中心に回転)
        /// </summary>
        /// <param name="angle">回転角度</param>
        /// <param name="cood">回転軸("X"/"Y"/"Z")</param>
        public void rotate(double angle, string cood)
        {
            FACE3D face = FACE3D.XY;
            if (cood == "X")
                face = FACE3D.YZ;
            else if (cood == "Y")
                face = FACE3D.ZX;
            m3DDraw.rotate(angle, face);
        }

        /// <summary>
        /// 拡大縮小(原点を中心に3方向指定の拡大率 plotScale[x/y/z])
        /// </summary>
        /// <param name="scale">拡大率 plotScale[x/y/z]</param>
        public void scale(Point3D scale)
        {
            m3DDraw.scale(scale);
        }

        /// <summary>
        /// 座標変換をリセット
        /// </summary>
        public void convCoodinateReset()
        {
            m3DDraw.convReset();
        }

        /// <summary>
        /// 座標変換マトリックスをスタックに保存
        /// </summary>
        public void convCoordinatePush()
        {
            m3DDraw.convPush();
        }

        /// <summary>
        /// 座標変換マトリックスをスタックから取出して設定
        /// </summary>
        public void convCoordinatePop()
        {
            m3DDraw.convPop();
        }

        /// <summary>
        /// 変換マトリックスにスタックの値を加える
        /// </summary>
        public void convCoordinatePeekMulti()
        {
            m3DDraw.convPeekMulti();
        }

        /// <summary>
        /// 線分表示
        /// </summary>
        /// <param name="line"></param>
        public void plotLine(Line3D line)
        {
            m3DDraw.createLineSurfaceData(line);
        }

        /// <summary>
        /// 複数線分
        /// </summary>
        /// <param name="plist"></param>
        public void plotLines(List<Point3D> plist)
        {
            m3DDraw.createLinesSurfaceData(plist);
        }

        /// <summary>
        /// ポリライン
        /// </summary>
        /// <param name="plist"></param>
        public void plotPolyline(List<Point3D> plist)
        {
            Polyline3D polyline = new Polyline3D(plist);
            m3DDraw.createPolylineSurfaceData(polyline);
        }

        /// <summary>
        /// ポリラインのループ
        /// </summary>
        /// <param name="plist"></param>
        public void plotPolyloop(List<Point3D> plist)
        {
            Polygon3D polygon = new Polygon3D(plist);
            m3DDraw.createPolyloopSurfaceData(polygon);
        }

        /// <summary>
        /// ポリゴン(閉領域)
        /// </summary>
        /// <param name="plist"></param>
        public void plotPolygon(List<Point3D> plist)
        {
            Polygon3D polygon = new Polygon3D(plist);
            m3DDraw.createPolygonSurfaceData(polygon);
        }

        /// <summary>
        /// 複数の３角形
        /// </summary>
        /// <param name="plist"></param>
        public void plotTriangles(List<Point3D> plist)
        {
            m3DDraw.createTrianglesSurfaceData(plist);
        }

        /// <summary>
        /// 複数の４角形
        /// </summary>
        /// <param name="plist"></param>
        public void plotQuads(List<Point3D> plist)
        {
            m3DDraw.createQuadsSurfaceData(plist);
        }

        /// <summary>
        /// 連続３角形
        /// </summary>
        /// <param name="plist"></param>
        public void plotTriangleStrip(List<Point3D> plist)
        {
            m3DDraw.createTriangleStripSurfaceData(plist);
        }

        /// <summary>
        /// 連続４角形
        /// </summary>
        /// <param name="plist"></param>
        public void plotQuadStrip(List<Point3D> plist)
        {
            m3DDraw.createQuadStripSurfaceData(plist);
        }

        /// <summary>
        /// 扇形３角形(1点目が中心)
        /// </summary>
        /// <param name="plist"></param>
        public void plotTriangleFan(List<Point3D> plist)
        {
            m3DDraw.createTriangleFanSurfaceData(plist);
        }

        /// <summary>
        /// 再表示
        /// </summary>
        public void draw()
        {
            renderFrame();
        }
    }
}
