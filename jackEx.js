"use strict";
const rl=require("readline").createInterface(process.stdin);

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
const level=9;
//level=8の時、獲得チップを上乗せするコンボ上限
const maxCombo=6;
//level=9の時のBET
const finalBet=99990000;
//level毎のBET一覧
const betList=[0,50,100,200,400,800,2000,2000,10000,finalBet];
//テスト時はBET=1とする
const test=false;

const main=function*(res){
	//BET
	var bet=test? 1: betList[level];
	//プレイヤーの手札
	var pl=new Cards(yield rl.once("line",res));
	var turn,combo;
	//level=2以降
	if(2<=level){
		//現在ターン数
		turn=parseInt(yield rl.once("line",res));
		//現在コンボ(連勝)数
		combo=parseInt(yield rl.once("line",res));
	}

	//【BETステップ】
	if(pl.isBet){
		//level=8の時は獲得チップを上乗せ
		if(8==level){
			//前ターンの獲得チップ数
			let lastPrize=parseInt(yield rl.once("line",res));
			//指定したコンボ数まで獲得チップでBETする
			if(0<combo && combo<maxCombo) bet=lastPrize;
		}
		//BETを出力して終了
		console.log(bet);
		return rl.close();
	}

	//【ドローステップ】
	//手札を引くかどうか
	var isHit=false;
	//level=0-2の時
	if(level<3){
		//手札の合計が17未満でカードを引く
		isHit=pl.total<17;
	}
	//level=3以上の時
	else{
		//最大BET
		let maxBet=parseInt(yield rl.once("line",res));
		//ディーラー(CPU)の手札
		let cpu=new Cards(yield rl.once("line",res));
		//ディーラーの手札が判明している場合の「基本判定条件」
		/* 下記のどちらかの時にカードを引く
			* プレイヤーの合計がディーラーの合計よりも少なく、ディーラーの合計が21以下の時
			* プレイヤーとディーラー双方の合計が17未満の時　*/
		let basicHit=
			pl.total<=cpu.total && cpu.total<=21
			|| pl.total<17 && cpu.total<17;

		//level=3-5の時は常時「基本判定条件」でカードを引く
		if(level<6){
			isHit=basicHit;
		}
		//level=6以上の時
		else{
			//標準入力のフラグ
			let isInput=false;
			//非同期で標準入力待機
			rl.once("line",v=>{isInput=true;res(v);});
			//山札文字列
			let deckStr=yield setTimeout(()=>{
				//パイプで100ms待っても値が返らないときは標準入力イベントを破棄
				if(!isInput){
					rl.removeListener("line",res);
					res("");
				}
			},process.stdin.isTTY? 0x7FFFFFFF: 100);

			//山札文字列が空でない時
			if(deckStr!=""){
				//山札
				let deck=new Cards(deckStr);
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
				if(0<deck.cards.length){
					if(cpu.total==21
					&& 3<=deck.cards.length
					&& new Cards(`${deck.cards[0]} ${deck.cards[2]}`).total==21)
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
			else{
				//基本判定条件でカードを引く
				isHit=basicHit;
			}
		}
	}
	//"HIT"または"STAND"を出力する
	console.log(isHit?"HIT":"STAND");
	rl.close();
}(v=>main.next(v));

//カード管理クラス
const Cards=(()=>{
	//private
	const aceCount=Symbol();

return class Cards{
	//コンストラクタ
	constructor(cardLine){
		var cardsStr=cardLine.split(/ /);
		//BETターンであるかどうか
		//一枚目のカードが"0"であるときはBETターン
		this.isBet=cardsStr[0]=="0";
		if(this.isBet){
			this.chip=parseFloat(cardsStr[1]);
			return;
		}
		//カードの束
		//スペース区切りの文字列をint型のリストに変換する
		this.cards=cardsStr.map(v=>parseInt(v,10));
		//Aの数
		this[aceCount]=this.cards.filter(v=>v==1).length;
	}

	//カードを1枚排出する
	drawCard(){
		return this.cards.shift();
	}

	//カードを1枚追加する
	addCard(card){
		this.cards.push(card);
		if(card==1) this[aceCount]++;
	}

	//カードの合計を返す(21を超えなければAは11として算出)
	get total(){
		//カードの合計
		var cardsSum=this.cards.reduce((a,b)=>a+b);
		//Aの数分、10をカードの合計に加算し、21を超えなければ値を返す
		for(let i=this[aceCount];0<i;i--){
			let aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}
};})();

main.next();
