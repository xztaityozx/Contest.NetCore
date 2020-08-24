using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using CS_Contest.Loop;
using static CS_Contest.IO.IO;
using static System.Math;
using static System.Console;

namespace ContestCore {
    public static class Utils2 {
        public static BigInteger GCD(BigInteger m, BigInteger n) {
            BigInteger tmp;
            if (m < n) {
                tmp = n;
                n = m;
                m = tmp;
            }

            while (m % n != 0) {
                tmp = n;
                n = m % n;
                m = tmp;
            }

            return n;
        }

        public static BigInteger LCM(BigInteger m, BigInteger n) => m * (n / GCD(m, n));

        public static long INF = long.MaxValue;

        public class MathUtils {
            public long[] Factorial { get; set; }
            public long[] Inverse { get; set; }
            private int N { get; set; }

            public MathUtils(int n = (int) 1e5 + 1) {
                N = n;
                Factorial = new long[N * 2];
                Inverse = new long[N * 2];
            }

            public void MakeFact() {
                Factorial[0] = Inverse[0] = 1;
                for (var i = 1; i < Factorial.Length; i++) {
                    Factorial[i] = Mod(Factorial[i - 1] * i);
                    Inverse[i] = ModPow(Factorial[i], ModValue - 2);
                }
            }

            public long Combination(long n, long k) {
                if (Factorial[0] == 0) MakeFact();
                if (n - k < 0) return 0;
                var rt = Factorial[n];
                rt *= Inverse[k];
                rt %= ModValue;
                rt *= Inverse[n - k];
                return Mod(rt);
            }

            public static long[,] CombinationTable(int n) {
                var rt = new long[n + 1, n + 1];
                for (var i = 0; i <= n; i++) {
                    for (var j = 0; j <= i; j++) {
                        if (j == 0 || i == j) rt[i, j] = 1L;
                        else rt[i, j] = (rt[i - 1, j - 1] + rt[i - 1, j]);
                    }
                }

                return rt;
            }

            public static long ModValue = (long) 1e9 + 7;
            public static long Mod(long x) => x % ModValue;
            public static long DivMod(long x, long y) => Mod(x * ModPow(y, (long) (1e9 + 5)));

            public static long ModPow(long x, long n) {
                long tmp = 1;
                while (n != 0) {
                    if (n % 2 == 1) {
                        tmp = Mod(tmp * x);
                    }

                    x = Mod(x * x);
                    n /= 2;
                }

                return tmp;
            }
        }

        public static bool IsPrime(long n) {
            if (n == 2) return true;
            if (n < 2 || n % 2 == 0) return false;
            var i = 3;
            var sq = (long) Sqrt(n);
            while (i <= sq) {
                if (n % i == 0) return false;
                i += 2;
            }

            return true;
        }

        public static IEnumerable<int> Primes(int maxnum) {
            yield return 2;
            yield return 3;
            var sieve = new BitArray(maxnum + 1);
            int squareroot = (int) Math.Sqrt(maxnum);
            for (int i = 2; i <= squareroot; i++) {
                if (sieve[i] == false) {
                    for (int n = i * 2; n <= maxnum; n += i)
                        sieve[n] = true;
                }

                for (var n = i * i + 1; n <= maxnum && n < (i + 1) * (i + 1); n++) {
                    if (!sieve[n])
                        yield return n;
                }
            }
        }

        public static int ManhattanDistance(int x1, int y1, int x2, int y2) => Abs(x2 - x1) + Abs(y2 - y1);
    }

    public static class Local {
        public static void Main(string[] args) {
            "===============Start of Solve================".WL();
            var stopwatch = new Stopwatch();
            var sw = new StreamWriter(OpenStandardOutput()) {AutoFlush = false};
            SetOut(sw);
            stopwatch.Start();
            Program.Calc.Solve();
            Out.Flush();
            stopwatch.Stop();
            "===============End of Solve================".WL();
            (stopwatch.ElapsedMilliseconds + " ms").WL();
            Out.Flush();
        }
    }

