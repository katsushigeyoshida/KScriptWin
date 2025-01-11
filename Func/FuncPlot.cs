using CoreLib;
using System.Windows;

namespace KScriptWin
{
    /// <summary>
    /// 2Dグラフィック表示の関数処理
    /// </summary>
    public class FuncPlot
    {
        public static string[] mFuncNames = new string[] {
            "plot.Window(left,bottom,right,top); 表示領域の設定",
            "plot.Disp(); グラフィックデータの再表示",
            "plot.Aspect(1); アスペクト比固定の設定(0(非固定)/1(固定))",
            "plot.Color(\"Blue\"); 要素の色設定",
            "plot.PointType(\"cross\"); 点種の設定(\"dot\", \"cross\", \"plus\", \"box\", \"circle\", \"triangle\")",
            "plot.LineType(\"dash\"); 線種の設定(\"solid\", \"dash\", \"center\", \"phantom\")",
            "plot.PointSize(3); 点サイズの設定",
            "plot.LineThickness(2); 線の太さの設定",
            "plot.Point(x,y); 点の表示",
            "plot.Line(sx,sy,ex,ey); 線分の表示(始点x,y、終点x,y)",
            "plot.Arc(cx,cy,r[,sa][,ea]); 円弧の表示(中心x,中心y,半径[、始角][、終角])",
            "plot.Text(text,x,y[,size[,rot[,ha[,va]]]]); 文字列の表示(文字列,X座標,Y座標,サイズ,回転角,水平アライメント,垂直アライメント)",
            "graph.Set(x[],y[][,Title]); グラフデータの設定(X[],Y[][,Title])",
            "graph.FontSize(5); グラフのフォントサイズの設定",
        };

        //  共有クラス
        public KScript mScript;
        public GraphView mGraph;

        private double mGraphFontSize = 12;
        private bool mAspectFix = true;

        private KParse mParse;
        private Variable mVar;
        private KLexer mLexer = new KLexer();
        private YLib ylib = new YLib();

        public FuncPlot(KScript script)
        {
            mScript = script;
            mParse = script.mParse;
            mGraph = script.mGraph;
            mVar = script.mVar;
        }

