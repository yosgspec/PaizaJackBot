"use strict";
const rl=require("readline").createInterface(process.stdin);

const Player=(()=>{
	const cardsAce=Symbol();
	const aceCount=Symbol();

return class Player{
	constructor(cardStr){
		this.cards=cardStr.split(/ /).map(v=>parseInt(v,10));
		this.isBet=this.cards[0]==0;
		if(this.isBet) return;
		this[aceCount]=this.cards.filter(v=>v==1).length;
	}

	hitCard(){
		return this.cards.shift();
	}

	addCard(card){
		this.cards.push(card);
		if(card==1) this[aceCount]++;
	}

	get total(){
		var cardsSum=this.cards.reduce((a,b)=>a+b);
		for(let i=this[aceCount];0<i;i--){
			let aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}
};})();

const betList=[0,50,100,200,400,800,2000];
const level=6;
const test=false;

const main=function*(res){
	var bet=test? 1: betList[level];
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
		let maxBet=parseInt(yield rl.once("line",res));
		let cpu=new Player(yield rl.once("line",res));
		if(level<6){
			isHit=pl.total<=cpu.total && cpu.total<=21
			   || pl.total<17 && cpu.total<17;
		}
		else{
			let deck=new Player(yield rl.once("line",res));
			if(cpu.total<17) cpu.addCard(deck.hitCard());
			isHit=pl.total+deck.cards[0]<=21;
		}
	}

	console.log(isHit?"HIT":"STAND");
	rl.close();
}(v=>main.next(v));
main.next();
