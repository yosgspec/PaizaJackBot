import std.stdio;
import std.conv;
import std.string;
import std.format;
import std.array;
import std.algorithm;
import std.algorithm.iteration;

class Player{
	int[] cards;
	bool isBet;
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

immutable betList=[0,50,100,200,400,800,2000,2000,10000];
immutable level=8;
immutable test=false;
immutable maxCombo=5;

void main(){
	auto bet=test? 1: betList[level];
	auto pl=new Player(readln.strip);
	int turn,combo;
	if(2<=level){
		turn=readln.strip.to!int;
		combo=readln.strip.to!int;
	}

	if(pl.isBet){
		if(8<=level){
			auto lastPrize=readln.strip.to!int;
			if(0<combo && combo<maxCombo) bet=lastPrize;
		}
		writeln(bet);
		return;
	}

	bool isHit=false;
	if(level<3){
		isHit=pl.total<17;
	}
	else{
		auto maxBet=readln.strip.to!int;
		auto cpu=new Player(readln.strip);
		auto basicHit=
			pl.total<=cpu.total && cpu.total<=21
			|| pl.total<17 && cpu.total<17;

		if(level<6){
			isHit=basicHit;
		}
		else{
			auto deckStr=readln.strip;
			if(deckStr!=""){
				auto deck=new Player(deckStr);
				if(cpu.total<17) cpu.addCard(deck.hitCard());
				isHit=
					cpu.total==21 && 4<=deck.cards.length
					&& new Player(format!"%s %s"(deck.cards[0],deck.cards[2])).total==21?
						true:
					cpu.total<17 && 21<cpu.total+deck.cards[0]?
						false:
					0<deck.cards.length?
						pl.total+deck.cards[0]<=21:
						basicHit;
			}
			else{
				isHit=pl.total<=cpu.total && cpu.total<=21
				   || pl.total<17 && cpu.total<17;
			}
		}
	}

	writeln(isHit?"HIT":"STAND");
}
