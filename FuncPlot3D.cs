﻿using CoreLib;

namespace KScriptWin
{
    public class FuncPlot3D
    {
        public static string[] mFuncNames = new string[] {
            "plot3D.setArea(min[],max[]); 3DViewの初期化と領域設定",
            "plot3D.setAxisFrame(axis,frame); 軸とフレームの表示(0:非表示/1:表示)",
            "plot3D.disp(); 要素の表示",
            "plot3D.setColor(\"Blue\"); 色の設定",
            "plot3D.plotTranslate(vec[]); 移動(移動量 vec[x/y/z])",
            "plot3D.plotRotate(angle,\"X\"); 回転(回転角度,回転軸 X/Y/Z軸)",
            "plot3D.plotScale(scale[]); 縮小拡大 (拡大率 scale[x/y/z])",
            "plot3D.plotReset(); 座標変換マトリックスをクリア",
            "plot3D.plotPush(); 座標変換マトリックスをスタックに保存",
            "plot3D.plotPop(); 座標変換マトリックスをスタックから戻す",
            "plot3D.plotPeekMulti(); 座標変換マトリックスにスタックの値をかける",
            "plot3D.plotLine(sp[x/y/z],ep[x/y/z]); 線分の設定",
            "plot3D.plotLines(plist[n,x/y/z]); 複数線分(plist :座標リスト)",
            "plot3D.plotPolyline(plist[n,x/y/z]); ポリライン",
            "plot3D.plotPolyloop(plist[n,x/y/z]); ポリラインループ",
            "plot3D.plotPolygon(plist[n,x/y/z]); ポリゴン(塗潰し)",
            "plot3D.plotTrianges(plist[n,x/y/z]); 複数三角形(塗潰し)",
            "plot3D.plotTriangeStrip(plist[n,x/y/z]); 連続三角形",
            "plot3D.plotQuads(plist[n,x/y/z]); 複数四角形",
            "plot3D.plotQuadeStrip(plist[n,x/y/z]); 連続四角形",
            "plot3D.plotTriangeFan(plist[n,x/y/z]); 扇形の連続三角形",
            "plot3D.translate(pos[],vec[]); 3D座標の移動(3D座標,移動量)",
            "plot3D.rotate(pos[],angle,axis); 3D座標の回転(3D座標,回転角,回転軸(X/Y/Z)",
            "plot3D.scale(pos[],cp[],scale); 3D座標の拡大縮小(3D座標,拡大中心,拡大率)",
        };

        //  共有クラス
        public KScript mScript;
        public Plot3DView mPlot3D;

        private KParse mParse;
        private YLib ylib = new YLib();

        public FuncPlot3D(KScript script)
        {
            mScript = script;
            mParse  = script.mParse;
            mPlot3D = script.mPlot3D;
        }

