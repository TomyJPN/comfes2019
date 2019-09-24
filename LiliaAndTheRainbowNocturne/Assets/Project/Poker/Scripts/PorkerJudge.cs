/*  http://www5a.biglobe.ne.jp/~iwase47/09cma1/poker2.c より
    一人で遊ぶポーカーのプログラム（ジョーカー入り）


        ジョーカーを一枚使う。最初にコインを何枚か賭ける。五枚のカードが手札と
        して配られ、そのうちの零枚から五枚を選んで、一度だけ交換すると、交換後
        の手札の役に応じた枚数のコインがもらえる。これのくり返し。

        トランプカードは、0 から 51 の数で表す。0 から 12 がスペードの A から K
        を、13 から 25 がハートの A から K を、26 から 38 がクラブの A から K
        を、39 から 51 がダイヤモンドの A から K を意味する。（こう決めると、数
        を 13 で割った商がスートを、余りがランクを表すことになる。これは、昔
        ながらの決まったやり方の一つらしい。）

        内部では、ジョーカーは 52 で表している。また、ジョーカーはその時点での
        手札に含まれないどんなカードの代わりにもなる、というルールを採用した。
        役の判定に当っては、ジョーカーを 52 枚のカードすべてで順に置き換えて、
        できた役のうち最高の役になったものを役としている。だから、ジョーカー
        を手札に含まれているカードで置き換えてできる役も含めている。（これでも
        同じ結果になるはずである。）

        ここまでで習ったことだけを使って書いてある。Ｃ言語の全貌を学べばもっと
        うまく書ける部分を含んでいる。

        （画面制御エスケープシーケンスは使っていない。）

        poker.c からの、その他の改変箇所は以下の通り。

        最初にカードが配られたときにも役を表示するようにした。

        乱数の種に、現在時刻だけでなく持っているコインの枚数も関係するように
        した。（前のままでは、二人で同時に動かして一人が全カードを交換し、それ
        を見てもう一人が交換するカードを選ぶという裏技ができたので。）

        当ったコインの枚数が、行頭でない位置に表示されるようにした。次に行頭
        から出力される、持っているコインの枚数と間違える場合があったため。

        交換するカードを入力し、Enter を押した時点で、再度、乱数が初期化される
        ようにした。そのほうが、自分で運を切りひらいた感じが強まると思うので。

        コイン枚数がオーバーフローして負になった場合も破産メッセージが出るよう
        にした。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorkerJudge : MonoBehaviour {
  //define
  int EMPTY = -1;
  int JOKER = 52;

  bool[] card = new bool[53];   /* ジョーカーを使うので 53 枚 */
  int[] hand = new int[6];    /* 手札。hand[0] は使わない */
  bool[] a = new bool[6];       /* hand[i] を新たにひくなら a[i] には true を、そうでなければ */
                                /* false を入れる。a[0] は使わない */

  void Start() {

  }

  void Update() {

  }

  /* card[ ]（カード）を初期化する */
  void initcard() {
    int i;

    for (i = 0; i <= 52; i++) {     /* true はまだそのカードが引かれていない */
      card[i] = true;              /* ことを示す印                         */
    }
  }

  /* カードをひく。a[i]（i は 1 から 5）が true であるような i に対し、hand[i] */
  /* を選ぶ。カードはスペードの A からダイヤモンドの K まで順番に並んでおり、 */
  /* そのあとにジョーカーが置かれている。乱数を使ってその中から一枚を選ぶ。   */
  /* すでにひかれたカードを再度選ぶことを防ぐため、ひいたカードに対する       */
  /* card[n] は true から false に変える。false のカードを選んでしまったら、再度     */
  /* ひく。このゲームでは最大で 10 枚のカードしか使わないので、このやり方で   */
  /* まず大丈夫であろう。                                                     */
  public void draw() {
    int i, n;

    for (i = 1; i <= 5; i++) {
      if (a[i] == true) {
        for (hand[i] = EMPTY; hand[i] == EMPTY;) {
          n = Random.Range(0, 52);            /* これが選ばれたカード。       */
          if (card[n] == true) {       /* 「残っている」だったら       */
            hand[i] = n;            /* そのカードを hand[i] に代入、*/
            card[n] = false;           /* 「残っていない」に変える     */
          }
        }
      }
    }
  }
  /* 手札の出力。非常にシンプルにしてある。ここだけを変えることも可能。hand[i] */
  /* の値は i 番目のカードを意味する。値とカードの関係は冒頭のコメントを参照。 */
  string printhands() {
    int i, y;
    string print = "";

    for (i = 1; i <= 5; i++) {  /* カードの数（ランク）を表示 */
      if (hand[i] == JOKER) {
        print += " ? ";          /* ジョーカーはランクの位置に「?」を書く */
      }
      else {
        y = hand[i] % 13;       /* これがカードの数（ランク）を示す */
        if (y == 0) {               /* 数（ランク）の表示 */
          print += " A ";
        }
        else if (y == 10) {
          print += " J ";
        }
        else if (y == 11) {
          print += " Q ";
        }
        else if (y == 12) {
          print += " K ";
        }
        else {
          print += (y + 1) + " ";
        }
      }
    }
    print += "\n";

    for (i = 1; i <= 5; i++) {  /* カードのマーク（スート）を表示 */
      if (hand[i] == JOKER) {     /* ジョーカーはスートの位置も「?」を書く */
        print += " ? ";
      }
      else {
        y = hand[i] / 13;       /* これがカードのマーク（スート）を示す */
        if (y == 0) {
          print += " S ";          /* スペード（のつもり）*/
        }
        else if (y == 1) {
          print += " H ";          /* ハート（のつもり）*/
        }
        else if (y == 2) {
          print += " C ";         /* クラブ（のつもり）*/
        }
        else {                    /*（「%」を出力させるときは「%%」）*/
          print += " D ";          /* ダイヤモンド（のつもり） */
        }
      }
    }
    print += "\n";
    Debug.Log(print);
    return print;
  }


  /* 手札を解析し、倍率を返す。具体的には、ジョーカーがあれば、それを 52 枚の  */
  /* 普通のカードで順に置き換えつつ analyse() を呼んで、返ってきた倍率のうちで */
  /* 最高のものを返す。ジョーカーがなければ、analyse() を呼んで、その返して    */
  /* きた値（倍率）を返すだけ。                                                */
  int analyse0() {
    int i, j, x, get = 0;
    bool joker;

    joker = false;
    for (i = 1; i <= 5; i++) {
      if (hand[i] == JOKER) {
        joker = true;
        get = 0;
        for (j = 0; j < 52; j++) {
          hand[i] = j;
          x = analyse();
          if (x > get) {
            get = x;
          }
        }
        hand[i] = JOKER;    /* 置き換えたのをジョーカーに戻しておく */
      }
    }
    if (joker == true) {
      return get;
    }
    else {
      return analyse();
    }
  }



  /* 手札の役の解析を行ない、役に応じた倍率を返す。ジョーカーは手札に含まれて  */
  /* いないと仮定しているが、ジョーカーを普通のカードで置き換えた場合も扱うの  */
  /* で、同じカードが二枚ある場合にも対応している！                            */
  int analyse() {
    int i, j, x, y;
    int samerank, sequence, sequence2, get;
    bool flush;

    /* 数（ランク）が一致する組（ペア）の数を数える。この数でワンペア、 */
    /* ツーペア、スリーカード、フルハウスなどが判定できるのは有名な事実 */
    samerank = 0;
    for (i = 1; i <= 5; i++) {
      for (j = i + 1; j <= 5; j++) {
        if (hand[i] % 13 == hand[j] % 13) {
          samerank++;
        }
      }
    }

    /* ランクが一致するペアがない場合に限り、隣り合ったランクのペアの数を  */
    /* 数える。これはストレートかどうかを判定するためなので、ランクが一致  */
    /* するペアがある場合は 0 とする。                                     */
    sequence = 0; sequence2 = 0;
    if (samerank == 0) {
      for (i = 1; i <= 5; i++) {
        for (j = i + 1; j <= 5; j++) {
          x = hand[i] % 13 - hand[j] % 13;
          if (x == 1 || x == -1) {
            sequence++;
          }
        }
      }
      /* ここで sequence が 4 ならストレート（10-J-Q-K-A 以外）*/

      for (i = 1; i <= 5; i++) {
        for (j = i + 1; j <= 5; j++) {
          if (hand[i] % 13 == 0) {    /* A は 13 と置き換えてから */
            x = 13;
          }
          else {
            x = hand[i] % 13;
          }
          if (hand[j] % 13 == 0) {
            y = 13;
          }
          else {
            y = hand[j] % 13;
          }
          if (x == y + 1 || x + 1 == y) {     /* 上と同じことを行なう */
            sequence2++;                    /* （細部は違うけど）   */
          }
        }
      }
      /* ここで sequence2 が 4 ならストレート（A-2-3-4-5 以外） */
    }

    /* 次に、マーク（スート）がすべて同じかどうかの判定を行なう */
    /* （素直に && で四つの条件をつなげたほうがよかったかも） */
    flush = true;
    for (i = 1; i <= 4; i++) {
      if (hand[i] / 13 != hand[i + 1] / 13) {
        flush = false;
      }
    }

    /* 以上で、解析の材料はそろった。あとは判定するのみ。get は倍率。*/

    if (samerank == 1) {
      get = 1;                        /* ワンペア */
    }
    else if (samerank == 2) {
      get = 2;                        /* ツーペア */
    }
    else if (samerank == 3) {
      get = 3;                        /* スリーカード */
    }
    else if ((sequence == 4 || sequence2 == 4) && flush == false) {
      get = 4;                        /* ストレート */
    }
    else if (sequence != 4 && sequence2 != 4 && flush == true) {
      get = 5;                        /* フラッシュ */
    }
    else if (samerank == 4) {
      get = 10;                       /* フルハウスです */
    }
    else if (samerank == 6) {
      get = 20;                       /* フォーカード */
    }
    else if (sequence == 4 && flush == true) {
      get = 50;                       /* ストレートフラッシュ */
    }
    else if (samerank == 10) {
      get = 100;                      /* ファイブカード */
    }
    else if (sequence != 4 && sequence2 == 4 && flush == true) {
      get = 500;                      /* ロイヤルストレートフラッシュ */
    }
    else {
      get = 0;
    }
    return get;
  }

  /* 結果の出力。ここと analyse() の終わり近くとで、数値が合っていないと */
  /* いけないのが、めんどうな、というか、うまく書けていないところ。      */
  string printresult(int get) {
    if (get == 1) {
      return "ワンペアです.  ";
    }
    else if (get == 2) {
      return "ツーペアです.  ";
    }
    else if (get == 3) {
      return "スリーカードです.  ";
    }
    else if (get == 4) {
      return "ストレートです!  ";
    }
    else if (get == 5) {
      return "フラッシュです!  ";
    }
    else if (get == 10) {
      return "フルハウスです!  ";
    }
    else if (get == 20) {
      return "フォーカードです!!  ";
    }
    else if (get == 50) {
      return "ストレートフラッシュです!!!  ";
    }
    else if (get == 100) {
      return "ファイブカードです!!!!  ";
    }
    else if (get == 500) {
      return "ロイヤルストレートフラッシュです!!!!!  ";
    }
    else return "エラー";
  }
}


