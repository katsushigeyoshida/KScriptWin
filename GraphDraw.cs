using CoreLib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KScriptWin
{
    public class GraphDraw
    {
        public Brush mBaseBackColor = Brushes.White;            //  背景色
        public enum GRAPHTYPE { SCATTER, LINE_GRAPH, BAR_GRAPH, STACKEDLINE_GRAPH, STACKEDBAR_GRAPH }
        private GRAPHTYPE mGraphType = GRAPHTYPE.LINE_GRAPH;
        private string[] mGraphTypeTitle = { "散布図", "折線", "棒グラフ", "積上げ式折線", "積上棒グラフ" };
        private Box mDispArea = new Box();
        private Box mDataArea = new Box();
        private double mBottomScreenMagine = 100;
        private double mLeftScreenMagine   = 100;
        private double mStepXsize = 1;
        private double mStepYsize = 1;
        private double mFontWorldSize = 8;

        public double mFontSize = 12;
        public double[] mX;
        public double[] mY;

        public List<Entity> mEntityList;
        public Brush mColor         = Brushes.Black;
        public LineType mLineType   = LineType.solid;
        public PointType mPointType = PointType.dot;
        public double mThickness    = 1.0;
        public double mPointSize    = 1.0;

        private Canvas mCanvas;
        private YWorldDraw ydraw;
        private YLib ylib = new YLib();

        public GraphDraw(Canvas canvas)
        {
            mCanvas = canvas;
            ydraw = new YWorldDraw(canvas);
            mEntityList = new List<Entity>();
            mDispArea = new Box(0, 0, 100, 100);
            mDispArea.normalize();
        }

        /// <summary>
        /// 描画領域の初期化
        /// </summary>
        public void drawInit()
        {
            ydraw.clear();
            ydraw.mFillColor = mBaseBackColor;
            ydraw.mBrush = Brushes.Black;
            //  View初期化
            ydraw.setViewArea(0.0, 0.0, mCanvas.ActualWidth, mCanvas.ActualHeight);
            ydraw.setWorldWindow(mDispArea.Left, mDispArea.Bottom, mDispArea.Right, mDispArea.Top);
            //ydraw.mAspectFix = false;         //  アスペクト比保持
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
            mEntityList.Add(ent);
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
        }

        /// <summary>
        /// グラフ領域を設定する
        /// </summary>
        public void setGraphWindow()
        {
            //  データ領域
            mDispArea = new Box(getGraphArea());
            mDispArea.normalize();
            //  表示領域(データ領域+目盛り領域)
            mDataArea = mDispArea.toCopy();
            ydraw.setWorldWindow(mDispArea.Left, mDispArea.Top, mDispArea.Right, mDispArea.Bottom);
            mDispArea = addAxisArea(mDispArea);
            mDispArea.normalize();
            ydraw.setWorldWindow(mDispArea.Left, mDispArea.Top, mDispArea.Right, mDispArea.Bottom);
            mFontWorldSize = ydraw.screen2worldYlength(mFontSize);
        }

        /// <summary>
        /// グラフを表示する
        /// </summary>
        public void drawGraph()
        {
            ydraw.drawWRectangle(mDispArea);    //  表示枠
            ydraw.mBrush = Brushes.Green;
            ydraw.drawWRectangle(mDataArea);    //  データ枠
            drawAxis();

            ydraw.mBrush = Brushes.Blue;
            for (int i = 0; i < mX.Length; i++) {
                ArcD arc = new ArcD(new PointD(mX[i], mY[i]), ydraw.screen2worldXlength(5));
                ydraw.drawWArc(arc);
            }
        }

        /// <summary>
        /// データ領域(ステップサイズを考慮)
        /// </summary>
        /// <returns></returns>
        public Rect getGraphArea()
        {
            Rect area = new Rect();
            area.X = mX.Min();
            area.Y = mY.Min();
            area.Width = mX.Max() - area.X;
            area.Height = mY.Max() - area.Y;
            double tmpX = area.X;
            double tmpY = area.Y;
            if (0 < area.Y) {
                area.Y = 0;
                area.Height += tmpY - area.Y;
            }
            if (0 < area.X) {
                area.X = 0;
                area.Width += tmpX - area.X;
            }

            mStepXsize = ylib.graphStepSize(area.Width, 10);
            mStepYsize = ylib.graphStepSize(area.Height, 5);
            if (area.Y < 0) {
                area.Y = ((int)(area.Y / mStepYsize) - 1) * mStepYsize;
                area.Height += tmpY - area.Y;
            }
            if (area.X < 0) {
                area.X = ((int)(area.X / mStepXsize) - 1) * mStepXsize;
                area.Width += tmpX - area.X;
            }
            int n = 2;
            while (area.Height > mStepYsize * n++) ;
            area.Height = mStepYsize * n;
            n = 2;
            while (area.Width > mStepXsize * n++) ;
            area.Width = mStepXsize * n;
            return area;
        }

        public Box addAxisArea(Box dataArea)
        {
            Box dispArea = dataArea.toCopy();
            dispArea.Left -= ydraw.screen2worldXlength(50);
            dispArea.Bottom += ydraw.screen2worldYlength(50);
            return dispArea;
        }

        public void drawAxis()
        {
            double x = mDataArea.Left;
            ydraw.mBrush = Brushes.Black;
            ydraw.drawWText(x.ToString(), new PointD(x, mDataArea.Bottom), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
            x += mStepXsize;
            while (x < mDataArea.Right) {
                ydraw.mBrush = Brushes.Aqua;
                ydraw.drawWLine(new PointD(x, mDataArea.Bottom), new PointD(x, mDataArea.Top));
                ydraw.mBrush = Brushes.Black;
                ydraw.drawWText(x.ToString(), new PointD(x, mDataArea.Bottom), mFontWorldSize, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
                x += mStepXsize;
            }

            double y = mDataArea.Bottom;
            ydraw.mBrush = Brushes.Black;
            ydraw.drawWText(y.ToString(), new PointD(mDataArea.Left, y), mFontWorldSize, 0, HorizontalAlignment.Right, VerticalAlignment.Center);
            y += mStepYsize;
            while (y < mDataArea.Top) {
                ydraw.mBrush = Brushes.Aqua;
                ydraw.drawWLine(new PointD(mDataArea.Left, y), new PointD(mDataArea.Right, y));
                ydraw.mBrush = Brushes.Black;
                ydraw.drawWText(y.ToString(), new PointD(mDataArea.Left, y), mFontWorldSize, 0, HorizontalAlignment.Right, VerticalAlignment.Center);
                y += mStepYsize;
            }
        }
    }
}
