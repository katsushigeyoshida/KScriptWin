namespace KScriptWin
{
    /// <summary>
    /// 配列変数の処理クラス
    /// </summary>
    public class ArrayName
    {
        public string mName;                //  配列名(インデックスなし)
        public List<string> mIndexs;        //  配列のインデックス(上位から下位に積まれる)
        public string mValue;               //  配列の値


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arrayName">配列名</param>
        /// <param name="indexs">インデックスリスト</param>
        /// <param name="value">値</param>
        public ArrayName(string arrayName, List<string> indexs, string value)
        {
            mName= arrayName;
            mIndexs = indexs;
            mValue = value;
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arrayName">配列名(a[1,2]など)</param>
        /// <param name="value">値</param>
        public ArrayName(string arrayName, string value)
        {
            int p = arrayName.IndexOf('[');
            if (p < 0) {
                mName = arrayName;
                mIndexs = null;
            } else {
                mName = arrayName.Substring(0, p);
                mIndexs = getArrayIndexList(arrayName);
            }
            mValue = value;
        }

        /// <summary>
        /// 配列名の取得(インデックス付き)
        /// </summary>
        /// <returns>配列名</returns>
        public string getArrayName()
        {
            string arrayName = mName + "[";
            for (int i = 0; i < mIndexs.Count; i++)
                arrayName += mIndexs[i] + ",";
            arrayName = arrayName.TrimEnd(',');
            return arrayName + "]";
        }

        /// <summary>
        /// インデックスに数値の値を設定する
        /// </summary>
        /// <param name="n">インデックスの位置</param>
        /// <param name="v">インデックスの値</param>
        public void setIntIndex(int n, int v)
        {
            mIndexs[n % mIndexs.Count] = v.ToString();
        }

        /// <summary>
        /// インデックスの数値を取得
        /// </summary>
        /// <param name="n">インデックスの位置</param>
        /// <returns>インデックスの値</returns>
        public int getIntIndex(int n)
        {
            return parseInt(mIndexs[n % mIndexs.Count]);
        }

        /// <summary>
        /// インデックスの数値リスト
        /// </summary>
        /// <returns>数値リスト</returns>
        public List<int> getIntIndexs()
        {
            List<int> indexs = new();
            for (int i = 0; i < mIndexs.Count; i++)
                indexs.Add(parseInt(mIndexs[i]));
            return indexs;
        }

        /// <summary>
        /// インデックスのリストを指定サイズで返す
        /// </summary>
        /// <param name="size">インデックスサイズ</param>
        /// <returns>インデックスリスト</returns>
        public string[] getIndex(int size = 0)
        {
            size = size == 0 ? mIndexs.Count : size;
            string[] array = new string[size];
            for (int i = size - 1, j = mIndexs.Count -1; i >= 0; i--, j--) {
                if (0 <= j)
                    array[i] = mIndexs[j];
                else
                    array[i] = "";
            }
            return array;
        }

        /// <summary>
        /// インデックスの大小比較
        /// </summary>
        /// <param name="arrayName">配列名</param>
        /// <returns>1/0/-1</returns>
        public int compareTo(ArrayName arrayName)
        {
            List<int> indexs = arrayName.getIntIndexs();
            if (mIndexs.Count > indexs.Count) return 1;
            else if (mIndexs.Count < indexs.Count) return -1;
            for (int i = 0; i < mIndexs.Count; i++) {
                if (parseInt(mIndexs[i]) > indexs[i]) return 1;
                if (parseInt(mIndexs[i]) < indexs[i]) return -1;
            }
            return 0;
        }

        /// <summary>
        /// 配列名からインデックスの抽出(a[1,2] => a [ 1 , 2 ])
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>インデックスリスト</returns>
        private List<string> getArrayIndexList(string args)
        {
            List<string> indexList = new();
            int n = args.IndexOf('[') + 1;
            if (n < 0)
                return indexList;
            string buf = "";
            while (n < args.Length) {
                if (args[n] == ']') {
                    indexList.Add(buf);
                    break;
                } else if (args[n] == ',') {
                    indexList.Add(buf);
                    buf = "";
                } else if (args[n] == '\"') {
                    buf = args[n++].ToString();
                    while (n < args.Length && args[n] != '\"') {
                        buf += args[n++].ToString();
                    }
                    buf += args[n].ToString();
                } else if (args[n] == ' ' || args[n] == '\t') {
                    //  読み飛ばし
                } else {
                    buf += args[n];
                }
                n++;
            }
            return indexList;
        }

        /// <summary>
        /// 文字列を整数に変換
        /// </summary>
        /// <param name="buf">文字列</param>
        /// <returns>整数</returns>
        private int parseInt(string buf)
        {
            int n = 0;
            if (int.TryParse(buf, out n))
                return n;
            return 0;
        }
    }
}
