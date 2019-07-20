using System;
using System.Collections.Generic;
using System.Linq;

class Player{
	List<int> cards;
	public bool isBet;
	public List<int> cardsAce;

	public Player(string cardStr){
		cards=cardStr.Split(' ').Select(int.Parse).ToList();
		isBet=cards[0]==0;
		if(isBet) return;
		cardsAce=cards.Select(v=>v==1?11:v).ToList();
	}

	public int total{get{
		var cardsAceTotal=cardsAce.Sum();
		return cardsAceTotal<=21?
			cardsAceTotal:
			cards.Sum();
	}}
}

class Program{
	static readonly int[] betList={0,50,100,200,400,800};
	const int level=5;
	const bool test=false;

	static void Main(){
		var bet=bet=test? 1: betList[level];
		var pl=new Player(Console.ReadLine());
		if(pl.isBet){
			Console.WriteLine(bet);
			return;
		}

		int turn,combo;
		if(2<=level){
			turn=int.Parse(Console.ReadLine());
			combo=int.Parse(Console.ReadLine());
		}

		bool isHit;
		if(level<3){
			isHit=pl.total<14;
		}
		else{
			var maxBet=int.Parse(Console.ReadLine());
			var cpu=new Player(Console.ReadLine());
			isHit=pl.total<=cpu.total && cpu.total<22 
			   || pl.total<17 && cpu.total<17;
		}

		Console.WriteLine(isHit?"HIT":"STAND");
	}
}
