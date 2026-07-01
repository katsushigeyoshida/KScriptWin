using CoreLib;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static KScriptWin.GraphDraw;

namespace KScriptWin
{
    /// <summary>
    /// グラフデータ
    /// </summary>
    public class GraphData
    {
        public List<PointD> mData = new List<PointD>();     //  グラフデータ
        public GRAPHTYPE mGraphType = GRAPHTYPE.LINE_GRAPH; //  グラフの種別
        public bool mDataDispOnly = false;                  //  データのみ表示
        public int mViewNo = 0;                             //  表示ViewNo
        public Rect mView;                                  //  View領域(Screen)
        public Box mDataArea;                               //  データの範囲
        public Box mDataDispArea;                           //  データの表示領域(World)
        public Box mDispArea;                               //  データ+目盛タイトルなどの表示領域(World)
        public double mStepXsize = 0;                       //  X軸補助線と目盛の間隔
        public double mStepYsize = 0;                       //  Y軸補助線と目盛の間隔
        public Brush mColor = Brushes.Black;                //  折れ線の色
        public LineType mLineType = LineType.solid;         //  折れ線の線種
        public double mThickness = 1.0;                     //  折れ線の太さ
        public PointType mPointType = PointType.circle;     //  点の種類
        public double mPointSize = 1.0;                     //  点の大きさ
        public Brush mFillColor = Brushes.White;            //  塗潰しの色
        public (int count, int pos) mBar = (1, 1);          //  棒の数と位置
        public double mFontSize = 12;                       //  タイトル文字サイズ(Screen)
        public string mTitle = "";                          //  グラフタイトル                 
        public string mXTitle = "";                         //  X軸タイトル
        public string mYTitle = "";                         //  Y軸タイトル

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dataList">座標データ</param>
        public GraphData(List<PointD> dataList)
        {
            mData = dataList;
            mDataArea = new Box(getXmin(),getYmax(),getXmax(),getYmin());
            mDataDispArea = getGraphArea(mDataArea);
            mDataDispArea.normalize();
        }

        /// <summary>
        /// データ領域(ステップサイズを考慮)
        /// </summary>
        /// <returns></returns>
        public Box getGraphArea(Box dataArea)
        {
            Box dataDispArea = dataArea.toCopy();
            mStepXsize = ylib.graphStepSize(dataArea.Width, 10);
            mStepYsize = ylib.graphStepSize(dataArea.Height, 5);
            if (dataArea.Bottom < 0) {
                dataDispArea.Bottom = ((int)(dataArea.Bottom / mStepYsize) - 1) * mStepYsize;
            } else
                dataDispArea.Bottom = 0;
            dataDispArea.Top = ((int)(dataArea.Top / mStepYsize) + 1) * mStepYsize;
            if (dataArea.Left < 0) {
                dataDispArea.Left = ((int)(dataArea.Left / mStepXsize) - 1) * mStepXsize;
            } else
                dataDispArea.Left = 0;
            dataDispArea.Right = ((int)(dataArea.Right / mStepXsize) + 1) * mStepXsize;
            return dataDispArea;
        }

        /// <summary>
        /// Xデータの最小値
        /// </summary>
        /// <returns></returns>
        public double getXmin()
        {
            return mData.Min(p => p.x);
        }
        /// <summary>
        /// Xデータの最大値
        /// </summary>
        /// <returns></returns>
        public double getXmax()
        {
            return mData.Max(p => p.x);
        }
        /// <summary>
        /// Yデータの最小値
        /// </summary>
        /// <returns></returns>
        public double getYmin()
        {
            return mData.Min(p => p.y);
        }
        /// <summary>
        /// Yデータの最大値
        /// </summary>
        /// <returns></returns>
        public double getYmax()
        {
            return mData.Max(p => p.y);
        }

        /// <summary>
        /// 最も狭い間隔
        /// </summary>
        /// <returns></returns>
        public double getXMinGap()
        {
            double min = double.MaxValue;
            for (int i = 0; i < mData.Count - 1; i++) {
                double gap = Math.Abs(mData[i].x - mData[i + 1].x);
                if (min > gap)
                    min = gap;
            }
            return min;
        }

