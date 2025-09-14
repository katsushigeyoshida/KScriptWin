using CoreLib;

namespace KScriptWin
{
    /// <summary>
    /// マトリックス関数
    ///     matrix.xxxx                                     マトリックス関数
    ///         a[,] = matrix.unit(size);                   単位行列の作成
    ///         b[,] = matrix.transpose(a[,]);              転置行列  行列Aの転置A^T
    ///         c[,] = matrix.multi(a[,], b[,]);            行列の積 AxB
    ///         c[,] = matrix.add(a[,], b[,]);              行列の和 A+B
    ///         b[,] = matrix.inverse(a[,]);                逆行列 A^-1
    ///         b[,] = matrix.copy(a[,]);                   行列のコピー
    /// </summary>
    public class FuncMatrix
    {
        public static string[] mFuncNames = new string[] {
            "matrix.unit(size); 単位行列(2次元)の作成(a[,]=...)",
            "matrix.transpose(a[,]); 転置行列(2次元行列Aの転置(A^T) b[,]=...)",
            "matrix.multi(a[,],b[,]); 行列の積 AxB (c[,]=...)",
            "matrix.add(a[,],b[,]); 行列の和 A+B c[,]=...)",
            "matrix.inverse(a[,]); 逆行列 A^-1 (b[,]=...)",
            "matrix.copy(a[,]); 行列のコピー(b[,]=...)",
            "matrix.translate(pos[,],vec[]); 2D座標の移動 translate(p[]/p[,],v[])",
            "matrix.rotate(pos[,],angle); 2D座標の回転 rotate(p[]/p[,],th)",
            "matrix.scale(pos[,],scale); 2D座標の拡大縮小 scale(p[]/p[,],scale[])",
            "matrix.translate3D(pos[,],vec[]); 3D座標の移動(3D座標,移動量)",
            "matrix.rotate3D(pos[,],angle,axis); 3D座標の回転(3D座標,回転角,回転軸(X/Y/Z)",
            "matrix.rotateAxis3D(pos[,],angle,axis[]); 3D座標を指定軸で回転(3D座標,回転角,回転軸ベクトル",
            "matrix.scale3D(pos[,],cp[],scale); 3D座標の拡大縮小(3D座標,拡大中心,拡大率)",
            //"matrix.holePlateQuads(outPolygon[,],innerPolygon[,],,); 中抜きの平面データの作成",
            //"matrix.polygonSideQuadStrip(polygon[,],thicknes); ポリゴンの側面データをQuadStripで作成",
            //"matrix.polygonSideQuads(polygon[,],thicknes); ポリゴンの側面データをQuadsで作成",
            //"matrix.polylinRotateQuads(polyline[,],centerline[,],divAng,sa,ea); ポリラインデーをセンタラインを中心に回転させたデータ",
        };

        //  共有クラス
        public KScript mScript;
        private KParse mParse;
        private Util mUtil = new Util();
        private Variable mVar;

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script"></param>
        public FuncMatrix(KScript script)
        {
            mScript = script;
            mParse = script.mParse;
            mVar = script.mVar;
        }

        /// <summary>
        /// 追加内部関数
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>戻り値</returns>
        public Token function(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "matrix.unit": return unitMatrix(args, ret);
                case "matrix.transpose": return matrixTranspose(args, ret);
                case "matrix.multi": return matrixMulti(args, ret);
                case "matrix.add": return matrixAdd(args, ret);
                case "matrix.inverse": return matrixInverse(args, ret);
                case "matrix.copy": return matrixCopy(args, ret);
                case "matrix.translate": return translate(args, ret);
                case "matrix.rotate": return rotate(args, ret);
                case "matrix.scale": return scale(args, ret);
                case "matrix.translate3D": return translate3D(args, ret);
                case "matrix.rotate3D": return rotate3D(args, ret);
                case "matrix.rotateAxis3D": return rotateAxis3D(args, ret);
                case "matrix.scale3D": return scale3D(args, ret);
                default: return new Token("not found func", TokenType.ERROR);
            }
        }


