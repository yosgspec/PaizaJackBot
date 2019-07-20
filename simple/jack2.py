bet=100

myCard=list(map(int,input().split(" ")))
turn=int(input())
combo=int(input())

if myCard[0]==0:
	print(bet) # 賭けチップ数
else:
	if sum(myCard) < 14: # ★カードを引く条件の合計値を変えてみよう！★
		print("HIT") # カード引く
	else:
		print("STAND") # 勝負