        /// <summary>
        /// 最も狭い間隔
        /// </summary>
        /// <returns></returns>
        public double getYMinGap()
        {
            double min = double.MaxValue;
            for (int i = 0; i < mData.Count - 1; i++) {
                double gap = Math.Abs(mData[i].y - mData[i + 1].y);
                if (min > gap)
                    min = gap;
            }
            return min;
        }
    }

    /// <summary>
    /// グラフの表示
    /// </summary>
    public class GraphDraw
    {
        //  グラフデータ
        public double mFontSize = 12;                   //  タイトル文字サイズ(Screen)
        public List<GraphData> mGraphData;              //  グラフデータリスト
        public enum GRAPHTYPE { SCATTER, LINE_GRAPH, BAR_GRAPH, STACKEDLINE_GRAPH, STACKEDBAR_GRAPH }       //  グラフの種類
        private string[] mGraphTypeTitle = { "散布図", "折線", "棒グラフ", "積上げ式折線", "積上棒グラフ" };//   グラフ名
        public GRAPHTYPE mGraphType = GRAPHTYPE.LINE_GRAPH;                                                 //  グラフの種別

        public int mWidthSplitNo = 1;                   //  Viewの横分割数
        public int mHeightSplitNo = 1;                  //  Viewの縦分割数
        public int mUseAreaNo = 0;                      //  表示するViewの位置
        public Box mDataDispArea;                       //  データ表示領域
        public (int count, int pos) mBar = (1, 0);      //  棒の数と位置
        public string mTitle = "Title";
        public string mXTitle = "X-Title";
        public string mYTitle = "Y-Title";

        private List<Rect> mViewList;                   //  分割Viewリスト
        private Box mDispArea = new Box();
        private double mFontWorldSize = 8;

        public Brush mBaseBackColor = Brushes.White;    //  背景色


        //  プロットデータ
        public List<Entity> mEntityList;
        public Brush mColor        = Brushes.Black;
        public LineType mLineType   = LineType.solid;
        public PointType mPointType = PointType.dot;
        public bool mFill           = false;
        public Brush mFillColor     = Brushes.Black;
        public double mThickness    = 1.0;
        public double mPointSize    = 1.0;

        private Canvas mCanvas;
        private YWorldDraw ydraw;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="canvas"></param>
        public GraphDraw(Canvas canvas)
        {
            mCanvas = canvas;
            ydraw = new YWorldDraw(canvas);
            mViewList = new List<Rect>();
            Rect view = new Rect(0, 0 ,mCanvas.ActualWidth,mCanvas.ActualHeight);
            mViewList.Add(view);

            mEntityList = new List<Entity>();
            mDispArea = new Box(0, 0, 100, 100);
            mDispArea.normalize();
            mGraphData = new List<GraphData>();
        }

        /// <summary>
        /// 描画領域の初期化
        /// </summary>
        public void drawInit(bool aspectFix = true)
        {
            ydraw.clear();
            ydraw.mFillColor = mBaseBackColor;
            ydraw.mBrush = Brushes.Black;
            //  View初期化
            ydraw.mAspectFix = aspectFix;       //  アスペクト比保持
            ydraw.setViewArea(0.0, 0.0, mCanvas.ActualWidth, mCanvas.ActualHeight);
            ydraw.setWorldWindow(mDispArea.Left, mDispArea.Bottom, mDispArea.Right, mDispArea.Top);
            ydraw.mClipping = true;             //  クリッピング可
        }

        /// <summary>
        /// ワールドウィンドウを設定
        /// </summary>
        /// <param name="left">LEFT</param>
        /// <param name="bottom">BOTTOM</param>
        /// <param name="right">RIGHT</param>
        /// <param name="top">TOP</param>
        public void setWindow(double left, double bottom, double right, double top)
        {
            mDispArea.Left   = left;
            mDispArea.Top    = bottom;
            mDispArea.Right  = right;
            mDispArea.Bottom = top;
        }

        /// <summary>
        /// Plotデータのクリア
        /// </summary>
        public void plotClear()
        {
            mEntityList.Clear();
            mColor     = Brushes.Black;
            mLineType  = LineType.solid;
            mPointType = PointType.dot;
            mThickness = 1.0;
            mPointSize = 1.0;
        }