        /// <summary>
        /// 単位行列の作成(n x n)  [unit(size)]
        /// </summary>
        /// <param name="size">行列の大きさ</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token unitMatrix(List<Token> args, Token ret)
        {
            double[,] matrix = ylib.unitMatrix(ylib.intParse(args[0].mValue));

            //  戻り値の設定
            mVar.setReturnArray(matrix, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 転置行列  行列Aの転置A^T  [transpose(a[,])]
        /// </summary>
        /// <param name="args">引数(行列 A</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixTranspose(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixTranspose(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 行列の積  AxB  [multi(a[,],b[,])]
        /// 行列の積では 結合の法則  (AxB)xC = Ax(BxC) , 分配の法則 (A+B)xC = AxC+BxC , Cx(A+B) = CxA + CxB　が可
        /// 交換の法則は成立しない  AxB ≠ BxA
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixMulti(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = mVar.cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixMulti(a, b);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 行列の和 A+B  [add(a[,],b[,])]
        /// 異なるサイズの行列はゼロ行列にする
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixAdd(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            double[,]? b = mVar.cnvArrayDouble2(args[1]);
            if (b == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixAdd(a, b);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列) [inverse(a[,])]
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixInverse(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.matrixInverse(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 行列のコピー(inner function) [copy(a[,])]
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixCopy(List<Token> args, Token ret)
        {
            //  2D配列を実数配列に変換
            double[,]? a = mVar.cnvArrayDouble2(args[0]);
            if (a == null) return new Token("", TokenType.ERROR);
            //  行列演算
            double[,] c = ylib.copyMatrix(a);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 2D座標移動  translate(p[]/p[,],v[])
        /// </summary>
        /// <param name="args">引数(p[]/p[,],v[])</param>
        /// <param name="ret">戻り変数(m[,])</param>
        /// <returns>戻り変数</returns>
        private Token translate(List<Token> args, Token ret)
        {
            if (args.Count < 2) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  移動ベクトル
            if (n0 == 0 && n1 != 1) return new Token("", TokenType.ERROR);
            List<double> v = mVar.cnvListDouble(args[1]);
            double[,] transMtrix = ylib.translate2DMatrix(v[0], v[1]);
            double[,] mp = getMatrixData(args[0]);
            double[,] c = ylib.matrixMulti(mp, transMtrix);
            c = setMatrixData(c);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 2D座標回転(原点中心) rotate(p[]/p[,],th)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token rotate(List<Token> args, Token ret)
        {
            if (args.Count < 2) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  回転角
            if (n0 == 0 && n1 != 0) return new Token("", TokenType.ERROR);
            double th = ylib.doubleParse(mVar.getVariable(args[1]).mValue);
            double[,] transMtrix = ylib.rotate2DMatrix(th);
            double[,] mp = getMatrixData(args[0]);
            double[,] c = ylib.matrixMulti(mp, transMtrix);
            c = setMatrixData(c);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 2D座標の拡大縮小(原点中心) scale(p[]/p[,],scale[])
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token scale(List<Token> args, Token ret)
        {
            if (args.Count < 2) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  スケール(ベクトル)
            if (n0 == 0 && n1 != 1) return new Token("", TokenType.ERROR);
            List<double> v = mVar.cnvListDouble(args[1]);
            double[,] scaleMtrix = ylib.scale2DMatrix(v[0], v[1]);
            double[,] mp = getMatrixData(args[0]);
            double[,] c = ylib.matrixMulti(mp, scaleMtrix);
            c = setMatrixData(c);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 3D座標移動  translate3D(p[]/p[,],v[])
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token translate3D(List<Token> args, Token ret)
        {
            if (args.Count < 2) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  移動ベクトル
            if (n0 == 0 && n1 != 1) return new Token("", TokenType.ERROR);
            //  移動ベクトル
            List<double> v = mVar.cnvListDouble(args[1]);
            double[,] transMtrix = ylib.translate3DMatrix(v[0], v[1], v[2]);
            //  変換元データ
            double[,] mp = getMatrixData(args[0], 3);
            double[,] c = ylib.matrixMulti(mp, transMtrix);
            c = setMatrixData(c, 3);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 3D座標の回転(3D座標,回転角,回転軸(X/Y/Z)"
        /// rotate3D(p[],th,"X/Y/Z")/rotate3D(p[,],th,"X/Y/Z")
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token rotate3D(List<Token> args, Token ret)
        {
            if (args.Count < 3) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  p[]/p[,] 座標
            int n1 = mVar.getArrayOder(args[1]);        //  th       回転角
            int n2 = mVar.getArrayOder(args[2]);        //  X/Y/Z    回転軸
            if (n0 == 0 && n1 != 0 && n2 != 0)
                return new Token("", TokenType.ERROR);
            double[,] mp = getMatrixData(args[0]);
            double th = ylib.doubleParse(mVar.getVariable(args[1]).mValue);
            double[,] rotateMtrix;
            string axis = args[2].getValue();
            if (axis == "X")
                rotateMtrix = ylib.rotateX3DMatrix(th);
            else if (axis == "Y")
                rotateMtrix = ylib.rotateY3DMatrix(th);
            else if (axis == "Z")
                rotateMtrix = ylib.rotateZ3DMatrix(th);
            else
                return new Token("", TokenType.ERROR);
            double[,] c = ylib.matrixMulti(mp, rotateMtrix);
            c = setMatrixData(c, 3);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 3D座標を指定軸で回転(3D座標,回転角,回転軸ベクトル
        /// rotateAxis3D(p[],v[],th)/rotateAxis3D(p[,],v[],th)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token rotateAxis3D(List<Token> args, Token ret)
        {
            if (args.Count < 3) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  p[]/p[,] 座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  v[]      回転軸ベクトル
            int n2 = mVar.getArrayOder(args[2]);        //  th       回転角
            if (n0 == 0 && n1 != 0 && n2 != 0)
                return new Token("", TokenType.ERROR);
            double[,] mp = getMatrixData(args[0]);
            List<double> v = mVar.cnvListDouble(args[1]);
            double th = ylib.doubleParse(mVar.getVariable(args[2]).mValue);
            double[,] transMtrix = ylib.rotate(new Point3D(v[0], v[1], v[2]), th);
            double[,] c = ylib.matrixMulti(mp, transMtrix);
            c = setMatrixData(c, 3);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 3D座標の拡大縮小(3D座標,拡大率)
        /// scale3D(p[],scale)/scale3D(p[,],scale)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token scale3D(List<Token> args, Token ret)
        {
            if (args.Count < 2) return new Token("", TokenType.ERROR);
            int n0 = mVar.getArrayOder(args[0]);        //  座標データ
            int n1 = mVar.getArrayOder(args[1]);        //  拡大ペクトル
            if (n0 == 0 && n1 != 1) return new Token("", TokenType.ERROR);
            List<double> v = mVar.cnvListDouble(args[1]);
            double[,] transMtrix = ylib.scale3DMatrix(v[0], v[1], v[2]);
            double[,] mp = getMatrixData(args[0]);
            double[,] c = ylib.matrixMulti(mp, transMtrix);
            c = setMatrixData(c, 3);
            //  戻り値の設定
            mVar.setReturnArray(c, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 引数からの座標データを取得しマトリックス配列に変換、3列目を追加
        /// </summary>
        /// <param name="arg">座標データ</param>
        /// <param name="n">次元</param>
        /// <returns>マトリックスデータ</returns>
        private double[,] getMatrixData(Token arg, int n = 2)
        {
            double[,] coord = mVar.cnvArrayDouble2(arg);
            double[,] mp = new double[coord.GetLength(0), n + 1];
            for (int i = 0; i < coord.GetLength(0); i++) {
                for (int j = 0; j < n; j++)
                    mp[i, j] = coord[i, j];
                mp[i, n] = 1;
            }
            return mp;
        }

        /// <summary>
        /// アフィン変換したマトリックスデータの最終列を削除し座標データ配列に変換
        /// </summary>
        /// <param name="data">マトリックスデータ</param>
        /// <param name="n">次元</param>
        /// <returns>座標データ</returns>
        private double[,] setMatrixData(double[,] data, int n = 2)
        {
            double[,] mp = new double[data.GetLength(0), n];
            for (int i = 0; i < mp.GetLength(0); i++) {
                for (int j = 0; j < n; j++)
                    mp[i, j] = data[i, j];
            }
            return mp;
        }
    }
}