    public class Deque<T> {
        private T[] buf;
        private int offset, count, capacity;
        public int Count => count;

        public Deque(int cap) {
            buf = new T[capacity = cap];
        }

        public Deque() {
            buf = new T[capacity = 16];
        }

        public T this[int index] {
            get => buf[getIndex(index)];
            set => buf[getIndex(index)] = value;
        }

        private int getIndex(int index) {
            if (index >= capacity)
                throw new IndexOutOfRangeException("out of range");
            var ret = index + offset;
            if (ret >= capacity)
                ret -= capacity;
            return ret;
        }

        public void PushFront(T item) {
            if (count == capacity) Extend();
            if (--offset < 0) offset += buf.Length;
            buf[offset] = item;
            ++count;
        }

        public T PopFront() {
            if (count == 0)
                throw new InvalidOperationException("collection is empty");
            --count;
            var ret = buf[offset++];
            if (offset >= capacity) offset -= capacity;
            return ret;
        }

        public void PushBack(T item) {
            if (count == capacity) Extend();
            var id = count++ + offset;
            if (id >= capacity) id -= capacity;
            buf[id] = item;
        }

        public T PopBack() {
            if (count == 0)
                throw new InvalidOperationException("collection is empty");
            return buf[getIndex(--count)];
        }

        public void Insert(int index, T item) {
            if (index > count) throw new IndexOutOfRangeException();
            this.PushFront(item);
            for (var i = 0; i < index; i++)
                this[i] = this[i + 1];
            this[index] = item;
        }

        public T RemoveAt(int index) {
            if (index < 0 || index >= count) throw new IndexOutOfRangeException();
            var ret = this[index];
            for (var i = index; i > 0; i--)
                this[i] = this[i - 1];
            this.PopFront();
            return ret;
        }

        private void Extend() {
            var newBuffer = new T[capacity << 1];
            if (offset > capacity - count) {
                var len = buf.Length - offset;
                Array.Copy(buf, offset, newBuffer, 0, len);
                Array.Copy(buf, 0, newBuffer, len, count - len);
            }
            else Array.Copy(buf, offset, newBuffer, 0, count);

            buf = newBuffer;
            offset = 0;
            capacity <<= 1;
        }

        public T[] Items //デバッグ時に中身を調べるためのプロパティ
        {
            get {
                var a = new T[count];
                for (var i = 0; i < count; i++)
                    a[i] = this[i];
                return a;
            }
        }
    }
}

namespace CS_Contest.Graph {
    using Ll = List<long>;
    using Li = List<int>;

    public class Labyrinth {
        private char Road { get; set; }
        private int H { get; set; }
        private int W { get; set; }
        private List<string> labyrinth;

        public Labyrinth(int h, int w, char road) {
            H = h;
            W = w;
            Road = road;
            labyrinth = new List<string>();

            for (int i = 0; i < H; i++) {
                labyrinth.Add(ReadLine());
            }
        }

        public int ShortestPath(int sx, int sy, int gx, int gy) {
            var queue = new Queue<Tuple<int, int, int>>();
            var used = new bool[H, W];
            var dx = new[] {1, 0, -1, 0};
            var dy = new[] {0, -1, 0, 1};
            queue.Enqueue(new Tuple<int, int, int>(sx, sy, 0));
            used[sy, sx] = true;
            var min = int.MaxValue;
            while (queue.Any()) {
                var v = queue.Dequeue();
                for (int i = 0; i < 4; i++) {
                    var x = v.Item1 + dx[i];
                    var y = v.Item2 + dy[i];
                    if (y < 0 || x < 0 || y >= H || x >= W) continue;
                    if (labyrinth[y][x] != Road) continue;
                    if (x == W - 1 && y == H - 1) {
                        min = Min(min, v.Item3 + 1);
                        continue;
                    }

                    if (used[y, x]) continue;
                    used[y, x] = true;
                    queue.Enqueue(new Tuple<int, int, int>(x, y, v.Item3 + 1));
                }
            }

            return min;
        }
    }

