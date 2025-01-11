using System.Collections;

namespace KScriptWin
{
    /// <summary>
    /// リングバッファ
    /// use
    ///     var buffer = new RingBuffer<string>(5);
    ///     buffer.Add("12");
    ///         :
    ///     buffer.Add("56");
    ///     string str = buffer[0];
    ///     int count = buffer.Count;
    ///     string str = bufferPop();
    ///     if (buffer.Contains("12") ...
    ///     foreach(string item in buffer) { Console.Write(item); }
    ///     int len = buffer.Max(s => s.Length);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingBuffer<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;               //  格納データ
        public int Count => _queue.Count;
        public int MaxCapacity {  get; private set; }   //  最大バッファサイズ
        public T this[int index] {
            get {
                if (index < 0 || index > this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)}={index}");
                return _queue.ElementAt(index);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="maxCapacity">最大サイズ</param>
        public RingBuffer(int maxCapacity)
        {
            MaxCapacity = maxCapacity;
            _queue = new Queue<T>(maxCapacity);
        }

        /// <summary>
        /// Item追加
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _queue.Enqueue(item);
            if (_queue.Count > MaxCapacity) {
                T removed = this.Pop();
            }
        }

        /// <summary>
        /// 先頭を取得して削除
        /// </summary>
        /// <returns></returns>
        public T Pop() => _queue.Dequeue();

        /// <summary>
        /// 先頭の取得(削除しない)
        /// </summary>
        /// <returns></returns>
        public T First() => _queue.Peek();

        /// <summary>
        /// 存在の有無
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) => _queue.Contains(item);

        /// <summary>
        /// 配列に変換
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() => _queue.ToArray();


        /// <summary>
        /// foreach で使用
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator ();
    }
}
