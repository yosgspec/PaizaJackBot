"use strict";
const rl=require("readline").createInterface(process.stdin,process.output);

var bet=200;
function getSum(card){
	var cardAceSum=card.map(v=>v==1?11:v).reduce((a,b)=>a+b);
	return cardAceSum<=21?
		cardAceSum:
		card.reduce((a,b)=>a+b);
}

const g=function*(res){
	const getCard=res=>rl.once("line",v=>res(v.split(/ /).map(v=>parseInt(v))));
	var myCard=yield getCard(res);
	var turn=parseInt(yield rl.once("line",res));
	var combo=parseInt(yield rl.once("line",res));

	if(myCard[0]==0){
		console.log(bet); // 賭けチップ数
	}
	else{
		let maxBet=parseInt(yield rl.once("line",res));
		let cpuCard=yield getCard(res);

		if(getSum(myCard)<=getSum(cpuCard) || getSum(myCard)<14){ //★カードを引く条件の合計値を変えてみよう！★
			console.log("HIT");// カード引く
		}
		else{
			console.log("STAND");// 勝負
		}
	}
	rl.close();
}(v=>g.next(v));
g.next();