    public class MaxFlow {
        private class Edge {
            public int To, Reverse, Capacity;
        }

        private int V { get; set; }
        private List<Edge>[] graph { get; set; }
        private int[] leveList, itr;

        public MaxFlow(int v) {
            V = v;
            graph = Enumerable.Repeat(1, V).Select(x => new List<Edge>()).ToArray();
        }

        public void Add(int from, int to, int capa, bool dir = true) {
            graph[from].Add(new Edge {Capacity = capa, Reverse = graph[to].Count, To = to});
            graph[to].Add(new Edge {To = from, Capacity = dir ? 0 : capa, Reverse = graph[from].Count - 1});
        }

        private void Bfs(int s) {
            leveList = Enumerable.Repeat(-1, V).ToArray();
            var queue = new Queue<int>();
            leveList[s] = 0;
            queue.Enqueue(s);
            while (queue.Any()) {
                var src = queue.Dequeue();
                foreach (var edge in graph[src]) {
                    if (edge.Capacity <= 0 || leveList[edge.To] >= 0) continue;
                    leveList[edge.To] = leveList[src] + 1;
                    queue.Enqueue(edge.To);
                }
            }
        }

        private int Dfs(int v, int t, int f) {
            if (v == t) return f;
            for (; itr[v] < graph[v].Count; itr[v]++) {
                var edge = graph[v][itr[v]];
                if (edge.Capacity <= 0 || leveList[v] >= leveList[edge.To]) continue;
                var d = Dfs(edge.To, t, Min(f, edge.Capacity));
                if (d <= 0) continue;
                edge.Capacity -= d;
                graph[edge.To][edge.Reverse].Capacity += d;
                return d;
            }

            return 0;
        }

        public int Run(int s, int t) {
            int rt = 0;
            Bfs(s);
            while (leveList[t] >= 0) {
                itr = new int[V];
                int f;
                while ((f = Dfs(s, t, int.MaxValue)) > 0) rt += f;
                Bfs(s);
            }

            return rt;
        }
    }

    public struct WeightedUnionFind {
        private readonly int N;
        public int[] Parent { get; private set; }
        public long[] Cost { get; private set; }
        public int[] Rank { get; private set; }

        public WeightedUnionFind(int n) {
            N = n;
            Parent = Enumerable.Range(0, N).ToArray();
            Cost = new long[N];
            Rank = new int[N];
        }

        public int Root(int u, out long cost) {
            if (Parent[u] == u) {
                cost = Cost[u];
                return u;
            }

            var v = Root(Parent[u], out cost);
            cost += Cost[u];
            Parent[u] = v;
            Cost[u] = cost;
            return v;
        }

        public void Unite(int lv, int rv, long cost) {
            long lc, rc;
            lv = Root(lv, out lc);
            rv = Root(rv, out rc);
            cost = -rc + cost + lc;

            if (Rank[lv] < Rank[rv]) {
                Unite(rv, lv, -cost);
                return;
            }

            Parent[rv] = lv;
            Cost[rv] = cost;
            Rank[lv] += Rank[rv] + 1;
        }

