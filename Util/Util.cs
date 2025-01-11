using CoreLib;

namespace KScriptWin
{
    public class Util
    {
        private YLib ylib = new YLib();

        /// <summary>
        /// 1次元配列名のインデックスを求める
        /// </summary>
        /// <param name="array">配列名</param>
        /// <returns>インデックス</returns>
        public int indexOfArray(string array)
        {
            int sp = array.IndexOf('[');
            int ep = array.IndexOf("]");
            if (0 < sp && sp < ep)
                return ylib.intParse(array.Substring(sp + 1, ep - sp - 1), -1);
            return -1;
        }

        /// <summary>
        /// 1次元配列リストからインデックスの最大値を求める
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public int maxIndexOfArray(Dictionary<string, Token> array)
        {
            int maxIndex = 0;
            foreach (var keyValue in array) {
                int index = indexOfArray(keyValue.Key);
                if (maxIndex < index)
                    maxIndex = index;
            }
            return maxIndex;
        }

    }
}
