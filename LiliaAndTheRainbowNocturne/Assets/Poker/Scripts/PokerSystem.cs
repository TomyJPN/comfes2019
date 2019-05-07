//http://amonution.sblo.jp/article/57308421.html
//より引用

using System.Linq;
using System.Collections.Generic;

// カードの意味
// １バイトの上位４ビット（0xF0）をスート、下位４ビット（0x0F）をランク、として扱う
// 
// 　　　　　　　　 Ａ ２ ３ ４ ５ ６ ７ ８ ９ 10 Ｊ Ｑ Ｋ
// ♠（スペード）： 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D
// ♥（ハート）　： 21 22 23 24 25 26 27 28 29 2A 2B 2C 2D
// ♦（ダイア）　： 31 32 33 34 35 36 37 38 39 3A 3B 3C 3D
// ♣（クラブ）　： 41 42 43 44 45 46 47 48 49 4A 4B 4C 4D
//

namespace PokerSystem {

  public class PorkerSystem {

    /// <summary>
    /// 手役
    /// </summary>
    public enum PokerHand {
      RoyalStraightFlush,
      StraightFlush,
      FourOfAKind,
      FullHouse,
      Flush,
      Straight,
      ThreeOfAKind,
      TwoPair,
      OnePair,
      NoPair,
    }

    /// <summary>
    /// 手役の判断
    /// </summary>
    /// <param name="cards">手札</param>
    /// <returns>手役</returns>
    /// 

    public static PokerHand Judge(List<int> cards) {  //静的にしたらエラー消えた
      // カードが２枚以上ない場合、判定なしで不成立
      if (cards.Count() < 2) return PokerHand.NoPair;

      // ①ペア系の判定
      // 各カードとペアになるカードの枚数をカウントする。
      // 枚数に応じてペア系の役が確定する。
      int count = 0;
      for (int i = 0; i < cards.Count() - 1; i++) {
        for (int j = i + 1; j < cards.Count(); j++) {
          if ((cards[i] & 0xF) == (cards[j] & 0xF)) {
            count++;
          }
        }
      }
      // 成立した役を返す
      switch (count) {
        case 1: return PokerHand.OnePair;
        case 2: return PokerHand.TwoPair;
        case 3: return PokerHand.ThreeOfAKind;
        case 4: return PokerHand.FullHouse;
        case 6: return PokerHand.FourOfAKind;
      }

      // ストレート系・フラッシュ系はカード５枚でないと不成立
      if (cards.Count() != 5) return PokerHand.NoPair;

      // ②ストレート系の判定
      bool straight = false;
      bool royal = false;

      // ５つの数字の差が４以内であればストレート成立
      {
        int max = cards.Max(card => card & 0x0F);
        int min = cards.Min(card => card & 0x0F);
        if (max - min <= 4) {
          // ストレート成立
          straight = true;
        }
      }
      // ストレート不成立の場合はロイヤルストレートの判断を行う。
      // エース(A)を14として判定し直す
      if (straight == false) {
        int max = cards.Max(card => ((card & 0x0F) == 1) ? 14 : (card & 0x0F));
        int min = cards.Min(card => ((card & 0x0F) == 1) ? 14 : (card & 0x0F));
        if (max - min <= 4) {
          // ロイヤルストレート成立
          straight = true;
          royal = true;
        }
      }

      // ③フラッシュ系の判定
      bool flush = true;

      // ５つのスートが全て同じならフラッシュ成立
      for (int i = 1; i < cards.Count(); i++) {
        if ((cards[0] & 0xF0) != (cards[i] & 0xF0)) {
          flush = false;
          break;
        }
      }

      // 成立した役を返す
      if (royal && flush) return PokerHand.RoyalStraightFlush;
      if (straight && flush) return PokerHand.StraightFlush;
      if (flush) return PokerHand.Flush;
      if (straight) return PokerHand.Straight;

      // 何も成立しなかった
      return PokerHand.NoPair;
    }

  }
}