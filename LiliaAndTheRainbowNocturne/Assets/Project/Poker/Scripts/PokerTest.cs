using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PokerSystem;
using System.Text;

public class PokerTest : MonoBehaviour {
	InputField inputField;

	[SerializeField]
	GameObject PlayerHands;

	[SerializeField]
	List<Image> playerHandsImage = new List<Image>();

	[SerializeField]
	List<Image> enemyHandsImage = new List<Image>();

	[SerializeField]
	InputField debugInput;

	[SerializeField]
	Text infoText;

	[SerializeField]
	GameObject uiText;
	Text playerCoinText;
	Text playerStackText;
	Text enemyCoinText;
	Text enemyStackText;
	Text tableCoinText;


	//define
	const int EMPTY = -1;
	const int JOKER = 52;
	const int PLAYER = 0;
	const int ENEMY = 1;

	bool[] card = new bool[53];   /* ジョーカーを使うので 53 枚 */
	int[,] hand = new int[2, 6];    /* 手札。hand[0] は使わない */
	bool[,] a = new bool[2, 6];         /* hand[i] を新たにひくなら a[i] には true を、そうでなければ */
										/* false を入れる。a[0] は使わない */
	int[] coin = new int[2]; //コイン
	int[] stackMoney = new int[2];

	int tableCoin = 0;
	int bettingPrice = 10;
	int bettingReturn;
	enum bettingReturnNum {
		fold,
		call,
		check,
		other
	}

	int clickedBtnCode;

	// Start is called before the first frame update
	void Start() {
		inputField = GetComponent<InputField>();

		playerCoinText = uiText.transform.Find("PlayerCoinText").GetComponent<Text>();
		playerStackText = uiText.transform.Find("PlayerStackText").GetComponent<Text>();
		enemyCoinText = uiText.transform.Find("EnemyCoinText").GetComponent<Text>();
		enemyStackText = uiText.transform.Find("EnemyStackText").GetComponent<Text>();
		tableCoinText= uiText.transform.Find("TableCoinText").GetComponent<Text>();

		coin[PLAYER] = 0;
		coin[ENEMY] = 0;
		IncreaseCoin(250, PLAYER);
		IncreaseCoin(250, ENEMY);

		//int count = 0;
		//foreach (Transform child in PlayerHands.transform) {
		//  playerHandsImage.Add(child.GetComponent<Image>());
		//  count++;
		//}
	}

	// Update is called once per frame
	void Update() {

	}

	IEnumerator Game() {
		infoText.text = "";


		initcard(); //山札初期化

		Ante(10); //アンティ

		infoText.text += "掛け:" + tableCoin + " ,自分:" + coin[PLAYER] + " ,敵:" + coin[ENEMY];

		//手札を引く
		for (int i = 1; i <= 5; i++) {      /* 最初は五枚ひくので全部 YES */
			a[PLAYER, i] = true;
			a[ENEMY, i] = true;
		}
		draw(PLAYER);
		draw(ENEMY);
		infoText.text += "プレイヤー:";
		printhands(PLAYER);
		infoText.text += printresult(analyse0(PLAYER));        /* 役を画面表示 */
		infoText.text += "\n敵:";
		printhands(ENEMY);
		infoText.text += printresult(analyse0(ENEMY));        /* 役を画面表示 */

		AppManager.Instance.viewMessage("1st ベッティングラウンド");
		bettingReturn = -1;
		StartCoroutine(Betting(PLAYER));
		while (bettingReturn == -1) {
			yield return null;
		}
		tableCoin += stackMoney[PLAYER] + stackMoney[ENEMY];
		tableCoinText.text = tableCoin.ToString();
		IncreaseStack(-stackMoney[0], 0);   //ゼロ初期化
		IncreaseStack(-stackMoney[1], 1);   //ゼロ初期化

		AppManager.Instance.viewMessage("交換");


		AppManager.Instance.viewMessage("2nd ベッティングラウンド");


		AppManager.Instance.viewMessage("ショーダウン");

		infoText.text += "掛け:" + tableCoin + " ,自分:" + coin[PLAYER] + " ,敵:" + coin[ENEMY];
		AppManager.Instance.viewMessage("終了");

	}

