using CoreLib;
using System.Windows.Media;

namespace KScriptWin
{
    public enum EntityId {
        Non, Point, Line, Arc, Text,
    }
    public enum PointType { dot, cross, plus, box, circle, triangle }
    public enum LineType { solid, dash, center, phantom }

    /// <summary>
    /// グラフィック表示の要素設定
    /// </summary>
    public abstract class Entity
    {
        //  属性
        public EntityId mId = EntityId.Non;
        public Brush mColor = Brushes.Black;
        public LineType mLineType = LineType.solid;
        public PointType mPointType = PointType.dot;
        public double mThickness = 1.0;
        public double mSize = 1.0;

        private YLib ylib = new YLib();

        /// <summary>
        /// 表示
        /// </summary>
        /// <param name="ydraw"></param>
        public abstract void draw(YWorldDraw ydraw);
    }

    /// <summary>
    /// 点要素
    /// </summary>
    public class PointEntity : Entity
    {
        public PointD mPoint;
        public PointEntity()
        {
            mId = EntityId.Point;
            mPoint = new PointD();
        }

        public override void draw(YWorldDraw ydraw)
        {
            ydraw.mBrush = mColor;
            ydraw.mPointSize = mSize;
            ydraw.mPointType = (int)mPointType;
            ydraw.drawWPoint(mPoint);
        }
    }

    /// <summary>
    /// 線分要素
    /// </summary>
    public class LineEntity : Entity
    {
        public LineD mLine;
        public LineEntity()
        {
            mId = EntityId.Line;
            mLine = new LineD();
        }

        public override void draw(YWorldDraw ydraw)
        {
            ydraw.mBrush = mColor;
            ydraw.mThickness = mThickness;
            ydraw.mLineType = (int)mLineType;
            ydraw.drawWLine(mLine);
        }
    }

    /// <summary>
    /// 円弧要素
    /// </summary>
    public class ArcEntity : Entity
    {
        public ArcD mArc;
        public ArcEntity()
        {
            mId = EntityId.Arc;
            mArc = new ArcD();
        }

        public override void draw(YWorldDraw ydraw)
        {
            ydraw.mBrush = mColor;
            ydraw.mThickness = mThickness;
            ydraw.mLineType = (int)mLineType;
            ydraw.drawWArc(mArc);
        }
    }

    /// <summary>
    /// 文字列要素
    /// </summary>
    public class TextEntity : Entity
    {
        public TextD mText;
        public TextEntity()
        {
            mId = EntityId.Text;
            mText = new TextD();
        }

        public override void draw(YWorldDraw ydraw)
        {
            ydraw.mBrush = mColor;
            ydraw.mThickness = mThickness;
            ydraw.mLineType = (int)mLineType;
            ydraw.drawWText(mText);
        }
    }
}
