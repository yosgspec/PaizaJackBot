using System;
using System.Collections.Generic;
using System.Linq;

/*
【標準入力】
-> BETステップ
行 | Lv  | 入力
---|-----|------------------------------
 1 | 0-9 | 0 [あなたの所持チップの枚数]
 2 | 2-9 | 現在何戦目かを示す数字
 3 | 2-9 | 現在何連勝かを示す数字
 4 | 8-9 | 前回の獲得コイン

-> ドローステップ
行 | Lv  | 入力
---|-----|------------------------------
 1 | 0-9 | あなたのカード数値群(1-10)
 2 | 2-9 | 現在何戦目かを示す数字
 3 | 2-9 | 現在何連勝かを示す数字
 4 | 3-9 | 最大BETチップ数
 5 | 3-9 | ディーラーのカード数値群(1-10)
 6 | 6-9 | 山札のカード数値群(1-10)※
※山札に1枚もカードが残っていない時、入力が入らないので言語ごとにトラップする必要あり

【ディーラーリスト】
Lv | 名前                  | BET    | 倍率 | 連戦 | 特徴
---|-----------------------|--------|------|------|--------------------------------------------------------------
 0 | 猫先生                |      0 |  0.0 |    1 | なし
 1 | 霧島 京子             |     50 |  2.0 |    1 | なし
 2 | 緑川 つばめ           |    100 |  2.0 |    3 | なし
 3 | 六村 リオ             |    200 |  2.0 |    3 | ディーラーカード情報が公開
 4 | 霧島 京子・バニー     |    400 |  2.0 |    5 | ディーラーカード情報が公開
 5 | 緑川 つばめ・バニー   |    800 |  2.5 |    7 | ディーラーカード情報が公開
 6 | 六村 リオ・バニー     |   2000 |  2.5 |   10 | ディーラーカードとデッキ情報が公開
 7 | 霧島 京子・赤バニー   |   2000 |  2.5 |   20 | ディーラーカードとデッキ情報が公開
 8 | 緑川 つばめ・赤バニー |  10000 |  3.0 |   20 | ディーラーカードとデッキ情報が公開・獲得チップをBETに上乗せ可
 9 | 六村 リオ・赤バニー   | 2000万 |  2.0 |    3 | ディーラーカードとデッキ情報が公開
   |                       |-9999万 |      |      |
*/

class Program{
	//挑戦するレベル
	const int level=9;
	//level=8の時、獲得チップを上乗せするコンボ上限
	const int maxCombo=6;
	//level=9の時のBET
	const int finalBet=99990000;
	//level毎のBET一覧
	static readonly int[] betList={0,50,100,200,400,800,2000,2000,10000,finalBet};
	//テスト時はBET=1とする
	const bool test=false;

	static void Main(){
		//BET
		var bet=test? 1: betList[level];
		//プレイヤーの手札
		var pl=new Cards(Console.ReadLine());
		int turn,combo;
		//level=2以降
		if(2<=level){
			//現在ターン数
			turn=int.Parse(Console.ReadLine());
			//現在コンボ(連勝)数
			combo=int.Parse(Console.ReadLine());
		}

		//【BETステップ】
		if(pl.isBet){
			//level=8の時は獲得チップを上乗せ
			if(8==level){
				//前ターンの獲得チップ数
				var lastPrize=int.Parse(Console.ReadLine());
				//指定したコンボ数まで獲得チップでBETする
				if(0<combo && combo<maxCombo) bet=lastPrize;
			}
			//BETを出力して終了
			Console.WriteLine(bet);
			return;
		}

		//【ドローステップ】
		//手札を引くかどうか
		bool isHit=false;
		//level=0-2の時
		if(level<3){
			//手札の合計が17未満でカードを引く
			isHit=pl.total<17;
		}
		//level=3以上の時
		else{
			//最大BET
			var maxBet=int.Parse(Console.ReadLine());
			//ディーラー(CPU)の手札
			var cpu=new Cards(Console.ReadLine());
			//ディーラーの手札が判明している場合の「基本判定条件」
			/* 下記のどちらかの時にカードを引く
				* プレイヤーの合計がディーラーの合計よりも少なく、ディーラーの合計が21以下の時
				* プレイヤーとディーラー双方の合計が17未満の時　*/
			var basicHit=
				pl.total<=cpu.total && cpu.total<=21
				|| pl.total<17 && cpu.total<17;

			//level=3-5の時は常時「基本判定条件」でカードを引く
			if(level<6){
				isHit=basicHit;
			}
			//level=6以上の時
			else{
				//山札が空の時、例外発動
				try{
					//山札
					var deck=new Cards(Console.ReadLine());
					//ディーラーのカードが17未満の時、ディーラーはカードを引く
					if(cpu.total<17) cpu.addCard(deck.drawCard());
					/* 下記条件に従いカードを引く(上である程優先度高
							* 山札にカードが1枚以上残っており、
							+ ディーラーの手札の合計が21(=負け確定)かつディーラーの次の初期手札の合計が21になる時、カードを引く
							+ ディーラーの合計が17未満で次のカードを引くとディーラーがバーストする時、カードは引かない
							+ ディーラーの合計が17未満で次の次のカードを引くとディーラーがバーストする時、カードを引く
							+ プレイヤーの合計が21の時、カードは引かない
							+ カードを引いてもプレイヤーがバーストしない時、カードを引く
						* 山札にカードが残っていない時、基本判定条件でカードを引く */
					if(0<deck.cards.Count){
						if(cpu.total==21
						&& 3<=deck.cards.Count
						&& new Cards($"{deck.cards[0]} {deck.cards[2]}").total==21)
							isHit=true;
						else if(cpu.total<17 && 21<cpu.total+deck.cards[0])
							isHit=false;
						else if(cpu.total<17 && 21<cpu.total+deck.cards[1] && pl.total+deck.cards[0]<=21)
							isHit=true;
						else if(pl.total==21)
							isHit=false;
						else
							isHit=pl.total+deck.cards[0]<=21;
					}
					else isHit=basicHit;
				}
				catch{
					//基本判定条件でカードを引く
					isHit=basicHit;
				}
			}
		}
		//"HIT"または"STAND"を出力する
		Console.WriteLine(isHit?"HIT":"STAND");
	}
}

//カード管理クラス
class Cards{
	public List<int> cards;
	public bool isBet;
	public long chip;
	int aceCount;

	//コンストラクタ
	public Cards(string cardLine){
		var cardsStr=cardLine.Split(' ');
		//BETターンであるかどうか
		//一枚目のカードが"0"であるときはBETターン
		isBet=cardsStr[0]=="0";
		if(isBet){
			chip=long.Parse(cardsStr[1]);
			return;
		}
		//カードの束
		//スペース区切りの文字列をint型のリストに変換する
		cards=cardsStr.Select(int.Parse).ToList();
		//Aの数
		aceCount=cards.Count(v=>v==1);
	}

	//カードを1枚排出する
	public int drawCard(){
		var card=cards[0];
		cards.RemoveAt(0);
		return card;
	}

	//カードを1枚追加する
	public void addCard(int card){
		cards.Add(card);
		if(card==1) aceCount++;
	}

	//カードの合計を返す(21を超えなければAは11として算出)
	public int total{get{
		//カードの合計
		var cardsSum=cards.Sum();
		//Aの数分、10をカードの合計に加算し、21を超えなければ値を返す
		for(var i=aceCount;0<i;i--){
			var aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}}
}