	IEnumerator Betting(int dealer) {
		int noDealer = InversionRole(dealer);
		stackMoney[0] = 0;
		stackMoney[1] = 0;
		bettingPrice = 10;

		//チェック　ベット100　フォールド
		Debug.Log("プレイヤー: f,b,c");
		infoText.text += "\n" + GetRoleName(dealer) + "：フォルドorベットorチェック\n";
		clickedBtnCode = 0;
		yield return new WaitUntil(() => clickedBtnCode == 1);  //待ち
		clickedBtnCode = 0;
		if (debugInput.text == "f") {
			AppManager.Instance.viewMessage(GetRoleName(dealer)+"が降りた " + GetRoleName(noDealer)+"+"+tableCoin);
			Fold(dealer, tableCoin);
		}
		else if (debugInput.text == "b") {
			IncreaseCoin(-bettingPrice, dealer);
			IncreaseStack(bettingPrice, dealer);
			AppManager.Instance.viewMessage("ベット " + GetRoleName(dealer) + "-" + bettingPrice);

			//相手コールorレイズorフォールド
			infoText.text += "\n"+GetRoleName(noDealer)+"：コールorレイズorフォルド\n";
			clickedBtnCode = 0;
			yield return new WaitUntil(() => clickedBtnCode == 1);  //待ち
			clickedBtnCode = 0;
			if (debugInput.text == "c") {
				//コール
				int call = stackMoney[dealer] - stackMoney[noDealer];
				AppManager.Instance.viewMessage("コール "+GetRoleName(noDealer)+"-" + call);
				IncreaseCoin(-call, noDealer);
				IncreaseStack(call, noDealer);
				bettingReturn = (int)bettingReturnNum.call;
				yield break;
			}
			else if (debugInput.text == "r") {
				//レイズ
				bettingPrice += 10;
				AppManager.Instance.viewMessage("レイズ " + GetRoleName(noDealer) + "-" + bettingPrice);
				IncreaseCoin(-bettingPrice, noDealer);
				IncreaseStack(bettingPrice, noDealer);
				//賭けループへ↓
			}
			else if (debugInput.text == "f") {
				//フォールド
				tableCoin += stackMoney[PLAYER] + stackMoney[ENEMY];
				tableCoinText.text = tableCoin.ToString();
				AppManager.Instance.viewMessage(GetRoleName(noDealer) + "が降りた " + GetRoleName(dealer) + "+" + tableCoin);
				Fold(noDealer, tableCoin);
				bettingReturn = (int)bettingReturnNum.fold;
				yield break;
			}
		}
		else if (debugInput.text == "c") {
			//チェック
			AppManager.Instance.viewMessage("チェック");
			infoText.text += "チェック\n";
			infoText.text += GetRoleName(noDealer) + "：フォルドorベットorチェック\n";
			clickedBtnCode = 0;
			yield return new WaitUntil(() => clickedBtnCode == 1);  //待ち
			clickedBtnCode = 0;
			if (debugInput.text == "f") {
				//フォールド
				tableCoin += stackMoney[PLAYER] + stackMoney[ENEMY];
				tableCoinText.text = tableCoin.ToString();
				AppManager.Instance.viewMessage(GetRoleName(noDealer) + "が降りた " + GetRoleName(dealer) + "+" + tableCoin);
				Fold(noDealer, tableCoin);
				bettingReturn = (int)bettingReturnNum.fold;
				yield break;
			}
			else if (debugInput.text == "b") {
				//ベット
				IncreaseCoin(-bettingPrice, noDealer);
				IncreaseStack(bettingPrice, noDealer);
				AppManager.Instance.viewMessage("ベット " + GetRoleName(noDealer) + "-" + bettingPrice);
				//賭けループへ↓
			}
			else if (debugInput.text=="c") {
				//チェック
				bettingReturn = (int)bettingReturnNum.check;
				yield break;
			}
		}
		int better = dealer;
		//賭けループ
		while (stackMoney[ENEMY]!=stackMoney[PLAYER]) {
			infoText.text += "\n" + GetRoleName(better) + "：コールorレイズorフォルド\n";
			clickedBtnCode = 0;
			yield return new WaitUntil(() => clickedBtnCode == 1);  //待ち
			clickedBtnCode = 0;
			if (debugInput.text == "c") {
				//コール
				int call=stackMoney[InversionRole(better)]- stackMoney[better];
				AppManager.Instance.viewMessage("コール " + GetRoleName(better) + "-" + call);
				IncreaseCoin(-call, better);
				IncreaseStack(call, better);
				bettingReturn = (int)bettingReturnNum.call;
				yield break;
			}
			else if (debugInput.text == "r") {
				//レイズ
				bettingPrice += 20;
				AppManager.Instance.viewMessage("レイズ " + GetRoleName(better) + "-" + bettingPrice);
				IncreaseCoin(-bettingPrice, better);
				IncreaseStack(bettingPrice, better);
				//賭けループへ↓
			}
			else if (debugInput.text == "f") {
				//フォールド
				tableCoin += stackMoney[PLAYER] + stackMoney[ENEMY];
				tableCoinText.text = tableCoin.ToString();
				AppManager.Instance.viewMessage(GetRoleName(better) + "が降りた " + GetRoleName(dealer) + "+" + tableCoin);
				Fold(better, tableCoin);
				bettingReturn = (int)bettingReturnNum.fold;
				yield break;
			}
			better = InversionRole(better);
		}
		AppManager.Instance.viewMessage("不正な終了");
		bettingReturn = 500;
	}

