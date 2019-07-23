#カード管理クラス
class Cards:
	#コンストラクタ
	def __init__(self,cardStr):
		#カードの束
		#スペース区切りの文字列をint型のリストに変換する
		self.cards=list(map(int,cardStr.split(" ")))
		#BETターンであるかどうか
		#一枚目のカードが「0」であるときはBETターン
		self.isBet=self.cards[0]==0
		if self.isBet: return
		#Aの数
		self.__aceCount=sum(1 for v in self.cards if v==10)

	#カードを1枚排出する
	def hitCard(self):
		return self.cards.pop(0)

	#カードを1枚追加する
	def addCard(self,card):
		self.cards.append(card)
		if card==1: self.__aceCount+=1

	#カードの合計を返す(21を超えなければAは11として算出)
	@property
	def total(self):
		#カードの合計
		cardsSum=sum(self.cards)
		#Aの数分、10をカードの合計に加算し、21を超えなければ値を返す
		for i in range(self.__aceCount,0,-1):
			aceTotal=cardsSum+10*i
			if aceTotal<=21: return aceTotal
		return cardsSum

"""【ディーラーリスト】
Lv | 名前                  | BET    | 倍率 | 連戦 | 特徴
---|-----------------------|--------|------|------|----------------------------------------------------------------
 0 | 猫先生                |      0 |  0.0 |    1 | なし
 1 | 霧島 京子             |     50 |  2.0 |    1 | なし
 2 | 緑川 つばめ           |    100 |  2.0 |    3 | なし
 3 | 六村 リオ             |    200 |  2.0 |    3 | ディーラーカード情報が公開
 4 | 霧島 京子・バニー     |    400 |  2.0 |    5 | ディーラーカード情報が公開
 5 | 緑川 つばめ・バニー   |    800 |  2.5 |    7 | ディーラーカード情報が公開
 6 | 六村 リオ・バニー     |   2000 |  2.5 |   10 | ディーラーカードとデッキ情報が公開
 7 | 霧島 京子・赤バニー   |   2000 |  2.5 |   20 | ディーラーカードとデッキ情報が公開
 8 | 緑川 つばめ・赤バニー |  10000 |  3.0 |   20 | ディーラーカードとデッキ情報が公開・獲得チップをBETに上乗せ可
 9 | 六村 リオ・赤バニー   | 2000万 |  2.0 |    3 | ディーラーカードとデッキ情報が公開
   |                       |-9999万 |      |      |
"""

#挑戦するレベル
level=9
#level=8の時、獲得チップを上乗せするコンボ上限
maxCombo=6
#level=9の時のBET
finalBet=99990000
#level毎のBET一覧
betList=[0,50,100,200,400,800,2000,2000,10000,finalBet]
#テスト時はBET=1とする
test=False

def main():
	#BET
	bet=1 if test else betList[level]
	#プレイヤーの手札
	pl=Cards(input())
	#level=2以降
	if 2<=level:
		#現在ターン数
		turn=int(input())
		#現在コンボ(連勝)数
		combo=int(input())

	#【BETターン】
	if pl.isBet:
		#level=8の時は獲得チップを上乗せ
		if 8==level:
			#前ターンの獲得チップ数
			lastPrize=int(input())
			#指定したコンボ数まで獲得チップでBETする
			if 0<combo and combo<maxCombo: bet+=lastPrize
		#BETを出力して終了
		print(bet)
		return

	#【ゲームターン】
	#手札を引くかどうか
	isHit=False
	#level=0-2の時
	if level<3:
		#手札の合計が17未満でカードを引く
		isHit=pl.total<17
	#level=3以上の時
	else:
		#最大BET
		maxBet=int(input());
		#ディーラー(CPU)の手札
		cpu=Cards(input());
		#ディーラーの手札が判明している場合の「基本判定条件」
		""" 下記のどちらかの時にカードを引く
			* プレイヤーの合計がディーラーの合計よりも少なく、ディーラーの合計が21以下の時
			* プレイヤーとディーラー双方の合計が17未満の時　"""
		basicHit=(
			pl.total<=cpu.total and cpu.total<=22
			or pl.total<17 and cpu.total<17)

		#level=3-5の時は常時「基本判定条件」でカードを引く
		if level<6:
			isHit=basicHit
		#level=6以上の時
		else:
			#山札が空の時、例外発動
			try:
				#山札
				deck=Cards(input())
				#ディーラーのカードが17未満の時、ディーラーはカードを引く
				if cpu.total<17: cpu.addCard(deck.hitCard())
				""" 下記条件に従いカードを引く(上である程優先度高
					* 山札にカードが1枚以上残っており、
						+ ディーラーの手札の合計が21(=負け確定)かつディーラーの次の初期手札の合計が21になる時、カードを引く
						+ ディーラーの合計が17未満で次のカードを引くとディーラーがバーストする時、カードは引かない
						+ ディーラーの合計が17未満で次の次のカードを引くとディーラーがバーストする時、カードを引く
						+ プレイヤーの合計が21の時、カードは引かない
						+ カードを引いてもプレイヤーがバーストしない時、カードを引く
					* 山札にカードが残っていない時、基本判定条件でカードを引く """
				if(0<len(deck.cards)):
					if(cpu.total==21
					and 3<=len(deck.cards)
					and Cards(f"{deck.cards[0]} {deck.cards[2]}").total==21):
						isHit=True
					elif cpu.total<17 and 21<cpu.total+deck.cards[0]:
						isHit=False
					elif cpu.total<17 and 21<cpu.total+deck.cards[1] and pl.total+deck.cards[0]<=21:
						isHit=True
					elif pl.total==21:
						isHit=False
					else:
						isHit=pl.total+deck.cards[0]<=21
				else: isHit=basicHit;
			except:
				#基本判定条件でカードを引く
				isHit=(pl.total<=cpu.total and cpu.total<=22 
				    or pl.total<17 and cpu.total<17)

	#"HIT"または"STAND"を出力する
	print("HIT" if isHit else "STAND")

if __name__=="__main__": main()
