using System;
using System.Collections.Generic;
using System.Linq;

class Player{
	public List<int> cards;
	public bool isBet;
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
	static readonly int[] betList={0,50,100,200,400,800,2000,2000,10000};
	const int level=8;
	const bool test=false;
	const int maxCombo=5;

	static void Main(){
		var bet=test? 1: betList[level];
		var pl=new Player(Console.ReadLine());
		int turn,combo;
		if(2<=level){
			turn=int.Parse(Console.ReadLine());
			combo=int.Parse(Console.ReadLine());
		}

		if(pl.isBet){
			if(8<=level){
				var lastPrize=int.Parse(Console.ReadLine());
				if(0<combo && combo<maxCombo) bet=lastPrize;
			}
			Console.WriteLine(bet);
			return;
		}

		bool isHit=false;
		if(level<3){
			isHit=pl.total<17;
		}
		else{
			var maxBet=int.Parse(Console.ReadLine());
			var cpu=new Player(Console.ReadLine());
			var basicHit=
				pl.total<=cpu.total && cpu.total<=21
				|| pl.total<17 && cpu.total<17;

			if(level<6){
				isHit=basicHit;
			}
			else{
				try{
					var deck=new Player(Console.ReadLine());
					if(cpu.total<17) cpu.addCard(deck.hitCard());
					isHit=
						cpu.total==21 && 4<=deck.cards.Count
						&& new Player($"{deck.cards[0]} {deck.cards[2]}").total==21?
							true:
						cpu.total<17 && 21<cpu.total+deck.cards[0]?
							false:
						0<deck.cards.Count?
							pl.total+deck.cards[0]<=21:
							basicHit;
				}
				catch{
					isHit=basicHit;
				}
			}
		}

		Console.WriteLine(isHit?"HIT":"STAND");
	}
}
