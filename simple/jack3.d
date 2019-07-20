import std.stdio;
import std.conv;
import std.string;
import std.array;
import std.algorithm;
import std.algorithm.iteration;

int bet=200;
int getSum(int[] card){
	auto cardAceSum=card.map!(v=>v==1?11:v).sum;
	return cardAceSum<=21?
		cardAceSum:
		card.sum;
}

void main(){
	auto getCard=()=>readln.strip.split(" ").map!(to!int).array;
	auto myCard=getCard();
	auto turn=readln.strip.to!int;
	auto combo=readln.strip.to!int;

	if(myCard[0]==0){
		writeln(bet); // 賭けチップ数
	}
	else{
		auto maxBet=readln.strip.to!int;
		auto cpuCard=getCard();

		if(getSum(myCard)<=getSum(cpuCard) || getSum(myCard)<14){ // ★カードを引く条件の合計値を変えてみよう！★
			writeln("HIT"); // カード引く
		}
		else{
			writeln("STAND"); // 勝負
		}
	}
}
