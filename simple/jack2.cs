using System;
using System.Collections.Generic;
using System.Linq;

class BlackJack{
	const int bet=100;

	static void Main(){
		var myCard=new List<int>(Console.ReadLine().Split(' ').Select(int.Parse));
		var turn=int.Parse(Console.ReadLine());
		var combo=int.Parse(Console.ReadLine());

		if(myCard[0]==0){
			Console.WriteLine(bet); // 賭けチップ数
		}
		else{
			if(myCard.Sum()<14){ // ★カードを引く条件の合計値を変えてみよう！★
				Console.WriteLine("HIT"); // カード引く
			}
			else{
				Console.WriteLine("STAND"); // 勝負
			}
		}
	}
}
