all: tester.out program.out

tester.out:  easy_date.o food_item.o headquarters.o transaction.o warehouse.o tester.o 
	g++ -g -o tester.out easy_date.o food_item.o headquarters.o transaction.o warehouse.o tester.o 

program.out: easy_date.o food_item.o headquarters.o transaction.o warehouse.o transaction_simulation_program.o
	g++ -g -o program.out easy_date.o food_item.o headquarters.o transaction.o warehouse.o transaction_simulation_program.o

headquarters.o: headquarters.cc
	g++ -g -c headquarters.cc 

easy_date.o: easy_date.cc
	g++ -g -c easy_date.cc

transaction.o: transaction.cc
	g++ -g -c transaction.cc

food_item.o: food_item.cc
	g++ -g -c food_item.cc

warehouse.o: warehouse.cc
	g++ -g -c warehouse.cc

transaction_simulation_program.o:  transaction_simulation_program.cc
	g++ -g -c transaction_simulation_program.cc

tester.o: tester.cc
	g++ -g -c tester.cc

clean: 
	rm -rf *.o *.out *.gch
