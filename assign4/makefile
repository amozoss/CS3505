all: program 
	g++ easy_date.o food_item.o headquarters.o transaction.o warehouse.o transaction_simulation_program.o

#a.out: transation_simulation_program.o easy_date.o food_item.o headquarters.o transaction.o warehouse.o
	#g++ -c easy_date.cc easy_date.h food_item.cc headquarters.cc headquarters.h transaction.cc warehouse.h warehouse.cc transaction_simulation_program.cc
#	g++ -c transation_simulation_program.cc
#a.out: program



headquarters: easy_date food_item transaction warehouse headquarters.o
	g++ -c headquarters.cc 

easy_date: easy_date.o
	g++ -c easy_date.cc
transaction: transaction.o
	g++ -c transaction.cc

food_item: food_item.o
	g++ -c food_item.cc

warehouse: warehouse.o
	g++ -c warehouse.cc warehouse.h
program: headquarters
	g++ -c transaction_simulation_program.cc

