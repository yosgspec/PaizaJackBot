"use strict";
const rl=require("readline").createInterface(process.stdin,process.output);
const bet=1;

const g=function*(res){
	var myCard=(yield rl.once("line",res)).split(/ /).map(parseInt);

	if(myCard[0]==0){
		console.log(bet); // 賭けチップ数
	}
	else{
		if(myCard.reduce((a,b)=>a+b)<14){ //★カードを引く条件の合計値を変えてみよう！★
			console.log("HIT");// カード引く
		}
		else{
			console.log("STAND");// 勝負
		}
	}
	rl.close();
}(v=>g.next(v));
g.next();
