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
            "plot.Fill(1); 塗潰し設定(0:塗潰しなし/1:塗潰しあり",
            "plot.FillColor(\"Blue\"); 塗潰しの色設定",
            "plot.PointType(\"cross\"); 点種の設定(\"dot\", \"cross\", \"plus\", \"box\", \"circle\", \"triangle\")",
            "plot.LineType(\"dash\"); 線種の設定(\"solid\", \"dash\", \"center\", \"phantom\")",
            "plot.PointSize(3); 点サイズの設定",
            "plot.LineThickness(2); 線の太さの設定",
            "plot.Point(x,y); 点の表示 Point(x,y/p[]/pl[,])",
            "plot.Line(sx,sy,ex,ey); 線分の表示 Line(sx,sy,ex,ey/sp[],ep[]/pl[,]))",
            "plot.Arc(cx,cy,r[,sa][,ea]); 円弧の表示(中心x,中心y,半径[、始角][、終角])",
            "plot.Polyline(x0,y0,x1,y1,...); ポリラインの表示 Polyline(x0,y0,x1,y1,.../p0[],p1[].../pl[,]))",
            "plot.Polygon(x0,y0,x1,y1,...); ポリゴンの表示 Polygon(x0,y0,x1,y1,.../p0[],p1[].../pl[,]))",
            "plot.Text(text,x,y[,size[,rot[,ha[,va]]]]); 文字列の表示(文字列,X座標,Y座標,サイズ,回転角,水平アライメント,垂直アライメント)",
            "graph.SetData(x[],y[]); グラフデータの設定 Set(x[],y[])/set(pl[,])",
            "graph.AddData(x[],y[]); グラフデータの追加 Add(x[],y[]/p[,])",
            "graph.GraphType(\"line\"); グラフの種別設定(折れ線\"line\",散布図\"scatter\",棒グラフ\"bar\")",
            "graph.LineColor(\"Blue\"); グラフの線の色設定",
            "graph.LineType(\"dash\"); 線種の設定(\"solid\", \"dash\", \"center\", \"phantom\")",
            "graph.LineThickness(1); 線の太さ",
            "graph.PointType(\"cross\"); 点種の設定(\"dot\", \"cross\", \"plus\", \"box\", \"circle\", \"triangle\")",
            "graph.PointSize(3); 点サイズの設定",
            "graph.Title(\"title\"); グラフのタイトル",
            "graph.XTitle(\"title\"); グラフのX軸タイトル\"",
            "graph.YTitle(\"title\"); グラフのY軸タイトル",
            "graph.FontSize(5); グラフのフォントサイズの設定",
        };

        //  共有クラス
        public KScript mScript;
        public GraphView mGraph;

        private double mGraphFontSize = 12;
        private bool mAspectFix = true;
        private GraphDraw.GRAPHTYPE mGraphType = GraphDraw.GRAPHTYPE.LINE_GRAPH;
        private string mLineColor = "Black";
        private string mLineType = "solid";
        private double mLineThickness = 1;
        private string mPointType = "circle";
        private double mPointSize = 1;
        private string mTitle = "";
        private string mXTitle = "";
        private string mYTitle = "";

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
                case "plot.Fill"         : plotSetFill(args); break;
                case "plot.FillColor"    : plotFillColor(args); break;
                case "plot.PointType"    : plotPointType(args); break;
                case "plot.LineType"     : plotLineType(args); break;
                case "plot.PointSize"    : plotPointSize(args); break;
                case "plot.LineThickness": plotLineThickness(args); break;
                case "plot.Point"        : plotPoint(args); break;
                case "plot.Line"         : plotLine(args); break;
                case "plot.Arc"          : plotArc(args); break;
                case "plot.Polyline"     : plotPolyline(args); break;
                case "plot.Polygon"      : plotPolygon(args); break;
                case "plot.Text"         : plotText(args); break;
                case "graph.SetData"     : setGraphData(args); break;
                case "graph.AddData"     : addtGraphData(args); break;
                case "graph.FontSize"    : graphFontSize(args); break;
                case "graph.GraphType"   : setGraphType(args); break;
                case "graph.SetColor"    : setLineColor(args); break;
                case "graph.LineType"    : setLineType(args); break;
                case "graph.LineThickness": setLineThickness(args); break;
                case "graph.PointType"   : setPointType(args); break;
                case "graph.PointSize"   : setPointSize(args); break;
                case "graph.Title"       : setTitle(args); break;
                case "graph.XTitle"      : setXTitle(args); break;
                case "graph.YTitle"      : setYTitle(args); break;
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
        /// 要素の塗潰し色設定(inner function)
        /// </summary>
        /// <param name="args">色名</param>
        public void plotFillColor(List<Token> args)
        {
            string colorName = ylib.stripBracketString(args[0].mValue, '"');
            mGraph.setFillColor(colorName);
        }

        /// <summary>
        /// 要素の塗潰し設定
        /// </summary>
        /// <param name="args">1=塗潰す other:塗り潰しなし</param>
        public void plotSetFill(List<Token> args)
        {
            int fill = ylib.intParse(args[0].mValue);
            mGraph.setFill(fill == 1 ? true : false);
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
        /// Point(x,y)/Point(p[])/Point(p[,])
        /// </summary>
        /// <param name="args">点座標x,y</param>
        public void plotPoint(List<Token> args)
        {
            if (args.Count == 2 && mVar.getArrayOder(args[0]) == 0 && mVar.getArrayOder(args[1]) ==0) {
                //  Point(x,y)
                List<double> datas = new List<double>();
                for (int i = 0; i < args.Count; i++)
                    datas.Add(ylib.doubleParse(args[i].mValue));
                if (1 < datas.Count) {
                    PointD point = new PointD(datas[0], datas[1]);
                    mGraph.plotPoint(point);
                }
            } else if (0 < args.Count && mVar.getArrayOder(args[0]) == 1) {
                //  Point(p[])
                List<double> datas = mVar.cnvListDouble(args[0]);
                if (1 < datas.Count) {
                    PointD point = new PointD(datas[0], datas[1]);
                    mGraph.plotPoint(point);
                }
            } else if (0 < args.Count && mVar.getArrayOder(args[0]) == 2) {
                //  Point(p[,])
                double[,] datas = mVar.cnvArrayDouble2(args[0]);
                if (1 < datas.GetLength(1)) {
                    for (int i = 0; i < datas.GetLength(0); i++) {
                        PointD point = new PointD(datas[i,0], datas[i,1]);
                        mGraph.plotPoint(point);
                    }
                }
            }
        }

        /// <summary>
        /// 線分の表示(始点x,y、終点x,y)(inner function)
        /// Line(sx,sy,ex,ey)/Line(sp[],ep[])/Line(pl[,])
        /// </summary>
        /// <param name="args">始終点座標</param>
        public void plotLine(List<Token> args)
        {
            if (args.Count == 4 && mVar.getArrayOder(args[0]) == 0 && mVar.getArrayOder(args[1]) == 0) {
                //  Line((sx,sy,ex,ey)
                List<double> datas = new List<double>();
                for (int i = 0; i < args.Count; i++)
                    datas.Add(ylib.doubleParse(args[i].mValue));
                if (3 < datas.Count) {
                    LineD line = new LineD(datas[0], datas[1], datas[2], datas[3]);
                    mGraph.plotLine(line);
                }
            }else if (args.Count == 2 && mVar.getArrayOder(args[0]) == 1 && mVar.getArrayOder(args[1]) == 1) {
                //  Line(sp[],ep[])
                List<double> data0 = mVar.cnvListDouble(args[0]);
                List<double> data1 = mVar.cnvListDouble(args[1]);
                if (1 < data0.Count && 1 < data1.Count) {
                    LineD line = new LineD(data0[0], data0[1], data1[0], data1[1]);
                    mGraph.plotLine(line);
                }
            } else if (args.Count == 1 && mVar.getArrayOder(args[0]) == 2) {
                //  Line(pl[,])
                double[,] datas = mVar.cnvArrayDouble2(args[0]);
                PointD sp, ep;
                if (1 < datas.GetLength(0) && 1 < datas.GetLength(1)) {
                    sp = new PointD(datas[0, 0], datas[0, 1]);
                    for (int i = 1; i < datas.GetLength(0); i++) {
                        ep = new PointD(datas[i, 0], datas[i, 1]);
                        LineD line = new LineD(sp, ep);
                        mGraph.plotLine(line);
                        sp = ep;
                    }
                }
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
        /// ポリラインの表示
        /// </summary>
        /// <param name="args"></param>
        private void plotPolyline(List<Token> args)
        {
            List<PointD> plist = mVar.args2PointList(args);
            if (plist != null && 1 < plist.Count) {
                PolylineD polyline = new PolylineD(plist);
                mGraph.plotPolyline(polyline);
            }
        }

        /// <summary>
        /// ポリゴンの表示
        /// </summary>
        /// <param name="args"></param>
        private void plotPolygon(List<Token> args)
        {
            List<PointD> plist = mVar.args2PointList(args);
            if (plist != null && 1 < plist.Count) {
                PolygonD polygon = new PolygonD(plist);
                mGraph.plotPolygon(polygon);
            }
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
        /// set(x[],y[],title)/set(pl[,],title)
        /// </summary>
        /// <param name="args">引数(x[],y[][,title]</param>
        public void setGraphData(List<Token> args)
        {
            List<PointD> dataList = mVar.args2PointList(args);
            if (dataList.Count == 0)
                return;
            if (mGraph != null)
                mGraph.Close();
            mGraph = new GraphView();
            mScript.mGraph = mGraph;
            mGraph.Show();

            mGraph.mDraw.mTitle = mTitle;
            mGraph.mDraw.mXTitle = mXTitle;
            mGraph.mDraw.mYTitle = mYTitle;
            mGraph.mAspectFix = false;
            mGraph.setFontSize(mGraphFontSize);
            
            mGraph.mDraw.mGraphType = mGraphType;
            mGraph.setColor(mLineColor);
            mGraph.setLineType(mLineType);
            mGraph.setLineThickness(mLineThickness);
            mGraph.setPointType(mPointType);
            mGraph.setPointSize(mPointSize);

            mGraph.setGraph(dataList);
        }

        /// <summary>
        /// グラフデータを追加 addGraphData(p[,]/x[],y[])
        /// </summary>
        /// <param name="args"></param>
        private void addtGraphData(List<Token> args )
        {
            List<PointD> dataList = mVar.args2PointList(args);
            if (dataList.Count == 0)
                return;
            mGraph.mDraw.mGraphType = mGraphType;
            mGraph.setColor(mLineColor);
            mGraph.setLineType(mLineType);
            mGraph.setLineThickness(mLineThickness);
            mGraph.setPointType(mPointType);
            mGraph.setPointSize(mPointSize);

            mGraph.addGraph(dataList); 
        }

        /// <summary>
        /// グラフの種類を設定 (setGraphType = scatter(散布図) / line(折れ線) / bar(棒グラフ) )
        /// </summary>
        /// <param name="args"></param>
        public void setGraphType(List<Token> args) {
            string type = mVar.getStringFromArg(args[0]);
            setGraphType(type);
        }

        /// <summary>
        /// グラフの種別設定
        /// </summary>
        /// <param name="type"></param>
        private void setGraphType(string type)
        {
            switch (type.ToLower()) {
                case "scatter": mGraphType = GraphDraw.GRAPHTYPE.SCATTER; break;
                case "line": mGraphType = GraphDraw.GRAPHTYPE.LINE_GRAPH; break;
                case "bar": mGraphType = GraphDraw.GRAPHTYPE.BAR_GRAPH; break;
            }
        }

        /// <summary>
        /// グラフの線の色設定
        /// </summary>
        /// <param name="args"></param>
        public void setLineColor(List<Token> args)
        {
            mLineColor = mVar.getStringFromArg(args[0]);
        }

        /// <summary>
        /// グラフの線種設定
        /// </summary>
        /// <param name="args"></param>
        public void setLineType(List<Token> args)
        {
            mLineType = ylib.stripBracketString(args[0].mValue, '"');
        }

        /// <summary>
        /// グラフの線の太さを設定
        /// </summary>
        /// <param name="args"></param>
        public void setLineThickness(List<Token> args)
        {
            mLineThickness = ylib.doubleParse(args[0].mValue);
        }


        /// <summary>
        /// 点種の設定("dot", "cross", "plus", "box", "circle", "triangle")
        /// </summary>
        /// <param name="args">点種</param>
        public void setPointType(List<Token> args)
        {
            mPointType = ylib.stripBracketString(args[0].mValue, '"');
        }

        /// <summary>
        /// 点サイズの設定
        /// </summary>
        /// <param name="args">サイズ</param>
        public void setPointSize(List<Token> args)
        {
            mPointSize = ylib.doubleParse(args[0].mValue);
        }

        /// <summary>
        /// タイトル設定
        /// </summary>
        /// <param name="args"></param>
        public void setTitle(List<Token> args) {
            mTitle = mVar.getStringFromArg(args[0]);
        }

        /// <summary>
        /// X軸タイトル設定
        /// </summary>
        /// <param name="args"></param>
        public void setXTitle(List<Token> args) {
            mXTitle = mVar.getStringFromArg(args[0]);
        }

        /// <summary>
        /// Y軸タイトル設定
        /// </summary>
        /// <param name="args"></param>
        public void setYTitle(List<Token> args) {
            mYTitle = mVar.getStringFromArg(args[0]);
        }
    }
}
