import std.stdio;
import std.conv;
import std.string;
import std.array;
import std.algorithm;
import std.algorithm.iteration;

class Player{
	int[] cards;
	bool isBet;
	int[] cardsAce;
	private int aceCount;

	this(string cardStr){
		cards=cardStr.strip.split(" ").map!(to!int).array;
		isBet=cards[0]==0;
		if(isBet) return;
		aceCount=cards.filter!(v=>v==1).array.length.to!int;
	}

	int hitCard(){
		auto card=cards[0];
		cards=cards[1..$];
		return card;
	}

	void addCard(int card){
		cards~=card;
		if(card==1) aceCount++;
	}

	@property total(){
		auto cardsSum=cards.sum();
		for(auto i=aceCount;0<i;i--){
			auto aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}
}

immutable betList=[0,50,100,200,400,800,2000];
immutable level=6;
immutable test=false;

void main(){
	auto bet=test? 1: betList[level];
	auto pl=new Player(readln.strip);
	if(pl.isBet){
		writeln(bet);
		return;
	}

	int turn,combo;
	if(2<=level){
		turn=readln.strip.to!int;
		combo=readln.strip.to!int;
	}

	int isHit;
	if(level<3){
		isHit=pl.total<14;
	}
	else{
		auto maxBet=readln.strip.to!int;
		auto cpu=new Player(readln.strip);
		if(level<6){
			isHit=pl.total<=cpu.total && cpu.total<=21
			   || pl.total<17 && cpu.total<17;
		}
		else{
			auto deck=new Player(readln.strip);
			if(cpu.total<17) cpu.addCard(deck.hitCard());
			isHit=pl.total+deck.cards[0]<=21;

		}
	}

	writeln(isHit?"HIT":"STAND");
}
