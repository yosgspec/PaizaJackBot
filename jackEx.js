"use strict";
const rl=require("readline").createInterface(process.stdin);

const Player=(()=>{
	const cardsToAce=Symbol();

return class Player{
	constructor(cardStr){
		this.cards=cardStr.split(/ /).map(v=>parseInt(v,10));
		this.isBet=this.cards[0]==0;
		if(this.isBet) return;
		this[cardsToAce]=this.cards.map(v=>v==1?11:v);
	}

	get total(){
		var cardsAceTotal=this[cardsToAce].reduce((a,b)=>a+b);
		return cardsAceTotal<=21?
			cardsAceTotal:
			this.cards.reduce((a,b)=>a+b);
	}
};})();

const betList=[0,50,100,200,400,800];
const level=5;

const main=function*(res){
	var bet=betList[level];
	var pl=new Player(yield rl.once("line",res));
	if(pl.isBet){
		console.log(bet);
		return;
	}

	var turn,combo;
	if(2<=level){
		turn=parseInt(yield rl.once("line",res));
		combo=parseInt(yield rl.once("line",res));
	}

	var isHit;
	if(level<3){
		isHit=pl.total<14;
	}
	else{
		var maxBet=parseInt(yield rl.once("line",res));
		var cpu=new Player(yield rl.once("line",res));
		isHit=pl.total<=cpu.total && cpu.total<22 
		   || pl.total<17 && cpu.total<17;	}

	console.log(isHit?"HIT":"STAND");
	rl.close();
}(v=>main.next(v));
main.next();
