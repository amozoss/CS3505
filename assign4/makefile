#all: a.out
#	a.out #g++ -c easy_date.o food_item.o headquarters.o transaction.o warehouse.o

a.out: easy_date.o food_item.o headquarters.o transaction.o warehouse.o
	g++ -c easy_date.cc food_item.cc headquarters.cc headquarters.h transaction.o warehouse.h warehouse.cc




headquarters: easy_date food_item warehouse headquarters.o
	g++ -c headquarters.cc easy_date.cc food_item.cc warehouse.cc warehouse.h

easy_date: easy_date.o
	g++ -c easy_date.cc

food_item: food_item.o
	g++ -c food_item.cc

warehouse: warehouse.o
	g++ -c warehouse.cc warehouse.h