	//アンティ
	void Ante(int fee) {
		IncreaseCoin(-fee, PLAYER);
		IncreaseCoin(-fee, ENEMY);
		tableCoin = fee * 2;
		tableCoinText.text = tableCoin.ToString();
	}

	//フォルド
	void Fold(int loser, int tableCoin) {
		if (loser == PLAYER) {
			IncreaseCoin(tableCoin, ENEMY);
		}
		else if (loser == ENEMY) {
			IncreaseCoin(tableCoin, PLAYER);
		}
		IncreaseStack(-stackMoney[PLAYER], PLAYER);
		IncreaseStack(-stackMoney[ENEMY], ENEMY);
		tableCoinText.text = "0";
	}

	int InversionRole(int role) {
		if (role == ENEMY) return PLAYER;
		else return ENEMY;
	}
	string GetRoleName(int role) {
		if (role == PLAYER) return "プレイヤー";
		else return "敵";
	}

	/// <summary>
	/// コインの変化+UI更新
	/// </summary>
	/// <param name="increase">増加量(減らす場合負)</param>
	/// <param name="role">PLAYERorENEMY</param>
	void IncreaseCoin(int increase, int role) {
		int newCoin = coin[role] + increase;
		coin[role] = newCoin;
		if (role == PLAYER) playerCoinText.text = "プレイヤー：" + newCoin.ToString();
		else enemyCoinText.text = "敵：" + newCoin.ToString();
	}

	/// <summary>
	/// スタックコインの変化+UI更新
	/// </summary>
	/// <param name="increase">増加量(減らす場合負)</param>
	/// <param name="role">PLAYERorENEMY</param>
	void IncreaseStack(int increase, int role) {
		int newCoin = stackMoney[role] + increase;
		stackMoney[role] = newCoin;
		if (role == PLAYER) playerStackText.text = newCoin.ToString();
		else enemyStackText.text = newCoin.ToString();
	}

	public void onDrawBtn() {
		StartCoroutine(Game());
	}

	public void onDebugBtn() {
		clickedBtnCode = 1;
	}

