using CoreLib;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace KScriptWin
{
    public class FuncMath
    {
        public static string[] mFuncNames = new string[] {
            "math.max(a[]); 配列の最大値",
            "math.min(a[,]); 配列の最小値",
            "math.sum(a[]);  配列の合計",
            "math.average(a[,]); 配列の平均",
            "math.variance(a[]); 配列の分散",
            "math.stdDeviation(a[]); 配列の標準偏差",
            "math.covariance(a[], b[]); 共分散",
            "math.corrCoeff(x[],y[]); 配列の相関係数",
            "math.regression(x[],y[]) 回帰分析の係数取得r[]=array.regression(x[],y[])",
            "math.quadraticEquation(a,b,c); 2次方程式の解(y = a*x^2+b*x+c)(y[] = solv..) ",
            "math.qubicEquation(a,b,c,d); 3次方程式の解(y = a*x^3+b*x^2+c*x+d)(y[] = solv..) ",
            "math.quarticEquation(a,b,c,d,e); 4次方程式の解(y = a*x^4+b*x^3+c*x^2+d*x+e)(y[] = solv..) ",
            "math.fourier(x[]); フーリエ変換(mag[] = math.fourier(x[]))",
        };

        //  共有クラス
        public KScript mScript;
        private KParse mParse;
        private Util mUtil = new Util();
        private Variable mVar;

        private YLib ylib = new YLib();

        public FuncMath(KScript script)
        {
            mScript = script;
            mParse = script.mParse;
            mVar = script.mVar;
        }

        /// <summary>
        /// 関数の分岐処理
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">返す変数名</param>
        /// <returns></returns>
        public Token function(Token funcName, Token arg, Token ret)
        {
            List<Token> args = mScript.getFuncArgs(arg.mValue);
            switch (funcName.mValue) {
                case "math.max": return max(args);
                case "math.min": return min(args);
                case "math.sum": return sum(args);
                case "math.average": return average(args);
                case "math.variance": return variance(args);
                case "math.stdDeviation": return standardDeviation(args);
                case "math.covariance": return covariance(args);
                case "math.corrCoeff": return correlationCoefficient(args);
                case "math.regression": return regression(args, ret);
                case "math.quadraticEquation": return solveQuadraticEquation(args, ret);
                case "math.qubicEquation": return solveCubicEquation(args, ret);
                case "math.quarticEquation": return solveQuarticEquation(args, ret);
                case "math.fourier": return fourier(args, ret);
                default: return new Token("not found func", TokenType.ERROR);
            }
        }

        /// <summary>
        /// 最大値を求める(max(arg1,arg2...))
        /// arg = 数値/変数/配列 max(4,a,b[],c[,]);
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最大値</returns>
        public Token max(List<Token> args)
        {
            List<double> doubleList = new List<double>();
            foreach (Token arg in args)
                doubleList.AddRange(mVar.getDoubleListfromArg(arg));
            if (doubleList.Count > 0) {
                double max = doubleList.Max();
                return new Token(max.ToString(), TokenType.LITERAL);
            } else {
                return new Token("", TokenType.EMPTY);
            }
        }

        /// <summary>
        /// 最小値を求める(min(arg1,arg2...))
        /// arg = 数値/変数/配列 min(4,a,b[],c[,]);
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>最小値</returns>
        public Token min(List<Token> args)
        {
            List<double> doubleList = new List<double>();
            foreach (Token arg in args)
                doubleList.AddRange(mVar.getDoubleListfromArg(arg));
            if (doubleList.Count > 0) {
                double min = doubleList.Min();
                return new Token(min.ToString(), TokenType.LITERAL);
            } else {
                return new Token("", TokenType.EMPTY);
            }
        }

        /// <summary>
        /// 配列の合計
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>合計</returns>
        public Token sum(List<Token> args)
        {
            List<double> doubleList = new List<double>();
            foreach (Token arg in args)
                doubleList.AddRange(mVar.getDoubleListfromArg(arg));
            if (doubleList.Count > 0) {
                double sum = doubleList.Sum();
                return new Token(sum.ToString(), TokenType.LITERAL);
            } else {
                return new Token("", TokenType.EMPTY);
            }
        }

        /// <summary>
        /// 平均値を求める
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>平均値</returns>
        public Token average(List<Token> args)
        {
            List<double> doubleList = new List<double>();
            foreach (Token arg in args)
                doubleList.AddRange(mVar.getDoubleListfromArg(arg));
            if (doubleList.Count > 0) {
                double ave = doubleList.Sum() / doubleList.Count;
                return new Token(ave.ToString(), TokenType.LITERAL);
            } else {
                return new Token("", TokenType.EMPTY);
            }
        }

        /// <summary>
        /// 分散
        /// Σ(xi - xa)^2 / n
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>分散値</returns>
        public Token variance(List<Token> args)
        {
            (string arrayName, int no) = mUtil.getArrayName(args[0]);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = mVar.cnvListDouble(args[0]);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double ave = listData.Sum() / listData.Count;
            double vari = listData.Sum(p => (p - ave) * (p - ave)) / listData.Count;
            return new Token(vari.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 標準偏差
        /// Sx = xの標準偏差　√(Σ(xi - xa)^2 / n)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>標準偏差</returns>
        public Token standardDeviation(List<Token> args)
        {
            Token token = variance(args);
            if (token.mType != TokenType.ERROR)
                return new Token(Math.Sqrt(ylib.doubleParse(token.mValue)).ToString(), TokenType.LITERAL);
            else
                return token;
        }

        /// <summary>
        /// 共分散(a[],b[])
        /// Sxy = (Σ(xi - xa)*(yi - ya)) / n
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>共分散</returns>
        public Token covariance(List<Token> args)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> listData0 = mVar.cnvListDouble(args[0]);
            List<double> listData1 = mVar.cnvListDouble(args[1]);
            if (listData0.Count != listData1.Count)
                return new Token("", TokenType.ERROR);
            double ave0 = listData0.Average();
            double ave1 = listData1.Average();
            double total = 0;
            for (int i = 0; i < listData0.Count; i++) {
                total += (listData0[i] - ave0) * (listData1[i] - ave1);
            }
            return new Token((total / listData0.Count).ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 相関係数(x[],y[])
        /// r = Sxy / (Sx * Sy)    Sxy = 共分散  Sx = xの標準偏差)　Sy = yの標準偏差)
        /// Sxy = Σ(xi - xa) * (yi - ya) / (n - 1)       xa,ya = xi, yiの平均値  n = データ数
        /// Sx, Sy = x,yの標準偏差　√(Σ(xi - xa)^2 / n)
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>相関係数</returns>
        public Token correlationCoefficient(List<Token> args)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> x = mVar.cnvListDouble(args[0]);
            List<double> y = mVar.cnvListDouble(args[1]);
            if (x.Count != y.Count)
                return new Token("", TokenType.ERROR);
            double avex = x.Average();
            double avey = y.Average();
            double cov = 0;
            for (int i = 0; i < x.Count; i++) {
                cov += (x[i] - avex) * (y[i] - avey);
            }
            cov /= x.Count;
            double stdx = Math.Sqrt(x.Sum(p => (p - avex) * (p - avex)) / x.Count);
            double stdy = Math.Sqrt(y.Sum(p => (p - avey) * (p - avey)) / y.Count);
            return new Token((cov / (stdx * stdy)).ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 回帰分析(regression analysis)の係数(a)の取得  y = ax + b
        /// r[] = array.regression(x[],y[]);    r[0] = a , r[1] = b
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public Token regression(List<Token> args, Token ret)
        {
            if (args.Count < 2)
                return new Token("", TokenType.ERROR);
            if (mVar.getArrayOder(args[0]) == 1 && mVar.getArrayOder(args[1]) == 1) {
                List<double> x = mVar.cnvListDouble(args[0]);
                List<double> y = mVar.cnvListDouble(args[1]);
                double a = ylib.getRegA(x, y);
                double b = ylib.getRegB(x, y);
                double[] c = new double[] { a, b };
                mVar.setReturnArray(c, ret);
            } else {
                return new Token("", TokenType.EMPTY);
            }

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 2次方程式の解を求める
        /// result[] = a * x^2 + b * x + c
        /// </summary>
        /// <param name="args">a,b,c</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveQuadraticEquation(List<Token> args, Token ret)
        {
            if (args.Count < 3) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double[] result = ylib.solveQuadraticEquation(a, b, c).ToArray();
            //  戻り値の設定
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 3次方程式の解を求める
        /// result[] = a * x^3 + b * x^2 + c * x + d
        /// </summary>
        /// <param name="args">a,b,c,d</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveCubicEquation(List<Token> args, Token ret)
        {
            if (args.Count < 4) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double d = ylib.doubleParse(args[3].mValue);
            double[] result = ylib.solveCubicEquation(a, b, c, d).ToArray();
            //  戻り値の設定
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// 4次方程式の解を求める
        /// result[] = a * x^4 + b * x^3 + c * x^2 + d * x + e
        /// </summary>
        /// <param name="args">a,b,c,d,e</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token solveQuarticEquation(List<Token> args, Token ret)
        {
            if (args.Count < 5) return new Token("", TokenType.ERROR);
            double a = ylib.doubleParse(args[0].mValue);
            double b = ylib.doubleParse(args[1].mValue);
            double c = ylib.doubleParse(args[2].mValue);
            double d = ylib.doubleParse(args[3].mValue);
            double e = ylib.doubleParse(args[4].mValue);
            double[] result = ylib.solveQuarticEquation(a, b, c, d, e).ToArray();
            //  戻り値の設定
            mVar.setReturnArray(result, ret);
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }

        /// <summary>
        /// フーリエ変換 mag[] = math.fourier(x[])
        /// </summary>
        /// <param name="args"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token fourier(List<Token> args, Token ret)
        {
            if (mVar.getArrayOder(args[0]) == 1) {
                List<double> signal = mVar.getDoubleArrayList(args[0]);
                Complex[] complexSignal = signal.Select(val => new Complex(val, 0)).ToArray();
                Fourier.Forward(complexSignal, FourierOptions.Default);
                double[] magnitudes = complexSignal.Take(signal.Count / 2).Select(c => c.Magnitude).ToArray();
                mVar.setReturnArray(magnitudes, ret);
            } else
                return new Token("", TokenType.EMPTY);

            //  戻り値の設定
            mVar.setVariable(new Token("return", TokenType.VARIABLE), ret);
            return mVar.getVariable("return");
        }
    }
}
