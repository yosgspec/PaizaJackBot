class Player:
	def __init__(self,cardStr):
		self.cards=list(map(int,cardStr.split(" ")))
		self.isBet=self.cards[0]==0
		if self.isBet: return
		self.__cardsToAce=[11 if v==1 else v for v in self.cards]

	@property
	def total(self):
		cardsAceTotal=sum(self.__cardsToAce)
		return (
			cardsAceTotal if cardsAceTotal<=21 else
			sum(self.cards))

betList=[0,50,100,200,400,800]
level=5

def main():
	bet=betList[level]
	pl=Player(input())
	if pl.isBet:
		print(bet)
		return

	if 2<=level:
		turn=int(input())
		combo=int(input())

	if level<3:
		isHit=pl.total<14
	else:
		maxBet=int(input());
		cpu=Player(input());
		isHit=(pl.total<=cpu.total and cpu.total<=17 
		    or pl.total<17 and cpu.total<17)

	print("HIT" if isHit else "STAND")

if __name__=="__main__": main()