        /// <summary>
        /// Plot図形の表示
        /// </summary>
        public void plotDraw()
        {
           for (int i = 0; i < mEntityList.Count; i++)
                mEntityList[i].draw(ydraw);
        }

        /// <summary>
        /// アスペクト比固定の設定
        /// </summary>
        /// <param name="aspect">0(非固定)/1(固定)</param>
        public void setAspectFix(bool aspect)
        {
            ydraw.mAspectFix = aspect;
        }

        /// <summary>
        /// 点種の設定(dot,cross,plus,box,circle,triangle)
        /// </summary>
        /// <param name="pointType">点種名</param>
        public void plotPointType(string pointType)
        {
            mPointType = (PointType)Enum.Parse(typeof(PointType), pointType);
        }

        /// <summary>
        /// 線種の設定(solid,dashd,center,phantom)
        /// </summary>
        /// <param name="lineType"></param>
        public void plotLineType(string lineType)
        {
            mLineType = (LineType)Enum.Parse(typeof(LineType), lineType);
        }

        /// <summary>
        /// 点の大きさの設定
        /// </summary>
        /// <param name="size">点サイズ</param>
        public void setPlotPointSize(double size)
        {
            mPointSize = size;
        }

        /// <summary>
        /// 線分の太さの設定
        /// </summary>
        /// <param name="thickness">太さ</param>
        public void setPlotLineThickness(double thickness)
        {
            mThickness = thickness;
        }

        /// <summary>
        /// 色の設定
        /// </summary>
        /// <param name="color">色名</param>
        public void plotColor(string color)
        {
            mColor = ydraw.getColor(color);
        }

        /// <summary>
        /// 塗り潰し色の設定
        /// </summary>
        /// <param name="color">色名</param>
        public void plotFillColor(string color)
        {
            mFillColor = ydraw.getColor(color);
        }