        public Token plotFunc(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "plot.Window"       : plotWindow(args); break;
                case "plot.Disp"         : plotDisp(); break;
                case "plot.Aspect"       : plotAspect(args); break;
                case "plot.Color"        : plotColor(args); break;
                case "plot.PointType"    : plotPointType(args); break;
                case "plot.LineType"     : plotLineType(args); break;
                case "plot.PointSize"    : plotPointSize(args); break;
                case "plot.LineThickness": plotLineThickness(args); break;
                case "plot.Point"        : plotPoint(args); break;
                case "plot.Line"         : plotLine(args); break;
                case "plot.Arc"          : plotArc(args); break;
                case "plot.Text"         : plotText(args); break;
                case "graph.Set"         : graphSet(args); break;
                case "graph.FontSize"    : graphFontSize(args); break;
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 表示領域の設定(left,bottom,right,top)(inner function)
        /// </summary>
        /// <param name="args">left,bottom,right,top</param>
        public void plotWindow(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (mGraph != null)
                mGraph.Close();
            mGraph = new GraphView();
            mGraph.mAspectFix = mAspectFix;
            mScript.mGraph = mGraph;
            mGraph.Show();
            if (3 < datas.Count)
                mGraph.setPlotWindow(datas[0], datas[1], datas[2], datas[3]);
        }

        /// <summary>
        /// 登録したデータを表示する
        /// </summary>
        public void plotDisp()
        {
            mGraph.plotDraw();
        }

        /// <summary>
        /// アスペクト比固定の設定(inner function)
        /// </summary>
        /// <param name="args">0(非固定)/1(固定)</param>
        public void plotAspect(List<Token> args)
        {
            int aspect = ylib.intParse(args[0].mValue);
            mAspectFix = aspect == 1 ? true : false;
            //mGraph.setAspectFix(aspect);
        }

        /// <summary>
        /// 要素の色設定(inner function)
        /// </summary>
        /// <param name="args">色名</param>
        public void plotColor(List<Token> args)
        {
            string colorName = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setColor(colorName);
        }

        /// <summary>
        /// 点種の設定("dot", "cross", "plus", "box", "circle", "triangle")(inner function)
        /// </summary>
        /// <param name="args">点種</param>
        public void plotPointType(List<Token> args)
        {
            string pointType = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setPointType(pointType);
        }

        /// <summary>
        /// 線種の設定("solid", "dash", "center", "phantom")(inner function)
        /// </summary>
        /// <param name="args">線種</param>
        public void plotLineType(List<Token> args)
        {
            string lineType = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setLineType(lineType);
        }

        /// <summary>
        /// 点サイズの設定(inner function)
        /// </summary>
        /// <param name="args">サイズ</param>
        public void plotPointSize(List<Token> args)
        {
            double pointSize = ylib.doubleParse(args[0].mValue);
            mGraph.setPointSize(pointSize);
        }

        /// <summary>
        /// 線の太さの設定(inner function)
        /// </summary>
        /// <param name="arg">線の太さ</param>
        public void plotLineThickness(List<Token> args)
        {
            double lineThickness = ylib.doubleParse(args[0].mValue);
            mGraph.setLineThickness(lineThickness);
        }

        /// <summary>
        /// 点の表示(inner function)
        /// </summary>
        /// <param name="args">点座標x,y</param>
        public void plotPoint(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (1 < datas.Count) {
                PointD point = new PointD(datas[0], datas[1]);
                mGraph.plotPoint(point);
            }
        }

        /// <summary>
        /// 線分の表示(始点x,y、終点x,y)(inner function)
        /// </summary>
        /// <param name="args">始終点座標</param>
        public void plotLine(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            if (3 < datas.Count) {
                LineD line = new LineD(datas[0], datas[1], datas[2], datas[3]);
                mGraph.plotLine(line);
            }
        }

        /// <summary>
        /// 円弧の表示(中心x,y,半径、始角、終角)(inner function)
        /// </summary>
        /// <param name="args">円弧データ</param>
        public void plotArc(List<Token> args)
        {
            List<double> datas = new List<double>();
            for (int i = 0; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            ArcD arc;
            if (datas.Count == 3) {
                arc = new ArcD(datas[0], datas[1], datas[2]);
            } else if (datas.Count == 4) {
                arc = new ArcD(datas[0], datas[1], datas[2], datas[3]);
            } else if (datas.Count == 5) {
                arc = new ArcD(datas[0], datas[1], datas[2], datas[3], datas[4]);
            } else
                return;
            mGraph.plotArc(arc);
        }

        /// <summary>
        /// 文字列の表示(文字列,座標x,座標y,サイズ,回転角,水平アライメント,垂直アライメント)(inner function)
        /// </summary>
        /// <param name="args"></param>
        public void plotText(List<Token> args)
        {
            List<double> datas = new List<double>();
            string str = mLexer.stripBracketString(args[0].mValue, '"');
            for (int i = 1; i < args.Count; i++)
                datas.Add(ylib.doubleParse(args[i].mValue));
            TextD text;
            if (datas.Count == 2) {
                //  文字列と座標
                text = new TextD(str, new PointD(datas[0], datas[1]));
            } else if (datas.Count == 3) {
                //  + サイズ
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2]);
            } else if (datas.Count == 4) {
                //  + 回転角
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3]);
            } else if (datas.Count == 5) {
                //  + 水平アライメント
                HorizontalAlignment ha = datas[4] == 1 ? HorizontalAlignment.Center : datas[4] == 2 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3], ha);
            } else if (datas.Count == 6) {
                //  + 水平アライメント + 垂直アライメント
                HorizontalAlignment ha = datas[4] == 1 ? HorizontalAlignment.Center : datas[4] == 2 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                VerticalAlignment va = datas[5] == 1 ? VerticalAlignment.Center : datas[5] == 2 ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                text = new TextD(str, new PointD(datas[0], datas[1]), datas[2], datas[3], ha, va);
            } else
                return;
            mGraph.plotText(text);
        }

        /// <summary>
        /// グラフのフォントサイズの設定(inner function)
        /// </summary>
        /// <param name="arg">サイズ</param>
        public void graphFontSize(List<Token> args)
        {
            if (args == null || args.Count == 0) return;
            double size = ylib.doubleParse(mScript.getValueToken(args[0].mValue).mValue);
            mGraphFontSize = size;
        }

        /// <summary>
        /// グラフデータの設定(X[], Y[][, Title])(inner function)
        /// </summary>
        /// <param name="args">引数(x[],y[][,title]</param>
        public void graphSet(List<Token> args)
        {
            if (args.Count < 2) return;
            List<double> x = mVar.cnvListDouble(args[0]);
            List<double> y = mVar.cnvListDouble(args[1]);
            if (x.Count != y.Count)
                return;
            string title = "";
            if (2 < args.Count)
                title = mScript.getValueToken(args[2].mValue).mValue.Trim('"');

            if (mGraph != null)
                mGraph.Close();
            mGraph = new GraphView();
            mScript.mGraph = mGraph;
            mGraph.Show();
            mGraph.setAspectFix(0);
            mGraph.setFontSize(mGraphFontSize);
            mGraph.setGraph(x.ToArray(), y.ToArray(), title);
        }
    }
}
