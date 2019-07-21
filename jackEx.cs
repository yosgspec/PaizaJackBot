using System;
using System.Collections.Generic;
using System.Linq;

class Player{
	public List<int> cards;
	public bool isBet;
	public List<int> cardsAce;
	int aceCount;

	public Player(string cardStr){
		cards=cardStr.Split(' ').Select(int.Parse).ToList();
		isBet=cards[0]==0;
		if(isBet) return;
		aceCount=cards.Count(v=>v==1);
	}

	public int hitCard(){
		var card=cards[0];
		cards.RemoveAt(0);
		return card;
	}

	public void addCard(int card){
		cards.Add(card);
		if(card==1) aceCount++;
	}

	public int total{get{
		var cardsSum=cards.Sum();
		for(var i=aceCount;0<i;i--){
			var aceTotal=cardsSum+10*i;
			if(aceTotal<=21) return aceTotal;
		}
		return cardsSum;
	}}
}

class Program{
	static readonly int[] betList={0,50,100,200,400,800,2000};
	const int level=6;
	const bool test=false;

	static void Main(){
		var bet=test? 1: betList[level];
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
			if(level<6){
				isHit=pl.total<=cpu.total && cpu.total<=21
				   || pl.total<17 && cpu.total<17;
			}
			else{
				var deck=new Player(Console.ReadLine());
				if(cpu.total<17) cpu.addCard(deck.hitCard());
				isHit=pl.total+deck.cards[0]<=21;
			}
		}

		Console.WriteLine(isHit?"HIT":"STAND");
	}
}