        /// <summary>
        /// 点の描画
        /// </summary>
        /// <param name="point">点座標</param>
        public void plotPoint(PointD point)
        {
            PointEntity ent = new PointEntity();
            ent.mPoint = point;
            ent.mColor = mColor;
            ent.mSize = mPointSize;
            ent.mPointType = mPointType;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="line">線分</param>
        public void plotLine(LineD line)
        {
            LineEntity ent = new LineEntity();
            ent.mLine = line;
            ent.mColor = mColor;
            ent.mThickness = mThickness;
            ent.mLineType = mLineType;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="arc">円弧</param>
        public void plotArc(ArcD arc)
        {
            ArcEntity ent = new ArcEntity();
            ent.mArc = arc;
            ent.mColor = mColor;
            ent.mThickness = mThickness;
            ent.mLineType = mLineType;
            ent.mFill = mFill;
            ent.mFillColor = mFillColor;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        /// <summary>
        /// ポリラインの描画
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        public void plotPolyline(PolylineD polyline)
        {
            PolylineEntity ent = new PolylineEntity();
            ent.mPolyline = polyline;
            ent.mColor = mColor;
            ent.mThickness = mThickness;
            ent.mLineType = mLineType;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        /// <summary>
        /// ポリゴンの描画
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        public void plotPolygon(PolygonD polygon)
        {
            PolygonEntity ent = new PolygonEntity();
            ent.mPolygon = polygon;
            ent.mColor = mColor;
            ent.mThickness = mThickness;
            ent.mLineType = mLineType;
            ent.mFill = mFill;
            ent.mFillColor = mFillColor;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列データ</param>
        public void plotText(TextD text)
        {
            TextEntity ent = new TextEntity();
            ent.mText = text;
            ent.mColor = mColor;
            ent.mThickness = mThickness;
            ent.mLineType = mLineType;
            mEntityList.Add(ent);
            ent.draw(ydraw);
        }

        //  ===   グラフ処理   =======

        /// <summary>
        /// グラフWindowの分割Viewリストの作成
        /// </summary>
        public void setSplitView()
        {
            mViewList.Clear();
            double dx = mCanvas.ActualWidth / mWidthSplitNo;
            double dy = mCanvas.ActualHeight / mHeightSplitNo;
            double x = 0, y = 0;
            for (int i = 0; i < mWidthSplitNo; i++) {
                y = 0;
                for (int j = 0; j < mHeightSplitNo; j++) {
                    Rect view = new Rect(x, y, dx, dy);
                    mViewList.Add(view);
                    y += dy;
                }
                x += dx;
            }
        }

        /// <summary>
        /// グラフデータの設定
        /// </summary>
        /// <param name="dataList">表示データ</param>
        /// <param name="dataDispOnly">補助線,目盛なし</param>
        public void setGraphData(List<PointD> dataList, bool dataDispOnly = false)
        {
            GraphData graphData = new GraphData(dataList);
            graphData.mViewNo = mUseAreaNo % mViewList.Count;
            graphData.mGraphType = mGraphType;
            graphData.mColor     = mColor;
            graphData.mLineType  = mLineType;
            graphData.mThickness = mThickness;
            graphData.mPointType = mPointType;
            graphData.mPointSize = mPointSize;
            graphData.mFontSize  = mFontSize;
            graphData.mFillColor = mFillColor;
            graphData.mBar       = mBar;
            graphData.mTitle  = mTitle;
            graphData.mXTitle = mXTitle;
            graphData.mYTitle = mYTitle;
            graphData.mDataDispOnly = dataDispOnly;
            if (mDataDispArea != null) {
                graphData.mDataDispArea = mDataDispArea.toCopy();
                mDataDispArea = null;
            }

            mGraphData.Add(graphData);
        }

        /// <summary>
        /// グラフを表示する
        /// </summary>
        public void drawGraph()
        {
            ydraw.clear();
            for (int i = 0; i < mGraphData.Count; i++)
                drawGraph(mGraphData[i]);
        }

        /// <summary>
        /// データを表示する
        /// </summary>
        /// <param name="graphData">グラフデータ</param>
        public void drawGraph(GraphData graphData)
        {
            if (!graphData.mDataDispOnly) {
                ydraw.mClipping = false;
                ydraw.mFillColor = mBaseBackColor;
                ydraw.mBrush = Brushes.Black;
                ydraw.mAspectFix = false;                       //  アスペクト比保持
                //  表示位置(View)設定
                Rect graphView = mViewList[graphData.mViewNo];
                ydraw.setViewArea(graphView.Left, graphView.Top, graphView.Right, graphView.Bottom);
                graphData.mDispArea = addAxisArea(graphData.mDataDispArea);
                ydraw.setWorldWindow(graphData.mDispArea.Left, graphData.mDispArea.Top, graphData.mDispArea.Right, graphData.mDispArea.Bottom);
                mFontWorldSize = ydraw.screen2worldYlength(graphData.mFontSize);

                //  表示枠
                ydraw.drawWRectangle(graphData.mDispArea);
                //  データ枠
                ydraw.mBrush = Brushes.Green;
                ydraw.drawWRectangle(graphData.mDataDispArea);
                //  補助線と目盛、タイトル表示
                drawAxis(graphData);
            }
            //  データ表示
            drawGraphData(graphData);
        }

        /// <summary>
        /// データの表示
        /// </summary>
        /// <param name="data"></param>
        public void drawGraphData(GraphData data)
        {
            ydraw.mClipping = true;
            ydraw.mClipBox = data.mDataDispArea;
            ydraw.mBrush = data.mColor;
            ydraw.mFillColor = data.mFillColor;
            if (data.mGraphType == GRAPHTYPE.SCATTER) {
                //  散布図
                ydraw.mPointType = (int)mPointType;
                ydraw.mPointSize = mPointSize;
                for (int i = 0; i < data.mData.Count; i++) {
                    ydraw.drawWPoint(data.mData[i]);
                }
            } else if (data.mGraphType == GRAPHTYPE.LINE_GRAPH) {
                //  折れ線
                ydraw.mLineType = (int)data.mLineType;
                ydraw.mThickness = data.mThickness;
                for (int i = 0; i < data.mData.Count - 1; i++) {
                    LineD l = new LineD(data.mData[i], data.mData[i + 1]);
                    ydraw.drawWLine(l);
                }
            } else if (data.mGraphType == GRAPHTYPE.BAR_GRAPH) {
                //  棒グラフ
                double barWidth = data.getXMinGap() * 0.8 / data.mBar.count;
                int pos = data.mBar.pos % data.mBar.count;
                for (int i = 0; i < data.mData.Count; i++) {
                    double xs = data.mData[i].x - barWidth * data.mBar.count / 2 + barWidth * pos;
                    Rect rect = new Rect(new Point(xs, 0.0), new Point(xs + barWidth, data.mData[i].y));
                    ydraw.drawWRectangle(rect);
                }
            }
        }

        /// <summary>
        /// データエリアに目盛とタイトル余白を追加
        /// </summary>
        /// <param name="dataArea"></param>
        /// <returns></returns>
        public Box addAxisArea(Box dataArea)
        {
            ydraw.setWorldWindow(dataArea.Left, dataArea.Top, dataArea.Right, dataArea.Bottom); //  仮の表示領域
            Box dispArea = dataArea.toCopy();
            dispArea.Left   -= ydraw.screen2worldXlength(mFontSize * 5);
            dispArea.Right  += ydraw.screen2worldXlength(mFontSize * 2);
            dispArea.Bottom += ydraw.screen2worldYlength(mFontSize * 4);
            dispArea.Top    -= ydraw.screen2worldYlength(mFontSize * 3);
            return dispArea;
        }

        /// <summary>
        /// 補助線と軸の標示
        /// </summary>
        public void drawAxis(GraphData graphData)
        {
            //  タイトル表示
            double cx = graphData.mDispArea.getCenter().x;
            double cy = graphData.mDispArea.getCenter().y;
            if (0 < graphData.mTitle.Length)
                ydraw.drawWText(graphData.mTitle, new PointD(cx, graphData.mDispArea.Top), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
            if (0 < graphData.mXTitle.Length)
                ydraw.drawWText(graphData.mXTitle, new PointD(cx, graphData.mDispArea.Bottom), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            if (0 < graphData.mYTitle.Length)
                ydraw.drawWText(graphData.mYTitle, new PointD(graphData.mDispArea.Left, cy), mFontWorldSize, Math.PI / 2, HorizontalAlignment.Center, VerticalAlignment.Top);

            //  X軸の補助線と目盛
            double x = graphData.mDataDispArea.Left;
            ydraw.mBrush = Brushes.Black;
            ydraw.drawWText(x.ToString(), new PointD(x, graphData.mDataDispArea.Bottom), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
            x += graphData.mStepXsize;
            while (x <= graphData.mDataDispArea.Right) {
                ydraw.mBrush = Brushes.Aqua;
                ydraw.drawWLine(new PointD(x, graphData.mDataDispArea.Bottom), new PointD(x, graphData.mDataDispArea.Top));
                ydraw.mBrush = Brushes.Black;
                ydraw.drawWText(ylib.axisScaleForm(x,4).ToString(), new PointD(x, graphData.mDataDispArea.Bottom), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
                x += graphData.mStepXsize;
            }
            //  Y軸の補助線と目盛
            double y = graphData.mDataDispArea.Bottom;
            ydraw.mBrush = Brushes.Black;
            ydraw.drawWText(y.ToString(), new PointD(graphData.mDataDispArea.Left, y), mFontWorldSize, 0, HorizontalAlignment.Right, VerticalAlignment.Center);
            y += graphData.mStepYsize;
            while (y <= graphData.mDataDispArea.Top) {
                ydraw.mBrush = Brushes.Aqua;
                ydraw.drawWLine(new PointD(graphData.mDataDispArea.Left, y), new PointD(graphData.mDataDispArea.Right, y));
                ydraw.mBrush = Brushes.Black;
                ydraw.drawWText(ylib.axisScaleForm(y,4).ToString(), new PointD(graphData.mDataDispArea.Left, y), mFontWorldSize, 0, HorizontalAlignment.Right, VerticalAlignment.Center);
                y += graphData.mStepYsize;
            }
        }
    }
}
