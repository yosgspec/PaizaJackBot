import std.stdio;
import std.conv;
import std.string;
import std.format;
import std.array;
import std.algorithm;
import std.algorithm.iteration;

//カード管理クラス
class Cards{
	int[] cards;
	bool isBet;
	private int aceCount;

	//コンストラクタ
	this(string cardStr){
		//カードの束
		//スペース区切りの文字列をint型のリストに変換する
		cards=cardStr.strip.split(" ").map!(to!int).array;
		//BETターンであるかどうか
		//一枚目のカードが「0」であるときはBETターン
		isBet=cards[0]==0;
		if(isBet) return;
		//Aの数
		aceCount=cards.filter!(v=>v==1).array.length.to!int;
	}

	//カードを1枚排出する
	int hitCard(){
		auto card=cards[0];
		cards=cards[1..$];
		return card;
	}

	//カードを1枚追加する
	void addCard(int card){
		cards~=card;
		if(card==1) aceCount++;
	}

	//カードの合計を返す(21を超えなければAは11として算出)
	@property total(){
		//カードの合計
		auto cardsSum=cards.sum();
		//Aの数分、10をカードの合計に加算し、21を超えなければ値を返す
		for(auto i=aceCount;0<i;i--){
			auto aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}
}

/* 【ディーラーリスト】
Lv | 名前                  | BET    | 倍率 | 連戦 | 特徴
---|-----------------------|--------|------|------|----------------------------------------------------------------
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

//挑戦するレベル
immutable level=8;
//level=8の時、獲得チップを上乗せするコンボ上限
immutable maxCombo=6;
//level=9の時のBET
immutable finalBet=20000000;
//level毎のBET一覧
immutable betList=[0,50,100,200,400,800,2000,2000,10000,finalBet];
//テスト時はBET=1とする
immutable test=false;

void main(){
	//BET
	auto bet=test? 1: betList[level];
	//プレイヤーの手札
	auto pl=new Cards(readln.strip);
	int turn,combo;
	//level=2以降
	if(2<=level){
		//現在ターン数
		turn=readln.strip.to!int;
		//現在コンボ(連勝)数
		combo=readln.strip.to!int;
	}

	//【BETターン】
	if(pl.isBet){
		//level=8の時は獲得チップを上乗せ
		if(8==level){
			//前ターンの獲得チップ数
			auto lastPrize=readln.strip.to!int;
			//指定したコンボ数まで獲得チップでBETする
			if(0<combo && combo<maxCombo) bet=lastPrize;
		}
		//BETを出力して終了
		writeln(bet);
		return;
	}

	//【ゲームターン】
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
		auto maxBet=readln.strip.to!int;
		//ディーラー(CPU)の手札
		auto cpu=new Cards(readln.strip);
		//ディーラーの手札が判明している場合の「基本判定条件」
		/* 下記のどちらかの時にカードを引く
			* プレイヤーの合計がディーラーの合計よりも少なく、ディーラーの合計が21以下の時
			* プレイヤーとディーラー双方の合計が17未満の時　*/
		auto basicHit=
			pl.total<=cpu.total && cpu.total<=21
			|| pl.total<17 && cpu.total<17;

		//level=3-5の時は常時「基本判定条件」でカードを引く
		if(level<6){
			isHit=basicHit;
		}
		//level=6以上の時
		else{
			//山札文字列
			auto deckStr=readln.strip;

			//山札文字列が空でない時
			if(deckStr!=""){
				//山札
				auto deck=new Cards(deckStr);
				//ディーラーのカードが17未満の時、ディーラーはカードを引く
				if(cpu.total<17) cpu.addCard(deck.hitCard());
				/* 下記条件に従いカードを引く
					* ディーラーの手札の合計が21(=負け確定)かつディーラーの次の初期手札の合計が21になる時、カードを引く
					* ディーラーの合計が17未満の時、次のカードを引くとディーラーがバーストする時、カードは引かない
					* デッキの残りが1枚以上残っており、カードを引いてもプレイヤーがバーストしない時、カードを引く
					* 山札にカードが残っていない時、基本判定条件でカードを引く */
				isHit=
					cpu.total==21 && 3<=deck.cards.length
					&& new Cards(format!"%s %s"(deck.cards[0],deck.cards[2])).total==21?
						true:
					cpu.total<17 && 21<cpu.total+deck.cards[0]?
						false:
					0<deck.cards.length && pl.total+deck.cards[0]<=21?
						true:
						basicHit;
			}
			else{
				//基本判定条件でカードを引く
				isHit=basicHit;
			}
		}
	}
	//"HIT"または"STAND"を出力する
	writeln(isHit?"HIT":"STAND");
}