        /// <summary>
        /// 関数の選択
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">戻り値の変数名</param>
        /// <returns>戻り値</returns>
        public Token plotFunc(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "plot3D.setArea"         : setArea(args); break;
                case "plot3D.setAxisFrame"    : setAxisFrameDisp(args); break;
                case "plot3D.disp"            : disp(); break;
                case "plot3D.setColor"        : setColor(args); break;
                case "plot3D.plotTranslate"   : plotTranslate(args); break;
                case "plot3D.plotRotate"      : plotRotate(args); break;
                case "plot3D.plotScale"       : plotScale(args); break;
                case "plot3D.plotReset"       : plotReset(); break;
                case "plot3D.plotPush"        : plotConvPush(); break;
                case "plot3D.plotPop"         : plotConvPop(); break;
                case "plot3D.plotPeekMulti"   : plotConvMulti(); break;
                case "plot3D.plotLine"        : plotLine(args); break;
                case "plot3D.plotLines"       : plotLines(args); break;
                case "plot3D.plotPolyline"    : plotPolyline(args); break;
                case "plot3D.plotPolyloop"    : plotPolyloop(args); break;
                case "plot3D.plotPolygon"     : plotPolygon(args); break;
                case "plot3D.plotTrianges"    : plotTriangles(args); break;
                case "plot3D.plotTriangeStrip": plotTriangleStrip(args); break;
                case "plot3D.plotQuads"       : plotQuads(args); break;
                case "plot3D.plotQuadeStrip"  : plotQuadStrip(args); break;
                case "plot3D.plotTriangeFan"  : plotTriangleFan(args); break;
                case "plot3D.translate"       : translate(args, ret); break;
                case "plot3D.rotate"          : rotate(args, ret); break;
                case "plot3D.scale"           : scale(args, ret); break;
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 領域設定と初期化
        /// </summary>
        /// <param name="args">min[],max[]</param>
        private void setArea(List<Token> args)
        {
            if (args.Count < 2) return ;
            List<double> listData0 = mParse.cnvListDouble(args[0]);
            List<double> listData1 = mParse.cnvListDouble(args[1]);
            Point3D sp = new Point3D(listData0[0], listData0[1], listData0[2]);
            Point3D ep = new Point3D(listData1[0], listData1[1], listData1[2]);
            if (mPlot3D != null)
                mPlot3D.Close();
            mPlot3D = new Plot3DView();
            mScript.mPlot3D = mPlot3D;
            mPlot3D.setArea(sp, ep);
            mPlot3D.Show();
        }

        /// <summary>
        /// 軸とフレームの表示/非表示(0:非表示/1:表示)
        /// </summary>
        /// <param name="args">axis,frame</param>
        private void setAxisFrameDisp(List<Token> args)
        {
            bool axisDisp = false;
            bool frameDisp = false;
            if (0 < args.Count)
                axisDisp = ylib.intParse(args[0].mValue) == 0 ? false : true;
            if (1 < args.Count)
                frameDisp = ylib.intParse(args[1].mValue) == 0 ? false : true;
            mPlot3D.setAxisFrameDisp(axisDisp, frameDisp);
        }

        /// <summary>
        /// 登録要素の表示
        /// </summary>
        private void disp()
        {
            mPlot3D.renderFrame();
        }

        /// <summary>
        /// 要素の色設定(文字列で指定)
        /// </summary>
        /// <param name="args">"color"</param>
        private void setColor(List<Token> args)
        {
            string colorName = ylib.stripBracketString(args[0].mValue, '"');
            mPlot3D.setColor(colorName);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="args">移動量(vec[x/y/z])</param>
        private void plotTranslate(List<Token> args)
        {
            List<double> vec = mParse.cnvListDouble(args[0]);
            if (vec == null || vec.Count < 3) return;
            mPlot3D.translate(new Point3D(vec[0], vec[1], vec[2]));
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="args">回転角度,回転軸("X"/"Y"/"Z")</param>
        private void plotRotate(List<Token> args)
        {
            if (2 <= args.Count) {
                double angle = ylib.doubleParse(args[0].mValue);
                string coord = args[1].getValue();
                mPlot3D.rotate(angle, coord);
            }
        }

        /// <summary>
        /// 拡大縮小 (3方向の拡大率)
        /// </summary>
        /// <param name="args">plotScale[x/y/z]</param>
        private void plotScale(List<Token> args)
        {
            List<double> scale = mParse.cnvListDouble(args[0]);
            if (scale == null || scale.Count < 3) return;
            mPlot3D.scale(new Point3D(scale[0], scale[1], scale[2]));
        }

        /// <summary>
        /// 座標変換をリセット
        /// </summary>
        private void plotReset()
        {
            mPlot3D.convCoodinateReset();
        }

        /// <summary>
        /// 座標変換マトリックスをスタックに保存
        /// </summary>
        private void plotConvPush()
        {
            mPlot3D.convCoordinatePush();
        }

        /// <summary>
        /// 座標変換マトリックスをスタックからとりだす
        /// </summary>
        private void plotConvPop()
        {
            mPlot3D.convCoordinatePop();
        }

        private void plotConvMulti()
        {
            mPlot3D.convCoordinatePeekMulti();
        }

        /// <summary>
        /// 線分の表示
        /// </summary>
        /// <param name="args">sp[x/y/z],ep[x/y/z]</param>
        private void plotLine(List<Token> args)
        {
            List<double> sp = mParse.cnvListDouble(args[0]);
            if (sp == null || sp.Count < 3) return;
            List<double> ep = mParse.cnvListDouble(args[1]);
            if (ep == null || ep.Count < 3) return;
            Line3D line = new Line3D(new Point3D(sp[0], sp[1], sp[2]), new Point3D(ep[0], ep[1], ep[2]));
            mPlot3D.plotLine(line);
        }

        /// <summary>
        /// 複数線分
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotLines(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotLines(points);
        }

        /// <summary>
        /// ポリライン
        /// </summary>
        /// <param name="args">poly[n,x/y/z]</param>
        private void plotPolyline(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotPolyline(points);
        }

        /// <summary>
        /// ポリラインループ(塗潰しなし)
        /// </summary>
        /// <param name="args">poly[n,x/y/z]</param>
        private void plotPolyloop(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotPolyloop(points);
        }

        /// <summary>
        /// ポリゴン(塗潰しあり)
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotPolygon(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotPolygon(points);
        }

        /// <summary>
        /// 複数三角形
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotTriangles(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotTriangles(points);
        }

        /// <summary>
        /// 複数四角形
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotQuads(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotQuads(points);
        }

        /// <summary>
        /// 連続三角形
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotTriangleStrip(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotTriangleStrip(points);
        }

        /// <summary>
        /// 連続四角形
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotQuadStrip(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotQuadStrip(points);
        }

        /// <summary>
        /// 扇形連続三角形
        /// </summary>
        /// <param name="args">poslist[n,x/y/z]</param>
        private void plotTriangleFan(List<Token> args)
        {
            double[,]? plist = mParse.cnvArrayDouble2(args[0]);
            if (plist == null) return;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < plist.GetLength(0); i++) {
                points.Add(new Point3D(plist[i, 0], plist[i, 1], plist[i, 2]));
            }
            mPlot3D.plotTriangleFan(points);
        }

        /// <summary>
        /// 3Dデータの移動
        /// </summary>
        /// <param name="args">pos[x/y/z]|pos[n,x/y/z],v[x,y,z]</param>
        /// <param name="ret">pos[x/y/z]|pos[n,x/y/z]</param>
        /// <returns></returns>
        private Token translate(List<Token> args, Token ret)
        {
            (string arrayName, int no) = mParse.getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            double[] vec = mParse.cnvListDouble(args[1]).ToArray();
            if (no == 1) {
                double[] src = mParse.cnvListDouble(args[0]).ToArray();
                Point3D pos = new Point3D(src[0], src[1], src[2]);
                pos.translate(vec[0], vec[1], vec[2]);
                double[] dest = new double[] { pos.x, pos.y, pos.z};
                mParse.setReturnArray(dest, ret);
            } else if (no == 2) {
                double[,] src = mParse.cnvArrayDouble2(args[0]);
                double[,] dest = new double[src.GetLength(0), 3];
                for (int i = 0; i < src.GetLength(0); i++) {
                    Point3D pos = new Point3D(src[i,0], src[i,1], src[i,2]);
                    pos.translate(vec[0], vec[1], vec[2]);
                    dest[i, 0] = pos.x;
                    dest[i, 1] = pos.y;
                    dest[i, 2] = pos.z;
                }
                mParse.setReturnArray(dest, ret);
            }
            //  戻り値の設定
            mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mParse.getVariable("return");
        }

        /// <summary>
        /// 3Dデータの回転
        /// </summary>
        /// <param name="args">pos[x/y/z]|pos[n,x/y/z],ang,axis("X"/"Y"/"Z")</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token rotate(List<Token> args, Token ret)
        {
            (string arrayName, int no) = mParse.getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            double ang = ylib.doubleParse(args[1].mValue);
            string coord = args[2].getValue();
            FACE3D face = coord == "X" ? FACE3D.YZ : coord == "Y" ? FACE3D.ZX : coord == "Z" ? FACE3D.XY: FACE3D.NON;
            if (no == 1) {
                double[] src = mParse.cnvListDouble(args[0]).ToArray();
                Point3D pos = new Point3D(src[0], src[1], src[2]);
                pos.rotate(ang, face);
                double[] dest = new double[] { pos.x, pos.y, pos.z };
                mParse.setReturnArray(dest, ret);
            } else if (no == 2) {
                double[,] src = mParse.cnvArrayDouble2(args[0]);
                double[,] dest = new double[src.GetLength(0), 3];
                for (int i = 0; i < src.GetLength(0); i++) {
                    Point3D pos = new Point3D(src[i, 0], src[i, 1], src[i, 2]);
                    pos.rotate(ang, face);
                    dest[i, 0] = pos.x;
                    dest[i, 1] = pos.y;
                    dest[i, 2] = pos.z;
                }
                mParse.setReturnArray(dest, ret);
            }
            //  戻り値の設定
            mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mParse.getVariable("return");
        }

        /// <summary>
        /// 拡大縮小
        /// </summary>
        /// <param name="args">pos[x/y/z]|pos[n,x/y/z],cp[x/y/z],scale</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token scale(List<Token> args, Token ret)
        {
            (string arrayName, int no) = mParse.getArrayName(new Token(args[0].mValue, TokenType.VARIABLE));
            double[] c = mParse.cnvListDouble(args[1]).ToArray();
            Point3D cp = new Point3D(c[0], c[1], c[2]);
            double scale = ylib.doubleParse(args[2].mValue);
            if (no == 1) {
                double[] src = mParse.cnvListDouble(args[0]).ToArray();
                Point3D pos = new Point3D(src[0], src[1], src[2]);
                pos.scale(cp, scale);
                double[] dest = new double[] { pos.x, pos.y, pos.z };
                mParse.setReturnArray(dest, ret);
            } else if (no == 2) {
                double[,] src = mParse.cnvArrayDouble2(args[0]);
                double[,] dest = new double[src.GetLength(0), 3];
                for (int i = 0; i < src.GetLength(0); i++) {
                    Point3D pos = new Point3D(src[i, 0], src[i, 1], src[i, 2]);
                    pos.scale(cp, scale);
                    dest[i, 0] = pos.x;
                    dest[i, 1] = pos.y;
                    dest[i, 2] = pos.z;
                }
                mParse.setReturnArray(dest, ret);
            }
            //  戻り値の設定
            mParse.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mParse.getVariable("return");
        }
    }
}