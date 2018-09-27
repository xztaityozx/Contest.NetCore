# ABC104 D We Love ABC
- A,B,C,?の4文字で構成される文字列Sがある
- SをA,B,Cのどれかにできるとき、前からABCになる組み合わせは何個あるかを求める問題
    - DP
- `dp[i][j] = i-1番目まででj個文字を選んでいるときの残りの文字を選ぶ組み合わせの数`
    - 式を立ててみる
    - ここで`N=|S|`とする
    - `dp[N][3] = 1` : N文字から3個選んでいる。あとは何もしないの1通りしかない
    - `dp[N][0~2] = 0` : N文字から0~2文字しか選んでいない。N文字見切っているので手遅れ0通りしかできない
    - `dp[i<N][3] = m * dp[i+1][3]` : すでに3個選んでいて、選ぶ余地はない。`m`は`S[i]`が`?`なら3それ以外なら1。
    - `dp[i<N[j=0~2] = m * dp[i+1][j] + k * dp[i+1][j+1]` : 3個選んでいないので選ばない(前半)、もしくは選ぶ(後半)ということができる
        - 選ばないなら組み合わせは`S[i]`が`?`なら3パターン増えて、それ以外なら1つ
        - 選ぶなら、`S[i]`が`?`もしくは`ABC`のうち`j`番目の文字なら選べるので1通り。それ以外なら0通り。
    - これを後ろからやっていくと答えが`dp[0][0]`に現れる。
```c#
public void Solve() {
    // ABC104 D
    var S = ReadLine();
    var N = S.Length;
    var dp = new Map<int,long>[N+1]; // dp[i][j]=i-1文字目まで処理していてj個選んでいるときの残りの文字の処理パターン数
    // dp[N][0~2] = 成立してないので0パターン, dp[N][3]=正常終了
    dp[N] = new Map<int, long> {[0] = 0, [1] = 0, [2] = 0, [3] = 1};
    var mask = "ABC_";

    Func<long, long> Mod = (x) => x % (long) (1e9 + 7);

    for (var i = N - 1; i >= 0; i--) {
        dp[i]=new Map<int, long>();
        foreach (var item in dp[i+1]) {
            var key = item.Key;
            
            var m = S[i] == '?' ? 3 : 1;
            var k = S[i] == '?' || S[i] == mask[key] ? 1 : 0;
            // すでに3つ選択しているのでやれることはない。組み合わせはS[i]が?なら3つ増える。それ以外は1つ
            if (key == 3) dp[i][3] = Mod(m * dp[i + 1][3]);
            // まだ3つ選択していないので選べる。ABCのうちkey番目の文字か?なら選べる。この文字を選ばないならパターンは上と同じ
            else dp[i][key] = Mod(Mod(m * dp[i + 1][key]) + Mod(k * dp[i + 1][key + 1]));
        }
    }
    
    dp[0][0].WL();


}
```

# SoundHound Inc. Programming Contest 2018 Masters Tournament D Saving Snuuk
- `N`個の都市間を`M`本の電車が結んでいる。この電車を何本か使って都市`s`から`t`へ行きたい。都市`ui`から`vi`へ行くには`ai`円かかり、`bi`スヌークかかる
- `s`から`t`へ旅する道中のどこかの駅で円を全額スヌークに変えておきたい。どこでも両替できるが`i`番目の駅の両替所は`i`年後に閉鎖する。
- `i`年後に出発し、`t`についたときの最大のスヌークを全部答えろ
- 単純に`s`から`t`への最短距離を選びたい。しかしある年以降はそのルートを辿ってもスヌークを最大化できなくなるかもしれない
- ここで`s`->`i`->`t` という道順を考える
    - `i`は両替を行う駅で、このルートのコストは __`s`から`i`へ向かうのかかる`円` + `i`から`t`へ向かうのにかかる`スヌーク`__ となる
    - これをすべての`i`について計算する。`i`番目のルートは`i`年後には使えなくなるのでそれ以降のルートのうち __最小コスト__ のものを選ぶ
- 肝心のルートのコストは`s` -> `i` にかかる円のグラフと `t` -> `i` にかかるスヌークのグラフを用意し、ダイクストラ法をする
    - `t` -> `i` と `i` -> `t` は同コスト
- `O(M+N)log(N))`
```c#
public void Solve() {
    // SoundHound Inc. Programming Contest 2018 -Masters Tournament- D
    int n = NextInt(), m = NextInt(), s = NextInt()-1, t = NextInt()-1;
    var yg = new Dijkstra(n);
    var sg=new Dijkstra(n);

    m.REP(i => {
        int ui = NextInt() - 1, vi = NextInt()-1, ai = NextInt(), bi = NextInt();
        yg.Add(ui, vi, ai, false);
        sg.Add(vi, ui, bi, false);
    });
    
    yg.Run(s);
    sg.Run(t);

    var ans = new Ll();
    n.REP(k => { ans.Add(yg.Distance[k] + sg.Distance[k]); });

    for (var k = n - 1; k > 0; k--) ans[k - 1] = Min(ans[k - 1], ans[k]);
    
    ans.Select(x=>1e15-x).WL();

}
```                                                                                                       

# SoundHound Inc. Programming Contest 2018 Masters Tournament 本戦(Open) B Neutralize
- 数列の`K`個の連続した区間を0にできるとき、数列の総和の最大値はいくつか
    - [B - Neutralize SoundHound Programming Contest 2018 - バイトの競プロメモ](http://baitop.hatenadiary.jp/entry/2018/07/29/204143)    
- `dp[i]=i番目までの最大値`
    - `i-K>=0` のとき
        - 候補は`これまでの最大値+数列の次の値`もしくは`K個前までの最大値`
        - 前者は0にする操作をしない
        - 後者は`i-K`から`i`までを0で埋める場合
        - `K`個前までの最大値は`Max(一つ前の最大値,dp[i])` で計算できる。
    - `i<K`のとき
        - `i`を終点に0埋めすることができない。ので普通に加算するしかない
        
- `O(N)`
```c#
public void Solve() {
    int N = NextInt(), K = NextInt();
    var b = new Ll();
    N.REP(i => b.Add(NextLong()));
    var dp = new long[N + 1];
    var max = new long[N + 1];

    for (var i = 1; i <= N; i++) {
        if (i - K >= 0) dp[i] = Max(dp[i - 1] + b[i - 1], max[i - K]);
        else dp[i] += dp[i - 1] + b[i - 1];

        max[i] = Max(max[i - 1], dp[i]);
    }
    
    dp[N].WL();
}
```

