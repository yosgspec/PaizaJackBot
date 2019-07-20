bet=200
def getSum(card):
	cardAceSum=sum([11 if v==1 else v for v in card])
	return (
		cardAceSum if cardAceSum<=21 else 
		sum(card))

getCard=lambda:list(map(int,input().split(" ")))
myCard=getCard()
turn=int(input())
combo=int(input())

if myCard[0]==0:
	print(bet) # 賭けチップ数
else:
	maxBet=int(input())
	cpuCard=getCard()

	if getSum(myCard)<=getSum(cpuCard) or getSum(myCard)<14: # ★カードを引く条件の合計値を変えてみよう！★
		print("HIT") # カード引く
	else:
		print("STAND") # 勝負
