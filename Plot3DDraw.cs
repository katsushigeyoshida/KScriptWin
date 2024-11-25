using CoreLib;
using System.Windows.Media;

namespace KScriptWin
{
    public class SurfaceData
    {
        public List<Point3D> mVertexList;           //  座標点リスト
        public DRAWTYPE mDrawType;                  //  描画方式
        public Brush mFaceColor = Brushes.Blue;
    }


    public class Plot3DDraw
    {
        public Box3D mArea;                                 //  初期表示領域
        public List<SurfaceData> mSurfaceDataList;          //  サーフェスデータリスト
        public Brush mFaceColor = Brushes.Black;            //  サーフェース色

        public double[,] mMatrix;                           //  座標変換マトリックス
        public Stack<double[,]> mMatrixStack;               //  変換マトリックスのスタック

        private YLib ylib = new YLib();


        public Plot3DDraw()
        {
            mSurfaceDataList = new List<SurfaceData>();
            mArea = new Box3D(1.0);
            mMatrix = ylib.unitMatrix(4);
            mMatrixStack = new Stack<double[,]>();
        }

        /// <summary>
        /// アフィン変換を反映させる
        /// </summary>
        /// <param name="plist"></param>
        private void convCoodinate(List<Point3D> plist)
        {
            for (int i = 0; i < plist.Count; i++)
                plist[i].matrix(mMatrix);
        }

        /// <summary>
        /// 変換パラメータをリセットする
        /// </summary>
        public void convReset()
        {
            mMatrix = ylib.unitMatrix(4);
        }

        /// <summary>
        /// 変換マトリックスをスタック
        /// </summary>
        public void convPush()
        {
            mMatrixStack.Push(mMatrix);
        }

        /// <summary>
        /// 変換マトリックスをスタックから取出す
        /// </summary>
        public void convPop()
        {
            mMatrix = mMatrixStack.Pop();
        }

        /// <summary>
        /// 変換マトリックスにスタックの値を加える
        /// </summary>
        public void convPeekMulti()
        {
            mMatrix = ylib.matrixMulti(mMatrix, mMatrixStack.Peek());

        }

        /// <summary>
        /// 移動のアフィン変換
        /// </summary>
        /// <param name="v">移動ベクタ</param>
        public void translate(Point3D v)
        {
            mMatrix = ylib.matrixMulti(mMatrix, ylib.translate3DMatrix(v.x, v.y, v.z));
        }

        /// <summary>
        /// 回転のアフィン変換(X/Y/Z軸で回転)
        /// </summary>
        /// <param name="ang">回転角</param>
        /// <param name="face">回転面</param>
        public void rotate(double ang, FACE3D face)
        {
            if (face == FACE3D.XY)
                mMatrix = ylib.matrixMulti(mMatrix, ylib.rotateZ3DMatrix(ang));
            else if (face == FACE3D.YZ)
                mMatrix = ylib.matrixMulti(mMatrix, ylib.rotateX3DMatrix(ang));
            else if (face == FACE3D.ZX)
                mMatrix = ylib.matrixMulti(mMatrix, ylib.rotateY3DMatrix(ang));
        }

        /// <summary>
        /// スケールのアフィン変換(原点を中心に拡大縮小)
        /// </summary>
        /// <param name="v"></param>
        public void scale(Point3D v)
        {
            mMatrix = ylib.matrixMulti(mMatrix, ylib.scale3DMatrix(v.x, v.y, v.z));
        }

        /// <summary>
        /// カラー設定
        /// </summary>
        /// <param name="color">色名</param>
        public void setColor(string color)
        {
            mFaceColor = ylib.getColor(color);
        }

        /// <summary>
        /// 線分
        /// </summary>
        /// <param name="line"></param>
        public void createLineSurfaceData(Line3D line)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = line.toPoint3D();
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.LINE_STRIP;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 複数線分
        /// </summary>
        /// <param name="lines"></param>
        public void createLinesSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.LINES;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// ポリライン
        /// </summary>
        /// <param name="polyline"></param>
        public void createPolylineSurfaceData(Polyline3D polyline)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = polyline.toPoint3D();
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.LINE_STRIP;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// ポリゴン(ループのみで塗潰しなし)
        /// </summary>
        /// <param name="polygon"></param>
        public void createPolyloopSurfaceData(Polygon3D polygon)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = polygon.toPoint3D();
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.LINE_LOOP;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// ポリゴン(多角形塗潰しあり)
        /// </summary>
        /// <param name="polygon"></param>
        public void createPolygonSurfaceData(Polygon3D polygon)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = polygon.toPoint3D();
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.POLYGON;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 複数3角形(座標は3の倍数)
        /// </summary>
        /// <param name="plist"></param>
        public void createTrianglesSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.TRIANGLES;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 複数4角形(座標は4の倍数)
        /// </summary>
        /// <param name="plist"></param>
        public void createQuadsSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.QUADS;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 連続3角形(座標数は3以上)
        /// </summary>
        /// <param name="plist"></param>
        public void createTriangleStripSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.TRIANGLE_STRIP;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 連続四角形(座標数は4以上で２の倍数)
        /// </summary>
        /// <param name="plist"></param>
        public void createQuadStripSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.QUAD_STRIP;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }

        /// <summary>
        /// 扇型3角形(座標数は3以上)
        /// </summary>
        /// <param name="plist"></param>
        public void createTriangleFanSurfaceData(List<Point3D> plist)
        {
            SurfaceData surfaceData = new SurfaceData();
            surfaceData.mVertexList = plist;
            convCoodinate(surfaceData.mVertexList);
            surfaceData.mDrawType = DRAWTYPE.TRIANGLE_FAN;
            surfaceData.mFaceColor = mFaceColor;
            mSurfaceDataList.Add(surfaceData);
        }
    }
}