        public bool IsValid() {
            for (var i = 0; i < N; i++) {
                long _;
                Root(i, out _);
            }

            for (var i = 0; i < N; i++) {
                if (Parent[i] == i && Cost[i] != 0L) {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// UnionFind
    /// </summary>
    public struct UnionFind {
        private readonly int[] _data;

        public UnionFind(int size) {
            _data = new int[size];
            for (var i = 0; i < size; i++) _data[i] = -1;
        }

        public bool Unite(int x, int y) {
            x = Root(x);
            y = Root(y);

            if (x == y) return x != y;
            if (_data[y] < _data[x]) {
                var tmp = y;
                y = x;
                x = tmp;
            }

            _data[x] += _data[y];
            _data[y] = x;
            return x != y;
        }

        public bool IsSameGroup(int x, int y) {
            return Root(x) == Root(y);
        }

        public int Root(int x) {
            return _data[x] < 0 ? x : _data[x] = Root(_data[x]);
        }
    }

    public class BellmanFord : CostGraph {
        public BellmanFord(int size) : base(size) {
        }

        public List<long> Distance { get; set; }

        private bool[] _negative;
        public bool HasCycle => _negative[Size - 1];

        public void Run(int s) {
            Distance = new Ll();
            Size.REP(i => Distance.Add(Inf));
            Distance[s] = 0;
            _negative = new bool[Size];

            (Size - 1).REP(i => Size.REP(j => Adjacency[j].Count.REP(k => {
                    var src = Adjacency[j][k];
                    if (Distance[src.To] > Distance[j] + src.Cost) Distance[src.To] = Distance[j] + src.Cost;
                }
            )));

            for (int i = 0; i < Size; i++) {
                Size.REP(j => {
                    Adjacency[j].Count.REP(k => {
                        var src = Adjacency[j][k];
                        if (Distance[src.To] > Distance[j] + src.Cost) {
                            Distance[src.To] = Distance[j] + src.Cost;
                            _negative[src.To] = true;
                        }

                        if (_negative[j]) _negative[src.To] = true;
                    });
                });
            }
        }
    }

    public class CostGraph {
        public struct Edge {
            public int To { get; set; }
            public long Cost { get; set; }


            public Edge(int to, long cost) {
                To = to;
                Cost = cost;
            }
        }

        public int Size { get; set; }
        public List<List<Edge>> Adjacency { get; set; }
        public const long Inf = (long) 1e15;

        public CostGraph(int size) {
            Size = size;
            Adjacency = new List<List<Edge>>();
            Size.REP(_ => Adjacency.Add(new List<Edge>()));
        }

        public void Add(int s, int t, long c, bool dir = true) {
            Adjacency[s].Add(new Edge(t, c));
            if (!dir) Adjacency[t].Add(new Edge(s, c));
        }
    }

    /// <summary>
    /// 優先度付きキュー
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> {
        private readonly List<T> heap;
        private readonly Comparison<T> compare;
        private int size;

        public PriorityQueue() : this(Comparer<T>.Default) {
        }

        public PriorityQueue(IComparer<T> comparer) : this(16, comparer.Compare) {
        }

        public PriorityQueue(Comparison<T> comparison) : this(16, comparison) {
        }

        public PriorityQueue(int capacity, Comparison<T> comparison) {
            this.heap = new List<T>(capacity);
            this.compare = comparison;
        }

        public void Enqueue(T item) {
            this.heap.Add(item);
            var i = size++;
            while (i > 0) {
                var p = (i - 1) >> 1;
                if (compare(this.heap[p], item) <= 0)
                    break;
                this.heap[i] = heap[p];
                i = p;
            }

            this.heap[i] = item;
        }

        public T Dequeue() {
            var ret = this.heap[0];
            var x = this.heap[--size];
            var i = 0;
            while ((i << 1) + 1 < size) {
                var a = (i << 1) + 1;
                var b = (i << 1) + 2;
                if (b < size && compare(heap[b], heap[a]) < 0) a = b;
                if (compare(heap[a], x) >= 0)
                    break;
                heap[i] = heap[a];
                i = a;
            }

            heap[i] = x;
            heap.RemoveAt(size);
            return ret;
        }

        public T Peek() {
            return heap[0];
        }

        public int Count => size;

        public bool Any() {
            return size > 0;
        }
    }


    public class Dijkstra : CostGraph {
        public Dijkstra(int size) : base(size) {
        }

        public int[] PreviousNodeList { get; set; }
        public long[] Distance { get; set; }

        public void Run(int s) {
            PreviousNodeList = new int[Size];
            Distance = new long[Size];
            Size.REP(_ => Distance[_] = Inf);

            var pq = new PriorityQueue<Edge>((x, y) => x.Cost.CompareTo(y.Cost));
            Distance[s] = 0;
            pq.Enqueue(new Edge(s, 0));
            while (pq.Any()) {
                var src = pq.Dequeue();
                if (Distance[src.To] < src.Cost) continue;
                for (var i = 0; i < Adjacency[src.To].Count; i++) {
                    var tmp = Adjacency[src.To][i];
                    var cost = tmp.Cost + src.Cost;
                    if (cost >= Distance[tmp.To]) continue;
                    Distance[tmp.To] = cost;
                    pq.Enqueue(new Edge(tmp.To, cost));
                    PreviousNodeList[tmp.To] = src.To;
                }
            }
        }
    }


    public class WarshallFloyd : CostGraph {
        public WarshallFloyd(int size) : base(size) {
        }

        public List<Ll> Run() {
            var rt = new List<Ll>();
            Size.REP(_ => rt.Add(new Ll()));

            Size.REP(i => Size.REP(k => rt[i].Add(i == k ? 0 : Inf)));

            Adjacency.ForeachWith((i, item) => {
                foreach (var k in item) {
                    rt[i][k.To] = k.Cost;
                }
            });

            Size.REP(i => Size.REP(j => Size.REP(k => { rt[j][k] = Min(rt[j][k], rt[j][i] + rt[i][k]); })));

            return rt;
        }
    }

    public class Kruskal {
        private List<Tuple<int, int, long>> edgeList;
        private int N { get; set; }

        public Kruskal(int n) {
            N = n;
            edgeList = new List<Tuple<int, int, long>>();
        }

        public void Add(int s, int t, long cost) => edgeList.Add(new Tuple<int, int, long>(s, t, cost));

        public long Run() {
            edgeList.Sort((a, b) => a.Item3.CompareTo(b.Item3));
            var rt = 0L;
            var uf = new UnionFind(N);
            foreach (var tuple in edgeList) {
                var s = tuple.Item1;
                var t = tuple.Item2;
                var cost = tuple.Item3;
                if (uf.IsSameGroup(s, t)) continue;
                uf.Unite(s, t);
                rt += cost;
            }

            return rt;
        }
    }

    public class BipartiteGraph : CostGraph {
        public BipartiteGraph(int size) : base(size) {
        }

        public enum State {
            Undefined,
            Black,
            White
        }

        public long BlackCount { get; private set; }

        public long WhiteCount {
            get { return Size - BlackCount; }
        }

        public bool IsBipartGraph() {
            Func<int, State, bool> dfs = null;
            var state = new State[Size];
            BlackCount = 0;
            dfs = (to, nextState) => {
                if (state[to] != State.Undefined) {
                    return state[to] == nextState;
                }

                state[to] = nextState;
                if (nextState == State.Black) BlackCount++;
                var rt = true;
                foreach (var edge in Adjacency[to]) {
                    rt &= dfs(edge.To, nextState == State.Black ? State.White : State.Black);
                }

                return rt;
            };
            return dfs(0, State.Black);
        }
    }

    /// <summary>
    /// 橋検出
    /// </summary>
    public class Bridge {
        public List<int>[] Graph;
        public int V;

        public Bridge(int N) {
            V = N;
            Graph = new Li[N];
            for (int i = 0; i < N; i++) {
                Graph[i] = new Li();
            }
        }

        public void Add(int s, int t, bool dir = true) {
            Graph[s].Add(t);
            if (!dir) Graph[t].Add(t);
        }

        public int Run() {
            var pre = Enumerable.Repeat(-1, V).ToArray();
            var low = Enumerable.Repeat(-1, V).ToArray();
            var res = new List<Tuple<int, int>>();
            var cnt = 0;

            Func<int, int, int> dfs = null;
            dfs = (v, from) => {
                pre[v] = cnt++;
                low[v] = pre[v];
                foreach (var node in Graph[v]) {
                    if (pre[node] == -1) {
                        low[v] = Min(low[v], dfs(node, v));
                        if (low[node] == pre[node]) {
                            res.Add(new Tuple<int, int>(v, node));
                        }
                    }
                    else {
                        if (from == node) continue;
                        low[v] = Min(low[v], low[node]);
                    }
                }

                return low[v];
            };
            dfs(0, -1);
            return res.Count;
        }
    }
}