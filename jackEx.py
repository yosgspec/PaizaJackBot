class Player:
	def __init__(self,cardStr):
		self.cards=list(map(int,cardStr.split(" ")))
		self.isBet=self.cards[0]==0
		if self.isBet: return
		self.__aceCount=sum(1 for v in self.cards if v==10)

	def hitCard(self):
		return self.cards.pop(0)

	def addCard(self,card):
		self.cards.append(card)
		if card==1: self.__aceCount+=1

	@property
	def total(self):
		cardsSum=sum(self.cards)
		for i in range(self.__aceCount,0,-1):
			aceTotal=cardsSum+10*i
			if aceTotal<=21: return aceTotal
		return cardsSum

betList=[0,50,100,200,400,800,2000]
level=6
test=False

def main():
	bet=1 if test else betList[level]
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
		if level<6:
			isHit=(pl.total<=cpu.total and cpu.total<=22 
			    or pl.total<17 and cpu.total<17)
		else:
			deck=Player(input())
			if cpu.total<17: cpu.addCard(deck.hitCard())
			isHit=pl.total+deck.cards[0]<=21

	print("HIT" if isHit else "STAND")

if __name__=="__main__": main()
