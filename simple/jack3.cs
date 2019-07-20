using System;
using System.Collections.Generic;
using System.Linq;

class BlackJack{
	static int bet=200;
	static int getSum(List<int> card){
		var cardAceSum=card.Select(v=>v==1?11:v).Sum();
		return cardAceSum<=21?
			cardAceSum:
			card.Sum();
	}

	static void Main(){
		Func<List<int>> getCard=()=>new List<int>(Console.ReadLine().Split(' ').Select(int.Parse));
		var myCard=getCard();
		var turn=int.Parse(Console.ReadLine());
		var combo=int.Parse(Console.ReadLine());

		if(myCard[0]==0){
			Console.WriteLine(bet); // 賭けチップ数
		}
		else{
			var maxBet=int.Parse(Console.ReadLine());
			var cpuCard=getCard();

			if(getSum(myCard)<=getSum(cpuCard) || getSum(myCard)<14){ // ★カードを引く条件の合計値を変えてみよう！★
				Console.WriteLine("HIT"); // カード引く
			}
			else{
				Console.WriteLine("STAND"); // 勝負
			}
		}
	}
}
