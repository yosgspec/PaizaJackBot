import std.stdio;
import std.conv;
import std.string;
import std.algorithm;
import std.algorithm.iteration;

immutable bet=50;

void main(){
	auto myCard=readln.strip.split(" ").map!(to!int);

	if(myCard[0]==0){
		writeln(bet); // 賭けチップ数
	}
	else{
		if(myCard.sum()<14){ // ★カードを引く条件の合計値を変えてみよう！★
			writeln("HIT"); // カード引く
		}
		else{
			writeln("STAND"); // 勝負
		}
	}
}