	public void InputLogger() {
		int i, x, y;
		int coins, bet, get;
		int COINS = 100;

		//for (coins = COINS; coins > 0;) {
		/*
		printf("--------------------\n");   //新しいゲームの始まりを示す線 
		for (bet = -1; bet < 0;) {
		  printf("%d 枚のコインを持っています.\n", coins);
		  printf("何枚賭けますか? （0 を入力すれば終了）＞");
		  scanf("%d", &bet);
		  if (bet < 0) {
			printf("負の枚数は賭けられません.\n");
		  }
		  if (bet > coins) {
			printf("そんなに持っていませんよ.\n");
			bet = -1;                   //これでやり直すことになる 
		  }
		}*/
		//if (bet == 0) {
		//coins = 0;                      /* これでプログラムの終了へ */
		//}
		// else {
		initcard();                     /* カードの初期化 */

		for (i = 1; i <= 5; i++) {      /* 最初は五枚ひくので全部 YES */
			a[0, i] = true;
			a[1, i] = true;
		}
		draw(0);                         /* カードをひく */

		/* 次の五行はデバッグの跡。復活させればインチキも可能！ */
		/*hand[1] = 0;
		hand[2] = 13;
		hand[3] = 26;
		hand[4] = 39;
		hand[5] = JOKER;*/

		printhands(0);                   /* 手札を画面表示 */
		Debug.Log(printresult(analyse0(0)));        /* 役を画面表示 */

		Debug.Log("どれを交換しますか？ --- 例：1, 2, 5 枚目");
		Debug.Log("なら 125 と入力。一枚も変えないなら 0 と入力\n");
		x = int.Parse(inputField.text);

		/* ユーザが x に入力した値を分析し、i 枚目を交換するなら a[i] に */
		/* YES が、交換しないなら NO がはいるようにする。次とその次の    */
		/* ループでそうなるんだけど、どうしてかは自分で考えて。          */
		for (i = 1; i <= 5; i++) {
			a[0, i] = false;
		}
		for (; x != 0; x = x / 10) {
			y = x % 10;
			if (y >= 1 && y <= 5) {
				a[0, y] = true;
			}
		}
		draw(0);                         /* カードを交換する（実際はひく）*/
		printhands(0);                   /* 手札を画面表示 */

		get = analyse0(0);               /* 手札を分析。倍率が返ってくる */
		Debug.Log(printresult(get));               /* 結果を出力 */

		if (get > 0) {
			Debug.Log("あたり");
			// Debug.Log((get * bet) + "枚のコインが当たりました.\n");
		}
		else {
			Debug.Log("残念でした");
		}
		// coins = coins + (get - 1) * bet;
		//if (coins <= 0) {
		// printf("あなたは破産しました....\n");
		// }
		//}
		// }

		InitInputField();
	}
	void InitInputField() {

		// 値をリセット
		inputField.text = "";

		// フォーカス
		inputField.ActivateInputField();
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
	public void draw(int pNum) {
		int i, n;

		for (i = 1; i <= 5; i++) {
			if (a[pNum, i] == true) {
				for (hand[pNum, i] = EMPTY; hand[pNum, i] == EMPTY;) {
					n = Random.Range(0, 52);            /* これが選ばれたカード。       */
					if (card[n] == true) {       /* 「残っている」だったら       */
						hand[pNum, i] = n;            /* そのカードを hand[i] に代入、*/
						card[n] = false;           /* 「残っていない」に変える     */
					}
				}
			}
		}
	}
	/* 手札の出力。非常にシンプルにしてある。ここだけを変えることも可能。hand[i] */
	/* の値は i 番目のカードを意味する。値とカードの関係は冒頭のコメントを参照。 */
	string printhands(int p) {
		int i, y;
		string print = "";

		for (i = 1; i <= 5; i++) {  /* カードの数（ランク）を表示 */
			if (hand[p, i] == JOKER) {
				print += " ? ";          /* ジョーカーはランクの位置に「?」を書く */
			}
			else {
				y = hand[p, i] % 13;       /* これがカードの数（ランク）を示す */
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
			if (hand[p, i] == JOKER) {     /* ジョーカーはスートの位置も「?」を書く */
				print += " ? ";
			}
			else {
				y = hand[p, i] / 13;       /* これがカードのマーク（スート）を示す */
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

		for (int j = 0; j < 5; j++) {
			if (p == PLAYER) {
				playerHandsImage[j].sprite = Resources.Load<Sprite>("images/trump/" + hand[p, j + 1].ToString());
				Debug.Log("images/trump/" + hand[p, j + 1]);
			}
			else if (p == ENEMY) {
				enemyHandsImage[j].sprite = Resources.Load<Sprite>("images/trump/" + hand[p, j + 1].ToString());
				Debug.Log("images/trump/" + hand[p, j + 1]);
			}
		}

		return print;
	}


	/* 手札を解析し、倍率を返す。具体的には、ジョーカーがあれば、それを 52 枚の  */
	/* 普通のカードで順に置き換えつつ analyse() を呼んで、返ってきた倍率のうちで */
	/* 最高のものを返す。ジョーカーがなければ、analyse() を呼んで、その返して    */
	/* きた値（倍率）を返すだけ。                                                */
	int analyse0(int p) {
		int i, j, x, get = 0;
		bool joker;

		joker = false;
		for (i = 1; i <= 5; i++) {
			if (hand[p, i] == JOKER) {
				joker = true;
				get = 0;
				for (j = 0; j < 52; j++) {
					hand[p, i] = j;
					x = analyse(p);
					if (x > get) {
						get = x;
					}
				}
				hand[p, i] = JOKER;    /* 置き換えたのをジョーカーに戻しておく */
			}
		}
		if (joker == true) {
			return get;
		}
		else {
			return analyse(p);
		}
	}



	/* 手札の役の解析を行ない、役に応じた倍率を返す。ジョーカーは手札に含まれて  */
	/* いないと仮定しているが、ジョーカーを普通のカードで置き換えた場合も扱うの  */
	/* で、同じカードが二枚ある場合にも対応している！                            */
	int analyse(int p) {
		int i, j, x, y;
		int samerank, sequence, sequence2, get;
		bool flush;

		/* 数（ランク）が一致する組（ペア）の数を数える。この数でワンペア、 */
		/* ツーペア、スリーカード、フルハウスなどが判定できるのは有名な事実 */
		samerank = 0;
		for (i = 1; i <= 5; i++) {
			for (j = i + 1; j <= 5; j++) {
				if (hand[p, i] % 13 == hand[p, j] % 13) {
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
					x = hand[p, i] % 13 - hand[p, j] % 13;
					if (x == 1 || x == -1) {
						sequence++;
					}
				}
			}
			/* ここで sequence が 4 ならストレート（10-J-Q-K-A 以外）*/

			for (i = 1; i <= 5; i++) {
				for (j = i + 1; j <= 5; j++) {
					if (hand[p, i] % 13 == 0) {    /* A は 13 と置き換えてから */
						x = 13;
					}
					else {
						x = hand[p, i] % 13;
					}
					if (hand[p, j] % 13 == 0) {
						y = 13;
					}
					else {
						y = hand[p, j] % 13;
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
			if (hand[p, i] / 13 != hand[p, i + 1] / 13) {
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
		else return "ノーペアです";
	}

}