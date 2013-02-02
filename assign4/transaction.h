/*
 * transaction.h
 *
 * Manages receive and request transactions
 * 
 * Authors: Michael Banks and Dan Willoughby
 *
 *
 */
#ifndef TRANSACTION_HEADER_IKL
#define TRANSACTION_HEADER_IKL
#include <string>
using namespace std;

class transaction
{
public:
  transaction(string trans, string da_date);
  ~transaction();

  enum transaction_type
  {
    request,
     receive
  };
  int get_type();
  string get_upc_code();
  string get_date();
  int get_quantity();
  int get_shelf_life();
  void set_shelf_life(int t_s_l);
  void dec_shelf_life();
private:
  int type_of_transaction;
  string upc_code; // upc code of food item
  string the_date; // the date the transaction was made
  int quantity; // how much of the food item was in the transaction
  int shelf_life; // the shelf life of the food item in the transaction

};
#endif
