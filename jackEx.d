import std.stdio;
import std.conv;
import std.string;
import std.array;
import std.algorithm;
import std.algorithm.iteration;

class Player{
	int[] cards;
	public bool isBet;
	public int[] cardsAce;

	public this(string cardStr){
		cards=cardStr.strip.split(" ").map!(to!int).array;
		isBet=cards[0]==0;
		if(isBet) return;
		cardsAce=cards.map!(v=>v==1?11:v).array;
	}

	public @property total(){
		auto cardsAceTotal=cardsAce.sum();
		return cardsAceTotal<=21?
			cardsAceTotal:
			cards.sum();
	}
}

immutable betList=[0,50,100,200,400,800];
immutable level=5;
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
		isHit=pl.total<=cpu.total && cpu.total<22 
		   || pl.total<17 && cpu.total<17;
	}

	writeln(isHit?"HIT":"STAND");
}
